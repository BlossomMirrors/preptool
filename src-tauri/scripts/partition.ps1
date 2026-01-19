<#
.SYNOPSIS
    Resizes the Windows root partition (C:) by creating a specified amount of free space.
    
.DESCRIPTION
    This script shrinks the Windows root partition to create a desired amount of unallocated
    space while ensuring space constraints are met. It validates that at least 20GB remains
    allocated to the partition after shrinking.
    
.PARAMETER FreeSpaceGB
    The desired amount of free/unallocated space to create in GB. The partition will be
    shrunk to accommodate this space.
    
.EXAMPLE
    .\partition.ps1 -FreeSpaceGB 129
    
.NOTES
    Requires Administrator privileges.
    Creates a temporary diskpart script file.
    Performs validation before executing diskpart commands.
#>

param(
    [Parameter(Mandatory=$true)]
    [int]$FreeSpaceGB,
    [bool]$AllowPartial = $true,
    [switch]$AutoOptimize
)

# Output functions for JSON
function Write-JsonOutput {
    param(
        [Parameter(Mandatory=$true)]
        [ValidateSet("success", "error", "warning", "info")]
        [string]$Status,
        [Parameter(Mandatory=$true)]
        [string]$Message,
        [hashtable]$Data = @{}
    )
    
    $output = @{
        status = $Status
        message = $Message
    } + $Data
    
    $output | ConvertTo-Json -Compress | Write-Host
}

function Write-Success {
    param([string]$Message, [hashtable]$Data = @{})
    Write-JsonOutput -Status "success" -Message $Message -Data $Data
}

function Write-Error-Custom {
    param([string]$Message, [hashtable]$Data = @{})
    Write-JsonOutput -Status "error" -Message $Message -Data $Data
}

function Write-Warning-Custom {
    param([string]$Message, [hashtable]$Data = @{})
    Write-JsonOutput -Status "warning" -Message $Message -Data $Data
}

function Write-Info {
    param([string]$Message, [hashtable]$Data = @{})
    Write-JsonOutput -Status "info" -Message $Message -Data $Data
}

