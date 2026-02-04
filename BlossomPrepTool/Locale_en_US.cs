using System;
using System.Collections.Generic;

namespace BlossomPrepTool
{
    internal static class Locale_en_US
    {
        public static readonly IReadOnlyDictionary<string, string> Strings =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["WizardWelcome.Title"] = "BlossomOS Switch",
                ["WizardWelcome.Label"] = "WELCOME",
                ["WizardWelcome.Description"] = @"Prepare your system for installing BlossomOS with this easy-to-use tool.
Follow the steps to ensure a smooth installation process.",
                ["WizardWelcome.ManualSetupTitle"] = @"Install the BlossomOS recovery environment
onto an existing USB drive",
                ["WizardWelcome.ManualSetupDesc"] = "Quick USB setup only",
                ["WizardWelcome.ManualSetupButton"] = "Manual setup",
                ["WizardWelcome.GetStartedTitle"] = @"Prepare your system and install the recovery
environment onto your USB drive.",
                ["WizardWelcome.GetStartedDesc"] = "Complete setup with system configuration",
                ["WizardWelcome.GetStartedButton"] = "Get started",

                ["WizardModeSelection.Title"] = "Choose Setup Mode",
                ["WizardModeSelection.SimpleMode"] = "Just Flash USB",
                ["WizardModeSelection.DualBootMode"] = "Dual-Boot Setup",

                ["WizardIsoSource.Title"] = "What would you like to do?",
                ["WizardIsoSource.DownloadTitle"] = "Download BlossomOS Image",
                ["WizardIsoSource.DownloadDesc"] = @"Download the latest BlossomOS recovery
environment image from our servers",
                ["WizardIsoSource.DownloadButton"] = "Download Image",
                ["WizardIsoSource.UseOwnTitle"] = "Use my own image",
                ["WizardIsoSource.UseOwnDesc"] = @"I already have a BlossomOS image file
on my computer",
                ["WizardIsoSource.UseOwnButton"] = "Select image file",
                ["WizardIsoSource.RestoreTitle"] = "Restore USB drive",
                ["WizardIsoSource.RestoreDesc"] = @"Revert a BlossomOS USB back to
a normal Windows USB drive",
                ["WizardIsoSource.RestoreButton"] = "Restore USB",
                ["WizardIsoSource.BackButton"] = "← Back",

                ["WizardUsbSelection.Title"] = "Select USB Drive",
                ["WizardUsbSelection.DriveLabel"] = "Choose your USB drive:",
                ["WizardUsbSelection.RefreshButton"] = "⟳ Refresh",
                ["WizardUsbSelection.NoUsbSelected"] = "No USB selected",
                ["WizardUsbSelection.ContinueButton"] = "Continue →",
                ["WizardUsbSelection.BackButton"] = "← Back",

                ["WizardPartition.Title"] = "Allocate Space",
                ["WizardPartition.Description"] = "Choose how much space to allocate for BlossomOS from your C: drive.",
                ["WizardPartition.AllocateLabel"] = "Space for BlossomOS:",
                ["WizardPartition.DefaultSize"] = "50",
                ["WizardPartition.GBLabel"] = "GB",
                ["WizardPartition.Status"] = "Minimum recommended: 50 GB",
                ["WizardPartition.NextButton"] = "Next →",
                ["WizardPartition.BackButton"] = "← Back",

                ["WizardFlash.Title"] = "Flash USB Drive",
                ["WizardFlash.Description"] = "This will write the image to your USB drive. All data will be erased.",
                ["WizardFlash.Status"] = "Ready",

                ["WizardSettings.Title"] = "System Settings",
                ["WizardSettings.Description"] = @"Configuring system settings for dual-boot compatibility:

• Setting hardware clock to UTC (required for Linux)
• Disabling Fast Startup (prevents partition access issues)",
                ["WizardSettings.Status"] = "Ready to configure settings",
                ["WizardSettings.ApplyButton"] = "Apply Settings",
                ["WizardSettings.BackButton"] = "← Back",

                ["WizardWinBTRFS.Title"] = "Install WinBtrfs",
                ["WizardWinBTRFS.Description"] = @"WinBtrfs is required to access BlossomOS partitions from Windows.

This will install the file system driver that allows Windows to read and write Btrfs partitions.",
                ["WizardWinBTRFS.Status"] = "Ready to install WinBtrfs",
                ["WizardWinBTRFS.InstallButton"] = "Install",
                ["WizardWinBTRFS.BackButton"] = "← Back",

                ["WizardDownload.Title"] = "Download Image",
                ["WizardDownload.Status"] = "Ready to download",

                ["WizardComplete.Title"] = "Setup Complete!",

                ["Common.BackButton"] = "← Back",
                ["Common.NextButton"] = "Next →",

