namespace VoidVideoGenerator;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

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

        // Tab Control
        tabControl = new TabControl();
        tabControl.Dock = DockStyle.Fill;
        
        // Tabs
        tabGenerate = new TabPage("Generate Video");
        tabSettings = new TabPage("Settings");
        tabStatus = new TabPage("System Status");
        tabDebug = new TabPage("Debug Console");
        
        tabControl.TabPages.Add(tabGenerate);
        tabControl.TabPages.Add(tabSettings);
        tabControl.TabPages.Add(tabStatus);
        tabControl.TabPages.Add(tabDebug);

        // === GENERATE TAB ===
        var generatePanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
        tabGenerate.Controls.Add(generatePanel);

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

        // === SETTINGS TAB ===
        var settingsPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
        tabSettings.Controls.Add(settingsPanel);

        yPos = 10;
        var lblOllamaUrl = new Label { Text = "Ollama URL:", Location = new Point(10, yPos), AutoSize = true };
        txtOllamaUrl = new TextBox { Location = new Point(200, yPos), Size = new Size(400, 25) };
        settingsPanel.Controls.Add(lblOllamaUrl);
        settingsPanel.Controls.Add(txtOllamaUrl);
        yPos += 35;

        var lblOllamaModel = new Label { Text = "Ollama Model:", Location = new Point(10, yPos), AutoSize = true };
        txtOllamaModel = new TextBox { Location = new Point(200, yPos), Size = new Size(400, 25) };
        settingsPanel.Controls.Add(lblOllamaModel);
        settingsPanel.Controls.Add(txtOllamaModel);
        yPos += 35;

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
        yPos += 50;

        btnSaveSettings = new Button { 
            Text = "Save Settings", 
            Location = new Point(200, yPos), 
            Size = new Size(150, 35) 
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
    private Button btnGenerate;
    private ProgressBar progressBar;
    private TextBox txtLog;
    
    // Settings Tab Controls
    private TextBox txtOllamaUrl;
    private TextBox txtOllamaModel;
    private TextBox txtPiperPath;
    private TextBox txtPiperModel;
    private TextBox txtFFmpegPath;
    private Button btnSaveSettings;
    
    // Status Tab Controls
    private TextBox txtSystemStatus;
    private Button btnCheckStatus;
    
    // Debug Console Tab Controls
    private TextBox txtOllamaConsole;
    private Button btnStartOllama;
    private Button btnStopOllama;
    private Button btnClearConsole;
}
