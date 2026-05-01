namespace VoidVideoGenerator.Components;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using VoidVideoGenerator.Models;

/// <summary>
/// Modern styled button with smooth hover effects and multiple style variants
/// </summary>
public class ModernButton : Button
{
    public enum ButtonStyle
    {
        Primary,    // Filled with primary color
        Secondary,  // Filled with secondary color
        Outline,    // Transparent with border
        Ghost,      // Transparent, no border
        Danger,     // Red/error color
        Success     // Green/success color
    }
    
    private ButtonStyle _style = ButtonStyle.Primary;
    private bool _isHovered = false;
    private bool _isPressed = false;
    private int _borderRadius = BorderRadius.MD;
    
    public ModernButton()
    {
        FlatStyle = FlatStyle.Flat;
        Font = ModernFonts.BodyBold;
        Cursor = Cursors.Hand;
        Padding = new Padding(Spacing.MD, Spacing.SM, Spacing.MD, Spacing.SM);
        Height = 40;
        
        // Remove default border
        FlatAppearance.BorderSize = 0;
        
        // Enable double buffering for smooth rendering
        SetStyle(ControlStyles.UserPaint | 
                 ControlStyles.AllPaintingInWmPaint | 
                 ControlStyles.OptimizedDoubleBuffer, true);
        
        // Add hover effects
        MouseEnter += (s, e) => { _isHovered = true; Invalidate(); };
        MouseLeave += (s, e) => { _isHovered = false; _isPressed = false; Invalidate(); };
        MouseDown += (s, e) => { _isPressed = true; Invalidate(); };
        MouseUp += (s, e) => { _isPressed = false; Invalidate(); };
        
        ApplyStyle();
    }
    
    public ButtonStyle Style
    {
        get => _style;
        set { _style = value; ApplyStyle(); Invalidate(); }
    }
    
    public int BorderRadius
    {
        get => _borderRadius;
        set { _borderRadius = value; Invalidate(); }
    }
    
    private void ApplyStyle()
    {
        switch (_style)
        {
            case ButtonStyle.Primary:
                BackColor = ModernTheme.Primary;
                ForeColor = Color.White;
                break;
                
            case ButtonStyle.Secondary:
                BackColor = ModernTheme.Surface;
                ForeColor = ModernTheme.TextPrimary;
                break;
                
            case ButtonStyle.Outline:
                BackColor = Color.Transparent;
                ForeColor = ModernTheme.Primary;
                break;
                
            case ButtonStyle.Ghost:
                BackColor = Color.Transparent;
                ForeColor = ModernTheme.TextSecondary;
                break;
                
            case ButtonStyle.Danger:
                BackColor = ModernTheme.Error;
                ForeColor = Color.White;
                break;
                
            case ButtonStyle.Success:
                BackColor = ModernTheme.Success;
                ForeColor = Color.White;
                break;
        }
    }
    
    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        
        // Get colors based on state
        Color bgColor = GetBackgroundColor();
        Color fgColor = GetForegroundColor();
        
        // Draw background
        using (var path = GetRoundedRectPath(ClientRectangle, _borderRadius))
        {
            using (var brush = new SolidBrush(bgColor))
            {
                e.Graphics.FillPath(brush, path);
            }
            
            // Draw border for outline style
            if (_style == ButtonStyle.Outline)
            {
                using (var pen = new Pen(_isHovered ? ModernTheme.PrimaryHover : ModernTheme.Primary, 2))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            }
        }
        
        // Draw text
        var textSize = e.Graphics.MeasureString(Text, Font);
        var textX = (Width - textSize.Width) / 2;
        var textY = (Height - textSize.Height) / 2;
        
        using (var brush = new SolidBrush(Enabled ? fgColor : ModernTheme.TextDisabled))
        {
            e.Graphics.DrawString(Text, Font, brush, textX, textY);
        }
    }
    
    private Color GetBackgroundColor()
    {
        if (!Enabled)
            return ModernTheme.SurfaceLight;
            
        if (_isPressed)
        {
            return _style switch
            {
                ButtonStyle.Primary => ModernTheme.PrimaryDark,
                ButtonStyle.Secondary => ModernTheme.SurfaceLight,
                ButtonStyle.Outline => Color.FromArgb(30, ModernTheme.Primary),
                ButtonStyle.Ghost => ModernTheme.SurfaceHover,
                ButtonStyle.Danger => Color.FromArgb(220, 38, 38),
                ButtonStyle.Success => Color.FromArgb(22, 163, 74),
                _ => BackColor
            };
        }
        
        if (_isHovered)
        {
            return _style switch
            {
                ButtonStyle.Primary => ModernTheme.PrimaryHover,
                ButtonStyle.Secondary => ModernTheme.SurfaceHover,
                ButtonStyle.Outline => Color.FromArgb(20, ModernTheme.Primary),
                ButtonStyle.Ghost => ModernTheme.SurfaceHover,
                ButtonStyle.Danger => Color.FromArgb(220, 38, 38),
                ButtonStyle.Success => Color.FromArgb(22, 163, 74),
                _ => BackColor
            };
        }
        
        return BackColor;
    }
    
    private Color GetForegroundColor()
    {
        if (_style == ButtonStyle.Outline)
            return _isHovered ? ModernTheme.PrimaryHover : ModernTheme.Primary;
            
        return ForeColor;
    }
    
    private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
    {
        rect = new Rectangle(rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
        var path = new GraphicsPath();
        
        if (radius <= 0)
        {
            path.AddRectangle(rect);
            return path;
        }
        
        path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
        path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
        path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
        path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
        path.CloseFigure();
        
        return path;
    }
}
