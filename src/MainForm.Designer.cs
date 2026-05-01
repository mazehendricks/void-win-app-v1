namespace VoidVideoGenerator;

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
        // Main Form
        this.Text = "Void Video Generator - Local AI Video Creator";
        this.Size = new Size(1000, 700);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.MinimumSize = new Size(800, 600);
        
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

        // Tab Control
        tabControl = new TabControl();
        tabControl.Dock = DockStyle.Fill;
        
        // Tabs
        tabGenerate = new TabPage("Generate Video");
        tabCaptions = new TabPage("Add Captions");
        tabSettings = new TabPage("Settings");
        tabStatus = new TabPage("System Status");
        tabDebug = new TabPage("Debug Console");
        
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
        var lblTitle = new Label { Text = "Video Title:", Location = new Point(10, yPos), AutoSize = true };
        txtTitle = new TextBox { Location = new Point(10, yPos + 25), Size = new Size(900, 25) };
        generatePanel.Controls.Add(lblTitle);
        generatePanel.Controls.Add(txtTitle);
        yPos += 60;

        // Topic
        var lblTopic = new Label { Text = "Topic/Description:", Location = new Point(10, yPos), AutoSize = true };
        txtTopic = new TextBox { 
            Location = new Point(10, yPos + 25), 
            Size = new Size(900, 80),
            Multiline = true,
            ScrollBars = ScrollBars.Vertical
        };
        generatePanel.Controls.Add(lblTopic);
        generatePanel.Controls.Add(txtTopic);
        yPos += 115;

        // Duration
        var lblDuration = new Label { Text = "Target Duration (seconds):", Location = new Point(10, yPos), AutoSize = true };
        numDuration = new NumericUpDown { 
            Location = new Point(200, yPos), 
            Size = new Size(100, 25),
            Minimum = 15,
            Maximum = 600,
            Value = 60
        };
        generatePanel.Controls.Add(lblDuration);
        generatePanel.Controls.Add(numDuration);
        yPos += 40;

        // Channel DNA Section
        var grpChannelDNA = new GroupBox { 
            Text = "Channel DNA", 
            Location = new Point(10, yPos), 
            Size = new Size(900, 180) 
        };
        
        var lblNiche = new Label { Text = "Niche:", Location = new Point(10, 25), AutoSize = true };
        txtNiche = new TextBox { Location = new Point(150, 22), Size = new Size(700, 25) };
        
        var lblPersona = new Label { Text = "Host Persona:", Location = new Point(10, 55), AutoSize = true };
        txtPersona = new TextBox { Location = new Point(150, 52), Size = new Size(700, 25) };
        
        var lblTone = new Label { Text = "Tone Guidelines:", Location = new Point(10, 85), AutoSize = true };
        txtTone = new TextBox { Location = new Point(150, 82), Size = new Size(700, 25) };
        
        var lblAudience = new Label { Text = "Target Audience:", Location = new Point(10, 115), AutoSize = true };
        txtAudience = new TextBox { Location = new Point(150, 112), Size = new Size(700, 25) };
        
        var lblStyle = new Label { Text = "Content Style:", Location = new Point(10, 145), AutoSize = true };
        txtStyle = new TextBox { Location = new Point(150, 142), Size = new Size(700, 25) };
        
        grpChannelDNA.Controls.AddRange(new Control[] { 
            lblNiche, txtNiche, lblPersona, txtPersona, lblTone, txtTone, 
            lblAudience, txtAudience, lblStyle, txtStyle 
        });
        generatePanel.Controls.Add(grpChannelDNA);
        yPos += 190;

        // Output Path
        var lblOutput = new Label { Text = "Output Folder:", Location = new Point(10, yPos), AutoSize = true };
        txtOutputPath = new TextBox { Location = new Point(10, yPos + 25), Size = new Size(800, 25) };
        btnBrowseOutput = new Button { 
            Text = "Browse...", 
            Location = new Point(820, yPos + 23), 
            Size = new Size(90, 27) 
        };
        btnBrowseOutput.Click += BtnBrowseOutput_Click;
        generatePanel.Controls.Add(lblOutput);
        generatePanel.Controls.Add(txtOutputPath);
        generatePanel.Controls.Add(btnBrowseOutput);
        yPos += 60;

        // Visuals Section
        var grpVisuals = new GroupBox {
            Text = "Visuals (Images for Video)",
            Location = new Point(10, yPos),
            Size = new Size(900, 120)
        };
        
        var lblVisualsInfo = new Label {
            Text = "Add images to use in your video. Images will be displayed in order during the video.",
            Location = new Point(10, 25),
            Size = new Size(880, 20),
            ForeColor = Color.DarkBlue
        };
        
        lstVisuals = new ListBox {
            Location = new Point(10, 50),
            Size = new Size(700, 60),
            SelectionMode = SelectionMode.MultiExtended
        };
        lstVisuals.SelectedIndexChanged += (s, e) => {
            btnRemoveImages.Enabled = lstVisuals.SelectedIndices.Count > 0;
        };
        
        btnAddImages = new Button {
            Text = "Add Images",
            Location = new Point(720, 50),
            Size = new Size(85, 27)
        };
        btnAddImages.Click += BtnAddImages_Click;
        
        btnRemoveImages = new Button {
            Text = "Remove",
            Location = new Point(720, 83),
            Size = new Size(85, 27),
            Enabled = false
        };
        btnRemoveImages.Click += BtnRemoveImages_Click;
        
        btnClearImages = new Button {
            Text = "Clear All",
            Location = new Point(810, 50),
            Size = new Size(80, 27)
        };
        btnClearImages.Click += BtnClearImages_Click;
        
        grpVisuals.Controls.AddRange(new Control[] {
            lblVisualsInfo, lstVisuals, btnAddImages, btnRemoveImages, btnClearImages
        });
        generatePanel.Controls.Add(grpVisuals);
        yPos += 130;

        // Generate Button
        btnGenerate = new Button {
            Text = "Generate Video",
            Location = new Point(10, yPos),
            Size = new Size(200, 40),
            Font = new Font("Segoe UI", 12, FontStyle.Bold)
        };
        btnGenerate.Click += BtnGenerate_Click;
        generatePanel.Controls.Add(btnGenerate);

        // Progress
        progressBar = new ProgressBar {
            Location = new Point(220, yPos + 5),
            Size = new Size(690, 30),
            Style = ProgressBarStyle.Marquee,
            Visible = false
        };
        generatePanel.Controls.Add(progressBar);
        yPos += 50;

        // Status Log
        var lblLog = new Label { Text = "Status Log:", Location = new Point(10, yPos), AutoSize = true };
        txtLog = new TextBox { 
            Location = new Point(10, yPos + 25), 
            Size = new Size(900, 100),
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            ReadOnly = true,
            BackColor = Color.White
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
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            AutoSize = true
        };
        captionsPanel.Controls.Add(lblCaptionsTitle);
        yPos += 40;

        // Input Video
        var lblInputVideo = new Label { Text = "Input Video:", Location = new Point(10, yPos), AutoSize = true };
        txtInputVideo = new TextBox { Location = new Point(120, yPos), Size = new Size(500, 25), ReadOnly = true };
        btnBrowseInputVideo = new Button { Text = "Browse...", Location = new Point(630, yPos - 2), Size = new Size(100, 30) };
        btnBrowseInputVideo.Click += BtnBrowseInputVideo_Click;
        captionsPanel.Controls.Add(lblInputVideo);
        captionsPanel.Controls.Add(txtInputVideo);
        captionsPanel.Controls.Add(btnBrowseInputVideo);
        yPos += 40;

        // Output Video
        var lblOutputVideo = new Label { Text = "Output Video:", Location = new Point(10, yPos), AutoSize = true };
        txtOutputVideo = new TextBox { Location = new Point(120, yPos), Size = new Size(500, 25) };
        btnBrowseOutputVideo = new Button { Text = "Browse...", Location = new Point(630, yPos - 2), Size = new Size(100, 30) };
        btnBrowseOutputVideo.Click += BtnBrowseOutputVideo_Click;
        captionsPanel.Controls.Add(lblOutputVideo);
        captionsPanel.Controls.Add(txtOutputVideo);
        captionsPanel.Controls.Add(btnBrowseOutputVideo);
        yPos += 40;

        // Caption Style
        var lblCaptionStyle = new Label { Text = "Caption Style:", Location = new Point(10, yPos), AutoSize = true };
        cmbCaptionStyle = new ComboBox {
            Location = new Point(120, yPos),
            Size = new Size(200, 25),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbCaptionStyle.Items.AddRange(new object[] { "YouTube", "TikTok", "Minimal", "Custom" });
        cmbCaptionStyle.SelectedIndex = 0;
        captionsPanel.Controls.Add(lblCaptionStyle);
        captionsPanel.Controls.Add(cmbCaptionStyle);
        yPos += 40;

        // Transcription Method
        var lblTranscriptionMethod = new Label { Text = "Transcription:", Location = new Point(10, yPos), AutoSize = true };
        cmbTranscriptionMethod = new ComboBox {
            Location = new Point(120, yPos),
            Size = new Size(200, 25),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbTranscriptionMethod.Items.AddRange(new object[] { "Whisper (Local)", "Whisper (OpenAI API)", "Manual SRT File" });
        cmbTranscriptionMethod.SelectedIndex = 0;
        captionsPanel.Controls.Add(lblTranscriptionMethod);
        captionsPanel.Controls.Add(cmbTranscriptionMethod);
        yPos += 40;

        // Generate Captions Button
        btnGenerateCaptions = new Button {
            Text = "Generate Captions",
            Location = new Point(10, yPos),
            Size = new Size(200, 40),
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnGenerateCaptions.Click += BtnGenerateCaptions_Click;
        captionsPanel.Controls.Add(btnGenerateCaptions);
        yPos += 60;

        // Progress Bar
        progressBarCaptions = new ProgressBar {
            Location = new Point(10, yPos),
            Size = new Size(900, 25),
            Style = ProgressBarStyle.Marquee,
            Visible = false
        };
        captionsPanel.Controls.Add(progressBarCaptions);
        yPos += 35;

        // Log
        var lblCaptionsLog = new Label { Text = "Log:", Location = new Point(10, yPos), AutoSize = true };
        captionsPanel.Controls.Add(lblCaptionsLog);
        yPos += 25;

        txtCaptionsLog = new TextBox {
            Location = new Point(10, yPos),
            Size = new Size(900, 300),
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            ReadOnly = true,
            Font = new Font("Consola", 9)
        };
        captionsPanel.Controls.Add(txtCaptionsLog);

        // === SETTINGS TAB ===
        var settingsScrollPanel = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
        var settingsPanel = new Panel { Dock = DockStyle.Top, Padding = new Padding(20), AutoSize = true };
        settingsScrollPanel.Controls.Add(settingsPanel);
        tabSettings.Controls.Add(settingsScrollPanel);

        yPos = 10;
        
        // AI Provider Selection
        var grpAiProvider = new GroupBox {
            Text = "AI Script Generator",
            Location = new Point(10, yPos),
            Size = new Size(600, 280)
        };
        
        var lblAiProvider = new Label { Text = "Provider:", Location = new Point(10, 25), AutoSize = true };
        cmbAiProvider = new ComboBox {
            Location = new Point(100, 22),
            Size = new Size(200, 25),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbAiProvider.Items.AddRange(new object[] { "Ollama (Local)", "OpenAI", "Anthropic Claude", "Google Gemini" });
        cmbAiProvider.SelectedIndex = 0;
        cmbAiProvider.SelectedIndexChanged += CmbAiProvider_SelectedIndexChanged;
        
        grpAiProvider.Controls.Add(lblAiProvider);
        grpAiProvider.Controls.Add(cmbAiProvider);
        
        int aiYPos = 55;
        
        // Ollama Settings
        var lblOllamaUrl = new Label { Text = "Ollama URL:", Location = new Point(10, aiYPos), AutoSize = true };
        txtOllamaUrl = new TextBox { Location = new Point(150, aiYPos), Size = new Size(430, 25) };
        grpAiProvider.Controls.Add(lblOllamaUrl);
        grpAiProvider.Controls.Add(txtOllamaUrl);
        aiYPos += 30;

        var lblOllamaModel = new Label { Text = "Ollama Model:", Location = new Point(10, aiYPos), AutoSize = true };
        txtOllamaModel = new TextBox { Location = new Point(150, aiYPos), Size = new Size(430, 25) };
        grpAiProvider.Controls.Add(lblOllamaModel);
        grpAiProvider.Controls.Add(txtOllamaModel);
        aiYPos += 35;
        
        // OpenAI Settings
        var lblOpenAiKey = new Label { Text = "OpenAI API Key:", Location = new Point(10, aiYPos), AutoSize = true };
        txtOpenAiApiKey = new TextBox { Location = new Point(150, aiYPos), Size = new Size(430, 25), UseSystemPasswordChar = true };
        grpAiProvider.Controls.Add(lblOpenAiKey);
        grpAiProvider.Controls.Add(txtOpenAiApiKey);
        aiYPos += 30;
        
        var lblOpenAiModel = new Label { Text = "OpenAI Model:", Location = new Point(10, aiYPos), AutoSize = true };
        txtOpenAiModel = new TextBox { Location = new Point(150, aiYPos), Size = new Size(430, 25), Text = "gpt-4" };
        grpAiProvider.Controls.Add(lblOpenAiModel);
        grpAiProvider.Controls.Add(txtOpenAiModel);
        aiYPos += 35;
        
        // Anthropic Settings
        var lblAnthropicKey = new Label { Text = "Anthropic API Key:", Location = new Point(10, aiYPos), AutoSize = true };
        txtAnthropicApiKey = new TextBox { Location = new Point(150, aiYPos), Size = new Size(430, 25), UseSystemPasswordChar = true };
        grpAiProvider.Controls.Add(lblAnthropicKey);
        grpAiProvider.Controls.Add(txtAnthropicApiKey);
        aiYPos += 30;
        
        var lblAnthropicModel = new Label { Text = "Anthropic Model:", Location = new Point(10, aiYPos), AutoSize = true };
        txtAnthropicModel = new TextBox { Location = new Point(150, aiYPos), Size = new Size(430, 25), Text = "claude-3-5-sonnet-20241022" };
        grpAiProvider.Controls.Add(lblAnthropicModel);
        grpAiProvider.Controls.Add(txtAnthropicModel);
        aiYPos += 35;
        
        // Gemini Settings
        var lblGeminiKey = new Label { Text = "Gemini API Key:", Location = new Point(10, aiYPos), AutoSize = true };
        txtGeminiApiKey = new TextBox { Location = new Point(150, aiYPos), Size = new Size(430, 25), UseSystemPasswordChar = true };
        grpAiProvider.Controls.Add(lblGeminiKey);
        grpAiProvider.Controls.Add(txtGeminiApiKey);
        aiYPos += 30;
        
        var lblGeminiModel = new Label { Text = "Gemini Model:", Location = new Point(10, aiYPos), AutoSize = true };
        txtGeminiModel = new TextBox { Location = new Point(150, aiYPos), Size = new Size(430, 25), Text = "gemini-1.5-pro" };
        grpAiProvider.Controls.Add(lblGeminiModel);
        grpAiProvider.Controls.Add(txtGeminiModel);
        
        settingsPanel.Controls.Add(grpAiProvider);
        yPos += 290;

        var lblPiperPath = new Label { Text = "Piper Path:", Location = new Point(10, yPos), AutoSize = true };
        txtPiperPath = new TextBox { Location = new Point(200, yPos), Size = new Size(400, 25) };
        settingsPanel.Controls.Add(lblPiperPath);
        settingsPanel.Controls.Add(txtPiperPath);
        yPos += 35;

        var lblPiperModel = new Label { Text = "Piper Model Path:", Location = new Point(10, yPos), AutoSize = true };
        txtPiperModel = new TextBox { Location = new Point(200, yPos), Size = new Size(400, 25) };
        settingsPanel.Controls.Add(lblPiperModel);
        settingsPanel.Controls.Add(txtPiperModel);
        yPos += 35;

        var lblFFmpegPath = new Label { Text = "FFmpeg Path:", Location = new Point(10, yPos), AutoSize = true };
        txtFFmpegPath = new TextBox { Location = new Point(200, yPos), Size = new Size(400, 25) };
        settingsPanel.Controls.Add(lblFFmpegPath);
        settingsPanel.Controls.Add(txtFFmpegPath);
        yPos += 35;

        // Unsplash API Section
        var grpUnsplash = new GroupBox {
            Text = "Unsplash Image Generation (Optional)",
            Location = new Point(10, yPos),
            Size = new Size(600, 110)
        };
        
        chkUseUnsplash = new CheckBox {
            Text = "Enable automatic image generation from Unsplash",
            Location = new Point(10, 25),
            Size = new Size(580, 20)
        };
        chkUseUnsplash.CheckedChanged += (s, e) => {
            txtUnsplashApiKey.Enabled = chkUseUnsplash.Checked;
        };
        
        var lblUnsplashKey = new Label {
            Text = "API Key:",
            Location = new Point(10, 55),
            AutoSize = true
        };
        
        txtUnsplashApiKey = new TextBox {
            Location = new Point(80, 52),
            Size = new Size(500, 25),
            Enabled = false
        };
        
        var lblUnsplashInfo = new Label {
            Text = "Get your free API key at: https://unsplash.com/developers",
            Location = new Point(10, 82),
            Size = new Size(580, 20),
            ForeColor = Color.DarkBlue
        };
        
        grpUnsplash.Controls.AddRange(new Control[] {
            chkUseUnsplash, lblUnsplashKey, txtUnsplashApiKey, lblUnsplashInfo
        });
        settingsPanel.Controls.Add(grpUnsplash);
        yPos += 120;

        // Dark Mode Toggle
        chkDarkMode = new CheckBox {
            Text = "Enable Dark Mode",
            Location = new Point(200, yPos),
            AutoSize = true
        };
        chkDarkMode.CheckedChanged += (s, e) => {
            // Theme will be applied when settings are saved
        };
        settingsPanel.Controls.Add(chkDarkMode);
        yPos += 35;

        // GPU Acceleration Section
        var grpGpuSettings = new GroupBox {
            Text = "Video Encoding Settings",
            Location = new Point(10, yPos),
            Size = new Size(600, 100)
        };
        
        chkUseGpu = new CheckBox {
            Text = "Enable GPU Acceleration (faster encoding, requires compatible GPU)",
            Location = new Point(10, 25),
            AutoSize = true
        };
        chkUseGpu.CheckedChanged += (s, e) => {
            cmbGpuEncoder.Enabled = chkUseGpu.Checked;
        };
        
        var lblGpuEncoder = new Label {
            Text = "GPU Encoder:",
            Location = new Point(10, 55),
            AutoSize = true
        };
        
        cmbGpuEncoder = new ComboBox {
            Location = new Point(120, 52),
            Size = new Size(200, 25),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbGpuEncoder.Items.AddRange(new object[] { "auto", "nvidia", "amd", "intel", "apple" });
        cmbGpuEncoder.SelectedIndex = 0;
        cmbGpuEncoder.Enabled = false;
        
        grpGpuSettings.Controls.Add(chkUseGpu);
        grpGpuSettings.Controls.Add(lblGpuEncoder);
        grpGpuSettings.Controls.Add(cmbGpuEncoder);
        settingsPanel.Controls.Add(grpGpuSettings);
        yPos += 110;

        // Video Output Settings Section
        var grpVideoOutput = new GroupBox {
            Text = "Video Output Settings",
            Location = new Point(10, yPos),
            Size = new Size(600, 180)
        };

        var lblResolution = new Label { Text = "Resolution:", Location = new Point(10, 25), AutoSize = true };
        cmbResolution = new ComboBox {
            Location = new Point(120, 22),
            Size = new Size(150, 25),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbResolution.Items.AddRange(new object[] { "720p", "1080p", "1440p", "4K" });
        cmbResolution.SelectedIndex = 1; // 1080p default

        var lblQuality = new Label { Text = "Quality Preset:", Location = new Point(300, 25), AutoSize = true };
        cmbQuality = new ComboBox {
            Location = new Point(410, 22),
            Size = new Size(150, 25),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbQuality.Items.AddRange(new object[] { "Low", "Medium", "High", "Ultra" });
        cmbQuality.SelectedIndex = 1; // Medium default

        var lblFrameRate = new Label { Text = "Frame Rate:", Location = new Point(10, 60), AutoSize = true };
        cmbFrameRate = new ComboBox {
            Location = new Point(120, 57),
            Size = new Size(150, 25),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbFrameRate.Items.AddRange(new object[] { "24 fps", "30 fps", "60 fps" });
        cmbFrameRate.SelectedIndex = 1; // 30 fps default

        var lblVideoBitrate = new Label { Text = "Video Bitrate:", Location = new Point(300, 60), AutoSize = true };
        numVideoBitrate = new NumericUpDown {
            Location = new Point(410, 57),
            Size = new Size(100, 25),
            Minimum = 1000,
            Maximum = 50000,
            Value = 5000,
            Increment = 500
        };
        var lblVideoBitrateUnit = new Label { Text = "kbps", Location = new Point(515, 60), AutoSize = true };

        var lblAudioBitrate = new Label { Text = "Audio Bitrate:", Location = new Point(10, 95), AutoSize = true };
        numAudioBitrate = new NumericUpDown {
            Location = new Point(120, 92),
            Size = new Size(100, 25),
            Minimum = 64,
            Maximum = 320,
            Value = 192,
            Increment = 32
        };
        var lblAudioBitrateUnit = new Label { Text = "kbps", Location = new Point(225, 95), AutoSize = true };

        var lblAudioChannels = new Label { Text = "Audio Channels:", Location = new Point(300, 95), AutoSize = true };
        cmbAudioChannels = new ComboBox {
            Location = new Point(410, 92),
            Size = new Size(150, 25),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbAudioChannels.Items.AddRange(new object[] { "Mono", "Stereo" });
        cmbAudioChannels.SelectedIndex = 1; // Stereo default

        var lblVideoInfo = new Label {
            Text = "Higher settings = better quality but larger file sizes",
            Location = new Point(10, 130),
            Size = new Size(580, 40),
            ForeColor = Color.DarkGreen
        };

        grpVideoOutput.Controls.AddRange(new Control[] {
            lblResolution, cmbResolution, lblQuality, cmbQuality,
            lblFrameRate, cmbFrameRate, lblVideoBitrate, numVideoBitrate, lblVideoBitrateUnit,
            lblAudioBitrate, numAudioBitrate, lblAudioBitrateUnit,
            lblAudioChannels, cmbAudioChannels, lblVideoInfo
        });
        settingsPanel.Controls.Add(grpVideoOutput);
        yPos += 190;

        btnSaveSettings = new Button {
            Text = "Save Settings",
            Location = new Point(200, yPos),
            Size = new Size(150, 35)
        };
        btnSaveSettings.Click += BtnSaveSettings_Click;
        settingsPanel.Controls.Add(btnSaveSettings);
        
        // Add info label
        var lblGpuInfo = new Label {
            Text = "Note: GPU acceleration requires FFmpeg with GPU support and compatible hardware.\n" +
                   "CPU encoding is more compatible but slower. GPU encoding is 3-10x faster.",
            Location = new Point(10, yPos + 45),
            Size = new Size(600, 40),
            ForeColor = Color.DarkBlue
        };
        settingsPanel.Controls.Add(lblGpuInfo);
        yPos += 100;

        // Animation Settings Section
        var grpAnimationSettings = new GroupBox {
            Text = "Video Animation Settings",
            Location = new Point(10, yPos),
            Size = new Size(600, 180)
        };
        
        int animYPos = 25;
        
        chkEnableKenBurns = new CheckBox {
            Text = "Enable Ken Burns Effect (zoom/pan on images)",
            Location = new Point(10, animYPos),
            Size = new Size(580, 20),
            Checked = true
        };
        grpAnimationSettings.Controls.Add(chkEnableKenBurns);
        animYPos += 30;
        
        chkEnableCrossfade = new CheckBox {
            Text = "Enable Crossfade Transitions between images",
            Location = new Point(10, animYPos),
            Size = new Size(580, 20),
            Checked = true
        };
        grpAnimationSettings.Controls.Add(chkEnableCrossfade);
        animYPos += 30;
        
        var lblTransitionDuration = new Label {
            Text = "Transition Duration (seconds):",
            Location = new Point(10, animYPos + 3),
            AutoSize = true
        };
        numTransitionDuration = new NumericUpDown {
            Location = new Point(220, animYPos),
            Size = new Size(80, 25),
            Minimum = 0.1m,
            Maximum = 5.0m,
            DecimalPlaces = 1,
            Increment = 0.1m,
            Value = 1.0m
        };
        grpAnimationSettings.Controls.Add(lblTransitionDuration);
        grpAnimationSettings.Controls.Add(numTransitionDuration);
        animYPos += 35;
        
        var lblZoomIntensity = new Label {
            Text = "Zoom Intensity (1.0 = no zoom, 1.5 = 50% zoom):",
            Location = new Point(10, animYPos + 3),
            AutoSize = true
        };
        numZoomIntensity = new NumericUpDown {
            Location = new Point(320, animYPos),
            Size = new Size(80, 25),
            Minimum = 1.0m,
            Maximum = 2.0m,
            DecimalPlaces = 1,
            Increment = 0.1m,
            Value = 1.2m
        };
        grpAnimationSettings.Controls.Add(lblZoomIntensity);
        grpAnimationSettings.Controls.Add(numZoomIntensity);
        
        settingsPanel.Controls.Add(grpAnimationSettings);
        yPos += 190;

        // Whisper Settings Section
        var grpWhisperSettings = new GroupBox {
            Text = "Whisper (Caption) Settings",
            Location = new Point(10, yPos),
            Size = new Size(600, 140)
        };
        
        int whisperYPos = 25;
        
        var lblWhisperPath = new Label {
            Text = "Whisper Command:",
            Location = new Point(10, whisperYPos + 3),
            AutoSize = true
        };
        txtWhisperPath = new TextBox {
            Location = new Point(150, whisperYPos),
            Size = new Size(430, 25),
            Text = "whisper"
        };
        grpWhisperSettings.Controls.Add(lblWhisperPath);
        grpWhisperSettings.Controls.Add(txtWhisperPath);
        whisperYPos += 35;
        
        var lblWhisperModel = new Label {
            Text = "Whisper Model:",
            Location = new Point(10, whisperYPos + 3),
            AutoSize = true
        };
        cmbWhisperModel = new ComboBox {
            Location = new Point(150, whisperYPos),
            Size = new Size(200, 25),
            DropDownStyle = ComboBoxStyle.DropDownList
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
            Checked = false
        };
        grpWhisperSettings.Controls.Add(chkUseWhisperApi);
        
        settingsPanel.Controls.Add(grpWhisperSettings);
        yPos += 150;

        // Save Settings Button
        btnSaveSettings = new Button {
            Text = "Save All Settings",
            Location = new Point(10, yPos),
            Size = new Size(200, 40),
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
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
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        statusPanel.Controls.Add(lblStatusTitle);

        txtSystemStatus = new TextBox { 
            Location = new Point(10, 40), 
            Size = new Size(900, 400),
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            ReadOnly = true,
            BackColor = Color.White,
            Font = new Font("Consolas", 10)
        };
        statusPanel.Controls.Add(txtSystemStatus);

        btnCheckStatus = new Button {
            Text = "Check System Status",
            Location = new Point(10, 450),
            Size = new Size(200, 35)
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
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
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
            Font = new Font("Consolas", 9)
        };
        debugPanel.Controls.Add(txtOllamaConsole);

        var buttonPanel = new FlowLayoutPanel {
            Location = new Point(10, 450),
            Size = new Size(900, 40),
            FlowDirection = FlowDirection.LeftToRight
        };

        btnStartOllama = new Button {
            Text = "Start Ollama Server",
            Size = new Size(150, 35),
            Margin = new Padding(0, 0, 10, 0)
        };
        btnStartOllama.Click += BtnStartOllama_Click;
        buttonPanel.Controls.Add(btnStartOllama);

        btnStopOllama = new Button {
            Text = "Stop Ollama Server",
            Size = new Size(150, 35),
            Enabled = false,
            Margin = new Padding(0, 0, 10, 0)
        };
        btnStopOllama.Click += BtnStopOllama_Click;
        buttonPanel.Controls.Add(btnStopOllama);

        btnClearConsole = new Button {
            Text = "Clear Console",
            Size = new Size(120, 35),
            Margin = new Padding(0, 0, 10, 0)
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
    private ProgressBar progressBar;
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
    private CheckBox chkDarkMode;
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
    private ProgressBar progressBarCaptions;
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
