namespace BlossomPrepTool
{
    partial class WizardCompleteView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizardCompleteView));
            this.cardMain = new System.Windows.Forms.Panel();
            this.lblQRMessage = new System.Windows.Forms.Label();
            this.picQR = new System.Windows.Forms.PictureBox();
            this.lblKeepUSB = new System.Windows.Forms.Label();
            this.lblInstructions = new System.Windows.Forms.Label();
            this.lblMessage = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnFinish = new System.Windows.Forms.Button();
            this.btnReboot = new System.Windows.Forms.Button();
            this.cardMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picQR)).BeginInit();
            this.SuspendLayout();
            // 
            // cardMain
            // 
            this.cardMain.AutoScroll = true;
            this.cardMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(46)))));
            this.cardMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cardMain.Controls.Add(this.lblQRMessage);
            this.cardMain.Controls.Add(this.picQR);
            this.cardMain.Controls.Add(this.lblKeepUSB);
            this.cardMain.Controls.Add(this.lblInstructions);
            this.cardMain.Controls.Add(this.lblMessage);
            this.cardMain.Controls.Add(this.lblTitle);
            this.cardMain.Location = new System.Drawing.Point(40, 40);
            this.cardMain.Name = "cardMain";
            this.cardMain.Size = new System.Drawing.Size(720, 440);
            this.cardMain.TabIndex = 0;
            // 
            // lblQRMessage
            // 
            this.lblQRMessage.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblQRMessage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(46)))));
            this.lblQRMessage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(161)))), ((int)(((byte)(161)))), ((int)(((byte)(170)))));
            this.lblQRMessage.Location = new System.Drawing.Point(480, 340);
            this.lblQRMessage.Name = "lblQRMessage";
            this.lblQRMessage.Size = new System.Drawing.Size(200, 40);
            this.lblQRMessage.TabIndex = 4;
            this.lblQRMessage.Text = "Scan with your mobile device\r\nfor video tutorial";
            this.lblQRMessage.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // picQR
            // 
            this.picQR.Location = new System.Drawing.Point(480, 132);
            this.picQR.Name = "picQR";
            this.picQR.Size = new System.Drawing.Size(200, 200);
            this.picQR.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picQR.TabIndex = 3;
            this.picQR.TabStop = false;
            // 
            // lblKeepUSB
            // 
            this.lblKeepUSB.AutoSize = true;
            this.lblKeepUSB.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblKeepUSB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(46)))));
            this.lblKeepUSB.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(211)))), ((int)(((byte)(77)))));
            this.lblKeepUSB.Location = new System.Drawing.Point(30, 100);
            this.lblKeepUSB.MaximumSize = new System.Drawing.Size(660, 0);
            this.lblKeepUSB.Name = "lblKeepUSB";
            this.lblKeepUSB.Size = new System.Drawing.Size(540, 17);
            this.lblKeepUSB.TabIndex = 5;
            this.lblKeepUSB.Text = "💡 Important: Keep your USB drive safe - it can be used to reinstall or recover B" +
    "lossomOS.";
            // 
            // lblInstructions
            // 
            this.lblInstructions.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblInstructions.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(46)))));
            this.lblInstructions.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(231)))));
            this.lblInstructions.Location = new System.Drawing.Point(30, 110);
            this.lblInstructions.MaximumSize = new System.Drawing.Size(400, 0);
            this.lblInstructions.Name = "lblInstructions";
            this.lblInstructions.Size = new System.Drawing.Size(400, 0);
            this.lblInstructions.TabIndex = 2;
            this.lblInstructions.Text = resources.GetString("lblInstructions.Text");
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblMessage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(46)))));
            this.lblMessage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(231)))));
            this.lblMessage.Location = new System.Drawing.Point(30, 70);
            this.lblMessage.MaximumSize = new System.Drawing.Size(660, 0);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(255, 20);
            this.lblMessage.TabIndex = 1;
            this.lblMessage.Text = "Your USB drive is ready to boot from!";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Georgia", 24F, System.Drawing.FontStyle.Bold);
            this.lblTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(46)))));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(197)))), ((int)(((byte)(94)))));
            this.lblTitle.Location = new System.Drawing.Point(30, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(317, 38);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "✓ Setup Complete!";
            // 
            // btnFinish
            // 
            this.btnFinish.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(23)))));
            this.btnFinish.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFinish.FlatAppearance.BorderSize = 0;
            this.btnFinish.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFinish.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnFinish.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(231)))));
            this.btnFinish.Location = new System.Drawing.Point(560, 500);
            this.btnFinish.Name = "btnFinish";
            this.btnFinish.Size = new System.Drawing.Size(200, 50);
            this.btnFinish.TabIndex = 1;
            this.btnFinish.Text = "Finish";
            this.btnFinish.UseVisualStyleBackColor = false;
            this.btnFinish.Click += new System.EventHandler(this.btnFinish_Click);
            // 
            // btnReboot
            // 
            this.btnReboot.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(92)))), ((int)(((byte)(100)))), ((int)(((byte)(255)))));
            this.btnReboot.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReboot.FlatAppearance.BorderSize = 0;
            this.btnReboot.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReboot.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnReboot.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(231)))));
            this.btnReboot.Location = new System.Drawing.Point(300, 500);
            this.btnReboot.Name = "btnReboot";
            this.btnReboot.Size = new System.Drawing.Size(240, 50);
            this.btnReboot.TabIndex = 2;
            this.btnReboot.Text = "Reboot to UEFI";
            this.btnReboot.UseVisualStyleBackColor = false;
            this.btnReboot.Click += new System.EventHandler(this.btnReboot_Click);
            // 
            // WizardCompleteView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(23)))));
            this.Controls.Add(this.btnReboot);
            this.Controls.Add(this.btnFinish);
            this.Controls.Add(this.cardMain);
            this.Name = "WizardCompleteView";
            this.Size = new System.Drawing.Size(800, 600);
            this.cardMain.ResumeLayout(false);
            this.cardMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picQR)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel cardMain;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Label lblKeepUSB;
        private System.Windows.Forms.Label lblInstructions;
        private System.Windows.Forms.PictureBox picQR;
        private System.Windows.Forms.Label lblQRMessage;
        private System.Windows.Forms.Button btnFinish;
        private System.Windows.Forms.Button btnReboot;
    }
}
