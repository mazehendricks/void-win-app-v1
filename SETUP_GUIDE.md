# Complete Setup Guide - Void Video Generator

This guide will walk you through setting up all required components for the Void Video Generator.

## 📋 Prerequisites Checklist

Before starting, ensure you have:
- [ ] Windows 10 or 11 (64-bit)
- [ ] Administrator access
- [ ] At least 50GB free disk space
- [ ] Stable internet connection (for initial downloads)

## 🔧 Step-by-Step Installation

### 1. Install .NET 8 SDK (Required)

**Time: 5 minutes**

1. Visit: https://dotnet.microsoft.com/download/dotnet/8.0
2. Download ".NET 8.0 SDK" (not just Runtime)
3. Run the installer
4. Accept defaults and complete installation

**Verify:**
```bash
# Open Command Prompt or PowerShell
dotnet --version
# Should show: 8.0.x or higher
```

---

### 2. Install Ollama (Local LLM)

**Time: 10-15 minutes**

#### 2.1 Install Ollama Application

1. Visit: https://ollama.com
2. Click "Download for Windows"
3. Run `OllamaSetup.exe`
4. Follow installation wizard
5. Ollama will start automatically

#### 2.2 Download AI Model

Open Command Prompt or PowerShell:

```bash
# Option 1: Llama 3.1 8B (Recommended - 4.7GB)
ollama pull llama3.1

# Option 2: Mistral 7B (Faster - 4.1GB)
ollama pull mistral

# Option 3: Llama 3.1 70B (Best quality - 40GB, requires 32GB+ RAM)
ollama pull llama3.1:70b
```

**Wait for download to complete** (5-10 minutes depending on internet speed)

**Verify:**
```bash
ollama list
# Should show your downloaded model

# Test the model
ollama run llama3.1
# Type: Hello, how are you?
# Press Ctrl+D to exit
```

#### 2.3 Ensure Ollama is Running

Ollama should run automatically. If not:
```bash
ollama serve
```

Keep this terminal open or run as a service.

---

### 3. Install Piper TTS (Local Voice)

**Time: 10 minutes**

#### 3.1 Download Piper

1. Visit: https://github.com/rhasspy/piper/releases
2. Download `piper_windows_amd64.zip` (latest version)
3. Extract to `C:\Tools\piper\`

Your folder structure should look like:
```
C:\Tools\piper\
├── piper.exe
└── (other files)
```

#### 3.2 Download Voice Model

1. Visit: https://github.com/rhasspy/piper/blob/master/VOICES.md
2. Choose a voice (recommendations below)
3. Download both `.onnx` and `.onnx.json` files

**Recommended Voices:**

**For Male Voice:**
- `en_US-lessac-medium` - Professional, clear
  - Download: https://huggingface.co/rhasspy/piper-voices/resolve/main/en/en_US/lessac/medium/en_US-lessac-medium.onnx
  - JSON: https://huggingface.co/rhasspy/piper-voices/resolve/main/en/en_US/lessac/medium/en_US-lessac-medium.onnx.json

**For Female Voice:**
- `en_US-amy-medium` - Natural, friendly
  - Download: https://huggingface.co/rhasspy/piper-voices/resolve/main/en/en_US/amy/medium/en_US-amy-medium.onnx
  - JSON: https://huggingface.co/rhasspy/piper-voices/resolve/main/en/en_US/amy/medium/en_US-amy-medium.onnx.json

#### 3.3 Organize Voice Files

Create a models folder and place your voice files:
```
C:\Tools\piper\
├── piper.exe
└── models\
    ├── en_US-lessac-medium.onnx
    └── en_US-lessac-medium.onnx.json
```

**Verify:**
```bash
cd C:\Tools\piper
echo "Hello, this is a test." | piper.exe --model models\en_US-lessac-medium.onnx --output_file test.wav
# Should create test.wav file
```

---

### 4. Install FFmpeg (Video Processing)

**Time: 5-10 minutes**

#### Option A: Using Chocolatey (Easiest)

If you have Chocolatey installed:
```bash
choco install ffmpeg
```

#### Option B: Using Scoop

If you have Scoop installed:
```bash
scoop install ffmpeg
```

#### Option C: Manual Installation

1. Visit: https://ffmpeg.org/download.html
2. Click "Windows builds from gyan.dev"
3. Download `ffmpeg-release-essentials.zip`
4. Extract to `C:\Tools\ffmpeg\`

**Add to PATH:**
1. Press `Win + X` → System
2. Click "Advanced system settings"
3. Click "Environment Variables"
4. Under "System variables", find "Path"
5. Click "Edit" → "New"
6. Add: `C:\Tools\ffmpeg\bin`
7. Click OK on all dialogs

**Verify:**
```bash
# Close and reopen Command Prompt
ffmpeg -version
# Should show FFmpeg version info
```

---

### 5. Build Void Video Generator

**Time: 5 minutes**

#### 5.1 Get the Source Code

```bash
# If using Git
git clone <repository-url>
cd void-win-app-v1

