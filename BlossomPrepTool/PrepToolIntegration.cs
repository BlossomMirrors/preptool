using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlossomPrepTool
{
    /// <summary>
    /// Integration helper for all preptool functionality
    /// Provides unified access to all managers and utilities
    /// </summary>
    public class PrepToolIntegration
    {
        private ISOManager _isoManager;
        private USBFlashManager _usbFlashManager;
        private USBRestoreManager _usbRestoreManager;
        private PartitionManager _partitionManager;
        private WinBTRFSManager _winbtrfsManager;
        private SystemSettingsManager _systemSettingsManager;

        public event EventHandler<EventArgs> StatusChanged;
        public event EventHandler<ISODownloadProgressEventArgs> DownloadProgress;

        public PrepToolIntegration()
        {
            _isoManager = new ISOManager();
            _usbFlashManager = new USBFlashManager();
            _usbRestoreManager = new USBRestoreManager();
            _partitionManager = new PartitionManager();
            _winbtrfsManager = new WinBTRFSManager();
            _systemSettingsManager = new SystemSettingsManager();

            // Hook up event handlers
            _usbFlashManager.ProgressUpdate += (s, e) => OnStatusChanged($"[{e.Status}] {e.Message}");
            _usbRestoreManager.ProgressUpdate += (s, e) => OnStatusChanged($"[{e.Status}] {e.Message}");
            _partitionManager.ProgressUpdate += (s, e) => OnStatusChanged($"[{e.Status}] {e.Message}");
            _winbtrfsManager.ProgressUpdate += (s, e) => OnStatusChanged($"[{e.Status}] {e.Message}");
            _isoManager.DownloadProgress += (s, e) => 
            {
                OnStatusChanged(
                    $"ISO Download: {e.BytesDownloaded / (1024.0 * 1024.0):F2}MB / {e.TotalBytes / (1024.0 * 1024.0):F2}MB ({e.ProgressPercentage}%)");
                DownloadProgress?.Invoke(this, e);
            };
        }

        /// <summary>
        /// Get list of available USB drives
        /// </summary>
        public List<USBInfo> GetUSBDrives()
        {
            return USBUtilities.GetUSBDrives();
        }

        /// <summary>
        /// Check if ISO is cached and valid
        /// </summary>
        public async Task<bool> IsISOCached()
        {
            return await _isoManager.CheckCachedISO();
        }

        /// <summary>
        /// Download ISO file
        /// </summary>
        public async Task<string> DownloadISO()
        {
            OnStatusChanged("Starting ISO download...");
            try
            {
                var result = await _isoManager.DownloadISO();
                OnStatusChanged("ISO download completed");
                return result;
            }
            catch (Exception ex)
            {
                OnStatusChanged($"ISO download failed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Cancel ISO download
        /// </summary>
        public void CancelISODownload()
        {
            _isoManager.CancelDownload();
            OnStatusChanged("ISO download cancelled");
        }

        /// <summary>
        /// Pause ISO download
        /// </summary>
        public void PauseISODownload()
        {
            _isoManager.PauseDownload();
            OnStatusChanged("ISO download paused");
        }

        /// <summary>
        /// Resume ISO download
        /// </summary>
        public void ResumeISODownload()
        {
            _isoManager.ResumeDownload();
            OnStatusChanged("ISO download resumed");
        }

        /// <summary>
        /// Get path to cached ISO
        /// </summary>
        public string GetISOPath()
        {
            return _isoManager.GetISOPath();
        }

        /// <summary>
        /// Flash USB drive with ISO
        /// </summary>
        public async Task<bool> FlashUSB(int diskNumber, string isoPath)
        {
            OnStatusChanged($"Starting USB flash to disk {diskNumber}...");
            try
            {
                var result = await _usbFlashManager.FlashUSB(diskNumber, isoPath);
                if (result)
                    OnStatusChanged("USB flash completed successfully");
                else
                    OnStatusChanged("USB flash failed");
                return result;
            }
            catch (Exception ex)
            {
                OnStatusChanged($"USB flash error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Cancel USB flash operation
        /// </summary>
        public void CancelUSBFlash()
        {
            _usbFlashManager.Cancel();
            OnStatusChanged("USB flash cancelled");
        }

        /// <summary>
        /// Get current C: drive size information
        /// </summary>
        public async Task<PartitionManager.DriveSizeInfo> GetCDriveSizeInfo()
        {
            return await _partitionManager.GetCDriveSizeInfo();
        }

        /// <summary>
        /// Resize C: partition
        /// </summary>
        public async Task<bool> ResizePartition(double targetFreeSpaceGB, bool autoOptimize = false)
        {
            OnStatusChanged($"Starting partition resize (target free space: {targetFreeSpaceGB}GB)...");
            try
            {
                var result = await _partitionManager.ResizePartition(targetFreeSpaceGB, autoOptimize);
                if (result)
                    OnStatusChanged("Partition resize completed");
                else
                    OnStatusChanged("Partition resize failed");
                return result;
            }
            catch (Exception ex)
            {
                OnStatusChanged($"Partition resize error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Install winbtrfs
        /// </summary>
        public async Task<bool> InstallWinBTRFS()
        {
            OnStatusChanged("Starting winbtrfs installation...");
            try
            {
                var result = await _winbtrfsManager.InstallWinBTRFS();
                if (result)
                    OnStatusChanged("winbtrfs installation completed");
                else
                    OnStatusChanged("winbtrfs installation failed");
                return result;
            }
            catch (Exception ex)
            {
                OnStatusChanged($"winbtrfs installation error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Restore USB drive back to normal Windows USB format
        /// </summary>
        public async Task<bool> RestoreUSB(int diskNumber)
        {
            OnStatusChanged($"Starting USB restore for disk {diskNumber}...");
            try
            {
                var result = await _usbRestoreManager.RestoreUSB(diskNumber);
                if (result)
                    OnStatusChanged("USB restore completed successfully");
                else
                    OnStatusChanged("USB restore failed");
                return result;
            }
            catch (Exception ex)
            {
                OnStatusChanged($"USB restore error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Set system hardware clock to UTC (required for Linux dual-boot)
        /// </summary>
        public async Task<bool> SetTimeToUTC()
        {
            OnStatusChanged("Setting hardware clock to UTC...");
            try
            {
                var result = await _systemSettingsManager.SetTimeToUTC();
                if (result)
                    OnStatusChanged("Hardware clock set to UTC");
                else
                    OnStatusChanged("Failed to set hardware clock to UTC");
                return result;
            }
            catch (Exception ex)
            {
                OnStatusChanged($"UTC time setting error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Disable Windows Fast Startup (prevents Linux partition access issues)
        /// </summary>
        public async Task<bool> DisableFastStartup()
        {
            OnStatusChanged("Disabling Fast Startup...");
            try
            {
                var result = await _systemSettingsManager.DisableFastStartup();
                if (result)
                    OnStatusChanged("Fast Startup disabled");
                else
                    OnStatusChanged("Failed to disable Fast Startup");
                return result;
            }
            catch (Exception ex)
            {
                OnStatusChanged($"Fast Startup disable error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Reboot computer into UEFI firmware settings
        /// </summary>
        public bool RebootToUEFI()
        {
            OnStatusChanged("Initiating reboot to UEFI...");
            return _systemSettingsManager.RebootToUEFI();
        }

        private void OnStatusChanged(string message)
        {
            StatusChanged?.Invoke(this, new EventArgs());
            // Can be extended to pass message or use a custom EventArgs
        }
    }

    /// <summary>
    /// Example usage class showing how to integrate PrepToolIntegration into UI
    /// </summary>
    public static class PrepToolUIHelper
    {
        public static async Task<List<USBInfo>> GetUSBDrivesForComboBox(PrepToolIntegration integration)
        {
            return await Task.Run(() => integration.GetUSBDrives());
        }

        public static void DisplayDriveInfo(ListBox listBox, List<USBInfo> drives)
        {
            listBox.Items.Clear();
            foreach (var drive in drives)
            {
                listBox.Items.Add($"Disk {drive.DiskNumber}: {drive.DisplayName} ({drive.SizeGB}GB)");
            }
        }

        public static void AddLogMessage(ListBox logBox, string message)
        {
            if (logBox.InvokeRequired)
            {
                logBox.Invoke(new Action(() => AddLogMessage(logBox, message)));
                return;
            }

            logBox.Items.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
            logBox.TopIndex = logBox.Items.Count - 1;
        }

        public static void ClearLog(ListBox logBox)
        {
            if (logBox.InvokeRequired)
            {
                logBox.Invoke(new Action(() => logBox.Items.Clear()));
                return;
            }

            logBox.Items.Clear();
        }
    }
}
