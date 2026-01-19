param(
    [string]$DiskNumber = ""
)

$ErrorActionPreference = 'Stop'

$result = @{ success = $false; message = ""; disk = "" }

# Auto-elevate if not running as Admin
if (-not ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    $scriptPath = $MyInvocation.MyCommand.Path
    Start-Process -FilePath "powershell.exe" -ArgumentList "-NoProfile -ExecutionPolicy Bypass -File `"$scriptPath`" -DiskNumber $DiskNumber" -Verb RunAs -Wait
    exit
}

try {
    if (-not $DiskNumber) {
        throw "Disk number not specified"
    }

    if (-not [int]::TryParse($DiskNumber, [ref]$null)) {
        throw "Invalid disk number: $DiskNumber"
    }

    $diskNum = [int]$DiskNumber
    
    $disk = Get-Disk -Number $diskNum -ErrorAction Stop
    if (-not $disk) {
        throw "Disk $diskNum not found"
    }

    # Set disk to online and writable
    Set-Disk -Number $diskNum -IsOffline $false -ErrorAction SilentlyContinue
    Start-Sleep -Milliseconds 500

    # Clear all partitions from the disk
    Clear-Disk -Number $diskNum -RemoveData -Confirm:$false -ErrorAction Stop
    Start-Sleep -Seconds 1

    # Create new FAT32 partition using entire disk
    $partition = New-Partition -DiskNumber $diskNum -UseMaximumSize -MbrType FAT32 -IsActive -ErrorAction Stop
    Start-Sleep -Milliseconds 500

    # Find the next available drive letter (starting from D:)
    $driveLetter = $null
    for ([int]$charCode = 68; $charCode -le 90; $charCode++) {
        $letter = [char]$charCode
        if (-not (Test-Path "${letter}:")) {
            $driveLetter = $letter
            break
        }
    }

    if (-not $driveLetter) {
        throw "Could not find available drive letter"
    }

    # Assign the drive letter to the partition
    $partition | Add-PartitionAccessPath -AccessPath "${driveLetter}:" -ErrorAction Stop
    Start-Sleep -Seconds 1

    # Format the partition as FAT32
    Format-Volume -DriveLetter $driveLetter -FileSystem FAT32 -NewFileSystemLabel "USB" -Force -Confirm:$false -ErrorAction Stop
    Start-Sleep -Seconds 2

    $result.success = $true
    $result.message = "USB drive restored and formatted successfully"
    $result.disk = $diskNum.ToString()
}
catch {
    $result.success = $false
    $result.message = "Error: " + $_.Exception.Message
}

$json = ConvertTo-Json -InputObject $result -Depth 3
Write-Output $json
