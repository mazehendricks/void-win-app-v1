namespace VoidVideoGenerator.Services;

using System.Net.Http;
using System.Text;
using System.Text.Json;
using VoidVideoGenerator.Models;

/// <summary>
/// Script generator using Google Gemini API
/// </summary>
public class GeminiScriptGenerator : IScriptGeneratorService
{
    private readonly string _apiKey;
    private readonly string _model;
    private readonly HttpClient _httpClient;
    private const string API_URL_TEMPLATE = "https://generativelanguage.googleapis.com/v1beta/models/{0}:generateContent?key={1}";

    public GeminiScriptGenerator(string apiKey, string model = "gemini-1.5-pro")
    {
        _apiKey = apiKey;
        _model = model;
        _httpClient = new HttpClient();
    }

    public async Task<bool> IsAvailableAsync()
    {
        try
        {
            var url = string.Format(API_URL_TEMPLATE, _model, _apiKey);
            var testRequest = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[] { new { text = "Hi" } }
                    }
                }
            };

            var json = JsonSerializer.Serialize(testRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync(url, content);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<VideoScript> GenerateScriptAsync(
        VideoRequest request,
        IProgress<string>? progress = null)
    {
        var channelDNA = new ChannelDNA(); // Use default if not provided
        return await GenerateScriptWithChannelDNAAsync(request, channelDNA, progress);
    }

    private async Task<VideoScript> GenerateScriptWithChannelDNAAsync(
        VideoRequest request,
        ChannelDNA channelDNA,
        IProgress<string>? progress = null)
    {
        progress?.Report($"Generating script with Google {_model}...");

        var systemPrompt = BuildSystemPrompt(channelDNA);
        var userPrompt = BuildUserPrompt(request);
        var fullPrompt = $"{systemPrompt}\n\n{userPrompt}";

        var url = string.Format(API_URL_TEMPLATE, _model, _apiKey);
        
        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[] { new { text = fullPrompt } }
                }
            },
            generationConfig = new
            {
                temperature = 0.7,
                maxOutputTokens = 2000
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Gemini API error: {response.StatusCode} - {responseContent}");
            }

            var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
            var scriptText = result
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? "";

            progress?.Report("Parsing script...");
            return ParseScript(scriptText);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to generate script with Gemini: {ex.Message}", ex);
        }
    }

    private string BuildSystemPrompt(ChannelDNA channelDNA)
    {
        return $@"You are a professional video script writer for a {channelDNA.Niche} channel.

Channel DNA:
- Niche: {channelDNA.Niche}
- Host Persona: {channelDNA.HostPersona}
- Tone: {channelDNA.ToneGuidelines}
- Target Audience: {channelDNA.TargetAudience}
- Content Style: {channelDNA.ContentStyle}

Your task is to write engaging video scripts that:
1. Hook viewers in the first 5 seconds
2. Deliver value throughout
3. Include visual cues in [brackets] for image selection
4. Use natural, conversational language
5. Match the channel's tone and style

Format your response as:
HOOK: [Opening hook]
MAIN: [Main content with [visual cues]]
CONCLUSION: [Closing statement]";
    }

    private string BuildUserPrompt(VideoRequest request)
    {
        var prompt = new StringBuilder();
        prompt.AppendLine($"Write a {request.TargetDurationSeconds}-second video script about: {request.Topic}");
        prompt.AppendLine($"Title: {request.Title}");
        
        if (!string.IsNullOrEmpty(request.Description))
        {
            prompt.AppendLine($"Description: {request.Description}");
        }

        if (request.Keywords != null && request.Keywords.Count > 0)
        {
            prompt.AppendLine($"Keywords to include: {string.Join(", ", request.Keywords)}");
        }

        prompt.AppendLine("\nInclude [visual cues] in brackets throughout the script to indicate what images should be shown.");
        prompt.AppendLine("Make it engaging, informative, and perfectly timed for the target duration.");

        return prompt.ToString();
    }

    private VideoScript ParseScript(string scriptText)
    {
        var script = new VideoScript();
        var lines = scriptText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        
        string currentSection = "";
        var sectionText = new StringBuilder();

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            
            if (trimmedLine.StartsWith("HOOK:", StringComparison.OrdinalIgnoreCase))
            {
                if (sectionText.Length > 0)
                {
                    AddSegment(script, currentSection, sectionText.ToString());
                    sectionText.Clear();
                }
                currentSection = "HOOK";
                sectionText.AppendLine(trimmedLine.Substring(5).Trim());
            }
            else if (trimmedLine.StartsWith("MAIN:", StringComparison.OrdinalIgnoreCase))
            {
                if (sectionText.Length > 0)
                {
                    AddSegment(script, currentSection, sectionText.ToString());
                    sectionText.Clear();
                }
                currentSection = "MAIN";
                sectionText.AppendLine(trimmedLine.Substring(5).Trim());
            }
            else if (trimmedLine.StartsWith("CONCLUSION:", StringComparison.OrdinalIgnoreCase))
            {
                if (sectionText.Length > 0)
                {
                    AddSegment(script, currentSection, sectionText.ToString());
                    sectionText.Clear();
                }
                currentSection = "CONCLUSION";
                sectionText.AppendLine(trimmedLine.Substring(11).Trim());
            }
            else if (!string.IsNullOrWhiteSpace(trimmedLine))
            {
                sectionText.AppendLine(trimmedLine);
            }
        }

        // Add final section
        if (sectionText.Length > 0)
        {
            AddSegment(script, currentSection, sectionText.ToString());
        }

        // Extract visual cues
        ExtractVisualCues(script);

        return script;
    }

    private void AddSegment(VideoScript script, string type, string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;

        var segment = new ScriptSegment
        {
            Type = type.ToUpper() switch
            {
                "HOOK" => "Hook",
                "MAIN" => "Main",
                "CONCLUSION" => "Conclusion",
                _ => "Main"
            },
            Text = text.Trim()
        };

        script.Segments.Add(segment);
    }

    private void ExtractVisualCues(VideoScript script)
    {
        var visualCueRegex = new System.Text.RegularExpressions.Regex(@"\[([^\]]+)\]");
        
        foreach (var segment in script.Segments)
        {
            var matches = visualCueRegex.Matches(segment.Text);
            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                var cue = match.Groups[1].Value.Trim();
                if (!string.IsNullOrWhiteSpace(cue) && !script.VisualCues.Contains(cue))
                {
                    script.VisualCues.Add(cue);
                }
            }
        }
    }
}
