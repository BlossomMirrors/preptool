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
	Write-Info "Setting hardware clock to UTC"
	$regPath = "HKLM:\\SYSTEM\\CurrentControlSet\\Control\\TimeZoneInformation"
	New-Item -Path $regPath -Force | Out-Null
	Set-ItemProperty -Path $regPath -Name "RealTimeIsUniversal" -Type DWord -Value 1
	$value = (Get-ItemProperty -Path $regPath -Name "RealTimeIsUniversal" -ErrorAction SilentlyContinue).RealTimeIsUniversal
	Write-Success "UTC hardware clock enabled" @{ registryPath = $regPath; value = $value }

	# Optional status of Windows Time service
	$svc = Get-Service -Name W32Time -ErrorAction SilentlyContinue
	if ($null -ne $svc) { Write-Info "Windows Time Service" @{ serviceStatus = $svc.Status.ToString(); startType = $svc.StartType.ToString() } }

	exit 0
} catch {
	Write-Error-Custom "Failed to set UTC hardware clock" @{ error = $_.ToString() }
	exit 1
}
