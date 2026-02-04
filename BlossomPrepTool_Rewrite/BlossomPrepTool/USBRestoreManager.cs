using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlossomPrepTool
{
    public class USBRestoreProgressEventArgs : EventArgs
    {
        public string Message { get; set; }
        public string Status { get; set; } // "info", "warning", "error", "success"
    }

    /// <summary>
    /// Manages USB drive restoration back to normal Windows USB format
    /// </summary>
    public class USBRestoreManager
    {
        public event EventHandler<USBRestoreProgressEventArgs> ProgressUpdate;

        public async Task<bool> RestoreUSB(int diskNumber)
        {
            try
            {
                ReportProgress($"Starting USB restore for Disk {diskNumber}...", "info");

                // Validate disk number
                if (diskNumber < 0)
                    throw new Exception($"Invalid disk number: {diskNumber}");

                // Create diskpart script to clean and prepare the disk
                var scriptPath = Path.GetTempFileName();
                var diskpartScript = new StringBuilder();
                diskpartScript.AppendLine($"select disk {diskNumber}");
                diskpartScript.AppendLine("online disk noerr");
                diskpartScript.AppendLine("attributes disk clear readonly noerr");
                diskpartScript.AppendLine("clean");
                diskpartScript.AppendLine("convert mbr");
                diskpartScript.AppendLine("create partition primary");
                diskpartScript.AppendLine("active");
                diskpartScript.AppendLine("format fs=fat32 quick label=\"USB\"");
                diskpartScript.AppendLine("assign");
                diskpartScript.AppendLine("exit");

                File.WriteAllText(scriptPath, diskpartScript.ToString());

                ReportProgress("Cleaning disk and removing all partitions...", "info");

                // Execute diskpart with the script
                var psi = new ProcessStartInfo
                {
                    FileName = "diskpart.exe",
                    Arguments = $"/s \"{scriptPath}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    Verb = "runas" // Run as administrator
                };

                using (var process = Process.Start(psi))
                {
                    var output = new StringBuilder();
                    var error = new StringBuilder();

                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            output.AppendLine(e.Data);
                            
                            // Report specific progress messages
                            if (e.Data.Contains("Cleaning") || e.Data.Contains("succeeded"))
                                ReportProgress(e.Data.Trim(), "info");
                        }
                    };

                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                            error.AppendLine(e.Data);
                    };

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    await Task.Run(() => process.WaitForExit());

                    // Clean up temp script file
                    try { File.Delete(scriptPath); } catch { }

                    if (process.ExitCode != 0)
                    {
                        var errorMsg = error.Length > 0 ? error.ToString() : "Diskpart failed";
                        throw new Exception($"Diskpart exited with code {process.ExitCode}: {errorMsg}");
                    }

                    var outputText = output.ToString();
                    
                    // Check for common error patterns in output
                    if (outputText.IndexOf("access is denied", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        throw new Exception("Access denied. Please run as administrator.");
                    }
                    
                    if (outputText.IndexOf("failed", StringComparison.OrdinalIgnoreCase) >= 0 && 
                        outputText.IndexOf("DiskPart successfully", StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        throw new Exception("Diskpart operation failed. Check if the disk is in use.");
                    }
                }

                // Give the system time to recognize the new partition
                await Task.Delay(2000);

                ReportProgress("USB drive restored successfully!", "success");
                return true;
            }
            catch (Exception ex)
            {
                ReportProgress($"Failed to restore USB: {ex.Message}", "error");
                return false;
            }
        }

        private void ReportProgress(string message, string status)
        {
            ProgressUpdate?.Invoke(this, new USBRestoreProgressEventArgs
            {
                Message = message,
                Status = status
            });
        }
    }
}
