namespace VoidVideoGenerator.Models;

/// <summary>
/// Represents a prompt for AI video generation
/// </summary>
public class VideoPrompt
{
    /// <summary>
    /// Main text/description of what to generate (alias for Description)
    /// </summary>
    public string Text
    {
        get => Description;
        set => Description = value;
    }
    
    /// <summary>
    /// Main description of what to generate
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Shot type: "close-up", "medium", "wide", "extreme-close-up", "establishing"
    /// </summary>
    public string? ShotType { get; set; }
    
    /// <summary>
    /// Lighting style: "natural", "dramatic", "soft", "hard", "golden-hour", "blue-hour"
    /// </summary>
    public string? Lighting { get; set; }
    
    /// <summary>
    /// Things to avoid in the generation
    /// </summary>
    public string? NegativePrompt { get; set; }
    
    /// <summary>
    /// Duration in seconds (typically 2-10 seconds per clip)
    /// </summary>
    public int Duration { get; set; } = 4;
    
    /// <summary>
    /// Aspect ratio: "16:9", "9:16", "1:1", "4:3"
    /// </summary>
    public string AspectRatio { get; set; } = "16:9";
    
    /// <summary>
    /// Seed for reproducibility (null for random)
    /// </summary>
    public int? Seed { get; set; }
    
    /// <summary>
    /// Motion intensity (0-10, where 0 is static and 10 is very dynamic)
    /// </summary>
    public float MotionIntensity { get; set; } = 5.0f;
    
    /// <summary>
    /// Visual style: "realistic", "cinematic", "animated", "artistic"
    /// </summary>
    public string Style { get; set; } = "realistic";
    
    /// <summary>
    /// Camera motion: "static", "pan_left", "pan_right", "zoom_in", "zoom_out", "orbit"
    /// </summary>
    public string? CameraMotion { get; set; }
    
    /// <summary>
    /// Optional reference image path for image-to-video generation
    /// </summary>
    public string? ReferenceImagePath { get; set; }
    
    /// <summary>
    /// Width in pixels (if supported by provider)
    /// </summary>
    public int? Width { get; set; }
    
    /// <summary>
    /// Height in pixels (if supported by provider)
    /// </summary>
    public int? Height { get; set; }
    
    /// <summary>
    /// Frame rate (if supported by provider)
    /// </summary>
    public int? FrameRate { get; set; }
    
    /// <summary>
    /// Provider-specific custom parameters
    /// </summary>
    public Dictionary<string, object>? CustomParameters { get; set; }
}
