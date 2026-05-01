namespace VoidVideoGenerator.Models;

/// <summary>
/// Application configuration
/// </summary>
public class AppConfig
{
    public string OllamaUrl { get; set; } = "http://localhost:11434";
    public string OllamaModel { get; set; } = "llama3.1";
    public string PiperPath { get; set; } = "piper";
    public string PiperModelPath { get; set; } = "models/voice.onnx";
    public string FFmpegPath { get; set; } = "ffmpeg";
    public string DefaultOutputPath { get; set; } = "output";
    
    /// <summary>
    /// Enable GPU acceleration for video encoding (requires compatible GPU and FFmpeg build)
    /// </summary>
    public bool UseGpuAcceleration { get; set; } = false;
    
    /// <summary>
    /// GPU encoder to use: nvidia (NVENC), amd (AMF), intel (QSV), or auto
    /// </summary>
    public string GpuEncoder { get; set; } = "auto";
    
    public ChannelDNA DefaultChannelDNA { get; set; } = new()
    {
        Niche = "Educational",
        HostPersona = "Friendly expert",
        ToneGuidelines = "Conversational, clear, engaging",
        TargetAudience = "General audience",
        ContentStyle = "Informative with storytelling"
    };
}
