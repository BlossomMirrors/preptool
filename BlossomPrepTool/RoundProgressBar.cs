using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BlossomPrepTool
{
    // Custom Round Progress Bar Control
    public class RoundProgressBar : Control
    {
        private int _value = 0;
        private int _maximum = 100;
        private Color _progressColor = Color.FromArgb(92, 100, 255); // #5c64ff
        private Color _backgroundColor = Color.FromArgb(50, 50, 60);

        public int Value
        {
            get { return _value; }
            set
            {
                _value = Math.Max(0, Math.Min(value, _maximum));
                this.Invalidate();
            }
        }

        public int Maximum
        {
            get { return _maximum; }
            set { _maximum = Math.Max(1, value); }
        }

        public RoundProgressBar()
        {
            this.DoubleBuffered = true;
            this.Size = new Size(200, 200);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.Clear(this.BackColor);

            int centerX = this.Width / 2;
            int centerY = this.Height / 2;
            int radius = Math.Min(this.Width, this.Height) / 2 - 10;

            // Draw background circle
            using (var pen = new Pen(_backgroundColor, 8))
            {
                e.Graphics.DrawArc(pen, centerX - radius, centerY - radius, radius * 2, radius * 2, 0, 360);
            }

            // Draw progress arc
            float progress = (float)_value / _maximum;
            float sweepAngle = 360 * progress;

            using (var pen = new Pen(_progressColor, 8) { StartCap = LineCap.Round, EndCap = LineCap.Round })
            {
                e.Graphics.DrawArc(pen, centerX - radius, centerY - radius, radius * 2, radius * 2, -90, sweepAngle);
            }

            // Draw center circle
            int innerRadius = radius - 15;
            using (var brush = new SolidBrush(Color.FromArgb(33, 33, 38)))
            {
                e.Graphics.FillEllipse(brush, centerX - innerRadius, centerY - innerRadius, innerRadius * 2, innerRadius * 2);
            }

            // Draw percentage text
            string percentText = $"{(int)(progress * 100)}%";
            using (var font = new Font("Segoe UI", 24, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.FromArgb(229, 229, 231)))
            {
                SizeF textSize = e.Graphics.MeasureString(percentText, font);
                e.Graphics.DrawString(percentText, font, brush,
                    centerX - textSize.Width / 2, centerY - textSize.Height / 2);
            }
        }
    }
}