# Or download and extract ZIP file
```

#### 5.2 Build the Application

```bash
cd src
dotnet restore
dotnet build
```

**Verify:**
```bash
dotnet run
# Application window should open
```

---

## ✅ Configuration

### First Launch Setup

1. **Launch the application:**
   ```bash
   cd src
   dotnet run
   ```

2. **Go to "System Status" tab**
   - Click "Check System Status"
   - Verify all services show ✓

3. **If any service shows ✗, go to "Settings" tab:**

   **Ollama Settings:**
   - URL: `http://localhost:11434`
   - Model: `llama3.1` (or your chosen model)

   **Piper Settings:**
   - Path: `C:\Tools\piper\piper.exe`
   - Model: `C:\Tools\piper\models\en_US-lessac-medium.onnx`

   **FFmpeg Settings:**
   - Path: `ffmpeg` (if in PATH)
   - Or: `C:\Tools\ffmpeg\bin\ffmpeg.exe`

4. **Click "Save Settings"**

5. **Return to "System Status" and verify again**

---

## 🎬 Generate Your First Video

### Test Video

1. Go to "Generate Video" tab

2. Fill in:
   - **Title:** "Test Video"
   - **Topic:** "Create a short introduction about productivity tips"
   - **Duration:** 30 seconds

3. Channel DNA (use defaults or customize):
   - **Niche:** "Educational"
   - **Persona:** "Friendly expert"
   - **Tone:** "Conversational, clear"
   - **Audience:** "General audience"
   - **Style:** "Informative"

4. **Output Folder:** Choose a location (e.g., `C:\Videos`)

5. **Click "Generate Video"**

6. **Wait for completion** (2-5 minutes for first video)

7. **Check output folder** for your video!

---

## 🔍 Troubleshooting

### Ollama Issues

**Problem:** "Ollama (LLM): Not Available"

**Solutions:**
```bash
# Check if Ollama is running
ollama list

# If not running, start it
ollama serve

# Test API
curl http://localhost:11434/api/tags

# If model not found
ollama pull llama3.1
```

### Piper Issues

**Problem:** "Piper (TTS): Not Available"

**Solutions:**
1. Verify `piper.exe` exists at specified path
2. Ensure `.onnx` and `.onnx.json` files are together
3. Test manually:
   ```bash
   cd C:\Tools\piper
   echo "test" | piper.exe --model models\en_US-lessac-medium.onnx --output_file test.wav
   ```

### FFmpeg Issues

**Problem:** "FFmpeg: Not Available"

**Solutions:**
1. Verify FFmpeg is in PATH:
   ```bash
   where ffmpeg
   ```
2. If not found, add to PATH or use full path in settings
3. Restart Command Prompt after PATH changes

### Build Errors

**Problem:** Build fails with errors

**Solutions:**
```bash
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build

# If still failing, check .NET version
dotnet --version
# Should be 8.0 or higher
```

---

## 📊 Performance Optimization

### For Faster Generation

1. **Use smaller models:**
   - `mistral` instead of `llama3.1`
   - 7B models instead of 70B

2. **GPU Acceleration:**
   - Ollama automatically uses NVIDIA GPUs
   - Ensure latest GPU drivers installed

3. **SSD Storage:**
   - Install models on SSD, not HDD
   - Use SSD for output folder

### For Better Quality

1. **Use larger models:**
   - `llama3.1:70b` for best scripts
   - Requires 32GB+ RAM

2. **Better voice models:**
   - Use "high" quality voices instead of "medium"
   - Download from Piper voices repository

3. **Detailed prompts:**
   - Provide specific topics
   - Customize Channel DNA thoroughly

---

## 🎓 Next Steps

Now that everything is set up:

1. **Read the main README.md** for usage guide
2. **Experiment with Channel DNA** to find your voice
3. **Generate test videos** with different settings
4. **Review and edit scripts** before final generation
5. **Join the community** for tips and tricks

---

## 📞 Getting Help

If you encounter issues:

1. **Check this guide** - Most issues are covered here
2. **Review error messages** - They often indicate the problem
3. **Check System Status** - Identifies which component is failing
4. **Search GitHub Issues** - Someone may have had the same problem
5. **Open a new issue** - Provide error messages and system info

---

## ✨ You're Ready!

Congratulations! You now have a fully functional local AI video generator.

**Remember:**
- All processing happens on your machine
- No cloud costs or subscriptions
- Complete privacy and control
- Unlimited video generation

Happy creating! 🎥
