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
    
    public ChannelDNA DefaultChannelDNA { get; set; } = new()
    {
        Niche = "Educational",
        HostPersona = "Friendly expert",
        ToneGuidelines = "Conversational, clear, engaging",
        TargetAudience = "General audience",
        ContentStyle = "Informative with storytelling"
    };
}
