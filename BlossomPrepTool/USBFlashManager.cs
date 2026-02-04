using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace BlossomPrepTool
{
    public class USBFlashProgressEventArgs : EventArgs
    {
        public string Message { get; set; }
        public string Status { get; set; } // "info", "warning", "error", "success"
    }

    /// <summary>
    /// Manages USB flashing with dd via Chocolatey
    /// </summary>
    public class USBFlashManager
    {
        public event EventHandler<USBFlashProgressEventArgs> ProgressUpdate;
        private CancellationTokenSource _cancellationTokenSource;

        public async Task<bool> FlashUSB(int diskNumber, string isoPath)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                ReportProgress("Starting USB flash process...", "info");

                // Validate inputs
                if (!File.Exists(isoPath))
                    throw new Exception($"ISO file not found: {isoPath}");

                ReportProgress($"ISO validated: {isoPath}", "info");

                // Ensure Chocolatey is installed
                if (!await EnsureChocoInstalled())
                    throw new Exception("Chocolatey installation failed");

                // Ensure dd is installed
                if (!await EnsureDDInstalled())
                    throw new Exception("dd installation failed");

                ReportProgress("dd located and ready", "success");

                // Offline disk and clean
                ReportProgress("Preparing disk (offlining)...", "info");
                await PrepareUSBDisk(diskNumber);
                ReportProgress("Disk prepared", "success");

                // Flash ISO
                ReportProgress("Flashing ISO to USB (this may take a while)...", "info");
                await ExecuteFlashCommand(diskNumber, isoPath);

                ReportProgress("USB flashing completed successfully!", "success");
                return true;
            }
            catch (OperationCanceledException)
            {
                ReportProgress("USB flashing cancelled", "warning");
                return false;
            }
            catch (Exception ex)
            {
                ReportProgress($"Error: {ex.Message}", "error");
                return false;
            }
        }

        public void Cancel()
        {
            _cancellationTokenSource?.Cancel();
        }

        private async Task<bool> EnsureChocoInstalled()
        {
            try
            {
                if (ChocolateyInstaller.IsInstalled())
                {
                    var chocoPath = ChocolateyInstaller.GetChocoPath();
                    ReportProgress($"Chocolatey detected: {chocoPath}", "success");
                    return true;
                }

                ReportProgress("Installing Chocolatey...", "info");
                return await ChocolateyInstaller.InstallChocolatey(msg => ReportProgress(msg, "info"));
            }
            catch (Exception ex)
            {
                ReportProgress($"Chocolatey check failed: {ex.Message}", "warning");
                return false;
            }
        }

        private async Task<bool> EnsureDDInstalled()
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (!ChocolateyInstaller.IsInstalled())
                        return false;

                    // Check if dd is installed
                    if (ChocolateyInstaller.IsPackageInstalled("dd"))
                    {
                        ReportProgress("dd already installed", "success");
                        return true;
                    }

                    ReportProgress("Installing dd...", "info");
                    bool result = ChocolateyInstaller.InstallPackage("dd", msg => ReportProgress(msg, "info"));
                    if (result)
                        ReportProgress("dd installed", "success");
                    return result;
                }
                catch (Exception ex)
                {
                    ReportProgress($"dd installation failed: {ex.Message}", "error");
                    return false;
                }
            });
        }

        private async Task PrepareUSBDisk(int diskNumber)
        {
            await Task.Run(() =>
            {
                try
                {
                    var diskpartScript = new StringBuilder();
                    diskpartScript.AppendLine($"select disk {diskNumber}");
                    diskpartScript.AppendLine("clean all");
                    diskpartScript.AppendLine("offline disk noerr");
                    diskpartScript.AppendLine("exit");

                    var scriptPath = Path.Combine(Path.GetTempPath(), $"diskpart_{Guid.NewGuid()}.txt");
                    File.WriteAllText(scriptPath, diskpartScript.ToString(), Encoding.ASCII);

                    try
                    {
                        RunCommand("diskpart.exe", $"/s \"{scriptPath}\"");
                        ReportProgress("Disk offline complete, waiting...", "info");
                        Thread.Sleep(5000); // Wait for system to process
                    }
                    finally
                    {
                        File.Delete(scriptPath);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Disk preparation failed: {ex.Message}");
                }
            });
        }

        private async Task ExecuteFlashCommand(int diskNumber, string isoPath)
        {
            await Task.Run(() =>
            {
                try
                {
                    var ddPath = GetDDPath();
                    if (string.IsNullOrEmpty(ddPath))
                        throw new Exception("dd executable not found");

                    var psi = new ProcessStartInfo
                    {
                        FileName = ddPath,
                        Arguments = $"if=\"{isoPath}\" of=\\\\?\\PhysicalDrive{diskNumber} bs=4M",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    };

                    using (var process = Process.Start(psi))
                    {
                        var outputThread = new Thread(() =>
                        {
                            string line;
                            while ((line = process.StandardOutput.ReadLine()) != null)
                            {
                                if (_cancellationTokenSource.Token.IsCancellationRequested)
                                    process.Kill();

                                ReportProgress(line, "info");
                            }
                        });

                        var errorThread = new Thread(() =>
                        {
                            string line;
                            while ((line = process.StandardError.ReadLine()) != null)
                            {
                                if (_cancellationTokenSource.Token.IsCancellationRequested)
                                    process.Kill();

                                ReportProgress(line, "warning");
                            }
                        });

                        outputThread.Start();
                        errorThread.Start();

                        process.WaitForExit();
                        outputThread.Join(5000);
                        errorThread.Join(5000);

                        if (process.ExitCode != 0)
                            throw new Exception($"dd exited with code {process.ExitCode}");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Flash command failed: {ex.Message}");
                }
            });
        }

        private string GetDDPath()
        {
            try
            {
                var result = RunCommand("where.exe", "dd");
                if (!string.IsNullOrEmpty(result))
                    return result.Trim();
            }
            catch { }

            return null;
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

                if (process.ExitCode != 0 && !string.IsNullOrEmpty(error))
                    throw new Exception(error);

                return output;
            }
        }

        private void ReportProgress(string message, string status)
        {
            ProgressUpdate?.Invoke(this, new USBFlashProgressEventArgs
            {
                Message = message,
                Status = status
            });
        }
    }
}
