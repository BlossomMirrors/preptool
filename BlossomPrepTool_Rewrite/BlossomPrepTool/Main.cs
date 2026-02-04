using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlossomPrepTool
{
    public partial class Main : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // width of ellipse
            int nHeightEllipse // height of ellipse
        );

        private const int RESIZE_HANDLE_SIZE = 10;
        private PrepToolIntegration _preptool;
        private Dictionary<int, USBInfo> _usbDiskMap = new Dictionary<int, USBInfo>();

        private class ButtonFadeState
        {
            public Timer Timer { get; set; }
            public Image NormalImage { get; set; }
            public Image HoverImage { get; set; }
            public float Progress { get; set; }
            public bool FadingToHover { get; set; }
            public PictureBox Control { get; set; }
        }

        private ButtonFadeState closeFadeState;
        private ButtonFadeState maximizeFadeState;
        private ButtonFadeState minimizeFadeState;
        
        private const int FADE_DURATION_MS = 100;
        private const int FADE_TIMER_INTERVAL = 15;
        
        private Size previousSize;
        private Point previousLocation;

        public Main()
        {
            InitializeComponent();
            
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
            
            // Initialize PrepTool backend
            _preptool = new PrepToolIntegration();
            
            InitializeFadeStates();
            SetupButtonHoverEffects();
        }
        
        private void LoadInitialData()
        {
            Task.Run(async () =>
            {
                try
                {
                    // Load data in background
                    var drives = _preptool.GetUSBDrives();
                    var isoPath = _preptool.GetISOPath();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error during initialization: {ex.Message}");
                }
            });
        }

        private void InitializeFadeStates()
        {
            closeFadeState = new ButtonFadeState
            {
                Timer = new Timer { Interval = FADE_TIMER_INTERVAL },
                Control = close,
                NormalImage = WindowControls.close,
                HoverImage = WindowControls.close_hover,
                Progress = 0f
            };
            closeFadeState.Timer.Tick += (s, e) => FadeTimer_Tick(closeFadeState);

            maximizeFadeState = new ButtonFadeState
            {
                Timer = new Timer { Interval = FADE_TIMER_INTERVAL },
                Control = maximize_normalize,
                Progress = 0f
            };
            maximizeFadeState.Timer.Tick += (s, e) => FadeTimer_Tick(maximizeFadeState);

            minimizeFadeState = new ButtonFadeState
            {
                Timer = new Timer { Interval = FADE_TIMER_INTERVAL },
                Control = minimize,
                NormalImage = WindowControls.minimize,
                HoverImage = WindowControls.minimize_hover,
                Progress = 0f
            };
            minimizeFadeState.Timer.Tick += (s, e) => FadeTimer_Tick(minimizeFadeState);
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
        }

        private void StartFade(ButtonFadeState state, bool toHover)
        {
            state.FadingToHover = toHover;
            state.Timer.Start();
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
            
            if (state.Control != null)
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
            lblISOStatus.Text = "Starting download...";

            try
            {
                var isoPath = await _preptool.DownloadISO();
                lblISOStatus.Text = "Download completed";
                LogMessage($"ISO downloaded to: {isoPath}");
            }
            catch (Exception ex)
            {
                lblISOStatus.Text = $"Error: {ex.Message}";
                LogMessage($"Download failed: {ex.Message}");
            }
            finally
            {
                btnDownloadISO.Enabled = true;
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
                MessageBox.Show("Please select a USB drive", "No Drive Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedDrive = _usbDiskMap[cmbUSBDrives.SelectedIndex];
            var isoPath = _preptool.GetISOPath();

            if (!System.IO.File.Exists(isoPath))
            {
                MessageBox.Show("ISO file not found. Please download it first.", "ISO Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"This will erase all data on Disk {selectedDrive.DiskNumber}. Continue?",
                "Confirm USB Flash", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            btnFlashUSB.Enabled = false;
            lblFlashStatus.Text = "Flashing...";

            try
            {
                var result = await _preptool.FlashUSB(selectedDrive.DiskNumber, isoPath);
                if (result)
                    lblFlashStatus.Text = "Flash completed successfully!";
                else
                    lblFlashStatus.Text = "Flash failed";
            }
            catch (Exception ex)
            {
                lblFlashStatus.Text = $"Flash error: {ex.Message}";
                LogMessage($"Flash failed: {ex.Message}");
            }
            finally
            {
                btnFlashUSB.Enabled = true;
            }
        }

        private async Task RefreshDriveInfo()
        {
            try
            {
                var driveInfo = await _preptool.GetCDriveSizeInfo();
                this.Invoke(new Action(() =>
                {
                    LogMessage($"C: Drive - Total: {driveInfo.TotalSizeGB}GB, Free: {driveInfo.FreeSpaceGB}GB, Used: {driveInfo.UsedSpaceGB}GB");
                }));
            }
            catch (Exception ex)
            {
                LogMessage($"Error getting drive info: {ex.Message}");
            }
        }

        private async void btnResizePartition_Click(object sender, EventArgs e)
        {
            var targetSize = (double)numPartitionSize.Value;

            if (MessageBox.Show(
                $"This will resize your C: partition to create {targetSize}GB of free space.\\nThis operation requires a restart and should not be interrupted!\\n\\nContinue?",
                "Confirm Partition Resize", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            btnResizePartition.Enabled = false;
            lblPartitionStatus.Text = "Resizing...";

            try
            {
                var result = await _preptool.ResizePartition((int)targetSize);
                if (result)
                {
                    lblPartitionStatus.Text = "Partition resized successfully! A restart may be required.";
                    await RefreshDriveInfo();
                }
                else
                    lblPartitionStatus.Text = "Partition resize failed";
            }
            catch (Exception ex)
            {
                lblPartitionStatus.ForeColor = Color.FromArgb(239, 68, 68);
                lblPartitionStatus.Text = $"Error: {ex.Message}";
            }
            finally
            {
                btnResizePartition.Enabled = true;
            }
        }

        private async void btnInstallWinBTRFS_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will install winbtrfs via Chocolatey.\\nContinue?",
                "Confirm Installation", MessageBoxButtons.YesNo, MessageBoxIcon.Information) != DialogResult.Yes)
                return;

            btnInstallWinBTRFS.Enabled = false;
            lblWinBTRFSStatus.Text = "Installing...";

            try
            {
                var result = await _preptool.InstallWinBTRFS();
                if (result)
                    lblWinBTRFSStatus.Text = "winbtrfs installed successfully!";
                else
                    lblWinBTRFSStatus.Text = "Installation failed";
            }
            catch (Exception ex)
            {
                lblWinBTRFSStatus.ForeColor = Color.FromArgb(239, 68, 68);
                lblWinBTRFSStatus.Text = $"Error: {ex.Message}";
            }
            finally
            {
                btnInstallWinBTRFS.Enabled = true;
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

        private bool mouseDown;
        private Point lastLocation;

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
    }
}
