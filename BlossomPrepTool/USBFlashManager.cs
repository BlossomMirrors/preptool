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
        private readonly string _logFilePath;

        public async Task<bool> FlashUSB(int diskNumber, string isoPath)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                AppendLog($"===== USB Flash started for Disk {diskNumber} =====");
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
                AppendLog($"Exception in FlashUSB: {ex}");
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
                    AppendLog($"Chocolatey found: {chocoPath}");
                    ReportProgress($"Chocolatey detected: {chocoPath}", "success");
                    return true;
                }

                AppendLog("Chocolatey not found, installing...");
                ReportProgress("Installing Chocolatey...", "info");
                return await ChocolateyInstaller.InstallChocolatey(msg => { AppendLog($"Choco install: {msg}"); ReportProgress(msg, "info"); });
            }
            catch (Exception ex)
            {
                AppendLog($"Exception in EnsureChocoInstalled: {ex}");
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
                    {
                        AppendLog("Chocolatey not installed, cannot install dd");
                        return false;
                    }

                    // Check if dd is installed
                    if (ChocolateyInstaller.IsPackageInstalled("dd"))
                    {
                        AppendLog("dd already installed");
                        ReportProgress("dd already installed", "success");
                        return true;
                    }

                    AppendLog("Installing dd package...");
                    ReportProgress("Installing dd...", "info");
                    bool result = ChocolateyInstaller.InstallPackage("dd", msg => { AppendLog($"dd install: {msg}"); ReportProgress(msg, "info"); });
                    if (result)
                    {
                        AppendLog("dd installed successfully");
                        ReportProgress("dd installed", "success");
                    }
                    else
                    {
                        AppendLog("dd installation returned false");
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    AppendLog($"Exception in EnsureDDInstalled: {ex}");
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
                    ReportProgress("Setting disk offline...", "info");
                    try { Win32DiskHelper.SetOffline(diskNumber); } catch { /* fallback: diskpart if fails */ }

                    ReportProgress("Cleaning disk...", "info");
                    var scriptPath = Path.Combine(Path.GetTempPath(), $"diskpart_{Guid.NewGuid()}.txt");
                    File.WriteAllText(scriptPath, $"select disk {diskNumber}\nclean\nexit", Encoding.ASCII);
                    try
                    {
                        RunCommand("diskpart.exe", $"/s \"{scriptPath}\"");
                    }
                    finally { File.Delete(scriptPath); }

                    Thread.Sleep(2000); // small delay to let system stabilize

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
                {
                    var ddPath = result.Trim();
                    AppendLog($"dd found at: {ddPath}");
                    return ddPath;
                }
                AppendLog("dd not found in PATH");
            }
            catch (Exception ex)
            {
                AppendLog($"Exception in GetDDPath: {ex}");
            }

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

                AppendLog($"Command: {filename} {arguments}\nExitCode: {process.ExitCode}\nStdOut: {output}\nStdErr: {error}");

                if (process.ExitCode != 0 && !string.IsNullOrEmpty(error))
                    throw new Exception(error);

                return output;
            }
        }

        private void ReportProgress(string message, string status)
        {
            AppendLog($"[{status}] {message}");
            ProgressUpdate?.Invoke(this, new USBFlashProgressEventArgs
            {
                Message = message,
                Status = status
            });
        }

        private void AppendLog(string message)
        {
            try
            {
                var logDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "BlossomPrepTool");

                if (!Directory.Exists(logDir))
                    Directory.CreateDirectory(logDir);

                var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} {message}";
                File.AppendAllText(_logFilePath, line + Environment.NewLine, Encoding.UTF8);
            }
            catch
            {
                // Intentionally ignore logging failures
            }
        }

        public USBFlashManager()
        {
            var logDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "BlossomPrepTool");
            _logFilePath = Path.Combine(logDir, "usb_flash.log");
        }
    }
}
