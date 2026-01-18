param(
    [Parameter(Mandatory=$false)]
    [string]$Message = "Hello from PowerShell!",

    [Parameter(Mandatory=$false)]
    [string]$Name = "User"
)

function Write-ColorOutput {
    param(
        [string]$Text,
        [string]$Color = "Green"
    )
    Write-Host $Text -ForegroundColor $Color
}

try {
    Write-ColorOutput "=== PowerShell Script Execution ===" "Cyan"
    Write-ColorOutput "Message: $Message" "Green"
    Write-ColorOutput "Name: $Name" "Green"

    $computerInfo = @{
        ComputerName = $env:COMPUTERNAME
        UserName = $env:USERNAME
        OSVersion = [System.Environment]::OSVersion.VersionString
        PowerShellVersion = $PSVersionTable.PSVersion.ToString()
    }

    Write-ColorOutput "`nSystem Information:" "Yellow"
    $computerInfo.GetEnumerator() | ForEach-Object {
        Write-Host "  $($_.Key): $($_.Value)"
    }

    $result = @{
        success = $true
        message = "Script executed successfully"
        data = $computerInfo
        timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    }

    $result | ConvertTo-Json -Compress

} catch {
    $errorResult = @{
        success = $false
        error = $_.Exception.Message
        timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    }

    $errorResult | ConvertTo-Json -Compress
    exit 1
}
