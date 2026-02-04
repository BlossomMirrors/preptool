using System;
using System.Drawing;
using System.Windows.Forms;

namespace BlossomPrepTool
{
    public partial class WizardWelcomeView : UserControl
    {
        private Panel cardLeft;
        private Panel cardRight;
        private Panel cardWelcome;
        private Button btnManualSetup;
        private Button btnGetStarted;
        private Label lblWelcome;
        private Label lblTitle;
        private Label lblDescription;
        private Label lblManualTitle;
        private Label lblManualDesc;
        private Label lblGetStartedTitle;
        private Label lblGetStartedDesc;
        private PictureBox pictureBoxLogo;

        // Theme colors
        private readonly Color DarkBg = Color.FromArgb(20, 20, 23);
        private readonly Color DarkPanel = Color.FromArgb(20, 20, 23);
        private readonly Color CardBg = Color.FromArgb(41, 41, 46);
        private readonly Color AccentColor = Color.FromArgb(92, 100, 255);
        private readonly Color TextColor = Color.FromArgb(229, 229, 231);
        private readonly Color TextSecondary = Color.FromArgb(161, 161, 170);

        public event EventHandler ManualSetupClicked;
        public event EventHandler GetStartedClicked;

        public WizardWelcomeView()
        {
            InitializeComponent();
            ApplyRoundedCards();
            FixLabelTransparency();
            WireUpEvents();
            LayoutCards();
            this.Resize += (s, e) => LayoutCards();
        }

        private void LayoutCards()
        {
            int padding = 40;
            int gap = 20;
            int headerHeight = 140;
            int topCards = padding + headerHeight + 20;
            int bottomPadding = 40;

            int availableWidth = Math.Max(0, this.ClientSize.Width - (padding * 2) - gap);
            int cardWidth = Math.Max(240, availableWidth / 2);
            int cardHeight = Math.Max(240, this.ClientSize.Height - topCards - bottomPadding);

            cardWelcome.SetBounds(padding, padding, Math.Max(300, this.ClientSize.Width - (padding * 2)), headerHeight);
            cardLeft.SetBounds(padding, topCards, cardWidth, cardHeight);
            cardRight.SetBounds(padding + cardWidth + gap, topCards, cardWidth, cardHeight);

            int buttonHeight = 45;
            int buttonMargin = 30;
            int buttonY = Math.Max(120, cardHeight - buttonHeight - buttonMargin);
            int buttonWidth = Math.Max(180, cardWidth - (buttonMargin * 2));

            btnManualSetup.SetBounds(buttonMargin, buttonY, buttonWidth, buttonHeight);
            btnGetStarted.SetBounds(buttonMargin, buttonY, buttonWidth, buttonHeight);

            lblManualTitle.MaximumSize = new Size(buttonWidth, 0);
            lblGetStartedTitle.MaximumSize = new Size(buttonWidth, 0);

            ApplyRoundedCards();
        }

        private void FixLabelTransparency()
        {
            // Force labels to properly support transparency
            lblWelcome.Parent = cardWelcome;
            lblTitle.Parent = cardWelcome;
            lblDescription.Parent = cardWelcome;
            lblManualTitle.Parent = cardLeft;
            lblManualDesc.Parent = cardLeft;
            lblGetStartedTitle.Parent = cardRight;
            lblGetStartedDesc.Parent = cardRight;
        }

        private void ApplyRoundedCards()
        {
            this.cardWelcome.Region = new System.Drawing.Region(RoundedRectangle(new Rectangle(0, 0, cardWelcome.Width, cardWelcome.Height), 12));
            this.cardLeft.Region = new System.Drawing.Region(RoundedRectangle(new Rectangle(0, 0, cardLeft.Width, cardLeft.Height), 12));
            this.cardRight.Region = new System.Drawing.Region(RoundedRectangle(new Rectangle(0, 0, cardRight.Width, cardRight.Height), 12));
            this.btnManualSetup.Region = new System.Drawing.Region(RoundedRectangle(new Rectangle(0, 0, btnManualSetup.Width, btnManualSetup.Height), 8));
            this.btnGetStarted.Region = new System.Drawing.Region(RoundedRectangle(new Rectangle(0, 0, btnGetStarted.Width, btnGetStarted.Height), 8));
        }

