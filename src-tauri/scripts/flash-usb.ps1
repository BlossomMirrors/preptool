param(
	[Parameter(Mandatory=$true)]
	[string]$DriveLetter,
	[Parameter(Mandatory=$true)]
	[string]$ISOPath
)

# JSON output helpers
function Write-JsonOutput {
	param(
		[Parameter(Mandatory=$true)]
		[ValidateSet("success","error","warning","info")]
		[string]$Status,
		[Parameter(Mandatory=$true)]
		[string]$Message,
		[hashtable]$Data = @{}
	)
	$output = @{ status = $Status; message = $Message } + $Data
	$output | ConvertTo-Json -Compress | Write-Host
}
function Write-Info { param([string]$Message,[hashtable]$Data=@{}) Write-JsonOutput -Status info -Message $Message -Data $Data }
function Write-Success { param([string]$Message,[hashtable]$Data=@{}) Write-JsonOutput -Status success -Message $Message -Data $Data }
function Write-Warning-Custom { param([string]$Message,[hashtable]$Data=@{}) Write-JsonOutput -Status warning -Message $Message -Data $Data }
function Write-Error-Custom { param([string]$Message,[hashtable]$Data=@{}) Write-JsonOutput -Status error -Message $Message -Data $Data }

# Auto-elevate if not running as Admin
if (-not ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
	Write-Info "Elevating privileges..."
	$scriptPath = $MyInvocation.MyCommand.Path
	Start-Process -FilePath "powershell.exe" -ArgumentList "-NoProfile -ExecutionPolicy Bypass -File `"$scriptPath`" -DriveLetter $DriveLetter -ISOPath `"$ISOPath`"" -Verb RunAs -Wait
	exit
}

# Inline Chocolatey installer to avoid module import issues
function Ensure-Chocolatey {
	Write-Info "Checking Chocolatey installation"

	$chocoPath = $null
	try {
		$chocoCmd = Get-Command choco -ErrorAction SilentlyContinue
		if ($null -ne $chocoCmd) { $chocoPath = $chocoCmd.Source }
	} catch {}
	if (-not $chocoPath) {
		$fallbackPath = Join-Path $env:ProgramData "chocolatey\bin\choco.exe"
		if (Test-Path $fallbackPath) { $chocoPath = $fallbackPath }
	}

	if (-not $chocoPath) {
		Write-Warning-Custom "Chocolatey not found; installing..."
		try {
			[Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12
			$tempDir = Join-Path $env:TEMP "chocoInstallLocal"
			New-Item -ItemType Directory -Force -Path $tempDir | Out-Null
			$scriptFile = Join-Path $tempDir "install.ps1"
			Invoke-WebRequest -UseBasicParsing -Uri "https://community.chocolatey.org/install.ps1" -OutFile $scriptFile

			$argList = "-NoProfile -ExecutionPolicy Bypass -File `"$scriptFile`""
			$proc = Start-Process -FilePath "powershell.exe" -ArgumentList $argList -Wait -NoNewWindow -PassThru
			Write-Info "Chocolatey bootstrap process finished" @{ exitCode = $proc.ExitCode }

			Start-Sleep -Seconds 2
			$chocoCmd = Get-Command choco -ErrorAction SilentlyContinue
			if ($null -ne $chocoCmd) { $chocoPath = $chocoCmd.Source } else {
				$fallbackPath = Join-Path $env:ProgramData "chocolatey\bin\choco.exe"
				if (Test-Path $fallbackPath) { $chocoPath = $fallbackPath }
			}
			if ($chocoPath) {
				Write-Success "Chocolatey installed" @{ path = $chocoPath }
			} else {
				Write-Error-Custom "Chocolatey installation appears to have failed"
				return $null
			}
		} catch {
			Write-Error-Custom "Chocolatey installation error" @{ error = $_.ToString() }
			return $null
		}
	} else {
		Write-Success "Chocolatey detected" @{ path = $chocoPath }
	}

	return $chocoPath
}

try {
	# Normalize drive letter (remove colon if present)
	$drive = $DriveLetter.TrimEnd(':').ToUpper()
	if ($drive.Length -ne 1 -or -not [char]::IsLetter($drive)) {
		Write-Error-Custom "Invalid drive letter" @{ provided = $DriveLetter }
		exit 1
	}

	# Validate ISO file exists
	if (-not (Test-Path $ISOPath)) {
		Write-Error-Custom "ISO file not found" @{ path = $ISOPath }
		exit 1
	}
	$ISOAbsPath = (Resolve-Path $ISOPath).Path
	Write-Info "ISO file validated" @{ path = $ISOAbsPath; size = ((Get-Item $ISOAbsPath).Length / 1GB).ToString("F2") + " GB" }

	# Validate drive exists
	$drivePath = "$drive`:"
	if (-not (Test-Path $drivePath)) {
		Write-Error-Custom "Drive not found" @{ drive = $drivePath }
		exit 1
	}
	Write-Info "Drive detected" @{ drive = $drivePath }

	# Ensure dd is installed via Chocolatey
	$chocoPath = Ensure-Chocolatey
	if (-not $chocoPath) {
		Write-Error-Custom "Cannot proceed without Chocolatey"
		exit 1
	}

	Write-Info "Checking if dd is installed"
	$ddInstalled = & $chocoPath list --local-only --exact gnuwin32-coreutils | Out-String
	if ($ddInstalled -notmatch 'gnuwin32-coreutils') {
		Write-Info "Installing dd via Chocolatey"
		$result = & $chocoPath install gnuwin32-coreutils -y 2>&1 | Out-String
		$exitCode = $LASTEXITCODE
		if ($exitCode -ne 0) {
			Write-Error-Custom "Failed to install dd" @{ exitCode = $exitCode }
			exit 1
		}
		Write-Success "dd installed"
	} else {
		Write-Success "dd already installed"
	}

	# Locate dd executable
	$ddPath = "dd"
	try {
		$ddCmd = Get-Command dd -ErrorAction SilentlyContinue
		if ($null -ne $ddCmd) { $ddPath = $ddCmd.Source }
	} catch {}
	Write-Info "dd located" @{ path = $ddPath }

	# Flash the ISO using dd
	Write-Info "Flashing ISO to USB..."
	$ddArgs = "if=`"$ISOAbsPath`" of=\\.\$drive`: bs=4M status=progress"
	$proc = Start-Process -FilePath $ddPath -ArgumentList $ddArgs -NoNewWindow -PassThru -Wait
	$exitCode = $proc.ExitCode
	Write-Info "dd process completed" @{ exitCode = $exitCode }

	if ($exitCode -eq 0) {
		Write-Success "USB flashing completed successfully" @{ drive = $drivePath; ISO = $ISOAbsPath }
		exit 0
	} else {
		Write-Error-Custom "dd exited with error" @{ exitCode = $exitCode }
		exit $exitCode
	}
} catch {
	Write-Error-Custom "Unexpected error during flashing" @{ error = $_.ToString() }
	exit 1
}
