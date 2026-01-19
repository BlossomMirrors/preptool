param(
	[Parameter(Mandatory=$true)]
	[string]$DiskNumber,
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
	Start-Process -FilePath "powershell.exe" -ArgumentList "-NoProfile -ExecutionPolicy Bypass -File `"$scriptPath`" -DiskNumber $DiskNumber -ISOPath `"$ISOPath`"" -Verb RunAs -Wait
	exit
}

# Inline Chocolatey installer to avoid module import issues
function Get-ChocoPath {
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
	# Validate disk number
	if (-not [int]::TryParse($DiskNumber, [ref]$null)) {
		Write-Error-Custom "Invalid disk number" @{ provided = $DiskNumber }
		exit 1
	}
	$diskNum = [int]$DiskNumber

	# Validate ISO file exists
	if (-not (Test-Path $ISOPath)) {
		Write-Error-Custom "ISO file not found" @{ path = $ISOPath }
		exit 1
	}
	$ISOAbsPath = (Resolve-Path $ISOPath).Path
	Write-Info "ISO file validated" @{ path = $ISOAbsPath; size = ((Get-Item $ISOAbsPath).Length / 1GB).ToString("F2") + " GB" }

	# Validate disk exists
	$disk = Get-Disk -Number $diskNum -ErrorAction SilentlyContinue
	if (-not $disk) {
		Write-Error-Custom "Disk not found" @{ diskNumber = $diskNum }
		exit 1
	}
	Write-Info "Disk detected" @{ diskNumber = $diskNum; size = ($disk.Size / 1GB).ToString("F2") + " GB" }

	# Ensure dd is installed via Chocolatey
	$chocoPath = Get-ChocoPath
	if (-not $chocoPath) {
		Write-Error-Custom "Cannot proceed without Chocolatey"
		exit 1
	}

	Write-Info "Checking if Win32 Disk Imager is installed"
	$imagerInstalled = & $chocoPath list --local-only --exact win32-disk-imager | Out-String
	if ($imagerInstalled -notmatch 'win32-disk-imager') {
		Write-Info "Win32 Disk Imager not available via Chocolatey, using diskpart method"
	} else {
		Write-Success "win32-disk-imager already installed"
		
		# Locate Win32 Disk Imager executable
		$imagerPath = "Win32DiskImager.exe"
		$commonPaths = @(
			"C:\Program Files\Win32DiskImager\Win32DiskImager.exe",
			"C:\Program Files (x86)\Win32DiskImager\Win32DiskImager.exe"
		)
		
		foreach ($path in $commonPaths) {
			if (Test-Path $path) {
				$imagerPath = $path
				break
			}
		}
		
		Write-Info "Win32 Disk Imager located" @{ path = $imagerPath }

		# Flash the ISO using Win32 Disk Imager
		Write-Info "Flashing ISO to USB disk..."
		$proc = Start-Process -FilePath $imagerPath -ArgumentList "-m `"$ISOAbsPath`" -d $diskNum" -NoNewWindow -PassThru -Wait
		$exitCode = $proc.ExitCode
		Write-Info "Win32 Disk Imager process completed" @{ exitCode = $exitCode }

		if ($exitCode -eq 0) {
			Write-Success "USB flashing completed successfully" @{ diskNumber = $diskNum; ISO = $ISOAbsPath }
			exit 0
		} else {
			Write-Error-Custom "Win32 Disk Imager exited with error" @{ exitCode = $exitCode }
			exit $exitCode
		}
	}

	# Ensure dd is installed via Chocolatey
	$chocoPath = Get-ChocoPath
	if (-not $chocoPath) {
		Write-Error-Custom "Cannot proceed without Chocolatey"
		exit 1
	}

	Write-Info "Checking if dd is installed"
	$ddInstalled = & $chocoPath list --local-only --exact dd | Out-String
	if ($ddInstalled -notmatch 'dd') {
		Write-Info "Installing dd via Chocolatey"
		& $chocoPath install dd -y 2>&1 | Out-Null
		$exitCode = $LASTEXITCODE
		if ($exitCode -ne 0) {
			Write-Error-Custom "Failed to install dd" @{ exitCode = $exitCode }
			exit 1
		}
		Write-Success "dd installed"
	} else {
		Write-Success "dd already installed"
	}

	# Offline the disk using diskpart so dd can write to it completely
	Write-Info "Removing all partitions and offlining disk..."
	$tempDir = $env:TEMP
	$diskpartScript = Join-Path $tempDir "offline_$([guid]::NewGuid()).txt"
	
	@(
		"select disk $diskNum",
		"clean all",
		"offline disk noerr",
		"exit"
	) | Out-File -FilePath $diskpartScript -Encoding ASCII

	$proc = Start-Process -FilePath "diskpart.exe" -ArgumentList "/s `"$diskpartScript`"" -NoNewWindow -PassThru -Wait
	Remove-Item $diskpartScript -Force -ErrorAction SilentlyContinue
	Write-Info "Disk prepared" @{ exitCode = $proc.ExitCode }
	
	# Wait for disk to be fully offline and cleaned
	Start-Sleep -Seconds 5

	# Locate dd executable
	$ddPath = "dd"
	try {
		$ddCmd = Get-Command dd -ErrorAction SilentlyContinue
		if ($null -ne $ddCmd) { $ddPath = $ddCmd.Source }
	} catch {}
	Write-Info "dd located" @{ path = $ddPath }

	# Final wait to ensure all handles are released
	Write-Info "Waiting for system to release all disk handles..."
	Start-Sleep -Seconds 5
	
	# Flash the ISO using dd (without unsupported options for Windows dd)
	Write-Info "Flashing ISO to USB disk (this may take several minutes)..."
	$isoSize = (Get-Item $ISOAbsPath).Length
	Write-Info "ISO size" @{ bytes = $isoSize; gb = ($isoSize / 1GB).ToString("F2") }
	
	# Windows dd uses different syntax - no status=progress option
	$ddArgs = "if=`"$ISOAbsPath`" of=\\.\PhysicalDrive$diskNum bs=4M"
	$proc = Start-Process -FilePath $ddPath -ArgumentList $ddArgs -NoNewWindow -PassThru -Wait
	$exitCode = $proc.ExitCode
	Write-Info "dd process completed" @{ exitCode = $exitCode }

	# Wait a moment for buffers to flush
	Start-Sleep -Seconds 2

	# Verify the operation
	if ($exitCode -eq 0) {
		Write-Info "Waiting for disk operations to complete..."
		Start-Sleep -Seconds 3
		Write-Success "USB flashing completed successfully" @{ diskNumber = $diskNum; ISO = $ISOAbsPath; sizeGB = ($isoSize / 1GB).ToString("F2") }
		exit 0
	} else {
		Write-Error-Custom "dd exited with error" @{ exitCode = $exitCode }
		exit $exitCode
	}
} catch {
	Write-Error-Custom "Unexpected error during flashing" @{ error = $_.ToString() }
	exit 1
}
