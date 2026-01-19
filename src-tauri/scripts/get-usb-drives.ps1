$ErrorActionPreference = 'SilentlyContinue'

$usbDrives = @()
$maxSizeBytes = 130 * 1GB

try {
    # Get all disks and their partitions
    $disks = Get-Disk -ErrorAction SilentlyContinue
    
    foreach ($disk in $disks) {
        # Skip system disks (those with volumes containing Windows)
        $isSystemDisk = $false
        $partitions = $disk | Get-Partition -ErrorAction SilentlyContinue
        
        if ($partitions) {
            foreach ($partition in $partitions) {
                $volume = $partition | Get-Volume -ErrorAction SilentlyContinue
                if ($volume -and $volume.DriveLetter) {
                    $driveLetter = $volume.DriveLetter
                    $systemPath = "${driveLetter}:\Windows\System32"
                    if (Test-Path $systemPath) {
                        $isSystemDisk = $true
                        break
                    }
                }
            }
        }
        
        # Skip if system disk or too large
        if ($isSystemDisk) {
            continue
        }
        
        $totalSize = $disk.Size
        if ($totalSize -and $totalSize -lt $maxSizeBytes) {
            $sizeGB = [math]::Round($totalSize / 1GB, 2)
            
            # Get volume names from partitions
            $volumeNames = @()
            foreach ($partition in $partitions) {
                $volume = $partition | Get-Volume -ErrorAction SilentlyContinue
                if ($volume -and $volume.FileSystemLabel) {
                    $volumeNames += $volume.FileSystemLabel
                }
            }
            
            $displayName = if ($volumeNames.Count -gt 0) { 
                $volumeNames -join " / " 
            } else { 
                "USB Drive" 
            }
            
            $usbDrives += @{
                name = $displayName
                size = "$sizeGB GB"
                diskNumber = $disk.Number
            }
        }
    }
}
catch {}

if ($usbDrives.Count -eq 0) {
    Write-Output "[]"
} else {
    $json = ConvertTo-Json -InputObject $usbDrives -Depth 3
    Write-Output $json
}
