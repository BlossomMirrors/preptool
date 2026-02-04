using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace BlossomPrepTool
{
    public partial class LanguageSelectionForm : Form
    {
        private CultureInfo _selectedCulture;

        public CultureInfo SelectedCulture => _selectedCulture;

        public LanguageSelectionForm()
        {
            InitializeComponent();
            _selectedCulture = Localizer.CurrentCulture;
        }

        private void InitializeComponent()
        {
            this.Text = "Select Language";
            this.Size = new Size(400, 250);
            this.StartPosition = FormStartPosition.CenterParent;
            this.ShowIcon = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            var panelMain = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(33, 33, 38),
                Padding = new Padding(20)
            };

            var lblTitle = new Label
            {
                Text = "Choose your language:",
                AutoSize = true,
                ForeColor = Color.FromArgb(229, 229, 231),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 20)
            };

            var flowLayout = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                AutoScroll = true,
                BackColor = Color.FromArgb(33, 33, 38),
                Margin = new Padding(0, 60, 0, 80)
            };

            foreach (var culture in Localizer.AvailableCultures)
            {
                var btn = new Button
                {
                    Text = culture.DisplayName + " (" + culture.Name + ")",
                    Size = new Size(360, 50),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = culture == Localizer.CurrentCulture
                        ? Color.FromArgb(92, 100, 255)
                        : Color.FromArgb(50, 50, 55),
                    ForeColor = Color.FromArgb(229, 229, 231),
                    Font = new Font("Segoe UI", 11),
                    Cursor = Cursors.Hand,
                    Tag = culture
                };

                btn.FlatAppearance.BorderColor = Color.FromArgb(92, 100, 255);
                btn.FlatAppearance.BorderSize = culture == Localizer.CurrentCulture ? 2 : 1;

                btn.Click += (s, e) =>
                {
                    _selectedCulture = culture;
                    Localizer.CurrentCulture = culture;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                };

                btn.MouseEnter += (s, e) =>
                {
                    btn.BackColor = Color.FromArgb(92, 100, 255);
                };

                btn.MouseLeave += (s, e) =>
                {
                    if (culture != _selectedCulture)
                        btn.BackColor = Color.FromArgb(50, 50, 55);
                };

                flowLayout.Controls.Add(btn);
            }

            var btnCancel = new Button
            {
                Text = "Cancel",
                Size = new Size(100, 40),
                Location = new Point(20, 180),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(50, 50, 55),
                ForeColor = Color.FromArgb(229, 229, 231),
                Font = new Font("Segoe UI", 10),
                Cursor = Cursors.Hand
            };

            btnCancel.FlatAppearance.BorderColor = Color.FromArgb(92, 100, 255);
            btnCancel.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            panelMain.Controls.Add(lblTitle);
            panelMain.Controls.Add(flowLayout);
            panelMain.Controls.Add(btnCancel);

            this.Controls.Add(panelMain);
        }
    }
}
