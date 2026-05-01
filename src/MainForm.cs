namespace VoidVideoGenerator;

using System.Text.Json;
using VoidVideoGenerator.Models;
using VoidVideoGenerator.Services;

public partial class MainForm : Form
{
    private AppConfig _config;
    private VideoGenerationPipeline? _pipeline;
    private readonly string _configPath = "config.json";
    private System.Diagnostics.Process? _ollamaProcess;
    private List<string> _visualImagePaths = new();

    public MainForm()
    {
        _config = new AppConfig(); // Initialize before InitializeComponent
        InitializeComponent();
        LoadConfiguration();
        InitializeServices();
        PopulateFormFromConfig();
        ApplyTheme(_config.DarkMode);
        CheckOllamaRunning();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);
        // Stop Ollama if we started it
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

    private void LoadConfiguration()
    {
        try
        {
            if (File.Exists(_configPath))
            {
                var json = File.ReadAllText(_configPath);
                _config = JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
                LogMessage("Configuration loaded successfully");
            }
            else
            {
                _config = new AppConfig();
                SaveConfiguration();
                LogMessage("Created default configuration");
            }
        }
        catch (Exception ex)
        {
            _config = new AppConfig();
            LogMessage($"Error loading config: {ex.Message}");
        }
    }

    private void SaveConfiguration()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(_config, options);
            File.WriteAllText(_configPath, json);
            LogMessage("Configuration saved successfully");
        }
        catch (Exception ex)
        {
            LogMessage($"Error saving config: {ex.Message}");
        }
    }

    private IScriptGeneratorService? _scriptGenerator;
    private FFmpegVideoAssembly? _videoAssembly;
    
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_scriptGenerator is IDisposable disposable)
            {
                disposable.Dispose();
            }
            _ollamaProcess?.Dispose();
            components?.Dispose();
        }
        base.Dispose(disposing);
    }
    
    private void InitializeServices()
    {
        try
        {
            // Initialize script generator based on AI provider
            _scriptGenerator = CreateScriptGenerator();
            
            var voiceGenerator = new PiperTTSService(_config.PiperPath, _config.PiperModelPath);
            _videoAssembly = new FFmpegVideoAssembly(_config.FFmpegPath, _config.UseGpuAcceleration, _config.GpuEncoder, _config.VideoSettings);

            _pipeline = new VideoGenerationPipeline(_scriptGenerator, voiceGenerator, _videoAssembly);
            LogMessage("Services initialized");
            LogMessage($"AI Provider: {_config.AiProvider.ToUpper()}");
            LogMessage($"GPU Acceleration: {(_config.UseGpuAcceleration ? "Enabled" : "Disabled")}");
            if (_config.UseGpuAcceleration)
            {
                LogMessage($"GPU Encoder: {_config.GpuEncoder}");
            }
            LogMessage($"Video Output: {_config.VideoSettings.Resolution} @ {_config.VideoSettings.FrameRate}fps, {_config.VideoSettings.QualityPreset} quality");
        }
        catch (Exception ex)
        {
            LogMessage($"Error initializing services: {ex.Message}");
        }
    }

    private IScriptGeneratorService CreateScriptGenerator()
    {
        var provider = _config.AiProvider.ToLower();
        
        switch (provider)
        {
            case "openai":
                if (string.IsNullOrEmpty(_config.OpenAiApiKey))
                {
                    throw new Exception("OpenAI API key is not configured. Please add it to config.json");
                }
                LogMessage($"Using OpenAI ({_config.OpenAiModel})");
                return new OpenAIScriptGenerator(_config.OpenAiApiKey, _config.OpenAiModel);
            
            case "anthropic":
            case "claude":
                if (string.IsNullOrEmpty(_config.AnthropicApiKey))
                {
                    throw new Exception("Anthropic API key is not configured. Please add it to config.json");
                }
                LogMessage($"Using Anthropic Claude ({_config.AnthropicModel})");
                return new AnthropicScriptGenerator(_config.AnthropicApiKey, _config.AnthropicModel);
            
            case "gemini":
            case "google":
                if (string.IsNullOrEmpty(_config.GeminiApiKey))
                {
                    throw new Exception("Gemini API key is not configured. Please add it to config.json");
                }
                LogMessage($"Using Google Gemini ({_config.GeminiModel})");
                return new GeminiScriptGenerator(_config.GeminiApiKey, _config.GeminiModel);
            
            case "ollama":
            default:
                LogMessage($"Using Ollama ({_config.OllamaModel})");
                return new OllamaScriptGenerator(_config.OllamaUrl, _config.OllamaModel);
        }
    }

    private void PopulateFormFromConfig()
    {
        // Settings tab - AI Provider
        cmbAiProvider.SelectedIndex = _config.AiProvider.ToLower() switch
        {
            "openai" => 1,
            "anthropic" or "claude" => 2,
            "gemini" or "google" => 3,
            _ => 0 // Ollama
        };
        
        txtOllamaUrl.Text = _config.OllamaUrl;
        txtOllamaModel.Text = _config.OllamaModel;
        txtOpenAiApiKey.Text = _config.OpenAiApiKey;
        txtOpenAiModel.Text = _config.OpenAiModel;
        txtAnthropicApiKey.Text = _config.AnthropicApiKey;
        txtAnthropicModel.Text = _config.AnthropicModel;
        txtGeminiApiKey.Text = _config.GeminiApiKey;
        txtGeminiModel.Text = _config.GeminiModel;
        txtPiperPath.Text = _config.PiperPath;
        txtPiperModel.Text = _config.PiperModelPath;
        txtFFmpegPath.Text = _config.FFmpegPath;
        chkUseUnsplash.Checked = _config.UseUnsplashImages;
        txtUnsplashApiKey.Text = _config.UnsplashApiKey;
        txtUnsplashApiKey.Enabled = _config.UseUnsplashImages;
        chkDarkMode.Checked = _config.DarkMode;
        chkUseGpu.Checked = _config.UseGpuAcceleration;
        cmbGpuEncoder.SelectedItem = _config.GpuEncoder;

        // Video output settings
        cmbResolution.SelectedItem = _config.VideoSettings.Resolution;
        cmbQuality.SelectedItem = _config.VideoSettings.QualityPreset;
        cmbFrameRate.SelectedItem = $"{_config.VideoSettings.FrameRate} fps";
        numVideoBitrate.Value = _config.VideoSettings.VideoBitrate;
        numAudioBitrate.Value = _config.VideoSettings.AudioBitrate;
        cmbAudioChannels.SelectedItem = _config.VideoSettings.AudioChannels == "mono" ? "Mono" : "Stereo";

        // Animation settings
        chkEnableKenBurns.Checked = _config.VideoSettings.EnableKenBurnsEffect;
        chkEnableCrossfade.Checked = _config.VideoSettings.EnableCrossfadeTransitions;
        numTransitionDuration.Value = (decimal)_config.VideoSettings.TransitionDuration;
        numZoomIntensity.Value = (decimal)_config.VideoSettings.ZoomIntensity;

        // Whisper settings
        txtWhisperPath.Text = _config.WhisperPath;
        cmbWhisperModel.SelectedItem = _config.WhisperModel;
        chkUseWhisperApi.Checked = _config.UseWhisperApi;

        // Generate tab - Channel DNA defaults
        txtNiche.Text = _config.DefaultChannelDNA.Niche;
        txtPersona.Text = _config.DefaultChannelDNA.HostPersona;
        txtTone.Text = _config.DefaultChannelDNA.ToneGuidelines;
        txtAudience.Text = _config.DefaultChannelDNA.TargetAudience;
        txtStyle.Text = _config.DefaultChannelDNA.ContentStyle;
        txtOutputPath.Text = _config.DefaultOutputPath;
    }

    private void BtnBrowseOutput_Click(object? sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = "Select output folder for generated videos",
            UseDescriptionForTitle = true
        };

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            txtOutputPath.Text = dialog.SelectedPath;
        }
    }

    private void BtnAddImages_Click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Select Images for Video",
            Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif|All Files|*.*",
            Multiselect = true
        };

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            foreach (var file in dialog.FileNames)
            {
                if (!_visualImagePaths.Contains(file))
                {
                    _visualImagePaths.Add(file);
                    lstVisuals.Items.Add(Path.GetFileName(file));
                }
            }
            
            LogMessage($"Added {dialog.FileNames.Length} image(s) to visuals");
        }
    }

    private void BtnRemoveImages_Click(object? sender, EventArgs e)
    {
        if (lstVisuals.SelectedIndices.Count == 0)
        {
            MessageBox.Show("Please select images to remove.", "No Selection",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        // Remove in reverse order to maintain correct indices
        var selectedIndices = lstVisuals.SelectedIndices.Cast<int>().OrderByDescending(i => i).ToList();
        foreach (var index in selectedIndices)
        {
            _visualImagePaths.RemoveAt(index);
            lstVisuals.Items.RemoveAt(index);
        }
        
        LogMessage($"Removed {selectedIndices.Count} image(s) from visuals");
    }

    private void BtnClearImages_Click(object? sender, EventArgs e)
    {
        if (_visualImagePaths.Count == 0)
        {
            return;
        }

        var result = MessageBox.Show(
            $"Are you sure you want to clear all {_visualImagePaths.Count} images?",
            "Clear All Images",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            _visualImagePaths.Clear();
            lstVisuals.Items.Clear();
            LogMessage("Cleared all images from visuals");
        }
    }

    private async void BtnGenerate_Click(object? sender, EventArgs e)
    {
        if (_pipeline == null)
        {
            MessageBox.Show("Services not initialized. Please check settings.", "Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // Validate inputs
        if (string.IsNullOrWhiteSpace(txtTitle.Text))
        {
            MessageBox.Show("Please enter a video title.", "Validation Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(txtTopic.Text))
        {
            MessageBox.Show("Please enter a topic/description.", "Validation Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Disable controls during generation
        btnGenerate.Enabled = false;
        progressBar.Visible = true;
        txtLog.Clear();

        try
        {
            var request = new VideoRequest
            {
                Title = txtTitle.Text.Trim(),
                Topic = txtTopic.Text.Trim(),
                TargetDurationSeconds = (int)numDuration.Value,
                OutputPath = Path.Combine(txtOutputPath.Text, 
                    $"{DateTime.Now:yyyyMMdd_HHmmss}_{SanitizeFileName(txtTitle.Text)}"),
                ChannelDNA = new ChannelDNA
                {
                    Niche = txtNiche.Text,
                    HostPersona = txtPersona.Text,
                    ToneGuidelines = txtTone.Text,
                    TargetAudience = txtAudience.Text,
                    ContentStyle = txtStyle.Text
                }
            };

            // Create output directory
            Directory.CreateDirectory(request.OutputPath);

            IProgress<string> progress = new Progress<string>(message =>
            {
                if (InvokeRequired)
                {
                    Invoke(() => LogMessage(message));
                }
                else
                {
                    LogMessage(message);
                }
            });

            LogMessage("=== Starting Video Generation ===");
            LogMessage($"Title: {request.Title}");
            LogMessage($"Duration: {request.TargetDurationSeconds} seconds");
            LogMessage($"Output: {request.OutputPath}");
            LogMessage("");

            // Generate script first to get visual cues
            var script = await _scriptGenerator!.GenerateScriptAsync(request, progress);
            
            // Handle visuals - priority: user images > Unsplash > placeholders
            var visualsDir = Path.Combine(request.OutputPath, "visuals");
            Directory.CreateDirectory(visualsDir);
            
            if (_visualImagePaths.Count > 0)
            {
                // Use user-provided images
                LogMessage($"Using {_visualImagePaths.Count} user-provided images...");
                
                for (int i = 0; i < _visualImagePaths.Count; i++)
                {
                    var sourcePath = _visualImagePaths[i];
                    var extension = Path.GetExtension(sourcePath);
                    var destPath = Path.Combine(visualsDir, $"image_{i:D3}{extension}");
                    File.Copy(sourcePath, destPath, true);
                }
                
                LogMessage($"✓ Copied {_visualImagePaths.Count} images to visuals directory");
            }
            else if (_config.UseUnsplashImages && !string.IsNullOrEmpty(_config.UnsplashApiKey))
            {
                // Use Unsplash to generate images from visual cues
                try
                {
                    LogMessage("Generating images from Unsplash based on visual cues...");
                    var unsplashService = new UnsplashImageService(_config.UnsplashApiKey);
                    
                    if (script.VisualCues.Count > 0)
                    {
                        await unsplashService.DownloadImagesFromCuesAsync(
                            script.VisualCues,
                            visualsDir,
                            imagesPerCue: 1,
                            progress);
                    }
                    else
                    {
                        LogMessage("⚠ No visual cues found in script, using topic for image search");
                        await unsplashService.DownloadImagesFromCuesAsync(
                            new List<string> { request.Topic },
                            visualsDir,
                            imagesPerCue: 3,
                            progress);
                    }
                }
                catch (Exception ex)
                {
                    LogMessage($"⚠ Unsplash image generation failed: {ex.Message}");
                    LogMessage("Falling back to placeholder visuals");
                }
            }
            else
            {
                LogMessage("⚠ No images provided - video will use placeholder visuals");
            }
            
            LogMessage("");

            // Continue with rest of pipeline (audio and video assembly)
            var audioDirectory = Path.Combine(request.OutputPath, "audio");
            Directory.CreateDirectory(audioDirectory);
            
            progress.Report("Step 2/4: Generating voice audio...");
            var voiceGenerator = new PiperTTSService(_config.PiperPath, _config.PiperModelPath);
            var audioFiles = await voiceGenerator.GenerateScriptAudioAsync(script, audioDirectory, progress);
            
            // Save script for reference
            var scriptPath = Path.Combine(request.OutputPath, "script.txt");
            await File.WriteAllTextAsync(scriptPath, script.FullText);
            progress.Report($"Script saved to: {scriptPath}");
            
            // Assemble video
            progress.Report("Step 4/4: Assembling final video...");
            var videoPath = Path.Combine(request.OutputPath, $"{SanitizeFileName(request.Title)}.mp4");
            await _videoAssembly!.CreateVideoFromScriptAsync(script, audioDirectory, visualsDir, videoPath, progress);
            
            progress.Report($"✓ Video generation complete: {videoPath}");

            MessageBox.Show($"Video generated successfully!\n\nLocation: {videoPath}", 
                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            LogMessage($"ERROR: {ex.Message}");
            MessageBox.Show($"Error generating video:\n\n{ex.Message}", "Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnGenerate.Enabled = true;
            progressBar.Visible = false;
        }
    }

    private async void BtnCheckStatus_Click(object? sender, EventArgs e)
    {
        if (_pipeline == null)
        {
            txtSystemStatus.Text = "Services not initialized. Please check settings.";
            return;
        }

        btnCheckStatus.Enabled = false;
        txtSystemStatus.Text = "Checking system status...\r\n\r\n";

        try
        {
            var status = await _pipeline.CheckServicesAsync();

            txtSystemStatus.AppendText("=== SYSTEM STATUS CHECK ===\r\n\r\n");

            foreach (var service in status)
            {
                var statusIcon = service.Value ? "✓" : "✗";
                var statusText = service.Value ? "Available" : "Not Available";
                txtSystemStatus.AppendText($"{statusIcon} {service.Key}: {statusText}\r\n");
            }

            // Get detailed Ollama diagnostics
            if (_scriptGenerator != null)
            {
                txtSystemStatus.AppendText("\r\n=== OLLAMA DIAGNOSTICS ===\r\n\r\n");
                var ollamaDiag = await _scriptGenerator.GetDiagnosticInfoAsync();
                txtSystemStatus.AppendText($"{ollamaDiag}\r\n");
            }

            txtSystemStatus.AppendText("\r\n=== CONFIGURATION ===\r\n\r\n");
            txtSystemStatus.AppendText($"Ollama URL: {_config.OllamaUrl}\r\n");
            txtSystemStatus.AppendText($"Ollama Model: {_config.OllamaModel}\r\n");
            txtSystemStatus.AppendText($"Piper Path: {_config.PiperPath}\r\n");
            txtSystemStatus.AppendText($"Piper Model: {_config.PiperModelPath}\r\n");
            txtSystemStatus.AppendText($"FFmpeg Path: {_config.FFmpegPath}\r\n");

            txtSystemStatus.AppendText("\r\n=== INSTALLATION NOTES ===\r\n\r\n");
            
            if (!status["Ollama (LLM)"])
            {
                txtSystemStatus.AppendText("⚠ Ollama not detected:\r\n");
                txtSystemStatus.AppendText("  1. Download from: https://ollama.com\r\n");
                txtSystemStatus.AppendText("  2. Install and run: ollama serve\r\n");
                txtSystemStatus.AppendText("  3. Pull a model: ollama pull llama3.1\r\n\r\n");
            }

            if (!status["Piper (TTS)"])
            {
                txtSystemStatus.AppendText("⚠ Piper not detected:\r\n");
                txtSystemStatus.AppendText("  1. Download from: https://github.com/rhasspy/piper\r\n");
                txtSystemStatus.AppendText("  2. Download a voice model (.onnx file)\r\n");
                txtSystemStatus.AppendText("  3. Update paths in Settings tab\r\n\r\n");
            }

            if (!status["FFmpeg"])
            {
                txtSystemStatus.AppendText("⚠ FFmpeg not detected:\r\n");
                txtSystemStatus.AppendText("  1. Download from: https://ffmpeg.org/download.html\r\n");
                txtSystemStatus.AppendText("  2. Add to system PATH or specify full path in Settings\r\n\r\n");
            }

            var allAvailable = status.Values.All(v => v);
            if (allAvailable)
            {
                txtSystemStatus.AppendText("\r\n✓ All systems ready! You can start generating videos.\r\n");
            }
            else
            {
                txtSystemStatus.AppendText("\r\n⚠ Some services are not available. Please install missing components.\r\n");
                txtSystemStatus.AppendText("See TROUBLESHOOTING.md for detailed help.\r\n");
            }
        }
        catch (Exception ex)
        {
            txtSystemStatus.AppendText($"\r\nError checking status: {ex.Message}\r\n");
        }
        finally
        {
            btnCheckStatus.Enabled = true;
        }
    }

    private void BtnSaveSettings_Click(object? sender, EventArgs e)
    {
        // AI Provider settings
        _config.AiProvider = cmbAiProvider.SelectedIndex switch
        {
            1 => "openai",
            2 => "anthropic",
            3 => "gemini",
            _ => "ollama"
        };
        
        _config.OllamaUrl = txtOllamaUrl.Text;
        _config.OllamaModel = txtOllamaModel.Text;
        _config.OpenAiApiKey = txtOpenAiApiKey.Text;
        _config.OpenAiModel = txtOpenAiModel.Text;
        _config.AnthropicApiKey = txtAnthropicApiKey.Text;
        _config.AnthropicModel = txtAnthropicModel.Text;
        _config.GeminiApiKey = txtGeminiApiKey.Text;
        _config.GeminiModel = txtGeminiModel.Text;
        _config.PiperPath = txtPiperPath.Text;
        _config.PiperModelPath = txtPiperModel.Text;
        _config.FFmpegPath = txtFFmpegPath.Text;
        _config.DefaultOutputPath = txtOutputPath.Text;
        _config.UseUnsplashImages = chkUseUnsplash.Checked;
        _config.UnsplashApiKey = txtUnsplashApiKey.Text;
        _config.DarkMode = chkDarkMode.Checked;
        _config.UseGpuAcceleration = chkUseGpu.Checked;
        _config.GpuEncoder = cmbGpuEncoder.SelectedItem?.ToString() ?? "auto";

        // Video output settings
        _config.VideoSettings.SetResolution(cmbResolution.SelectedItem?.ToString() ?? "1080p");
        _config.VideoSettings.SetQualityPreset(cmbQuality.SelectedItem?.ToString() ?? "Medium");
        _config.VideoSettings.FrameRate = int.Parse((cmbFrameRate.SelectedItem?.ToString() ?? "30 fps").Replace(" fps", ""));
        _config.VideoSettings.VideoBitrate = (int)numVideoBitrate.Value;
        _config.VideoSettings.AudioBitrate = (int)numAudioBitrate.Value;
        var audioChannelSelection = cmbAudioChannels.SelectedItem?.ToString() ?? "Stereo";
        _config.VideoSettings.AudioChannels = audioChannelSelection.ToLower() == "mono" ? "mono" : "stereo";

        // Animation settings
        _config.VideoSettings.EnableKenBurnsEffect = chkEnableKenBurns.Checked;
        _config.VideoSettings.EnableCrossfadeTransitions = chkEnableCrossfade.Checked;
        _config.VideoSettings.TransitionDuration = (double)numTransitionDuration.Value;
        _config.VideoSettings.ZoomIntensity = (double)numZoomIntensity.Value;

        // Whisper settings
        _config.WhisperPath = txtWhisperPath.Text;
        _config.WhisperModel = cmbWhisperModel.SelectedItem?.ToString() ?? "base";
        _config.UseWhisperApi = chkUseWhisperApi.Checked;

        _config.DefaultChannelDNA = new ChannelDNA
        {
            Niche = txtNiche.Text,
            HostPersona = txtPersona.Text,
            ToneGuidelines = txtTone.Text,
            TargetAudience = txtAudience.Text,
            ContentStyle = txtStyle.Text
        };

        SaveConfiguration();
        InitializeServices();
        ApplyTheme(_config.DarkMode);

        MessageBox.Show("Settings saved successfully!", "Success",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void ApplyTheme(bool darkMode)
    {
        var theme = darkMode ? ThemeColors.Dark : ThemeColors.Light;

        // Apply to form
        this.BackColor = theme.Background;
        this.ForeColor = theme.Text;

        // Apply to all controls recursively
        ApplyThemeToControls(this.Controls, theme);
    }

    private void ApplyThemeToControls(Control.ControlCollection controls, dynamic theme)
    {
        foreach (Control control in controls)
        {
            // Skip controls that should maintain their own colors
            if (control is Button btn)
            {
                btn.BackColor = theme.Primary;
                btn.ForeColor = Color.White;
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderColor = theme.Border;
                btn.FlatAppearance.BorderSize = 0;
                btn.Padding = new Padding(10, 5, 10, 5);
                btn.Cursor = Cursors.Hand;
            }
            else if (control is TextBox txt)
            {
                txt.BackColor = theme.InputBackground;
                txt.ForeColor = theme.Text;
                txt.BorderStyle = BorderStyle.FixedSingle;
            }
            else if (control is RichTextBox rtxt)
            {
                rtxt.BackColor = theme.InputBackground;
                rtxt.ForeColor = theme.Text;
                rtxt.BorderStyle = BorderStyle.FixedSingle;
            }
            else if (control is ComboBox cmb)
            {
                cmb.BackColor = theme.InputBackground;
                cmb.ForeColor = theme.Text;
                cmb.FlatStyle = FlatStyle.Flat;
            }
            else if (control is NumericUpDown num)
            {
                num.BackColor = theme.InputBackground;
                num.ForeColor = theme.Text;
                num.BorderStyle = BorderStyle.FixedSingle;
            }
            else if (control is ListBox lst)
            {
                lst.BackColor = theme.InputBackground;
                lst.ForeColor = theme.Text;
                lst.BorderStyle = BorderStyle.FixedSingle;
            }
            else if (control is CheckBox || control is RadioButton || control is Label)
            {
                control.ForeColor = theme.Text;
            }
            else if (control is GroupBox grp)
            {
                grp.ForeColor = theme.Text;
                grp.FlatStyle = FlatStyle.Flat;
            }
            else if (control is TabControl tabControl)
            {
                tabControl.BackColor = theme.Background;
                foreach (TabPage page in tabControl.TabPages)
                {
                    page.BackColor = theme.Background;
                    page.ForeColor = theme.Text;
                }
            }
            else if (control is TabPage page)
            {
                page.BackColor = theme.Background;
                page.ForeColor = theme.Text;
            }
            else if (control is Panel || control is FlowLayoutPanel || control is SplitContainer)
            {
                control.BackColor = theme.Background;
            }
            else if (control is ProgressBar pb)
            {
                // ProgressBar styling is limited in WinForms
                pb.BackColor = theme.Surface;
            }

            // Recursively apply to child controls
            if (control.HasChildren)
            {
                ApplyThemeToControls(control.Controls, theme);
            }
        }
    }

    private void LogMessage(string message)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        txtLog.AppendText($"[{timestamp}] {message}\r\n");
        txtLog.SelectionStart = txtLog.Text.Length;
        txtLog.ScrollToCaret();
    }

    private string SanitizeFileName(string fileName)
    {
        var invalid = Path.GetInvalidFileNameChars();
        return string.Join("_", fileName.Split(invalid, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
    }

    // === DEBUG CONSOLE METHODS ===

    private void CheckOllamaRunning()
    {
        Task.Run(async () =>
        {
            try
            {
                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };
                var response = await client.GetAsync("http://localhost:11434/api/tags");
                
                if (InvokeRequired)
                {
                    Invoke(() =>
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            AppendConsole("✓ Ollama server is already running");
                            btnStartOllama.Enabled = false;
                            btnStopOllama.Enabled = false; // External process
                        }
                        else
                        {
                            AppendConsole("⚠ Ollama server not detected");
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                if (InvokeRequired)
                {
                    Invoke(() => AppendConsole($"⚠ Ollama server not detected - Click 'Start Ollama Server' to launch. Error: {ex.Message}"));
                }
            }
        }).ContinueWith(t =>
        {
            if (t.IsFaulted && t.Exception != null)
            {
                // Log unhandled task exceptions
                System.Diagnostics.Debug.WriteLine($"CheckOllamaRunning failed: {t.Exception}");
            }
        }, TaskScheduler.Default);
    }

    private void BtnStartOllama_Click(object? sender, EventArgs e)
    {
        try
        {
            AppendConsole("=== Starting Ollama Server ===");
            AppendConsole($"Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            AppendConsole("");

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

            // Capture output
            _ollamaProcess.OutputDataReceived += (s, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    AppendConsole($"[OUT] {args.Data}");
                }
            };

            _ollamaProcess.ErrorDataReceived += (s, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    AppendConsole($"[ERR] {args.Data}");
                }
            };

            _ollamaProcess.Start();
            _ollamaProcess.BeginOutputReadLine();
            _ollamaProcess.BeginErrorReadLine();

            btnStartOllama.Enabled = false;
            btnStopOllama.Enabled = true;

            AppendConsole("✓ Ollama server started successfully");
            AppendConsole("Listening on http://localhost:11434");
            AppendConsole("");
        }
        catch (Exception ex)
        {
            AppendConsole($"✗ Failed to start Ollama: {ex.Message}");
            AppendConsole("");
            AppendConsole("Make sure Ollama is installed:");
            AppendConsole("  Download from: https://ollama.com");
            AppendConsole("  Or run manually: ollama serve");
            MessageBox.Show($"Failed to start Ollama:\n\n{ex.Message}\n\nMake sure Ollama is installed.",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnStopOllama_Click(object? sender, EventArgs e)
    {
        try
        {
            if (_ollamaProcess != null && !_ollamaProcess.HasExited)
            {
                AppendConsole("");
                AppendConsole("=== Stopping Ollama Server ===");
                _ollamaProcess.Kill();
                _ollamaProcess.Dispose();
                _ollamaProcess = null;

                btnStartOllama.Enabled = true;
                btnStopOllama.Enabled = false;

                AppendConsole("✓ Ollama server stopped");
                AppendConsole("");
            }
        }
        catch (Exception ex)
        {
            AppendConsole($"✗ Error stopping Ollama: {ex.Message}");
            MessageBox.Show($"Error stopping Ollama:\n\n{ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnClearConsole_Click(object? sender, EventArgs e)
    {
        txtOllamaConsole.Clear();
        AppendConsole("=== Console Cleared ===");
        AppendConsole($"Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        AppendConsole("");
    }

    private void AppendConsole(string message)
    {
        if (InvokeRequired)
        {
            Invoke(() => AppendConsole(message));
            return;
        }

        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        txtOllamaConsole.AppendText($"[{timestamp}] {message}\r\n");
        txtOllamaConsole.SelectionStart = txtOllamaConsole.Text.Length;
        txtOllamaConsole.ScrollToCaret();
    }

    private void CmbAiProvider_SelectedIndexChanged(object? sender, EventArgs e)
    {
        // Show/hide relevant fields based on selected provider
        // This provides visual feedback about which settings are active
        var selectedIndex = cmbAiProvider.SelectedIndex;
        
        // You could add visual indicators here if desired
        // For now, all fields remain visible for easy configuration
        LogMessage($"AI Provider changed to: {cmbAiProvider.SelectedItem}");
    }

    // === CAPTIONS TAB EVENT HANDLERS ===
    
    private void BtnBrowseInputVideo_Click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Filter = "Video Files|*.mp4;*.avi;*.mov;*.mkv;*.wmv;*.flv|All Files|*.*",
            Title = "Select Input Video"
        };

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            txtInputVideo.Text = dialog.FileName;
            
            // Auto-suggest output filename
            if (string.IsNullOrEmpty(txtOutputVideo.Text))
            {
                var dir = Path.GetDirectoryName(dialog.FileName) ?? "";
                var filename = Path.GetFileNameWithoutExtension(dialog.FileName);
                txtOutputVideo.Text = Path.Combine(dir, $"{filename}_captioned.mp4");
            }
        }
    }

    private void BtnBrowseOutputVideo_Click(object? sender, EventArgs e)
    {
        using var dialog = new SaveFileDialog
        {
            Filter = "MP4 Video|*.mp4|All Files|*.*",
            Title = "Save Captioned Video As",
            DefaultExt = "mp4"
        };

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            txtOutputVideo.Text = dialog.FileName;
        }
    }

    private async void BtnGenerateCaptions_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtInputVideo.Text) || !File.Exists(txtInputVideo.Text))
        {
            MessageBox.Show("Please select a valid input video file.", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (string.IsNullOrEmpty(txtOutputVideo.Text))
        {
            MessageBox.Show("Please specify an output video path.", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        btnGenerateCaptions.Enabled = false;
        progressBarCaptions.Visible = true;
        txtCaptionsLog.Clear();

        try
        {
            var progress = new Progress<string>(message =>
            {
                if (InvokeRequired)
                {
                    Invoke(() => LogCaptionMessage(message));
                }
                else
                {
                    LogCaptionMessage(message);
                }
            });

            LogCaptionMessage("=== Starting Caption Generation ===");
            LogCaptionMessage($"Input: {txtInputVideo.Text}");
            LogCaptionMessage($"Output: {txtOutputVideo.Text}");
            LogCaptionMessage("");

            // Step 1: Transcribe audio
            LogCaptionMessage("Step 1/2: Transcribing audio...");
            
            var whisperPath = _config.UseWhisperApi ? "api" : _config.WhisperPath;
            var apiKey = _config.UseWhisperApi ? _config.OpenAiApiKey : null;
            var transcriptionService = new WhisperTranscriptionService(whisperPath, _config.WhisperModel, apiKey);
            
            var segments = await transcriptionService.TranscribeAsync(txtInputVideo.Text, progress);
            
            LogCaptionMessage($"✓ Transcription complete: {segments.Count} segments");
            LogCaptionMessage("");

            // Step 2: Add captions to video
            LogCaptionMessage("Step 2/2: Adding captions to video...");
            var captionService = new VideoCaptionService(_config.FFmpegPath);
            
            var style = cmbCaptionStyle.SelectedIndex switch
            {
                1 => CaptionStyle.TikTok,
                2 => CaptionStyle.Minimal,
                _ => CaptionStyle.YouTube
            };

            await captionService.AddCaptionsToVideoAsync(
                txtInputVideo.Text,
                segments,
                txtOutputVideo.Text,
                style,
                progress);

            LogCaptionMessage("");
            LogCaptionMessage($"✓ Caption generation complete!");
            LogCaptionMessage($"Output saved to: {txtOutputVideo.Text}");

            MessageBox.Show($"Captions added successfully!\n\nOutput: {txtOutputVideo.Text}",
                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            LogCaptionMessage($"ERROR: {ex.Message}");
            MessageBox.Show($"Error generating captions:\n\n{ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnGenerateCaptions.Enabled = true;
            progressBarCaptions.Visible = false;
        }
    }

    private void LogCaptionMessage(string message)
    {
        txtCaptionsLog.AppendText($"{message}\r\n");
        txtCaptionsLog.SelectionStart = txtCaptionsLog.Text.Length;
        txtCaptionsLog.ScrollToCaret();
    }
}
