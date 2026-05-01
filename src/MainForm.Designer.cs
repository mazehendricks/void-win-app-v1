namespace VoidVideoGenerator;

using VoidVideoGenerator.Models;
using VoidVideoGenerator.Components;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        // Main Form - Modern UI
        this.Text = "Void Video Generator - Local AI Video Creator";
        this.Size = new Size(1000, 700);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.MinimumSize = new Size(800, 600);
        this.BackColor = ModernTheme.Background;
        this.ForeColor = ModernTheme.TextPrimary;
        
        // Set application icon
        try
        {
            if (File.Exists("src/icon.ico"))
            {
                this.Icon = new Icon("src/icon.ico");
            }
            else if (File.Exists("icon.ico"))
            {
                this.Icon = new Icon("icon.ico");
            }
        }
        catch (Exception ex)
        {
            // Icon file not found or invalid, continue without it
            System.Diagnostics.Debug.WriteLine($"Failed to load icon: {ex.Message}");
        }

        // Tab Control - Modern UI
        tabControl = new TabControl();
        tabControl.Dock = DockStyle.Fill;
        tabControl.BackColor = ModernTheme.Surface;
        tabControl.ForeColor = ModernTheme.TextPrimary;
        
        // Tabs - Modern UI
        tabGenerate = new TabPage("Generate Video");
        tabGenerate.BackColor = ModernTheme.Background;
        tabGenerate.ForeColor = ModernTheme.TextPrimary;
        
        tabCaptions = new TabPage("Add Captions");
        tabCaptions.BackColor = ModernTheme.Background;
        tabCaptions.ForeColor = ModernTheme.TextPrimary;
        
        tabSettings = new TabPage("Settings");
        tabSettings.BackColor = ModernTheme.Background;
        tabSettings.ForeColor = ModernTheme.TextPrimary;
        
        tabStatus = new TabPage("System Status");
        tabStatus.BackColor = ModernTheme.Background;
        tabStatus.ForeColor = ModernTheme.TextPrimary;
        
        tabDebug = new TabPage("Debug Console");
        tabDebug.BackColor = ModernTheme.Background;
        tabDebug.ForeColor = ModernTheme.TextPrimary;
        
        tabControl.TabPages.Add(tabGenerate);
        tabControl.TabPages.Add(tabCaptions);
        tabControl.TabPages.Add(tabSettings);
        tabControl.TabPages.Add(tabStatus);
        tabControl.TabPages.Add(tabDebug);

        // === GENERATE TAB ===
        var generateScrollPanel = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
        var generatePanel = new Panel { Dock = DockStyle.Top, Padding = new Padding(20), AutoSize = true };
        generateScrollPanel.Controls.Add(generatePanel);
        tabGenerate.Controls.Add(generateScrollPanel);

        int yPos = 10;

        // Title
        var lblTitle = new Label {
            Text = "Video Title:",
            Location = new Point(10, yPos),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        txtTitle = new TextBox {
            Location = new Point(10, yPos + 25),
            Size = new Size(900, 25),
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle
        };
        generatePanel.Controls.Add(lblTitle);
        generatePanel.Controls.Add(txtTitle);
        yPos += 60;

        // Topic
        var lblTopic = new Label {
            Text = "Topic/Description:",
            Location = new Point(10, yPos),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        txtTopic = new TextBox {
            Location = new Point(10, yPos + 25),
            Size = new Size(900, 80),
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle
        };
        generatePanel.Controls.Add(lblTopic);
        generatePanel.Controls.Add(txtTopic);
        yPos += 115;

        // Duration
        var lblDuration = new Label {
            Text = "Target Duration (seconds):",
            Location = new Point(10, yPos),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        numDuration = new NumericUpDown {
            Location = new Point(200, yPos),
            Size = new Size(100, 25),
            Minimum = 15,
            Maximum = 600,
            Value = 60,
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle
        };
        generatePanel.Controls.Add(lblDuration);
        generatePanel.Controls.Add(numDuration);
        yPos += 40;

        // Channel DNA Section - Modern UI
        var grpChannelDNA = new GroupBox {
            Text = "Channel DNA",
            Location = new Point(10, yPos),
            Size = new Size(900, 180),
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.H4
        };
        
        var lblNiche = new Label {
            Text = "Niche:",
            Location = new Point(10, 25),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        txtNiche = new TextBox {
            Location = new Point(150, 22),
            Size = new Size(700, 25),
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle
        };
        
        var lblPersona = new Label {
            Text = "Host Persona:",
            Location = new Point(10, 55),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        txtPersona = new TextBox {
            Location = new Point(150, 52),
            Size = new Size(700, 25),
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle
        };
        
        var lblTone = new Label {
            Text = "Tone Guidelines:",
            Location = new Point(10, 85),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        txtTone = new TextBox {
            Location = new Point(150, 82),
            Size = new Size(700, 25),
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle
        };
        
        var lblAudience = new Label {
            Text = "Target Audience:",
            Location = new Point(10, 115),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        txtAudience = new TextBox {
            Location = new Point(150, 112),
            Size = new Size(700, 25),
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle
        };
        
        var lblStyle = new Label {
            Text = "Content Style:",
            Location = new Point(10, 145),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        txtStyle = new TextBox {
            Location = new Point(150, 142),
            Size = new Size(700, 25),
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle
        };
        
        grpChannelDNA.Controls.AddRange(new Control[] { 
            lblNiche, txtNiche, lblPersona, txtPersona, lblTone, txtTone, 
            lblAudience, txtAudience, lblStyle, txtStyle 
        });
        generatePanel.Controls.Add(grpChannelDNA);
        yPos += 190;

        // Output Path - Modern UI
        var lblOutput = new Label {
            Text = "Output Folder:",
            Location = new Point(10, yPos),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        txtOutputPath = new TextBox {
            Location = new Point(10, yPos + 25),
            Size = new Size(800, 25),
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle
        };
        btnBrowseOutput = new ModernButton {
            Text = "Browse...",
            Location = new Point(820, yPos + 23),
            Size = new Size(100, 30),
            Style = ModernButton.ButtonStyle.Secondary,
            BorderRadius = BorderRadius.SM
        };
        btnBrowseOutput.Click += BtnBrowseOutput_Click;
        generatePanel.Controls.Add(lblOutput);
        generatePanel.Controls.Add(txtOutputPath);
        generatePanel.Controls.Add(btnBrowseOutput);
        yPos += 60;

        // Visuals Section - Modern UI
        var grpVisuals = new GroupBox {
            Text = "Visuals (Images for Video)",
            Location = new Point(10, yPos),
            Size = new Size(900, 120),
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.H4
        };
        
        var lblVisualsInfo = new Label {
            Text = "Add images to use in your video. Images will be displayed in order during the video.",
            Location = new Point(10, 25),
            Size = new Size(880, 20),
            ForeColor = ModernTheme.TextSecondary
        };
        
        lstVisuals = new ListBox {
            Location = new Point(10, 50),
            Size = new Size(700, 60),
            SelectionMode = SelectionMode.MultiExtended,
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle
        };
        lstVisuals.SelectedIndexChanged += (s, e) => {
            btnRemoveImages.Enabled = lstVisuals.SelectedIndices.Count > 0;
        };
        
        btnAddImages = new ModernButton {
            Text = "Add Images",
            Location = new Point(720, 50),
            Size = new Size(100, 30),
            Style = ModernButton.ButtonStyle.Secondary,
            BorderRadius = BorderRadius.SM
        };
        btnAddImages.Click += BtnAddImages_Click;
        
        btnRemoveImages = new ModernButton {
            Text = "Remove",
            Location = new Point(720, 83),
            Size = new Size(100, 30),
            Enabled = false,
            Style = ModernButton.ButtonStyle.Outline,
            BorderRadius = BorderRadius.SM
        };
        btnRemoveImages.Click += BtnRemoveImages_Click;
        
        btnClearImages = new ModernButton {
            Text = "Clear All",
            Location = new Point(830, 50),
            Size = new Size(100, 30),
            Style = ModernButton.ButtonStyle.Outline,
            BorderRadius = BorderRadius.SM
        };
        btnClearImages.Click += BtnClearImages_Click;
        
        grpVisuals.Controls.AddRange(new Control[] {
            lblVisualsInfo, lstVisuals, btnAddImages, btnRemoveImages, btnClearImages
        });
        generatePanel.Controls.Add(grpVisuals);
        yPos += 130;

        // Generate Button - Modern UI
        btnGenerate = new ModernButton {
            Text = "🎬 Generate Video",
            Location = new Point(10, yPos),
            Size = new Size(200, 40),
            Style = ModernButton.ButtonStyle.Primary,
            BorderRadius = BorderRadius.MD
        };
        btnGenerate.Click += BtnGenerate_Click;
        generatePanel.Controls.Add(btnGenerate);

        // Progress - Modern UI
        progressBar = new ModernProgressBar {
            Location = new Point(220, yPos + 5),
            Size = new Size(690, 30),
            Visible = false,
            ShowPercentage = true,
            BarHeight = 30
        };
        generatePanel.Controls.Add(progressBar);
        yPos += 50;

        // Status Log - Modern UI
        var lblLog = new Label {
            Text = "Status Log:",
            Location = new Point(10, yPos),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        txtLog = new TextBox {
            Location = new Point(10, yPos + 25),
            Size = new Size(900, 100),
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            ReadOnly = true,
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle,
            Font = ModernFonts.Code
        };
        generatePanel.Controls.Add(lblLog);
        generatePanel.Controls.Add(txtLog);

        // === CAPTIONS TAB ===
        var captionsScrollPanel = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
        var captionsPanel = new Panel { Dock = DockStyle.Top, Padding = new Padding(20), AutoSize = true };
        captionsScrollPanel.Controls.Add(captionsPanel);
        tabCaptions.Controls.Add(captionsScrollPanel);

        yPos = 10;
        
        var lblCaptionsTitle = new Label {
            Text = "Add Captions to Existing Video",
            Location = new Point(10, yPos),
            Font = ModernFonts.H2,
            ForeColor = ModernTheme.TextPrimary,
            AutoSize = true
        };
        captionsPanel.Controls.Add(lblCaptionsTitle);
        yPos += 40;

        // Input Video - Modern UI
        var lblInputVideo = new Label {
            Text = "Input Video:",
            Location = new Point(10, yPos),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        txtInputVideo = new TextBox {
            Location = new Point(120, yPos),
            Size = new Size(500, 25),
            ReadOnly = true,
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle
        };
        btnBrowseInputVideo = new ModernButton {
            Text = "Browse...",
            Location = new Point(630, yPos - 2),
            Size = new Size(100, 30),
            Style = ModernButton.ButtonStyle.Secondary,
            BorderRadius = BorderRadius.SM
        };
        btnBrowseInputVideo.Click += BtnBrowseInputVideo_Click;
        captionsPanel.Controls.Add(lblInputVideo);
        captionsPanel.Controls.Add(txtInputVideo);
        captionsPanel.Controls.Add(btnBrowseInputVideo);
        yPos += 40;

        // Output Video - Modern UI
        var lblOutputVideo = new Label {
            Text = "Output Video:",
            Location = new Point(10, yPos),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        txtOutputVideo = new TextBox {
            Location = new Point(120, yPos),
            Size = new Size(500, 25),
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle
        };
        btnBrowseOutputVideo = new ModernButton {
            Text = "Browse...",
            Location = new Point(630, yPos - 2),
            Size = new Size(100, 30),
            Style = ModernButton.ButtonStyle.Secondary,
            BorderRadius = BorderRadius.SM
        };
        btnBrowseOutputVideo.Click += BtnBrowseOutputVideo_Click;
        captionsPanel.Controls.Add(lblOutputVideo);
        captionsPanel.Controls.Add(txtOutputVideo);
        captionsPanel.Controls.Add(btnBrowseOutputVideo);
        yPos += 40;

        // Caption Style - Modern UI
        var lblCaptionStyle = new Label {
            Text = "Caption Style:",
            Location = new Point(10, yPos),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        cmbCaptionStyle = new ComboBox {
            Location = new Point(120, yPos),
            Size = new Size(200, 25),
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            FlatStyle = FlatStyle.Flat
        };
        cmbCaptionStyle.Items.AddRange(new object[] { "YouTube", "TikTok", "Minimal", "Custom" });
        cmbCaptionStyle.SelectedIndex = 0;
        captionsPanel.Controls.Add(lblCaptionStyle);
        captionsPanel.Controls.Add(cmbCaptionStyle);
        yPos += 40;

        // Transcription Method - Modern UI
        var lblTranscriptionMethod = new Label {
            Text = "Transcription:",
            Location = new Point(10, yPos),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        cmbTranscriptionMethod = new ComboBox {
            Location = new Point(120, yPos),
            Size = new Size(200, 25),
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            FlatStyle = FlatStyle.Flat
        };
        cmbTranscriptionMethod.Items.AddRange(new object[] { "Whisper (Local)", "Whisper (OpenAI API)", "Manual SRT File" });
        cmbTranscriptionMethod.SelectedIndex = 0;
        captionsPanel.Controls.Add(lblTranscriptionMethod);
        captionsPanel.Controls.Add(cmbTranscriptionMethod);
        yPos += 40;

        // Generate Captions Button - Modern UI
        btnGenerateCaptions = new ModernButton {
            Text = "🎬 Generate Captions",
            Location = new Point(10, yPos),
            Size = new Size(220, 45),
            Style = ModernButton.ButtonStyle.Primary,
            BorderRadius = BorderRadius.MD
        };
        btnGenerateCaptions.Click += BtnGenerateCaptions_Click;
        captionsPanel.Controls.Add(btnGenerateCaptions);
        yPos += 60;

        // Progress Bar - Modern UI
        progressBarCaptions = new ModernProgressBar {
            Location = new Point(10, yPos),
            Size = new Size(900, 30),
            Visible = false,
            ShowPercentage = true,
            BarHeight = 30
        };
        captionsPanel.Controls.Add(progressBarCaptions);
        yPos += 40;

        // Log - Modern UI
        var lblCaptionsLog = new Label {
            Text = "Log:",
            Location = new Point(10, yPos),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        captionsPanel.Controls.Add(lblCaptionsLog);
        yPos += 25;

        txtCaptionsLog = new TextBox {
            Location = new Point(10, yPos),
            Size = new Size(900, 300),
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            ReadOnly = true,
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle,
            Font = ModernFonts.Code
        };
        captionsPanel.Controls.Add(txtCaptionsLog);

        // === SETTINGS TAB ===
        var settingsScrollPanel = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
        var settingsPanel = new Panel { Dock = DockStyle.Top, Padding = new Padding(20), AutoSize = true };
        settingsScrollPanel.Controls.Add(settingsPanel);
        tabSettings.Controls.Add(settingsScrollPanel);

        yPos = 10;
        
        // AI Provider Selection - Modern UI
        var grpAiProvider = new GroupBox {
            Text = "AI Script Generator",
            Location = new Point(10, yPos),
            Size = new Size(600, 280),
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.H4
        };
        
        var lblAiProvider = new Label {
            Text = "Provider:",
            Location = new Point(10, 25),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        cmbAiProvider = new ComboBox {
            Location = new Point(100, 22),
            Size = new Size(200, 25),
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            FlatStyle = FlatStyle.Flat
        };
        cmbAiProvider.Items.AddRange(new object[] { "Ollama (Local)", "OpenAI", "Anthropic Claude", "Google Gemini" });
        cmbAiProvider.SelectedIndex = 0;
        cmbAiProvider.SelectedIndexChanged += CmbAiProvider_SelectedIndexChanged;
        
        grpAiProvider.Controls.Add(lblAiProvider);
        grpAiProvider.Controls.Add(cmbAiProvider);
        
        int aiYPos = 55;
        
        // Ollama Settings - Modern UI
        var lblOllamaUrl = new Label {
            Text = "Ollama URL:",
            Location = new Point(10, aiYPos),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        txtOllamaUrl = new TextBox {
            Location = new Point(150, aiYPos),
            Size = new Size(430, 25),
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle
        };
        grpAiProvider.Controls.Add(lblOllamaUrl);
        grpAiProvider.Controls.Add(txtOllamaUrl);
        aiYPos += 30;

        var lblOllamaModel = new Label {
            Text = "Ollama Model:",
            Location = new Point(10, aiYPos),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        txtOllamaModel = new TextBox {
            Location = new Point(150, aiYPos),
            Size = new Size(430, 25),
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle
        };
        grpAiProvider.Controls.Add(lblOllamaModel);
        grpAiProvider.Controls.Add(txtOllamaModel);
        aiYPos += 35;
        
        // OpenAI Settings - Modern UI
        var lblOpenAiKey = new Label {
            Text = "OpenAI API Key:",
            Location = new Point(10, aiYPos),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        txtOpenAiApiKey = new TextBox {
            Location = new Point(150, aiYPos),
            Size = new Size(350, 25),
            UseSystemPasswordChar = true,
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle
        };
        var btnTestOpenAI = new ModernButton {
            Text = "Test",
            Location = new Point(510, aiYPos - 2),
            Size = new Size(70, 28),
            Style = ModernButton.ButtonStyle.Success,
            BorderRadius = BorderRadius.SM
        };
        btnTestOpenAI.Click += async (s, e) => await TestApiConnection("openai");
        grpAiProvider.Controls.Add(lblOpenAiKey);
        grpAiProvider.Controls.Add(txtOpenAiApiKey);
        grpAiProvider.Controls.Add(btnTestOpenAI);
        aiYPos += 30;
        
        var lblOpenAiModel = new Label {
            Text = "OpenAI Model:",
            Location = new Point(10, aiYPos),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        txtOpenAiModel = new TextBox {
            Location = new Point(150, aiYPos),
            Size = new Size(430, 25),
            Text = "gpt-4",
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle
        };
        grpAiProvider.Controls.Add(lblOpenAiModel);
        grpAiProvider.Controls.Add(txtOpenAiModel);
        aiYPos += 35;
        
        // Anthropic Settings - Modern UI
        var lblAnthropicKey = new Label {
            Text = "Anthropic API Key:",
            Location = new Point(10, aiYPos),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        txtAnthropicApiKey = new TextBox {
            Location = new Point(150, aiYPos),
            Size = new Size(350, 25),
            UseSystemPasswordChar = true,
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle
        };
        var btnTestAnthropic = new ModernButton {
            Text = "Test",
            Location = new Point(510, aiYPos - 2),
            Size = new Size(70, 28),
            Style = ModernButton.ButtonStyle.Success,
            BorderRadius = BorderRadius.SM
        };
        btnTestAnthropic.Click += async (s, e) => await TestApiConnection("anthropic");
        grpAiProvider.Controls.Add(lblAnthropicKey);
        grpAiProvider.Controls.Add(txtAnthropicApiKey);
        grpAiProvider.Controls.Add(btnTestAnthropic);
        aiYPos += 30;
        
        var lblAnthropicModel = new Label {
            Text = "Anthropic Model:",
            Location = new Point(10, aiYPos),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        txtAnthropicModel = new TextBox {
            Location = new Point(150, aiYPos),
            Size = new Size(430, 25),
            Text = "claude-3-5-sonnet-20241022",
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle
        };
        grpAiProvider.Controls.Add(lblAnthropicModel);
        grpAiProvider.Controls.Add(txtAnthropicModel);
        aiYPos += 35;
        
        // Gemini Settings - Modern UI
        var lblGeminiKey = new Label {
            Text = "Gemini API Key:",
            Location = new Point(10, aiYPos),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        txtGeminiApiKey = new TextBox {
            Location = new Point(150, aiYPos),
            Size = new Size(350, 25),
            UseSystemPasswordChar = true,
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle
        };
        var btnTestGemini = new ModernButton {
            Text = "Test",
            Location = new Point(510, aiYPos - 2),
            Size = new Size(70, 28),
            Style = ModernButton.ButtonStyle.Success,
            BorderRadius = BorderRadius.SM
        };
        btnTestGemini.Click += async (s, e) => await TestApiConnection("gemini");
        grpAiProvider.Controls.Add(lblGeminiKey);
        grpAiProvider.Controls.Add(txtGeminiApiKey);
        grpAiProvider.Controls.Add(btnTestGemini);
        aiYPos += 30;
        
        var lblGeminiModel = new Label {
            Text = "Gemini Model:",
            Location = new Point(10, aiYPos),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        txtGeminiModel = new TextBox {
            Location = new Point(150, aiYPos),
            Size = new Size(430, 25),
            Text = "gemini-1.5-pro",
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle
        };
        grpAiProvider.Controls.Add(lblGeminiModel);
        grpAiProvider.Controls.Add(txtGeminiModel);
        
        settingsPanel.Controls.Add(grpAiProvider);
        yPos += 290;

        var lblPiperPath = new Label {
            Text = "Piper Path:",
            Location = new Point(10, yPos),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        txtPiperPath = new TextBox {
            Location = new Point(200, yPos),
            Size = new Size(400, 25),
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle
        };
        settingsPanel.Controls.Add(lblPiperPath);
        settingsPanel.Controls.Add(txtPiperPath);
        yPos += 35;

        var lblPiperModel = new Label {
            Text = "Piper Model Path:",
            Location = new Point(10, yPos),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        txtPiperModel = new TextBox {
            Location = new Point(200, yPos),
            Size = new Size(400, 25),
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle
        };
        settingsPanel.Controls.Add(lblPiperModel);
        settingsPanel.Controls.Add(txtPiperModel);
        yPos += 35;

        var lblFFmpegPath = new Label {
            Text = "FFmpeg Path:",
            Location = new Point(10, yPos),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        txtFFmpegPath = new TextBox {
            Location = new Point(200, yPos),
            Size = new Size(400, 25),
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle
        };
        settingsPanel.Controls.Add(lblFFmpegPath);
        settingsPanel.Controls.Add(txtFFmpegPath);
        yPos += 35;

        // Unsplash API Section - Modern UI
        var grpUnsplash = new GroupBox {
            Text = "Unsplash Image Generation (Optional)",
            Location = new Point(10, yPos),
            Size = new Size(600, 110),
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.H4
        };
        
        chkUseUnsplash = new CheckBox {
            Text = "Enable automatic image generation from Unsplash",
            Location = new Point(10, 25),
            Size = new Size(580, 20),
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        chkUseUnsplash.CheckedChanged += (s, e) => {
            txtUnsplashApiKey.Enabled = chkUseUnsplash.Checked;
        };
        
        var lblUnsplashKey = new Label {
            Text = "API Key:",
            Location = new Point(10, 55),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        
        txtUnsplashApiKey = new TextBox {
            Location = new Point(80, 52),
            Size = new Size(500, 25),
            Enabled = false,
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle
        };
        
        var lblUnsplashInfo = new Label {
            Text = "Get your free API key at: https://unsplash.com/developers",
            Location = new Point(10, 82),
            Size = new Size(580, 20),
            ForeColor = ModernTheme.TextSecondary
        };
        
        grpUnsplash.Controls.AddRange(new Control[] {
            chkUseUnsplash, lblUnsplashKey, txtUnsplashApiKey, lblUnsplashInfo
        });
        settingsPanel.Controls.Add(grpUnsplash);
        yPos += 120;

        // GPU Acceleration Section - Modern UI
        var grpGpuSettings = new GroupBox {
            Text = "Video Encoding Settings",
            Location = new Point(10, yPos),
            Size = new Size(600, 100),
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.H4
        };
        
        chkUseGpu = new CheckBox {
            Text = "Enable GPU Acceleration (faster encoding, requires compatible GPU)",
            Location = new Point(10, 25),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        chkUseGpu.CheckedChanged += (s, e) => {
            cmbGpuEncoder.Enabled = chkUseGpu.Checked;
        };
        
        var lblGpuEncoder = new Label {
            Text = "GPU Encoder:",
            Location = new Point(10, 55),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        
        cmbGpuEncoder = new ComboBox {
            Location = new Point(120, 52),
            Size = new Size(200, 25),
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            FlatStyle = FlatStyle.Flat
        };
        cmbGpuEncoder.Items.AddRange(new object[] { "auto", "nvidia", "amd", "intel", "apple" });
        cmbGpuEncoder.SelectedIndex = 0;
        cmbGpuEncoder.Enabled = false;
        
        grpGpuSettings.Controls.Add(chkUseGpu);
        grpGpuSettings.Controls.Add(lblGpuEncoder);
        grpGpuSettings.Controls.Add(cmbGpuEncoder);
        settingsPanel.Controls.Add(grpGpuSettings);
        yPos += 110;

        // Video Output Settings Section - Modern UI
        var grpVideoOutput = new GroupBox {
            Text = "Video Output Settings",
            Location = new Point(10, yPos),
            Size = new Size(600, 180),
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.H4
        };

        var lblResolution = new Label {
            Text = "Resolution:",
            Location = new Point(10, 25),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        cmbResolution = new ComboBox {
            Location = new Point(120, 22),
            Size = new Size(150, 25),
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            FlatStyle = FlatStyle.Flat
        };
        cmbResolution.Items.AddRange(new object[] { "720p", "1080p", "1440p", "4K" });
        cmbResolution.SelectedIndex = 1; // 1080p default

        var lblQuality = new Label {
            Text = "Quality Preset:",
            Location = new Point(300, 25),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        cmbQuality = new ComboBox {
            Location = new Point(410, 22),
            Size = new Size(150, 25),
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            FlatStyle = FlatStyle.Flat
        };
        cmbQuality.Items.AddRange(new object[] { "Low", "Medium", "High", "Ultra" });
        cmbQuality.SelectedIndex = 1; // Medium default

        var lblFrameRate = new Label {
            Text = "Frame Rate:",
            Location = new Point(10, 60),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        cmbFrameRate = new ComboBox {
            Location = new Point(120, 57),
            Size = new Size(150, 25),
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            FlatStyle = FlatStyle.Flat
        };
        cmbFrameRate.Items.AddRange(new object[] { "24 fps", "30 fps", "60 fps" });
        cmbFrameRate.SelectedIndex = 1; // 30 fps default

        var lblVideoBitrate = new Label {
            Text = "Video Bitrate:",
            Location = new Point(300, 60),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        numVideoBitrate = new NumericUpDown {
            Location = new Point(410, 57),
            Size = new Size(100, 25),
            Minimum = 1000,
            Maximum = 50000,
            Value = 5000,
            Increment = 500,
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle
        };
        var lblVideoBitrateUnit = new Label {
            Text = "kbps",
            Location = new Point(515, 60),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };

        var lblAudioBitrate = new Label {
            Text = "Audio Bitrate:",
            Location = new Point(10, 95),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        numAudioBitrate = new NumericUpDown {
            Location = new Point(120, 92),
            Size = new Size(100, 25),
            Minimum = 64,
            Maximum = 320,
            Value = 192,
            Increment = 32,
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle
        };
        var lblAudioBitrateUnit = new Label {
            Text = "kbps",
            Location = new Point(225, 95),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };

        var lblAudioChannels = new Label {
            Text = "Audio Channels:",
            Location = new Point(300, 95),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        cmbAudioChannels = new ComboBox {
            Location = new Point(410, 92),
            Size = new Size(150, 25),
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            FlatStyle = FlatStyle.Flat
        };
        cmbAudioChannels.Items.AddRange(new object[] { "Mono", "Stereo" });
        cmbAudioChannels.SelectedIndex = 1; // Stereo default

        var lblVideoInfo = new Label {
            Text = "Higher settings = better quality but larger file sizes",
            Location = new Point(10, 130),
            Size = new Size(580, 40),
            ForeColor = ModernTheme.Success,
            Font = ModernFonts.Small
        };

        grpVideoOutput.Controls.AddRange(new Control[] {
            lblResolution, cmbResolution, lblQuality, cmbQuality,
            lblFrameRate, cmbFrameRate, lblVideoBitrate, numVideoBitrate, lblVideoBitrateUnit,
            lblAudioBitrate, numAudioBitrate, lblAudioBitrateUnit,
            lblAudioChannels, cmbAudioChannels, lblVideoInfo
        });
        settingsPanel.Controls.Add(grpVideoOutput);
        yPos += 190;

        // GPU info label - Modern UI
        var lblGpuInfo = new Label {
            Text = "Note: GPU acceleration requires FFmpeg with GPU support and compatible hardware.\n" +
                   "CPU encoding is more compatible but slower. GPU encoding is 3-10x faster.",
            Location = new Point(10, yPos),
            Size = new Size(600, 40),
            ForeColor = ModernTheme.TextSecondary,
            Font = ModernFonts.Small
        };
        settingsPanel.Controls.Add(lblGpuInfo);
        yPos += 50;

        // Animation Settings Section - Modern UI
        var grpAnimationSettings = new GroupBox {
            Text = "Video Animation Settings",
            Location = new Point(10, yPos),
            Size = new Size(600, 180),
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.H4
        };
        
        int animYPos = 25;
        
        chkEnableKenBurns = new CheckBox {
            Text = "Enable Ken Burns Effect (zoom/pan on images)",
            Location = new Point(10, animYPos),
            Size = new Size(580, 20),
            Checked = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        grpAnimationSettings.Controls.Add(chkEnableKenBurns);
        animYPos += 30;
        
        chkEnableCrossfade = new CheckBox {
            Text = "Enable Crossfade Transitions between images",
            Location = new Point(10, animYPos),
            Size = new Size(580, 20),
            Checked = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        grpAnimationSettings.Controls.Add(chkEnableCrossfade);
        animYPos += 30;
        
        var lblTransitionDuration = new Label {
            Text = "Transition Duration (seconds):",
            Location = new Point(10, animYPos + 3),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        numTransitionDuration = new NumericUpDown {
            Location = new Point(220, animYPos),
            Size = new Size(80, 25),
            Minimum = 0.1m,
            Maximum = 5.0m,
            DecimalPlaces = 1,
            Increment = 0.1m,
            Value = 1.0m,
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle
        };
        grpAnimationSettings.Controls.Add(lblTransitionDuration);
        grpAnimationSettings.Controls.Add(numTransitionDuration);
        animYPos += 35;
        
        var lblZoomIntensity = new Label {
            Text = "Zoom Intensity (1.0 = no zoom, 1.5 = 50% zoom):",
            Location = new Point(10, animYPos + 3),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        numZoomIntensity = new NumericUpDown {
            Location = new Point(320, animYPos),
            Size = new Size(80, 25),
            Minimum = 1.0m,
            Maximum = 2.0m,
            DecimalPlaces = 1,
            Increment = 0.1m,
            Value = 1.2m,
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle
        };
        grpAnimationSettings.Controls.Add(lblZoomIntensity);
        grpAnimationSettings.Controls.Add(numZoomIntensity);
        
        settingsPanel.Controls.Add(grpAnimationSettings);
        yPos += 190;

        // Whisper Settings Section - Modern UI
        var grpWhisperSettings = new GroupBox {
            Text = "Whisper (Caption) Settings",
            Location = new Point(10, yPos),
            Size = new Size(600, 140),
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.H4
        };
        
        int whisperYPos = 25;
        
        var lblWhisperPath = new Label {
            Text = "Whisper Command:",
            Location = new Point(10, whisperYPos + 3),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        txtWhisperPath = new TextBox {
            Location = new Point(150, whisperYPos),
            Size = new Size(430, 25),
            Text = "whisper",
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle
        };
        grpWhisperSettings.Controls.Add(lblWhisperPath);
        grpWhisperSettings.Controls.Add(txtWhisperPath);
        whisperYPos += 35;
        
        var lblWhisperModel = new Label {
            Text = "Whisper Model:",
            Location = new Point(10, whisperYPos + 3),
            AutoSize = true,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        cmbWhisperModel = new ComboBox {
            Location = new Point(150, whisperYPos),
            Size = new Size(200, 25),
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            FlatStyle = FlatStyle.Flat
        };
        cmbWhisperModel.Items.AddRange(new object[] { "tiny", "base", "small", "medium", "large" });
        cmbWhisperModel.SelectedIndex = 1; // base
        grpWhisperSettings.Controls.Add(lblWhisperModel);
        grpWhisperSettings.Controls.Add(cmbWhisperModel);
        whisperYPos += 35;
        
        chkUseWhisperApi = new CheckBox {
            Text = "Use OpenAI Whisper API (requires OpenAI API key above)",
            Location = new Point(10, whisperYPos),
            Size = new Size(580, 20),
            Checked = false,
            ForeColor = ModernTheme.TextPrimary,
            Font = ModernFonts.Body
        };
        grpWhisperSettings.Controls.Add(chkUseWhisperApi);
        
        settingsPanel.Controls.Add(grpWhisperSettings);
        yPos += 150;

        // Save Settings Button - Modern UI
        btnSaveSettings = new ModernButton {
            Text = "💾 Save All Settings",
            Location = new Point(10, yPos),
            Size = new Size(200, 40),
            Style = ModernButton.ButtonStyle.Success,
            BorderRadius = BorderRadius.MD
        };
        btnSaveSettings.Click += BtnSaveSettings_Click;
        settingsPanel.Controls.Add(btnSaveSettings);

        // === STATUS TAB ===
        var statusPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
        tabStatus.Controls.Add(statusPanel);

        var lblStatusTitle = new Label {
            Text = "System Status - Check if all required tools are installed",
            Location = new Point(10, 10),
            AutoSize = true,
            Font = ModernFonts.H3,
            ForeColor = ModernTheme.TextPrimary
        };
        statusPanel.Controls.Add(lblStatusTitle);

        txtSystemStatus = new TextBox {
            Location = new Point(10, 40),
            Size = new Size(900, 400),
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            ReadOnly = true,
            BackColor = ModernTheme.Surface,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle,
            Font = ModernFonts.Code
        };
        statusPanel.Controls.Add(txtSystemStatus);

        btnCheckStatus = new ModernButton {
            Text = "🔍 Check System Status",
            Location = new Point(10, 450),
            Size = new Size(200, 35),
            Style = ModernButton.ButtonStyle.Primary,
            BorderRadius = BorderRadius.MD
        };
        btnCheckStatus.Click += BtnCheckStatus_Click;
        statusPanel.Controls.Add(btnCheckStatus);

        // === DEBUG CONSOLE TAB ===
        var debugPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
        tabDebug.Controls.Add(debugPanel);

        var lblDebugTitle = new Label {
            Text = "Ollama Server Console - Live Output",
            Location = new Point(10, 10),
            AutoSize = true,
            Font = ModernFonts.H3,
            ForeColor = ModernTheme.TextPrimary
        };
        debugPanel.Controls.Add(lblDebugTitle);

        txtOllamaConsole = new TextBox {
            Location = new Point(10, 40),
            Size = new Size(900, 400),
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            ReadOnly = true,
            BackColor = Color.Black,
            ForeColor = Color.LimeGreen,
            BorderStyle = BorderStyle.FixedSingle,
            Font = ModernFonts.Code
        };
        debugPanel.Controls.Add(txtOllamaConsole);

        var buttonPanel = new FlowLayoutPanel {
            Location = new Point(10, 450),
            Size = new Size(900, 40),
            FlowDirection = FlowDirection.LeftToRight
        };

        btnStartOllama = new ModernButton {
            Text = "▶️ Start Ollama Server",
            Size = new Size(180, 40),
            Margin = new Padding(0, 0, 10, 0),
            Style = ModernButton.ButtonStyle.Success,
            BorderRadius = BorderRadius.SM
        };
        btnStartOllama.Click += BtnStartOllama_Click;
        buttonPanel.Controls.Add(btnStartOllama);

        btnStopOllama = new ModernButton {
            Text = "⏹️ Stop Ollama Server",
            Size = new Size(180, 40),
            Enabled = false,
            Margin = new Padding(0, 0, 10, 0),
            Style = ModernButton.ButtonStyle.Danger,
            BorderRadius = BorderRadius.SM
        };
        btnStopOllama.Click += BtnStopOllama_Click;
        buttonPanel.Controls.Add(btnStopOllama);

        btnClearConsole = new ModernButton {
            Text = "🗑️ Clear Console",
            Size = new Size(150, 40),
            Margin = new Padding(0, 0, 10, 0),
            Style = ModernButton.ButtonStyle.Outline,
            BorderRadius = BorderRadius.SM
        };
        btnClearConsole.Click += BtnClearConsole_Click;
        buttonPanel.Controls.Add(btnClearConsole);

        debugPanel.Controls.Add(buttonPanel);

        // Add tab control to form
        this.Controls.Add(tabControl);
    }

    #endregion

    private TabControl tabControl;
    private TabPage tabGenerate;
    private TabPage tabCaptions;
    private TabPage tabSettings;
    private TabPage tabStatus;
    private TabPage tabDebug;
    
    // Generate Tab Controls
    private TextBox txtTitle;
    private TextBox txtTopic;
    private NumericUpDown numDuration;
    private TextBox txtNiche;
    private TextBox txtPersona;
    private TextBox txtTone;
    private TextBox txtAudience;
    private TextBox txtStyle;
    private TextBox txtOutputPath;
    private Button btnBrowseOutput;
    private ListBox lstVisuals;
    private Button btnAddImages;
    private Button btnRemoveImages;
    private Button btnClearImages;
    private Button btnGenerate;
    private ModernProgressBar progressBar;
    private TextBox txtLog;
    
    // Settings Tab Controls
    private ComboBox cmbAiProvider;
    private TextBox txtOllamaUrl;
    private TextBox txtOllamaModel;
    private TextBox txtOpenAiApiKey;
    private TextBox txtOpenAiModel;
    private TextBox txtAnthropicApiKey;
    private TextBox txtAnthropicModel;
    private TextBox txtGeminiApiKey;
    private TextBox txtGeminiModel;
    private TextBox txtPiperPath;
    private TextBox txtPiperModel;
    private TextBox txtFFmpegPath;
    private CheckBox chkUseUnsplash;
    private TextBox txtUnsplashApiKey;
    private CheckBox chkUseGpu;
    private ComboBox cmbGpuEncoder;
    private ComboBox cmbResolution;
    private ComboBox cmbQuality;
    private ComboBox cmbFrameRate;
    private NumericUpDown numVideoBitrate;
    private NumericUpDown numAudioBitrate;
    private ComboBox cmbAudioChannels;
    private CheckBox chkEnableKenBurns;
    private CheckBox chkEnableCrossfade;
    private NumericUpDown numTransitionDuration;
    private NumericUpDown numZoomIntensity;
    private TextBox txtWhisperPath;
    private ComboBox cmbWhisperModel;
    private CheckBox chkUseWhisperApi;
    private Button btnSaveSettings;
    
    // Captions Tab Controls
    private TextBox txtInputVideo;
    private Button btnBrowseInputVideo;
    private TextBox txtOutputVideo;
    private Button btnBrowseOutputVideo;
    private ComboBox cmbCaptionStyle;
    private ComboBox cmbTranscriptionMethod;
    private Button btnGenerateCaptions;
    private ModernProgressBar progressBarCaptions;
    private TextBox txtCaptionsLog;
    
    // Status Tab Controls
    private TextBox txtSystemStatus;
    private Button btnCheckStatus;
    
    // Debug Console Tab Controls
    private TextBox txtOllamaConsole;
    private Button btnStartOllama;
    private Button btnStopOllama;
    private Button btnClearConsole;
}
