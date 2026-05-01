namespace VoidVideoGenerator.Models;

/// <summary>
/// Application configuration
/// </summary>
public class AppConfig
{
    // AI Model Provider Selection
    public string AiProvider { get; set; } = "ollama"; // ollama, openai, anthropic, gemini
    
    // Ollama Settings (Local)
    public string OllamaUrl { get; set; } = "http://localhost:11434";
    public string OllamaModel { get; set; } = "llama3.1";
    
    // OpenAI Settings
    public string OpenAiApiKey { get; set; } = "";
    public string OpenAiModel { get; set; } = "gpt-4";
    
    // Anthropic Settings
    public string AnthropicApiKey { get; set; } = "";
    public string AnthropicModel { get; set; } = "claude-3-5-sonnet-20241022";
    
    // Google Gemini Settings
    public string GeminiApiKey { get; set; } = "";
    public string GeminiModel { get; set; } = "gemini-1.5-pro";
    
    // TTS and Video Tools
    public string PiperPath { get; set; } = "C:\\Tools\\piper\\piper.exe";
    public string PiperModelPath { get; set; } = "C:\\Tools\\piper\\models\\voice.onnx";
    public string FFmpegPath { get; set; } = "ffmpeg";
    public string DefaultOutputPath { get; set; } = "output";
    
    /// <summary>
    /// Unsplash API Access Key for automatic image generation
    /// Get your free API key at: https://unsplash.com/developers
    /// </summary>
    public string UnsplashApiKey { get; set; } = "";
    
    /// <summary>
    /// Enable automatic image generation from Unsplash
    /// </summary>
    public bool UseUnsplashImages { get; set; } = false;
    
    /// <summary>
    /// Enable dark mode theme
    /// </summary>
    public bool DarkMode { get; set; } = false;
    
    /// <summary>
    /// Enable GPU acceleration for video encoding (requires compatible GPU and FFmpeg build)
    /// </summary>
    public bool UseGpuAcceleration { get; set; } = false;
    
    /// <summary>
    /// GPU encoder to use: nvidia (NVENC), amd (AMF), intel (QSV), or auto
    /// </summary>
    public string GpuEncoder { get; set; } = "auto";
    
    /// <summary>
    /// Video output settings
    /// </summary>
    public VideoOutputSettings VideoSettings { get; set; } = new();
    
    public ChannelDNA DefaultChannelDNA { get; set; } = new()
    {
        Niche = "Educational",
        HostPersona = "Friendly expert",
        ToneGuidelines = "Conversational, clear, engaging",
        TargetAudience = "General audience",
        ContentStyle = "Informative with storytelling"
    };
}
