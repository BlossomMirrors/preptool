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
                ReportProgress($"Chocolatey check failed: {ex.Message}", "error");
                return false;
            }
        }

        private async Task<bool> IsWinBTRFSInstalled()
        {
            return await Task.Run(() =>
            {
                try
                {
                    return ChocolateyInstaller.IsPackageInstalled("winbtrfs");
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
                    if (!ChocolateyInstaller.IsInstalled())
                        throw new Exception("Chocolatey not found");

                    return ChocolateyInstaller.InstallPackage(packageName, msg => ReportProgress(msg, "info"));
                }
                catch (Exception ex)
                {
                    ReportProgress($"Install execution failed: {ex.Message}", "error");
                    return false;
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
            ProgressUpdate?.Invoke(this, new WinBTRFSProgressEventArgs
            {
                Message = message,
                Status = status
            });
        }
    }
}