        private void WireUpEvents()
        {
            this.btnManualSetup.Click += (s, e) => ManualSetupClicked?.Invoke(this, EventArgs.Empty);
            this.btnGetStarted.Click += (s, e) => GetStartedClicked?.Invoke(this, EventArgs.Empty);
            this.cardLeft.Click += (s, e) => ManualSetupClicked?.Invoke(this, EventArgs.Empty);
            this.cardRight.Click += (s, e) => GetStartedClicked?.Invoke(this, EventArgs.Empty);
        }

        private System.Drawing.Drawing2D.GraphicsPath RoundedRectangle(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            var path = new System.Drawing.Drawing2D.GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            path.AddArc(arc, 180, 90);

            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            arc.Y = bounds.Bottom - diameter;
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 0, 90);

            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        private void InitializeComponent()
        {
            this.cardWelcome = new System.Windows.Forms.Panel();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.lblWelcome = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.cardLeft = new System.Windows.Forms.Panel();
            this.lblManualTitle = new System.Windows.Forms.Label();
            this.lblManualDesc = new System.Windows.Forms.Label();
            this.btnManualSetup = new System.Windows.Forms.Button();
            this.cardRight = new System.Windows.Forms.Panel();
            this.lblGetStartedTitle = new System.Windows.Forms.Label();
            this.lblGetStartedDesc = new System.Windows.Forms.Label();
            this.btnGetStarted = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.cardWelcome.SuspendLayout();
            this.cardLeft.SuspendLayout();
            this.cardRight.SuspendLayout();
            this.SuspendLayout();
            
            // cardWelcome
            this.cardWelcome.BackColor = CardBg;
            this.cardWelcome.Location = new System.Drawing.Point(40, 40);
            this.cardWelcome.Size = new System.Drawing.Size(720, 140);
            this.cardWelcome.Padding = new System.Windows.Forms.Padding(0);
            this.cardWelcome.Controls.Add(this.pictureBoxLogo);
            this.cardWelcome.Controls.Add(this.lblWelcome);
            this.cardWelcome.Controls.Add(this.lblTitle);
            this.cardWelcome.Controls.Add(this.lblDescription);
            
            // pictureBoxLogo
            this.pictureBoxLogo.Image = DefaultBitmaps.icon;
            this.pictureBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxLogo.Size = new System.Drawing.Size(64, 64);
            this.pictureBoxLogo.Location = new System.Drawing.Point(30, 25);
            this.pictureBoxLogo.BackColor = Color.Transparent;
            
            // lblWelcome
            this.lblWelcome.Text = "WELCOME";
            this.lblWelcome.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblWelcome.ForeColor = AccentColor;
            this.lblWelcome.AutoSize = true;
            this.lblWelcome.Location = new System.Drawing.Point(110, 25);
            this.lblWelcome.BackColor = CardBg;
            
            // lblTitle
            this.lblTitle.Text = "BlossomOS Switch";
            this.lblTitle.Font = new System.Drawing.Font("Georgia", 20F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = TextColor;
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(110, 45);
            this.lblTitle.BackColor = CardBg;
            
            // lblDescription
            this.lblDescription.Text = "Prepare your system for installing BlossomOS with this easy-to-use tool.\nFollow the steps to ensure a smooth installation process.";
            this.lblDescription.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblDescription.ForeColor = TextSecondary;
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(110, 80);
            this.lblDescription.BackColor = CardBg;
            
            // cardLeft
            this.cardLeft.BackColor = CardBg;
            this.cardLeft.Location = new System.Drawing.Point(40, 200);
            this.cardLeft.Size = new System.Drawing.Size(340, 280);
            this.cardLeft.Padding = new System.Windows.Forms.Padding(0);
            this.cardLeft.Cursor = Cursors.Hand;
            this.cardLeft.Controls.Add(this.lblManualTitle);
            this.cardLeft.Controls.Add(this.lblManualDesc);
            this.cardLeft.Controls.Add(this.btnManualSetup);
            
            // lblManualTitle
            this.lblManualTitle.Text = "Install the BlossomOS recovery environment\nonto an existing USB drive";
            this.lblManualTitle.Font = new System.Drawing.Font("Georgia", 11F);
            this.lblManualTitle.ForeColor = TextSecondary;
            this.lblManualTitle.AutoSize = true;
            this.lblManualTitle.MaximumSize = new System.Drawing.Size(280, 0);
            this.lblManualTitle.Location = new System.Drawing.Point(30, 40);
            this.lblManualTitle.BackColor = Color.Transparent;
            
            // lblManualDesc
            this.lblManualDesc.Text = "Quick USB setup only";
            this.lblManualDesc.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblManualDesc.ForeColor = TextSecondary;
            this.lblManualDesc.AutoSize = true;
            this.lblManualDesc.Location = new System.Drawing.Point(30, 110);
            this.lblManualDesc.BackColor = Color.Transparent;
            
            // btnManualSetup
            this.btnManualSetup.Text = "Manual setup";
            this.btnManualSetup.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnManualSetup.ForeColor = TextColor;
            this.btnManualSetup.BackColor = DarkPanel;
            this.btnManualSetup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnManualSetup.Size = new System.Drawing.Size(280, 45);
            this.btnManualSetup.Location = new System.Drawing.Point(30, 210);
            this.btnManualSetup.FlatAppearance.BorderSize = 1;
            this.btnManualSetup.FlatAppearance.BorderColor = Color.FromArgb(80, 80, 88);
            this.btnManualSetup.Cursor = Cursors.Hand;
            
            // cardRight
            this.cardRight.BackColor = CardBg;
            this.cardRight.Location = new System.Drawing.Point(420, 200);
            this.cardRight.Size = new System.Drawing.Size(340, 280);
            this.cardRight.Padding = new System.Windows.Forms.Padding(0);
            this.cardRight.Cursor = Cursors.Hand;
            this.cardRight.Controls.Add(this.lblGetStartedTitle);
            this.cardRight.Controls.Add(this.lblGetStartedDesc);
            this.cardRight.Controls.Add(this.btnGetStarted);
            
            // lblGetStartedTitle
            this.lblGetStartedTitle.Text = "Prepare your system and install the recovery\nenvironment onto your USB drive.";
            this.lblGetStartedTitle.Font = new System.Drawing.Font("Georgia", 11F);
            this.lblGetStartedTitle.ForeColor = TextSecondary;
            this.lblGetStartedTitle.AutoSize = true;
            this.lblGetStartedTitle.MaximumSize = new System.Drawing.Size(280, 0);
            this.lblGetStartedTitle.Location = new System.Drawing.Point(30, 40);
            this.lblGetStartedTitle.BackColor = Color.Transparent;
            
            // lblGetStartedDesc
            this.lblGetStartedDesc.Text = "Complete setup with system configuration";
            this.lblGetStartedDesc.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblGetStartedDesc.ForeColor = TextSecondary;
            this.lblGetStartedDesc.AutoSize = true;
            this.lblGetStartedDesc.Location = new System.Drawing.Point(30, 110);
            this.lblGetStartedDesc.BackColor = Color.Transparent;
            
            // btnGetStarted
            this.btnGetStarted.Text = "Get started";
            this.btnGetStarted.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnGetStarted.ForeColor = Color.White;
            this.btnGetStarted.BackColor = DarkPanel;
            this.btnGetStarted.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGetStarted.Size = new System.Drawing.Size(280, 45);
            this.btnGetStarted.Location = new System.Drawing.Point(30, 210);
            this.btnGetStarted.FlatAppearance.BorderSize = 0;
            this.btnGetStarted.FlatAppearance.MouseDownBackColor = Color.FromArgb(92, 100, 255);
            this.btnGetStarted.FlatAppearance.MouseOverBackColor = DarkPanel;
            this.btnGetStarted.Cursor = Cursors.Hand;
            
            // WizardWelcomeView
            this.BackColor = DarkPanel;
            this.Controls.Add(this.cardWelcome);
            this.Controls.Add(this.cardLeft);
            this.Controls.Add(this.cardRight);
            this.Name = "WizardWelcomeView";
            this.Size = new System.Drawing.Size(800, 520);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.cardWelcome.ResumeLayout(false);
            this.cardWelcome.PerformLayout();
            this.cardLeft.ResumeLayout(false);
            this.cardLeft.PerformLayout();
            this.cardRight.ResumeLayout(false);
            this.cardRight.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
