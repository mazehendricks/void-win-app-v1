namespace VoidVideoGenerator.Models;

/// <summary>
/// Video output configuration settings
/// </summary>
public class VideoOutputSettings
{
    // Resolution settings
    public string Resolution { get; set; } = "1080p";
    public int Width { get; set; } = 1920;
    public int Height { get; set; } = 1080;
    
    // Quality settings
    public string QualityPreset { get; set; } = "Medium";
    public int VideoBitrate { get; set; } = 5000; // kbps
    
    // Frame rate
    public int FrameRate { get; set; } = 30;
    
    // Format settings
    public string VideoFormat { get; set; } = "mp4";
    public string VideoCodec { get; set; } = "h264";
    
    // Audio settings
    public int AudioBitrate { get; set; } = 192; // kbps
    public int AudioSampleRate { get; set; } = 48000; // Hz
    public string AudioChannels { get; set; } = "stereo";
    
    // Animation settings
    public bool EnableKenBurnsEffect { get; set; } = true;
    public bool EnableCrossfadeTransitions { get; set; } = true;
    public double TransitionDuration { get; set; } = 1.0; // seconds
    public double ZoomIntensity { get; set; } = 1.2; // 1.0 = no zoom, 1.5 = 50% zoom
    
    /// <summary>
    /// Apply resolution preset
    /// </summary>
    public void SetResolution(string preset)
    {
        Resolution = preset;
        switch (preset.ToLower())
        {
            case "720p":
                Width = 1280;
                Height = 720;
                break;
            case "1080p":
                Width = 1920;
                Height = 1080;
                break;
            case "1440p":
            case "2k":
                Width = 2560;
                Height = 1440;
                break;
            case "4k":
            case "2160p":
                Width = 3840;
                Height = 2160;
                break;
            default:
                Width = 1920;
                Height = 1080;
                break;
        }
    }
    
    /// <summary>
    /// Apply quality preset
    /// </summary>
    public void SetQualityPreset(string preset)
    {
        QualityPreset = preset;
        switch (preset.ToLower())
        {
            case "low":
                VideoBitrate = 2500;
                AudioBitrate = 128;
                break;
            case "medium":
                VideoBitrate = 5000;
                AudioBitrate = 192;
                break;
            case "high":
                VideoBitrate = 8000;
                AudioBitrate = 256;
                break;
            case "ultra":
                VideoBitrate = 15000;
                AudioBitrate = 320;
                break;
            default:
                VideoBitrate = 5000;
                AudioBitrate = 192;
                break;
        }
    }
}
