namespace VoidVideoGenerator.Models;

/// <summary>
/// Configuration for AI video generation providers
/// </summary>
public class AIVideoConfig
{
    /// <summary>
    /// Selected provider: "RunwayML", "LumaAI", "AnimateDiff", "Hybrid", "None"
    /// </summary>
    public string Provider { get; set; } = "None";
    
    /// <summary>
    /// Runway ML configuration
    /// </summary>
    public RunwayMLConfig? RunwayML { get; set; }
    
    /// <summary>
    /// Luma AI configuration
    /// </summary>
    public LumaAIConfig? LumaAI { get; set; }
    
    /// <summary>
    /// AnimateDiff local configuration
    /// </summary>
    public AnimateDiffConfig? AnimateDiff { get; set; }
    
    /// <summary>
    /// Hybrid generation configuration
    /// </summary>
    public HybridConfig? Hybrid { get; set; }
    
    /// <summary>
    /// Default settings for all providers
    /// </summary>
    public DefaultVideoSettings DefaultSettings { get; set; } = new();
}

public class RunwayMLConfig
{
    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://api.runwayml.com/v1";
    public int MaxDuration { get; set; } = 10;
    public string DefaultModel { get; set; } = "gen3";
    public int TimeoutSeconds { get; set; } = 300;
}

public class LumaAIConfig
{
    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://api.lumalabs.ai/v1";
    public int MaxDuration { get; set; } = 5;
    public int TimeoutSeconds { get; set; } = 300;
}

public class AnimateDiffConfig
{
    public string ComfyUIEndpoint { get; set; } = "http://localhost:8188";
    public string ModelPath { get; set; } = "models/animatediff/mm_sd_v15_v2.ckpt";
    public string CheckpointPath { get; set; } = "models/checkpoints/realisticVisionV51.safetensors";
    public int Steps { get; set; } = 20;
    public float CFG { get; set; } = 7.5f;
    public string Sampler { get; set; } = "euler_ancestral";
    public int TimeoutSeconds { get; set; } = 600;
}

public class HybridConfig
{
    public string ImageModelPath { get; set; } = string.Empty;
    public string InterpolationMethod { get; set; } = "RIFE"; // "RIFE" or "FILM"
    public int KeyframeInterval { get; set; } = 12; // frames between keyframes
    public int TargetFPS { get; set; } = 24;
}

public class DefaultVideoSettings
{
    public string AspectRatio { get; set; } = "16:9";
    public float MotionIntensity { get; set; } = 5.0f;
    public string Style { get; set; } = "realistic";
    public string NegativePrompt { get; set; } = "blurry, low quality, distorted, watermark, text";
    public int DefaultDuration { get; set; } = 4;
}
