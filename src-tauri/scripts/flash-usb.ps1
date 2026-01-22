param(
    [Parameter(Mandatory=$true)]
    [string]$DiskNumber,
    [Parameter(Mandatory=$true)]
    [string]$ISOPath
)

# JSON helpers
function Write-JsonOutput {
    param(
        [ValidateSet("success","error","warning","info")]
        [string]$Status,
        [string]$Message,
        [hashtable]$Data=@{}
    )
    $out = @{ status=$Status; message=$Message } + $Data
    Write-Output ($out | ConvertTo-Json -Compress)
}
function Write-Info { param($m,$d=@{}) Write-JsonOutput -Status info -Message $m -Data $d }
function Write-Success { param($m,$d=@{}) Write-JsonOutput -Status success -Message $m -Data $d }
function Write-Warning-Custom { param($m,$d=@{}) Write-JsonOutput -Status warning -Message $m -Data $d }
function Write-Error-Custom { param($m,$d=@{}) Write-JsonOutput -Status error -Message $m -Data $d }

# Chocolatey check/installer
function Get-ChocoPath {
    Write-Info "Checking Chocolatey installation"
    $chocoCmd = Get-Command choco -ErrorAction SilentlyContinue
    $choco = if ($chocoCmd) { $chocoCmd.Source } else { $null }
    if (-not $choco) {
        $fallback = Join-Path $env:ProgramData "chocolatey\bin\choco.exe"
        if (Test-Path $fallback) { $choco = $fallback }
    }
    if (-not $choco) {
        Write-Warning-Custom "Chocolatey not found, installing..."
        try {
            [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
            $tmp = Join-Path $env:TEMP "chocoInstall"
            New-Item -ItemType Directory -Force -Path $tmp | Out-Null
            $installScript = Join-Path $tmp "install.ps1"
            Invoke-WebRequest "https://community.chocolatey.org/install.ps1" -OutFile $installScript -UseBasicParsing
            Start-Process -FilePath powershell.exe -ArgumentList "-NoProfile -ExecutionPolicy Bypass -File `"$installScript`"" -Wait -NoNewWindow
            $chocoCmd = Get-Command choco -ErrorAction SilentlyContinue
            $choco = if ($chocoCmd) { $chocoCmd.Source } else { $null }
            if ($choco) { Write-Success "Chocolatey installed" @{ path=$choco } }
            else { Write-Error-Custom "Chocolatey installation failed"; return $null }
        } catch { Write-Error-Custom "Chocolatey install error" @{ error=$_ }; return $null }
    } else { Write-Success "Chocolatey detected" @{ path=$choco } }
    return $choco
}

try {
    # Validate inputs
    if (-not [int]::TryParse($DiskNumber,[ref]$null)) { throw "Invalid disk number: $DiskNumber" }
    $diskNum = [int]$DiskNumber
    if (-not $ISOPath) { throw "ISO path not provided" }
    if (-not (Test-Path $ISOPath)) { 
        $expandedPath = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($ISOPath)
        throw "ISO not found at path: $expandedPath (provided: $ISOPath)"
    }
    $ISOAbs = (Resolve-Path $ISOPath).Path
    Write-Info "ISO validated" @{ path=$ISOAbs; sizeGB=([math]::Round((Get-Item $ISOAbs).Length/1GB,2)) }

    $disk = Get-Disk -Number $diskNum -ErrorAction Stop
    Write-Info "Disk detected" @{ disk=$diskNum; sizeGB=([math]::Round($disk.Size/1GB,2)) }

    # Ensure dd via Chocolatey
    $choco = Get-ChocoPath
    if (-not $choco) { throw "Cannot proceed without Chocolatey" }

    $ddInstalled = (& $choco list --local-only --exact dd | Out-String)
    if ($ddInstalled -notmatch 'dd') {
        Write-Info "Installing dd via Chocolatey"
        & $choco install dd -y | Out-Null
        if ($LASTEXITCODE -ne 0) { throw "dd install failed" }
        Write-Success "dd installed"
    } else { Write-Success "dd already installed" }

    # Offline disk + clean partitions
    Write-Info "Offlining and cleaning disk..."
    $dpFile = Join-Path $env:TEMP "diskpart_$([guid]::NewGuid()).txt"
    @("select disk $diskNum","clean all","offline disk noerr","exit") | Out-File $dpFile -Encoding ASCII
    $diskpartOut = & diskpart.exe /s $dpFile 2>&1
    Remove-Item $dpFile -Force
    Write-Info "Disk prepared" @{ output=($diskpartOut -join "`n") }
    Start-Sleep -Seconds 5

    # Locate dd
    $ddPath = (Get-Command dd -ErrorAction Stop).Source
    Write-Info "dd located" @{ path=$ddPath }

    # Flash ISO with dd and stream output
    Write-Info "Flashing ISO to USB..."
    $procInfo = New-Object System.Diagnostics.ProcessStartInfo
    $procInfo.FileName = $ddPath
    $procInfo.Arguments = "if=`"$ISOAbs`" of=\\.\PhysicalDrive$diskNum bs=4M"
    $procInfo.RedirectStandardOutput = $true
    $procInfo.RedirectStandardError = $true
    $procInfo.UseShellExecute = $false
    $procInfo.CreateNoWindow = $true

    $proc = New-Object System.Diagnostics.Process
    $proc.StartInfo = $procInfo
    $proc.Start() | Out-Null

    while (-not $proc.HasExited) {
        while (-not $proc.StandardOutput.EndOfStream) { Write-Info ($proc.StandardOutput.ReadLine()) }
        Start-Sleep -Milliseconds 200
    }
    while (-not $proc.StandardOutput.EndOfStream) { Write-Info ($proc.StandardOutput.ReadLine()) }
    while (-not $proc.StandardError.EndOfStream) { Write-Warning-Custom ($proc.StandardError.ReadLine()) }

    $exitCode = $proc.ExitCode
    if ($exitCode -eq 0) {
        Write-Success "USB flashing completed" @{ disk=$diskNum; ISO=$ISOAbs }
        exit 0
    } else {
        Write-Error-Custom "dd exited with error" @{ exitCode=$exitCode }
        exit $exitCode
    }

} catch {
    Write-Error-Custom "Unexpected error" @{ error=$_ }
    exit 1
}
