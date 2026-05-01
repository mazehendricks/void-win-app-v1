namespace VoidVideoGenerator.Models;

/// <summary>
/// Represents the status of an AI video generation job
/// </summary>
public class VideoGenerationStatus
{
    /// <summary>
    /// Unique job identifier
    /// </summary>
    public string JobId { get; set; } = string.Empty;
    
    /// <summary>
    /// Current status: "queued", "processing", "completed", "failed", "cancelled"
    /// </summary>
    public string Status { get; set; } = "queued";
    
    /// <summary>
    /// Progress percentage (0-100)
    /// </summary>
    public int Progress { get; set; }
    
    /// <summary>
    /// URL or path to the generated video (when completed)
    /// </summary>
    public string? VideoUrl { get; set; }
    
    /// <summary>
    /// Local file path (after download)
    /// </summary>
    public string? LocalPath { get; set; }
    
    /// <summary>
    /// Error message (if failed)
    /// </summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// Estimated time remaining in seconds
    /// </summary>
    public int? EstimatedTimeRemaining { get; set; }
    
    /// <summary>
    /// When the job was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// When the job was completed
    /// </summary>
    public DateTime? CompletedAt { get; set; }
    
    /// <summary>
    /// Additional metadata from the provider
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }
}
