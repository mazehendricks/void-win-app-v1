# AI Video Generation Setup Guide

## Overview

Void Video Generator now supports **actual AI-generated video content** instead of just image slideshows! Choose from 4 different providers based on your needs and budget.

## Quick Start

1. **Open Settings Tab** in the application
2. **Scroll to "AI Video Generation" section**
3. **Select a provider** from the dropdown
4. **Configure** based on provider type (see below)
5. **Click "Save Settings"**
6. **Generate videos** - they'll now use AI video instead of images!

---

## Provider Options

### 1. Runway ML Gen-3 ⭐ (Best Quality)

**Type**: Cloud-based  
**Cost**: ~$3.00 per minute of video  
**Quality**: Excellent (industry-leading)  
**Max Duration**: 10 seconds per clip  

**Setup**:
1. Go to https://runwayml.com
2. Sign up for an account
3. Navigate to API settings
4. Generate an API key
5. In Void Video Generator:
   - Select "Runway ML (Cloud - Best Quality)"
   - Paste your API key
   - Adjust motion intensity (0-10)
   - Choose style (realistic, cinematic, animated, artistic)
   - Save settings

**Best For**: Professional content, marketing videos, high-quality productions

---

### 2. Luma AI Dream Machine ⭐ (Excellent Quality)

**Type**: Cloud-based  
**Cost**: ~$3.60 per minute of video  
**Quality**: Excellent  
**Max Duration**: 5 seconds per clip  

**Setup**:
1. Go to https://lumalabs.ai
2. Create an account
3. Get your API key from account settings
4. In Void Video Generator:
   - Select "Luma AI (Cloud - Excellent)"
   - Enter your API key
   - Configure motion and style settings
   - Save settings

**Best For**: High-quality short clips, social media content

---

### 3. AnimateDiff (Local) ⭐⭐⭐ (Free, Best Privacy)

**Type**: Local generation via ComfyUI  
**Cost**: Free (electricity only)  
**Quality**: Good (depends on models)  
**Max Duration**: 10 seconds per clip  
**Requirements**: RTX 3060+ GPU, 8GB+ VRAM

**Setup**:

#### Step 1: Install ComfyUI
```bash
# Clone ComfyUI
git clone https://github.com/comfyanonymous/ComfyUI
cd ComfyUI

# Install dependencies
pip install -r requirements.txt
```

#### Step 2: Download AnimateDiff Models
```bash
# Download motion module
cd models/animatediff
wget https://huggingface.co/guoyww/animatediff/resolve/main/mm_sd_v15_v2.ckpt

# Download checkpoint (Realistic Vision recommended)
cd ../checkpoints
wget https://huggingface.co/SG161222/Realistic_Vision_V5.1_noVAE/resolve/main/realisticVisionV51_v51VAE.safetensors
```

#### Step 3: Start ComfyUI
```bash
python main.py
# ComfyUI will start on http://localhost:8188
```

#### Step 4: Configure in Void Video Generator
1. Select "AnimateDiff (Local - Free)"
2. Ensure ComfyUI endpoint is `http://localhost:8188`
3. Verify model paths match your installation
4. Save settings

**Best For**: Unlimited generations, complete privacy, offline use, budget-conscious creators

---

### 4. Hybrid (Local) 💰 (Budget Option)

**Type**: Local keyframe generation + interpolation  
**Cost**: Free  
**Quality**: Moderate  
**Max Duration**: 30 seconds per clip  
**Requirements**: FFmpeg only

**Setup**:
1. Ensure FFmpeg is installed (already required for Void Video Generator)
2. Select "Hybrid (Local - Budget)"
3. Configure interpolation method (RIFE recommended)
4. Set keyframe interval (12 frames = smooth)
5. Save settings

**How It Works**:
- Generates keyframes using Stable Diffusion (or placeholders)
- Interpolates between keyframes using RIFE/FILM
- Creates smooth video from static images

**Best For**: Testing, prototyping, very long videos, minimal cost

---

## Configuration File

All settings are saved to `config.json`. Example:

```json
{
  "AIVideoGeneration": {
    "Provider": "RunwayML",
    "RunwayML": {
      "ApiKey": "your-runway-api-key-here",
      "BaseUrl": "https://api.runwayml.com/v1",
      "MaxDuration": 10,
      "DefaultModel": "gen3",
      "TimeoutSeconds": 300
    },
    "LumaAI": {
      "ApiKey": "your-luma-api-key-here",
      "BaseUrl": "https://api.lumalabs.ai/v1",
      "MaxDuration": 5,
      "TimeoutSeconds": 300
    },
    "AnimateDiff": {
      "ComfyUIEndpoint": "http://localhost:8188",
      "ModelPath": "models/animatediff/mm_sd_v15_v2.ckpt",
      "CheckpointPath": "models/checkpoints/realisticVisionV51.safetensors",
      "Steps": 20,
      "CFG": 7.5,
      "Sampler": "euler_ancestral",
      "TimeoutSeconds": 600
    },
    "Hybrid": {
      "InterpolationMethod": "RIFE",
      "KeyframeInterval": 12,
      "TargetFPS": 24
    },
    "DefaultSettings": {
      "AspectRatio": "16:9",
      "MotionIntensity": 5.0,
      "Style": "realistic",
      "NegativePrompt": "blurry, low quality, distorted, watermark, text",
      "DefaultDuration": 4
    }
  }
}
```

