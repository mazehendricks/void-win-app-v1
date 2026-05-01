namespace VoidVideoGenerator.Services;

using System.Net.Http;
using System.Text;
using System.Text.Json;
using VoidVideoGenerator.Models;

/// <summary>
/// Script generator using OpenAI API (GPT-4, GPT-3.5, etc.)
/// </summary>
public class OpenAIScriptGenerator : IScriptGeneratorService
{
    private readonly string _apiKey;
    private readonly string _model;
    private readonly HttpClient _httpClient;
    private const string API_URL = "https://api.openai.com/v1/chat/completions";

    public OpenAIScriptGenerator(string apiKey, string model = "gpt-4")
    {
        _apiKey = apiKey;
        _model = model;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
    }

    public async Task<bool> IsAvailableAsync()
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.openai.com/v1/models");
            request.Headers.Add("Authorization", $"Bearer {_apiKey}");
            
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<VideoScript> GenerateScriptAsync(
        VideoRequest request,
        ChannelDNA channelDNA,
        IProgress<string>? progress = null)
    {
        progress?.Report($"Generating script with OpenAI {_model}...");

        var systemPrompt = BuildSystemPrompt(channelDNA);
        var userPrompt = BuildUserPrompt(request);

        var requestBody = new
        {
            model = _model,
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userPrompt }
            },
            temperature = 0.7,
            max_tokens = 2000
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(API_URL, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"OpenAI API error: {response.StatusCode} - {responseContent}");
            }

            var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
            var scriptText = result
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "";

            progress?.Report("Parsing script...");
            return ParseScript(scriptText);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to generate script with OpenAI: {ex.Message}", ex);
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
