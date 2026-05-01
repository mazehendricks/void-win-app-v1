namespace VoidVideoGenerator.Models;

/// <summary>
/// Modern theme color definitions - Updated to use ModernTheme palette
/// Maintains backward compatibility with existing code
/// </summary>
public static class ThemeColors
{
    public static class Dark
    {
        // Modern dark theme using Indigo/Slate palette
        public static Color Background = ModernTheme.Background;           // Slate-900
        public static Color Surface = ModernTheme.Surface;                 // Slate-800
        public static Color SurfaceVariant = ModernTheme.SurfaceHover;     // Slate-700
        public static Color Text = ModernTheme.TextPrimary;                // Slate-50
        public static Color TextSecondary = ModernTheme.TextSecondary;     // Slate-300
        public static Color Primary = ModernTheme.Primary;                 // Indigo-500
        public static Color PrimaryHover = ModernTheme.PrimaryHover;       // Indigo-600
        public static Color Secondary = ModernTheme.Info;                  // Blue-500
        public static Color Border = ModernTheme.Border;                   // Slate-700
        public static Color InputBackground = ModernTheme.Surface;         // Slate-800
        public static Color ButtonBackground = ModernTheme.SurfaceHover;   // Slate-700
        public static Color Success = ModernTheme.Success;                 // Green-500
        public static Color Warning = ModernTheme.Warning;                 // Orange-400
        public static Color Error = ModernTheme.Error;                     // Red-500
    }

    public static class Light
    {
        // Light theme (for future use)
        public static Color Background = Color.FromArgb(248, 249, 250);
        public static Color Surface = Color.White;
        public static Color SurfaceVariant = Color.FromArgb(245, 245, 247);
        public static Color Text = Color.FromArgb(20, 20, 25);
        public static Color TextSecondary = Color.FromArgb(100, 100, 110);
        public static Color Primary = ModernTheme.Primary;
        public static Color PrimaryHover = ModernTheme.PrimaryHover;
        public static Color Secondary = ModernTheme.Info;
        public static Color Border = Color.FromArgb(220, 220, 225);
        public static Color InputBackground = Color.White;
        public static Color ButtonBackground = Color.FromArgb(240, 240, 242);
        public static Color Success = ModernTheme.Success;
        public static Color Warning = ModernTheme.Warning;
        public static Color Error = ModernTheme.Error;
    }
}
