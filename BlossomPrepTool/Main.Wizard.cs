using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlossomPrepTool
{
    public partial class Main : Form
    {
        // Wizard State
        private enum WizardMode { None, Simple, DualBoot }
        private enum WizardStep { Welcome, ModeSelection, USBSelection, ISOSource, Download, Flash, Partition, Settings, WinBTRFS, Complete }
        private WizardMode _currentMode = WizardMode.None;
        private WizardStep _currentStep = WizardStep.Welcome;
        private bool _isDownloadPaused;
        private string _selectedISOPath;
        private bool _isRestoreMode;
        private bool _isFlashing;

        private void HideAllDesignerControls()
        {
            // Hide all controls from designer except window chrome
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl == close || ctrl == maximize_normalize || ctrl == minimize ||
                    ctrl == wizardWelcomeView || ctrl == wizardModeSelectionView || ctrl == wizardUsbSelectionView ||
                    ctrl == wizardIsoSourceView || ctrl == wizardDownloadView || 
                    ctrl == wizardFlashView || ctrl == wizardPartitionView || 
                    ctrl == wizardSettingsView || ctrl == wizardWinBTRFSView || ctrl == wizardCompleteView)
                {
                    continue;
                }

                if (ctrl.GetType().Name != "Panel")
                {
                    ctrl.Visible = false;
                }
            }
        }

        private void InitializeWizardPages()
        {
            wizardModeSelectionView.SimpleModeSelected += (s, e) => SelectMode(WizardMode.Simple);
            wizardModeSelectionView.DualBootModeSelected += (s, e) => SelectMode(WizardMode.DualBoot);

            wizardUsbSelectionView.RefreshClicked += (s, e) =>
            {
                wizardUsbSelectionView.RefreshButton.Enabled = false;
                var drives = _preptool.GetUSBDrives();
                _usbDiskMap.Clear();
                wizardUsbSelectionView.DriveComboBox.Items.Clear();
                foreach (var drive in drives)
                {
                    _usbDiskMap[wizardUsbSelectionView.DriveComboBox.Items.Count] = drive;
                    wizardUsbSelectionView.DriveComboBox.Items.Add($"Disk {drive.DiskNumber}: {drive.DisplayName} ({drive.SizeGB}GB)");
                }
                if (wizardUsbSelectionView.DriveComboBox.Items.Count > 0)
                    wizardUsbSelectionView.DriveComboBox.SelectedIndex = 0;
                wizardUsbSelectionView.RefreshButton.Enabled = true;
            };

            wizardUsbSelectionView.DriveComboBox.SelectedIndexChanged += (s, e) =>
            {
                if (wizardUsbSelectionView.DriveComboBox.SelectedIndex >= 0 && _usbDiskMap.ContainsKey(wizardUsbSelectionView.DriveComboBox.SelectedIndex))
                {
                    var drive = _usbDiskMap[wizardUsbSelectionView.DriveComboBox.SelectedIndex];
                    wizardUsbSelectionView.SelectedLabel.Text = $"Selected: Disk {drive.DiskNumber} ({drive.SizeGB}GB)";
                    wizardUsbSelectionView.SelectedLabel.ForeColor = SuccessColor;
                    wizardUsbSelectionView.NextButton.Enabled = true;
                }
                else
                {
                    wizardUsbSelectionView.SelectedLabel.Text = "No USB selected";
                    wizardUsbSelectionView.SelectedLabel.ForeColor = TextSecondary;
                    wizardUsbSelectionView.NextButton.Enabled = false;
                }
            };

            wizardUsbSelectionView.NextClicked += (s, e) =>
            {
                if (_currentMode == WizardMode.Simple)
                {
                    GoToStep(WizardStep.ISOSource);
                }
                else // DualBoot mode
                {
                    GoToStep(WizardStep.Partition);
                }
            };
            wizardUsbSelectionView.BackClicked += (s, e) => GoToStep(WizardStep.Welcome);

            wizardWelcomeView.ManualSetupClicked += (s, e) => SelectMode(WizardMode.Simple);
            wizardWelcomeView.GetStartedClicked += (s, e) => SelectMode(WizardMode.DualBoot);

            wizardIsoSourceView.DownloadClicked += (s, e) => GoToStep(WizardStep.Download);
            wizardIsoSourceView.UseOwnClicked += (s, e) => SelectOwnISO();
            wizardIsoSourceView.RestoreClicked += (s, e) => RestoreUSB();
            wizardIsoSourceView.BackClicked += (s, e) => GoToStep(WizardStep.USBSelection);

            wizardDownloadView.NextClicked += (s, e) => 
            {
                if (_currentMode == WizardMode.Simple)
                {
                    GoToStep(WizardStep.Flash);
                }
                else // DualBoot mode
                {
                    GoToStep(WizardStep.Flash);
                }
            };
            wizardDownloadView.PauseClicked += (s, e) => ToggleDownloadPause();
            wizardDownloadView.CancelClicked += (s, e) => CancelDownload();
            wizardDownloadView.BackClicked += (s, e) => GoToStep(_currentMode == WizardMode.Simple ? WizardStep.ISOSource : WizardStep.Partition);
            
            wizardPartitionView.NextClicked += (s, e) => ExecuteResizePartition();
            wizardPartitionView.BackClicked += (s, e) => GoToStep(WizardStep.USBSelection);
            
            wizardFlashView.StartClicked += (s, e) => ExecuteFlashUSB();
            wizardFlashView.BackClicked += (s, e) =>
            {
                GoToStep(WizardStep.Welcome);
                _isRestoreMode = false;
            };
            
            wizardSettingsView.NextClicked += (s, e) => ExecuteSystemSettings();
            wizardSettingsView.BackClicked += (s, e) => GoToStep(WizardStep.Flash);
            
            wizardWinBTRFSView.NextClicked += (s, e) => ExecuteInstallWinBTRFS();
            wizardWinBTRFSView.BackClicked += (s, e) => GoToStep(WizardStep.Partition);
            
            wizardCompleteView.FinishClicked += (s, e) =>
            {
                Application.Exit();
            };
            wizardCompleteView.RebootClicked += (s, e) =>
            {
                if (MessageBox.Show(Localizer.GetString("Message.RebootToUefi"), 
                    Localizer.GetString("MessageBox.ConfirmReboot"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _preptool.RebootToUEFI();
                }
            };

            wizardUsbSelectionView.NextButton.Enabled = false;
            wizardUsbSelectionView.RefreshButton.PerformClick();
        }

        private void SelectMode(WizardMode mode)
        {
            _currentMode = mode;
            GoToStep(WizardStep.USBSelection);
        }

        private void GoToStep(WizardStep step)
        {
            _currentStep = step;
            ShowStep(step);
        }

        private void ShowStep(WizardStep step)
        {
            // Hide all panels
            wizardWelcomeView.Visible = false;
            wizardModeSelectionView.Visible = false;
            wizardUsbSelectionView.Visible = false;
            wizardIsoSourceView.Visible = false;
            wizardDownloadView.Visible = false;
            wizardFlashView.Visible = false;
            wizardPartitionView.Visible = false;
            wizardSettingsView.Visible = false;
            wizardWinBTRFSView.Visible = false;
            wizardCompleteView.Visible = false;

            // Show current step
            switch (step)
            {
                case WizardStep.Welcome:
                    wizardWelcomeView.Visible = true;
                    break;
                case WizardStep.ModeSelection:
                    wizardModeSelectionView.Visible = true;
                    break;
                case WizardStep.USBSelection:
                    wizardUsbSelectionView.Visible = true;
                    break;
                case WizardStep.ISOSource:
                    wizardIsoSourceView.Visible = true;
                    break;
                case WizardStep.Partition:
                    wizardPartitionView.Visible = true;
                    wizardPartitionView.BackButton.Visible = (_currentMode != WizardMode.DualBoot);
                    LoadPartitionInfo();
                    break;
                case WizardStep.Download:
                    wizardDownloadView.Visible = true;
                    ExecuteDownloadISO(); // Auto-start download
                    break;
                case WizardStep.Flash:
                    wizardFlashView.Visible = true;
                    // Control button visibility and text based on restore mode
                    if (_isRestoreMode)
                    {
                        wizardFlashView.StartButton.Visible = false;
                        wizardFlashView.BackButton.Enabled = false;
                        wizardFlashView.TitleLabel.Text = "Restore USB Drive";
                        wizardFlashView.DescriptionLabel.Text = "Restoring your USB drive to normal Windows format. All data will be erased.";
                    }
                    else
                    {
                        wizardFlashView.StartButton.Visible = true;
                        wizardFlashView.BackButton.Enabled = true;
                        wizardFlashView.TitleLabel.Text = "Flash USB Drive";
                        wizardFlashView.DescriptionLabel.Text = "This will write the ISO to your USB drive. All data will be erased.";
                    }
                    break;
                case WizardStep.Settings:
                    wizardSettingsView.Visible = true;
                    wizardSettingsView.BackButton.Visible = (_currentMode != WizardMode.DualBoot);
                    break;
                case WizardStep.WinBTRFS:
                    wizardWinBTRFSView.Visible = true;
                    wizardWinBTRFSView.BackButton.Visible = (_currentMode != WizardMode.DualBoot);
                    break;
                case WizardStep.Complete:
                    wizardCompleteView.Visible = true;
                    // Hide reboot button in Simple mode
                    wizardCompleteView.RebootButton.Visible = (_currentMode == WizardMode.DualBoot);
                    break;
            }
        }

        private async void ExecuteDownloadISO()
        {
            wizardDownloadView.StatusLabel.Text = "⠋ Checking for cached ISO...";
            wizardDownloadView.StatusLabel.ForeColor = WarningColor;
            wizardDownloadView.ProgressBar.Value = 0;
            wizardDownloadView.NextButton.Enabled = false;
            wizardDownloadView.PauseButton.Enabled = false;
            wizardDownloadView.CancelButton.Enabled = true;
            wizardDownloadView.BackButton.Enabled = false;
            wizardDownloadView.PauseButton.Text = "Pause";
            _isDownloadPaused = false;
            StartSpinner(wizardDownloadView.StatusLabel);

            _cancellationTokenSource = new CancellationTokenSource();

            // Subscribe to download progress events
            EventHandler<ISODownloadProgressEventArgs> progressHandler = (s, e) =>
            {
                this.Invoke(new Action(() =>
                {
                    wizardDownloadView.ProgressBar.Value = e.ProgressPercentage;
                    wizardDownloadView.StatusLabel.Text =
                        $"{_spinnerFrames[_spinnerFrame]} Downloading: {e.BytesDownloaded / (1024.0 * 1024.0):F2}MB / {e.TotalBytes / (1024.0 * 1024.0):F2}MB ({e.ProgressPercentage}%)";
                }));
            };

            _preptool.DownloadProgress += progressHandler;

            try
            {
                await Task.Run(async () =>
                {
                    // Check if ISO is already cached and valid
                    var isCached = await _preptool.IsISOCached();

                    if (isCached)
                    {
                        this.Invoke(new Action(() =>
                        {
                            StopSpinner();
                            wizardDownloadView.ProgressBar.Value = 100;
                            wizardDownloadView.StatusLabel.Text = "✓ Cached ISO verified successfully!";
                            wizardDownloadView.StatusLabel.ForeColor = SuccessColor;
                            wizardDownloadView.NextButton.Enabled = true;
                            wizardDownloadView.PauseButton.Enabled = false;
                            wizardDownloadView.CancelButton.Enabled = false;
                            wizardDownloadView.BackButton.Enabled = true;
                            wizardDownloadView.PauseButton.Text = "Pause";
                            _isDownloadPaused = false;
                            _completedSteps++;
                            LogMessage("Using cached ISO");
                        }));
                        return;
                    }

                    // ISO not cached, proceed with download
                    this.Invoke(new Action(() =>
                    {
                        wizardDownloadView.StatusLabel.Text = "⠋ Starting download...";
                        wizardDownloadView.StatusLabel.ForeColor = WarningColor;
                        wizardDownloadView.ProgressBar.Value = 0;
                        wizardDownloadView.PauseButton.Enabled = true;
                        StartSpinner(wizardDownloadView.StatusLabel);
                    }));

                    var isoPath = await _preptool.DownloadISO();

                    this.Invoke(new Action(() =>
                    {
                        StopSpinner();
                        wizardDownloadView.ProgressBar.Value = 100;
                        wizardDownloadView.StatusLabel.Text = "✓ ISO downloaded successfully!";
                        wizardDownloadView.StatusLabel.ForeColor = SuccessColor;
                        wizardDownloadView.NextButton.Enabled = true;
                        wizardDownloadView.PauseButton.Enabled = false;
                        wizardDownloadView.CancelButton.Enabled = false;
                        wizardDownloadView.BackButton.Enabled = true;
                        wizardDownloadView.PauseButton.Text = "Pause";
                        _isDownloadPaused = false;
                        _completedSteps++;
                        LogMessage($"ISO: {isoPath}");
                    }));
                }, _cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                this.Invoke(new Action(() =>
                {
                    StopSpinner();
                    wizardDownloadView.StatusLabel.Text = "⊘ Download cancelled";
                    wizardDownloadView.StatusLabel.ForeColor = TextSecondary;
                    wizardDownloadView.NextButton.Enabled = false;
                    wizardDownloadView.PauseButton.Enabled = false;
                    wizardDownloadView.CancelButton.Enabled = false;
                    wizardDownloadView.BackButton.Enabled = true;
                    wizardDownloadView.PauseButton.Text = "Pause";
                    _isDownloadPaused = false;
                }));
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    StopSpinner();
                    wizardDownloadView.StatusLabel.Text = $"✗ Error: {ex.Message}";
                    wizardDownloadView.StatusLabel.ForeColor = ErrorColor;
                    wizardDownloadView.NextButton.Enabled = false;
                    wizardDownloadView.PauseButton.Enabled = false;
                    wizardDownloadView.CancelButton.Enabled = false;
                    wizardDownloadView.BackButton.Enabled = true;
                    wizardDownloadView.PauseButton.Text = "Pause";
                    _isDownloadPaused = false;
                    LogMessage($"Download failed: {ex.Message}");
                }));
            }
            finally
            {
                // Unsubscribe from progress events
                _preptool.DownloadProgress -= progressHandler;
            }
        }

        private void ToggleDownloadPause()
        {
            if (_isDownloadPaused)
            {
                _preptool.ResumeISODownload();
                _isDownloadPaused = false;
                wizardDownloadView.PauseButton.Text = "Pause";
                wizardDownloadView.StatusLabel.Text = "⠋ Resuming download...";
                wizardDownloadView.StatusLabel.ForeColor = WarningColor;
                StartSpinner(wizardDownloadView.StatusLabel);
            }
            else
            {
                _preptool.PauseISODownload();
                _isDownloadPaused = true;
                wizardDownloadView.PauseButton.Text = "Resume";
                wizardDownloadView.StatusLabel.Text = "⏸ Download paused";
                wizardDownloadView.StatusLabel.ForeColor = TextSecondary;
                StopSpinner();
            }
        }

        private void CancelDownload()
        {
            _preptool.CancelISODownload();
            wizardDownloadView.PauseButton.Enabled = false;
            wizardDownloadView.CancelButton.Enabled = false;
            wizardDownloadView.NextButton.Enabled = false;
            wizardDownloadView.BackButton.Enabled = true;
            wizardDownloadView.PauseButton.Text = "Pause";
            _isDownloadPaused = false;
        }

        private void SelectOwnISO()
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "ISO files (*.iso)|*.iso|All files (*.*)|*.*";
                openFileDialog.Title = "Select BlossomOS ISO file";
                openFileDialog.CheckFileExists = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _selectedISOPath = openFileDialog.FileName;

                    // Verify it's a valid file
                    var fileInfo = new System.IO.FileInfo(_selectedISOPath);
                    if (fileInfo.Length < 1024 * 1024 * 100) // Less than 100MB is suspicious
                    {
                        var result = MessageBox.Show(
                            "The selected file seems unusually small for a BlossomOS ISO. Are you sure this is the correct file?",
                            "Confirm ISO File",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning);

                        if (result == DialogResult.No)
                        {
                            _selectedISOPath = null;
                            return;
                        }
                    }

                    LogMessage($"User selected ISO: {_selectedISOPath}");

                    // Skip directly to flash step
                    GoToStep(WizardStep.Flash);
                }
            }
        }

        private async void RestoreUSB()
        {
            if (wizardUsbSelectionView.DriveComboBox.SelectedIndex < 0)
            {
                MessageBox.Show(Localizer.GetString("Message.NoUsbSelected"), Localizer.GetString("MessageBox.NoUsbSelected"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                GoToStep(WizardStep.USBSelection);
                return;
            }

            var selectedDrive = _usbDiskMap[wizardUsbSelectionView.DriveComboBox.SelectedIndex];

            var confirmResult = MessageBox.Show(
                $"This will restore Disk {selectedDrive.DiskNumber} to a normal Windows USB drive.\n\n" +
                $"All data on the drive will be erased and it will be formatted as FAT32.\n\n" +
                $"Continue?",
                "Confirm USB Restore",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmResult != DialogResult.Yes)
            {
                return;
            }

            // Use flash view to show restore progress
            _isRestoreMode = true;
            GoToStep(WizardStep.Flash);

            // Disable UI during restore
            this.ControlBox = false;

            wizardFlashView.StatusLabel.Text = "⠋ Restoring USB drive...";
            wizardFlashView.StatusLabel.ForeColor = WarningColor;
            StartSpinner(wizardFlashView.StatusLabel);

            try
            {
                var result = await _preptool.RestoreUSB(selectedDrive.DiskNumber);

                this.Invoke(new Action(() =>
                {
                    StopSpinner();
                    if (result)
                    {
                        wizardFlashView.StatusLabel.Text = "✓ USB drive restored successfully!";
                        wizardFlashView.StatusLabel.ForeColor = SuccessColor;
                        LogMessage($"USB Disk {selectedDrive.DiskNumber}: Restored to Windows USB");

                        // Enable back button, re-enable form controls, and reset mode
                        wizardFlashView.BackButton.Enabled = true;
                        this.ControlBox = true;
                        _isRestoreMode = false;
                    }
                    else
                    {
                        wizardFlashView.StatusLabel.Text = "✗ USB restore failed";
                        wizardFlashView.StatusLabel.ForeColor = ErrorColor;
                        LogMessage("USB restore failed");

                        // Enable back button, re-enable form controls, and reset mode
                        wizardFlashView.BackButton.Enabled = true;
                        this.ControlBox = true;
                        _isRestoreMode = false;
                    }
                }));
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    StopSpinner();
                    wizardFlashView.StatusLabel.Text = $"✗ Error: {ex.Message}";
                    wizardFlashView.StatusLabel.ForeColor = ErrorColor;
                    LogMessage($"USB restore error: {ex.Message}");

                    // Enable back button, re-enable form controls, and reset mode
                    wizardFlashView.BackButton.Enabled = true;
                    this.ControlBox = true;
                    _isRestoreMode = false;
                }));
            }
        }

        private async void ExecuteFlashUSB()
        {
            if (wizardUsbSelectionView.DriveComboBox.SelectedIndex < 0)
            {
                MessageBox.Show(Localizer.GetString("Message.NoUsbSelected"), Localizer.GetString("MessageBox.NoUsbSelected"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedDrive = _usbDiskMap[wizardUsbSelectionView.DriveComboBox.SelectedIndex];

            // Use selected ISO if available, otherwise use downloaded ISO
            var isoPath = !string.IsNullOrEmpty(_selectedISOPath) ? _selectedISOPath : _preptool.GetISOPath();

            if (!System.IO.File.Exists(isoPath))
            {
                MessageBox.Show(Localizer.GetString("Message.IsoNotFound"), Localizer.GetString("MessageBox.IsoNotFound"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show(Localizer.GetString("Message.EraseUsbWarning", selectedDrive.DiskNumber),
                Localizer.GetString("MessageBox.ConfirmUsbFlash"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            // Disable UI during flashing
            _isFlashing = true;
            wizardFlashView.BackButton.Enabled = false;
            wizardFlashView.StartButton.Enabled = false;
            this.ControlBox = false;

            wizardFlashView.StatusLabel.Text = "⠋ Flashing USB...";
            wizardFlashView.StatusLabel.ForeColor = WarningColor;
            StartSpinner(wizardFlashView.StatusLabel);

            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                await Task.Run(async () =>
                {
                    var result = await _preptool.FlashUSB(selectedDrive.DiskNumber, isoPath);
                    this.Invoke(new Action(() =>
                    {
                        StopSpinner();
                        _isFlashing = false;
                        this.ControlBox = true;
                        if (result)
                        {
                            wizardFlashView.StatusLabel.Text = "✓ USB flashed successfully!";
                            wizardFlashView.StatusLabel.ForeColor = SuccessColor;
                            _completedSteps++;
                            LogMessage($"USB Disk {selectedDrive.DiskNumber}: Flash complete");
                        }
                        else
                        {
                            wizardFlashView.StatusLabel.Text = "✗ Flash operation failed";
                            wizardFlashView.StatusLabel.ForeColor = ErrorColor;
                            wizardFlashView.BackButton.Enabled = true;
                            wizardFlashView.StartButton.Enabled = true;
                        }
                    }));

                    // Auto-advance to next step after successful flash
                    if (result)
                    {
                        await Task.Delay(1000);
                        this.Invoke(new Action(() =>
                        {
                            if (_currentMode == WizardMode.DualBoot)
                            {
                                GoToStep(WizardStep.Settings);
                            }
                            else
                            {
                                GoToStep(WizardStep.Complete);
                            }
                        }));
                    }
                }, _cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                this.Invoke(new Action(() =>
                {
                    StopSpinner();
                    _isFlashing = false;
                    this.ControlBox = true;
                    wizardFlashView.BackButton.Enabled = true;
                    wizardFlashView.StartButton.Enabled = true;
                    wizardFlashView.StatusLabel.Text = "⊘ Flash cancelled";
                    wizardFlashView.StatusLabel.ForeColor = TextSecondary;
                }));
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    StopSpinner();
                    _isFlashing = false;
                    this.ControlBox = true;
                    wizardFlashView.BackButton.Enabled = true;
                    wizardFlashView.StartButton.Enabled = true;
                    wizardFlashView.StatusLabel.Text = $"✗ Error: {ex.Message}";
                    wizardFlashView.StatusLabel.ForeColor = ErrorColor;
                    LogMessage($"Flash failed: {ex.Message}");
                }));
            }
        }

        private async void LoadPartitionInfo()
        {
            try
            {
                var sizeInfo = await _preptool.GetCDriveSizeInfo();
                wizardPartitionView.DiskInfoLabel.Text = 
                    $"C: Drive: {sizeInfo.TotalSizeGB:F1} GB total, {sizeInfo.UsedSpaceGB:F1} GB used, {sizeInfo.FreeSpaceGB:F1} GB free";
                wizardPartitionView.DiskInfoLabel.ForeColor = SuccessColor;
                
                // Set default allocation to 50GB or 20% of free space, whichever is larger
                var recommendedGB = Math.Max(50, (int)(sizeInfo.FreeSpaceGB * 0.2));
                wizardPartitionView.AllocateTextBox.Text = recommendedGB.ToString();
            }
            catch (Exception ex)
            {
                wizardPartitionView.DiskInfoLabel.Text = $"Error loading disk info: {ex.Message}";
                wizardPartitionView.DiskInfoLabel.ForeColor = ErrorColor;
            }
        }

        private async void ExecuteResizePartition()
        {
            if (!double.TryParse(wizardPartitionView.AllocateTextBox.Text, out double allocateGB) || allocateGB < 20)
            {
                MessageBox.Show(Localizer.GetString("Message.InvalidPartitionSize"), Localizer.GetString("MessageBox.InvalidSize"), 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            wizardPartitionView.StatusLabel.Text = "⠋ Resizing partition...";
            wizardPartitionView.StatusLabel.ForeColor = WarningColor;
            wizardPartitionView.NextButton.Enabled = false;
            wizardPartitionView.BackButton.Enabled = false;
            StartSpinner(wizardPartitionView.StatusLabel);

            try
            {
                var result = await _preptool.ResizePartition(allocateGB);
                
                this.Invoke(new Action(() =>
                {
                    StopSpinner();
                    if (result)
                    {
                        wizardPartitionView.StatusLabel.Text = "✓ Partition resized successfully!";
                        wizardPartitionView.StatusLabel.ForeColor = SuccessColor;
                        LogMessage($"Partition resized: {allocateGB}GB allocated for BlossomOS");
                        
                        Task.Delay(1000).ContinueWith(_ =>
                        {
                            this.Invoke(new Action(() => GoToStep(WizardStep.Download)));
                        });
                    }
                    else
                    {
                        wizardPartitionView.StatusLabel.Text = "✗ Partition resize failed";
                        wizardPartitionView.StatusLabel.ForeColor = ErrorColor;
                        wizardPartitionView.NextButton.Enabled = true;
                        wizardPartitionView.BackButton.Enabled = true;
                    }
                }));
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    StopSpinner();
                    wizardPartitionView.StatusLabel.Text = $"✗ Error: {ex.Message}";
                    wizardPartitionView.StatusLabel.ForeColor = ErrorColor;
                    wizardPartitionView.NextButton.Enabled = true;
                    wizardPartitionView.BackButton.Enabled = true;
                    LogMessage($"Partition resize error: {ex.Message}");
                }));
            }
        }

        private async void ExecuteSystemSettings()
        {
            wizardSettingsView.StatusLabel.Text = "⠋ Applying system settings...";
            wizardSettingsView.StatusLabel.ForeColor = WarningColor;
            wizardSettingsView.NextButton.Enabled = false;
            wizardSettingsView.BackButton.Enabled = false;
            StartSpinner(wizardSettingsView.StatusLabel);

            try
            {
                var utcResult = await _preptool.SetTimeToUTC();
                var fastStartupResult = await _preptool.DisableFastStartup();

                this.Invoke(new Action(() =>
                {
                    StopSpinner();
                    if (utcResult && fastStartupResult)
                    {
                        wizardSettingsView.StatusLabel.Text = "✓ System settings configured successfully!";
                        wizardSettingsView.StatusLabel.ForeColor = SuccessColor;
                        LogMessage("System settings: UTC time enabled, Fast Startup disabled");
                        
                        Task.Delay(1000).ContinueWith(_ =>
                        {
                            this.Invoke(new Action(() => GoToStep(WizardStep.WinBTRFS)));
                        });
                    }
                    else
                    {
                        wizardSettingsView.StatusLabel.Text = "⚠ Some settings could not be applied";
                        wizardSettingsView.StatusLabel.ForeColor = WarningColor;
                        wizardSettingsView.NextButton.Enabled = true;
                        wizardSettingsView.BackButton.Enabled = true;
                    }
                }));
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    StopSpinner();
                    wizardSettingsView.StatusLabel.Text = $"✗ Error: {ex.Message}";
                    wizardSettingsView.StatusLabel.ForeColor = ErrorColor;
                    wizardSettingsView.NextButton.Enabled = true;
                    wizardSettingsView.BackButton.Enabled = true;
                    LogMessage($"System settings error: {ex.Message}");
                }));
            }
        }

        private async void ExecuteInstallWinBTRFS()
        {
            wizardWinBTRFSView.StatusLabel.Text = "⠋ Installing WinBtrfs...";
            wizardWinBTRFSView.StatusLabel.ForeColor = WarningColor;
            wizardWinBTRFSView.NextButton.Enabled = false;
            wizardWinBTRFSView.BackButton.Enabled = false;
            StartSpinner(wizardWinBTRFSView.StatusLabel);

            try
            {
                var result = await _preptool.InstallWinBTRFS();

                this.Invoke(new Action(() =>
                {
                    StopSpinner();
                    if (result)
                    {
                        wizardWinBTRFSView.StatusLabel.Text = "✓ WinBtrfs installed successfully!";
                        wizardWinBTRFSView.StatusLabel.ForeColor = SuccessColor;
                        LogMessage("WinBtrfs installation completed");
                        
                        Task.Delay(1000).ContinueWith(_ =>
                        {
                            this.Invoke(new Action(() => GoToStep(WizardStep.Complete)));
                        });
                    }
                    else
                    {
                        wizardWinBTRFSView.StatusLabel.Text = "✗ WinBtrfs installation failed";
                        wizardWinBTRFSView.StatusLabel.ForeColor = ErrorColor;
                        wizardWinBTRFSView.NextButton.Enabled = true;
                        wizardWinBTRFSView.BackButton.Enabled = true;
                    }
                }));
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    StopSpinner();
                    wizardWinBTRFSView.StatusLabel.Text = $"✗ Error: {ex.Message}";
                    wizardWinBTRFSView.StatusLabel.ForeColor = ErrorColor;
                    wizardWinBTRFSView.NextButton.Enabled = true;
                    wizardWinBTRFSView.BackButton.Enabled = true;
                    LogMessage($"WinBtrfs installation error: {ex.Message}");
                }));
            }
        }
    }
}
