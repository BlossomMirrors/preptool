using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BlossomPrepTool
{
    /// <summary>
    /// Native C# Chocolatey installer without PowerShell dependency
    /// </summary>
    public static class ChocolateyInstaller
    {
        private const string ChocoInstallUrl = "https://community.chocolatey.org/install.ps1";

        /// <summary>
        /// Check if Chocolatey is installed
        /// </summary>
        public static bool IsInstalled()
        {
            var chocoPath = GetChocoPath();
            return !string.IsNullOrEmpty(chocoPath) && File.Exists(chocoPath);
        }

        /// <summary>
        /// Get the path to choco.exe
        /// </summary>
        public static string GetChocoPath()
        {
            try
            {
                // Try to find choco in PATH
                var result = RunCommand("where.exe", "choco");
                if (!string.IsNullOrEmpty(result))
                {
                    var lines = result.Trim().Split('\n');
                    if (lines.Length > 0)
                        return lines[0].Trim();
                }
            }
            catch { }

            // Check default installation path
            var fallbackPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "chocolatey", "bin", "choco.exe");

            return File.Exists(fallbackPath) ? fallbackPath : null;
        }

        /// <summary>
        /// Install Chocolatey using native C# implementation
        /// </summary>
        public static async Task<bool> InstallChocolatey(Action<string> progressCallback = null)
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "ChocoInstall_" + Guid.NewGuid().ToString("N"));

            try
            {
                progressCallback?.Invoke("Downloading Chocolatey installer...");

                // Download the install script
                string installScript;
                using (var httpClient = new HttpClient())
                {
                    installScript = await httpClient.GetStringAsync(ChocoInstallUrl);
                }

                progressCallback?.Invoke("Executing Chocolatey installation...");

                // Create temp directory and save the PowerShell script directly
                Directory.CreateDirectory(tempDir);
                var scriptPath = Path.Combine(tempDir, "install_choco.ps1");
                File.WriteAllText(scriptPath, installScript, Encoding.UTF8);

                // Execute the PowerShell script directly with -File parameter (safer than -Command)
                var psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -ExecutionPolicy Bypass -File \"{scriptPath}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = tempDir
                };

                using (var process = Process.Start(psi))
                {
                    var output = await process.StandardOutput.ReadToEndAsync();
                    var error = await process.StandardError.ReadToEndAsync();
                    
                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        progressCallback?.Invoke($"Installation failed: {error}");
                        return false;
                    }

                    // Refresh environment variables
                    RefreshEnvironmentPath();

                    progressCallback?.Invoke("Chocolatey installed successfully");
                    return IsInstalled();
                }
            }
            catch (Exception ex)
            {
                progressCallback?.Invoke($"Installation error: {ex.Message}");
                return false;
            }
            finally
            {
                // Clean up
                try
                {
                    if (Directory.Exists(tempDir))
                        Directory.Delete(tempDir, true);
                }
                catch { }
            }
        }

        /// <summary>
        /// Check if a Chocolatey package is installed
        /// </summary>
        public static bool IsPackageInstalled(string packageName)
        {
            try
            {
                var chocoPath = GetChocoPath();
                if (string.IsNullOrEmpty(chocoPath))
                    return false;

                var output = RunCommand(chocoPath, $"list --local-only --exact {packageName}");
                return output.IndexOf(packageName, StringComparison.OrdinalIgnoreCase) >= 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Install a Chocolatey package
        /// </summary>
        public static bool InstallPackage(string packageName, Action<string> progressCallback = null)
        {
            try
            {
                var chocoPath = GetChocoPath();
                if (string.IsNullOrEmpty(chocoPath))
                {
                    progressCallback?.Invoke("Chocolatey not found");
                    return false;
                }

                progressCallback?.Invoke($"Installing {packageName}...");

                var psi = new ProcessStartInfo
                {
                    FileName = chocoPath,
                    Arguments = $"install {packageName} -y --no-progress",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(psi))
                {
                    string line;
                    while ((line = process.StandardOutput.ReadLine()) != null)
                    {
                        progressCallback?.Invoke(line);
                    }

                    process.WaitForExit();

                    if (process.ExitCode == 0)
                    {
                        progressCallback?.Invoke($"{packageName} installed successfully");
                        return true;
                    }
                    else
                    {
                        var error = process.StandardError.ReadToEnd();
                        progressCallback?.Invoke($"Installation failed: {error}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                progressCallback?.Invoke($"Error installing package: {ex.Message}");
                return false;
            }
        }

        private static string RunCommand(string filename, string arguments)
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

        private static void RefreshEnvironmentPath()
        {
            // Refresh the PATH environment variable for the current process
            var path = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);
            var userPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User);
            
            if (!string.IsNullOrEmpty(userPath))
                path = path + ";" + userPath;

            Environment.SetEnvironmentVariable("PATH", path, EnvironmentVariableTarget.Process);
        }
    }
}
