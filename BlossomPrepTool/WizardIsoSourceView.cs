using System;
using System.Drawing;
using System.Windows.Forms;

namespace BlossomPrepTool
{
    public partial class WizardIsoSourceView : UserControl
    {
        private Panel cardDownload;
        private Panel cardUseOwn;
        private Panel cardRestore;
        private Button btnDownload;
        private Button btnUseOwn;
        private Button btnRestore;
        private Button btnBack;
        private Label lblDownloadTitle;
        private Label lblDownloadDesc;
        private Label lblUseOwnTitle;
        private Label lblUseOwnDesc;
        private Label lblRestoreTitle;
        private Label lblRestoreDesc;
        private Label lblTitle;

        // Theme colors
        private readonly Color DarkBg = Color.FromArgb(20, 20, 23);
        private readonly Color DarkPanel = Color.FromArgb(20, 20, 23);
        private readonly Color CardBg = Color.FromArgb(35, 35, 40);
        private readonly Color CardBorder = Color.FromArgb(50, 50, 55);
        private readonly Color AccentColor = Color.FromArgb(92, 100, 255);
        private readonly Color TextColor = Color.FromArgb(229, 229, 231);
        private readonly Color TextSecondary = Color.FromArgb(161, 161, 170);

        public event EventHandler DownloadClicked;
        public event EventHandler UseOwnClicked;
        public event EventHandler RestoreClicked;
        public event EventHandler BackClicked;

        public WizardIsoSourceView()
        {
            InitializeComponent();
            ApplyLocalization();
            ApplyRoundedCards();
            FixLabelTransparency();
            WireUpEvents();
            LayoutCards();
            this.Resize += (s, e) => 
            {
                LayoutCards();
                Invalidate();
            };
        }

        private void ApplyLocalization()
        {
            lblTitle.Text = Localizer.GetString("WizardIsoSource.Title");
            lblDownloadTitle.Text = Localizer.GetString("WizardIsoSource.DownloadTitle");
            lblDownloadDesc.Text = Localizer.GetString("WizardIsoSource.DownloadDesc");
            btnDownload.Text = Localizer.GetString("WizardIsoSource.DownloadButton");
            lblUseOwnTitle.Text = Localizer.GetString("WizardIsoSource.UseOwnTitle");
            lblUseOwnDesc.Text = Localizer.GetString("WizardIsoSource.UseOwnDesc");
            btnUseOwn.Text = Localizer.GetString("WizardIsoSource.UseOwnButton");
            lblRestoreTitle.Text = Localizer.GetString("WizardIsoSource.RestoreTitle");
            lblRestoreDesc.Text = Localizer.GetString("WizardIsoSource.RestoreDesc");
            btnRestore.Text = Localizer.GetString("WizardIsoSource.RestoreButton");
            btnBack.Text = Localizer.GetString("WizardIsoSource.BackButton");
        }

        private void LayoutCards()
        {
            int padding = 40;
            int gap = 20;
            int top = 120;
            int bottomPadding = 115;

            int availableWidth = Math.Max(0, this.ClientSize.Width - (padding * 2) - (gap * 2));
            int cardWidth = Math.Max(200, availableWidth / 3);
            int cardHeight = Math.Max(240, this.ClientSize.Height - top - bottomPadding);

            cardDownload.SetBounds(padding, top, cardWidth, cardHeight);
            cardUseOwn.SetBounds(padding + cardWidth + gap, top, cardWidth, cardHeight);
            cardRestore.SetBounds(padding + (cardWidth + gap) * 2, top, cardWidth, cardHeight);

            btnBack.Size = new Size(200, 50);
            btnBack.Location = new Point(padding, this.ClientSize.Height - btnBack.Height - 40);
            btnBack.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

            btnDownload.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnUseOwn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnRestore.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            ApplyRoundedCards();
        }

