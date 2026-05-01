# Void Video Generator

A **100% local-only** faceless AI video generator for Windows. Create professional YouTube videos using local AI models without any cloud services or recurring costs.

![Version](https://img.shields.io/badge/version-1.0.0-blue)
![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![License](https://img.shields.io/badge/license-MIT-green)

## 🎯 Features

- **Local LLM Script Generation** - Uses Ollama (Llama 3.1, Mistral, etc.) for intelligent script writing
- **Channel DNA System** - Define your unique voice, niche, and persona for authentic content
- **Local Voice Synthesis** - Piper TTS for high-quality, natural-sounding narration
- **Video Assembly** - FFmpeg-powered video creation with audio and visuals
- **Zero Cloud Costs** - Everything runs on your machine
- **Privacy First** - No data sent to external services
- **2026 Compliance** - Built-in originality features to avoid platform penalties

## 🖥️ System Requirements

### Minimum
- **OS**: Windows 10/11 (64-bit)
- **CPU**: Intel i5 / AMD Ryzen 5 (8+ cores recommended)
- **RAM**: 16GB (32GB recommended)
- **Storage**: 100GB+ SSD for models and output
- **.NET**: .NET 8.0 Runtime

### Recommended
- **GPU**: NVIDIA RTX 3060+ (12GB VRAM) for faster processing
- **CPU**: Intel i7 / AMD Ryzen 7
- **RAM**: 32GB
- **Storage**: 256GB+ NVMe SSD

## 📦 Installation

### Step 1: Install .NET 8 SDK

Download and install from: https://dotnet.microsoft.com/download/dotnet/8.0

Verify installation:
```bash
dotnet --version
```

### Step 2: Install Ollama (Local LLM)

1. Download from: https://ollama.com
2. Install the application
3. Open terminal and pull a model:

```bash
# Recommended: Llama 3.1 (8B parameters)
ollama pull llama3.1

# Alternative: Mistral (7B parameters, faster)
ollama pull mistral

# For better quality (requires more RAM):
ollama pull llama3.1:70b
```

4. Verify Ollama is running:
```bash
ollama list
```

### Step 3: Install Piper TTS (Local Voice)

**Quick Install (Recommended):**

**Windows:**
```bash
# Run the automated installer
install-piper.bat
```

**Linux/Codespaces:**
```bash
# Run the automated installer
./install-piper.sh
```

**Manual Installation:**

1. Download Piper from: https://github.com/rhasspy/piper/releases
2. Extract to a folder (e.g., `C:\Tools\piper`)
3. Download a voice model from: https://github.com/rhasspy/piper/blob/master/VOICES.md

**Recommended voices:**
- `en_US-lessac-medium` - Clear, professional male voice
- `en_US-amy-medium` - Natural female voice
- `en_GB-alan-medium` - British accent

4. Extract the voice model (`.onnx` and `.json` files) to `C:\Tools\piper\models\`

### Step 4: Install FFmpeg

**Option A: Using Package Manager (Recommended)**
```bash
# Using Chocolatey
choco install ffmpeg

# Using Scoop
scoop install ffmpeg
```

**Option B: Manual Installation**
1. Download from: https://ffmpeg.org/download.html
2. Extract to `C:\Tools\ffmpeg`
3. Add to system PATH:
   - Open System Properties → Environment Variables
   - Edit `Path` variable
   - Add `C:\Tools\ffmpeg\bin`

4. Verify installation:
```bash
ffmpeg -version
```

### Step 5: Build the Application

1. Clone or download this repository
2. Open terminal in the project directory
3. Build the application:

```bash
cd src
dotnet restore
dotnet build
```

4. Run the application:
```bash
dotnet run
```

## 🚀 Quick Start

### First Launch

1. **Check System Status**
   - Open the "System Status" tab
   - Click "Check System Status"
   - Verify all services show ✓ (Available)

2. **Configure Settings** (if needed)
   - Go to "Settings" tab
   - Update paths if you installed tools in custom locations:
     - Ollama URL: `http://localhost:11434` (default)
     - Ollama Model: `llama3.1` (or your chosen model)
     - Piper Path: `C:\Tools\piper\piper.exe`
     - Piper Model: `C:\Tools\piper\models\en_US-lessac-medium.onnx`
     - FFmpeg Path: `ffmpeg` (if in PATH) or full path
   - Click "Save Settings"

### Generate Your First Video

1. **Go to "Generate Video" tab**

2. **Enter Video Details:**
   - **Title**: "5 Productivity Hacks That Actually Work"
   - **Topic**: "Share 5 practical productivity tips backed by research, focusing on time management and focus techniques"
   - **Duration**: 60 seconds

3. **Configure Channel DNA:**
   - **Niche**: "Productivity & Self-Improvement"
   - **Host Persona**: "Enthusiastic productivity coach"
   - **Tone**: "Energetic, motivational, practical"
   - **Target Audience**: "Young professionals and students"
   - **Content Style**: "Quick tips with actionable advice"

4. **Select Output Folder** (e.g., `C:\Videos\Output`)

5. **Click "Generate Video"**

The process will:
1. Generate a script using your local LLM (1-2 minutes)
2. Create voice audio with Piper TTS (30-60 seconds)
3. Assemble the video with FFmpeg (10-30 seconds)

## 📁 Project Structure

```
void-win-app-v1/
├── src/
│   ├── Models/
│   │   ├── VideoRequest.cs      # Video generation request model
│   │   ├── VideoScript.cs       # Script structure with segments
│   │   └── AppConfig.cs         # Application configuration
│   ├── Services/
│   │   ├── IScriptGeneratorService.cs
│   │   ├── OllamaScriptGenerator.cs      # Local LLM integration
│   │   ├── IVoiceGeneratorService.cs
│   │   ├── PiperTTSService.cs            # Local TTS integration
│   │   ├── IVideoAssemblyService.cs
│   │   ├── FFmpegVideoAssembly.cs        # Video assembly
│   │   └── VideoGenerationPipeline.cs    # Main orchestration
│   ├── MainForm.cs              # Main UI logic
│   ├── MainForm.Designer.cs     # UI layout
│   ├── Program.cs               # Entry point
│   └── VoidVideoGenerator.csproj
├── README.md
└── .gitignore
```

## 🎨 Channel DNA Explained

The **Channel DNA** system ensures your content has a unique voice and avoids generic AI detection:

- **Niche**: Your content category (e.g., "Tech Reviews", "Cooking", "Finance")
- **Host Persona**: The character delivering content (e.g., "Skeptical expert", "Friendly teacher")
- **Tone Guidelines**: How you communicate (e.g., "Casual and humorous", "Professional and authoritative")
- **Target Audience**: Who you're speaking to (e.g., "Beginners", "Advanced users")
- **Content Style**: Your approach (e.g., "Story-driven", "Data-focused", "Tutorial-based")

### Examples

**Tech Channel:**
```
Niche: Technology & Gadgets
Persona: Enthusiastic tech geek
Tone: Excited but honest, conversational
Audience: Tech enthusiasts aged 18-35
Style: In-depth reviews with real-world testing
```

**Finance Channel:**
```
Niche: Personal Finance & Investing
Persona: Experienced financial advisor
Tone: Clear, trustworthy, educational
Audience: Young adults starting their financial journey
Style: Practical advice with step-by-step guidance
```

## 🔧 Advanced Configuration

### Using Different LLM Models

Edit [`config.json`](config.json) after first run:

```json
{
  "OllamaModel": "mistral",  // Faster, good quality
  "OllamaModel": "llama3.1", // Balanced (recommended)
  "OllamaModel": "llama3.1:70b" // Best quality, slower
}
```

### Custom Voice Models

1. Browse available voices: https://github.com/rhasspy/piper/blob/master/VOICES.md
2. Download your preferred voice
3. Update [`PiperModelPath`](src/Models/AppConfig.cs) in settings

### Output Structure

Generated videos are saved in:
```
output/
└── YYYYMMDD_HHMMSS_VideoTitle/
    ├── audio/
    │   ├── segment_000_hook.wav
    │   ├── segment_001_body.wav
    │   └── segment_002_cta.wav
    ├── visuals/
    │   └── (placeholder for images)
    ├── script.txt
    └── VideoTitle.mp4
```

## 🎯 2026 Compliance & Best Practices

### Avoiding AI Detection

1. **Use Specific Channel DNA** - Generic prompts = generic content
2. **Add Human Touch** - Review and edit scripts (20% human input recommended)
3. **Mix Delivery Styles** - Vary pacing, add pauses, use emphasis
4. **Original Research** - Include unique insights, not just AI knowledge
5. **Visual Variety** - Use multiple angles and dynamic visuals

### Content Quality Checklist

- ✅ Unique hook in first 10 seconds
- ✅ Clear promise to the viewer
- ✅ Specific examples and actionable advice
- ✅ No generic AI phrases ("in this video", "let's dive in")
- ✅ Natural transitions between sections
- ✅ Strong call-to-action

## 🔍 Debugging

### Built-in Debug Console

The application includes a **Debug Console** tab that provides live monitoring of the Ollama server:

**Features:**
- **Live Server Output** - See Ollama server logs in real-time
- **Start/Stop Controls** - Launch and manage Ollama directly from the app
- **Request Monitoring** - Watch incoming requests and responses
- **Automatic Detection** - Detects if Ollama is already running

**How to Use:**

1. **Open Debug Console Tab**
   - Launch the application
   - Click on the "Debug Console" tab

2. **Start Ollama Server** (if not running)
   - Click "Start Ollama Server"
   - Watch the console for startup messages
   - Server will listen on `http://localhost:11434`

3. **Monitor Activity**
   - Generate a video from the "Generate Video" tab
   - Switch to "Debug Console" tab
   - Watch real-time logs showing:
     - Incoming requests
     - Model loading
     - Token generation
     - Response completion

4. **Troubleshoot Issues**
   - If no activity appears when generating, there's a configuration issue
   - Console will show connection errors, model loading issues, etc.
   - Use "Clear Console" to reset the view

**Console Output Example:**
```
[17:05:55] === Starting Ollama Server ===
[17:05:55] Time: 2026-04-30 17:05:55
[17:05:55]
[17:05:56] [OUT] Ollama server starting...
[17:05:56] [OUT] Listening on http://localhost:11434
[17:05:56] ✓ Ollama server started successfully
[17:06:23] [OUT] POST /api/generate
[17:06:23] [OUT] Loading model: llama3.1
[17:06:25] [OUT] Generating response...
[17:07:12] [OUT] Response complete (2456 tokens)
```

## 🔍 Debugging (Advanced)

### Enable Detailed Logging

The application provides detailed progress logging in the "Generate Video" tab. Watch the log output to see exactly where issues occur.

### Debug Mode - Step by Step

#### 1. Test Ollama Independently

Run the included test script:
```bash
test-ollama.bat
```

This verifies Ollama works outside the application.

#### 2. Use Built-in System Diagnostics

1. Open the application
2. Navigate to "System Status" tab
3. Click "Check System Status"
4. Review the output for each component:
   - ✓ = Working correctly
   - ✗ = Not available
   - ⚠️ = Warning/partial issue

**Example Output:**
```
=== SYSTEM STATUS CHECK ===

✓ Ollama (LLM): Available
✓ Piper (TTS): Available
✓ FFmpeg: Available

=== OLLAMA DIAGNOSTICS ===

✓ Ollama is ready
  URL: http://localhost:11434
  Model: llama3.1
  Available models: llama3.1, mistral, phi3
```

#### 3. Test Each Component Separately

**Test Ollama:**
```bash
# Check if running
curl http://localhost:11434/api/tags

# Test generation
curl http://localhost:11434/api/generate -d "{\"model\":\"llama3.1\",\"prompt\":\"Hello\",\"stream\":false}"
```

**Test Piper:**
```bash
# Navigate to Piper directory
cd C:\Tools\piper

# Test voice generation
echo "Hello world" | piper.exe --model models\en_US-lessac-medium.onnx --output_file test.wav
```

**Test FFmpeg:**
```bash
ffmpeg -version
```

#### 4. Check Configuration File

Location: `config.json` (created after first run)

**Verify settings:**
```json
{
  "OllamaUrl": "http://localhost:11434",
  "OllamaModel": "llama3.1",
  "PiperPath": "C:\\Tools\\piper\\piper.exe",
  "PiperModelPath": "C:\\Tools\\piper\\models\\en_US-lessac-medium.onnx",
  "FFmpegPath": "ffmpeg",
  "DefaultOutputPath": "C:\\Users\\YourName\\Videos\\Void Gen Output"
}
```

**Common mistakes:**
- Wrong path separators (use `\\` in JSON)
- Incorrect Ollama URL
- Model name doesn't match installed model
- Piper paths point to non-existent files

#### 5. Monitor During Generation

When you click "Generate Video", watch the log output:

**Expected flow:**
```
[17:05:55] === Starting Video Generation ===
[17:05:55] Title: Test Video
[17:05:55] Duration: 60 seconds
[17:05:55] Output: C:\Users\...\20260430_170555_Test Video
[17:05:55]
[17:05:55] Step 1/4: Generating script...
[17:05:55] Generating script with local LLM...
[17:05:55] Checking Ollama connection at http://localhost:11434...
[17:05:56] ✓ Ollama connection verified
[17:05:56] Sending request to Ollama (model: llama3.1)...
[17:05:56] Prompt length: 1234 bytes
[17:05:56] Waiting for LLM response (this may take 1-5 minutes)...
[17:07:23] Received response (2456 chars), parsing...
[17:07:23] Script generated successfully (2456 chars)
[17:07:23] Parsing script...
[17:07:23] Step 2/4: Generating voice audio...
...
```

**If it hangs at "Waiting for LLM response":**
- Check if Ollama terminal shows activity
- If NO activity: Configuration issue (wrong URL)
- If activity but slow: Model is processing (wait or use smaller model)
- If timeout: Model too large for your hardware

#### 6. Common Debug Scenarios

**Scenario A: "Cannot connect to Ollama"**
```
Error: Cannot connect to Ollama at http://localhost:11434
```

**Solution:**
1. Open new terminal
2. Run: `ollama serve`
3. Keep terminal open
4. Try again

**Scenario B: "Model not found"**
```
⚠️ Model 'llama3.1' not found
Available models: mistral, phi3
```

**Solution:**
```bash
ollama pull llama3.1
```

**Scenario C: "Ollama connection verified" but then timeout**
```
[17:05:56] ✓ Ollama connection verified
[17:05:56] Waiting for LLM response...
[17:10:56] Error: LLM request timed out
```

**Solution:**
- Model is too slow for your hardware
- Try smaller model: `ollama pull llama3.2:1b`
- Update config to use faster model

**Scenario D: No error but Ollama shows no activity**
```
[17:05:56] Waiting for LLM response...
(Ollama terminal shows nothing)
```

**Solution:**
1. Wrong Ollama URL in config
2. Check `config.json` - should be `http://localhost:11434`
3. Restart application after fixing

### Debug Checklist

Before reporting issues, verify:

- [ ] Ran `test-ollama.bat` successfully
- [ ] Ollama is running (`ollama serve` in terminal)
- [ ] Model is installed (`ollama list`)
- [ ] Config file has correct paths
- [ ] System Status Check shows all ✓
- [ ] Checked log output for specific error messages
- [ ] Tried with a smaller model (if timeout issues)
- [ ] Restarted application after config changes

### Advanced Debugging

#### Enable Verbose HTTP Logging

For deep debugging, you can monitor HTTP traffic:

**Using Fiddler or similar:**
1. Install Fiddler (https://www.telerik.com/fiddler)
2. Run Fiddler
3. Run the application
4. Watch HTTP requests to `localhost:11434`
5. Verify requests are being sent

#### Check Ollama Logs

Ollama logs show what it's processing:

```bash
# Run Ollama with verbose logging
ollama serve --verbose
```

Watch for incoming requests when you click "Generate Video".

#### Manual API Test

Test the exact request the app makes:

```bash
curl -X POST http://localhost:11434/api/generate ^
  -H "Content-Type: application/json" ^
  -d "{\"model\":\"llama3.1\",\"prompt\":\"Test\",\"stream\":false}"
```

If this works but the app doesn't, there's an issue with the app's HTTP client.

## � Troubleshooting

**For detailed troubleshooting, see [TROUBLESHOOTING.md](TROUBLESHOOTING.md)**

### Quick Diagnostics

1. **Use the built-in System Status Check**
   - Open the "System Status" tab in the application
   - Click "Check System Status"
   - Review the detailed diagnostics for each component

2. **Common Issues**

#### Ollama Not Detected

```bash
# Check if Ollama is running
ollama list

# Start Ollama service (if not running)
ollama serve

# Test API
curl http://localhost:11434/api/tags
```

#### Piper TTS Errors

- Verify `.onnx` and `.json` files are in the same directory
- Check file paths don't contain special characters
- Ensure you have the correct voice model for your language

#### FFmpeg Not Found

```bash
# Test FFmpeg
ffmpeg -version

# If not found, add to PATH or use full path in settings
# Example: C:\Tools\ffmpeg\bin\ffmpeg.exe
```

#### Out of Memory Errors

- Use smaller LLM models (7B instead of 70B)
- Close other applications
- Reduce video duration
- Upgrade RAM if possible

#### Application Hangs at "Waiting for LLM response..."

This is the most common issue. See [TROUBLESHOOTING.md](TROUBLESHOOTING.md) for detailed solutions:
- Verify Ollama is running
- Check if the model is downloaded
- Try a smaller/faster model
- Check system resources

## 📊 Performance Tips

### Speed Optimization

1. **Use GPU acceleration** (if available)
   - Ollama automatically uses GPU
   - Significant speed improvement for LLM

2. **Choose appropriate model size**
   - 7B models: Fast, good quality
   - 13B models: Balanced
   - 70B+ models: Best quality, much slower

3. **Batch processing**
   - Generate multiple scripts at once
   - Process audio in parallel (future feature)

### Quality Optimization

1. **Script Generation**
   - Provide detailed topics
   - Use specific Channel DNA
   - Review and edit generated scripts

2. **Voice Quality**
   - Use medium or high-quality voice models
   - Adjust speaking rate if needed
   - Consider voice cloning for unique sound

3. **Video Assembly**
   - Use high-resolution images (1920x1080)
   - Add background music (manual for now)
   - Include text overlays for key points

## 🛣️ Roadmap

### Version 1.1 (Planned)
- [ ] Stable Diffusion integration for image generation
- [ ] Background music library
- [ ] Text overlay automation
- [ ] Batch video generation
- [ ] Template system for common video types

### Version 1.2 (Planned)
- [ ] Voice cloning with XTTS
- [ ] Talking head avatars (Wav2Lip/SadTalker)
- [ ] YouTube API integration for direct upload
- [ ] SEO optimization tools
- [ ] Analytics dashboard

### Version 2.0 (Future)
- [ ] Multi-language support
- [ ] Advanced editing timeline
- [ ] Stock footage integration
- [ ] Thumbnail generator
- [ ] A/B testing tools

## 💰 Cost Comparison

| Component | Cloud Service | Local (This App) |
|-----------|--------------|------------------|
| Script Generation | $20-100/mo | $0 |
| Voice Synthesis | $22-99/mo | $0 |
| Video Assembly | $29-149/mo | $0 |
| **Total Monthly** | **$71-348** | **$0** |

**One-time costs:**
- GPU (optional): $300-800
- Electricity: ~$5-15/month

## 📝 License

MIT License - See LICENSE file for details

## 🤝 Contributing

Contributions are welcome! Please feel free to submit pull requests or open issues.

## 📧 Support

For issues and questions:
- Open an issue on GitHub
- Check the troubleshooting section
- Review Ollama/Piper/FFmpeg documentation

## 🙏 Acknowledgments

- **Ollama** - Local LLM runtime
- **Piper** - High-quality TTS
- **FFmpeg** - Video processing
- **.NET Team** - Excellent framework

## ⚠️ Disclaimer

This tool is for creating original content. Users are responsible for:
- Ensuring content complies with platform policies
- Adding human creativity and originality (20%+ recommended)
- Respecting copyright and intellectual property
- Following YouTube's AI disclosure requirements

---

**Built with ❤️ for content creators who value privacy and independence**
