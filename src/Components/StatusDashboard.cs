namespace VoidVideoGenerator.Components;

using System.Diagnostics;
using VoidVideoGenerator.Models;

/// <summary>
/// Real-time status dashboard with visual indicators
/// Shows system health, resource usage, and activity log
/// </summary>
public class StatusDashboard : Panel
{
    private Panel _servicesPanel = null!;
    private Panel _resourcesPanel = null!;
    private Panel _activityPanel = null!;
    private Dictionary<string, ServiceStatusIndicator> _serviceIndicators = new();
    private ProgressBar _cpuBar = null!;
    private ProgressBar _ramBar = null!;
    private ProgressBar _gpuBar = null!;
    private ProgressBar _diskBar = null!;
    private Label _cpuLabel = null!;
    private Label _ramLabel = null!;
    private Label _gpuLabel = null!;
    private Label _diskLabel = null!;
    private ListBox _activityLog = null!;
    private System.Windows.Forms.Timer _updateTimer = null!;
    
    public StatusDashboard()
    {
        InitializeComponent();
        StartMonitoring();
    }
    
    private void InitializeComponent()
    {
        this.BackColor = ModernTheme.Background;
        this.Padding = new Padding(20);
        this.AutoScroll = true;
        
        int yPos = 0;
        
        // Services section
        var servicesTitle = new Label
        {
            Text = "Core Services",
            Location = new Point(0, yPos),
            AutoSize = true,
            Font = ModernFonts.H3,
            ForeColor = ModernTheme.TextPrimary
        };
        this.Controls.Add(servicesTitle);
        yPos += 40;
        
        _servicesPanel = new Panel
        {
            Location = new Point(0, yPos),
            Size = new Size(800, 200),
            BackColor = ModernTheme.Surface,
            Padding = new Padding(20)
        };
        this.Controls.Add(_servicesPanel);
        yPos += 220;
        
        // Add service indicators
        AddServiceIndicator("Ollama", 0);
        AddServiceIndicator("Piper TTS", 1);
        AddServiceIndicator("FFmpeg", 2);
        AddServiceIndicator("GPU", 3);
        
        // Resources section
        var resourcesTitle = new Label
        {
            Text = "System Resources",
            Location = new Point(0, yPos),
            AutoSize = true,
            Font = ModernFonts.H3,
            ForeColor = ModernTheme.TextPrimary
        };
        this.Controls.Add(resourcesTitle);
        yPos += 40;
        
        _resourcesPanel = new Panel
        {
            Location = new Point(0, yPos),
            Size = new Size(800, 200),
            BackColor = ModernTheme.Surface,
            Padding = new Padding(20)
        };
        this.Controls.Add(_resourcesPanel);
        
        CreateResourceMonitors();
        yPos += 220;
        
        // Activity section
        var activityTitle = new Label
        {
            Text = "Recent Activity",
            Location = new Point(0, yPos),
            AutoSize = true,
            Font = ModernFonts.H3,
            ForeColor = ModernTheme.TextPrimary
        };
        this.Controls.Add(activityTitle);
        yPos += 40;
        
        _activityPanel = new Panel
        {
            Location = new Point(0, yPos),
            Size = new Size(800, 300),
            BackColor = ModernTheme.Surface,
            Padding = new Padding(20)
        };
        this.Controls.Add(_activityPanel);
        
        _activityLog = new ListBox
        {
            Dock = DockStyle.Fill,
            BackColor = ModernTheme.Background,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.None,
            Font = ModernFonts.Code
        };
        _activityPanel.Controls.Add(_activityLog);
    }
    
    private void AddServiceIndicator(string serviceName, int index)
    {
        var indicator = new ServiceStatusIndicator(serviceName)
        {
            Location = new Point(20, 20 + (index * 40)),
            Size = new Size(760, 35)
        };
        _servicesPanel.Controls.Add(indicator);
        _serviceIndicators[serviceName] = indicator;
    }
    
