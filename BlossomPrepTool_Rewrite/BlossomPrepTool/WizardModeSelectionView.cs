using System;
using System.Windows.Forms;

namespace BlossomPrepTool
{
    public partial class WizardModeSelectionView : UserControl
    {
        public event EventHandler SimpleModeSelected;
        public event EventHandler DualBootModeSelected;

        public WizardModeSelectionView()
        {
            InitializeComponent();
        }

        private void btnSimpleMode_Click(object sender, EventArgs e)
        {
            SimpleModeSelected?.Invoke(this, e);
        }

        private void btnDualBootMode_Click(object sender, EventArgs e)
        {
            DualBootModeSelected?.Invoke(this, e);
        }
    }
}