        private void ApplyRoundedCards()
        {
            this.cardDownload.Region = new System.Drawing.Region(RoundedRectangle(new Rectangle(0, 0, cardDownload.Width, cardDownload.Height), 12));
            this.cardUseOwn.Region = new System.Drawing.Region(RoundedRectangle(new Rectangle(0, 0, cardUseOwn.Width, cardUseOwn.Height), 12));
            this.cardRestore.Region = new System.Drawing.Region(RoundedRectangle(new Rectangle(0, 0, cardRestore.Width, cardRestore.Height), 12));
            this.btnDownload.Region = new System.Drawing.Region(RoundedRectangle(new Rectangle(0, 0, btnDownload.Width, btnDownload.Height), 8));
            this.btnUseOwn.Region = new System.Drawing.Region(RoundedRectangle(new Rectangle(0, 0, btnUseOwn.Width, btnUseOwn.Height), 8));
            this.btnRestore.Region = new System.Drawing.Region(RoundedRectangle(new Rectangle(0, 0, btnRestore.Width, btnRestore.Height), 8));
            this.btnBack.Region = new System.Drawing.Region(RoundedRectangle(new Rectangle(0, 0, btnBack.Width, btnBack.Height), 8));
        }

        private void FixLabelTransparency()
        {
            SetLabelOnBackground(lblTitle);
            SetLabelOnCard(lblDownloadTitle, cardDownload);
            SetLabelOnCard(lblDownloadDesc, cardDownload);
            SetLabelOnCard(lblUseOwnTitle, cardUseOwn);
            SetLabelOnCard(lblUseOwnDesc, cardUseOwn);
            SetLabelOnCard(lblRestoreTitle, cardRestore);
            SetLabelOnCard(lblRestoreDesc, cardRestore);
        }

        private void SetLabelOnCard(Label label, Panel card)
        {
            label.Parent = card;
            label.BackColor = card.BackColor;
        }

        private void SetLabelOnBackground(Label label)
        {
            label.Parent = this;
            label.BackColor = this.BackColor;
        }

