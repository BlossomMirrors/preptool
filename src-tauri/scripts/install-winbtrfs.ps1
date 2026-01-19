param()

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
	Start-Process -FilePath "powershell.exe" -ArgumentList "-NoProfile -ExecutionPolicy Bypass -File `"$scriptPath`"" -Verb RunAs -Wait
	exit
}

Write-Info "Checking Chocolatey installation"

# Detect Chocolatey
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
		# Download the Chocolatey installer to a temp file and execute with -File
		[Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12
		$tempDir = Join-Path $env:TEMP "chocoInstallLocal"
		New-Item -ItemType Directory -Force -Path $tempDir | Out-Null
		$scriptFile = Join-Path $tempDir "install.ps1"
		Invoke-WebRequest -UseBasicParsing -Uri "https://community.chocolatey.org/install.ps1" -OutFile $scriptFile

		$argList = "-NoProfile -ExecutionPolicy Bypass -File `"$scriptFile`""
		$proc = Start-Process -FilePath "powershell.exe" -ArgumentList $argList -Wait -NoNewWindow -PassThru
		Write-Info "Chocolatey bootstrap process finished" @{ exitCode = $proc.ExitCode }

		# Give PATH update a moment and verify installation
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
			exit 1
		}
	} catch {
		Write-Error-Custom "Chocolatey installation error" @{ error = $_.ToString() }
		exit 1
	}
} else {
	Write-Success "Chocolatey detected" @{ path = $chocoPath }
}

# Check if winbtrfs is already installed
Write-Info "Checking winbtrfs installation"
$installed = & $chocoPath list --local-only --exact winbtrfs | Out-String
if ($installed -match '^winbtrfs') {
	Write-Success "winbtrfs already installed"
	exit 0
}

# Install winbtrfs
Write-Info "Installing winbtrfs via Chocolatey"
$result = & $chocoPath install winbtrfs -y 2>&1 | Out-String
$exitCode = $LASTEXITCODE
Write-Info "Chocolatey install result" @{ exitCode = $exitCode; output = $result }
if ($exitCode -eq 0) {
	Write-Success "winbtrfs installation completed"
	exit 0
} else {
	Write-Error-Custom "winbtrfs installation failed" @{ exitCode = $exitCode; output = $result }
	exit $exitCode
}

