namespace VoidVideoGenerator.Components;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using VoidVideoGenerator.Models;

/// <summary>
/// Modern card panel with rounded corners and subtle border
/// </summary>
public class ModernCard : Panel
{
    private int _borderRadius = BorderRadius.LG;
    private bool _showBorder = true;
    private bool _showShadow = false;
    
    public ModernCard()
    {
        BackColor = ModernTheme.Surface;
        Padding = new Padding(Spacing.LG);
        
        // Enable double buffering for smooth rendering
        DoubleBuffered = true;
        SetStyle(ControlStyles.UserPaint | 
                 ControlStyles.AllPaintingInWmPaint | 
                 ControlStyles.OptimizedDoubleBuffer, true);
    }
    
    public int BorderRadius
    {
        get => _borderRadius;
        set { _borderRadius = value; Invalidate(); }
    }
    
    public bool ShowBorder
    {
        get => _showBorder;
        set { _showBorder = value; Invalidate(); }
    }
    
    public bool ShowShadow
    {
        get => _showShadow;
        set { _showShadow = value; Invalidate(); }
    }
    
    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        
        // Draw shadow if enabled
        if (_showShadow)
        {
            DrawShadow(e.Graphics);
        }
        
        // Draw rounded rectangle background
        using (var path = GetRoundedRectPath(ClientRectangle, _borderRadius))
        {
            using (var brush = new SolidBrush(BackColor))
            {
                e.Graphics.FillPath(brush, path);
            }
            
            // Draw border
            if (_showBorder)
            {
                using (var pen = new Pen(ModernTheme.Border, 1))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            }
        }
    }
    
    private void DrawShadow(Graphics g)
    {
        var shadowRect = new Rectangle(
            ClientRectangle.X + 2,
            ClientRectangle.Y + 2,
            ClientRectangle.Width - 4,
            ClientRectangle.Height - 4
        );
        
        using (var path = GetRoundedRectPath(shadowRect, _borderRadius))
        {
            using (var brush = new SolidBrush(ModernTheme.Shadow))
            {
                g.FillPath(brush, path);
            }
        }
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
