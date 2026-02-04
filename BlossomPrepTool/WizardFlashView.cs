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
        public Label TitleLabel => lblTitle;
        public Label DescriptionLabel => lblDescription;
        public Button StartButton => btnStart;
        public Button BackButton => btnBack;

        public WizardFlashView()
        {
            InitializeComponent();
            ApplyLocalization();
            ApplyRoundedCorners();
            FixLabelTransparency();
            this.SizeChanged += (s, e) => 
            {
                ApplyRoundedCorners();
                Invalidate();
            };
            ApplyCardBorders();
        }

        private void ApplyLocalization()
        {
            lblTitle.Text = Localizer.GetString("WizardFlash.Title");
            lblDescription.Text = Localizer.GetString("WizardFlash.Description");
            lblStatus.Text = Localizer.GetString("WizardFlash.Status");
        }

        private void ApplyCardBorders()
        {
            cardMain.BackColor = System.Drawing.Color.FromArgb(35, 35, 40);
            Main.ApplyCardBorder(cardMain, System.Drawing.Color.FromArgb(50, 50, 55));
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
            SetLabelOnCard(lblTitle);
            SetLabelOnCard(lblDescription);
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
