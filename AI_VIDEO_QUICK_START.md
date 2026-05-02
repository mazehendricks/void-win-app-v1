# AI Video Generation - Quick Start Guide 🚀

Get started with AI video generation in 5 minutes!

---

## 🎯 Choose Your Path

### Path 1: Cloud (Best Quality) - 5 minutes
**Best for**: Professional videos, no GPU required

1. **Get an API Key**
   - Runway ML: https://runwayml.com → Sign up → Get API key
   - OR Luma AI: https://lumalabs.ai → Sign up → Get API key

2. **Configure the App**
   - Open Void Video Generator
   - Go to **Settings** tab
   - Scroll to **AI Video Generation** section
   - Select "Runway ML (Cloud)" or "Luma AI (Cloud)"
   - Paste your API key
   - Set Motion Intensity: 7 (recommended)
   - Set Style: cinematic (recommended)
   - Click **Save Settings**

3. **Generate Your First Video**
   - Go to **Generate Video** tab
   - Title: "Test Video"
   - Topic: "A beautiful sunset over the ocean"
   - Click **Generate Video**
   - Wait 2-3 minutes
   - Done! 🎉

**Cost**: ~$0.50 for a 10-second video

---

### Path 2: Local Free (Requires GPU) - 15 minutes
**Best for**: Unlimited videos, have RTX 3060+ GPU

1. **Install ComfyUI**
   ```bash
   git clone https://github.com/comfyanonymous/ComfyUI
   cd ComfyUI
   pip install -r requirements.txt
   python main.py
   ```
   ComfyUI should start on http://localhost:8188

2. **Configure the App**
   - Open Void Video Generator
   - Go to **Settings** tab
   - Scroll to **AI Video Generation** section
   - Select "AnimateDiff (Local)"
   - Set Motion Intensity: 5-7
   - Set Style: realistic
   - Click **Save Settings**

3. **Generate Your First Video**
   - Go to **Generate Video** tab
   - Title: "Test Video"
   - Topic: "A cat playing with a ball"
   - Click **Generate Video**
   - Wait 5-10 minutes (first time is slower)
   - Done! 🎉

**Cost**: Free! (uses your GPU)

---

### Path 3: Hybrid Free (No GPU) - 2 minutes
**Best for**: Budget-friendly, no GPU, decent quality

1. **Ensure FFmpeg is Installed**
   - Already included if you followed the main setup guide
   - Test: Open terminal and type `ffmpeg -version`

2. **Configure the App**
   - Open Void Video Generator
   - Go to **Settings** tab
   - Scroll to **AI Video Generation** section
   - Select "Hybrid (Local)"
   - Set Motion Intensity: 5
   - Set Style: realistic
   - Click **Save Settings**

3. **Generate Your First Video**
   - Go to **Generate Video** tab
   - Title: "Test Video"
   - Topic: "A peaceful forest scene"
   - Click **Generate Video**
   - Wait 3-5 minutes
   - Done! 🎉

**Cost**: Free! (uses CPU only)

---

### Path 4: Traditional (Images Only) - 0 minutes
**Best for**: Quick videos, no AI video needed

1. **Already Configured!**
   - Default setting is "None (Images Only)"
   - Uses image slideshows with Ken Burns effect
   - Works exactly like before

2. **Generate a Video**
   - Go to **Generate Video** tab
   - Add images or enable Unsplash
   - Click **Generate Video**
   - Done! 🎉

**Cost**: Free!

---

## 🎬 Example Videos You Can Create

### 1. Educational Content
```
Title: "How Photosynthesis Works"
Topic: "Explain the process of photosynthesis in plants"
Provider: Runway ML
Motion: 5 (moderate)
Style: realistic
Duration: 60 seconds
```

### 2. Product Demo
```
Title: "Introducing Our New App"
Topic: "Showcase the features of our mobile app"
Provider: Luma AI
Motion: 7 (dynamic)
Style: cinematic
Duration: 30 seconds
```

### 3. Story Time
```
Title: "The Adventure Begins"
Topic: "A hero's journey through a magical forest"
Provider: AnimateDiff
Motion: 6 (moderate-high)
Style: animated
Duration: 90 seconds
```

### 4. Nature Documentary
```
Title: "Wildlife of the Amazon"
Topic: "Explore the diverse wildlife of the Amazon rainforest"
Provider: Hybrid
Motion: 4 (subtle)
Style: cinematic
Duration: 120 seconds
```

---

## 💡 Pro Tips

### For Best Results:
1. **Be Specific**: "A red sports car driving on a coastal highway at sunset" is better than "a car"
2. **Keep Clips Short**: 5-10 seconds per segment works best
3. **Match Style to Content**: Use "realistic" for documentaries, "cinematic" for stories, "animated" for fun content
4. **Adjust Motion**: Lower for calm scenes, higher for action

### Motion Intensity Guide:
- **0-2**: Almost static (portraits, landscapes)
- **3-5**: Gentle motion (nature, calm scenes)
- **6-8**: Moderate motion (most content)
- **9-10**: High motion (action, sports)

### Style Guide:
- **realistic**: Photorealistic, documentary-style
- **cinematic**: Film-like, dramatic lighting, color grading
- **animated**: Stylized, cartoon-like, fun
- **artistic**: Creative, painterly, unique

---

## 🐛 Common Issues

### "API key invalid"
- Double-check you copied the entire key
- Ensure no extra spaces
- Verify your account has credits

### "ComfyUI not responding"
- Check ComfyUI is running: http://localhost:8188
- Restart ComfyUI if needed
- Check firewall isn't blocking port 8188

### "Video generation failed"
- Check your internet connection (for cloud providers)
- Verify you have enough credits (for cloud providers)
- Check logs in Debug Console tab

### "Out of memory"
- Reduce video duration
- Lower motion intensity
- Use Hybrid mode instead of AnimateDiff

---

## 📊 Cost Comparison

| Provider | 10 sec | 30 sec | 60 sec | 120 sec |
|----------|--------|--------|--------|---------|
| Runway ML | $0.50 | $1.50 | $3.00 | $6.00 |
| Luma AI | $0.60 | $1.80 | $3.60 | $7.20 |
| AnimateDiff | Free | Free | Free | Free |
| Hybrid | Free | Free | Free | Free |

---

## 🎓 Next Steps

1. ✅ Generate your first test video
2. ✅ Experiment with different providers
3. ✅ Try different motion intensities and styles
4. ✅ Create a real video for your project
5. ✅ Share your results!

---

## 📚 More Information

- **Full Documentation**: [AI_VIDEO_GENERATION_COMPLETE.md](AI_VIDEO_GENERATION_COMPLETE.md)
- **Implementation Details**: [AI_VIDEO_GENERATION_IMPLEMENTATION.md](AI_VIDEO_GENERATION_IMPLEMENTATION.md)
- **Troubleshooting**: [TROUBLESHOOTING.md](TROUBLESHOOTING.md)
- **GPU Setup**: [GPU_SETUP.md](GPU_SETUP.md)

---

## 🎉 You're Ready!

Pick a path above and start creating amazing AI-generated videos in minutes!

**Questions?** Check the documentation or open an issue on GitHub.

**Happy creating!** 🎬✨
