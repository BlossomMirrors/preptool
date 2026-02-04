using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BlossomPrepTool
{
    public partial class WizardFlashView : UserControl
    {
        public event EventHandler StartClicked;
        public event EventHandler BackClicked;

        public Label StatusLabel => lblStatus;

        public WizardFlashView()
        {
            InitializeComponent();
            ApplyRoundedCorners();
            FixLabelTransparency();
        }

        private void ApplyRoundedCorners()
        {
            // Round card
            cardMain.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, cardMain.Width, cardMain.Height, 12, 12));
            
            // Round buttons
            btnStart.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnStart.Width, btnStart.Height, 8, 8));
            btnBack.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnBack.Width, btnBack.Height, 8, 8));
        }

        private void FixLabelTransparency()
        {
            lblTitle.Parent = cardMain;
            lblDescription.Parent = cardMain;
            lblStatus.Parent = cardMain;
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

        private void btnStart_Click(object sender, EventArgs e)
        {
            StartClicked?.Invoke(this, e);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            BackClicked?.Invoke(this, e);
        }
    }
}
