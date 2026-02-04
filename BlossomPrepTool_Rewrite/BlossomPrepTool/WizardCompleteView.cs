using System;
using System.Windows.Forms;

namespace BlossomPrepTool
{
    public partial class WizardCompleteView : UserControl
    {
        public event EventHandler FinishClicked;

        public WizardCompleteView()
        {
            InitializeComponent();
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            FinishClicked?.Invoke(this, e);
        }
    }
}
