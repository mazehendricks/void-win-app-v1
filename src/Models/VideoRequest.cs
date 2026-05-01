namespace VoidVideoGenerator.Models;

/// <summary>
/// Represents a request to generate a video
/// </summary>
public class VideoRequest
{
    public string Title { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Keywords { get; set; } = new();
    public ChannelDNA ChannelDNA { get; set; } = new();
    public int TargetDurationSeconds { get; set; } = 60;
    public string OutputPath { get; set; } = "output";
    public List<string>? VisualImagePaths { get; set; } // For user-provided images
    public VideoOutputSettings OutputSettings { get; set; } = new();
}

/// <summary>
/// Channel DNA defines the unique voice, style, and persona of the content
/// </summary>
public class ChannelDNA
{
    public string Niche { get; set; } = "Educational";
    public string HostPersona { get; set; } = "Friendly expert";
    public string ToneGuidelines { get; set; } = "Conversational, clear, engaging";
    public string TargetAudience { get; set; } = "General audience";
    public string ContentStyle { get; set; } = "Informative with storytelling";
    
    /// <summary>
    /// Converts Channel DNA to a system prompt for the LLM
    /// </summary>
    public string ToSystemPrompt()
    {
        return $@"You are a {HostPersona} creating content for a {Niche} channel.

TARGET AUDIENCE: {TargetAudience}
TONE: {ToneGuidelines}
STYLE: {ContentStyle}

Your goal is to create engaging, original content that provides real value. Avoid generic AI phrases and clichés.
Focus on specific examples, actionable insights, and unique perspectives.

Structure your scripts with:
1. HOOK (first 10 seconds) - Grab attention immediately
2. PROMISE - What will the viewer learn?
3. CONTENT - Deliver on the promise with clear explanations
4. CALL TO ACTION - What should they do next?

Include visual cues in [brackets] to indicate what should be shown on screen.";
    }
}
