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

## 🐛 Troubleshooting

### Ollama Not Detected

```bash
# Check if Ollama is running
ollama list

# Start Ollama service (if not running)
ollama serve

# Test API
curl http://localhost:11434/api/tags
```

### Piper TTS Errors

- Verify `.onnx` and `.json` files are in the same directory
- Check file paths don't contain special characters
- Ensure you have the correct voice model for your language

### FFmpeg Not Found

```bash
# Test FFmpeg
ffmpeg -version

# If not found, add to PATH or use full path in settings
# Example: C:\Tools\ffmpeg\bin\ffmpeg.exe
```

### Out of Memory Errors

- Use smaller LLM models (7B instead of 70B)
- Close other applications
- Reduce video duration
- Upgrade RAM if possible

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
