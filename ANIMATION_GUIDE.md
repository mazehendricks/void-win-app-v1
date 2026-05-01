# Video Animation Guide

## Overview

The Void Video Generator now supports **smooth animations** to make your videos more engaging and professional-looking. This includes:

1. **Ken Burns Effect** - Smooth zoom and pan movements on images
2. **Crossfade Transitions** - Smooth fading between images
3. **Configurable Settings** - Control animation intensity and duration

---

## Animation Features

### 🎬 Ken Burns Effect

The Ken Burns effect adds life to static images by slowly zooming in or out while panning across the image. This creates a cinematic feel and keeps viewers engaged.

**How it works:**
- Images alternate between zoom-in and zoom-out for variety
- Smooth panning keeps the subject centered
- Configurable zoom intensity (default: 1.2x = 20% zoom)

### 🌊 Crossfade Transitions

Smooth fade transitions between images instead of hard cuts, creating a professional, flowing video.

**How it works:**
- Images fade into each other over a configurable duration
- Default transition duration: 1.0 second
- Seamlessly blends with Ken Burns effect

---

## Configuration

### Enable/Disable Animations

Edit your `config.json` file to control animation settings:

```json
{
  "VideoSettings": {
    "EnableKenBurnsEffect": true,
    "EnableCrossfadeTransitions": true,
    "TransitionDuration": 1.0,
    "ZoomIntensity": 1.2
  }
}
```

### Animation Settings Explained

| Setting | Type | Default | Description |
|---------|------|---------|-------------|
| `EnableKenBurnsEffect` | boolean | `true` | Enable zoom/pan animations on images |
| `EnableCrossfadeTransitions` | boolean | `true` | Enable smooth fade transitions between images |
| `TransitionDuration` | number | `1.0` | Duration of crossfade transitions in seconds |
| `ZoomIntensity` | number | `1.2` | Maximum zoom level (1.0 = no zoom, 1.5 = 50% zoom) |

### Recommended Settings

**For Smooth, Professional Videos:**
```json
{
  "EnableKenBurnsEffect": true,
  "EnableCrossfadeTransitions": true,
  "TransitionDuration": 1.0,
  "ZoomIntensity": 1.2
}
```

**For Subtle Animations:**
```json
{
  "EnableKenBurnsEffect": true,
  "EnableCrossfadeTransitions": true,
  "TransitionDuration": 0.5,
  "ZoomIntensity": 1.1
}
```

**For Dramatic Effect:**
```json
{
  "EnableKenBurnsEffect": true,
  "EnableCrossfadeTransitions": true,
  "TransitionDuration": 1.5,
  "ZoomIntensity": 1.5
}
```

**For Static Videos (No Animation):**
```json
{
  "EnableKenBurnsEffect": false,
  "EnableCrossfadeTransitions": false
}
```

---

## Performance Considerations

### GPU Acceleration

Animations work with both CPU and GPU encoding. For best performance with animations:

1. **Enable GPU acceleration** in your config:
   ```json
   {
     "UseGpuAcceleration": true,
     "GpuEncoder": "auto"
   }
   ```

2. **Supported GPU encoders:**
   - NVIDIA: `nvidia` or `nvenc` (requires CUDA-enabled FFmpeg)
   - AMD: `amd` or `amf` (requires AMF-enabled FFmpeg)
   - Intel: `intel` or `qsv` (requires Quick Sync Video)
   - Apple: `apple` or `videotoolbox` (macOS only)

### Rendering Time

Animations increase rendering time compared to static videos:

- **Static video**: ~1-2 minutes for 60-second video
- **Animated video (CPU)**: ~3-5 minutes for 60-second video
- **Animated video (GPU)**: ~1.5-3 minutes for 60-second video

The exact time depends on:
- Number of images
- Video resolution (1080p vs 4K)
- CPU/GPU performance
- Animation complexity

---

## Troubleshooting

### Animations Not Working

If animations aren't appearing in your videos:

1. **Check FFmpeg version**: Ensure you have FFmpeg 4.3 or later
   ```bash
   ffmpeg -version
   ```

2. **Verify settings**: Make sure animation settings are enabled in `config.json`

3. **Check logs**: Look for "Creating animated video" in the generation logs

4. **Fallback mode**: If animation fails, the system automatically falls back to static video

### Performance Issues

If video generation is too slow:

