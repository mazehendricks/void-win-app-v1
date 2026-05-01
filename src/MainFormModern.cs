namespace VoidVideoGenerator;

using System.Text.Json;
using VoidVideoGenerator.Models;
using VoidVideoGenerator.Services;
using VoidVideoGenerator.Components;

/// <summary>
/// Modern UI version of MainForm with sidebar navigation and card-based layouts
/// </summary>
public partial class MainFormModern : Form
{
    private AppConfig _config;
    private VideoGenerationPipeline? _pipeline;
    private readonly string _configPath = "config.json";
    private System.Diagnostics.Process? _ollamaProcess;
    private List<string> _visualImagePaths = new();
    
    // Modern UI Components
    private ModernSidebar _sidebar;
    private Panel _contentArea;
    private DirectorsConsole _directorsConsole;
    private StatusDashboard _statusDashboard;
    private Panel _currentPage;
    
    // Page panels
    private Panel _generatePage;
    private Panel _libraryPage;
    private Panel _settingsPage;
    private Panel _statusPage;
    private Panel _debugPage;
    
    // Services
    private IScriptGeneratorService? _scriptGenerator;
    private FFmpegVideoAssembly? _videoAssembly;
    
    public MainFormModern()
    {
        _config = new AppConfig();
        InitializeComponent();
        LoadConfiguration();
        InitializeServices();
        InitializeModernUI();
        CheckSystemStatus();
    }
    
    private void InitializeComponent()
    {
        // Main Form
        this.Text = "VOID VIDEO GENERATOR - Professional AI Video Platform";
        this.Size = new Size(1400, 900);
        this.MinimumSize = new Size(1200, 700);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = ModernTheme.Background;
        this.ForeColor = ModernTheme.TextPrimary;
        
        // Set application icon
        try
        {
            if (File.Exists("src/icon.ico"))
                this.Icon = new Icon("src/icon.ico");
            else if (File.Exists("icon.ico"))
                this.Icon = new Icon("icon.ico");
        }
        catch { }
    }
    
    private void InitializeModernUI()
    {
        // Create sidebar
        _sidebar = new ModernSidebar();
        _sidebar.NavigationChanged += OnNavigationChanged;
        this.Controls.Add(_sidebar);
        
        // Create content area
        _contentArea = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = ModernTheme.Background,
            Padding = new Padding(20),
            AutoScroll = true
        };
        this.Controls.Add(_contentArea);
        
        // Initialize all pages
        InitializeGeneratePage();
        InitializeLibraryPage();
        InitializeSettingsPage();
        InitializeStatusPage();
        InitializeDebugPage();
        
        // Show default page
        ShowPage("generate");
        
