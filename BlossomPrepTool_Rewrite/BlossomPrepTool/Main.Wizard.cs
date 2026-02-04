using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlossomPrepTool
{
    public partial class Main : Form
    {
        // Wizard State
        private enum WizardMode { None, Simple, DualBoot }
        private enum WizardStep { Welcome, ModeSelection, USBSelection, Download, Flash, Partition, WinBTRFS, Complete }
        private WizardMode _currentMode = WizardMode.None;
        private WizardStep _currentStep = WizardStep.Welcome;
        private bool _isDownloadPaused;

        private void HideAllDesignerControls()
        {
            // Hide all controls from designer except window chrome
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl == close || ctrl == maximize_normalize || ctrl == minimize ||
                    ctrl == wizardModeSelectionView || ctrl == wizardUsbSelectionView ||
                    ctrl == wizardDownloadView || ctrl == wizardFlashView ||
                    ctrl == wizardCompleteView)
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

            wizardUsbSelectionView.NextClicked += (s, e) => GoToStep(WizardStep.Download);
            wizardUsbSelectionView.BackClicked += (s, e) => GoToStep(WizardStep.Welcome);

            wizardWelcomeView.ManualSetupClicked += (s, e) => SelectMode(WizardMode.Simple);
            wizardWelcomeView.GetStartedClicked += (s, e) => SelectMode(WizardMode.DualBoot);

            wizardDownloadView.NextClicked += (s, e) => GoToStep(WizardStep.Flash);
            wizardDownloadView.PauseClicked += (s, e) => ToggleDownloadPause();
            wizardDownloadView.CancelClicked += (s, e) => CancelDownload();
            wizardDownloadView.BackClicked += (s, e) => GoToStep(WizardStep.USBSelection);
            wizardFlashView.StartClicked += (s, e) => ExecuteFlashUSB();
            wizardFlashView.BackClicked += (s, e) => GoToStep(WizardStep.Download);
            wizardCompleteView.FinishClicked += (s, e) => this.Close();

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
            wizardModeSelectionView.Visible = false;
            wizardUsbSelectionView.Visible = false;
            wizardDownloadView.Visible = false;
            wizardFlashView.Visible = false;
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
                case WizardStep.Download:
                    wizardDownloadView.Visible = true;
                    ExecuteDownloadISO(); // Auto-start download
                    break;
                case WizardStep.Flash:
                    wizardFlashView.Visible = true;
                    break;
                case WizardStep.Complete:
                    wizardCompleteView.Visible = true;
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

        private async void ExecuteFlashUSB()
        {
            if (wizardUsbSelectionView.DriveComboBox.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a USB drive", "No Drive Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedDrive = _usbDiskMap[wizardUsbSelectionView.DriveComboBox.SelectedIndex];
            var isoPath = _preptool.GetISOPath();

            if (!System.IO.File.Exists(isoPath))
            {
                MessageBox.Show("ISO file not found. Please download it first.", "ISO Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"This will erase all data on Disk {selectedDrive.DiskNumber}. Continue?",
                "Confirm USB Flash", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

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
                        if (result)
                        {
                            wizardFlashView.StatusLabel.Text = "✓ USB flashed successfully!";
                            wizardFlashView.StatusLabel.ForeColor = SuccessColor;
                            _completedSteps++;
                            LogMessage($"USB Disk {selectedDrive.DiskNumber}: Flash complete");

                            // Auto-advance to complete
                            Task.Delay(1000).ContinueWith(_ => GoToStep(WizardStep.Complete));
                        }
                        else
                        {
                            wizardFlashView.StatusLabel.Text = "✗ Flash operation failed";
                            wizardFlashView.StatusLabel.ForeColor = ErrorColor;
                        }
                    }));
                }, _cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                this.Invoke(new Action(() =>
                {
                    StopSpinner();
                    wizardFlashView.StatusLabel.Text = "⊘ Flash cancelled";
                    wizardFlashView.StatusLabel.ForeColor = TextSecondary;
                }));
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    StopSpinner();
                    wizardFlashView.StatusLabel.Text = $"✗ Error: {ex.Message}";
                    wizardFlashView.StatusLabel.ForeColor = ErrorColor;
                    LogMessage($"Flash failed: {ex.Message}");
                }));
            }
        }
    }
}
