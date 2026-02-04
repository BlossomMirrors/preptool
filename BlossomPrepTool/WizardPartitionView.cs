using System;
using System.Drawing;
using System.Windows.Forms;

namespace BlossomPrepTool
{
    public partial class WizardPartitionView : UserControl
    {
        public event EventHandler NextClicked;
        public event EventHandler BackClicked;

        public Label DiskInfoLabel => lblDiskInfo;
        public Label StatusLabel => lblStatus;
        public TextBox AllocateTextBox => txtAllocateGB;
        public Button NextButton => btnNext;
        public Button BackButton => btnBack;

        public WizardPartitionView()
        {
            InitializeComponent();
            ApplyLocalization();
            ApplyRoundedCorners();
            FixLabelTransparency();
            
            // Sync TrackBar with TextBox
            txtAllocateGB.TextChanged += (s, e) =>
            {
                if (int.TryParse(txtAllocateGB.Text, out int value))
                {
                    trkAllocateGB.Value = Math.Max(trkAllocateGB.Minimum, Math.Min(trkAllocateGB.Maximum, value));
                }
            };
            
            this.SizeChanged += (s, e) => 
            {
                ApplyRoundedCorners();
                Invalidate();
            };
            ApplyCardBorders();
        }

        private void ApplyLocalization()
        {
            lblTitle.Text = Localizer.GetString("WizardPartition.Title");
            lblDescription.Text = Localizer.GetString("WizardPartition.Description");
            lblDiskInfo.Text = Localizer.GetString("WizardPartition.DiskInfo");
            lblAllocateLabel.Text = Localizer.GetString("WizardPartition.AllocateLabel");
            lblGBLabel.Text = Localizer.GetString("WizardPartition.GBLabel");
            lblStatus.Text = Localizer.GetString("WizardPartition.Status");
            btnNext.Text = Localizer.GetString("WizardPartition.NextButton");
            btnBack.Text = Localizer.GetString("WizardPartition.BackButton");
        }

        private void ApplyCardBorders()
        {
            cardMain.BackColor = System.Drawing.Color.FromArgb(35, 35, 40);
            Main.ApplyCardBorder(cardMain, System.Drawing.Color.FromArgb(50, 50, 55));
        }

        private void ApplyRoundedCorners()
        {
            cardMain.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, cardMain.Width, cardMain.Height, 12, 12));
            btnNext.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnNext.Width, btnNext.Height, 8, 8));
            btnBack.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnBack.Width, btnBack.Height, 8, 8));
            txtAllocateGB.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, txtAllocateGB.Width, txtAllocateGB.Height, 6, 6));
        }

        private void FixLabelTransparency()
        {
            SetLabelOnCard(lblTitle);
            SetLabelOnCard(lblDescription);
            SetLabelOnCard(lblDiskInfo);
            SetLabelOnCard(lblAllocateLabel);
            SetLabelOnCard(lblGBLabel);
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

        private void btnBack_Click(object sender, EventArgs e)
        {
            BackClicked?.Invoke(this, e);
        }

        private void trkAllocateGB_Scroll(object sender, EventArgs e)
        {
            txtAllocateGB.Text = trkAllocateGB.Value.ToString();
        }
    }
}
