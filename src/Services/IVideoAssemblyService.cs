namespace VoidVideoGenerator.Services;

using VoidVideoGenerator.Models;

/// <summary>
/// Interface for assembling final video from components
/// </summary>
public interface IVideoAssemblyService
{
    /// <summary>
    /// Assemble video from audio files and visual assets
    /// </summary>
    Task<string> AssembleVideoAsync(
        List<string> audioFiles,
        List<string> imageFiles,
        string outputPath,
        IProgress<string>? progress = null);
    
    /// <summary>
    /// Create a complete video from a script
    /// </summary>
    Task<string> CreateVideoFromScriptAsync(
        VideoScript script,
        string audioDirectory,
        string visualsDirectory,
        string outputPath,
        IProgress<string>? progress = null);
    
    /// <summary>
    /// Check if FFmpeg is available
    /// </summary>
    Task<bool> IsAvailableAsync();
}
