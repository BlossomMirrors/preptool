$ErrorActionPreference = 'SilentlyContinue'

param(
    [string]$UsbDrive = ""
)

$result = @{ success = $false; message = ""; drive = "" }

try {
    if (-not $UsbDrive) {
        throw "USB drive letter not specified"
    }

    $driveLetter = $UsbDrive.TrimEnd(':')
    $diskPath = "${driveLetter}:"
    
    if (-not (Test-Path $diskPath)) {
        throw "USB drive $driveLetter not found"
    }

    $partition = Get-Partition -ErrorAction Stop | Where-Object { $_.DriveLetter -eq $driveLetter }
    if (-not $partition) {
        throw "Could not find partition for drive $driveLetter"
    }
    
    $diskNumber = $partition.DiskNumber
    Clear-Disk -Number $diskNumber -RemoveData -Confirm:$false -ErrorAction Stop 2>$null | Out-Null
    $newPartition = New-Partition -DiskNumber $diskNumber -UseMaximumSize -FileSystem FAT32 -ErrorAction Stop 2>$null | Out-Null
    Start-Sleep -Seconds 2
    $result.success = $true
    $result.message = "USB drive restored successfully"
    $result.drive = $driveLetter
}
catch {
    $result.success = $false
    $result.message = "Error: $_"
}

$json = ConvertTo-Json -InputObject $result -Depth 3
Write-Output $json