        private void WireUpEvents()
        {
            this.btnDownload.Click += (s, e) => DownloadClicked?.Invoke(this, EventArgs.Empty);
            this.btnUseOwn.Click += (s, e) => UseOwnClicked?.Invoke(this, EventArgs.Empty);
            this.btnRestore.Click += (s, e) => RestoreClicked?.Invoke(this, EventArgs.Empty);
            this.btnBack.Click += (s, e) => BackClicked?.Invoke(this, EventArgs.Empty);
            this.cardDownload.Click += (s, e) => DownloadClicked?.Invoke(this, EventArgs.Empty);
            this.cardUseOwn.Click += (s, e) => UseOwnClicked?.Invoke(this, EventArgs.Empty);
            this.cardRestore.Click += (s, e) => RestoreClicked?.Invoke(this, EventArgs.Empty);
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.cardDownload = new System.Windows.Forms.Panel();
            this.lblDownloadTitle = new System.Windows.Forms.Label();
            this.lblDownloadDesc = new System.Windows.Forms.Label();
            this.btnDownload = new System.Windows.Forms.Button();
            this.cardUseOwn = new System.Windows.Forms.Panel();
            this.lblUseOwnTitle = new System.Windows.Forms.Label();
            this.lblUseOwnDesc = new System.Windows.Forms.Label();
            this.btnUseOwn = new System.Windows.Forms.Button();
            this.cardRestore = new System.Windows.Forms.Panel();
            this.lblRestoreTitle = new System.Windows.Forms.Label();
            this.lblRestoreDesc = new System.Windows.Forms.Label();
            this.btnRestore = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.cardDownload.SuspendLayout();
            this.cardUseOwn.SuspendLayout();
            this.cardRestore.SuspendLayout();
            this.SuspendLayout();
            
            // lblTitle
            this.lblTitle.Text = "What would you like to do?";
            this.lblTitle.Font = new System.Drawing.Font("Georgia", 24F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = TextColor;
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(40, 40);
            this.lblTitle.BackColor = Color.Transparent;
            
            // cardDownload
            this.cardDownload.BackColor = CardBg;
            this.cardDownload.Location = new System.Drawing.Point(40, 120);
            this.cardDownload.Size = new System.Drawing.Size(240, 280);
            this.cardDownload.Padding = new System.Windows.Forms.Padding(0);
            this.cardDownload.Cursor = Cursors.Hand;
            this.cardDownload.Controls.Add(this.lblDownloadTitle);
            this.cardDownload.Controls.Add(this.lblDownloadDesc);
            this.cardDownload.Controls.Add(this.btnDownload);
            
            // lblDownloadTitle
            this.lblDownloadTitle.Text = "Download BlossomOS ISO";
            this.lblDownloadTitle.Font = new System.Drawing.Font("Georgia", 14F, System.Drawing.FontStyle.Bold);
            this.lblDownloadTitle.ForeColor = TextColor;
            this.lblDownloadTitle.AutoSize = true;
            this.lblDownloadTitle.Location = new System.Drawing.Point(30, 40);
            this.lblDownloadTitle.BackColor = Color.Transparent;
            
            // lblDownloadDesc
            this.lblDownloadDesc.Text = "Download the latest BlossomOS recovery\nenvironment ISO from our servers";
            this.lblDownloadDesc.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblDownloadDesc.ForeColor = TextSecondary;
            this.lblDownloadDesc.AutoSize = true;
            this.lblDownloadDesc.Location = new System.Drawing.Point(30, 80);
            this.lblDownloadDesc.BackColor = Color.Transparent;
            
            // btnDownload
            this.btnDownload.Text = "Download ISO";
            this.btnDownload.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnDownload.ForeColor = TextColor;
            this.btnDownload.BackColor = DarkPanel;
            this.btnDownload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDownload.Size = new System.Drawing.Size(180, 45);
            this.btnDownload.Location = new System.Drawing.Point(30, 210);
            this.btnDownload.FlatAppearance.BorderSize = 1;
            this.btnDownload.FlatAppearance.BorderColor = Color.FromArgb(80, 80, 88);
            this.btnDownload.FlatAppearance.MouseDownBackColor = AccentColor;
            this.btnDownload.FlatAppearance.MouseOverBackColor = DarkPanel;
            this.btnDownload.Cursor = Cursors.Hand;
            
            // cardUseOwn
            this.cardUseOwn.BackColor = CardBg;
            this.cardUseOwn.Location = new System.Drawing.Point(300, 120);
            this.cardUseOwn.Size = new System.Drawing.Size(240, 280);
            this.cardUseOwn.Padding = new System.Windows.Forms.Padding(0);
            this.cardUseOwn.Cursor = Cursors.Hand;
            this.cardUseOwn.Controls.Add(this.lblUseOwnTitle);
            this.cardUseOwn.Controls.Add(this.lblUseOwnDesc);
            this.cardUseOwn.Controls.Add(this.btnUseOwn);
            
            // lblUseOwnTitle
            this.lblUseOwnTitle.Text = "Use my own ISO";
            this.lblUseOwnTitle.Font = new System.Drawing.Font("Georgia", 14F, System.Drawing.FontStyle.Bold);
            this.lblUseOwnTitle.ForeColor = TextColor;
            this.lblUseOwnTitle.AutoSize = true;
            this.lblUseOwnTitle.Location = new System.Drawing.Point(30, 40);
            this.lblUseOwnTitle.BackColor = Color.Transparent;
            
            // lblUseOwnDesc
            this.lblUseOwnDesc.Text = "I already have a BlossomOS ISO file\non my computer";
            this.lblUseOwnDesc.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblUseOwnDesc.ForeColor = TextSecondary;
            this.lblUseOwnDesc.AutoSize = true;
            this.lblUseOwnDesc.Location = new System.Drawing.Point(30, 80);
            this.lblUseOwnDesc.BackColor = Color.Transparent;
            
            // btnUseOwn
            this.btnUseOwn.Text = "Select ISO file";
            this.btnUseOwn.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnUseOwn.ForeColor = Color.White;
            this.btnUseOwn.BackColor = DarkPanel;
            this.btnUseOwn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUseOwn.Size = new System.Drawing.Size(180, 45);
            this.btnUseOwn.Location = new System.Drawing.Point(30, 210);
            this.btnUseOwn.FlatAppearance.BorderSize = 0;
            this.btnUseOwn.FlatAppearance.MouseDownBackColor = AccentColor;
            this.btnUseOwn.FlatAppearance.MouseOverBackColor = DarkPanel;
            this.btnUseOwn.Cursor = Cursors.Hand;
            
            // cardRestore
            this.cardRestore.BackColor = CardBg;
            this.cardRestore.Location = new System.Drawing.Point(560, 120);
            this.cardRestore.Size = new System.Drawing.Size(240, 280);
            this.cardRestore.Padding = new System.Windows.Forms.Padding(0);
            this.cardRestore.Cursor = Cursors.Hand;
            this.cardRestore.Controls.Add(this.lblRestoreTitle);
            this.cardRestore.Controls.Add(this.lblRestoreDesc);
            this.cardRestore.Controls.Add(this.btnRestore);
            
            // lblRestoreTitle
            this.lblRestoreTitle.Text = "Restore USB drive";
            this.lblRestoreTitle.Font = new System.Drawing.Font("Georgia", 14F, System.Drawing.FontStyle.Bold);
            this.lblRestoreTitle.ForeColor = TextColor;
            this.lblRestoreTitle.AutoSize = true;
            this.lblRestoreTitle.Location = new System.Drawing.Point(30, 40);
            this.lblRestoreTitle.BackColor = Color.Transparent;
            
            // lblRestoreDesc
            this.lblRestoreDesc.Text = "Revert a BlossomOS USB back to\na normal Windows USB drive";
            this.lblRestoreDesc.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblRestoreDesc.ForeColor = TextSecondary;
            this.lblRestoreDesc.AutoSize = true;
            this.lblRestoreDesc.Location = new System.Drawing.Point(30, 80);
            this.lblRestoreDesc.BackColor = Color.Transparent;
            
            // btnRestore
            this.btnRestore.Text = "Restore USB";
            this.btnRestore.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnRestore.ForeColor = Color.White;
            this.btnRestore.BackColor = DarkPanel;
            this.btnRestore.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRestore.Size = new System.Drawing.Size(180, 45);
            this.btnRestore.Location = new System.Drawing.Point(30, 210);
            this.btnRestore.FlatAppearance.BorderSize = 0;
            this.btnRestore.FlatAppearance.MouseDownBackColor = AccentColor;
            this.btnRestore.FlatAppearance.MouseOverBackColor = DarkPanel;
            this.btnRestore.Cursor = Cursors.Hand;
            
            // btnBack
            this.btnBack.Text = "← Back";
            this.btnBack.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnBack.ForeColor = TextColor;
            this.btnBack.BackColor = DarkPanel;
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBack.Size = new System.Drawing.Size(120, 45);
            this.btnBack.Location = new System.Drawing.Point(40, 430);
            this.btnBack.FlatAppearance.BorderSize = 1;
            this.btnBack.FlatAppearance.BorderColor = Color.FromArgb(80, 80, 88);
            this.btnBack.Cursor = Cursors.Hand;
            
            // WizardIsoSourceView
            this.BackColor = DarkPanel;
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.cardDownload);
            this.Controls.Add(this.cardUseOwn);
            this.Controls.Add(this.cardRestore);
            this.Controls.Add(this.btnBack);
            this.Name = "WizardIsoSourceView";
            this.Size = new System.Drawing.Size(800, 520);
            this.cardDownload.ResumeLayout(false);
            this.cardDownload.PerformLayout();
            this.cardUseOwn.ResumeLayout(false);
            this.cardUseOwn.PerformLayout();
            this.cardRestore.ResumeLayout(false);
            this.cardRestore.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