        // Update sidebar status
        UpdateSidebarStatus();
    }
    
    private void InitializeGeneratePage()
    {
        _generatePage = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = ModernTheme.Background,
            AutoScroll = true
        };
        
        int yPos = 0;
        
        // Page title
        var titleLabel = new Label
        {
            Text = "🎬 Generate Video",
            Location = new Point(0, yPos),
            AutoSize = true,
            Font = new Font("Segoe UI", 24f, FontStyle.Bold),
            ForeColor = ModernTheme.TextPrimary
        };
        _generatePage.Controls.Add(titleLabel);
        yPos += 50;
        
        // Director's Console
        _directorsConsole = new DirectorsConsole
        {
            Location = new Point(0, yPos),
            Width = 900
        };
        _generatePage.Controls.Add(_directorsConsole);
        yPos += _directorsConsole.Height + 20;
        
        // Channel DNA Card
        var channelCard = new ModernSettingsCard
        {
            Title = "Channel DNA",
            Icon = "🎯",
            Description = "Define your channel's identity and style",
            Location = new Point(0, yPos),
            Width = 900
        };
        
        var txtNiche = new TextBox
        {
            Size = new Size(700, 25),
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle,
            Text = _config.DefaultChannelDNA.Niche
        };
        channelCard.AddLabeledControl("Niche:", txtNiche, 0);
        
        var txtPersona = new TextBox
        {
            Size = new Size(700, 25),
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle,
            Text = _config.DefaultChannelDNA.HostPersona
        };
        channelCard.AddLabeledControl("Persona:", txtPersona, 35);
        
        var txtStyle = new TextBox
        {
            Size = new Size(700, 25),
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle,
            Text = _config.DefaultChannelDNA.ContentStyle
        };
        channelCard.AddLabeledControl("Style:", txtStyle, 70);
        
        _generatePage.Controls.Add(channelCard);
        yPos += channelCard.Height + 20;
        
        // Output Settings Card
        var outputCard = new ModernSettingsCard
        {
            Title = "Output Settings",
            Icon = "💾",
            Description = "Configure video output location and format",
            Location = new Point(0, yPos),
            Width = 900
        };
        
        var txtOutputPath = new TextBox
        {
            Size = new Size(600, 25),
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle,
            Text = _config.DefaultOutputPath
        };
        outputCard.AddLabeledControl("Output Folder:", txtOutputPath, 0);
        
        var btnBrowse = new ModernButton
        {
            Text = "Browse...",
            Location = new Point(760, 0),
            Size = new Size(100, 30),
            Style = ModernButton.ButtonStyle.Secondary
        };
        btnBrowse.Click += (s, e) => BrowseOutputFolder(txtOutputPath);
        outputCard.ContentArea.Controls.Add(btnBrowse);
        
        _generatePage.Controls.Add(outputCard);
        yPos += outputCard.Height + 20;
        
        // Generate Button
        var btnGenerate = new ModernButton
        {
            Text = "🎬 Generate Video",
            Location = new Point(0, yPos),
            Size = new Size(200, 45),
            Style = ModernButton.ButtonStyle.Primary,
            BorderRadius = BorderRadius.MD
        };
        btnGenerate.Click += async (s, e) => await GenerateVideoAsync();
        _generatePage.Controls.Add(btnGenerate);
        
        // Progress indicator
        var progressBar = new ModernProgressBar
        {
            Location = new Point(220, yPos + 7),
            Size = new Size(680, 30),
            Visible = false,
            ShowPercentage = true
        };
        _generatePage.Controls.Add(progressBar);
    }
    
    private void InitializeLibraryPage()
    {
        _libraryPage = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = ModernTheme.Background,
            AutoScroll = true
        };
        
        var titleLabel = new Label
        {
            Text = "📚 Video Library",
            Location = new Point(0, 0),
            AutoSize = true,
            Font = new Font("Segoe UI", 24f, FontStyle.Bold),
            ForeColor = ModernTheme.TextPrimary
        };
        _libraryPage.Controls.Add(titleLabel);
        
        var infoLabel = new Label
        {
            Text = "Your generated videos will appear here. This feature is coming soon!",
            Location = new Point(0, 60),
            AutoSize = true,
            Font = ModernFonts.Body,
            ForeColor = ModernTheme.TextSecondary
        };
        _libraryPage.Controls.Add(infoLabel);
    }
    
    private void InitializeSettingsPage()
    {
        _settingsPage = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = ModernTheme.Background,
            AutoScroll = true
        };
        
        int yPos = 0;
        
        var titleLabel = new Label
        {
            Text = "⚙️ Settings",
            Location = new Point(0, yPos),
            AutoSize = true,
            Font = new Font("Segoe UI", 24f, FontStyle.Bold),
            ForeColor = ModernTheme.TextPrimary
        };
        _settingsPage.Controls.Add(titleLabel);
        yPos += 50;
        
        // AI Provider Card
        var aiCard = new ModernSettingsCard
        {
            Title = "AI Script Generator",
            Icon = "🤖",
            Description = "Configure your AI provider for script generation",
            Location = new Point(0, yPos),
            Width = 900
        };
        
        var cmbProvider = new ComboBox
        {
            Size = new Size(300, 25),
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary
        };
        cmbProvider.Items.AddRange(new[] { "Ollama (Local)", "OpenAI", "Anthropic Claude", "Google Gemini" });
        cmbProvider.SelectedIndex = _config.AiProvider.ToLower() switch
        {
            "openai" => 1,
            "anthropic" => 2,
            "gemini" => 3,
            _ => 0
        };
        aiCard.AddLabeledControl("Provider:", cmbProvider, 0);
        
        var txtModel = new TextBox
        {
            Size = new Size(300, 25),
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle,
            Text = _config.OllamaModel
        };
        aiCard.AddLabeledControl("Model:", txtModel, 35);
        
        _settingsPage.Controls.Add(aiCard);
        yPos += aiCard.Height + 16;
        
        // Voice Generation Card
        var voiceCard = new ModernSettingsCard
        {
            Title = "Voice Generation",
            Icon = "🎙️",
            Description = "Text-to-speech settings for narration",
            Location = new Point(0, yPos),
            Width = 900
        };
        
        var txtPiperPath = new TextBox
        {
            Size = new Size(600, 25),
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle,
            Text = _config.PiperPath
        };
        voiceCard.AddLabeledControl("Piper Path:", txtPiperPath, 0);
        
        var txtPiperModel = new TextBox
        {
            Size = new Size(600, 25),
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle,
            Text = _config.PiperModelPath
        };
        voiceCard.AddLabeledControl("Model Path:", txtPiperModel, 35);
        
        _settingsPage.Controls.Add(voiceCard);
        yPos += voiceCard.Height + 16;
        
        // Video Encoding Card
        var videoCard = new ModernSettingsCard
        {
            Title = "Video Encoding",
            Icon = "🎬",
            Description = "FFmpeg and encoding settings",
            Location = new Point(0, yPos),
            Width = 900
        };
        
        var txtFFmpegPath = new TextBox
        {
            Size = new Size(600, 25),
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle,
            Text = _config.FFmpegPath
        };
        videoCard.AddLabeledControl("FFmpeg Path:", txtFFmpegPath, 0);
        
        var chkGpu = new CheckBox
        {
            Text = "Enable GPU Acceleration",
            Checked = _config.UseGpuAcceleration,
            ForeColor = ModernTheme.TextPrimary,
            AutoSize = true
        };
        videoCard.ContentArea.Controls.Add(chkGpu);
        chkGpu.Location = new Point(150, 35);
        
        _settingsPage.Controls.Add(videoCard);
        yPos += videoCard.Height + 16;
        
        // Save Button
        var btnSave = new ModernButton
        {
            Text = "💾 Save All Settings",
            Location = new Point(0, yPos),
            Size = new Size(200, 40),
            Style = ModernButton.ButtonStyle.Success,
            BorderRadius = BorderRadius.MD
        };
        btnSave.Click += (s, e) => SaveAllSettings();
        _settingsPage.Controls.Add(btnSave);
    }
    
    private void InitializeStatusPage()
    {
        _statusPage = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = ModernTheme.Background
        };
        
        _statusDashboard = new StatusDashboard
        {
            Dock = DockStyle.Fill
        };
        _statusPage.Controls.Add(_statusDashboard);
    }
    
    private void InitializeDebugPage()
    {
        _debugPage = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = ModernTheme.Background,
            Padding = new Padding(20)
        };
        
        var titleLabel = new Label
        {
            Text = "🐛 Debug Console",
            Location = new Point(0, 0),
            AutoSize = true,
            Font = new Font("Segoe UI", 24f, FontStyle.Bold),
            ForeColor = ModernTheme.TextPrimary
        };
        _debugPage.Controls.Add(titleLabel);
        
        var consoleBox = new TextBox
        {
            Location = new Point(0, 50),
            Size = new Size(1100, 600),
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            ReadOnly = true,
            BackColor = Color.Black,
            ForeColor = Color.LimeGreen,
            BorderStyle = BorderStyle.FixedSingle,
            Font = ModernFonts.Code
        };
        _debugPage.Controls.Add(consoleBox);
        
        var btnStartOllama = new ModernButton
        {
            Text = "▶️ Start Ollama",
            Location = new Point(0, 660),
            Size = new Size(150, 35),
            Style = ModernButton.ButtonStyle.Success
        };
        btnStartOllama.Click += (s, e) => StartOllama(consoleBox);
        _debugPage.Controls.Add(btnStartOllama);
        
        var btnClear = new ModernButton
        {
            Text = "🗑️ Clear",
            Location = new Point(160, 660),
            Size = new Size(100, 35),
            Style = ModernButton.ButtonStyle.Outline
        };
        btnClear.Click += (s, e) => consoleBox.Clear();
        _debugPage.Controls.Add(btnClear);
    }
    
    private void OnNavigationChanged(object? sender, string pageId)
    {
        ShowPage(pageId);
    }
    
    private void ShowPage(string pageId)
    {
        _contentArea.Controls.Clear();
        
        _currentPage = pageId switch
        {
            "generate" => _generatePage,
            "library" => _libraryPage,
            "settings" => _settingsPage,
            "status" => _statusPage,
            "debug" => _debugPage,
            _ => _generatePage
        };
        
        _contentArea.Controls.Add(_currentPage);
        _sidebar.SetActiveItem(pageId);
    }
    
    private void LoadConfiguration()
    {
        try
        {
            if (File.Exists(_configPath))
            {
                var json = File.ReadAllText(_configPath);
                _config = JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
                LogActivity("Configuration loaded successfully", ActivityType.Success);
            }
            else
            {
                _config = new AppConfig();
                SaveConfiguration();
                LogActivity("Created default configuration", ActivityType.Info);
            }
        }
        catch (Exception ex)
        {
            _config = new AppConfig();
            LogActivity($"Error loading config: {ex.Message}", ActivityType.Error);
        }
    }
    
    private void SaveConfiguration()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(_config, options);
            File.WriteAllText(_configPath, json);
            LogActivity("Configuration saved successfully", ActivityType.Success);
        }
        catch (Exception ex)
        {
            LogActivity($"Error saving config: {ex.Message}", ActivityType.Error);
        }
    }
    
    private void InitializeServices()
    {
        try
        {
            _scriptGenerator = CreateScriptGenerator();
            var voiceGenerator = new PiperTTSService(_config.PiperPath, _config.PiperModelPath);
            _videoAssembly = new FFmpegVideoAssembly(_config.FFmpegPath, _config.UseGpuAcceleration, _config.GpuEncoder, _config.VideoSettings);
            
            IAIVideoGeneratorService? aiVideoGenerator = null;
            if (_config.AIVideoGeneration.Provider != "None")
            {
                try
                {
                    var factory = new AIVideoGeneratorFactory(_config.AIVideoGeneration);
                    aiVideoGenerator = factory.CreateGenerator();
                }
                catch (Exception ex)
                {
                    LogActivity($"Could not initialize AI video generator: {ex.Message}", ActivityType.Warning);
                }
            }
            
            _pipeline = new VideoGenerationPipeline(_scriptGenerator, voiceGenerator, _videoAssembly, aiVideoGenerator, _config.AIVideoGeneration);
            LogActivity("Services initialized successfully", ActivityType.Success);
        }
        catch (Exception ex)
        {
            LogActivity($"Error initializing services: {ex.Message}", ActivityType.Error);
        }
    }
    
    private IScriptGeneratorService CreateScriptGenerator()
    {
        return _config.AiProvider.ToLower() switch
        {
            "openai" => new OpenAIScriptGenerator(_config.OpenAiApiKey, _config.OpenAiModel),
            "anthropic" => new AnthropicScriptGenerator(_config.AnthropicApiKey, _config.AnthropicModel),
            "gemini" => new GeminiScriptGenerator(_config.GeminiApiKey, _config.GeminiModel),
            _ => new OllamaScriptGenerator(_config.OllamaUrl, _config.OllamaModel)
        };
    }
    
    private async void CheckSystemStatus()
    {
        if (_pipeline == null) return;
        
        try
        {
            var status = await _pipeline.CheckServicesAsync();
            
            _sidebar.UpdateServiceStatus("Ollama", 
                status.GetValueOrDefault("Ollama") ? ServiceStatus.Healthy : ServiceStatus.Error);
            _sidebar.UpdateServiceStatus("Piper", 
                status.GetValueOrDefault("Piper (TTS)") ? ServiceStatus.Healthy : ServiceStatus.Error);
            _sidebar.UpdateServiceStatus("FFmpeg", 
                status.GetValueOrDefault("FFmpeg") ? ServiceStatus.Healthy : ServiceStatus.Error);
            
            if (_statusDashboard != null)
            {
                _statusDashboard.UpdateServiceStatus("Ollama",
                    status.GetValueOrDefault("Ollama") ? ServiceStatus.Healthy : ServiceStatus.Error,
                    status.GetValueOrDefault("Ollama") ? "Connected" : "Not available");
                _statusDashboard.UpdateServiceStatus("Piper TTS",
                    status.GetValueOrDefault("Piper (TTS)") ? ServiceStatus.Healthy : ServiceStatus.Error,
                    status.GetValueOrDefault("Piper (TTS)") ? "Ready" : "Not available");
                _statusDashboard.UpdateServiceStatus("FFmpeg",
                    status.GetValueOrDefault("FFmpeg") ? ServiceStatus.Healthy : ServiceStatus.Error,
                    status.GetValueOrDefault("FFmpeg") ? "Available" : "Not available");
            }
        }
        catch (Exception ex)
        {
            LogActivity($"Status check failed: {ex.Message}", ActivityType.Error);
        }
    }
    
    private void UpdateSidebarStatus()
    {
        // Initial status - will be updated by CheckSystemStatus
        _sidebar.UpdateServiceStatus("Ollama", ServiceStatus.Unknown);
        _sidebar.UpdateServiceStatus("Piper", ServiceStatus.Unknown);
        _sidebar.UpdateServiceStatus("FFmpeg", ServiceStatus.Unknown);
    }
    
    private async Task GenerateVideoAsync()
    {
        LogActivity("Starting video generation...", ActivityType.Info);
        
        try
        {
            var prompt = _directorsConsole.GetVideoPrompt();
            LogActivity($"Using prompt: {prompt.Text}", ActivityType.Info);
            
            // TODO: Implement full generation pipeline
            await Task.Delay(1000); // Placeholder
            
            LogActivity("Video generation complete!", ActivityType.Success);
            MessageBox.Show("Video generation feature will be fully integrated soon!", 
                "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            LogActivity($"Generation failed: {ex.Message}", ActivityType.Error);
            MessageBox.Show($"Error: {ex.Message}", "Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    
    private void BrowseOutputFolder(TextBox textBox)
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = "Select output folder for generated videos",
            UseDescriptionForTitle = true
        };
        
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            textBox.Text = dialog.SelectedPath;
        }
    }
    
    private void SaveAllSettings()
    {
        SaveConfiguration();
        MessageBox.Show("Settings saved successfully!", "Success", 
            MessageBoxButtons.OK, MessageBoxIcon.Information);
        LogActivity("Settings saved", ActivityType.Success);
    }
    
    private void StartOllama(TextBox consoleBox)
    {
        try
        {
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "ollama",
                Arguments = "serve",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            
            _ollamaProcess = new System.Diagnostics.Process { StartInfo = startInfo };
            
            _ollamaProcess.OutputDataReceived += (s, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    if (consoleBox.InvokeRequired)
                        consoleBox.Invoke(() => consoleBox.AppendText($"[OUT] {args.Data}\r\n"));
                    else
                        consoleBox.AppendText($"[OUT] {args.Data}\r\n");
                }
            };
            
            _ollamaProcess.Start();
            _ollamaProcess.BeginOutputReadLine();
            _ollamaProcess.BeginErrorReadLine();
            
            LogActivity("Ollama server started", ActivityType.Success);
        }
        catch (Exception ex)
        {
            LogActivity($"Failed to start Ollama: {ex.Message}", ActivityType.Error);
        }
    }
    
    private void LogActivity(string message, ActivityType type)
    {
        _statusDashboard?.LogActivity(message, type);
    }
    
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);
        
        if (_ollamaProcess != null && !_ollamaProcess.HasExited)
        {
            try
            {
                _ollamaProcess.Kill();
                _ollamaProcess.Dispose();
            }
            catch { }
        }
    }
}
