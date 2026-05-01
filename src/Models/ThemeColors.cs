namespace VoidVideoGenerator.Models;

/// <summary>
/// Modern theme color definitions for light and dark modes
/// </summary>
public static class ThemeColors
{
    public static class Dark
    {
        // Modern dark theme with purple/blue accents
        public static Color Background = Color.FromArgb(18, 18, 18);      // Very dark background
        public static Color Surface = Color.FromArgb(28, 28, 30);         // Slightly lighter surface
        public static Color SurfaceVariant = Color.FromArgb(38, 38, 42);  // Card/panel background
        public static Color Text = Color.FromArgb(240, 240, 245);         // Off-white text
        public static Color TextSecondary = Color.FromArgb(160, 160, 170); // Muted text
        public static Color Primary = Color.FromArgb(138, 43, 226);       // Purple accent
        public static Color PrimaryHover = Color.FromArgb(155, 70, 235);  // Lighter purple on hover
        public static Color Secondary = Color.FromArgb(0, 150, 255);      // Blue accent
        public static Color Border = Color.FromArgb(50, 50, 55);          // Subtle border
        public static Color InputBackground = Color.FromArgb(32, 32, 35); // Input fields
        public static Color ButtonBackground = Color.FromArgb(45, 45, 50); // Buttons
        public static Color Success = Color.FromArgb(76, 175, 80);        // Green
        public static Color Warning = Color.FromArgb(255, 152, 0);        // Orange
        public static Color Error = Color.FromArgb(244, 67, 54);          // Red
    }

    public static class Light
    {
        // Modern light theme
        public static Color Background = Color.FromArgb(248, 249, 250);
        public static Color Surface = Color.White;
        public static Color SurfaceVariant = Color.FromArgb(245, 245, 247);
        public static Color Text = Color.FromArgb(20, 20, 25);
        public static Color TextSecondary = Color.FromArgb(100, 100, 110);
        public static Color Primary = Color.FromArgb(138, 43, 226);
        public static Color PrimaryHover = Color.FromArgb(155, 70, 235);
        public static Color Secondary = Color.FromArgb(0, 120, 215);
        public static Color Border = Color.FromArgb(220, 220, 225);
        public static Color InputBackground = Color.White;
        public static Color ButtonBackground = Color.FromArgb(240, 240, 242);
        public static Color Success = Color.FromArgb(76, 175, 80);
        public static Color Warning = Color.FromArgb(255, 152, 0);
        public static Color Error = Color.FromArgb(244, 67, 54);
    }
}
