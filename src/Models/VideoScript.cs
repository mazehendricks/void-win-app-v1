namespace VoidVideoGenerator.Models;

/// <summary>
/// Represents a generated video script with timing and visual cues
/// </summary>
public class VideoScript
{
    public string Title { get; set; } = string.Empty;
    public string FullText { get; set; } = string.Empty;
    public List<ScriptSegment> Segments { get; set; } = new();
    public List<string> VisualCues { get; set; } = new();
    public int EstimatedDurationSeconds { get; set; }
}

/// <summary>
/// A segment of the script (hook, body, CTA, etc.)
/// </summary>
public class ScriptSegment
{
    public string Type { get; set; } = string.Empty; // "hook", "body", "cta"
    public string Text { get; set; } = string.Empty;
    public int StartTime { get; set; } // in seconds
    public int Duration { get; set; } // in seconds
    public List<string> VisualCues { get; set; } = new();
}
