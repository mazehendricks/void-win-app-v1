 # GPU Acceleration Setup Guide

The Void Video Generator supports GPU-accelerated video encoding for significantly faster video generation (3-10x faster than CPU encoding).

## Supported GPUs

✅ **NVIDIA** (NVENC) - GeForce GTX 600 series and newer
✅ **AMD** (AMF) - Radeon RX 400 series and newer  
✅ **Intel** (Quick Sync Video) - 6th gen Core processors and newer
✅ **Apple** (VideoToolbox) - M1/M2/M3 chips

## Quick Setup

### 1. Enable GPU Acceleration

1. Open **Void Video Generator**
2. Go to **Settings** tab
3. Find **"Video Encoding Settings"** section
4. Check **"Enable GPU Acceleration"**
5. Select your GPU type from dropdown:
   - **auto** - Automatically detect (recommended)
   - **nvidia** - Force NVIDIA NVENC
   - **amd** - Force AMD AMF
   - **intel** - Force Intel Quick Sync
   - **apple** - Force Apple VideoToolbox
6. Click **"Save Settings"**

### 2. Verify FFmpeg Has GPU Support

The application will automatically detect if your FFmpeg installation supports GPU encoding. If not, you'll need to install a GPU-enabled version.

## Installing GPU-Enabled FFmpeg

### Windows

#### Option 1: Download Pre-built (Easiest)
1. Download from: https://github.com/BtbN/FFmpeg-Builds/releases
2. Get the **"ffmpeg-master-latest-win64-gpl.zip"** build
3. Extract to `C:\Tools\ffmpeg\`
4. In Settings, set FFmpeg Path to: `C:\Tools\ffmpeg\bin\ffmpeg.exe`

#### Option 2: Use Chocolatey
```powershell
choco install ffmpeg-full
```

### Linux

```bash
# Ubuntu/Debian with NVIDIA
sudo apt install ffmpeg nvidia-cuda-toolkit

# Arch Linux
sudo pacman -S ffmpeg

# Build from source with GPU support (advanced)
# See: https://trac.ffmpeg.org/wiki/CompilationGuide
```

### macOS

```bash
# Using Homebrew
brew install ffmpeg
```

## GPU-Specific Setup

### NVIDIA GPUs

**Requirements:**
- NVIDIA GPU with NVENC support (GTX 600+, RTX series)
- Latest NVIDIA drivers
- CUDA toolkit (optional, but recommended)

**Verify Support:**
```bash
ffmpeg -encoders | grep nvenc
```

You should see:
```
V..... h264_nvenc           NVIDIA NVENC H.264 encoder
V..... hevc_nvenc           NVIDIA NVENC hevc encoder
```

**Driver Download:**
https://www.nvidia.com/Download/index.aspx

### AMD GPUs

**Requirements:**
- AMD GPU with VCE/VCN support (RX 400+)
- Latest AMD Adrenalin drivers
- FFmpeg with AMF support

**Verify Support:**
```bash
ffmpeg -encoders | grep amf
```

You should see:
```
V..... h264_amf             AMD AMF H.264 Encoder
V..... hevc_amf             AMD AMF HEVC encoder
```

**Driver Download:**
https://www.amd.com/en/support

### Intel GPUs

**Requirements:**
- Intel CPU with Quick Sync Video (6th gen+)
- Latest Intel graphics drivers
- FFmpeg with QSV support

**Verify Support:**
```bash
ffmpeg -encoders | grep qsv
```

You should see:
```
V..... h264_qsv             H.264 (Intel Quick Sync Video acceleration)
```

**Driver Download:**
https://www.intel.com/content/www/us/en/download-center/home.html

## Performance Comparison

### CPU Encoding (libx264)
- **Speed:** 1x (baseline)
- **Quality:** Excellent
- **Compatibility:** 100%
- **Use Case:** Maximum quality, no GPU

### GPU Encoding

| GPU Type | Speed | Quality | Best For |
|----------|-------|---------|----------|
| NVIDIA NVENC | 5-10x | Very Good | RTX cards, high volume |
| AMD AMF | 3-7x | Good | RX 6000/7000 series |
| Intel QSV | 3-5x | Good | Laptops, integrated graphics |
| Apple VideoToolbox | 4-8x | Very Good | M1/M2/M3 Macs |

## Troubleshooting

### "GPU Acceleration enabled but using CPU"

**Check 1: Verify FFmpeg Support**
```bash
ffmpeg -encoders | grep -E "nvenc|amf|qsv|videotoolbox"
```

If nothing appears, your FFmpeg doesn't have GPU support. Reinstall FFmpeg with GPU support.

**Check 2: Check Drivers**
- NVIDIA: Run `nvidia-smi` to verify driver
- AMD: Check AMD Radeon Software
- Intel: Update Intel Graphics drivers

**Check 3: Application Logs**
Look in the Status Log during video generation. It will show:
```
GPU Acceleration: Enabled
GPU Encoder: nvidia (or auto)
```

### "Failed to initialize encoder"

**Solution 1: Update Drivers**
- Download latest GPU drivers from manufacturer

**Solution 2: Try Different Encoder**
- Change from "auto" to specific GPU type
- Or disable GPU acceleration temporarily

**Solution 3: Check GPU Usage**
- Close other GPU-intensive applications
- Check if GPU is being used by games/mining

### "Video quality is worse with GPU"

**Solution:**
- GPU encoding trades some quality for speed
- Increase video bitrate in Settings → Video Output Settings
- Try "High" or "Ultra" quality preset
- For maximum quality, use CPU encoding

## Verifying GPU Usage

### Windows

**Task Manager Method:**
1. Open Task Manager (Ctrl+Shift+Esc)
2. Go to "Performance" tab
3. Select your GPU
4. Look for "Video Encode" or "3D" usage during video generation

**GPU-Z Method:**
1. Download GPU-Z: https://www.techpowerup.com/gpuz/
2. Monitor "Video Engine Load" during encoding

### Linux

```bash
# NVIDIA
nvidia-smi -l 1

