using System.Drawing;

namespace BlossomPrepTool
{
    /// <summary>
    /// Centralized design system for all wizard views
    /// </summary>
    public static class WizardDesignSystem
    {
        // Colors
        public static readonly Color DarkBg = Color.FromArgb(20, 20, 23);
        public static readonly Color DarkPanel = Color.FromArgb(20, 20, 23);
        public static readonly Color CardBg = Color.FromArgb(41, 41, 46);
        public static readonly Color AccentColor = Color.FromArgb(92, 100, 255);
        public static readonly Color TextColor = Color.FromArgb(229, 229, 231);
        public static readonly Color TextSecondary = Color.FromArgb(161, 161, 170);
        public static readonly Color BorderColor = Color.FromArgb(80, 80, 88);

        // Typography
        public static readonly Font HeadingFont = new Font("Georgia", 20F, FontStyle.Bold);
        public static readonly Font SubHeadingFont = new Font("Georgia", 14F, FontStyle.Bold);
        public static readonly Font BodyFont = new Font("Segoe UI", 11F);
        public static readonly Font SmallFont = new Font("Segoe UI", 9F);
        public static readonly Font ButtonFont = new Font("Segoe UI", 11F);

        // Spacing
        public const int CardPadding = 30;
        public const int CardMargin = 40;
        public const int CardRadius = 12;
        public const int ButtonRadius = 8;
    }
}