---

## Usage

### Basic Workflow

1. **Enter your video topic** in the Generate tab
2. **Configure Channel DNA** (niche, persona, tone, etc.)
3. **Click "Generate Video"**
4. **Watch the magic happen**:
   - Script generation (AI writes the script)
   - AI video generation (creates actual video clips)
   - Voice synthesis (adds narration)
   - Final assembly (combines everything)

### Advanced Settings

**Motion Intensity** (0-10):
- 0-3: Subtle, minimal movement
- 4-6: Moderate motion (recommended)
- 7-10: Dynamic, high energy

**Style Options**:
- `realistic`: Photorealistic content
- `cinematic`: Film-like quality
- `animated`: Cartoon/animation style
- `artistic`: Creative, stylized

**Aspect Ratios**:
- `16:9`: YouTube, landscape
- `9:16`: TikTok, Instagram Reels, vertical
- `1:1`: Instagram posts, square
- `4:3`: Classic format

---

## Cost Comparison

### Per 60-Second Video

| Provider | Cost | Time | Quality | Privacy |
|----------|------|------|---------|---------|
| Runway ML | ~$3.00 | 2-3 min | ⭐⭐⭐⭐⭐ | Cloud |
| Luma AI | ~$3.60 | 2-4 min | ⭐⭐⭐⭐⭐ | Cloud |
| AnimateDiff | $0 | 10-30 min | ⭐⭐⭐⭐ | Local |
| Hybrid | $0 | 5-10 min | ⭐⭐⭐ | Local |
| Images Only | $0 | 1 min | ⭐⭐ | Local |

### Annual Cost (100 videos/month)

- **Runway ML**: $3,600/year
- **Luma AI**: $4,320/year
- **AnimateDiff**: $0/year (electricity ~$50)
- **Hybrid**: $0/year

---

## Troubleshooting

### "API key is not configured"
- Ensure you've entered your API key in Settings
- Click "Save Settings" after entering the key
- Restart the application

### "ComfyUI is not running"
- Start ComfyUI: `python main.py` in ComfyUI directory
- Verify it's accessible at http://localhost:8188
- Check firewall settings

### "Video generation failed"
- Check your API key is valid and has credits
- Verify internet connection (for cloud providers)
- Check logs in the Status Log section
- Try reducing motion intensity or duration

### "Out of memory" (AnimateDiff)
- Reduce video duration
- Lower resolution in settings
- Close other GPU-intensive applications
- Upgrade GPU (RTX 3060 minimum recommended)

### Slow generation (AnimateDiff)
- Normal: 2-5 minutes per 2-second clip
- Reduce steps (20 is good balance)
- Use faster sampler (euler_ancestral)
- Ensure GPU acceleration is enabled

---

## Tips & Best Practices

### For Best Results

1. **Write clear, descriptive prompts** in your video topic
2. **Use specific visual cues** in Channel DNA content style
3. **Start with shorter videos** (30-60 seconds) to test
4. **Adjust motion intensity** based on content type
5. **Use negative prompts** to avoid unwanted elements

### Recommended Settings by Use Case

**YouTube Explainer**:
- Provider: Runway ML or AnimateDiff
- Style: realistic
- Motion: 5-6
- Aspect Ratio: 16:9

**TikTok/Reels**:
- Provider: Luma AI or Runway ML
- Style: cinematic
- Motion: 7-8
- Aspect Ratio: 9:16

**Product Demo**:
- Provider: Runway ML
- Style: realistic
- Motion: 3-4
- Aspect Ratio: 16:9 or 1:1

**Artistic Content**:
- Provider: AnimateDiff
- Style: artistic
- Motion: 6-8
- Aspect Ratio: Any

---

## FAQ

**Q: Can I mix providers?**  
A: Not in a single video, but you can switch providers between videos.

**Q: Do I need all providers configured?**  
A: No, just configure the one(s) you want to use.

**Q: Can I use my own images with AI video?**  
A: Yes! Add images in the Generate tab. They'll be used as reference frames.

**Q: How long does generation take?**  
A: Cloud: 2-5 minutes. Local: 10-30 minutes depending on GPU.

**Q: Can I cancel during generation?**  
A: Yes, close the application or use the cancel button (if available).

**Q: Is my API key secure?**  
A: Keys are stored locally in config.json. Never share this file.

---

## Support

For issues or questions:
1. Check the Status tab for service availability
2. Review logs in the Status Log section
3. Consult the troubleshooting section above
4. Check provider documentation (Runway ML, Luma AI, ComfyUI)

---

## What's Next?

See [`ADVANCED_FEATURES_ROADMAP.md`](ADVANCED_FEATURES_ROADMAP.md) for upcoming features:
- Storyboard preview
- Character consistency
- Voice cloning
- Multi-lingual support
- Template library
- And much more!