# AMD
radeontop

# Intel
intel_gpu_top
```

### macOS

```bash
# Activity Monitor
# Look for "WindowServer" GPU usage
```

## Optimal Settings for GPU Encoding

### For Speed (Fastest)
- Resolution: 720p
- Quality: Low or Medium
- Frame Rate: 30 fps
- GPU: Enabled (auto)

### For Quality (Best)
- Resolution: 1080p or 4K
- Quality: High or Ultra
- Frame Rate: 30 or 60 fps
- GPU: Enabled (nvidia for best quality)

### For Balance (Recommended)
- Resolution: 1080p
- Quality: Medium
- Frame Rate: 30 fps
- GPU: Enabled (auto)
- Video Bitrate: 5000 kbps

## Advanced: Force Specific GPU (Multi-GPU Systems)

If you have multiple GPUs (e.g., integrated + dedicated), Windows may use the wrong one.

### Windows 10/11

1. Open **Settings** → **System** → **Display**
2. Scroll down → **Graphics settings**
3. Click **Browse** → Select `VoidVideoGenerator.exe`
4. Click **Options** → Select **High performance**
5. Restart the application

### NVIDIA Control Panel

1. Open **NVIDIA Control Panel**
2. Go to **Manage 3D Settings**
3. **Program Settings** tab
4. Add `VoidVideoGenerator.exe`
5. Set **Preferred graphics processor** to **High-performance NVIDIA processor**

## FAQ

**Q: Will GPU encoding damage my GPU?**
A: No. Video encoding is a normal GPU workload, much lighter than gaming.

**Q: Can I use GPU for other tasks while encoding?**
A: Yes, but it may slow down both tasks. Close games/heavy apps for best performance.

**Q: Does GPU encoding use more power?**
A: Yes, but less than gaming. Laptop users may see increased battery drain.

**Q: My GPU isn't listed, will it work?**
A: If it's from the last 5-7 years, probably yes. Try "auto" mode.

**Q: Can I encode multiple videos simultaneously?**
A: Not recommended. It may overload the GPU and cause errors.

## Support

For GPU-specific issues:
- **NVIDIA:** https://www.nvidia.com/en-us/support/
- **AMD:** https://www.amd.com/en/support
- **Intel:** https://www.intel.com/content/www/us/en/support.html

For application issues, check TROUBLESHOOTING.md

---

**Note:** GPU acceleration is optional. The application works perfectly fine with CPU encoding if you prefer compatibility over speed.
