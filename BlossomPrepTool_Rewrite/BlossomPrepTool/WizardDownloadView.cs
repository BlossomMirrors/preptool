using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BlossomPrepTool
{
    public partial class WizardDownloadView : UserControl
    {
        public event EventHandler NextClicked;
        public event EventHandler PauseClicked;
        public event EventHandler CancelClicked;
        public event EventHandler BackClicked;

        public Label StatusLabel => lblStatus;
        public RoundProgressBar ProgressBar => progressBar;
        public Button NextButton => btnNext;
        public Button PauseButton => btnPause;
        public Button CancelButton => btnCancel;
        public Button BackButton => btnBack;

        public WizardDownloadView()
        {
            InitializeComponent();
            ApplyRoundedCorners();
            FixLabelTransparency();
            this.SizeChanged += (s, e) => ApplyRoundedCorners();
        }

        private void ApplyRoundedCorners()
        {
            // Round card
            cardMain.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, cardMain.Width, cardMain.Height, 12, 12));
            
            // Round buttons
            btnBack.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnBack.Width, btnBack.Height, 8, 8));
            btnPause.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnPause.Width, btnPause.Height, 8, 8));
            btnCancel.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnCancel.Width, btnCancel.Height, 8, 8));
            btnNext.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnNext.Width, btnNext.Height, 8, 8));
        }

        private void FixLabelTransparency()
        {
            SetLabelOnCard(lblTitle);
            SetLabelOnCard(lblStatus);
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

        private void btnNext_Click(object sender, EventArgs e)
        {
            NextClicked?.Invoke(this, e);
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            PauseClicked?.Invoke(this, e);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            CancelClicked?.Invoke(this, e);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            BackClicked?.Invoke(this, e);
        }
    }
}
