namespace VoidVideoGenerator.Components;

using VoidVideoGenerator.Models;

/// <summary>
/// Advanced prompting interface for AI video generation
/// Provides both natural language and structured parameter input
/// </summary>
public class DirectorsConsole : Panel
{
    private TabControl _modeTabControl = null!;
    private TextBox _simplePromptBox = null!;
    private Panel _advancedPanel = null!;
    private ComboBox _shotTypeCombo = null!;
    private ComboBox _lightingCombo = null!;
    private ComboBox _cameraMotionCombo = null!;
    private NumericUpDown _durationNumeric = null!;
    private TextBox _jsonPreviewBox = null!;
    private ComboBox _templateCombo = null!;
    
    public string Prompt => _simplePromptBox.Text;
    public string ShotType => _shotTypeCombo.SelectedItem?.ToString() ?? "medium";
    public string Lighting => _lightingCombo.SelectedItem?.ToString() ?? "natural";
    public string CameraMotion => _cameraMotionCombo.SelectedItem?.ToString() ?? "static";
    public int Duration => (int)_durationNumeric.Value;
    
    public event EventHandler? PromptChanged;
    
    public DirectorsConsole()
    {
        InitializeComponent();
    }
    
    private void InitializeComponent()
    {
        this.BackColor = ModernTheme.Surface;
        this.Padding = new Padding(20);
        this.AutoSize = true;
        this.MinimumSize = new Size(800, 300);
        
        // Title
        var titleLabel = new Label
        {
            Text = "🎬 Director's Console",
            Location = new Point(20, 20),
            AutoSize = true,
            Font = ModernFonts.H3,
            ForeColor = ModernTheme.TextPrimary
        };
        this.Controls.Add(titleLabel);
        
        // Mode tabs
        _modeTabControl = new TabControl
        {
            Location = new Point(20, 60),
            Size = new Size(760, 220),
            BackColor = ModernTheme.Background,
            ForeColor = ModernTheme.TextPrimary
        };
        
        // Simple mode tab
        var simpleTab = new TabPage("Simple Mode")
        {
            BackColor = ModernTheme.Background,
            Padding = new Padding(15)
        };
        
        var simpleLabel = new Label
        {
            Text = "Describe your video in natural language:",
            Location = new Point(10, 10),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        simpleTab.Controls.Add(simpleLabel);
        
        _simplePromptBox = new TextBox
        {
            Location = new Point(10, 40),
            Size = new Size(710, 120),
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle,
            Font = ModernFonts.Body,
            Text = "Create a cinematic video showing..."
        };
        _simplePromptBox.TextChanged += (s, e) => PromptChanged?.Invoke(this, EventArgs.Empty);
        simpleTab.Controls.Add(_simplePromptBox);
        
        _modeTabControl.TabPages.Add(simpleTab);
        
        // Advanced mode tab
        var advancedTab = new TabPage("Advanced Mode")
        {
            BackColor = ModernTheme.Background,
            Padding = new Padding(15)
        };
        
        _advancedPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = ModernTheme.Background,
            AutoScroll = true
        };
        
        int yPos = 10;
        
        // Shot Type
        var shotLabel = new Label
        {
            Text = "Shot Type:",
            Location = new Point(10, yPos + 3),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        _advancedPanel.Controls.Add(shotLabel);
        
        _shotTypeCombo = new ComboBox
        {
            Location = new Point(150, yPos),
            Size = new Size(200, 25),
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary
        };
        _shotTypeCombo.Items.AddRange(new[] { "wide", "medium", "close-up", "extreme-close-up", "pov", "over-shoulder" });
        _shotTypeCombo.SelectedIndex = 1;
        _advancedPanel.Controls.Add(_shotTypeCombo);
        yPos += 35;
        
        // Lighting
        var lightingLabel = new Label
        {
            Text = "Lighting:",
            Location = new Point(10, yPos + 3),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        _advancedPanel.Controls.Add(lightingLabel);
        
        _lightingCombo = new ComboBox
        {
            Location = new Point(150, yPos),
            Size = new Size(200, 25),
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary
        };
        _lightingCombo.Items.AddRange(new[] { "natural", "dramatic", "soft", "hard", "golden-hour", "blue-hour", "neon" });
        _lightingCombo.SelectedIndex = 0;
        _advancedPanel.Controls.Add(_lightingCombo);
        yPos += 35;
        
        // Camera Motion
        var motionLabel = new Label
        {
            Text = "Camera Motion:",
            Location = new Point(10, yPos + 3),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        _advancedPanel.Controls.Add(motionLabel);
        
        _cameraMotionCombo = new ComboBox
        {
            Location = new Point(150, yPos),
            Size = new Size(200, 25),
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary
        };
        _cameraMotionCombo.Items.AddRange(new[] { "static", "pan-left", "pan-right", "zoom-in", "zoom-out", "dolly", "crane", "handheld" });
        _cameraMotionCombo.SelectedIndex = 0;
        _advancedPanel.Controls.Add(_cameraMotionCombo);
        yPos += 35;
        
        // Duration
        var durationLabel = new Label
        {
            Text = "Duration (sec):",
            Location = new Point(10, yPos + 3),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        _advancedPanel.Controls.Add(durationLabel);
        
        _durationNumeric = new NumericUpDown
        {
            Location = new Point(150, yPos),
            Size = new Size(100, 25),
            Minimum = 1,
            Maximum = 30,
            Value = 5,
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle
        };
        _advancedPanel.Controls.Add(_durationNumeric);
        
        advancedTab.Controls.Add(_advancedPanel);
        _modeTabControl.TabPages.Add(advancedTab);
        
        // JSON mode tab
        var jsonTab = new TabPage("JSON Preview")
        {
            BackColor = ModernTheme.Background,
            Padding = new Padding(15)
        };
        
        _jsonPreviewBox = new TextBox
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            BackColor = Color.FromArgb(30, 30, 30),
            ForeColor = Color.FromArgb(220, 220, 220),
            BorderStyle = BorderStyle.None,
            Font = ModernFonts.Code,
            ReadOnly = true
        };
        jsonTab.Controls.Add(_jsonPreviewBox);
        
        _modeTabControl.TabPages.Add(jsonTab);
        _modeTabControl.SelectedIndexChanged += (s, e) => UpdateJsonPreview();
        
        this.Controls.Add(_modeTabControl);
        
        // Template selector
        var templateLabel = new Label
        {
            Text = "Templates:",
            Location = new Point(20, 290),
            AutoSize = true,
            ForeColor = ModernTheme.TextSecondary,
            Font = ModernFonts.Small
        };
        this.Controls.Add(templateLabel);
        
        _templateCombo = new ComboBox
        {
            Location = new Point(100, 287),
            Size = new Size(200, 25),
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary
        };
        _templateCombo.Items.AddRange(new[] { 
            "None", 
            "Product Showcase", 
            "Tutorial Intro", 
            "Dramatic Reveal",
            "Nature Documentary",
            "Tech Review"
        });
        _templateCombo.SelectedIndex = 0;
        _templateCombo.SelectedIndexChanged += ApplyTemplate;
        this.Controls.Add(_templateCombo);
    }
    
    private void UpdateJsonPreview()
    {
        var json = $@"{{
  ""prompt"": ""{_simplePromptBox.Text}"",
  ""parameters"": {{
    ""shot_type"": ""{ShotType}"",
    ""lighting"": ""{Lighting}"",
    ""camera_motion"": ""{CameraMotion}"",
    ""duration"": {Duration}
  }}
}}";
        _jsonPreviewBox.Text = json;
    }
    
    private void ApplyTemplate(object? sender, EventArgs e)
    {
        var template = _templateCombo.SelectedItem?.ToString();
        
        switch (template)
        {
            case "Product Showcase":
                _simplePromptBox.Text = "Elegant product showcase with smooth camera rotation, professional lighting highlighting key features";
                _shotTypeCombo.SelectedItem = "close-up";
                _lightingCombo.SelectedItem = "soft";
                _cameraMotionCombo.SelectedItem = "dolly";
                break;
                
            case "Tutorial Intro":
                _simplePromptBox.Text = "Engaging tutorial introduction with dynamic text overlays and upbeat energy";
                _shotTypeCombo.SelectedItem = "medium";
                _lightingCombo.SelectedItem = "natural";
                _cameraMotionCombo.SelectedItem = "static";
                break;
                
            case "Dramatic Reveal":
                _simplePromptBox.Text = "Dramatic reveal with slow zoom, building tension and anticipation";
                _shotTypeCombo.SelectedItem = "wide";
                _lightingCombo.SelectedItem = "dramatic";
                _cameraMotionCombo.SelectedItem = "zoom-in";
                break;
                
            case "Nature Documentary":
                _simplePromptBox.Text = "Serene nature scene with golden hour lighting, peaceful and contemplative";
                _shotTypeCombo.SelectedItem = "wide";
                _lightingCombo.SelectedItem = "golden-hour";
                _cameraMotionCombo.SelectedItem = "pan-right";
                break;
                
            case "Tech Review":
                _simplePromptBox.Text = "Modern tech review setup with clean lighting and professional presentation";
                _shotTypeCombo.SelectedItem = "medium";
                _lightingCombo.SelectedItem = "soft";
                _cameraMotionCombo.SelectedItem = "static";
                break;
        }
        
        UpdateJsonPreview();
    }
    
    public VideoPrompt GetVideoPrompt()
    {
        return new VideoPrompt
        {
            Text = Prompt,
            ShotType = ShotType,
            Lighting = Lighting,
            CameraMotion = CameraMotion,
            Duration = Duration
        };
    }
}
