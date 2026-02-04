using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace BlossomPrepTool
{
    public class WinBTRFSProgressEventArgs : EventArgs
    {
        public string Message { get; set; }
        public string Status { get; set; } // "info", "warning", "error", "success"
    }

    /// <summary>
    /// Manages winbtrfs installation via Chocolatey
    /// </summary>
    public class WinBTRFSManager
    {
        public event EventHandler<WinBTRFSProgressEventArgs> ProgressUpdate;

        public async Task<bool> InstallWinBTRFS()
        {
            try
            {
                ReportProgress("Starting winbtrfs installation...", "info");

                // Ensure Chocolatey is installed
                if (!await EnsureChocoInstalled())
                    throw new Exception("Chocolatey not available");

                // Check if already installed
                ReportProgress("Checking winbtrfs installation status...", "info");
                if (await IsWinBTRFSInstalled())
                {
                    ReportProgress("winbtrfs is already installed", "success");
                    return true;
                }

                // Install winbtrfs
                ReportProgress("Installing winbtrfs...", "info");
                return await ExecuteChocoInstall("winbtrfs");
            }
            catch (Exception ex)
            {
                ReportProgress($"Installation failed: {ex.Message}", "error");
                return false;
            }
        }

        private async Task<bool> EnsureChocoInstalled()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var chocoPath = GetChocoPath();
                    if (!string.IsNullOrEmpty(chocoPath))
                    {
                        ReportProgress($"Chocolatey detected: {chocoPath}", "success");
                        return true;
                    }

                    ReportProgress("Installing Chocolatey...", "info");
                    return InstallChocolatey();
                }
                catch (Exception ex)
                {
                    ReportProgress($"Chocolatey check failed: {ex.Message}", "error");
                    return false;
                }
            });
        }

        private async Task<bool> IsWinBTRFSInstalled()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var chocoPath = GetChocoPath();
                    if (string.IsNullOrEmpty(chocoPath))
                        return false;

                    var output = RunCommand(chocoPath, "list --local-only --exact winbtrfs");
                    return output.IndexOf("winbtrfs", StringComparison.OrdinalIgnoreCase) >= 0;
                }
                catch
                {
                    return false;
                }
            });
        }

        private async Task<bool> ExecuteChocoInstall(string packageName)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var chocoPath = GetChocoPath();
                    if (string.IsNullOrEmpty(chocoPath))
                        throw new Exception("Chocolatey not found");

                    var psi = new ProcessStartInfo
                    {
                        FileName = chocoPath,
                        Arguments = $"install {packageName} -y",
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

                        ReportProgress(output, "info");

                        if (process.ExitCode != 0)
                        {
                            ReportProgress($"Installation failed: {error}", "error");
                            return false;
                        }

                        ReportProgress($"{packageName} installed successfully", "success");
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    ReportProgress($"Install execution failed: {ex.Message}", "error");
                    return false;
                }
            });
        }

        private bool InstallChocolatey()
        {
            try
            {
                var tempDir = Path.Combine(Path.GetTempPath(), "chocoInstall");
                Directory.CreateDirectory(tempDir);
                var scriptPath = Path.Combine(tempDir, "install.ps1");

                ReportProgress("Downloading Chocolatey installer...", "info");

                // Download Chocolatey installer
                using (var client = new System.Net.WebClient())
                {
                    client.DownloadFile(
                        "https://community.chocolatey.org/install.ps1",
                        scriptPath);
                }

                ReportProgress("Executing Chocolatey installer...", "info");

                // Execute installer with proper shell handling
                var psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -ExecutionPolicy Bypass -File \"{scriptPath}\"",
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

                    ReportProgress(output, "info");

                    if (process.ExitCode != 0)
                    {
                        ReportProgress($"Chocolatey installation failed: {error}", "error");
                        return false;
                    }

                    ReportProgress("Chocolatey installed successfully", "success");
                    return true;
                }
            }
            catch (Exception ex)
            {
                ReportProgress($"Chocolatey installation error: {ex.Message}", "error");
                return false;
            }
        }

        private string GetChocoPath()
        {
            try
            {
                var result = RunCommand("where.exe", "choco");
                if (!string.IsNullOrEmpty(result))
                    return result.Trim().Split('\n')[0]; // Get first result in case of multiple
            }
            catch { }

            var fallback = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "chocolatey", "bin", "choco.exe");

            return File.Exists(fallback) ? fallback : null;
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
            ProgressUpdate?.Invoke(this, new WinBTRFSProgressEventArgs
            {
                Message = message,
                Status = status
            });
        }
    }
}
