# Quick Start Guide

Get up and running with Void Video Generator in 5 minutes!

## ⚡ Prerequisites

Before you start, make sure you have:
- ✅ Windows 10/11
- ✅ .NET 8 SDK installed
- ✅ Ollama installed with a model downloaded
- ✅ Piper TTS installed with a voice model
- ✅ FFmpeg installed

> **Don't have these?** See [SETUP_GUIDE.md](SETUP_GUIDE.md) for detailed installation instructions.

## 🚀 Quick Setup (5 Minutes)

### 1. Build the Application

**Windows:**
```bash
# Double-click build.bat
# OR run in Command Prompt:
build.bat
```

**Command Line:**
```bash
cd src
dotnet restore
dotnet build
```

### 2. Run the Application

**Windows:**
```bash
# Double-click run.bat
# OR run in Command Prompt:
run.bat
```

**Command Line:**
```bash
cd src
dotnet run
```

### 3. Check System Status

1. Click the **"System Status"** tab
2. Click **"Check System Status"**
3. Verify all services show ✓ (green checkmark)

If any service shows ✗:
- Go to **"Settings"** tab
- Update the paths for missing services
- Click **"Save Settings"**
- Return to System Status and check again

### 4. Generate Your First Video

1. Go to **"Generate Video"** tab

2. **Fill in the basics:**
   ```
   Title: My First AI Video
   Topic: Create a 30-second introduction about the benefits of morning exercise
   Duration: 30 seconds
   ```

3. **Use default Channel DNA** (or customize it)

4. **Select output folder** (e.g., `C:\Videos`)

5. **Click "Generate Video"**

6. **Wait 2-3 minutes** while the AI:
   - Generates the script
   - Creates voice audio
   - Assembles the video

7. **Done!** Your video is in the output folder

## 📝 Example Configurations

### Quick Tip Video (30-60 seconds)
```
Title: 5 Productivity Hacks That Actually Work
Topic: Share 5 quick productivity tips with specific examples
Duration: 60 seconds

Channel DNA:
- Niche: Productivity
- Persona: Energetic coach
- Tone: Motivational, practical
- Audience: Young professionals
- Style: Quick actionable tips
```

### Educational Content (90-120 seconds)
```
Title: How Compound Interest Makes You Rich
Topic: Explain compound interest with a real example showing how $100/month becomes $100K over 20 years
Duration: 90 seconds

Channel DNA:
- Niche: Personal Finance
- Persona: Friendly financial advisor
- Tone: Clear, trustworthy
- Audience: Financial beginners
- Style: Simple explanations with examples
```

## 🎯 Tips for Best Results

### ✅ DO:
- Be specific in your topic description
- Include examples you want covered
- Review and edit generated scripts
- Start with shorter videos (30-60 seconds)
- Customize Channel DNA for your niche

### ❌ DON'T:
- Use vague topics like "talk about productivity"
- Generate without checking system status first
- Skip the Channel DNA configuration
- Expect perfection on first try (iterate!)
- Forget to add your human touch (20% editing)

## 🔧 Common Issues

### "Ollama not available"
```bash
# Start Ollama
ollama serve

# Verify it's running
ollama list
```

### "Piper not available"
- Check the path in Settings tab
- Ensure both `.onnx` and `.onnx.json` files exist
- Test manually: `piper.exe --version`

### "FFmpeg not available"
- Verify FFmpeg is in PATH: `ffmpeg -version`
- Or provide full path in Settings

### "Build failed"
```bash
# Clean and rebuild
cd src
dotnet clean
dotnet restore
dotnet build
```

## 📚 Next Steps

1. **Read [EXAMPLES.md](EXAMPLES.md)** - See example configurations for different niches
2. **Read [README.md](README.md)** - Full documentation and features
3. **Experiment!** - Try different Channel DNA settings
4. **Iterate** - Refine your prompts based on results
5. **Add your touch** - Edit scripts to add personality

## 💡 Pro Tips

### Faster Generation
- Use `mistral` model instead of `llama3.1` (Settings tab)
- Generate shorter videos (30-45 seconds)
- Close other applications

### Better Quality
- Use detailed topic descriptions
- Include specific examples in your topic
- Review scripts before final generation
- Add personal anecdotes manually

### Unique Content
- Customize Channel DNA thoroughly
- Use specific niches (not just "educational")
- Add your unique perspective to scripts
- Include current events or trends

## 🎬 You're Ready!

That's it! You now have everything you need to start creating AI-generated videos locally.

**Remember:**
- ✅ All processing is local (no cloud costs)
- ✅ Unlimited video generation
- ✅ Complete privacy
- ✅ Full control over content

Start creating and have fun! 🚀

---

**Need help?** Check [SETUP_GUIDE.md](SETUP_GUIDE.md) for detailed troubleshooting.
