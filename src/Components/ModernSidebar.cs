namespace VoidVideoGenerator.Components;

using System.Drawing.Drawing2D;
using VoidVideoGenerator.Models;

/// <summary>
/// Modern sidebar navigation component with collapsible design
/// Provides primary navigation for the application with visual status indicators
/// </summary>
public class ModernSidebar : Panel
{
    private const int ExpandedWidth = 240;
    private const int CollapsedWidth = 60;
    private const int ItemHeight = 50;
    private const int StatusIndicatorSize = 12;
    
    private bool _isCollapsed = false;
    private List<SidebarItem> _items = new();
    private SidebarItem? _activeItem;
    private Panel _statusPanel;
    private Dictionary<string, ServiceStatus> _serviceStatuses = new();
    
    public event EventHandler<string>? NavigationChanged;
    
    public bool IsCollapsed
    {
        get => _isCollapsed;
        set
        {
            _isCollapsed = value;
            AnimateCollapse();
        }
    }
    
    public ModernSidebar()
    {
        InitializeComponent();
        SetupDefaultItems();
    }
    
    private void InitializeComponent()
    {
        this.Width = ExpandedWidth;
        this.Dock = DockStyle.Left;
        this.BackColor = ModernTheme.SurfaceVariant;
        this.Padding = new Padding(0);
        
        // Enable double buffering for smooth rendering
        this.DoubleBuffered = true;
        this.SetStyle(ControlStyles.OptimizedDoubleBuffer | 
                     ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.UserPaint, true);
        
        // Create status panel at bottom
        _statusPanel = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 200,
            BackColor = ModernTheme.Surface,
            Padding = new Padding(10)
        };
        this.Controls.Add(_statusPanel);
        
