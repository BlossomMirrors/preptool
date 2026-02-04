using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BlossomPrepTool
{
    public partial class WizardUsbSelectionView : UserControl
    {
        public event EventHandler RefreshClicked;
        public event EventHandler NextClicked;
        public event EventHandler BackClicked;

        public ComboBox DriveComboBox => cmbUSBDrives;
        public Button RefreshButton => btnRefresh;
        public Button NextButton => btnNext;
        public Label SelectedLabel => lblSelected;

        public WizardUsbSelectionView()
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
            
            // Trigger initial USB refresh when view is loaded
            this.Load += (s, e) => RefreshClicked?.Invoke(this, EventArgs.Empty);
        }

        private void ApplyLocalization()
        {
            lblTitle.Text = Localizer.GetString("WizardUsbSelection.Title");
            lblDriveLabel.Text = Localizer.GetString("WizardUsbSelection.DriveLabel");
            btnRefresh.Text = Localizer.GetString("WizardUsbSelection.RefreshButton");
            lblSelected.Text = Localizer.GetString("WizardUsbSelection.NoUsbSelected");
            btnNext.Text = Localizer.GetString("WizardUsbSelection.ContinueButton");
            btnBack.Text = Localizer.GetString("WizardUsbSelection.BackButton");
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
            btnRefresh.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnRefresh.Width, btnRefresh.Height, 8, 8));
            btnNext.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnNext.Width, btnNext.Height, 8, 8));
            btnBack.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnBack.Width, btnBack.Height, 8, 8));
        }

        private void FixLabelTransparency()
        {
            SetLabelOnCard(lblTitle);
            SetLabelOnCard(lblDriveLabel);
            SetLabelOnCard(lblSelected);
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

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshClicked?.Invoke(this, e);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            NextClicked?.Invoke(this, e);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            BackClicked?.Invoke(this, e);
        }
    }
}
