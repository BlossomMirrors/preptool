param(
    [Parameter(Mandatory=$true)]
    [int]$FreeSpaceGB,
    [bool]$AllowPartial = $true,
    [switch]$AutoOptimize
)

# JSON helpers
function Write-JsonOutput {
    param(
        [ValidateSet("success","error","warning","info")]
        [string]$Status,
        [string]$Message,
        [hashtable]$Data=@{}
    )
    $out = @{ status=$Status; message=$Message } + $Data
    Write-Output ($out | ConvertTo-Json -Compress)
}
function Write-Info { param($m,$d=@{}) Write-JsonOutput -Status info -Message $m -Data $d }
function Write-Success { param($m,$d=@{}) Write-JsonOutput -Status success -Message $m -Data $d }
function Write-Warning-Custom { param($m,$d=@{}) Write-JsonOutput -Status warning -Message $m -Data $d }
function Write-Error-Custom { param($m,$d=@{}) Write-JsonOutput -Status error -Message $m -Data $d }

Write-Info "Partition Resize Script - Windows Root Partition Manager"

$tempDiskpartFile = [System.IO.Path]::GetTempFileName()

try {
    # Get C: partition info
    $cVolume = Get-Volume -DriveLetter C -ErrorAction Stop
    $cPartition = Get-Partition -DriveLetter C -ErrorAction Stop
    $cDiskNumber = $cPartition.DiskNumber
    $cPartitionNumber = $cPartition.PartitionNumber
    $cSizeGB = [math]::Round($cVolume.Size/1GB,2)
    $cFreeGB = [math]::Round($cVolume.SizeRemaining/1GB,2)
    $usedGB = [math]::Round(($cVolume.Size - $cVolume.SizeRemaining)/1GB,2)

    Write-Info "C: drive info" @{ sizeGB=$cSizeGB; freeGB=$cFreeGB; usedGB=$usedGB; disk=$cDiskNumber; partition=$cPartitionNumber }

    # Validations
    if ($FreeSpaceGB -le 0) { throw "Free space must be > 0GB" }
    
    # Calculate what the C: partition size would be after shrinking
    $maxAllowedShrink = $cSizeGB - 20  # Keep at least 20GB for Windows
    
    if ($FreeSpaceGB -gt $maxAllowedShrink) { 
        if ($AllowPartial) { 
            Write-Warning-Custom "Requested partition exceeds available space, using maximum available" @{ requestedGB=$FreeSpaceGB; maxAvailableGB=$maxAllowedShrink; cDriveSizeGB=$cSizeGB }
            $FreeSpaceGB = $maxAllowedShrink
        } else { 
            throw "Requested partition size ($FreeSpaceGB GB) exceeds maximum available ($maxAllowedShrink GB) - must keep at least 20GB for Windows"
        }
    }
    
    $targetGB = [math]::Round($cSizeGB - $FreeSpaceGB,2)
    if ($targetGB -lt 20) { throw "Resulting partition would be below 20GB minimum" }

    Write-Info "Shrink parameters calculated" @{ freeSpaceGB=$FreeSpaceGB; targetPartitionGB=$targetGB }

    # Optional AutoOptimize: hibernation off + defrag
    if ($AutoOptimize.IsPresent -and $FreeSpaceGB -gt ($cFreeGB-2)) {
        Write-Info "Auto-optimizing..."
        try { powercfg /h off | Out-Null; Write-Success "Hibernation disabled" } catch { Write-Warning-Custom "Failed to disable hibernation: $_" }
        try { Start-Process -FilePath "defrag.exe" -ArgumentList "C: /X" -Wait -NoNewWindow; Write-Success "Defrag completed" } catch { Write-Warning-Custom "Defrag failed: $_" }
    }

    # Query maximum shrinkable space
    $queryScript = @"
select disk $cDiskNumber
select partition $cPartitionNumber
shrink querymax
"@
    $queryScript | Out-File $tempDiskpartFile -Encoding ASCII -Force
    $queryResult = diskpart /s $tempDiskpartFile 2>&1 | Out-String
    Write-Info "Shrink query result" @{ output=$queryResult }

    $maxMB = ($queryResult | Select-String -Pattern ":\s+(\d+)\s+MB" -AllMatches | ForEach-Object { $_.Matches[-1].Groups[1].Value })[0]
    $maxShrinkGB = [math]::Round($maxMB/1024,2)
    Write-Info "Maximum shrinkable" @{ maxShrinkGB=$maxShrinkGB; maxMB=$maxMB }

    $shrinkMB = [int]($FreeSpaceGB*1024)
    if ($shrinkMB -gt $maxMB) { 
        if ($AllowPartial) { 
            Write-Warning-Custom "Requested shrink > max, using max available" @{ requestedMB=$shrinkMB; maxMB=$maxMB }
            $shrinkMB = $maxMB
        } else { throw "Requested shrink > max and partial not allowed" }
    }

    $resultingGB = [math]::Round($cSizeGB - ($shrinkMB/1024),2)
    if ($resultingGB -lt 20) { throw "Resulting partition < 20GB" }

    # Execute shrink
    $shrinkScript = @"
select disk $cDiskNumber
select partition $cPartitionNumber
shrink desired=$shrinkMB
"@
    $shrinkScript | Out-File $tempDiskpartFile -Encoding ASCII -Force
    $diskpartOut = diskpart /s $tempDiskpartFile 2>&1 | Out-String
    Write-Info "Diskpart output" @{ output=$diskpartOut }

    $success = ($diskpartOut -match "freigegeben|freed|successfully") -and ($diskpartOut -notmatch "Fehler|error|failed")
    if ($success) {
        Write-Success "Partition shrink completed" @{
            requestedShrinkGB=$FreeSpaceGB
            actualShrinkGB=[math]::Round($shrinkMB/1024,2)
            resultingPartitionGB=$resultingGB
            diskNumber=$cDiskNumber
            partitionNumber=$cPartitionNumber
        }
    } else {
        Write-Error-Custom "Partition shrink failed" @{ output=$diskpartOut }
        exit 1
    }
}
catch {
    Write-Error-Custom "Unexpected error" @{ error=$_ }
    exit 1
}
finally {
    if (Test-Path $tempDiskpartFile) { Remove-Item $tempDiskpartFile -Force -ErrorAction SilentlyContinue }
}

Write-Success "Script execution completed"