    private void CreateResourceMonitors()
    {
        int yPos = 20;
        
        // CPU
        _cpuLabel = new Label
        {
            Text = "CPU: 0%",
            Location = new Point(20, yPos),
            Size = new Size(200, 20),
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        _resourcesPanel.Controls.Add(_cpuLabel);
        
        _cpuBar = new ProgressBar
        {
            Location = new Point(230, yPos),
            Size = new Size(500, 20),
            Style = ProgressBarStyle.Continuous,
            ForeColor = ModernTheme.Primary
        };
        _resourcesPanel.Controls.Add(_cpuBar);
        yPos += 35;
        
        // RAM
        _ramLabel = new Label
        {
            Text = "RAM: 0% (0GB / 0GB)",
            Location = new Point(20, yPos),
            Size = new Size(200, 20),
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        _resourcesPanel.Controls.Add(_ramLabel);
        
        _ramBar = new ProgressBar
        {
            Location = new Point(230, yPos),
            Size = new Size(500, 20),
            Style = ProgressBarStyle.Continuous
        };
        _resourcesPanel.Controls.Add(_ramBar);
        yPos += 35;
        
        // GPU
        _gpuLabel = new Label
        {
            Text = "GPU: 0%",
            Location = new Point(20, yPos),
            Size = new Size(200, 20),
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        _resourcesPanel.Controls.Add(_gpuLabel);
        
        _gpuBar = new ProgressBar
        {
            Location = new Point(230, yPos),
            Size = new Size(500, 20),
            Style = ProgressBarStyle.Continuous
        };
        _resourcesPanel.Controls.Add(_gpuBar);
        yPos += 35;
        
        // Disk
        _diskLabel = new Label
        {
            Text = "Disk: 0% (0GB / 0GB)",
            Location = new Point(20, yPos),
            Size = new Size(200, 20),
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        _resourcesPanel.Controls.Add(_diskLabel);
        
        _diskBar = new ProgressBar
        {
            Location = new Point(230, yPos),
            Size = new Size(500, 20),
            Style = ProgressBarStyle.Continuous
        };
        _resourcesPanel.Controls.Add(_diskBar);
    }
    
    private void StartMonitoring()
    {
        _updateTimer = new System.Windows.Forms.Timer { Interval = 2000 };
        _updateTimer.Tick += UpdateMetrics;
        _updateTimer.Start();
        
        // Initial update
        UpdateMetrics(null, EventArgs.Empty);
    }
    
    private void UpdateMetrics(object? sender, EventArgs e)
    {
        try
        {
            // Update CPU
            var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpuCounter.NextValue(); // First call always returns 0
            System.Threading.Thread.Sleep(100);
            var cpuUsage = (int)cpuCounter.NextValue();
            _cpuBar.Value = Math.Min(cpuUsage, 100);
            _cpuLabel.Text = $"CPU: {cpuUsage}%";
            
            // Update RAM
            var ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            var availableRam = ramCounter.NextValue();
            var totalRam = GetTotalPhysicalMemory();
            var usedRam = totalRam - availableRam;
            var ramPercent = (int)((usedRam / totalRam) * 100);
            _ramBar.Value = Math.Min(ramPercent, 100);
            _ramLabel.Text = $"RAM: {ramPercent}% ({usedRam / 1024:F1}GB / {totalRam / 1024:F1}GB)";
            
            // Update Disk
            var drive = new DriveInfo(Path.GetPathRoot(Environment.CurrentDirectory) ?? "C:\\");
            var diskPercent = (int)(((drive.TotalSize - drive.AvailableFreeSpace) / (double)drive.TotalSize) * 100);
            _diskBar.Value = Math.Min(diskPercent, 100);
            _diskLabel.Text = $"Disk: {diskPercent}% ({(drive.TotalSize - drive.AvailableFreeSpace) / 1024.0 / 1024 / 1024:F0}GB / {drive.TotalSize / 1024.0 / 1024 / 1024:F0}GB)";
        }
        catch
        {
            // Silently fail if performance counters are not available
        }
    }
    
    private float GetTotalPhysicalMemory()
    {
        try
        {
            var computerInfo = new Microsoft.VisualBasic.Devices.ComputerInfo();
            return computerInfo.TotalPhysicalMemory / 1024f / 1024f; // Convert to MB
        }
        catch
        {
            return 16384; // Default to 16GB if unable to detect
        }
    }
    
    public void UpdateServiceStatus(string serviceName, ServiceStatus status, string details = "")
    {
        if (_serviceIndicators.TryGetValue(serviceName, out var indicator))
        {
            indicator.UpdateStatus(status, details);
        }
    }
    
    public void LogActivity(string message, ActivityType type = ActivityType.Info)
    {
        if (InvokeRequired)
        {
            Invoke(() => LogActivity(message, type));
            return;
        }
        
        var icon = type switch
        {
            ActivityType.Success => "✓",
            ActivityType.Warning => "⚠",
            ActivityType.Error => "✗",
            ActivityType.Info => "ℹ",
            _ => "•"
        };
        
        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        var logEntry = $"[{timestamp}] {icon} {message}";
        
        _activityLog.Items.Insert(0, logEntry);
        
        // Keep only last 100 entries
        while (_activityLog.Items.Count > 100)
        {
            _activityLog.Items.RemoveAt(_activityLog.Items.Count - 1);
        }
    }
    
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _updateTimer?.Stop();
            _updateTimer?.Dispose();
        }
        base.Dispose(disposing);
    }
}

/// <summary>
/// Individual service status indicator component
/// </summary>
public class ServiceStatusIndicator : Panel
{
    private Panel _statusDot;
    private Label _nameLabel;
    private Label _statusLabel;
    private Label _detailsLabel;
    private ServiceStatus _currentStatus = ServiceStatus.Unknown;
    
    public ServiceStatusIndicator(string serviceName)
    {
        this.BackColor = Color.Transparent;
        this.Height = 35;
        
        // Status dot
        _statusDot = new Panel
        {
            Location = new Point(0, 10),
            Size = new Size(12, 12),
            BackColor = ModernTheme.TextDisabled
        };
        
        // Make circular
        var path = new System.Drawing.Drawing2D.GraphicsPath();
        path.AddEllipse(0, 0, 12, 12);
        _statusDot.Region = new Region(path);
        
        this.Controls.Add(_statusDot);
        
        // Service name
        _nameLabel = new Label
        {
            Text = serviceName,
            Location = new Point(25, 8),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        this.Controls.Add(_nameLabel);
        
        // Status text
        _statusLabel = new Label
        {
            Text = "[Unknown]",
            Location = new Point(200, 8),
            AutoSize = true,
            ForeColor = ModernTheme.TextSecondary,
            Font = ModernFonts.Small
        };
        this.Controls.Add(_statusLabel);
        
        // Details
        _detailsLabel = new Label
        {
            Text = "",
            Location = new Point(350, 8),
            AutoSize = true,
            ForeColor = ModernTheme.TextSecondary,
            Font = ModernFonts.Small
        };
        this.Controls.Add(_detailsLabel);
    }
    
    public void UpdateStatus(ServiceStatus status, string details = "")
    {
        _currentStatus = status;
        
        _statusDot.BackColor = status switch
        {
            ServiceStatus.Healthy => ModernTheme.Success,
            ServiceStatus.Warning => ModernTheme.Warning,
            ServiceStatus.Error => ModernTheme.Danger,
            _ => ModernTheme.TextDisabled
        };
        
        _statusLabel.Text = status switch
        {
            ServiceStatus.Healthy => "✓ Healthy",
            ServiceStatus.Warning => "⚠ Warning",
            ServiceStatus.Error => "✗ Error",
            _ => "○ Unknown"
        };
        
        _detailsLabel.Text = details;
    }
}

public enum ActivityType
{
    Info,
    Success,
    Warning,
    Error
}
