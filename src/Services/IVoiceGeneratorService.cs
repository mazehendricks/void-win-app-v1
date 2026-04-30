namespace VoidVideoGenerator.Services;

using VoidVideoGenerator.Models;

/// <summary>
/// Interface for generating voice audio from text
/// </summary>
public interface IVoiceGeneratorService
{
    /// <summary>
    /// Generate audio file from text
    /// </summary>
    Task<string> GenerateAudioAsync(string text, string outputPath, IProgress<string>? progress = null);
    
    /// <summary>
    /// Generate audio for an entire script with segments
    /// </summary>
    Task<List<string>> GenerateScriptAudioAsync(VideoScript script, string outputDirectory, IProgress<string>? progress = null);
    
    /// <summary>
    /// Check if the TTS service is available
    /// </summary>
    Task<bool> IsAvailableAsync();
}
