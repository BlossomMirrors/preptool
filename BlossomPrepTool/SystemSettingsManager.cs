using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BlossomPrepTool
{
    /// <summary>
    /// Manages system-level settings for dual-boot preparation (timezone, fast startup, UEFI reboot)
    /// </summary>
    public class SystemSettingsManager
    {
        /// <summary>
        /// Set hardware clock to UTC (required for Linux dual-boot)
        /// </summary>
        public async Task<bool> SetTimeToUTC()
        {
            return await Task.Run(() =>
            {
                try
                {
                    const string regPath = @"SYSTEM\CurrentControlSet\Control\TimeZoneInformation";
                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(regPath, true))
                    {
                        if (key == null)
                        {
                            return false;
                        }
                        key.SetValue("RealTimeIsUniversal", 1, RegistryValueKind.DWord);
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            });
        }

        /// <summary>
        /// Disable Windows Fast Startup (prevents Linux partition access issues)
        /// </summary>
        public async Task<bool> DisableFastStartup()
        {
            return await Task.Run(() =>
            {
                try
                {
                    // Disable Fast Startup registry key
                    const string regPath = @"SYSTEM\CurrentControlSet\Control\Session Manager\Power";
                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(regPath, true))
                    {
                        if (key == null)
                        {
                            using (RegistryKey newKey = Registry.LocalMachine.CreateSubKey(regPath))
                            {
                                newKey?.SetValue("HiberbootEnabled", 0, RegistryValueKind.DWord);
                            }
                        }
                        else
                        {
                            key.SetValue("HiberbootEnabled", 0, RegistryValueKind.DWord);
                        }
                    }

                    // Disable hibernation
                    var psi = new ProcessStartInfo
                    {
                        FileName = "powercfg.exe",
                        Arguments = "/h off",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };

                    using (var process = Process.Start(psi))
                    {
                        process?.WaitForExit();
                    }

                    return true;
                }
                catch
                {
                    return false;
                }
            });
        }

        /// <summary>
        /// Reboot computer into UEFI firmware settings
        /// </summary>
        public bool RebootToUEFI()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "shutdown.exe",
                    Arguments = "/r /fw /t 0",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                Process.Start(psi);
                return true;
            }
            catch
            {
                // Fallback to normal reboot if firmware reboot not supported
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = "shutdown.exe",
                        Arguments = "/r /t 0",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    Process.Start(psi);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
