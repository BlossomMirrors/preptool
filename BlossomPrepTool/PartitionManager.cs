using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BlossomPrepTool
{
    public class PartitionProgressEventArgs : EventArgs
    {
        public string Message { get; set; }
        public string Status { get; set; } // "info", "warning", "error", "success"
    }

    /// <summary>
    /// Manages disk partition resizing
    /// </summary>
    public class PartitionManager
    {
        public event EventHandler<PartitionProgressEventArgs> ProgressUpdate;

        public class DriveSizeInfo
        {
            public double TotalSizeGB { get; set; }
            public double FreeSpaceGB { get; set; }
            public double UsedSpaceGB { get; set; }
            public int DiskNumber { get; set; }
            public int PartitionNumber { get; set; }
        }

        public async Task<DriveSizeInfo> GetCDriveSizeInfo()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var driveInfo = new System.IO.DriveInfo("C");
                    var totalBytes = driveInfo.TotalSize;
                    var freeBytes = driveInfo.AvailableFreeSpace;
                    var usedBytes = totalBytes - freeBytes;

                    // Query WMI to get disk and partition numbers for C: drive
                    int diskNumber = 0;
                    int partitionNumber = 1;

                    try
                    {
                        // Map the logical disk to its partition, then get disk info
                        using (var searcher = new ManagementObjectSearcher(
                            "SELECT Antecedent, Dependent FROM Win32_LogicalDiskToPartition"))
                        {
                            var collection = searcher.Get();
                            foreach (var obj in collection)
                            {
                                string dependent = obj["Dependent"].ToString();
                                if (dependent.Contains("C:"))
                                {
                                    // Extract partition name and query its disk info
                                    string partitionName = dependent.Split('"')[1];
                                    using (var diskPartitionSearcher = new ManagementObjectSearcher(
                                        $"SELECT DiskIndex, Index FROM Win32_DiskPartition WHERE Name='{partitionName}'"))
                                    {
                                        var diskPartitions = diskPartitionSearcher.Get();
                                        foreach (var diskPart in diskPartitions)
                                        {
                                            diskNumber = Convert.ToInt32(diskPart["DiskIndex"]);
                                            partitionNumber = Convert.ToInt32(diskPart["Index"]);
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception wmiEx)
                    {
                        ReportProgress($"Warning: Could not retrieve partition info from WMI: {wmiEx.Message}. Using defaults (Disk 0, Partition 1).", "warning");
                        diskNumber = 0;
                        partitionNumber = 1;
                    }

                    ReportProgress($"Retrieved C: drive info (Disk {diskNumber}, Partition {partitionNumber})", "info");

                    return new DriveSizeInfo
                    {
                        TotalSizeGB = Math.Round(totalBytes / (1024.0 * 1024.0 * 1024.0), 2),
                        FreeSpaceGB = Math.Round(freeBytes / (1024.0 * 1024.0 * 1024.0), 2),
                        UsedSpaceGB = Math.Round(usedBytes / (1024.0 * 1024.0 * 1024.0), 2),
                        DiskNumber = diskNumber,
                        PartitionNumber = partitionNumber
                    };
                }
                catch (Exception ex)
                {
                    ReportProgress($"Error getting drive info: {ex.Message}", "error");
                    throw;
                }
            });
        }

        public async Task<bool> ResizePartition(double shrinkAmountGB, bool autoOptimize = false)
        {
            try
            {
                ReportProgress("Starting partition resize...", "info");

                var driveInfo = await GetCDriveSizeInfo();
                ReportProgress(
                    $"C: drive - Total: {driveInfo.TotalSizeGB}GB, Free: {driveInfo.FreeSpaceGB}GB, Used: {driveInfo.UsedSpaceGB}GB",
                    "info");

                // Validate
                if (shrinkAmountGB <= 0)
                    throw new Exception("Shrink amount must be > 0GB");

                if (shrinkAmountGB > driveInfo.FreeSpaceGB)
                    throw new Exception($"Shrink amount ({shrinkAmountGB}GB) exceeds available free space ({driveInfo.FreeSpaceGB}GB). Please enter a smaller value.");

                const double minWindowsSize = 20.0; // Keep at least 20GB for Windows
                var resultingPartitionGB = driveInfo.TotalSizeGB - shrinkAmountGB;

                if (resultingPartitionGB < minWindowsSize)
                {
                    throw new Exception($"Resulting partition would be below {minWindowsSize}GB minimum. Maximum shrink allowed: {driveInfo.TotalSizeGB - minWindowsSize}GB");
                }

                ReportProgress(
                    $"Shrink parameters - Shrinking by: {shrinkAmountGB}GB, resulting C: size: {resultingPartitionGB}GB",
                    "info");

                // Optional auto-optimize: run when shrink amount exceeds current free space
                if (autoOptimize && shrinkAmountGB > driveInfo.FreeSpaceGB)
                {
                    ReportProgress("Auto-optimizing disk to free up space...", "info");
                    await DisableHibernation();
                    await RunDefragmentation();
                }

                // Execute shrink with detected disk and partition numbers
                return await ExecuteShrink(shrinkAmountGB, driveInfo.DiskNumber, driveInfo.PartitionNumber);
            }
            catch (Exception ex)
            {
                ReportProgress($"Partition resize failed: {ex.Message}", "error");
                return false;
            }
        }

        private async Task<bool> ExecuteShrink(double shrinkAmountGB, int diskNumber, int partitionNumber)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var scriptPath = Path.Combine(Path.GetTempPath(), $"diskpart_{Guid.NewGuid()}.txt");

                    var queryMaxScript = new StringBuilder();
                    queryMaxScript.AppendLine($"select disk {diskNumber}");
                    queryMaxScript.AppendLine($"select partition {partitionNumber}");
                    queryMaxScript.AppendLine("shrink querymax");

                    File.WriteAllText(scriptPath, queryMaxScript.ToString(), Encoding.ASCII);
                    var queryOutput = RunCommand("diskpart.exe", $"/s \"{scriptPath}\"");
                    File.Delete(scriptPath);

                    var match = Regex.Match(queryOutput, @"Maximum possible shrink: (\d+) MB", RegexOptions.IgnoreCase);
                    if (!match.Success)
                    {
                        ReportProgress("Could not determine maximum shrink size. Aborting.", "error");
                        return false;
                    }

                    long maxShrinkMB = long.Parse(match.Groups[1].Value);
                    long desiredShrinkMB = (long)(shrinkAmountGB * 1024);

                    if (desiredShrinkMB > maxShrinkMB)
                    {
                        ReportProgress($"Requested shrink ({desiredShrinkMB} MB) exceeds maximum possible ({maxShrinkMB} MB). Shrinking only max possible.", "warning");
                        desiredShrinkMB = maxShrinkMB;
                    }

                    scriptPath = Path.Combine(Path.GetTempPath(), $"diskpart_{Guid.NewGuid()}.txt");
                    var shrinkScript = new StringBuilder();
                    shrinkScript.AppendLine($"select disk {diskNumber}");
                    shrinkScript.AppendLine($"select partition {partitionNumber}");
                    shrinkScript.AppendLine($"shrink desired={desiredShrinkMB} minimum=100");

                    File.WriteAllText(scriptPath, shrinkScript.ToString(), Encoding.ASCII);
                    var shrinkOutput = RunCommand("diskpart.exe", $"/s \"{scriptPath}\"");
                    File.Delete(scriptPath);

                    ReportProgress("Diskpart output: " + shrinkOutput, "info");

                    if (shrinkOutput.IndexOf("error", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        shrinkOutput.IndexOf("failed", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        ReportProgress("Shrink operation reported an error. Please ensure the partition has enough contiguous free space.", "error");
                        return false;
                    }

                    ReportProgress($"Partition resized successfully by {desiredShrinkMB / 1024.0:F2} GB", "success");
                    return true;
                }
                catch (Exception ex)
                {
                    ReportProgress($"Shrink execution failed: {ex.Message}", "error");
                    return false;
                }
            });
        }

        private async Task DisableHibernation()
        {
            await Task.Run(() =>
            {
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = "powercfg.exe",
                        Arguments = "/h off",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true
                    };

                    using (var process = Process.Start(psi))
                    {
                        process.WaitForExit();
                        if (process.ExitCode == 0)
                            ReportProgress("Hibernation disabled", "success");
                        else
                            ReportProgress("Failed to disable hibernation", "warning");
                    }
                }
                catch (Exception ex)
                {
                    ReportProgress($"Error disabling hibernation: {ex.Message}", "warning");
                }
            });
        }

        private async Task RunDefragmentation()
        {
            await Task.Run(() =>
            {
                try
                {
                    ReportProgress("Running defragmentation...", "info");

                    var psi = new ProcessStartInfo
                    {
                        FileName = "defrag.exe",
                        Arguments = "C:",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true
                    };

                    using (var process = Process.Start(psi))
                    {
                        process.WaitForExit();
                        if (process.ExitCode == 0)
                            ReportProgress("Defragmentation completed successfully", "success");
                        else
                            ReportProgress("Defragmentation completed with exit code: " + process.ExitCode, "warning");
                    }
                }
                catch (Exception ex)
                {
                    ReportProgress($"Error during defragmentation: {ex.Message}", "warning");
                }
            });
        }

        private string RunCommand(string filename, string arguments)
        {
            var psi = new ProcessStartInfo
            {
                FileName = filename,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (var process = Process.Start(psi))
            {
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                return !string.IsNullOrEmpty(error) ? error : output;
            }
        }

        private void ReportProgress(string message, string status)
        {
            ProgressUpdate?.Invoke(this, new PartitionProgressEventArgs
            {
                Message = message,
                Status = status
            });
        }
    }
}