        // Add collapse button at top
        var collapseBtn = new Button
        {
            Text = "☰",
            Size = new Size(40, 40),
            Location = new Point(10, 10),
            FlatStyle = FlatStyle.Flat,
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            Font = new Font("Segoe UI", 16f),
            Cursor = Cursors.Hand
        };
        collapseBtn.FlatAppearance.BorderSize = 0;
        collapseBtn.Click += (s, e) => IsCollapsed = !IsCollapsed;
        this.Controls.Add(collapseBtn);
    }
    
    private void SetupDefaultItems()
    {
        AddNavigationItem("generate", "🎬", "Generate", "Create new videos");
        AddNavigationItem("library", "📚", "Library", "Browse your videos");
        AddNavigationItem("settings", "⚙️", "Settings", "Configure application");
        AddNavigationItem("status", "📊", "Status", "System diagnostics");
        AddNavigationItem("debug", "🐛", "Debug", "Debug console");
        
        // Set first item as active
        if (_items.Count > 0)
        {
            SetActiveItem(_items[0]);
        }
        
        // Setup default service statuses
        UpdateServiceStatus("Ollama", ServiceStatus.Unknown);
        UpdateServiceStatus("Piper", ServiceStatus.Unknown);
        UpdateServiceStatus("FFmpeg", ServiceStatus.Unknown);
    }
    
    public void AddNavigationItem(string id, string icon, string text, string description)
    {
        var item = new SidebarItem
        {
            Id = id,
            Icon = icon,
            Text = text,
            Description = description,
            Bounds = new Rectangle(0, 60 + (_items.Count * ItemHeight), ExpandedWidth, ItemHeight)
        };
        
        _items.Add(item);
        this.Invalidate();
    }
    
    public void SetActiveItem(string id)
    {
        var item = _items.FirstOrDefault(i => i.Id == id);
        if (item != null)
        {
            SetActiveItem(item);
        }
    }
    
    private void SetActiveItem(SidebarItem item)
    {
        if (_activeItem != null)
        {
            _activeItem.IsActive = false;
        }
        
        _activeItem = item;
        _activeItem.IsActive = true;
        this.Invalidate();
        
        NavigationChanged?.Invoke(this, item.Id);
    }
    
    public void UpdateServiceStatus(string serviceName, ServiceStatus status)
    {
        _serviceStatuses[serviceName] = status;
        UpdateStatusPanel();
    }
    
    private void UpdateStatusPanel()
    {
        _statusPanel.Controls.Clear();
        
        var titleLabel = new Label
        {
            Text = _isCollapsed ? "●" : "CORE SERVICES",
            Location = new Point(10, 10),
            AutoSize = true,
            ForeColor = ModernTheme.TextSecondary,
            Font = new Font("Segoe UI", 10f, FontStyle.Bold)
        };
        _statusPanel.Controls.Add(titleLabel);
        
        int yPos = 40;
        foreach (var kvp in _serviceStatuses)
        {
            var statusControl = CreateStatusIndicator(kvp.Key, kvp.Value, yPos);
            _statusPanel.Controls.Add(statusControl);
            yPos += 35;
        }
    }
    
    private Panel CreateStatusIndicator(string serviceName, ServiceStatus status, int yPos)
    {
        var panel = new Panel
        {
            Location = new Point(10, yPos),
            Size = new Size(ExpandedWidth - 20, 30),
            BackColor = Color.Transparent
        };
        
        // Status dot
        var dot = new Panel
        {
            Location = new Point(0, 8),
            Size = new Size(StatusIndicatorSize, StatusIndicatorSize),
            BackColor = GetStatusColor(status)
        };
        
        // Make dot circular
        var path = new GraphicsPath();
        path.AddEllipse(0, 0, StatusIndicatorSize, StatusIndicatorSize);
        dot.Region = new Region(path);
        
        panel.Controls.Add(dot);
        
        if (!_isCollapsed)
        {
            // Service name
            var nameLabel = new Label
            {
                Text = serviceName,
                Location = new Point(20, 5),
                AutoSize = true,
                ForeColor = ModernTheme.TextPrimary,
                Font = new Font("Segoe UI", 10f)
            };
            panel.Controls.Add(nameLabel);
            
            // Status text
            var statusLabel = new Label
            {
                Text = GetStatusText(status),
                Location = new Point(120, 5),
                AutoSize = true,
                ForeColor = ModernTheme.TextSecondary,
                Font = new Font("Segoe UI", 9f)
            };
            panel.Controls.Add(statusLabel);
        }
        
        return panel;
    }
    
    private Color GetStatusColor(ServiceStatus status)
    {
        return status switch
        {
            ServiceStatus.Healthy => ModernTheme.Success,
            ServiceStatus.Warning => ModernTheme.Warning,
            ServiceStatus.Error => ModernTheme.Danger,
            _ => ModernTheme.TextDisabled
        };
    }
    
    private string GetStatusText(ServiceStatus status)
    {
        return status switch
        {
            ServiceStatus.Healthy => "✓ Ready",
            ServiceStatus.Warning => "⚠ Warning",
            ServiceStatus.Error => "✗ Error",
            _ => "○ Unknown"
        };
    }
    
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        
        // Draw navigation items
        foreach (var item in _items)
        {
            DrawNavigationItem(g, item);
        }
    }
    
    private void DrawNavigationItem(Graphics g, SidebarItem item)
    {
        var bounds = item.Bounds;
        
        // Draw active indicator bar
        if (item.IsActive)
        {
            using var brush = new SolidBrush(ModernTheme.Primary);
            g.FillRectangle(brush, 0, bounds.Y, 4, bounds.Height);
        }
        
        // Draw hover background
        if (item.IsHovered)
        {
            using var brush = new SolidBrush(Color.FromArgb(20, 255, 255, 255));
            g.FillRectangle(brush, bounds);
        }
        
        // Draw icon
        var iconFont = new Font("Segoe UI Emoji", 18f);
        var iconBrush = new SolidBrush(item.IsActive ? ModernTheme.Primary : ModernTheme.TextPrimary);
        var iconX = _isCollapsed ? (CollapsedWidth - 30) / 2 : 20;
        g.DrawString(item.Icon, iconFont, iconBrush, iconX, bounds.Y + 12);
        
        // Draw text (only when expanded)
        if (!_isCollapsed)
        {
            var textFont = new Font("Segoe UI", 12f, item.IsActive ? FontStyle.Bold : FontStyle.Regular);
            var textBrush = new SolidBrush(item.IsActive ? ModernTheme.Primary : ModernTheme.TextPrimary);
            g.DrawString(item.Text, textFont, textBrush, 60, bounds.Y + 15);
        }
    }
    
    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        
        bool needsRedraw = false;
        foreach (var item in _items)
        {
            bool wasHovered = item.IsHovered;
            item.IsHovered = item.Bounds.Contains(e.Location);
            
            if (wasHovered != item.IsHovered)
            {
                needsRedraw = true;
            }
        }
        
        if (needsRedraw)
        {
            this.Invalidate();
        }
        
        // Update cursor
        var hoveredItem = _items.FirstOrDefault(i => i.IsHovered);
        this.Cursor = hoveredItem != null ? Cursors.Hand : Cursors.Default;
    }
    
    protected override void OnMouseClick(MouseEventArgs e)
    {
        base.OnMouseClick(e);
        
        var clickedItem = _items.FirstOrDefault(i => i.Bounds.Contains(e.Location));
        if (clickedItem != null && clickedItem != _activeItem)
        {
            SetActiveItem(clickedItem);
        }
    }
    
    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        
        foreach (var item in _items)
        {
            item.IsHovered = false;
        }
        this.Invalidate();
    }
    
    private void AnimateCollapse()
    {
        // Simple animation using timer
        var targetWidth = _isCollapsed ? CollapsedWidth : ExpandedWidth;
        var timer = new System.Windows.Forms.Timer { Interval = 10 };
        
        timer.Tick += (s, e) =>
        {
            var step = 20;
            if (Math.Abs(this.Width - targetWidth) <= step)
            {
                this.Width = targetWidth;
                timer.Stop();
                timer.Dispose();
                UpdateStatusPanel();
            }
            else
            {
                this.Width += this.Width < targetWidth ? step : -step;
            }
            this.Invalidate();
        };
        
        timer.Start();
    }
}

/// <summary>
/// Represents a navigation item in the sidebar
/// </summary>
public class SidebarItem
{
    public string Id { get; set; } = "";
    public string Icon { get; set; } = "";
    public string Text { get; set; } = "";
    public string Description { get; set; } = "";
    public Rectangle Bounds { get; set; }
    public bool IsActive { get; set; }
    public bool IsHovered { get; set; }
}

/// <summary>
/// Service status enumeration
/// </summary>
public enum ServiceStatus
{
    Unknown,
    Healthy,
    Warning,
    Error
}
