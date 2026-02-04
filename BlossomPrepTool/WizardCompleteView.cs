using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BlossomPrepTool
{
    public partial class WizardCompleteView : UserControl
    {
        public event EventHandler FinishClicked;
        public event EventHandler RebootClicked;

        public Button RebootButton => btnReboot;

        public WizardCompleteView()
        {
            InitializeComponent();
            ApplyRoundedCorners();
            FixLabelTransparency();
            LoadQRCode();
            this.SizeChanged += (s, e) => 
            {
                ApplyRoundedCorners();
                Invalidate();
            };
            EnableAntialiasing();
            ApplyCardBorders();
        }

        private void EnableAntialiasing()
        {
            picQR.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            };
        }

        private void ApplyCardBorders()
        {
            cardMain.BackColor = System.Drawing.Color.FromArgb(35, 35, 40);
            Main.ApplyCardBorder(cardMain, System.Drawing.Color.FromArgb(50, 50, 55));
        }

        private void LoadQRCode()
        {
            try
            {
                picQR.Image = DefaultBitmaps.qr;
            }
            catch
            {
                // QR code not available
                picQR.Visible = false;
                lblQRMessage.Visible = false;
            }
        }

        private void ApplyRoundedCorners()
        {
            // Round card
            cardMain.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, cardMain.Width, cardMain.Height, 12, 12));
            
            // Round buttons
            btnFinish.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnFinish.Width, btnFinish.Height, 8, 8));
            btnReboot.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnReboot.Width, btnReboot.Height, 8, 8));

            // Round QR image
            picQR.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, picQR.Width, picQR.Height, 10, 10));
        }

        private void FixLabelTransparency()
        {
            SetLabelOnCard(lblTitle);
            SetLabelOnCard(lblMessage);
            SetLabelOnCard(lblKeepUSB);
            SetLabelOnCard(lblInstructions);
            SetLabelOnCard(lblQRMessage);
        }

        private void SetLabelOnCard(Label label)
        {
            label.Parent = cardMain;
            label.BackColor = cardMain.BackColor;
        }

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );

        private void btnFinish_Click(object sender, EventArgs e)
        {
            FinishClicked?.Invoke(this, e);
        }

        private void btnReboot_Click(object sender, EventArgs e)
        {
            RebootClicked?.Invoke(this, e);
        }
    }
}