1. **Enable GPU acceleration** (if you have a compatible GPU)
2. **Reduce zoom intensity** to 1.1 or lower
3. **Shorten transition duration** to 0.5 seconds
4. **Lower video resolution** (e.g., 720p instead of 1080p)
5. **Disable Ken Burns effect** and keep only crossfades

### Quality Issues

If animations look choppy or low quality:

1. **Increase frame rate** to 60 FPS:
   ```json
   {
     "FrameRate": 60
   }
   ```

2. **Increase video bitrate**:
   ```json
   {
     "VideoBitrate": 8000
   }
   ```

3. **Use higher quality preset**:
   ```json
   {
     "QualityPreset": "High"
   }
   ```

---

## Examples

### Example 1: Educational Video
```json
{
  "EnableKenBurnsEffect": true,
  "EnableCrossfadeTransitions": true,
  "TransitionDuration": 1.0,
  "ZoomIntensity": 1.15,
  "FrameRate": 30
}
```
Result: Smooth, professional animations that don't distract from content

### Example 2: Cinematic Video
```json
{
  "EnableKenBurnsEffect": true,
  "EnableCrossfadeTransitions": true,
  "TransitionDuration": 2.0,
  "ZoomIntensity": 1.4,
  "FrameRate": 60
}
```
Result: Dramatic, movie-like animations with slow, sweeping movements

### Example 3: Fast-Paced Video
```json
{
  "EnableKenBurnsEffect": true,
  "EnableCrossfadeTransitions": true,
  "TransitionDuration": 0.3,
  "ZoomIntensity": 1.1,
  "FrameRate": 30
}
```
Result: Quick, energetic transitions for dynamic content

---

## Technical Details

### How It Works

The animation system uses FFmpeg's powerful filter complex to:

1. **Scale images** to 2x resolution for zoom headroom
2. **Apply zoompan filter** for Ken Burns effect
3. **Chain xfade filters** for crossfade transitions
4. **Synchronize with audio** for perfect timing

### Filter Complex Example

For a 3-image video with animations:
```
[0:v]scale=3840:2160,zoompan=z='if(lte(zoom,1.0),1.0,1.0+(0.0001*on))':d=150:x='iw/2-(iw/zoom/2)':y='ih/2-(ih/zoom/2)':s=1920x1080:fps=30[v0];
[1:v]scale=3840:2160,zoompan=z='if(lte(zoom,1.0),1.2,1.2+(-0.0001*on))':d=150:x='iw/2-(iw/zoom/2)':y='ih/2-(ih/zoom/2)':s=1920x1080:fps=30[v1];
[2:v]scale=3840:2160,zoompan=z='if(lte(zoom,1.0),1.0,1.0+(0.0001*on))':d=150:x='iw/2-(iw/zoom/2)':y='ih/2-(ih/zoom/2)':s=1920x1080:fps=30[v2];
[v0][v1]xfade=transition=fade:duration=1.0:offset=4.0[vx1];
[vx1][v2]xfade=transition=fade:duration=1.0:offset=9.0[outv]
```

---

## Best Practices

1. **Match animation style to content**
   - Educational: Subtle animations (1.1-1.2x zoom)
   - Entertainment: Moderate animations (1.2-1.3x zoom)
   - Cinematic: Dramatic animations (1.3-1.5x zoom)

2. **Consider video length**
   - Short videos (<30s): Faster transitions (0.5s)
   - Medium videos (30-120s): Standard transitions (1.0s)
   - Long videos (>120s): Slower transitions (1.5-2.0s)

3. **Test different settings**
   - Generate a short test video first
   - Adjust settings based on results
   - Find the sweet spot for your content style

4. **Balance quality and performance**
   - Use GPU acceleration when possible
   - Don't over-zoom (keep under 1.5x)
   - Match frame rate to content needs

---

## Future Enhancements

Planned features for future releases:

- [ ] Multiple transition types (wipe, slide, dissolve)
- [ ] Custom zoom patterns per image
- [ ] Text overlay animations
- [ ] Audio-reactive animations
- [ ] Particle effects and overlays
- [ ] 3D transitions

---

## Support

If you encounter issues with animations:

1. Check the [TROUBLESHOOTING.md](TROUBLESHOOTING.md) guide
2. Verify your FFmpeg installation supports required filters
3. Review the generation logs for error messages
4. Try disabling animations to isolate the issue

For more help, see the main [README.md](README.md) documentation.
