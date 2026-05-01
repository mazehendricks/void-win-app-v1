namespace VoidVideoGenerator.Components;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using VoidVideoGenerator.Models;

/// <summary>
/// Modern progress bar with smooth gradient and percentage display
/// </summary>
public class ModernProgressBar : Control
{
    private int _value = 0;
    private int _maximum = 100;
    private string _text = "";
    private bool _showPercentage = true;
    private int _height = 8;
    
    public ModernProgressBar()
    {
        Height = _height;
        BackColor = ModernTheme.Surface;
        DoubleBuffered = true;
        SetStyle(ControlStyles.UserPaint | 
                 ControlStyles.AllPaintingInWmPaint | 
                 ControlStyles.OptimizedDoubleBuffer, true);
    }
    
    public int Value
    {
        get => _value;
        set 
        { 
            _value = Math.Min(Math.Max(value, 0), _maximum);
            Invalidate();
        }
    }
    
    public int Maximum
    {
        get => _maximum;
        set 
        { 
            _maximum = Math.Max(value, 1);
            _value = Math.Min(_value, _maximum);
            Invalidate();
        }
    }
    
    public string ProgressText
    {
        get => _text;
        set { _text = value; Invalidate(); }
    }
    
    public bool ShowPercentage
    {
        get => _showPercentage;
        set { _showPercentage = value; Invalidate(); }
    }
    
    public int BarHeight
    {
        get => _height;
        set { _height = value; Height = value; Invalidate(); }
    }
    
    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        
        // Draw background
        using (var path = GetRoundedRectPath(ClientRectangle, BorderRadius.Full))
        {
            using (var brush = new SolidBrush(ModernTheme.Surface))
            {
                e.Graphics.FillPath(brush, path);
            }
        }
        
        // Draw progress
        if (_value > 0)
        {
            var progressWidth = (int)((float)_value / _maximum * Width);
            var progressRect = new Rectangle(0, 0, progressWidth, Height);
            
            using (var path = GetRoundedRectPath(progressRect, BorderRadius.Full))
            {
                // Gradient from primary to primary hover
                using (var brush = new LinearGradientBrush(
                    progressRect,
                    ModernTheme.Primary,
                    ModernTheme.PrimaryHover,
                    LinearGradientMode.Horizontal))
                {
                    e.Graphics.FillPath(brush, path);
                }
            }
        }
        
        // Draw text
        if (_showPercentage || !string.IsNullOrEmpty(_text))
        {
            var displayText = !string.IsNullOrEmpty(_text) 
                ? _text 
                : $"{(int)((float)_value / _maximum * 100)}%";
                
            using (var brush = new SolidBrush(ModernTheme.TextPrimary))
            {
                var textSize = e.Graphics.MeasureString(displayText, ModernFonts.Small);
                var textX = (Width - textSize.Width) / 2;
                var textY = (Height - textSize.Height) / 2;
                
                // Draw text shadow for better visibility
                using (var shadowBrush = new SolidBrush(Color.FromArgb(100, 0, 0, 0)))
                {
                    e.Graphics.DrawString(displayText, ModernFonts.Small, shadowBrush, textX + 1, textY + 1);
                }
                
                e.Graphics.DrawString(displayText, ModernFonts.Small, brush, textX, textY);
            }
        }
    }
    
    private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
    {
        var path = new GraphicsPath();
        
        if (radius <= 0 || rect.Width < radius || rect.Height < radius)
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
