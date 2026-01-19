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

# Get Chocolatey path, installing if needed
function Get-ChocoPath {
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
