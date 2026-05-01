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
        SetupTooltips();
        SetupKeyboardShortcuts();
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

        // Enhanced validation
        if (string.IsNullOrWhiteSpace(txtTitle.Text))
        {
            MessageBox.Show("Please enter a video title.", "Validation Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtTitle.Focus();
            return;
        }

        if (txtTitle.Text.Length < 5)
        {
            MessageBox.Show("Video title is too short. Please enter at least 5 characters.", "Validation Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtTitle.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(txtTopic.Text))
        {
            MessageBox.Show("Please enter a topic/description.", "Validation Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtTopic.Focus();
            return;
        }

        if (txtTopic.Text.Length < 10)
        {
            MessageBox.Show("Topic description is too short. Please provide more details (at least 10 characters).", "Validation Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtTopic.Focus();
            return;
        }

        if (!Directory.Exists(txtOutputPath.Text))
        {
            var result = MessageBox.Show($"Output directory does not exist:\n{txtOutputPath.Text}\n\nCreate it now?",
                "Directory Not Found", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    Directory.CreateDirectory(txtOutputPath.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to create directory:\n{ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                return;
            }
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

            txtSystemStatus.AppendText("╔════════════════════════════════════════════════════════════╗\r\n");
            txtSystemStatus.AppendText("║           VOID VIDEO GENERATOR - SYSTEM STATUS            ║\r\n");
            txtSystemStatus.AppendText("╚════════════════════════════════════════════════════════════╝\r\n\r\n");
            txtSystemStatus.AppendText($"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\r\n");
            txtSystemStatus.AppendText($"OS: {Environment.OSVersion}\r\n");
            txtSystemStatus.AppendText($".NET Version: {Environment.Version}\r\n\r\n");

            txtSystemStatus.AppendText("=== CORE SERVICES STATUS ===\r\n\r\n");

            foreach (var service in status)
            {
                var statusIcon = service.Value ? "✓" : "✗";
                var statusText = service.Value ? "Available" : "Not Available";
                txtSystemStatus.AppendText($"{statusIcon} {service.Key}: {statusText}\r\n");
            }

            // Detailed AI Provider diagnostics
            txtSystemStatus.AppendText("\r\n=== AI SCRIPT GENERATOR ===\r\n\r\n");
            txtSystemStatus.AppendText($"Provider: {_config.AiProvider.ToUpper()}\r\n");
            
            if (_scriptGenerator != null)
            {
                var isAvailable = await _scriptGenerator.IsAvailableAsync();
                txtSystemStatus.AppendText($"Status: {(isAvailable ? "✓ Available" : "✗ Not Available")}\r\n");
                
                switch (_config.AiProvider.ToLower())
                {
                    case "ollama":
                        txtSystemStatus.AppendText($"URL: {_config.OllamaUrl}\r\n");
                        txtSystemStatus.AppendText($"Model: {_config.OllamaModel}\r\n");
                        if (!isAvailable)
                        {
                            txtSystemStatus.AppendText("\r\n⚠ Troubleshooting:\r\n");
                            txtSystemStatus.AppendText("  • Check if Ollama is running: ollama serve\r\n");
                            txtSystemStatus.AppendText("  • Verify model is installed: ollama list\r\n");
                            txtSystemStatus.AppendText("  • Test connection: curl http://localhost:11434/api/tags\r\n");
                        }
                        break;
                    case "openai":
                        txtSystemStatus.AppendText($"Model: {_config.OpenAiModel}\r\n");
                        txtSystemStatus.AppendText($"API Key: {(string.IsNullOrEmpty(_config.OpenAiApiKey) ? "✗ Not Set" : "✓ Configured")}\r\n");
                        if (!isAvailable)
                        {
                            txtSystemStatus.AppendText("\r\n⚠ Troubleshooting:\r\n");
                            txtSystemStatus.AppendText("  • Verify API key is correct\r\n");
                            txtSystemStatus.AppendText("  • Check internet connection\r\n");
                            txtSystemStatus.AppendText("  • Verify OpenAI service status\r\n");
                        }
                        break;
                    case "anthropic":
                    case "claude":
                        txtSystemStatus.AppendText($"Model: {_config.AnthropicModel}\r\n");
                        txtSystemStatus.AppendText($"API Key: {(string.IsNullOrEmpty(_config.AnthropicApiKey) ? "✗ Not Set" : "✓ Configured")}\r\n");
                        if (!isAvailable)
                        {
                            txtSystemStatus.AppendText("\r\n⚠ Troubleshooting:\r\n");
                            txtSystemStatus.AppendText("  • Verify API key is correct\r\n");
                            txtSystemStatus.AppendText("  • Check internet connection\r\n");
                            txtSystemStatus.AppendText("  • Verify Anthropic service status\r\n");
                        }
                        break;
                    case "gemini":
                    case "google":
                        txtSystemStatus.AppendText($"Model: {_config.GeminiModel}\r\n");
                        txtSystemStatus.AppendText($"API Key: {(string.IsNullOrEmpty(_config.GeminiApiKey) ? "✗ Not Set" : "✓ Configured")}\r\n");
                        if (!isAvailable)
                        {
                            txtSystemStatus.AppendText("\r\n⚠ Troubleshooting:\r\n");
                            txtSystemStatus.AppendText("  • Verify API key is correct\r\n");
                            txtSystemStatus.AppendText("  • Check internet connection\r\n");
                            txtSystemStatus.AppendText("  • Verify Google AI service status\r\n");
                        }
                        break;
                }
            }

            // Voice Generator diagnostics
            txtSystemStatus.AppendText("\r\n=== VOICE GENERATOR (TTS) ===\r\n\r\n");
            txtSystemStatus.AppendText($"Engine: Piper TTS\r\n");
            txtSystemStatus.AppendText($"Executable: {_config.PiperPath}\r\n");
            txtSystemStatus.AppendText($"Model: {_config.PiperModelPath}\r\n");
            txtSystemStatus.AppendText($"Executable Exists: {(File.Exists(_config.PiperPath) ? "✓ Yes" : "✗ No")}\r\n");
            txtSystemStatus.AppendText($"Model Exists: {(File.Exists(_config.PiperModelPath) ? "✓ Yes" : "✗ No")}\r\n");
            
            if (!status["Piper (TTS)"])
            {
                txtSystemStatus.AppendText("\r\n⚠ Troubleshooting:\r\n");
                txtSystemStatus.AppendText("  • Run install-piper.bat (Windows) or install-piper.sh (Linux/Mac)\r\n");
                txtSystemStatus.AppendText("  • Download from: https://github.com/rhasspy/piper/releases\r\n");
                txtSystemStatus.AppendText("  • Download voice model (.onnx + .json) from Piper voices repo\r\n");
                txtSystemStatus.AppendText("  • Update paths in Settings tab\r\n");
            }

            // Video Assembly diagnostics
            txtSystemStatus.AppendText("\r\n=== VIDEO ASSEMBLY ===\r\n\r\n");
            txtSystemStatus.AppendText($"Engine: FFmpeg\r\n");
            txtSystemStatus.AppendText($"Path: {_config.FFmpegPath}\r\n");
            txtSystemStatus.AppendText($"GPU Acceleration: {(_config.UseGpuAcceleration ? "✓ Enabled" : "✗ Disabled")}\r\n");
            if (_config.UseGpuAcceleration)
            {
                txtSystemStatus.AppendText($"GPU Encoder: {_config.GpuEncoder}\r\n");
            }
            txtSystemStatus.AppendText($"Resolution: {_config.VideoSettings.Resolution}\r\n");
            txtSystemStatus.AppendText($"Frame Rate: {_config.VideoSettings.FrameRate} fps\r\n");
            txtSystemStatus.AppendText($"Quality: {_config.VideoSettings.QualityPreset}\r\n");
            txtSystemStatus.AppendText($"Ken Burns Effect: {(_config.VideoSettings.EnableKenBurnsEffect ? "✓ Enabled" : "✗ Disabled")}\r\n");
            txtSystemStatus.AppendText($"Crossfade Transitions: {(_config.VideoSettings.EnableCrossfadeTransitions ? "✓ Enabled" : "✗ Disabled")}\r\n");
            
            if (!status["FFmpeg"])
            {
                txtSystemStatus.AppendText("\r\n⚠ Troubleshooting:\r\n");
                txtSystemStatus.AppendText("  • Download from: https://ffmpeg.org/download.html\r\n");
                txtSystemStatus.AppendText("  • Add to system PATH or specify full path in Settings\r\n");
                txtSystemStatus.AppendText("  • Test: ffmpeg -version\r\n");
            }

            // Whisper diagnostics
            txtSystemStatus.AppendText("\r\n=== CAPTION GENERATOR (WHISPER) ===\r\n\r\n");
            txtSystemStatus.AppendText($"Mode: {(_config.UseWhisperApi ? "OpenAI API" : "Local Whisper")}\r\n");
            if (_config.UseWhisperApi)
            {
                txtSystemStatus.AppendText($"API Key: {(string.IsNullOrEmpty(_config.OpenAiApiKey) ? "✗ Not Set" : "✓ Configured")}\r\n");
            }
            else
            {
                txtSystemStatus.AppendText($"Path: {_config.WhisperPath}\r\n");
                txtSystemStatus.AppendText($"Model: {_config.WhisperModel}\r\n");
            }

            // Image Service diagnostics
            txtSystemStatus.AppendText("\r\n=== IMAGE SERVICE ===\r\n\r\n");
            txtSystemStatus.AppendText($"Unsplash Integration: {(_config.UseUnsplashImages ? "✓ Enabled" : "✗ Disabled")}\r\n");
            if (_config.UseUnsplashImages)
            {
                txtSystemStatus.AppendText($"API Key: {(string.IsNullOrEmpty(_config.UnsplashApiKey) ? "✗ Not Set" : "✓ Configured")}\r\n");
            }

            // System Resources
            txtSystemStatus.AppendText("\r\n=== SYSTEM RESOURCES ===\r\n\r\n");
            txtSystemStatus.AppendText($"Working Directory: {Environment.CurrentDirectory}\r\n");
            txtSystemStatus.AppendText($"Config File: {(File.Exists(_configPath) ? "✓ Found" : "✗ Not Found")}\r\n");
            txtSystemStatus.AppendText($"Output Directory: {_config.DefaultOutputPath}\r\n");
            txtSystemStatus.AppendText($"Output Dir Exists: {(Directory.Exists(_config.DefaultOutputPath) ? "✓ Yes" : "✗ No")}\r\n");
            
            try
            {
                var drive = new DriveInfo(Path.GetPathRoot(Environment.CurrentDirectory) ?? "C:\\");
                var freeSpaceGB = drive.AvailableFreeSpace / (1024.0 * 1024.0 * 1024.0);
                txtSystemStatus.AppendText($"Free Disk Space: {freeSpaceGB:F2} GB\r\n");
                if (freeSpaceGB < 5)
                {
                    txtSystemStatus.AppendText("⚠ Warning: Low disk space (< 5 GB)\r\n");
                }
            }
            catch { }

            // Final Summary
            txtSystemStatus.AppendText("\r\n");
            txtSystemStatus.AppendText("═══════════════════════════════════════════════════════════\r\n");
            var allAvailable = status.Values.All(v => v);
            if (allAvailable)
            {
                txtSystemStatus.AppendText("✓ ALL SYSTEMS OPERATIONAL - Ready to generate videos!\r\n");
            }
            else
            {
                var missingCount = status.Values.Count(v => !v);
                txtSystemStatus.AppendText($"⚠ {missingCount} SERVICE(S) UNAVAILABLE - Please resolve issues above\r\n");
                txtSystemStatus.AppendText("\r\nQuick Fixes:\r\n");
                txtSystemStatus.AppendText("  1. Check Settings tab for correct paths\r\n");
                txtSystemStatus.AppendText("  2. Run installation scripts (install-piper.bat, etc.)\r\n");
                txtSystemStatus.AppendText("  3. See TROUBLESHOOTING.md for detailed help\r\n");
                txtSystemStatus.AppendText("  4. Use Debug Console tab to start/monitor Ollama\r\n");
            }
            txtSystemStatus.AppendText("═══════════════════════════════════════════════════════════\r\n");
        }
        catch (Exception ex)
        {
            txtSystemStatus.AppendText($"\r\n✗ ERROR DURING STATUS CHECK:\r\n");
            txtSystemStatus.AppendText($"Message: {ex.Message}\r\n");
            txtSystemStatus.AppendText($"Type: {ex.GetType().Name}\r\n");
            if (ex.InnerException != null)
            {
                txtSystemStatus.AppendText($"Inner Exception: {ex.InnerException.Message}\r\n");
            }
            txtSystemStatus.AppendText($"\r\nStack Trace:\r\n{ex.StackTrace}\r\n");
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
        _config.DarkMode = true; // Always use dark mode
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
        
        // Log which AI provider is being configured
        LogMessage($"Settings saved - AI Provider: {_config.AiProvider.ToUpper()}");
        
        try
        {
            InitializeServices();
            LogMessage("Services reinitialized successfully");
            
            MessageBox.Show(
                "Settings saved successfully!\n\n" +
                $"AI Provider: {_config.AiProvider.ToUpper()}\n" +
                "Services have been reinitialized.\n\n" +
                "Go to System Status tab to verify all services are available.",
                "Success",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            LogMessage($"Error reinitializing services: {ex.Message}");
            MessageBox.Show(
                $"Settings saved, but there was an issue initializing services:\n\n{ex.Message}\n\n" +
                "Please check the System Status tab for details.",
                "Warning",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
        
        ApplyTheme(_config.DarkMode);
    }

    private async Task TestApiConnection(string provider)
    {
        try
        {
            string apiKey = "";
            string model = "";
            IScriptGeneratorService? testService = null;

            switch (provider.ToLower())
            {
                case "openai":
                    apiKey = txtOpenAiApiKey.Text;
                    model = txtOpenAiModel.Text;
                    if (string.IsNullOrWhiteSpace(apiKey))
                    {
                        MessageBox.Show("Please enter an OpenAI API key first.", "Missing API Key",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    LogMessage($"Testing OpenAI API connection with model {model}...");
                    testService = new OpenAIScriptGenerator(apiKey, model);
                    break;

                case "anthropic":
                    apiKey = txtAnthropicApiKey.Text;
                    model = txtAnthropicModel.Text;
                    if (string.IsNullOrWhiteSpace(apiKey))
                    {
                        MessageBox.Show("Please enter an Anthropic API key first.", "Missing API Key",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    LogMessage($"Testing Anthropic API connection with model {model}...");
                    testService = new AnthropicScriptGenerator(apiKey, model);
                    break;

                case "gemini":
                    apiKey = txtGeminiApiKey.Text;
                    model = txtGeminiModel.Text;
                    if (string.IsNullOrWhiteSpace(apiKey))
                    {
                        MessageBox.Show("Please enter a Gemini API key first.", "Missing API Key",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    LogMessage($"Testing Gemini API connection with model {model}...");
                    testService = new GeminiScriptGenerator(apiKey, model);
                    break;

                default:
                    MessageBox.Show($"Unknown provider: {provider}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
            }

            // Test the connection
            var isAvailable = await testService.IsAvailableAsync();

            if (isAvailable)
            {
                LogMessage($"✓ {provider.ToUpper()} API connection successful!");
                MessageBox.Show(
                    $"✓ Connection Successful!\n\n" +
                    $"Provider: {provider.ToUpper()}\n" +
                    $"Model: {model}\n\n" +
                    "Your API key is valid and the service is available.\n" +
                    "Click 'Save Settings' to use this configuration.",
                    "Connection Test Passed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                LogMessage($"✗ {provider.ToUpper()} API connection failed");
                MessageBox.Show(
                    $"✗ Connection Failed\n\n" +
                    $"Provider: {provider.ToUpper()}\n" +
                    $"Model: {model}\n\n" +
                    "Possible issues:\n" +
                    "• Invalid or expired API key\n" +
                    "• No internet connection\n" +
                    "• Service is temporarily unavailable\n" +
                    "• Rate limit exceeded\n\n" +
                    $"See {provider.ToUpper()}_TROUBLESHOOTING.md for detailed help.",
                    "Connection Test Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            LogMessage($"Error testing {provider} API: {ex.Message}");
            MessageBox.Show(
                $"Error testing {provider.ToUpper()} API:\n\n{ex.Message}\n\n" +
                "Check the console log for more details.",
                "Test Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void ApplyTheme(bool darkMode)
    {
        var theme = darkMode ? (object)new
        {
            Background = ThemeColors.Dark.Background,
            Surface = ThemeColors.Dark.Surface,
            SurfaceVariant = ThemeColors.Dark.SurfaceVariant,
            Text = ThemeColors.Dark.Text,
            TextSecondary = ThemeColors.Dark.TextSecondary,
            Primary = ThemeColors.Dark.Primary,
            Border = ThemeColors.Dark.Border,
            InputBackground = ThemeColors.Dark.InputBackground,
            ButtonBackground = ThemeColors.Dark.ButtonBackground
        } : new
        {
            Background = ThemeColors.Light.Background,
            Surface = ThemeColors.Light.Surface,
            SurfaceVariant = ThemeColors.Light.SurfaceVariant,
            Text = ThemeColors.Light.Text,
            TextSecondary = ThemeColors.Light.TextSecondary,
            Primary = ThemeColors.Light.Primary,
            Border = ThemeColors.Light.Border,
            InputBackground = ThemeColors.Light.InputBackground,
            ButtonBackground = ThemeColors.Light.ButtonBackground
        };

        // Apply to form
        this.BackColor = ((dynamic)theme).Background;
        this.ForeColor = ((dynamic)theme).Text;

        // Apply to all controls recursively
        ApplyThemeToControls(this.Controls, theme);
    }

    private void ApplyThemeToControls(Control.ControlCollection controls, object themeObj)
    {
        dynamic theme = themeObj;
        
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
        AppendConsole("╔════════════════════════════════════════════════════════╗");
        AppendConsole("║         VOID VIDEO GENERATOR - DEBUG CONSOLE          ║");
        AppendConsole("╚════════════════════════════════════════════════════════╝");
        AppendConsole("");
        AppendConsole("Console cleared");
        AppendConsole($"Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        AppendConsole("");
        AppendConsole("Debug Console Commands:");
        AppendConsole("  • Start Ollama Server - Launch local Ollama instance");
        AppendConsole("  • Stop Ollama Server - Terminate Ollama process");
        AppendConsole("  • Clear Console - Clear this output");
        AppendConsole("");
        AppendConsole("Tip: All Ollama output will appear here in real-time");
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

    // === ENHANCEMENT METHODS ===

    private void SetupTooltips()
    {
        var toolTip = new ToolTip
        {
            AutoPopDelay = 5000,
            InitialDelay = 500,
            ReshowDelay = 100,
            ShowAlways = true
        };

        // Generate Tab Tooltips
        toolTip.SetToolTip(txtTitle, "Enter a catchy title for your video (e.g., '10 Amazing Facts About Space')");
        toolTip.SetToolTip(txtTopic, "Describe what your video should be about. Be specific for better results.");
        toolTip.SetToolTip(numDuration, "Target video length in seconds. Actual length may vary slightly.");
        toolTip.SetToolTip(txtNiche, "Your channel's focus area (e.g., Technology, Education, Entertainment)");
        toolTip.SetToolTip(txtPersona, "The personality of your narrator (e.g., Friendly expert, Enthusiastic teacher)");
        toolTip.SetToolTip(txtTone, "How the script should sound (e.g., Professional, Casual, Humorous)");
        toolTip.SetToolTip(txtAudience, "Who is watching? (e.g., Beginners, Professionals, General audience)");
        toolTip.SetToolTip(txtStyle, "Content approach (e.g., Tutorial, Storytelling, Documentary)");
        toolTip.SetToolTip(btnAddImages, "Add custom images to use in your video. Images will be shown in order.");
        toolTip.SetToolTip(btnGenerate, "Start generating your video! This may take several minutes.");

        // Captions Tab Tooltips
        toolTip.SetToolTip(txtInputVideo, "Select the video file you want to add captions to");
        toolTip.SetToolTip(txtOutputVideo, "Where to save the video with captions");
        toolTip.SetToolTip(cmbCaptionStyle, "Choose caption style: YouTube (classic), TikTok (bold), or Minimal");
        toolTip.SetToolTip(btnGenerateCaptions, "Generate and add captions to your video using AI transcription");

        // Settings Tab Tooltips
        toolTip.SetToolTip(cmbAiProvider, "Choose your AI provider: Ollama (local), OpenAI, Anthropic, or Gemini");
        toolTip.SetToolTip(txtOllamaUrl, "URL where Ollama is running (default: http://localhost:11434)");
        toolTip.SetToolTip(txtOllamaModel, "Ollama model to use (e.g., llama3.1, mistral, phi3)");
        toolTip.SetToolTip(txtPiperPath, "Path to Piper TTS executable for voice generation");
        toolTip.SetToolTip(txtFFmpegPath, "Path to FFmpeg executable for video processing");
        toolTip.SetToolTip(chkUseGpu, "Enable GPU acceleration for faster video encoding (requires compatible GPU)");
        toolTip.SetToolTip(cmbGpuEncoder, "GPU encoder to use: h264_nvenc (NVIDIA), h264_amf (AMD), h264_qsv (Intel)");
        toolTip.SetToolTip(chkEnableKenBurns, "Add smooth zoom and pan effects to images (Ken Burns effect)");
        toolTip.SetToolTip(chkEnableCrossfade, "Smooth transitions between images instead of hard cuts");
        toolTip.SetToolTip(numTransitionDuration, "How long transitions last in seconds");
        toolTip.SetToolTip(numZoomIntensity, "Zoom strength for Ken Burns effect (1.0 = no zoom, 1.5 = strong zoom)");
        toolTip.SetToolTip(btnSaveSettings, "Save all settings to config.json");

        // Status Tab Tooltips
        toolTip.SetToolTip(btnCheckStatus, "Run comprehensive system diagnostics to check if all components are working");

        // Debug Console Tooltips
        toolTip.SetToolTip(btnStartOllama, "Launch Ollama server process for local AI script generation");
        toolTip.SetToolTip(btnStopOllama, "Stop the Ollama server process");
        toolTip.SetToolTip(btnClearConsole, "Clear the debug console output");
    }

    private void SetupKeyboardShortcuts()
    {
        // Ctrl+G = Generate Video
        this.KeyPreview = true;
        this.KeyDown += (s, e) =>
        {
            if (e.Control && e.KeyCode == Keys.G && btnGenerate.Enabled)
            {
                e.Handled = true;
                BtnGenerate_Click(null, EventArgs.Empty);
            }
            // Ctrl+S = Save Settings
            else if (e.Control && e.KeyCode == Keys.S && tabControl.SelectedTab == tabSettings)
            {
                e.Handled = true;
                BtnSaveSettings_Click(null, EventArgs.Empty);
            }
            // Ctrl+T = Check Status
            else if (e.Control && e.KeyCode == Keys.T && tabControl.SelectedTab == tabStatus)
            {
                e.Handled = true;
                BtnCheckStatus_Click(null, EventArgs.Empty);
            }
            // F5 = Refresh/Check Status
            else if (e.KeyCode == Keys.F5 && tabControl.SelectedTab == tabStatus)
            {
                e.Handled = true;
                BtnCheckStatus_Click(null, EventArgs.Empty);
            }
            // Ctrl+Q = Quit
            else if (e.Control && e.KeyCode == Keys.Q)
            {
                e.Handled = true;
                this.Close();
            }
            // F1 = Switch to Generate Tab
            else if (e.KeyCode == Keys.F1)
            {
                e.Handled = true;
                tabControl.SelectedTab = tabGenerate;
            }
            // F2 = Switch to Captions Tab
            else if (e.KeyCode == Keys.F2)
            {
                e.Handled = true;
                tabControl.SelectedTab = tabCaptions;
            }
            // F3 = Switch to Settings Tab
            else if (e.KeyCode == Keys.F3)
            {
                e.Handled = true;
                tabControl.SelectedTab = tabSettings;
            }
            // F4 = Switch to Status Tab
            else if (e.KeyCode == Keys.F4)
            {
                e.Handled = true;
                tabControl.SelectedTab = tabStatus;
            }
        };

        // Update form title to show shortcuts
        this.Text = "Void Video Generator - [Ctrl+G: Generate | F1-F4: Switch Tabs | Ctrl+Q: Quit]";
    }

}
