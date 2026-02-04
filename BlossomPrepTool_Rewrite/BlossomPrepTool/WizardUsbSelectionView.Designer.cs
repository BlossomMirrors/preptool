namespace BlossomPrepTool
{
    partial class WizardUsbSelectionView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cardMain = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblDriveLabel = new System.Windows.Forms.Label();
            this.cmbUSBDrives = new System.Windows.Forms.ComboBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lblSelected = new System.Windows.Forms.Label();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.cardMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // cardMain
            // 
            this.cardMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(46)))));
            this.cardMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cardMain.Controls.Add(this.lblSelected);
            this.cardMain.Controls.Add(this.btnRefresh);
            this.cardMain.Controls.Add(this.cmbUSBDrives);
            this.cardMain.Controls.Add(this.lblDriveLabel);
            this.cardMain.Controls.Add(this.lblTitle);
            this.cardMain.Location = new System.Drawing.Point(40, 80);
            this.cardMain.Name = "cardMain";
            this.cardMain.Size = new System.Drawing.Size(720, 340);
            this.cardMain.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Georgia", 20F, System.Drawing.FontStyle.Bold);
            this.lblTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(46)))));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(231)))));
            this.lblTitle.Location = new System.Drawing.Point(30, 30);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(259, 31);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Select USB Drive";
            // 
            // lblDriveLabel
            // 
            this.lblDriveLabel.AutoSize = true;
            this.lblDriveLabel.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblDriveLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(46)))));
            this.lblDriveLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(231)))));
            this.lblDriveLabel.Location = new System.Drawing.Point(30, 90);
            this.lblDriveLabel.Name = "lblDriveLabel";
            this.lblDriveLabel.Size = new System.Drawing.Size(168, 20);
            this.lblDriveLabel.TabIndex = 1;
            this.lblDriveLabel.Text = "Choose your USB drive:";
            // 
            // cmbUSBDrives
            // 
            this.cmbUSBDrives.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(23)))));
            this.cmbUSBDrives.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUSBDrives.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbUSBDrives.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.cmbUSBDrives.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(231)))));
            this.cmbUSBDrives.FormattingEnabled = true;
            this.cmbUSBDrives.Location = new System.Drawing.Point(30, 120);
            this.cmbUSBDrives.Name = "cmbUSBDrives";
            this.cmbUSBDrives.Size = new System.Drawing.Size(660, 28);
            this.cmbUSBDrives.TabIndex = 2;
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(23)))));
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnRefresh.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(231)))));
            this.btnRefresh.Location = new System.Drawing.Point(30, 170);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(150, 40);
            this.btnRefresh.TabIndex = 3;
            this.btnRefresh.Text = "⟳ Refresh";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // lblSelected
            // 
            this.lblSelected.AutoSize = true;
            this.lblSelected.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblSelected.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(46)))));
            this.lblSelected.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(161)))), ((int)(((byte)(161)))), ((int)(((byte)(170)))));
            this.lblSelected.Location = new System.Drawing.Point(30, 235);
            this.lblSelected.MaximumSize = new System.Drawing.Size(660, 0);
            this.lblSelected.Name = "lblSelected";
            this.lblSelected.Size = new System.Drawing.Size(111, 19);
            this.lblSelected.TabIndex = 4;
            this.lblSelected.Text = "No USB selected";
            // 
            // btnNext
            // 
            this.btnNext.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(23)))));
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(88)))));
            this.btnNext.FlatAppearance.BorderSize = 0;
            this.btnNext.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(92)))), ((int)(((byte)(100)))), ((int)(((byte)(255)))));
            this.btnNext.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(23)))));
            this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNext.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnNext.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(231)))));
            this.btnNext.Location = new System.Drawing.Point(560, 500);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(200, 50);
            this.btnNext.TabIndex = 1;
            this.btnNext.Text = "Continue →";
            this.btnNext.UseVisualStyleBackColor = false;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnBack
            // 
            this.btnBack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(23)))));
            this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBack.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(88)))));
            this.btnBack.FlatAppearance.BorderSize = 0;
            this.btnBack.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(92)))), ((int)(((byte)(100)))), ((int)(((byte)(255)))));
            this.btnBack.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(23)))));
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBack.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnBack.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(231)))));
            this.btnBack.Location = new System.Drawing.Point(40, 500);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(200, 50);
            this.btnBack.TabIndex = 2;
            this.btnBack.Text = "← Back";
            this.btnBack.UseVisualStyleBackColor = false;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // WizardUsbSelectionView
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(23)))));
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.cardMain);
            this.Name = "WizardUsbSelectionView";
            this.Size = new System.Drawing.Size(800, 600);
            this.cardMain.ResumeLayout(false);
            this.cardMain.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel cardMain;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblDriveLabel;
        private System.Windows.Forms.ComboBox cmbUSBDrives;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label lblSelected;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnBack;
    }
}
