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

try {
	Write-Info "Disabling Fast Startup"
	$regPath = "HKLM:\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Power"
	New-Item -Path $regPath -Force | Out-Null
	Set-ItemProperty -Path $regPath -Name "HiberbootEnabled" -Type DWord -Value 0
	Write-Success "Fast Startup disabled" @{ registryPath = $regPath; value = 0 }

	Write-Info "Disabling hibernation (optional but recommended)"
	$powercfg = & powercfg /h off 2>&1 | Out-String
	Write-Info "powercfg result" @{ output = $powercfg }
	Write-Success "Hibernation disabled"
	exit 0
} catch {
	Write-Error-Custom "Failed to disable Fast Startup" @{ error = $_.ToString() }
	exit 1
}

