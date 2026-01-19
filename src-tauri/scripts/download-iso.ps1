$ErrorActionPreference = 'SilentlyContinue'

param(
    [string]$UsbDrive = ""
)

$cacheDir = "$env:TEMP\BlossomOS"
$isoPath = Join-Path $cacheDir "BlossomOS.iso"
$isoUrl = "https://cdn.blossomos.org/iso/BlossomOS-2026.01.16-x86_64.iso"
$result = @{ success = $false; message = ""; path = "" }

try {
    if (-not (Test-Path $cacheDir)) {
        New-Item -ItemType Directory -Path $cacheDir -Force | Out-Null
    }

    if (Test-Path $isoPath) {
        # Verify SHA256 if cached file exists
        $sha256Url = "$isoUrl.sha256"
        try {
            Write-Output "[PROGRESS] Verifying cached ISO..."
            $ProgressPreference = 'SilentlyContinue'
            $sha256Content = (Invoke-WebRequest -Uri $sha256Url -UseBasicParsing -ErrorAction Stop).Content
            $expectedHash = ($sha256Content -split '\s+')[0]
            Write-Output "[PROGRESS] Computing checksum..."
            $fileHash = (Get-FileHash -Path $isoPath -Algorithm SHA256).Hash
            
            if ($fileHash -eq $expectedHash) {
                Write-Output "[PROGRESS] Verification complete"
                $result.success = $true
                $result.message = "ISO already cached and verified"
                $result.path = $isoPath
            }
            else {
                Remove-Item $isoPath -Force
                throw "SHA256 mismatch"
            }
        }
        catch {
            # If verification fails, consider it not cached
            if (Test-Path $isoPath) {
                Remove-Item $isoPath -Force
            }
            throw $_
        }
    }
    else {
        throw "ISO not found"
    }
}
catch {
    # Download the ISO
    try {
        Write-Output "[PROGRESS] Starting download..."
        $ProgressPreference = 'SilentlyContinue'
        
        # Use a custom progress tracking for download
        $startTime = Get-Date
        Invoke-WebRequest -Uri $isoUrl -OutFile $isoPath -UseBasicParsing -ErrorAction Stop 2>$null
        $downloadTime = ((Get-Date) - $startTime).TotalSeconds
        
        Write-Output "[PROGRESS] Download complete, verifying SHA256..."
        $sha256Url = "$isoUrl.sha256"
        $sha256Content = (Invoke-WebRequest -Uri $sha256Url -UseBasicParsing -ErrorAction Stop).Content
        $expectedHash = ($sha256Content -split '\s+')[0]
        
        Write-Output "[PROGRESS] Computing checksum..."
        $fileHash = (Get-FileHash -Path $isoPath -Algorithm SHA256).Hash
        
        if ($fileHash -eq $expectedHash) {
            Write-Output "[PROGRESS] Verification complete"
            $result.success = $true
            $result.message = "Downloaded successfully and verified"
            $result.path = $isoPath
        }
        else {
            Remove-Item $isoPath -Force
            throw "SHA256 verification failed"
        }
    }
    catch {
        $result.success = $false
        $result.message = "Error: $_"
        if (Test-Path $isoPath) {
            Remove-Item $isoPath -Force
        }
    }
}

$json = ConvertTo-Json -InputObject $result -Depth 3
Write-Output $json
