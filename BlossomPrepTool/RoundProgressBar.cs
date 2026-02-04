using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BlossomPrepTool
{
    // Custom Rounded Rectangle Progress Bar Control
    public class RoundProgressBar : Control
    {
        private int _value = 0;
        private int _maximum = 100;
        private Color _progressColor = Color.FromArgb(92, 100, 255); // #5c64ff
        private Color _backgroundColor = Color.FromArgb(50, 50, 60);
        private int _cornerRadius = 8;

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
            this.Size = new Size(400, 40);
            this.Height = 30;
        }

        private GraphicsPath RoundedRectangle(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            float r = radius;
            path.AddArc(rect.X, rect.Y, r, r, 180, 90);
            path.AddArc(rect.X + rect.Width - r, rect.Y, r, r, 270, 90);
            path.AddArc(rect.X + rect.Width - r, rect.Y + rect.Height - r, r, r, 0, 90);
            path.AddArc(rect.X, rect.Y + rect.Height - r, r, r, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.Clear(this.BackColor);

            int padding = 2;
            Rectangle backgroundRect = new Rectangle(padding, padding, this.Width - padding * 2, this.Height - padding * 2);

            // Draw background rounded rectangle
            using (var bgPath = RoundedRectangle(backgroundRect, _cornerRadius))
            using (var bgBrush = new SolidBrush(_backgroundColor))
            {
                e.Graphics.FillPath(bgBrush, bgPath);
            }

            // Draw progress rounded rectangle
            float progress = (float)_value / _maximum;
            int progressWidth = (int)(backgroundRect.Width * progress);
            
            if (progressWidth > 0)
            {
                Rectangle progressRect = new Rectangle(backgroundRect.X, backgroundRect.Y, progressWidth, backgroundRect.Height);
                using (var progressPath = RoundedRectangle(progressRect, _cornerRadius))
                using (var progressBrush = new SolidBrush(_progressColor))
                {
                    e.Graphics.FillPath(progressBrush, progressPath);
                }
            }

            // Draw border
            using (var borderPath = RoundedRectangle(backgroundRect, _cornerRadius))
            using (var borderPen = new Pen(Color.FromArgb(70, 70, 80), 1))
            {
                e.Graphics.DrawPath(borderPen, borderPath);
            }

            // Draw percentage text
            string percentText = $"{(int)(progress * 100)}%";
            using (var font = new Font("Segoe UI", 11, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.FromArgb(229, 229, 231)))
            {
                SizeF textSize = e.Graphics.MeasureString(percentText, font);
                e.Graphics.DrawString(percentText, font, brush,
                    this.Width / 2 - textSize.Width / 2, this.Height / 2 - textSize.Height / 2);
            }
        }
    }
}
