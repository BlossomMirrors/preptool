namespace BlossomPrepTool
{
    partial class WizardModeSelectionView
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
            this.lblModeTitle = new System.Windows.Forms.Label();
            this.btnSimpleMode = new System.Windows.Forms.Button();
            this.btnDualBootMode = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblModeTitle
            // 
            this.lblModeTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblModeTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblModeTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(231)))));
            this.lblModeTitle.Location = new System.Drawing.Point(0, 0);
            this.lblModeTitle.Name = "lblModeTitle";
            this.lblModeTitle.Padding = new System.Windows.Forms.Padding(20, 20, 20, 10);
            this.lblModeTitle.Size = new System.Drawing.Size(800, 60);
            this.lblModeTitle.TabIndex = 0;
            this.lblModeTitle.Text = "Choose Setup Mode";
            // 
            // btnSimpleMode
            // 
            this.btnSimpleMode.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnSimpleMode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSimpleMode.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnSimpleMode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(231)))));
            this.btnSimpleMode.Location = new System.Drawing.Point(0, 60);
            this.btnSimpleMode.Name = "btnSimpleMode";
            this.btnSimpleMode.Padding = new System.Windows.Forms.Padding(20, 10, 20, 10);
            this.btnSimpleMode.Size = new System.Drawing.Size(800, 60);
            this.btnSimpleMode.TabIndex = 1;
            this.btnSimpleMode.Text = "Just Flash USB";
            this.btnSimpleMode.UseVisualStyleBackColor = true;
            this.btnSimpleMode.Click += new System.EventHandler(this.btnSimpleMode_Click);
            // 
            // btnDualBootMode
            // 
            this.btnDualBootMode.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnDualBootMode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDualBootMode.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnDualBootMode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(231)))));
            this.btnDualBootMode.Location = new System.Drawing.Point(0, 120);
            this.btnDualBootMode.Name = "btnDualBootMode";
            this.btnDualBootMode.Padding = new System.Windows.Forms.Padding(20, 10, 20, 10);
            this.btnDualBootMode.Size = new System.Drawing.Size(800, 60);
            this.btnDualBootMode.TabIndex = 2;
            this.btnDualBootMode.Text = "Dual-Boot Setup";
            this.btnDualBootMode.UseVisualStyleBackColor = true;
            this.btnDualBootMode.Click += new System.EventHandler(this.btnDualBootMode_Click);
            // 
            // WizardModeSelectionView
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(23)))));
            this.Controls.Add(this.btnDualBootMode);
            this.Controls.Add(this.btnSimpleMode);
            this.Controls.Add(this.lblModeTitle);
            this.Name = "WizardModeSelectionView";
            this.Size = new System.Drawing.Size(800, 600);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblModeTitle;
        private System.Windows.Forms.Button btnSimpleMode;
        private System.Windows.Forms.Button btnDualBootMode;
    }
}
