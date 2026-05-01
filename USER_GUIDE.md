# Void Video Generator - User Guide

## 🎬 Complete Guide to Creating AI-Generated Videos

Welcome to Void Video Generator! This guide will help you create professional videos using AI.

---

## 📋 Table of Contents

1. [Quick Start](#quick-start)
2. [Keyboard Shortcuts](#keyboard-shortcuts)
3. [Generate Video Tab](#generate-video-tab)
4. [Add Captions Tab](#add-captions-tab)
5. [Settings Tab](#settings-tab)
6. [System Status Tab](#system-status-tab)
7. [Debug Console Tab](#debug-console-tab)
8. [Tips & Best Practices](#tips--best-practices)
9. [Troubleshooting](#troubleshooting)

---

## 🚀 Quick Start

### First Time Setup

1. **Install Dependencies**
   - Run `build.bat` to install all required components
   - Or manually install: Ollama, Piper TTS, FFmpeg, and Whisper

2. **Configure Settings** (F3)
   - Choose your AI provider (Ollama, OpenAI, Anthropic, or Gemini)
   - Set paths for Piper TTS and FFmpeg
   - Configure video quality settings
   - Click "Save All Settings"

3. **Check System Status** (F4)
   - Click "Check System Status" to verify all components are working
   - Fix any issues shown in red (✗)

4. **Generate Your First Video** (F1)
   - Enter a video title
   - Describe your topic
   - Click "Generate Video" or press Ctrl+G

---

## ⌨️ Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| **Ctrl+G** | Generate Video (when on Generate tab) |
| **Ctrl+S** | Save Settings (when on Settings tab) |
| **Ctrl+T** | Check System Status (when on Status tab) |
| **F1** | Switch to Generate Video tab |
| **F2** | Switch to Add Captions tab |
| **F3** | Switch to Settings tab |
| **F4** | Switch to System Status tab |
| **F5** | Refresh System Status |
| **Ctrl+Q** | Quit application |

---

## 🎥 Generate Video Tab

### Basic Information

**Video Title** (Required, min 5 characters)
- Enter a catchy, descriptive title
- Example: "10 Amazing Facts About Space"
- This will be used for the output filename

**Topic/Description** (Required, min 10 characters)
- Describe what your video should cover
- Be specific for better AI-generated scripts
- Example: "Explore fascinating facts about our solar system, including planet sizes, distances, and unique features"

**Target Duration**
- Set desired video length (15-600 seconds)
- Actual length may vary slightly based on content
- Default: 60 seconds

### Channel DNA

Configure your channel's unique voice and style:

**Niche**
- Your channel's focus area
- Examples: Technology, Education, Entertainment, Science, Gaming

**Host Persona**
- The personality of your narrator
- Examples: Friendly expert, Enthusiastic teacher, Professional guide

**Tone Guidelines**
- How the script should sound
- Examples: Professional, Casual, Humorous, Inspirational

**Target Audience**
- Who is watching your videos?
- Examples: Beginners, Professionals, Students, General audience

**Content Style**
- Your content approach
- Examples: Tutorial, Storytelling, Documentary, List-based

### Visuals

**Add Images** (Optional)
- Click "Add Images" to select custom images
- Images will be displayed in the order added
- Supports: PNG, JPG, JPEG, BMP, GIF
- If no images added, placeholder visuals will be used
- Or enable Unsplash in Settings for automatic image generation

**Output Folder**
- Where your generated video will be saved
- Each video gets its own timestamped folder
- Contains: final video, script, audio files, and images

### Generate

Click **"Generate Video"** or press **Ctrl+G** to start!

The process includes:
1. ✅ Generating AI script based on your topic
2. 🎤 Creating voice narration with Piper TTS
3. 🖼️ Processing images (with Ken Burns effect if enabled)
4. 🎬 Assembling final video with FFmpeg
5. ✨ Adding transitions and effects

**Progress Log** shows real-time status updates.

---

## 📝 Add Captions Tab

Add professional captions to any video using AI transcription.

### Input Video
- Click "Browse..." to select your video file
- Supports: MP4, AVI, MOV, MKV, WMV, FLV

### Output Video
- Choose where to save the captioned video
- Auto-suggests filename with "_captioned" suffix

### Caption Style
- **YouTube**: Classic white text with black outline
- **TikTok**: Bold, large text with yellow highlight
- **Minimal**: Simple, clean subtitles

### Transcription Method
- **Whisper (Local)**: Uses local Whisper installation (free, private)
- **Whisper (OpenAI API)**: Uses OpenAI's API (requires API key, more accurate)
- **Manual SRT File**: Import your own subtitle file

### Generate Captions

Click **"Generate Captions"** to:
1. 🎤 Transcribe audio using Whisper AI
2. ⏱️ Generate timestamps for each word/phrase
3. 📝 Create SRT subtitle file
4. 🎬 Overlay captions on video with chosen style

---

## ⚙️ Settings Tab

### AI Provider

Choose your script generation engine:

**Ollama** (Local, Free)
- Runs on your computer
- No API costs
- Requires: Ollama installed and running
- Models: llama3.1, mistral, phi3, etc.
- URL: http://localhost:11434 (default)

**OpenAI** (Cloud, Paid)
- GPT-4, GPT-3.5-turbo
- Requires: OpenAI API key
- Best quality, fastest generation

**Anthropic Claude** (Cloud, Paid)
- Claude 3.5 Sonnet, Claude 3 Opus
- Requires: Anthropic API key
- Excellent for creative content

**Google Gemini** (Cloud, Paid)
- Gemini 1.5 Pro
- Requires: Google AI API key
- Good balance of quality and speed

### Voice & Video Paths

**Piper Path**
- Path to Piper TTS executable
- Example: `C:\piper\piper.exe`
- Run `install-piper.bat` to install

**Piper Model**
- Path to voice model file (.onnx)
- Example: `C:\piper\voices\en_US-lessac-medium.onnx`
- Download from Piper voices repository

**FFmpeg Path**
- Path to FFmpeg executable
- Example: `ffmpeg` (if in PATH) or `C:\ffmpeg\bin\ffmpeg.exe`
- Download from ffmpeg.org

### Unsplash Integration

**Enable Unsplash Images**
- Automatically generate images from visual cues in script
- Requires: Unsplash API key (free tier available)
- Get key at: https://unsplash.com/developers

### Video Encoding Settings

**GPU Acceleration**
- Enable for 3-10x faster video encoding
- Requires: Compatible GPU and FFmpeg with GPU support
- Encoders:
  - `h264_nvenc`: NVIDIA GPUs
  - `h264_amf`: AMD GPUs
  - `h264_qsv`: Intel GPUs
  - `auto`: Automatically detect

**Resolution**
- 4K (3840x2160): Ultra HD
- 1080p (1920x1080): Full HD (recommended)
- 720p (1280x720): HD
- 480p (854x480): SD

**Quality Preset**
- High: Best quality, larger file size
- Medium: Balanced (recommended)
- Low: Faster encoding, smaller files

**Frame Rate**
- 60 fps: Smooth, modern look
- 30 fps: Standard (recommended)
- 24 fps: Cinematic feel

### Video Animation Settings

**Ken Burns Effect**
- Adds smooth zoom and pan to images
- Creates dynamic, professional look
- Alternates between zoom-in and zoom-out

**Crossfade Transitions**
- Smooth fades between images
- Alternative to hard cuts
- Duration: 0.5-3.0 seconds (default: 1.0)

**Zoom Intensity**
- How much to zoom during Ken Burns effect
- 1.0 = no zoom
- 1.2 = subtle zoom (recommended)
- 1.5 = dramatic zoom

### Whisper Settings

**Whisper Path**
- Path to Whisper executable
- Example: `whisper` (if in PATH)
- Run `install-whisper.bat` to install

**Whisper Model**
- tiny: Fastest, least accurate
- base: Good balance (recommended)
- small: Better accuracy
- medium: High accuracy
- large: Best accuracy, slowest

**Use Whisper API**
- Use OpenAI's Whisper API instead of local
- Requires: OpenAI API key
- More accurate, faster, but costs money

### Save Settings

Click **"Save All Settings"** or press **Ctrl+S** to save your configuration.

---

## 🔍 System Status Tab

Comprehensive diagnostics for all components.

### Check System Status

Click **"Check System Status"** or press **Ctrl+T** or **F5** to run diagnostics.

### Status Report Includes:

**System Information**
- Operating System version
- .NET Framework version
- Current timestamp

**Core Services**
- ✓ = Available and working
- ✗ = Not available or not working

**AI Script Generator**
- Provider status
- Model information
- API key validation
- Troubleshooting steps if unavailable

**Voice Generator (Piper TTS)**
- Executable location
- Model file location
- File existence checks
- Installation instructions

**Video Assembly (FFmpeg)**
- FFmpeg availability
- GPU acceleration status
- Video settings summary
- Animation features status

**Caption Generator (Whisper)**
- Local or API mode
- Model configuration
- API key status

**Image Service (Unsplash)**
- Integration status
- API key validation

**System Resources**
- Working directory
- Config file status
- Output directory validation
- Free disk space (warns if < 5 GB)

### Troubleshooting

Each unavailable service includes:
- ⚠️ Specific error details
- 📋 Step-by-step fix instructions
- 🔗 Download links
- 💡 Common solutions

---

## 🐛 Debug Console Tab

Monitor and control Ollama server.

### Features

**Start Ollama Server**
- Launches Ollama process
- Shows real-time output
- Displays process ID (PID)
- Auto-tests connection after startup

**Stop Ollama Server**
- Gracefully terminates Ollama
- Confirms shutdown

**Clear Console**
- Clears output log
- Shows helpful command reference

### Console Output

- `[OLLAMA]`: Server output messages
- `[INFO]`: Informational messages
- `✓`: Success indicators
- `⚠`: Warnings
- `✗`: Errors

---

## 💡 Tips & Best Practices

### For Best Video Quality

1. **Use Specific Topics**
   - ❌ Bad: "Tell me about space"
   - ✅ Good: "Explain the 5 largest planets in our solar system, their unique features, and interesting facts about each"

2. **Optimize Channel DNA**
   - Match your actual channel's style
   - Be consistent across videos
   - Update as your channel evolves

3. **Choose Right Duration**
   - Short-form (15-60s): Quick facts, tips
   - Medium (60-180s): Tutorials, explanations
   - Long-form (180-600s): Deep dives, stories

4. **Image Selection**
   - Use high-resolution images (1920x1080 or higher)
   - Ensure images relate to your topic
   - Mix wide shots and close-ups for variety

5. **Enable Animations**
   - Ken Burns effect adds professionalism
   - Crossfade transitions look smoother
   - Adjust zoom intensity based on content

### For Faster Generation

1. **Use GPU Acceleration**
   - 3-10x faster video encoding
   - Requires compatible GPU

2. **Choose Efficient Settings**
   - 720p instead of 1080p
   - 30 fps instead of 60 fps
   - Medium quality preset

3. **Use Local AI (Ollama)**
   - No API latency
   - Unlimited generations
   - Smaller models (phi3) are faster

### For Better Captions

1. **Use Clear Audio**
   - Minimize background noise
   - Clear speech
   - Good microphone quality

2. **Choose Right Whisper Model**
   - Base: Good for most content
   - Medium/Large: For accents or technical terms
   - API: For best accuracy

3. **Select Appropriate Style**
   - YouTube: General content
   - TikTok: Short-form, attention-grabbing
   - Minimal: Professional, corporate

---

## 🔧 Troubleshooting

### Video Generation Fails

**"Services not initialized"**
- Go to Settings tab
- Verify all paths are correct
- Click "Save All Settings"
- Restart application

**"Ollama not available"**
- Open Debug Console tab
- Click "Start Ollama Server"
- Wait for "✓ Server is responding"
- Or run `ollama serve` in terminal

**"Piper not found"**
- Run `install-piper.bat`
- Or download from: https://github.com/rhasspy/piper/releases
- Update path in Settings

**"FFmpeg not found"**
- Download from: https://ffmpeg.org/download.html
- Add to system PATH
- Or specify full path in Settings

### Caption Generation Fails

**"Whisper not found"**
- Run `install-whisper.bat`
- Or: `pip install openai-whisper`
- Update path in Settings

**"Transcription failed"**
- Check audio quality
- Try different Whisper model
- Consider using Whisper API

### Poor Video Quality

**Blurry or pixelated**
- Increase resolution in Settings
- Use higher quality images
- Increase video bitrate

**Choppy or laggy**
- Increase frame rate
- Enable GPU acceleration
- Check system resources

**Audio out of sync**
- Regenerate video
- Check FFmpeg version
- Try different audio codec

### Application Crashes

1. Check System Status (F4)
2. Review Debug Console for errors
3. Verify all dependencies installed
4. Check free disk space (need 5+ GB)
5. See TROUBLESHOOTING.md for details

---

## 📚 Additional Resources

- **QUICKSTART.md**: Fast setup guide
- **SETUP_GUIDE.md**: Detailed installation
- **TROUBLESHOOTING.md**: Common issues
- **GPU_SETUP.md**: GPU acceleration guide
- **ONLINE_AI_SETUP.md**: Cloud AI providers
- **ANIMATION_GUIDE.md**: Video effects guide
- **UNSPLASH_SETUP.md**: Image integration

---

## 🎓 Example Workflows

### Workflow 1: Educational Video

1. **Generate Tab**
   - Title: "How Photosynthesis Works"
   - Topic: "Explain the process of photosynthesis in plants, including light-dependent and light-independent reactions"
   - Duration: 120 seconds
   - Niche: Education
   - Persona: Friendly teacher
   - Tone: Clear and engaging

2. **Add Images**
   - Plant diagrams
   - Chloroplast illustrations
   - Sunlight photos

3. **Settings**
   - Enable Ken Burns
   - Enable Crossfade
   - 1080p, 30fps

4. **Generate!**

### Workflow 2: Quick Social Media Clip

1. **Generate Tab**
   - Title: "3 Productivity Hacks"
   - Topic: "Share 3 quick productivity tips for remote workers"
   - Duration: 30 seconds
   - Style: List-based, energetic

2. **Settings**
   - 720p for faster encoding
   - Strong zoom intensity (1.4)
   - Fast transitions (0.5s)

3. **Generate!**

4. **Add Captions**
   - Use TikTok style
   - Bold, attention-grabbing

### Workflow 3: Professional Tutorial

1. **Generate Tab**
   - Title: "Introduction to Python Programming"
   - Topic: "Beginner-friendly introduction to Python, covering variables, data types, and basic syntax"
   - Duration: 300 seconds
   - Niche: Technology
   - Persona: Professional instructor
   - Tone: Clear and methodical

2. **Add Custom Images**
   - Code screenshots
   - Python logo
   - IDE screenshots

3. **Settings**
   - 1080p, 60fps
   - High quality
   - Subtle animations

4. **Generate!**

5. **Add Captions**
   - YouTube style
   - For accessibility

---

## 🎉 You're Ready!

Start creating amazing AI-generated videos!

**Remember:**
- Press **F1-F4** to switch tabs quickly
- Press **Ctrl+G** to generate videos
- Press **F5** to check system status
- Hover over any field for helpful tooltips

**Need Help?**
- Check System Status tab for diagnostics
- Review troubleshooting section
- See additional documentation files

Happy video creating! 🎬✨
