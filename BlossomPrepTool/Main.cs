using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlossomPrepTool
{
    public partial class Main : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );

        // Dark Theme Colors
        private readonly Color DarkBg = Color.FromArgb(20, 20, 23);
        private readonly Color DarkPanel = Color.FromArgb(33, 33, 38);
        private readonly Color CardBg = Color.FromArgb(35, 35, 40);
        private readonly Color CardBorder = Color.FromArgb(50, 50, 55);
        private readonly Color AccentColor = Color.FromArgb(92, 100, 255);
        private readonly Color TextColor = Color.FromArgb(229, 229, 231);
        private readonly Color TextSecondary = Color.FromArgb(161, 161, 170);
        private readonly Color SuccessColor = Color.FromArgb(34, 197, 94);
        private readonly Color ErrorColor = Color.FromArgb(239, 68, 68);
        private readonly Color WarningColor = Color.FromArgb(251, 146, 60);

        // Window Control Fade States
        private class ButtonFadeState
        {
            public System.Windows.Forms.Timer Timer { get; set; }
            public Image NormalImage { get; set; }
            public Image HoverImage { get; set; }
            public float Progress { get; set; }
            public bool FadingToHover { get; set; }
            public PictureBox Control { get; set; }
        }

        private ButtonFadeState closeFadeState;
        private ButtonFadeState maximizeFadeState;
        private ButtonFadeState minimizeFadeState;

        private class ButtonThemeState
        {
            public Color? EnabledBackColor { get; set; }
            public Color? EnabledForeColor { get; set; }
        }

        private readonly Dictionary<Button, ButtonThemeState> _buttonThemeStates = new Dictionary<Button, ButtonThemeState>();

        private const int FADE_DURATION_MS = 100;
        private const int FADE_TIMER_INTERVAL = 15;
        private const int RESIZE_HANDLE_SIZE = 10;

        private PrepToolIntegration _preptool;
        private Dictionary<int, USBInfo> _usbDiskMap = new Dictionary<int, USBInfo>();
        private CancellationTokenSource _cancellationTokenSource;

        private Size previousSize;
        private Point previousLocation;
        private bool mouseDown;
        private Point lastLocation;

        private int _completedSteps = 0;

        private System.Windows.Forms.Timer _spinnerTimer;
        private int _spinnerFrame = 0;
        private readonly string[] _spinnerFrames = new[] { "⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏" };
        private Label _currentSpinnerLabel;
        private string _spinnerBaseMessage;

        public Main()
        {
            InitializeComponent();
            _spinnerBaseMessage = Localizer.GetString("Status.Processing");

            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            this.BackColor = DarkBg;

            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));

            _preptool = new PrepToolIntegration();

            InitializeFadeStates();
            ApplyDarkTheme();
            ApplyMainWindowLocalization();
            SetupButtonHoverEffects();
            InitializeWizardPages();
            
            // Prevent closing during critical operations
            this.FormClosing += Main_FormClosing;

            // Hide all designer controls - wizard mode only
            HideAllDesignerControls();
            
            ShowStep(_currentStep);
        }


        private void InitializeFadeStates()
        {
            closeFadeState = new ButtonFadeState
            {
                Timer = new System.Windows.Forms.Timer { Interval = FADE_TIMER_INTERVAL },
                Control = close,
                NormalImage = WindowControls.close,
                HoverImage = WindowControls.close_hover,
                Progress = 0f
            };
            closeFadeState.Timer.Tick += (s, e) => FadeTimer_Tick(closeFadeState);

            maximizeFadeState = new ButtonFadeState
            {
                Timer = new System.Windows.Forms.Timer { Interval = FADE_TIMER_INTERVAL },
                Control = maximize_normalize,
                NormalImage = WindowControls.maximize,
                HoverImage = WindowControls.maximize_hover,
                Progress = 0f
            };
            maximizeFadeState.Timer.Tick += (s, e) => FadeTimer_Tick(maximizeFadeState);

            minimizeFadeState = new ButtonFadeState
            {
                Timer = new System.Windows.Forms.Timer { Interval = FADE_TIMER_INTERVAL },
                Control = minimize,
                NormalImage = WindowControls.minimize,
                HoverImage = WindowControls.minimize_hover,
                Progress = 0f
            };
            minimizeFadeState.Timer.Tick += (s, e) => FadeTimer_Tick(minimizeFadeState);
        }

        private void FadeTimer_Tick(ButtonFadeState state)
        {
            float increment = (float)FADE_TIMER_INTERVAL / FADE_DURATION_MS;

            if (state.FadingToHover)
            {
                state.Progress += increment;
                if (state.Progress >= 1f)
                {
                    state.Progress = 1f;
                    state.Timer.Stop();
                }
            }
            else
            {
                state.Progress -= increment;
                if (state.Progress <= 0f)
                {
                    state.Progress = 0f;
                    state.Timer.Stop();
                }
            }

            if (state.Control != null && state.NormalImage != null && state.HoverImage != null)
            {
                state.Control.BackgroundImage = BlendImages(state.NormalImage, state.HoverImage, state.Progress);
            }
        }

        private Image BlendImages(Image normal, Image hover, float progress)
        {
            if (progress <= 0f) return normal;
            if (progress >= 1f) return hover;

            Bitmap result = new Bitmap(normal.Width, normal.Height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.CompositingMode = CompositingMode.SourceOver;

                ColorMatrix normalMatrix = new ColorMatrix();
                normalMatrix.Matrix33 = 1f - progress;
                ImageAttributes normalAttributes = new ImageAttributes();
                normalAttributes.SetColorMatrix(normalMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                g.DrawImage(normal, new Rectangle(0, 0, normal.Width, normal.Height),
                           0, 0, normal.Width, normal.Height, GraphicsUnit.Pixel, normalAttributes);

                ColorMatrix hoverMatrix = new ColorMatrix();
                hoverMatrix.Matrix33 = progress;
                ImageAttributes hoverAttributes = new ImageAttributes();
                hoverAttributes.SetColorMatrix(hoverMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                g.DrawImage(hover, new Rectangle(0, 0, hover.Width, hover.Height),
                           0, 0, hover.Width, hover.Height, GraphicsUnit.Pixel, hoverAttributes);
            }

            return result;
        }

        private void StartFade(ButtonFadeState state, bool toHover)
        {
            state.FadingToHover = toHover;
            state.Timer.Start();
        }

        private void ApplyDarkTheme()
        {
            this.ForeColor = TextColor;

            foreach (Control ctrl in GetAllControls(this))
            {
                if (ctrl is Button btn)
                {
                    ApplyButtonTheme(btn);
                    btn.EnabledChanged -= ButtonEnabledChanged;
                    btn.EnabledChanged += ButtonEnabledChanged;
                }
                else if (ctrl is Label lbl)
                {
                    if (!string.Equals(lbl.Tag as string, "ThemeOverride", StringComparison.Ordinal))
                    {
                        bool hasCustomFore = lbl.ForeColor != SystemColors.ControlText && lbl.ForeColor != Color.Empty && lbl.ForeColor != Color.Transparent;

                        if (lbl.Parent != null)
                        {
                            lbl.BackColor = lbl.Parent.BackColor;
                        }
                        else
                        {
                            lbl.BackColor = DarkBg;
                        }

                        if (!hasCustomFore)
                        {
                            lbl.ForeColor = TextColor;
                        }
                    }
                }
                else if (ctrl is ListBox lstBox)
                {
                    lstBox.BackColor = DarkPanel;
                    lstBox.ForeColor = TextColor;
                }
                else if (ctrl is ComboBox combo)
                {
                    combo.BackColor = DarkPanel;
                    combo.ForeColor = TextColor;
                }
                else if (ctrl is NumericUpDown num)
                {
                    num.BackColor = DarkPanel;
                    num.ForeColor = TextColor;
                }
            }
        }

        private void ApplyButtonTheme(Button btn)
        {
            bool themeOverride = string.Equals(btn.Tag as string, "ThemeOverride", StringComparison.Ordinal);
            bool hasCustomBack = btn.BackColor != SystemColors.Control && btn.BackColor != Color.Empty && btn.BackColor != Color.Transparent;
            bool hasCustomFore = btn.ForeColor != SystemColors.ControlText && btn.ForeColor != Color.Empty && btn.ForeColor != Color.Transparent;

            if (!_buttonThemeStates.TryGetValue(btn, out var state))
            {
                state = new ButtonThemeState();
                _buttonThemeStates[btn] = state;
            }

            if (btn.Enabled)
            {
                if (!themeOverride)
                {
                    if (!hasCustomBack)
                    {
                        btn.BackColor = DarkPanel;
                    }

                    if (!hasCustomFore)
                    {
                        btn.ForeColor = TextColor;
                    }
                }

                state.EnabledBackColor = btn.BackColor;
                state.EnabledForeColor = btn.ForeColor;
            }

            btn.FlatStyle = FlatStyle.Flat;
            btn.UseVisualStyleBackColor = false;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseDownBackColor = AccentColor;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 70);
            btn.Cursor = btn.Enabled ? Cursors.Hand : Cursors.Default;

            if (!btn.Enabled && !themeOverride)
            {
                var baseBack = state.EnabledBackColor ?? btn.BackColor;
                var baseFore = state.EnabledForeColor ?? btn.ForeColor;

                btn.BackColor = ControlPaint.Dark(baseBack, 0.2f);
                btn.ForeColor = Color.FromArgb(120, 120, 130);

                if (baseFore != Color.Empty && baseFore != Color.Transparent && baseFore != SystemColors.ControlText)
                {
                    btn.ForeColor = ControlPaint.Light(baseFore, 0.2f);
                }

                btn.FlatAppearance.MouseDownBackColor = btn.BackColor;
                btn.FlatAppearance.MouseOverBackColor = btn.BackColor;
            }
        }

        private void ApplyMainWindowLocalization()
        {
            // Main window title
            this.Text = Localizer.GetString("Main.Title");

            // Button text
            btnDownloadISO.Text = Localizer.GetString("Main.DownloadISO");
            btnFlashUSB.Text = Localizer.GetString("Main.FlashUSB");
            btnResizePartition.Text = Localizer.GetString("Main.ResizePartition");
            btnInstallWinBTRFS.Text = Localizer.GetString("Main.InstallWinBTRFS");
            btnRefreshUSB.Text = Localizer.GetString("Main.Refresh");
            btnClearLog.Text = Localizer.GetString("Main.ClearLog");

            // Label text
            lblUSBDrives.Text = Localizer.GetString("Main.USBDrives");
            lblPartitionSize.Text = Localizer.GetString("Main.PartitionSize");
            lblISOStatus.Text = Localizer.GetString("Main.Ready");
            lblFlashStatus.Text = Localizer.GetString("Main.Ready");
            lblPartitionStatus.Text = Localizer.GetString("Main.Ready");
            lblWinBTRFSStatus.Text = Localizer.GetString("Main.Ready");
        }

        private void ButtonEnabledChanged(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                ApplyButtonTheme(btn);
            }
        }

        private IEnumerable<Control> GetAllControls(Control container)
        {
            foreach (Control ctrl in container.Controls)
            {
                yield return ctrl;
                foreach (Control child in GetAllControls(ctrl))
                    yield return child;
            }
        }

        private void SetupButtonHoverEffects()
        {
            close.MouseEnter += (s, e) => StartFade(closeFadeState, true);
            close.MouseLeave += (s, e) => StartFade(closeFadeState, false);

            maximize_normalize.MouseEnter += (s, e) =>
            {
                bool isMaximized = this.Size.Width == Screen.PrimaryScreen.WorkingArea.Width &&
                                   this.Size.Height == Screen.PrimaryScreen.WorkingArea.Height;
                maximizeFadeState.NormalImage = isMaximized ? WindowControls.normalize : WindowControls.maximize;
                maximizeFadeState.HoverImage = isMaximized ? WindowControls.normalize_hover : WindowControls.maximize_hover;
                StartFade(maximizeFadeState, true);
            };
            maximize_normalize.MouseLeave += (s, e) =>
            {
                bool isMaximized = this.Size.Width == Screen.PrimaryScreen.WorkingArea.Width &&
                                   this.Size.Height == Screen.PrimaryScreen.WorkingArea.Height;
                maximizeFadeState.NormalImage = isMaximized ? WindowControls.normalize : WindowControls.maximize;
                maximizeFadeState.HoverImage = isMaximized ? WindowControls.normalize_hover : WindowControls.maximize_hover;
                StartFade(maximizeFadeState, false);
            };

            minimize.MouseEnter += (s, e) => StartFade(minimizeFadeState, true);
            minimize.MouseLeave += (s, e) => StartFade(minimizeFadeState, false);

            // Regular button hover for other buttons
            foreach (Control ctrl in GetAllControls(this))
            {
                if (ctrl is Button btn && btn != btnDownloadISO && btn != btnFlashUSB &&
                    btn != btnResizePartition && btn != btnInstallWinBTRFS)
                {
                    bool themeOverride = string.Equals(btn.Tag as string, "ThemeOverride", StringComparison.Ordinal);
                    bool hasCustomBack = btn.BackColor != SystemColors.Control && btn.BackColor != Color.Empty && btn.BackColor != Color.Transparent;

                    if (!themeOverride && !hasCustomBack)
                    {
                        btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(60, 60, 70);
                    }

                    btn.MouseLeave += (s, e) => ApplyButtonTheme(btn);
                }
            }
        }

        private void StartSpinner(Label statusLabel)
        {
            _currentSpinnerLabel = statusLabel;
            if (_spinnerTimer == null)
            {
                _spinnerTimer = new System.Windows.Forms.Timer { Interval = 80 };
                _spinnerTimer.Tick += (s, e) =>
                {
                    if (_currentSpinnerLabel != null && !_currentSpinnerLabel.IsDisposed)
                    {
                        try
                        {
                            if (this.InvokeRequired)
                            {
                                this.Invoke(new Action(() =>
                                {
                                    _spinnerFrame = (_spinnerFrame + 1) % _spinnerFrames.Length;
                                    // Only update if the text starts with a spinner character or is the base message
                                    var currentText = _currentSpinnerLabel.Text;
                                    if (string.IsNullOrEmpty(currentText) || currentText == _spinnerBaseMessage || Array.Exists(_spinnerFrames, frame => currentText.StartsWith(frame)))
                                    {
                                        _currentSpinnerLabel.Text = _spinnerFrames[_spinnerFrame] + " " + _spinnerBaseMessage;
                                    }
                                }));
                            }
                            else
                            {
                                _spinnerFrame = (_spinnerFrame + 1) % _spinnerFrames.Length;
                                var currentText = _currentSpinnerLabel.Text;
                                if (string.IsNullOrEmpty(currentText) || currentText == _spinnerBaseMessage || Array.Exists(_spinnerFrames, frame => currentText.StartsWith(frame)))
                                {
                                    _currentSpinnerLabel.Text = _spinnerFrames[_spinnerFrame] + " " + _spinnerBaseMessage;
                                }
                            }
                        }
                        catch
                        {
                            _spinnerTimer.Stop();
                        }
                    }
                    else
                    {
                        _spinnerTimer.Stop();
                    }
                };
            }
            _spinnerFrame = 0;
            _spinnerTimer.Start();
        }

        private void StopSpinner()
        {
            if (_spinnerTimer != null)
            {
                _spinnerTimer.Stop();
            }
            _currentSpinnerLabel = null;
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x0084;
            const int HTCLIENT = 1;
            const int HTLEFT = 10;
            const int HTRIGHT = 11;
            const int HTTOP = 12;
            const int HTTOPLEFT = 13;
            const int HTTOPRIGHT = 14;
            const int HTBOTTOM = 15;
            const int HTBOTTOMLEFT = 16;
            const int HTBOTTOMRIGHT = 17;

            if (m.Msg == WM_NCHITTEST)
            {
                base.WndProc(ref m);

                if ((int)m.Result == HTCLIENT)
                {
                    Point screenPoint = new Point(m.LParam.ToInt32());
                    Point clientPoint = this.PointToClient(screenPoint);

                    if (clientPoint.X <= RESIZE_HANDLE_SIZE && clientPoint.Y <= RESIZE_HANDLE_SIZE)
                    {
                        m.Result = (IntPtr)HTTOPLEFT;
                        return;
                    }

                    if (clientPoint.X >= this.ClientSize.Width - RESIZE_HANDLE_SIZE && clientPoint.Y <= RESIZE_HANDLE_SIZE)
                    {
                        m.Result = (IntPtr)HTTOPRIGHT;
                        return;
                    }

                    if (clientPoint.X <= RESIZE_HANDLE_SIZE && clientPoint.Y >= this.ClientSize.Height - RESIZE_HANDLE_SIZE)
                    {
                        m.Result = (IntPtr)HTBOTTOMLEFT;
                        return;
                    }

                    if (clientPoint.X >= this.ClientSize.Width - RESIZE_HANDLE_SIZE && clientPoint.Y >= this.ClientSize.Height - RESIZE_HANDLE_SIZE)
                    {
                        m.Result = (IntPtr)HTBOTTOMRIGHT;
                        return;
                    }

                    if (clientPoint.X <= RESIZE_HANDLE_SIZE)
                    {
                        m.Result = (IntPtr)HTLEFT;
                        return;
                    }

                    if (clientPoint.X >= this.ClientSize.Width - RESIZE_HANDLE_SIZE)
                    {
                        m.Result = (IntPtr)HTRIGHT;
                        return;
                    }

                    if (clientPoint.Y <= RESIZE_HANDLE_SIZE)
                    {
                        m.Result = (IntPtr)HTTOP;
                        return;
                    }

                    if (clientPoint.Y >= this.ClientSize.Height - RESIZE_HANDLE_SIZE)
                    {
                        m.Result = (IntPtr)HTBOTTOM;
                        return;
                    }
                }

                return;
            }

            base.WndProc(ref m);
        }

        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;

            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();

            return path;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.Clear(this.BackColor);

            int radius = 12;
            int thickness = 1;

            Rectangle rect = new Rectangle(
                thickness,
                thickness,
                Width - 2 * thickness,
                Height - 2 * thickness
            );

            var path = GetRoundedRectPath(rect, radius);
            var pen = new Pen(Color.FromArgb(30, 241, 241, 243), thickness)
            {
                Alignment = PenAlignment.Inset
            };

            e.Graphics.DrawPath(pen, path);

            path.Dispose();
            pen.Dispose();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (Region != null)
            {
                Region.Dispose();
            }

            Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, Width + 3, Height + 2, 20, 20)
            );

            this.Invalidate();
        }

        // ==================== UI Event Handlers ====================

        private async void btnDownloadISO_Click(object sender, EventArgs e)
        {
            btnDownloadISO.Enabled = false;
            lblISOStatus.Text = "⠋ Downloading ISO...";
            lblISOStatus.ForeColor = WarningColor;
            StartSpinner(lblISOStatus);

            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                await Task.Run(async () =>
                {
                    var isoPath = await _preptool.DownloadISO();
                    this.Invoke(new Action(() =>
                    {
                        StopSpinner();
                        lblISOStatus.Text = "✓ ISO downloaded successfully!";
                        lblISOStatus.ForeColor = SuccessColor;
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
                    lblISOStatus.Text = "⊘ Download cancelled";
                    lblISOStatus.ForeColor = TextSecondary;
                }));
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    StopSpinner();
                    lblISOStatus.Text = $"✗ Error: {ex.Message}";
                    lblISOStatus.ForeColor = ErrorColor;
                    LogMessage($"Download failed: {ex.Message}");
                }));
            }
            finally
            {
                this.Invoke(new Action(() => btnDownloadISO.Enabled = true));
            }
        }

        private async Task RefreshUSBDrives()
        {
            var drives = _preptool.GetUSBDrives();
            _usbDiskMap.Clear();
            cmbUSBDrives.Items.Clear();

            foreach (var drive in drives)
            {
                _usbDiskMap[cmbUSBDrives.Items.Count] = drive;
                cmbUSBDrives.Items.Add($"Disk {drive.DiskNumber}: {drive.DisplayName} ({drive.SizeGB}GB)");
            }

            if (cmbUSBDrives.Items.Count > 0)
                cmbUSBDrives.SelectedIndex = 0;
        }

        private async void btnRefreshUSB_Click(object sender, EventArgs e)
        {
            btnRefreshUSB.Enabled = false;
            await RefreshUSBDrives();
            btnRefreshUSB.Enabled = true;
            LogMessage("USB drives refreshed");
        }

        private async void btnFlashUSB_Click(object sender, EventArgs e)
        {
            if (cmbUSBDrives.SelectedIndex < 0)
            {
                MessageBox.Show(Localizer.GetString("Message.NoUsbSelected"), Localizer.GetString("MessageBox.NoUsbSelected"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedDrive = _usbDiskMap[cmbUSBDrives.SelectedIndex];
            var isoPath = _preptool.GetISOPath();

            if (!System.IO.File.Exists(isoPath))
            {
                MessageBox.Show(Localizer.GetString("Message.IsoNotFoundSimple"), Localizer.GetString("MessageBox.IsoNotFound"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show(Localizer.GetString("Message.EraseUsbWarning", selectedDrive.DiskNumber),
                Localizer.GetString("MessageBox.ConfirmUsbFlash"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            btnFlashUSB.Enabled = false;
            lblFlashStatus.Text = "⠋ Flashing USB...";
            lblFlashStatus.ForeColor = WarningColor;
            StartSpinner(lblFlashStatus);

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
                            lblFlashStatus.Text = "✓ USB flashed successfully!";
                            lblFlashStatus.ForeColor = SuccessColor;
                            _completedSteps++;
                            LogMessage($"USB Disk {selectedDrive.DiskNumber}: Flash complete");
                        }
                        else
                        {
                            lblFlashStatus.Text = "✗ Flash operation failed";
                            lblFlashStatus.ForeColor = ErrorColor;
                        }
                    }));
                }, _cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                this.Invoke(new Action(() =>
                {
                    StopSpinner();
                    lblFlashStatus.Text = "⊘ Flash cancelled";
                    lblFlashStatus.ForeColor = TextSecondary;
                }));
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    StopSpinner();
                    lblFlashStatus.Text = $"✗ Error: {ex.Message}";
                    lblFlashStatus.ForeColor = ErrorColor;
                    LogMessage($"Flash failed: {ex.Message}");
                }));
            }
            finally
            {
                this.Invoke(new Action(() => btnFlashUSB.Enabled = true));
            }
        }

        private async void btnResizePartition_Click(object sender, EventArgs e)
        {
            var targetSize = (double)numPartitionSize.Value;

            if (MessageBox.Show(
                Localizer.GetString("Message.PartitionResizeWarning"),
                Localizer.GetString("MessageBox.ConfirmPartitionResize"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            btnResizePartition.Enabled = false;
            lblPartitionStatus.Text = "⠋ Resizing partition...";
            lblPartitionStatus.ForeColor = WarningColor;
            StartSpinner(lblPartitionStatus);

            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                await Task.Run(async () =>
                {
                    var result = await _preptool.ResizePartition((int)targetSize);
                    this.Invoke(new Action(() =>
                    {
                        StopSpinner();
                        if (result)
                        {
                            lblPartitionStatus.Text = "✓ Partition resized! Restart required.";
                            lblPartitionStatus.ForeColor = SuccessColor;
                            _completedSteps++;
                            LogMessage($"Partition resized to {targetSize}GB free space");
                        }
                        else
                        {
                            lblPartitionStatus.Text = "✗ Partition resize failed";
                            lblPartitionStatus.ForeColor = ErrorColor;
                        }
                    }));
                }, _cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                this.Invoke(new Action(() =>
                {
                    StopSpinner();
                    lblPartitionStatus.Text = "⊘ Resize cancelled";
                    lblPartitionStatus.ForeColor = TextSecondary;
                }));
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    StopSpinner();
                    lblPartitionStatus.Text = $"✗ Error: {ex.Message}";
                    lblPartitionStatus.ForeColor = ErrorColor;
                    LogMessage($"Partition resize failed: {ex.Message}");
                }));
            }
            finally
            {
                this.Invoke(new Action(() => btnResizePartition.Enabled = true));
            }
        }

        private async void btnInstallWinBTRFS_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Localizer.GetString("Message.InstallWinBtrfs"),
                Localizer.GetString("MessageBox.ConfirmInstallation"), MessageBoxButtons.YesNo, MessageBoxIcon.Information) != DialogResult.Yes)
                return;

            btnInstallWinBTRFS.Enabled = false;
            lblWinBTRFSStatus.Text = "⠋ Installing winbtrfs...";
            lblWinBTRFSStatus.ForeColor = WarningColor;
            StartSpinner(lblWinBTRFSStatus);

            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                await Task.Run(async () =>
                {
                    var result = await _preptool.InstallWinBTRFS();
                    this.Invoke(new Action(() =>
                    {
                        StopSpinner();
                        if (result)
                        {
                            lblWinBTRFSStatus.Text = "✓ winbtrfs installed!";
                            lblWinBTRFSStatus.ForeColor = SuccessColor;
                            _completedSteps++;
                            LogMessage("winbtrfs installation completed");
                        }
                        else
                        {
                            lblWinBTRFSStatus.Text = "✗ Installation failed";
                            lblWinBTRFSStatus.ForeColor = ErrorColor;
                        }
                    }));
                }, _cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                this.Invoke(new Action(() =>
                {
                    StopSpinner();
                    lblWinBTRFSStatus.Text = "⊘ Installation cancelled";
                    lblWinBTRFSStatus.ForeColor = TextSecondary;
                }));
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    StopSpinner();
                    lblWinBTRFSStatus.Text = $"✗ Error: {ex.Message}";
                    lblWinBTRFSStatus.ForeColor = ErrorColor;
                    LogMessage($"Installation failed: {ex.Message}");
                }));
            }
            finally
            {
                this.Invoke(new Action(() => btnInstallWinBTRFS.Enabled = true));
            }
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            lstLog.Items.Clear();
        }

        private void LogMessage(string message)
        {
            if (lstLog.InvokeRequired)
            {
                lstLog.Invoke(new Action(() => LogMessage(message)));
                return;
            }

            lstLog.Items.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
            lstLog.TopIndex = Math.Max(0, lstLog.Items.Count - 1);
        }

        private void Main_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
        }

        private void Main_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new Point(
                    (this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);

                this.Update();
            }
        }

        private void Main_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Prevent closing during flashing or restore operations
            if (_isFlashing || _isRestoreMode)
            {
                e.Cancel = true;
                MessageBox.Show(
                    "Cannot close the application while a USB operation is in progress.\n\nPlease wait for the operation to complete.",
                    "Operation in Progress",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Rectangle workingRectangle = Screen.PrimaryScreen.WorkingArea;

            if (this.Size.Width == workingRectangle.Width && this.Size.Height == workingRectangle.Height)
            {
                this.FormBorderStyle = FormBorderStyle.None;
                this.Size = previousSize;
                this.Location = previousLocation;
                maximize_normalize.BackgroundImage = WindowControls.maximize;
            }
            else
            {
                previousSize = this.Size;
                previousLocation = this.Location;
                this.FormBorderStyle = FormBorderStyle.None;
                this.Size = new Size(workingRectangle.Width, workingRectangle.Height);
                this.Location = new Point(0, 0);
                maximize_normalize.BackgroundImage = WindowControls.normalize;
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        /// <summary>
        /// Apply card border styling to a panel control
        /// </summary>
        public static void ApplyCardBorder(Panel card, Color borderColor)
        {
            card.Paint -= CardPaintHandler;
            card.Paint += (s, e) =>
            {
                using (Pen borderPen = new Pen(borderColor, 1))
                {
                    e.Graphics.DrawRectangle(borderPen, 0, 0, card.Width - 1, card.Height - 1);
                }
            };
        }

        private static void CardPaintHandler(object sender, PaintEventArgs e)
        {
            // Placeholder handler for removal
        }
    }
}