# Validate and auto-elevate Administrator privileges
if (-not ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    Write-Info "Elevating privileges..."
    
    # Rebuild the command line arguments
    $args_str = if ($FreeSpaceGB) { "-FreeSpaceGB $FreeSpaceGB" } else { "" }
    
    # Re-run the script with elevated privileges
    $scriptPath = $MyInvocation.MyCommand.Path
    Start-Process -FilePath "powershell.exe" -ArgumentList "-NoProfile -ExecutionPolicy Bypass -File `"$scriptPath`" $args_str" -Verb RunAs -Wait
    exit
}

Write-Info "Partition Resize Script - Windows Root Partition Manager"

# Get current partition information
Write-Info "Retrieving current partition information..."

$diskpartQuery = @"
list disk
select disk 0
list partition
"@

# Create temporary file for diskpart commands
$tempDiskpartFile = [System.IO.Path]::GetTempFileName()

try {
    # Execute diskpart to get current partition info
    $diskpartQuery | Out-File -FilePath $tempDiskpartFile -Encoding ASCII -Force
    $diskpartOutput = diskpart /s $tempDiskpartFile 2>&1
    
    Write-Info "Raw diskpart output" @{
        output = ($diskpartOutput | Out-String)
    }
    
    # First, get the actual C: drive partition using Windows API
    $cDrive = Get-Volume -DriveLetter C -ErrorAction SilentlyContinue
    if ($null -eq $cDrive) {
        Write-Error-Custom "Could not find C: drive"
        exit 1
    }
    
    $cDriveSizeGB = [math]::Round($cDrive.Size / 1GB, 2)
    $cDrivePartitionNumber = 0
    
    # Try to find the partition number through partition information
    try {
        # Get disk and partition info using Get-Partition
        $partitions = Get-Partition -ErrorAction SilentlyContinue | Where-Object { $_.DriveLetter -eq 'C' }
        if ($partitions) {
            $cDrivePartitionNumber = $partitions[0].PartitionNumber
            $cDriveDiskNumber = $partitions[0].DiskNumber
        }
    }
    catch {
        # Fallback: try to parse from diskpart output more robustly
        Write-Info "Using diskpart parsing fallback..."
    }
    
    if ($cDrivePartitionNumber -eq 0) {
        Write-Error-Custom "Could not determine C: drive partition number"
        exit 1
    }
    
    $cDrivePartition = @{
        Number = $cDrivePartitionNumber
        DiskNumber = $cDriveDiskNumber
        SizeGB = $cDriveSizeGB
    }
    
    $partitionsList = @{
        number = $cDrivePartition.Number
        sizeGB = $cDrivePartition.SizeGB
        diskNumber = $cDrivePartition.DiskNumber
    }
    
    Write-Info "C: drive partition identified" @{ partition = $partitionsList }
    
    # Validation checks
    Write-Info "Validating resize parameters..."
    
    # Calculate target partition size from desired free space
    $targetPartitionSizeGB = [math]::Round($cDrivePartition.SizeGB - $FreeSpaceGB, 2)
    
    # Check 1: Free space must be positive
    if ($FreeSpaceGB -le 0) {
        Write-Error-Custom "Free space must be greater than 0 GB" @{
            specified = $FreeSpaceGB
        }
        exit 1
    }
    
    Write-Success "Free space parameter is valid" @{
        freeSpaceGB = $FreeSpaceGB
    }
    
    # Check 2: Resulting partition must be at least 20GB
    if ($targetPartitionSizeGB -lt 20) {
        Write-Error-Custom "Resulting partition size would be below minimum (20GB)" @{
            desiredFreeSpaceGB = $FreeSpaceGB
            resultingPartitionGB = $targetPartitionSizeGB
            minimumPartitionGB = 20
        }
        exit 1
    }
    
    Write-Success "Target partition size is valid" @{
        freeSpaceGB = $FreeSpaceGB
        resultingPartitionGB = $targetPartitionSizeGB
        minimumPartitionGB = 20
    }
    
    # Check 3: Free space cannot exceed current partition size
    if ($FreeSpaceGB -gt $cDrivePartition.SizeGB) {
        Write-Error-Custom "Desired free space exceeds current partition size" @{
            desiredFreeSpaceGB = $FreeSpaceGB
            currentPartitionGB = $cDrivePartition.SizeGB
        }
        exit 1
    }
    
    $shrinkAmount = $FreeSpaceGB
    Write-Success "Partition shrink parameters calculated" @{
        shrinkAmountGB = $shrinkAmount
        currentSizeGB = $cDrivePartition.SizeGB
        targetPartitionSizeGB = $targetPartitionSizeGB
        diskNumber = $cDrivePartition.DiskNumber
    }
    
    # Check 4: Ensure sufficient free space on the partition for the shrink operation
    $cDriveVolume = Get-Volume -DriveLetter C -ErrorAction SilentlyContinue
    if ($null -ne $cDriveVolume) {
        $currentFreeSpaceGB = [math]::Round($cDriveVolume.SizeRemaining / 1GB, 2)
        $usedSpaceGB = [math]::Round(($cDriveVolume.Size - $cDriveVolume.SizeRemaining) / 1GB, 2)
        $totalGB = [math]::Round($cDriveVolume.Size / 1GB, 2)
        
        Write-Info "C: Drive usage retrieved" @{
            usedGB = $usedSpaceGB
            currentFreeGB = $currentFreeSpaceGB
            totalGB = $totalGB
        }
        
        # Ensure we have at least 2GB free space for the shrink operation
        if ($currentFreeSpaceGB -lt 2) {
            Write-Error-Custom "Insufficient free space for shrink operation" @{
                required = 2
                available = $currentFreeSpaceGB
            }
            exit 1
        }
        
        Write-Success "Sufficient free space available" @{
            freeGB = $currentFreeSpaceGB
            requiredGB = 2
        }
    }
    
    # Display summary
    Write-Info "Resize operation ready" @{
        currentSizeGB = $cDrivePartition.SizeGB
        freeSpaceToCreateGB = $FreeSpaceGB
        resultingPartitionSizeGB = $targetPartitionSizeGB
        partitionNumber = $cDrivePartition.Number
    }
    Write-Warning-Custom "This operation cannot be undone without additional recovery steps"

    # Query maximum shrinkable space
    Write-Info "Querying maximum shrinkable size..."
    $queryScript = @"
select disk $($cDrivePartition.DiskNumber)
select partition $($cDrivePartition.Number)
shrink querymax
"@
    $queryScript | Out-File -FilePath $tempDiskpartFile -Encoding ASCII -Force
    $queryResult = diskpart /s $tempDiskpartFile 2>&1
    $queryResultString = $queryResult | Out-String
    Write-Info "Shrink query result" @{ output = $queryResultString }
    $maxShrinkMatch = $queryResultString | Select-String -Pattern ":\s+(\d+)\s+MB" -AllMatches
    $maxShrinkableMB = 0
    if ($maxShrinkMatch.Matches) {
        $lastMatch = $maxShrinkMatch.Matches[-1]
        $maxShrinkableMB = [int]$lastMatch.Groups[1].Value
    }
    $maxShrinkableGB = [math]::Round($maxShrinkableMB / 1024, 2)
    Write-Info "Maximum shrinkable space determined" @{ maxShrinkableMB = $maxShrinkableMB; maxShrinkableGB = $maxShrinkableGB }

    # Optional attempt to free up space
    if ($AutoOptimize.IsPresent -and ($FreeSpaceGB * 1024) -gt $maxShrinkableMB) {
        Write-Info "Auto-optimizing: disabling hibernation and defragmenting..."
        try { powercfg /h off 2>&1 | Out-Null; Write-Success "Hibernation disabled" } catch { Write-Warning-Custom "Failed to disable hibernation: $_" }
        try { Start-Process -FilePath "defrag.exe" -ArgumentList "C: /X" -Wait -NoNewWindow; Write-Success "Defrag completed" } catch { Write-Warning-Custom "Defrag failed: $_" }
        $queryScript | Out-File -FilePath $tempDiskpartFile -Encoding ASCII -Force
        $queryResult = diskpart /s $tempDiskpartFile 2>&1
        $queryResultString = $queryResult | Out-String
        $maxShrinkMatch = $queryResultString | Select-String -Pattern ":\s+(\d+)\s+MB" -AllMatches
        if ($maxShrinkMatch.Matches) {
            $lastMatch = $maxShrinkMatch.Matches[-1]
            $maxShrinkableMB = [int]$lastMatch.Groups[1].Value
            $maxShrinkableGB = [math]::Round($maxShrinkableMB / 1024, 2)
            Write-Info "Recomputed max shrink" @{ maxShrinkableMB = $maxShrinkableMB; maxShrinkableGB = $maxShrinkableGB }
        }
    }

    # Compute shrink amount from requested and max
    $requestedMB = [int]($FreeSpaceGB * 1024)
    if ($requestedMB -gt $maxShrinkableMB) {
        if ($AllowPartial) {
            Write-Warning-Custom "Requested shrink exceeds maximum; using maximum available" @{ requestedMB = $requestedMB; maxShrinkableMB = $maxShrinkableMB }
            $shrinkAmountMB = $maxShrinkableMB
        } else {
            Write-Error-Custom "Requested shrink exceeds maximum and partial not allowed" @{ requestedMB = $requestedMB; maxShrinkableMB = $maxShrinkableMB }
            exit 1
        }
    } else {
        $shrinkAmountMB = $requestedMB
    }

    # Ensure resulting partition >= 20GB
    $resultingGB = [math]::Round($cDrivePartition.SizeGB - ($shrinkAmountMB / 1024), 2)
    if ($resultingGB -lt 20) {
        Write-Error-Custom "Resulting partition would be below 20GB minimum" @{ resultingGB = $resultingGB }
        exit 1
    }

    # Execute shrink with computed amount
    Write-Info "Creating diskpart script..."
    $diskpartScript = @"
select disk $($cDrivePartition.DiskNumber)
select partition $($cDrivePartition.Number)
shrink desired=$($shrinkAmountMB)
"@
    Write-Info "Executing diskpart commands..."
    $diskpartScript | Out-File -FilePath $tempDiskpartFile -Encoding ASCII -Force
    $result = diskpart /s $tempDiskpartFile 2>&1
    $resultString = $result | Out-String
    Write-Info "Diskpart operation result" @{ output = $resultString; exitCode = $LASTEXITCODE }
    $shrinkSucceeded = ($resultString -match "MB\s+freigegeben|freed|successfully") -and ($resultString -notmatch "Fehler|error|failed")
    if ($shrinkSucceeded -or $LASTEXITCODE -eq 0) {
        Write-Success "Partition shrink completed" @{
            requestedShrinkGB = $FreeSpaceGB
            actualShrinkGB = [math]::Round($shrinkAmountMB / 1024, 2)
            diskNumber = $cDrivePartition.DiskNumber
            partitionNumber = $cDrivePartition.Number
            resultingPartitionGB = $resultingGB
        }
    } else {
        Write-Error-Custom "Partition shrink command encountered an error" @{ exitCode = $LASTEXITCODE; output = $resultString }
        exit 1
    }
}
catch {
    Write-Error-Custom "An error occurred" @{
        error = $_.ToString()
    }
    exit 1
}
finally {
    # Clean up temporary file
    if (Test-Path $tempDiskpartFile) {
        Remove-Item -Path $tempDiskpartFile -Force -ErrorAction SilentlyContinue
    }
}

Write-Success "Script execution completed"
