namespace VoidVideoGenerator.Models;

using System.Drawing;

/// <summary>
/// Modern color palette and theme system inspired by Tailwind CSS
/// </summary>
public static class ModernTheme
{
    // Primary Colors (Indigo)
    public static readonly Color Primary = Color.FromArgb(99, 102, 241);      // Indigo-500
    public static readonly Color PrimaryHover = Color.FromArgb(79, 70, 229);  // Indigo-600
    public static readonly Color PrimaryLight = Color.FromArgb(165, 180, 252); // Indigo-300
    public static readonly Color PrimaryDark = Color.FromArgb(67, 56, 202);   // Indigo-700
    
    // Background Colors (Slate)
    public static readonly Color Background = Color.FromArgb(15, 23, 42);     // Slate-900
    public static readonly Color Surface = Color.FromArgb(30, 41, 59);        // Slate-800
    public static readonly Color SurfaceVariant = Color.FromArgb(24, 33, 50); // Slate-850 (between 900 and 800)
    public static readonly Color SurfaceHover = Color.FromArgb(51, 65, 85);   // Slate-700
    public static readonly Color SurfaceLight = Color.FromArgb(71, 85, 105);  // Slate-600
    
    // Text Colors
    public static readonly Color TextPrimary = Color.FromArgb(248, 250, 252); // Slate-50
    public static readonly Color TextSecondary = Color.FromArgb(203, 213, 225); // Slate-300
    public static readonly Color TextMuted = Color.FromArgb(148, 163, 184);   // Slate-400
    public static readonly Color TextDisabled = Color.FromArgb(100, 116, 139); // Slate-500
    
    // Accent Colors
    public static readonly Color Success = Color.FromArgb(34, 197, 94);       // Green-500
    public static readonly Color SuccessLight = Color.FromArgb(74, 222, 128); // Green-400
    public static readonly Color Warning = Color.FromArgb(251, 146, 60);      // Orange-400
    public static readonly Color WarningLight = Color.FromArgb(251, 191, 36); // Amber-400
    public static readonly Color Error = Color.FromArgb(239, 68, 68);         // Red-500
    public static readonly Color ErrorLight = Color.FromArgb(248, 113, 113);  // Red-400
    public static readonly Color Danger = Color.FromArgb(220, 38, 38);        // Red-600 (darker red for danger)
    public static readonly Color Info = Color.FromArgb(59, 130, 246);         // Blue-500
    public static readonly Color InfoLight = Color.FromArgb(96, 165, 250);    // Blue-400
    
    // Borders & Dividers
    public static readonly Color Border = Color.FromArgb(51, 65, 85);         // Slate-700
    public static readonly Color BorderLight = Color.FromArgb(71, 85, 105);   // Slate-600
    public static readonly Color BorderFocus = Primary;
    
    // Shadows
    public static readonly Color Shadow = Color.FromArgb(30, 0, 0, 0);        // 30% opacity black
    public static readonly Color ShadowStrong = Color.FromArgb(60, 0, 0, 0);  // 60% opacity black
    
    // Special Colors
    public static readonly Color Overlay = Color.FromArgb(100, 0, 0, 0);      // Semi-transparent overlay
    public static readonly Color Transparent = Color.Transparent;
}

/// <summary>
/// Modern typography system
/// </summary>
public static class ModernFonts
{
    // Headings
    public static readonly Font H1 = new Font("Segoe UI", 32, FontStyle.Bold);
    public static readonly Font H2 = new Font("Segoe UI", 24, FontStyle.Bold);
    public static readonly Font H3 = new Font("Segoe UI", 18, FontStyle.Bold);
    public static readonly Font H4 = new Font("Segoe UI", 16, FontStyle.Bold);
    public static readonly Font H5 = new Font("Segoe UI", 14, FontStyle.Bold);
    
    // Body Text
    public static readonly Font Body = new Font("Segoe UI", 14, FontStyle.Regular);
    public static readonly Font BodyBold = new Font("Segoe UI", 14, FontStyle.Bold);
    public static readonly Font Small = new Font("Segoe UI", 12, FontStyle.Regular);
    public static readonly Font SmallBold = new Font("Segoe UI", 12, FontStyle.Bold);
    public static readonly Font Tiny = new Font("Segoe UI", 10, FontStyle.Regular);
    
    // Monospace (for code/logs)
    public static readonly Font Code = new Font("Consolas", 12, FontStyle.Regular);
    public static readonly Font CodeSmall = new Font("Consolas", 10, FontStyle.Regular);
    
    // Icon Font
    public static readonly Font Icon = new Font("Segoe MDL2 Assets", 16);
    public static readonly Font IconLarge = new Font("Segoe MDL2 Assets", 24);
}

/// <summary>
/// Spacing system for consistent layouts
/// </summary>
public static class Spacing
{
    public const int XS = 4;
    public const int SM = 8;
    public const int MD = 16;
    public const int LG = 24;
    public const int XL = 32;
    public const int XXL = 48;
}

/// <summary>
/// Border radius values
/// </summary>
public static class BorderRadius
{
    public const int None = 0;
    public const int SM = 4;
    public const int MD = 8;
    public const int LG = 12;
    public const int XL = 16;
    public const int XXL = 24;
    public const int Full = 9999; // Fully rounded
}

/// <summary>
/// Icon constants using Segoe MDL2 Assets
/// </summary>
public static class Icons
{
    public const string Play = "\uE768";
    public const string Pause = "\uE769";
    public const string Stop = "\uE71A";
    public const string Settings = "\uE713";
    public const string Add = "\uE710";
    public const string Remove = "\uE738";
    public const string Edit = "\uE70F";
    public const string Save = "\uE74E";
    public const string Folder = "\uE8B7";
    public const string Image = "\uE91B";
    public const string Video = "\uE714";
    public const string Audio = "\uE8D6";
    public const string Download = "\uE896";
    public const string Upload = "\uE898";
    public const string Check = "\uE73E";
    public const string Close = "\uE711";
    public const string Search = "\uE721";
    public const string Filter = "\uE71C";
    public const string More = "\uE712";
    public const string Info = "\uE946";
    public const string Warning = "\uE7BA";
    public const string Error = "\uE783";
    public const string Success = "\uE73E";
    public const string Generate = "\uE768";
    public const string Refresh = "\uE72C";
    public const string Delete = "\uE74D";
    public const string Copy = "\uE8C8";
    public const string Paste = "\uE77F";
}
