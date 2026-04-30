namespace VoidVideoGenerator;

using System.Text.Json;
using VoidVideoGenerator.Models;
using VoidVideoGenerator.Services;

public partial class MainForm : Form
{
    private AppConfig _config;
    private VideoGenerationPipeline? _pipeline;
    private readonly string _configPath = "config.json";

    public MainForm()
    {
        InitializeComponent();
        LoadConfiguration();
        InitializeServices();
        PopulateFormFromConfig();
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

    private void InitializeServices()
    {
        try
        {
            var scriptGenerator = new OllamaScriptGenerator(_config.OllamaUrl, _config.OllamaModel);
            var voiceGenerator = new PiperTTSService(_config.PiperPath, _config.PiperModelPath);
            var videoAssembly = new FFmpegVideoAssembly(_config.FFmpegPath);

            _pipeline = new VideoGenerationPipeline(scriptGenerator, voiceGenerator, videoAssembly);
            LogMessage("Services initialized");
        }
        catch (Exception ex)
        {
            LogMessage($"Error initializing services: {ex.Message}");
        }
    }

    private void PopulateFormFromConfig()
    {
        // Settings tab
        txtOllamaUrl.Text = _config.OllamaUrl;
        txtOllamaModel.Text = _config.OllamaModel;
        txtPiperPath.Text = _config.PiperPath;
        txtPiperModel.Text = _config.PiperModelPath;
        txtFFmpegPath.Text = _config.FFmpegPath;

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

            var progress = new Progress<string>(message =>
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

            var videoPath = await _pipeline.GenerateVideoAsync(request, progress);

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
                txtSystemStatus.AppendText("  2. Install and run: ollama pull llama3.1\r\n\r\n");
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
        _config.OllamaUrl = txtOllamaUrl.Text;
        _config.OllamaModel = txtOllamaModel.Text;
        _config.PiperPath = txtPiperPath.Text;
        _config.PiperModelPath = txtPiperModel.Text;
        _config.FFmpegPath = txtFFmpegPath.Text;
        _config.DefaultOutputPath = txtOutputPath.Text;

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

        MessageBox.Show("Settings saved successfully!", "Success", 
            MessageBoxButtons.OK, MessageBoxIcon.Information);
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
}
