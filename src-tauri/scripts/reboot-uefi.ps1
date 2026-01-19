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
	Write-Info "Preparing reboot to firmware (UEFI)"

	# Check if UEFI is supported (best-effort)
	$uefi = $false
	try {
		$sb = Confirm-SecureBootUEFI -ErrorAction Stop
		$uefi = $true
	} catch {
		# If cmdlet not available or throws on non-UEFI, keep $uefi as $false
	}
	Write-Info "UEFI support check" @{ uefi = $uefi }

	# Attempt reboot to firmware; if not supported, perform normal reboot
	$cmd = "shutdown.exe"
	$args = "/r /fw /t 0"
	$proc = Start-Process -FilePath $cmd -ArgumentList $args -PassThru -Wait
	Write-Info "shutdown invoked" @{ exitCode = $proc.ExitCode; args = $args }
	Write-Success "Reboot initiated to firmware (if supported)"
	exit 0
} catch {
	Write-Error-Custom "Failed to initiate firmware reboot" @{ error = $_.ToString() }
	Write-Info "Falling back to normal reboot"
	try {
		Start-Process -FilePath "shutdown.exe" -ArgumentList "/r /t 0" -Wait
		Write-Success "Normal reboot initiated"
		exit 0
	} catch {
		Write-Error-Custom "Fallback reboot failed" @{ error = $_.ToString() }
		exit 1
	}
}

