$ErrorActionPreference = 'SilentlyContinue'

$cacheDir = "$env:TEMP\BlossomOS"
$isoPath = Join-Path $cacheDir "BlossomOS.iso"

$exists = (Test-Path $isoPath) -and (Get-Item $isoPath).Length -gt 0
$result = @{
    exists = $exists
    path = if ($exists) { $isoPath } else { $null }
}

$json = ConvertTo-Json -InputObject $result -Depth 3
Write-Output $json
