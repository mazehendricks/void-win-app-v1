namespace VoidVideoGenerator.Services;

/// <summary>
/// Interface for audio/video transcription services
/// </summary>
public interface ITranscriptionService
{
    /// <summary>
    /// Check if the transcription service is available
    /// </summary>
    Task<bool> IsAvailableAsync();

    /// <summary>
    /// Transcribe audio/video file to text with timestamps
    /// </summary>
    /// <param name="audioOrVideoPath">Path to audio or video file</param>
    /// <param name="progress">Progress reporter</param>
    /// <returns>List of transcription segments with timestamps</returns>
    Task<List<TranscriptionSegment>> TranscribeAsync(string audioOrVideoPath, IProgress<string>? progress = null);
}

/// <summary>
/// Represents a transcription segment with timing information
/// </summary>
public class TranscriptionSegment
{
    public string Text { get; set; } = "";
    public double StartTime { get; set; } // in seconds
    public double EndTime { get; set; } // in seconds
    public double Duration => EndTime - StartTime;
}
