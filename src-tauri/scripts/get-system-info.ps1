param(
    [Parameter(Mandatory=$false)]
    [switch]$Detailed = $false
)

try {
    $systemInfo = @{
        Computer = $env:COMPUTERNAME
        User = $env:USERNAME
        Domain = $env:USERDOMAIN
        OS = [System.Environment]::OSVersion.VersionString
        PowerShellVersion = $PSVersionTable.PSVersion.ToString()
        Architecture = [System.Environment]::Is64BitOperatingSystem
        ProcessorCount = [System.Environment]::ProcessorCount
        MachineName = [System.Environment]::MachineName
        SystemDirectory = [System.Environment]::SystemDirectory
        UserDomainName = [System.Environment]::UserDomainName
        Timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    }

    if ($Detailed) {
        $computerSystem = Get-CimInstance Win32_ComputerSystem
        $operatingSystem = Get-CimInstance Win32_OperatingSystem
        $processor = Get-CimInstance Win32_Processor | Select-Object -First 1

        $systemInfo.TotalPhysicalMemoryGB = [Math]::Round($computerSystem.TotalPhysicalMemory / 1GB, 2)
        $systemInfo.FreePhysicalMemoryGB = [Math]::Round($operatingSystem.FreePhysicalMemory / 1MB / 1024, 2)
        $systemInfo.Manufacturer = $computerSystem.Manufacturer
        $systemInfo.Model = $computerSystem.Model
        $systemInfo.ProcessorName = $processor.Name
        $systemInfo.OSName = $operatingSystem.Caption
        $systemInfo.OSArchitecture = $operatingSystem.OSArchitecture
        $systemInfo.InstallDate = $operatingSystem.InstallDate
        $systemInfo.LastBootUpTime = $operatingSystem.LastBootUpTime
    }

    $result = @{
        success = $true
        data = $systemInfo
    }

    $result | ConvertTo-Json -Compress
    exit 0

} catch {
    $errorResult = @{
        success = $false
        error = $_.Exception.Message
        stackTrace = $_.ScriptStackTrace
    }

    $errorResult | ConvertTo-Json -Compress
    exit 1
}
