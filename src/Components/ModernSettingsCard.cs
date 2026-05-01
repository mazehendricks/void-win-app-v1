namespace VoidVideoGenerator.Components;

using System.Drawing.Drawing2D;
using VoidVideoGenerator.Models;

/// <summary>
/// Modern card component for settings sections
/// Provides expandable/collapsible grouped settings with visual hierarchy
/// </summary>
public class ModernSettingsCard : Panel
{
    private const int HeaderHeight = 60;
    private const int CollapsedHeight = 60;
    private const int Padding = 20;
    private const int BorderRadius = 8;
    
    private string _title = "";
    private string _icon = "";
    private string _description = "";
    private bool _isExpanded = true;
    private bool _isHovered = false;
    private Panel _headerPanel;
    private Panel _contentPanel;
    private Label _titleLabel;
    private Label _descriptionLabel;
    private Label _iconLabel;
    private Button _expandButton;
    
    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            if (_titleLabel != null)
                _titleLabel.Text = value;
        }
    }
    
    public string Icon
    {
        get => _icon;
        set
        {
            _icon = value;
            if (_iconLabel != null)
                _iconLabel.Text = value;
        }
    }
    
    public string Description
    {
        get => _description;
        set
        {
            _description = value;
            if (_descriptionLabel != null)
                _descriptionLabel.Text = value;
        }
    }
    
    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            _isExpanded = value;
            AnimateExpansion();
        }
    }
    
    public Panel ContentArea => _contentPanel;
    
    public ModernSettingsCard()
    {
        InitializeComponent();
    }
    
    public ModernSettingsCard(string title, string icon, string description) : this()
    {
        Title = title;
        Icon = icon;
        Description = description;
    }
    
    private void InitializeComponent()
    {
        // Card container
        this.BackColor = ModernTheme.Surface;
        this.Padding = new Padding(0);
        this.Margin = new Padding(0, 0, 0, 16);
        this.MinimumSize = new Size(400, CollapsedHeight);
        
        // Enable double buffering
        this.DoubleBuffered = true;
        this.SetStyle(ControlStyles.OptimizedDoubleBuffer | 
                     ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.UserPaint, true);
        
        // Header panel
        _headerPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = HeaderHeight,
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand
        };
        _headerPanel.MouseEnter += (s, e) => { _isHovered = true; this.Invalidate(); };
        _headerPanel.MouseLeave += (s, e) => { _isHovered = false; this.Invalidate(); };
        _headerPanel.Click += (s, e) => IsExpanded = !IsExpanded;
        
        // Icon
        _iconLabel = new Label
        {
            Text = _icon,
            Location = new Point(Padding, 15),
            AutoSize = true,
            Font = new Font("Segoe UI Emoji", 20f),
            ForeColor = ModernTheme.Primary,
            BackColor = Color.Transparent
        };
        _iconLabel.Click += (s, e) => IsExpanded = !IsExpanded;
        _headerPanel.Controls.Add(_iconLabel);
        
        // Title
        _titleLabel = new Label
        {
            Text = _title,
            Location = new Point(60, 12),
            AutoSize = true,
            Font = ModernFonts.H4,
            ForeColor = ModernTheme.TextPrimary,
            BackColor = Color.Transparent
        };
        _titleLabel.Click += (s, e) => IsExpanded = !IsExpanded;
        _headerPanel.Controls.Add(_titleLabel);
        
        // Description
        _descriptionLabel = new Label
        {
            Text = _description,
            Location = new Point(60, 35),
            AutoSize = true,
            MaximumSize = new Size(600, 0),
            Font = ModernFonts.Small,
            ForeColor = ModernTheme.TextSecondary,
            BackColor = Color.Transparent
        };
        _descriptionLabel.Click += (s, e) => IsExpanded = !IsExpanded;
        _headerPanel.Controls.Add(_descriptionLabel);
        
        // Expand/collapse button
        _expandButton = new Button
        {
            Text = "▼",
            Size = new Size(30, 30),
            Location = new Point(this.Width - 40, 15),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.Transparent,
            ForeColor = ModernTheme.TextSecondary,
            Font = new Font("Segoe UI", 10f),
            Cursor = Cursors.Hand
        };
        _expandButton.FlatAppearance.BorderSize = 0;
        _expandButton.Click += (s, e) => IsExpanded = !IsExpanded;
        _headerPanel.Controls.Add(_expandButton);
        
        this.Controls.Add(_headerPanel);
        
        // Content panel
        _contentPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            Padding = new Padding(Padding),
            AutoScroll = true
        };
        this.Controls.Add(_contentPanel);
        
        // Set initial height
        this.Height = _isExpanded ? 300 : CollapsedHeight;
    }
    
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        
        // Draw card background with rounded corners
        using var path = GetRoundedRectangle(this.ClientRectangle, BorderRadius);
        using var brush = new SolidBrush(ModernTheme.Surface);
        g.FillPath(brush, path);
        
        // Draw hover effect
        if (_isHovered && _headerPanel.ClientRectangle.Contains(_headerPanel.PointToClient(Cursor.Position)))
        {
            using var hoverBrush = new SolidBrush(Color.FromArgb(10, 255, 255, 255));
            using var headerPath = GetRoundedRectangle(
                new Rectangle(0, 0, this.Width, HeaderHeight), 
                BorderRadius, 
                true, true, false, false);
            g.FillPath(hoverBrush, headerPath);
        }
        
        // Draw shadow
        DrawShadow(g);
        
        // Draw border
        using var borderPen = new Pen(ModernTheme.Border, 1);
        g.DrawPath(borderPen, path);
    }
    
    private void DrawShadow(Graphics g)
    {
        // Simple shadow effect
        var shadowRect = new Rectangle(2, 2, this.Width - 4, this.Height - 4);
        using var shadowPath = GetRoundedRectangle(shadowRect, BorderRadius);
        using var shadowBrush = new SolidBrush(Color.FromArgb(20, 0, 0, 0));
        g.FillPath(shadowBrush, shadowPath);
    }
    
    private GraphicsPath GetRoundedRectangle(Rectangle bounds, int radius, 
        bool topLeft = true, bool topRight = true, bool bottomLeft = true, bool bottomRight = true)
    {
        var path = new GraphicsPath();
        var diameter = radius * 2;
        
        // Top left
        if (topLeft)
            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
        else
            path.AddLine(bounds.X, bounds.Y, bounds.X, bounds.Y);
        
        // Top right
        if (topRight)
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
        else
            path.AddLine(bounds.Right, bounds.Y, bounds.Right, bounds.Y);
        
        // Bottom right
        if (bottomRight)
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
        else
            path.AddLine(bounds.Right, bounds.Bottom, bounds.Right, bounds.Bottom);
        
        // Bottom left
        if (bottomLeft)
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
        else
            path.AddLine(bounds.X, bounds.Bottom, bounds.X, bounds.Bottom);
        
        path.CloseFigure();
        return path;
    }
    
    private void AnimateExpansion()
    {
        // Update expand button
        _expandButton.Text = _isExpanded ? "▼" : "▶";
        
        // Show/hide content
        _contentPanel.Visible = _isExpanded;
        
        // Animate height
        var targetHeight = _isExpanded ? CalculateExpandedHeight() : CollapsedHeight;
        var timer = new System.Windows.Forms.Timer { Interval = 10 };
        
        timer.Tick += (s, e) =>
        {
            var step = 20;
            if (Math.Abs(this.Height - targetHeight) <= step)
            {
                this.Height = targetHeight;
                timer.Stop();
                timer.Dispose();
            }
            else
            {
                this.Height += this.Height < targetHeight ? step : -step;
            }
        };
        
        timer.Start();
    }
    
    private int CalculateExpandedHeight()
    {
        // Calculate based on content
        var contentHeight = 0;
        foreach (Control control in _contentPanel.Controls)
        {
            var bottom = control.Bottom + control.Margin.Bottom;
            if (bottom > contentHeight)
                contentHeight = bottom;
        }
        
        return HeaderHeight + contentHeight + Padding * 2;
    }
    
    public void AddControl(Control control)
    {
        _contentPanel.Controls.Add(control);
        if (_isExpanded)
        {
            this.Height = CalculateExpandedHeight();
        }
    }
    
    public void AddLabeledControl(string label, Control control, int yPos)
    {
        var lbl = new Label
        {
            Text = label,
            Location = new Point(0, yPos + 3),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        _contentPanel.Controls.Add(lbl);
        
        control.Location = new Point(150, yPos);
        _contentPanel.Controls.Add(control);
    }
}
