namespace BlossomPrepTool
{
    partial class Main
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.close = new System.Windows.Forms.PictureBox();
            this.maximize_normalize = new System.Windows.Forms.PictureBox();
            this.minimize = new System.Windows.Forms.PictureBox();
            this.wizardWelcomeView = new BlossomPrepTool.WizardWelcomeView();
            this.wizardModeSelectionView = new BlossomPrepTool.WizardModeSelectionView();
            this.wizardUsbSelectionView = new BlossomPrepTool.WizardUsbSelectionView();
            this.wizardIsoSourceView = new BlossomPrepTool.WizardIsoSourceView();
            this.wizardDownloadView = new BlossomPrepTool.WizardDownloadView();
            this.wizardFlashView = new BlossomPrepTool.WizardFlashView();
            this.wizardPartitionView = new BlossomPrepTool.WizardPartitionView();
            this.wizardSettingsView = new BlossomPrepTool.WizardSettingsView();
            this.wizardWinBTRFSView = new BlossomPrepTool.WizardWinBTRFSView();
            this.wizardCompleteView = new BlossomPrepTool.WizardCompleteView();
            this.btnDownloadISO = new System.Windows.Forms.Button();
            this.btnFlashUSB = new System.Windows.Forms.Button();
            this.btnResizePartition = new System.Windows.Forms.Button();
            this.btnInstallWinBTRFS = new System.Windows.Forms.Button();
            this.btnRefreshUSB = new System.Windows.Forms.Button();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.lblISOStatus = new System.Windows.Forms.Label();
            this.lblFlashStatus = new System.Windows.Forms.Label();
            this.lblPartitionStatus = new System.Windows.Forms.Label();
            this.lblWinBTRFSStatus = new System.Windows.Forms.Label();
            this.cmbUSBDrives = new System.Windows.Forms.ComboBox();
            this.numPartitionSize = new System.Windows.Forms.NumericUpDown();
            this.lstLog = new System.Windows.Forms.ListBox();
            this.lblUSBDrives = new System.Windows.Forms.Label();
            this.lblPartitionSize = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.close)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maximize_normalize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minimize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPartitionSize)).BeginInit();
            this.SuspendLayout();
            // 
            // close
            // 
            this.close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.close.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("close.BackgroundImage")));
            this.close.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.close.Location = new System.Drawing.Point(782, 9);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(16, 16);
            this.close.TabIndex = 0;
            this.close.TabStop = false;
            this.close.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // maximize_normalize
            // 
            this.maximize_normalize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.maximize_normalize.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("maximize_normalize.BackgroundImage")));
            this.maximize_normalize.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.maximize_normalize.Location = new System.Drawing.Point(752, 9);
            this.maximize_normalize.Name = "maximize_normalize";
            this.maximize_normalize.Size = new System.Drawing.Size(16, 16);
            this.maximize_normalize.TabIndex = 1;
            this.maximize_normalize.TabStop = false;
            this.maximize_normalize.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // minimize
            // 
            this.minimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.minimize.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("minimize.BackgroundImage")));
            this.minimize.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.minimize.Location = new System.Drawing.Point(721, 9);
            this.minimize.Name = "minimize";
            this.minimize.Size = new System.Drawing.Size(16, 16);
            this.minimize.TabIndex = 2;
            this.minimize.TabStop = false;
            this.minimize.Click += new System.EventHandler(this.pictureBox3_Click);
            // 
            // wizardWelcomeView
            // 
            this.wizardWelcomeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardWelcomeView.Location = new System.Drawing.Point(10, 30);
            this.wizardWelcomeView.Name = "wizardWelcomeView";
            this.wizardWelcomeView.Size = new System.Drawing.Size(790, 580);
            this.wizardWelcomeView.TabIndex = 100;
            this.wizardWelcomeView.Visible = false;
            // 
            // wizardModeSelectionView
            // 
            this.wizardModeSelectionView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardModeSelectionView.Location = new System.Drawing.Point(10, 30);
            this.wizardModeSelectionView.Name = "wizardModeSelectionView";
            this.wizardModeSelectionView.Size = new System.Drawing.Size(790, 580);
            this.wizardModeSelectionView.TabIndex = 100;
            this.wizardModeSelectionView.Visible = false;
            // 
            // wizardUsbSelectionView
            // 
            this.wizardUsbSelectionView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardUsbSelectionView.Location = new System.Drawing.Point(10, 30);
            this.wizardUsbSelectionView.Name = "wizardUsbSelectionView";
            this.wizardUsbSelectionView.Size = new System.Drawing.Size(790, 580);
            this.wizardUsbSelectionView.TabIndex = 101;
            this.wizardUsbSelectionView.Visible = false;
            // 
            // wizardIsoSourceView
            // 
            this.wizardIsoSourceView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardIsoSourceView.Location = new System.Drawing.Point(10, 30);
            this.wizardIsoSourceView.Name = "wizardIsoSourceView";
            this.wizardIsoSourceView.Size = new System.Drawing.Size(790, 580);
            this.wizardIsoSourceView.TabIndex = 105;
            this.wizardIsoSourceView.Visible = false;
            // 
            // wizardDownloadView
            // 
            this.wizardDownloadView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardDownloadView.Location = new System.Drawing.Point(10, 30);
            this.wizardDownloadView.Name = "wizardDownloadView";
            this.wizardDownloadView.Size = new System.Drawing.Size(790, 580);
            this.wizardDownloadView.TabIndex = 102;
            this.wizardDownloadView.Visible = false;
            // 
            // wizardFlashView
            // 
            this.wizardFlashView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardFlashView.Location = new System.Drawing.Point(10, 30);
            this.wizardFlashView.Name = "wizardFlashView";
            this.wizardFlashView.Size = new System.Drawing.Size(790, 580);
            this.wizardFlashView.TabIndex = 103;
            this.wizardFlashView.Visible = false;
            // 
            // wizardPartitionView
            // 
            this.wizardPartitionView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardPartitionView.Location = new System.Drawing.Point(10, 30);
            this.wizardPartitionView.Name = "wizardPartitionView";
            this.wizardPartitionView.Size = new System.Drawing.Size(790, 580);
            this.wizardPartitionView.TabIndex = 105;
            this.wizardPartitionView.Visible = false;
            // 
            // wizardSettingsView
            // 
            this.wizardSettingsView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardSettingsView.Location = new System.Drawing.Point(10, 30);
            this.wizardSettingsView.Name = "wizardSettingsView";
            this.wizardSettingsView.Size = new System.Drawing.Size(790, 580);
            this.wizardSettingsView.TabIndex = 106;
            this.wizardSettingsView.Visible = false;
            // 
            // wizardWinBTRFSView
            // 
            this.wizardWinBTRFSView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardWinBTRFSView.Location = new System.Drawing.Point(10, 30);
            this.wizardWinBTRFSView.Name = "wizardWinBTRFSView";
            this.wizardWinBTRFSView.Size = new System.Drawing.Size(790, 580);
            this.wizardWinBTRFSView.TabIndex = 107;
            this.wizardWinBTRFSView.Visible = false;
            // 
            // wizardCompleteView
            // 
            this.wizardCompleteView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardCompleteView.Location = new System.Drawing.Point(10, 30);
            this.wizardCompleteView.Name = "wizardCompleteView";
            this.wizardCompleteView.Size = new System.Drawing.Size(790, 580);
            this.wizardCompleteView.TabIndex = 104;
            this.wizardCompleteView.Visible = false;
            // 
            // btnDownloadISO
            // 
            this.btnDownloadISO.Location = new System.Drawing.Point(13, 45);
            this.btnDownloadISO.Name = "btnDownloadISO";
            this.btnDownloadISO.Size = new System.Drawing.Size(100, 25);
            this.btnDownloadISO.TabIndex = 3;
            this.btnDownloadISO.Text = "Download ISO";
            this.btnDownloadISO.UseVisualStyleBackColor = true;
            this.btnDownloadISO.Click += new System.EventHandler(this.btnDownloadISO_Click);
            // 
            // btnFlashUSB
            // 
            this.btnFlashUSB.Location = new System.Drawing.Point(13, 105);
            this.btnFlashUSB.Name = "btnFlashUSB";
            this.btnFlashUSB.Size = new System.Drawing.Size(100, 25);
            this.btnFlashUSB.TabIndex = 8;
            this.btnFlashUSB.Text = "Flash USB";
            this.btnFlashUSB.UseVisualStyleBackColor = true;
            this.btnFlashUSB.Click += new System.EventHandler(this.btnFlashUSB_Click);
            // 
            // btnResizePartition
            // 
            this.btnResizePartition.Location = new System.Drawing.Point(13, 165);
            this.btnResizePartition.Name = "btnResizePartition";
            this.btnResizePartition.Size = new System.Drawing.Size(100, 25);
            this.btnResizePartition.TabIndex = 12;
            this.btnResizePartition.Text = "Resize Partition";
            this.btnResizePartition.UseVisualStyleBackColor = true;
            this.btnResizePartition.Click += new System.EventHandler(this.btnResizePartition_Click);
            // 
            // btnInstallWinBTRFS
            // 
            this.btnInstallWinBTRFS.Location = new System.Drawing.Point(13, 200);
            this.btnInstallWinBTRFS.Name = "btnInstallWinBTRFS";
            this.btnInstallWinBTRFS.Size = new System.Drawing.Size(100, 25);
            this.btnInstallWinBTRFS.TabIndex = 14;
            this.btnInstallWinBTRFS.Text = "Install WinBTRFS";
            this.btnInstallWinBTRFS.UseVisualStyleBackColor = true;
            this.btnInstallWinBTRFS.Click += new System.EventHandler(this.btnInstallWinBTRFS_Click);
            // 
            // btnRefreshUSB
            // 
            this.btnRefreshUSB.Location = new System.Drawing.Point(289, 77);
            this.btnRefreshUSB.Name = "btnRefreshUSB";
            this.btnRefreshUSB.Size = new System.Drawing.Size(80, 20);
            this.btnRefreshUSB.TabIndex = 7;
            this.btnRefreshUSB.Text = "Refresh";
            this.btnRefreshUSB.UseVisualStyleBackColor = true;
            this.btnRefreshUSB.Click += new System.EventHandler(this.btnRefreshUSB_Click);
            // 
            // btnClearLog
            // 
            this.btnClearLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClearLog.Location = new System.Drawing.Point(13, 580);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(80, 25);
            this.btnClearLog.TabIndex = 17;
            this.btnClearLog.Text = "Clear Log";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            // 
            // lblISOStatus
            // 
            this.lblISOStatus.AutoSize = true;
            this.lblISOStatus.Location = new System.Drawing.Point(120, 50);
            this.lblISOStatus.Name = "lblISOStatus";
            this.lblISOStatus.Size = new System.Drawing.Size(38, 13);
            this.lblISOStatus.TabIndex = 4;
            this.lblISOStatus.Text = "Ready";
            // 
            // lblFlashStatus
            // 
            this.lblFlashStatus.AutoSize = true;
            this.lblFlashStatus.Location = new System.Drawing.Point(120, 110);
            this.lblFlashStatus.Name = "lblFlashStatus";
            this.lblFlashStatus.Size = new System.Drawing.Size(38, 13);
            this.lblFlashStatus.TabIndex = 9;
            this.lblFlashStatus.Text = "Ready";
            // 
            // lblPartitionStatus
            // 
            this.lblPartitionStatus.AutoSize = true;
            this.lblPartitionStatus.Location = new System.Drawing.Point(120, 170);
            this.lblPartitionStatus.Name = "lblPartitionStatus";
            this.lblPartitionStatus.Size = new System.Drawing.Size(38, 13);
            this.lblPartitionStatus.TabIndex = 13;
            this.lblPartitionStatus.Text = "Ready";
            // 
            // lblWinBTRFSStatus
            // 
            this.lblWinBTRFSStatus.AutoSize = true;
            this.lblWinBTRFSStatus.Location = new System.Drawing.Point(120, 205);
            this.lblWinBTRFSStatus.Name = "lblWinBTRFSStatus";
            this.lblWinBTRFSStatus.Size = new System.Drawing.Size(38, 13);
            this.lblWinBTRFSStatus.TabIndex = 15;
            this.lblWinBTRFSStatus.Text = "Ready";
            // 
            // cmbUSBDrives
            // 
            this.cmbUSBDrives.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUSBDrives.FormattingEnabled = true;
            this.cmbUSBDrives.Location = new System.Drawing.Point(82, 77);
            this.cmbUSBDrives.Name = "cmbUSBDrives";
            this.cmbUSBDrives.Size = new System.Drawing.Size(200, 21);
            this.cmbUSBDrives.TabIndex = 6;
            // 
            // numPartitionSize
            // 
            this.numPartitionSize.Location = new System.Drawing.Point(90, 138);
            this.numPartitionSize.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numPartitionSize.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numPartitionSize.Name = "numPartitionSize";
            this.numPartitionSize.Size = new System.Drawing.Size(80, 20);
            this.numPartitionSize.TabIndex = 11;
            this.numPartitionSize.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // lstLog
            // 
            this.lstLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstLog.Location = new System.Drawing.Point(13, 240);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(785, 329);
            this.lstLog.TabIndex = 16;
            // 
            // lblUSBDrives
            // 
            this.lblUSBDrives.AutoSize = true;
            this.lblUSBDrives.Location = new System.Drawing.Point(13, 80);
            this.lblUSBDrives.Name = "lblUSBDrives";
            this.lblUSBDrives.Size = new System.Drawing.Size(65, 13);
            this.lblUSBDrives.TabIndex = 5;
            this.lblUSBDrives.Text = "USB Drives:";
            // 
            // lblPartitionSize
            // 
            this.lblPartitionSize.AutoSize = true;
            this.lblPartitionSize.Location = new System.Drawing.Point(13, 140);
            this.lblPartitionSize.Name = "lblPartitionSize";
            this.lblPartitionSize.Size = new System.Drawing.Size(54, 13);
            this.lblPartitionSize.TabIndex = 10;
            this.lblPartitionSize.Text = "Size (GB):";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(38)))));
            this.ClientSize = new System.Drawing.Size(810, 620);
            this.Controls.Add(this.btnClearLog);
            this.Controls.Add(this.lstLog);
            this.Controls.Add(this.lblWinBTRFSStatus);
            this.Controls.Add(this.btnInstallWinBTRFS);
            this.Controls.Add(this.lblPartitionStatus);
            this.Controls.Add(this.btnResizePartition);
            this.Controls.Add(this.numPartitionSize);
            this.Controls.Add(this.lblPartitionSize);
            this.Controls.Add(this.lblFlashStatus);
            this.Controls.Add(this.btnFlashUSB);
            this.Controls.Add(this.btnRefreshUSB);
            this.Controls.Add(this.cmbUSBDrives);
            this.Controls.Add(this.lblUSBDrives);
            this.Controls.Add(this.lblISOStatus);
            this.Controls.Add(this.wizardCompleteView);
            this.Controls.Add(this.wizardWinBTRFSView);
            this.Controls.Add(this.wizardSettingsView);
            this.Controls.Add(this.wizardPartitionView);
            this.Controls.Add(this.wizardFlashView);
            this.Controls.Add(this.wizardDownloadView);
            this.Controls.Add(this.wizardIsoSourceView);
            this.Controls.Add(this.wizardUsbSelectionView);
            this.Controls.Add(this.wizardModeSelectionView);
            this.Controls.Add(this.wizardWelcomeView);
            this.Controls.Add(this.btnDownloadISO);
            this.Controls.Add(this.minimize);
            this.Controls.Add(this.maximize_normalize);
            this.Controls.Add(this.close);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(231)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main";
            this.Padding = new System.Windows.Forms.Padding(10, 30, 10, 10);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BlossomPrep Tool";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Main_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Main_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Main_MouseUp);
            ((System.ComponentModel.ISupportInitialize)(this.close)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maximize_normalize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minimize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPartitionSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox close;
        private System.Windows.Forms.PictureBox maximize_normalize;
        private System.Windows.Forms.PictureBox minimize;
        private WizardWelcomeView wizardWelcomeView;
        private WizardModeSelectionView wizardModeSelectionView;
        private WizardUsbSelectionView wizardUsbSelectionView;
        private WizardIsoSourceView wizardIsoSourceView;
        private WizardDownloadView wizardDownloadView;
        private WizardFlashView wizardFlashView;
        private WizardPartitionView wizardPartitionView;
        private WizardSettingsView wizardSettingsView;
        private WizardWinBTRFSView wizardWinBTRFSView;
        private WizardCompleteView wizardCompleteView;
        private System.Windows.Forms.Button btnDownloadISO;
        private System.Windows.Forms.Label lblISOStatus;
        private System.Windows.Forms.Label lblUSBDrives;
        private System.Windows.Forms.ComboBox cmbUSBDrives;
        private System.Windows.Forms.Button btnRefreshUSB;
        private System.Windows.Forms.Button btnFlashUSB;
        private System.Windows.Forms.Label lblFlashStatus;
        private System.Windows.Forms.Label lblPartitionSize;
        private System.Windows.Forms.NumericUpDown numPartitionSize;
        private System.Windows.Forms.Button btnResizePartition;
        private System.Windows.Forms.Label lblPartitionStatus;
        private System.Windows.Forms.Button btnInstallWinBTRFS;
        private System.Windows.Forms.Label lblWinBTRFSStatus;
        private System.Windows.Forms.ListBox lstLog;
        private System.Windows.Forms.Button btnClearLog;
    }
}

