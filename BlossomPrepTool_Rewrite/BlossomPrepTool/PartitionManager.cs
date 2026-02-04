using System;
using System.Diagnostics;
using System.IO;
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

                    ReportProgress("Retrieved C: drive info", "info");

                    return new DriveSizeInfo
                    {
                        TotalSizeGB = Math.Round(totalBytes / (1024.0 * 1024.0 * 1024.0), 2),
                        FreeSpaceGB = Math.Round(freeBytes / (1024.0 * 1024.0 * 1024.0), 2),
                        UsedSpaceGB = Math.Round(usedBytes / (1024.0 * 1024.0 * 1024.0), 2),
                        DiskNumber = 0, // Will be retrieved from WMI
                        PartitionNumber = 1 // Assuming C: is partition 1
                    };
                }
                catch (Exception ex)
                {
                    ReportProgress($"Error getting drive info: {ex.Message}", "error");
                    throw;
                }
            });
        }

        public async Task<bool> ResizePartition(double targetFreeSpaceGB, bool autoOptimize = false)
        {
            try
            {
                ReportProgress("Starting partition resize...", "info");

                var driveInfo = await GetCDriveSizeInfo();
                ReportProgress(
                    $"C: drive - Total: {driveInfo.TotalSizeGB}GB, Free: {driveInfo.FreeSpaceGB}GB, Used: {driveInfo.UsedSpaceGB}GB",
                    "info");

                // Validate
                if (targetFreeSpaceGB <= 0)
                    throw new Exception("Free space must be > 0GB");

                const double minWindowsSize = 20.0; // Keep at least 20GB for Windows
                var maxAllowedShrink = driveInfo.TotalSizeGB - minWindowsSize;

                if (targetFreeSpaceGB > maxAllowedShrink)
                {
                    ReportProgress(
                        $"Requested partition exceeds available space, using maximum: {maxAllowedShrink}GB",
                        "warning");
                    targetFreeSpaceGB = maxAllowedShrink;
                }

                var resultingPartitionGB = driveInfo.TotalSizeGB - targetFreeSpaceGB;
                if (resultingPartitionGB < minWindowsSize)
                    throw new Exception($"Resulting partition would be below {minWindowsSize}GB minimum");

                ReportProgress(
                    $"Shrink parameters - Free space requested: {targetFreeSpaceGB}GB, resulting C: size: {resultingPartitionGB}GB",
                    "info");

                // Optional auto-optimize
                if (autoOptimize && targetFreeSpaceGB > (driveInfo.FreeSpaceGB - 2))
                {
                    ReportProgress("Auto-optimizing disk...", "info");
                    await DisableHibernation();
                    await RunDefragmentation();
                }

                // Execute shrink
                return await ExecuteShrink(targetFreeSpaceGB);
            }
            catch (Exception ex)
            {
                ReportProgress($"Partition resize failed: {ex.Message}", "error");
                return false;
            }
        }

        private async Task<bool> ExecuteShrink(double targetFreeSpaceGB)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var shrinkMB = (long)(targetFreeSpaceGB * 1024);
                    var scriptPath = Path.Combine(Path.GetTempPath(), $"diskpart_{Guid.NewGuid()}.txt");

                    // Create diskpart script
                    var diskpartScript = new StringBuilder();
                    diskpartScript.AppendLine("select disk 0");
                    diskpartScript.AppendLine("select partition 2"); // Assuming C: is partition 2 on system disk
                    diskpartScript.AppendLine($"shrink desired={shrinkMB}");

                    File.WriteAllText(scriptPath, diskpartScript.ToString(), Encoding.ASCII);

                    try
                    {
                        var output = RunCommand("diskpart.exe", $"/s \"{scriptPath}\"");
                        ReportProgress("Diskpart output: " + output, "info");

                        if (output.IndexOf("error", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            ReportProgress("Shrink operation reported an error", "error");
                            return false;
                        }

                        ReportProgress("Partition resize completed successfully", "success");
                        return true;
                    }
                    finally
                    {
                        try { File.Delete(scriptPath); } catch { }
                    }
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
                        Arguments = "C: /X",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true
                    };

                    using (var process = Process.Start(psi))
                    {
                        process.WaitForExit();
                        if (process.ExitCode == 0)
                            ReportProgress("Defragmentation completed", "success");
                        else
                            ReportProgress("Defragmentation completed with warnings", "warning");
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
