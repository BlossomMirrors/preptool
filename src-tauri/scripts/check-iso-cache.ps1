$ErrorActionPreference = 'SilentlyContinue'

$cacheDir = "$env:TEMP\BlossomOS"
$isoPath = Join-Path $cacheDir "BlossomOS.iso"

$exists = Test-Path $isoPath
$result = @{
    exists = $exists
    path = if ($exists) { $isoPath } else { "" }
}

$json = ConvertTo-Json -InputObject $result -Depth 3
Write-Output $json
