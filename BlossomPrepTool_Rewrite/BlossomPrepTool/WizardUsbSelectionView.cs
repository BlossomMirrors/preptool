using System;
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
            
            // Trigger initial USB refresh when view is loaded
            this.Load += (s, e) => RefreshClicked?.Invoke(this, EventArgs.Empty);
        }

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
