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
            ApplyLocalization();
            lblModeTitle.BackColor = this.BackColor;
        }

        private void ApplyLocalization()
        {
            lblModeTitle.Text = Localizer.GetString("WizardModeSelection.Title");
            btnSimpleMode.Text = Localizer.GetString("WizardModeSelection.SimpleMode");
            btnDualBootMode.Text = Localizer.GetString("WizardModeSelection.DualBootMode");
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
