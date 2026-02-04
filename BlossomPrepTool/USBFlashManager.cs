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

                // Ensure balena-etcher-cli is installed
                if (!await EnsureBalenaEtcherInstalled())
                    throw new Exception("balena-etcher-cli installation failed");

                ReportProgress("balena-etcher-cli located and ready", "success");

                // Flash ISO (balena-etcher handles disk preparation and cleaning)
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

        private async Task<bool> EnsureBalenaEtcherInstalled()
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (!ChocolateyInstaller.IsInstalled())
                    {
                        AppendLog("Chocolatey not installed, cannot install balena-cli");
                        return false;
                    }

                    // Check if balena-cli is installed
                    if (ChocolateyInstaller.IsPackageInstalled("balena-cli"))
                    {
                        AppendLog("balena-cli already installed");
                        ReportProgress("balena-cli already installed", "success");
                        return true;
                    }

                    AppendLog("Installing balena-cli package...");
                    ReportProgress("Installing balena-cli...", "info");
                    bool result = ChocolateyInstaller.InstallPackage("balena-cli", msg => { AppendLog($"etcher install: {msg}"); ReportProgress(msg, "info"); });
                    if (result)
                    {
                        AppendLog("balena-cli installed successfully");
                        ReportProgress("balena-cli installed", "success");
                    }
                    else
                    {
                        AppendLog("balena-cli installation returned false");
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    AppendLog($"Exception in EnsureBalenaEtcherInstalled: {ex}");
                    ReportProgress($"balena-cli installation failed: {ex.Message}", "error");
                    return false;
                }
            });
        }



        private async Task ExecuteFlashCommand(int diskNumber, string isoPath)
        {
            await Task.Run(() =>
            {
                try
                {
                    var balenaPath = GetBalenaEtcherPath();
                    if (string.IsNullOrEmpty(balenaPath))
                        throw new Exception("balena-cli executable not found");

                    // Get the drive path from disk number (e.g., \\.\PhysicalDrive1)
                    var drivePath = $"\\\\.\\PhysicalDrive{diskNumber}";

                    var psi = new ProcessStartInfo
                    {
                        FileName = balenaPath,
                        Arguments = $"local flash \"{isoPath}\" --drive \"{drivePath}\" --yes",
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
                                {
                                    try { process.Kill(); } catch { }
                                }

                                AppendLog($"[etcher stdout] {line}");
                                ReportProgress(line, "info");
                            }
                        });

                        var errorThread = new Thread(() =>
                        {
                            string line;
                            while ((line = process.StandardError.ReadLine()) != null)
                            {
                                if (_cancellationTokenSource.Token.IsCancellationRequested)
                                {
                                    try { process.Kill(); } catch { }
                                }

                                AppendLog($"[etcher stderr] {line}");
                                ReportProgress(line, "warning");
                            }
                        });

                        outputThread.Start();
                        errorThread.Start();

                        // Wait with cancellation support
                        while (!process.HasExited)
                        {
                            if (_cancellationTokenSource.Token.IsCancellationRequested)
                            {
                                try { process.Kill(); } catch { }
                                AppendLog("Flash process killed by user");
                                throw new OperationCanceledException();
                            }
                            Thread.Sleep(200);
                        }

                        outputThread.Join(5000);
                        errorThread.Join(5000);

                        AppendLog($"balena-cli exited with code: {process.ExitCode}");
                        if (process.ExitCode != 0)
                            throw new Exception($"balena-cli exited with code {process.ExitCode}");
                    }
                }
                catch (Exception ex)
                {
                    AppendLog($"Exception in ExecuteFlashCommand: {ex}");
                    throw new Exception($"Flash command failed: {ex.Message}");
                }
            });
        }

        private string GetBalenaEtcherPath()
        {
            try
            {
                var result = RunCommand("where.exe", "balena");
                if (!string.IsNullOrEmpty(result))
                {
                    var etcherPath = result.Trim();
                    AppendLog($"balena-cli found at: {etcherPath}");
                    return etcherPath;
                }
                AppendLog("balena-cli not found in PATH");
            }
            catch (Exception ex)
            {
                AppendLog($"Exception in GetBalenaEtcherPath: {ex}");
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
