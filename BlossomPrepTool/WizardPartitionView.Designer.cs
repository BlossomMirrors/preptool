namespace BlossomPrepTool
{
    partial class WizardPartitionView
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
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblDiskInfo = new System.Windows.Forms.Label();
            this.lblAllocateLabel = new System.Windows.Forms.Label();
            this.txtAllocateGB = new System.Windows.Forms.TextBox();
            this.lblGBLabel = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.cardMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // cardMain
            // 
            this.cardMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(40)))));
            this.cardMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cardMain.Controls.Add(this.lblStatus);
            this.cardMain.Controls.Add(this.lblGBLabel);
            this.cardMain.Controls.Add(this.txtAllocateGB);
            this.cardMain.Controls.Add(this.lblAllocateLabel);
            this.cardMain.Controls.Add(this.lblDiskInfo);
            this.cardMain.Controls.Add(this.lblDescription);
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
            this.lblTitle.Size = new System.Drawing.Size(226, 31);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Allocate Space";
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblDescription.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(46)))));
            this.lblDescription.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(231)))));
            this.lblDescription.Location = new System.Drawing.Point(30, 80);
            this.lblDescription.MaximumSize = new System.Drawing.Size(660, 0);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(528, 20);
            this.lblDescription.TabIndex = 1;
            this.lblDescription.Text = "Choose how much space to allocate for BlossomOS from your C: drive.";
            // 
            // lblDiskInfo
            // 
            this.lblDiskInfo.AutoSize = true;
            this.lblDiskInfo.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblDiskInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(46)))));
            this.lblDiskInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(161)))), ((int)(((byte)(161)))), ((int)(((byte)(170)))));
            this.lblDiskInfo.Location = new System.Drawing.Point(30, 120);
            this.lblDiskInfo.MaximumSize = new System.Drawing.Size(660, 0);
            this.lblDiskInfo.Name = "lblDiskInfo";
            this.lblDiskInfo.Size = new System.Drawing.Size(155, 19);
            this.lblDiskInfo.TabIndex = 2;
            this.lblDiskInfo.Text = "Loading disk information...";
            // 
            // lblAllocateLabel
            // 
            this.lblAllocateLabel.AutoSize = true;
            this.lblAllocateLabel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblAllocateLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(46)))));
            this.lblAllocateLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(231)))));
            this.lblAllocateLabel.Location = new System.Drawing.Point(30, 170);
            this.lblAllocateLabel.Name = "lblAllocateLabel";
            this.lblAllocateLabel.Size = new System.Drawing.Size(168, 20);
            this.lblAllocateLabel.TabIndex = 3;
            this.lblAllocateLabel.Text = "Space for BlossomOS:";
            // 
            // txtAllocateGB
            // 
            this.txtAllocateGB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(38)))));
            this.txtAllocateGB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAllocateGB.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtAllocateGB.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(231)))));
            this.txtAllocateGB.Location = new System.Drawing.Point(220, 168);
            this.txtAllocateGB.Name = "txtAllocateGB";
            this.txtAllocateGB.Size = new System.Drawing.Size(100, 29);
            this.txtAllocateGB.TabIndex = 4;
            this.txtAllocateGB.Text = "50";
            this.txtAllocateGB.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblGBLabel
            // 
            this.lblGBLabel.AutoSize = true;
            this.lblGBLabel.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblGBLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(46)))));
            this.lblGBLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(161)))), ((int)(((byte)(161)))), ((int)(((byte)(170)))));
            this.lblGBLabel.Location = new System.Drawing.Point(330, 172);
            this.lblGBLabel.Name = "lblGBLabel";
            this.lblGBLabel.Size = new System.Drawing.Size(27, 20);
            this.lblGBLabel.TabIndex = 5;
            this.lblGBLabel.Text = "GB";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(46)))));
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(161)))), ((int)(((byte)(161)))), ((int)(((byte)(170)))));
            this.lblStatus.Location = new System.Drawing.Point(30, 230);
            this.lblStatus.MaximumSize = new System.Drawing.Size(660, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(207, 19);
            this.lblStatus.TabIndex = 6;
            this.lblStatus.Text = "Minimum recommended: 50 GB";
            // 
            // btnNext
            // 
            this.btnNext.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(92)))), ((int)(((byte)(100)))), ((int)(((byte)(255)))));
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.FlatAppearance.BorderSize = 0;
            this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNext.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnNext.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(231)))));
            this.btnNext.Location = new System.Drawing.Point(560, 500);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(200, 50);
            this.btnNext.TabIndex = 1;
            this.btnNext.Text = "Next →";
            this.btnNext.UseVisualStyleBackColor = false;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnBack
            // 
            this.btnBack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(23)))));
            this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBack.FlatAppearance.BorderSize = 0;
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
            // WizardPartitionView
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(23)))));
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.cardMain);
            this.Name = "WizardPartitionView";
            this.Size = new System.Drawing.Size(800, 600);
            this.cardMain.ResumeLayout(false);
            this.cardMain.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel cardMain;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Label lblDiskInfo;
        private System.Windows.Forms.Label lblAllocateLabel;
        private System.Windows.Forms.TextBox txtAllocateGB;
        private System.Windows.Forms.Label lblGBLabel;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnBack;
    }
}
