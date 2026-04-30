namespace VoidVideoGenerator.Services;

using VoidVideoGenerator.Models;

/// <summary>
/// Interface for generating video scripts using local LLM
/// </summary>
public interface IScriptGeneratorService
{
    /// <summary>
    /// Generate a video script based on the request
    /// </summary>
    Task<VideoScript> GenerateScriptAsync(VideoRequest request, IProgress<string>? progress = null);
    
    /// <summary>
    /// Check if the local LLM service is available
    /// </summary>
    Task<bool> IsAvailableAsync();
}
