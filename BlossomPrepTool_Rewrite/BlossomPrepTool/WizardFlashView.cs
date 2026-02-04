using System;
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
        }

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