                ["MessageBox.ConfirmReboot"] = "Confirm Reboot",
                ["MessageBox.NoUsbSelected"] = "No Drive Selected",
                ["MessageBox.IsoNotFound"] = "Image Not Found",
                ["MessageBox.ConfirmUsbFlash"] = "Confirm USB Flash",
                ["MessageBox.InvalidSize"] = "Invalid Size",
                ["MessageBox.ConfirmPartitionResize"] = "Confirm Partition Resize",
                ["MessageBox.ConfirmInstallation"] = "Confirm Installation",

                ["Message.RebootToUefi"] = "This will reboot your computer into UEFI firmware settings. Continue?",
                ["Message.EraseUsbWarning"] = "This will erase all data on Disk {0}. Continue?",
                ["Message.NoUsbSelected"] = "Please select a USB drive first",
                ["Message.IsoNotFound"] = "Image file not found. Please download or select an image first.",
                ["Message.IsoNotFoundSimple"] = "Image file not found. Please download it first.",
                ["Message.InvalidPartitionSize"] = "Please enter a valid size (minimum 20 GB recommended)",
                ["Message.PartitionResizeWarning"] = "This will resize your C: drive partition. Make sure you have a backup of your important data. Continue?",
                ["Message.InstallWinBtrfs"] = @"This will install winbtrfs via Chocolatey.
Continue?",

                ["Button.Pause"] = "Pause",
                ["Button.Cancel"] = "Cancel",
                ["Button.StartFlash"] = "Start Flash",
                ["Button.Finish"] = "Finish",
                ["Button.RebootToUEFI"] = "Reboot to UEFI",
                ["Button.Refresh"] = "⟳ Refresh",
                ["Button.Install"] = "Install",
                ["Button.ApplySettings"] = "Apply Settings",

                ["Status.Processing"] = "Processing...",
                ["Status.CheckingCache"] = "⠋ Checking for cached image...",
                ["Status.CacheVerified"] = "✓ Cached image verified successfully!",
                ["Status.UsingCached"] = "Using cached image",

                ["WizardComplete.QRMessage"] = @"Scan with your mobile device
for video tutorial",
                ["WizardComplete.KeepUSB"] = "💡 Important: Keep your USB drive safe - it can be used to reinstall or recover BlossomOS.",
                ["WizardComplete.Message"] = "Your USB drive is ready to boot from!",

                ["Status.StartingDownload"] = "⠋ Starting download...",
                ["Status.DownloadSuccess"] = "✓ Image downloaded successfully!",
                ["Status.DownloadCancelled"] = "⊘ Download cancelled",
                ["Status.ResumeDownload"] = "⠋ Resuming download...",
                ["Status.DownloadPaused"] = "⏸ Download paused",

                ["Status.RestoringUSB"] = "⠋ Restoring USB drive...",
                ["Status.RestoreSuccess"] = "✓ USB drive restored successfully!",
                ["Status.RestoreFailed"] = "✗ USB restore failed",
                ["Status.FlashingUSB"] = "⠋ Flashing USB...",
                ["Status.FlashSuccess"] = "✓ USB flashed successfully!",
                ["Status.FlashFailed"] = "✗ Flash operation failed",
                ["Status.FlashCancelled"] = "⊘ Flash cancelled",

                ["Message.RestoreUSB"] = "Restoring your USB drive to normal Windows format. All data will be erased.",
                ["WizardFlash.RestoreTitle"] = "Restore USB Drive",

                ["Status.ResizingPartition"] = "⠋ Resizing partition...",

                ["Button.Resume"] = "Resume",

                ["Main.DownloadISO"] = "Download Image",
                ["Main.FlashUSB"] = "Flash USB",
                ["Main.ResizePartition"] = "Resize Partition",
                ["Main.InstallWinBTRFS"] = "Install WinBTRFS",
                ["Main.Refresh"] = "Refresh",
                ["Main.ClearLog"] = "Clear Log",

                ["Main.USBDrives"] = "USB Drives:",
                ["Main.PartitionSize"] = "Size (GB):",
                ["Main.Ready"] = "Ready",
                ["Main.Title"] = "BlossomPrep Tool",

                ["Form.SelectLanguage"] = "Select Language",

                ["WizardPartition.DiskInfo"] = "C: Drive: {0:F1} GB total, {1:F1} GB used, {2:F1} GB free",
                ["WizardUSBSelection.Selected"] = "Selected: Disk {0} ({1}GB)",
                ["WizardUSBSelection.NoUSB"] = "No USB selected",

                ["Status.PartitionResizeSuccess"] = "✓ Partition resized successfully!",
                ["Status.PartitionResizeFailed"] = "✗ Partition resize failed",

                ["Status.ApplyingSettings"] = "⠋ Applying system settings...",
                ["Status.SettingsSuccess"] = "✓ System settings configured successfully!",
                ["Status.SettingsPartialFail"] = "⚠ Some settings could not be applied",

                ["Status.InstallingWinBTRFS"] = "⠋ Installing WinBtrfs...",
                ["Status.WinBTRFSSuccess"] = "✓ WinBtrfs installed successfully!",
                ["Status.WinBTRFSFailed"] = "✗ WinBtrfs installation failed"
            };
    }
}
