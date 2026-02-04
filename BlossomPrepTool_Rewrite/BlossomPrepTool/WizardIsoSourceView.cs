using System;
using System.Drawing;
using System.Windows.Forms;

namespace BlossomPrepTool
{
    public partial class WizardIsoSourceView : UserControl
    {
        private Panel cardDownload;
        private Panel cardUseOwn;
        private Button btnDownload;
        private Button btnUseOwn;
        private Button btnBack;
        private Label lblDownloadTitle;
        private Label lblDownloadDesc;
        private Label lblUseOwnTitle;
        private Label lblUseOwnDesc;
        private Label lblTitle;

        // Theme colors
        private readonly Color DarkBg = Color.FromArgb(20, 20, 23);
        private readonly Color DarkPanel = Color.FromArgb(20, 20, 23);
        private readonly Color CardBg = Color.FromArgb(41, 41, 46);
        private readonly Color AccentColor = Color.FromArgb(92, 100, 255);
        private readonly Color TextColor = Color.FromArgb(229, 229, 231);
        private readonly Color TextSecondary = Color.FromArgb(161, 161, 170);

        public event EventHandler DownloadClicked;
        public event EventHandler UseOwnClicked;
        public event EventHandler BackClicked;

        public WizardIsoSourceView()
        {
            InitializeComponent();
            ApplyRoundedCards();
            FixLabelTransparency();
            WireUpEvents();
        }

        private void ApplyRoundedCards()
        {
            this.cardDownload.Region = new System.Drawing.Region(RoundedRectangle(new Rectangle(0, 0, cardDownload.Width, cardDownload.Height), 12));
            this.cardUseOwn.Region = new System.Drawing.Region(RoundedRectangle(new Rectangle(0, 0, cardUseOwn.Width, cardUseOwn.Height), 12));
            this.btnDownload.Region = new System.Drawing.Region(RoundedRectangle(new Rectangle(0, 0, btnDownload.Width, btnDownload.Height), 8));
            this.btnUseOwn.Region = new System.Drawing.Region(RoundedRectangle(new Rectangle(0, 0, btnUseOwn.Width, btnUseOwn.Height), 8));
            this.btnBack.Region = new System.Drawing.Region(RoundedRectangle(new Rectangle(0, 0, btnBack.Width, btnBack.Height), 8));
        }

        private void FixLabelTransparency()
        {
            lblTitle.Parent = this;
            lblDownloadTitle.Parent = cardDownload;
            lblDownloadDesc.Parent = cardDownload;
            lblUseOwnTitle.Parent = cardUseOwn;
            lblUseOwnDesc.Parent = cardUseOwn;
        }

        private void WireUpEvents()
        {
            this.btnDownload.Click += (s, e) => DownloadClicked?.Invoke(this, EventArgs.Empty);
            this.btnUseOwn.Click += (s, e) => UseOwnClicked?.Invoke(this, EventArgs.Empty);
            this.btnBack.Click += (s, e) => BackClicked?.Invoke(this, EventArgs.Empty);
            this.cardDownload.Click += (s, e) => DownloadClicked?.Invoke(this, EventArgs.Empty);
            this.cardUseOwn.Click += (s, e) => UseOwnClicked?.Invoke(this, EventArgs.Empty);
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
            this.btnBack = new System.Windows.Forms.Button();
            this.cardDownload.SuspendLayout();
            this.cardUseOwn.SuspendLayout();
            this.SuspendLayout();
            
            // lblTitle
            this.lblTitle.Text = "Do you have a BlossomOS ISO?";
            this.lblTitle.Font = new System.Drawing.Font("Georgia", 24F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = TextColor;
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(40, 40);
            this.lblTitle.BackColor = Color.Transparent;
            
            // cardDownload
            this.cardDownload.BackColor = CardBg;
            this.cardDownload.Location = new System.Drawing.Point(40, 120);
            this.cardDownload.Size = new System.Drawing.Size(340, 280);
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
            this.btnDownload.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnDownload.ForeColor = TextColor;
            this.btnDownload.BackColor = DarkPanel;
            this.btnDownload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDownload.Size = new System.Drawing.Size(280, 45);
            this.btnDownload.Location = new System.Drawing.Point(30, 210);
            this.btnDownload.FlatAppearance.BorderSize = 1;
            this.btnDownload.FlatAppearance.BorderColor = Color.FromArgb(80, 80, 88);
            this.btnDownload.FlatAppearance.MouseDownBackColor = AccentColor;
            this.btnDownload.FlatAppearance.MouseOverBackColor = DarkPanel;
            this.btnDownload.Cursor = Cursors.Hand;
            
            // cardUseOwn
            this.cardUseOwn.BackColor = CardBg;
            this.cardUseOwn.Location = new System.Drawing.Point(420, 120);
            this.cardUseOwn.Size = new System.Drawing.Size(340, 280);
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
            this.btnUseOwn.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnUseOwn.ForeColor = Color.White;
            this.btnUseOwn.BackColor = DarkPanel;
            this.btnUseOwn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUseOwn.Size = new System.Drawing.Size(280, 45);
            this.btnUseOwn.Location = new System.Drawing.Point(30, 210);
            this.btnUseOwn.FlatAppearance.BorderSize = 0;
            this.btnUseOwn.FlatAppearance.MouseDownBackColor = AccentColor;
            this.btnUseOwn.FlatAppearance.MouseOverBackColor = DarkPanel;
            this.btnUseOwn.Cursor = Cursors.Hand;
            
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
            this.Controls.Add(this.btnBack);
            this.Name = "WizardIsoSourceView";
            this.Size = new System.Drawing.Size(800, 520);
            this.cardDownload.ResumeLayout(false);
            this.cardDownload.PerformLayout();
            this.cardUseOwn.ResumeLayout(false);
            this.cardUseOwn.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
