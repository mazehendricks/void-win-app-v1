namespace VoidVideoGenerator.Models;

/// <summary>
/// Theme color definitions for light and dark modes
/// </summary>
public static class ThemeColors
{
    public static class Light
    {
        public static Color Background = Color.White;
        public static Color Surface = Color.FromArgb(240, 240, 240);
        public static Color Primary = Color.FromArgb(0, 120, 215);
        public static Color Text = Color.Black;
        public static Color TextSecondary = Color.FromArgb(64, 64, 64);
        public static Color Border = Color.FromArgb(200, 200, 200);
        public static Color InputBackground = Color.White;
        public static Color ButtonBackground = Color.FromArgb(225, 225, 225);
    }

    public static class Dark
    {
        public static Color Background = Color.FromArgb(32, 32, 32);
        public static Color Surface = Color.FromArgb(45, 45, 45);
        public static Color Primary = Color.FromArgb(0, 150, 255);
        public static Color Text = Color.FromArgb(240, 240, 240);
        public static Color TextSecondary = Color.FromArgb(180, 180, 180);
        public static Color Border = Color.FromArgb(60, 60, 60);
        public static Color InputBackground = Color.FromArgb(50, 50, 50);
        public static Color ButtonBackground = Color.FromArgb(60, 60, 60);
    }
}
