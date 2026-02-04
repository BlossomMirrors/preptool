using System;
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
            CenterProgressBar();
        }

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

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            CenterProgressBar();
        }

        private void CenterProgressBar()
        {
            if (progressBar == null) return;
            progressBar.Left = (this.ClientSize.Width - progressBar.Width) / 2;
        }
    }
}
