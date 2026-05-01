namespace VoidVideoGenerator.Services;

using VoidVideoGenerator.Models;

/// <summary>
/// Interface for AI video generation services
/// </summary>
public interface IAIVideoGeneratorService
{
    /// <summary>
    /// Generate a video from a prompt
    /// </summary>
    /// <param name="prompt">Video generation prompt</param>
    /// <param name="progress">Optional progress callback</param>
    /// <returns>Path to the generated video file</returns>
    Task<string> GenerateVideoAsync(VideoPrompt prompt, IProgress<int>? progress = null);
    
    /// <summary>
    /// Check if the service is available and configured
    /// </summary>
    Task<bool> IsAvailableAsync();
    
    /// <summary>
    /// Get the status of a generation job
    /// </summary>
    /// <param name="jobId">Job identifier</param>
    Task<VideoGenerationStatus> GetStatusAsync(string jobId);
    
    /// <summary>
    /// Cancel a running generation job
    /// </summary>
    /// <param name="jobId">Job identifier</param>
    Task CancelGenerationAsync(string jobId);
    
    /// <summary>
    /// Get the name of this provider
    /// </summary>
    string ProviderName { get; }
    
    /// <summary>
    /// Get the maximum supported duration in seconds
    /// </summary>
    int MaxDuration { get; }
    
    /// <summary>
    /// Check if this provider supports image-to-video
    /// </summary>
    bool SupportsImageToVideo { get; }
}
