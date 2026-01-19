$ErrorActionPreference = 'SilentlyContinue'

$usbDrives = @()

try {
    $wmiDisks = Get-WmiObject Win32_LogicalDisk -ErrorAction SilentlyContinue | Where-Object {
        $_.DriveType -eq 2
    }

    foreach ($disk in $wmiDisks) {
        $driveLetter = $disk.Name
        $volumeName = $disk.VolumeName
        $size = $disk.Size
        
        if ($size -and $driveLetter) {
            $sizeGB = [math]::Round($size / 1GB, 2)
            $displayName = if ($volumeName) { "$volumeName" } else { "USB Drive" }
            
            $usbDrives += @{
                name = $displayName
                size = "$sizeGB GB"
                letter = $driveLetter.TrimEnd(':')
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
