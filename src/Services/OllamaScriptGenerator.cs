namespace VoidVideoGenerator.Services;

using System.Text.Json;
using System.Text;
using VoidVideoGenerator.Models;

/// <summary>
/// Script generator using local Ollama LLM
/// </summary>
public class OllamaScriptGenerator : IScriptGeneratorService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly string _model;

    public OllamaScriptGenerator(string baseUrl = "http://localhost:11434", string model = "llama3.1")
    {
        _baseUrl = baseUrl;
        _model = model;
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromMinutes(5)
        };
    }

    public async Task<bool> IsAvailableAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/tags");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<VideoScript> GenerateScriptAsync(VideoRequest request, IProgress<string>? progress = null)
    {
        progress?.Report("Generating script with local LLM...");

        var systemPrompt = request.ChannelDNA.ToSystemPrompt();
        var userPrompt = $@"Create a {request.TargetDurationSeconds}-second video script about: {request.Topic}

Title: {request.Title}

Requirements:
- Start with a powerful hook in the first 10 seconds
- Include specific examples and actionable insights
- Add visual cues in [brackets] for what should be shown
- End with a clear call to action
- Avoid generic AI phrases like 'in this video', 'let's dive in', etc.
- Make it feel authentic and human

Format the script with clear sections: HOOK, BODY, and CTA.";

        var requestBody = new
        {
            model = _model,
            prompt = $"{systemPrompt}\n\n{userPrompt}",
            stream = false,
            options = new
            {
                temperature = 0.7,
                top_p = 0.9
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        progress?.Report("Waiting for LLM response...");
        var response = await _httpClient.PostAsync($"{_baseUrl}/api/generate", content);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<OllamaResponse>(responseJson);

        if (result?.Response == null)
        {
            throw new Exception("Failed to generate script from LLM");
        }

        progress?.Report("Parsing script...");
        return ParseScript(result.Response, request);
    }

    private VideoScript ParseScript(string scriptText, VideoRequest request)
    {
        var script = new VideoScript
        {
            Title = request.Title,
            FullText = scriptText,
            EstimatedDurationSeconds = request.TargetDurationSeconds
        };

        // Extract visual cues
        var visualCues = System.Text.RegularExpressions.Regex.Matches(scriptText, @"\[([^\]]+)\]");
        script.VisualCues = visualCues.Select(m => m.Groups[1].Value).ToList();

        // Parse segments (simplified - you can enhance this)
        var lines = scriptText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        string currentSection = "body";
        var currentText = new StringBuilder();
        int currentTime = 0;

        foreach (var line in lines)
        {
            var upperLine = line.ToUpper().Trim();
            
            if (upperLine.StartsWith("HOOK"))
            {
                if (currentText.Length > 0)
                {
                    AddSegment(script, currentSection, currentText.ToString(), ref currentTime);
                    currentText.Clear();
                }
                currentSection = "hook";
                continue;
            }
            else if (upperLine.StartsWith("BODY") || upperLine.StartsWith("CONTENT"))
            {
                if (currentText.Length > 0)
                {
                    AddSegment(script, currentSection, currentText.ToString(), ref currentTime);
                    currentText.Clear();
                }
                currentSection = "body";
                continue;
            }
            else if (upperLine.StartsWith("CTA") || upperLine.StartsWith("CALL TO ACTION"))
            {
                if (currentText.Length > 0)
                {
                    AddSegment(script, currentSection, currentText.ToString(), ref currentTime);
                    currentText.Clear();
                }
                currentSection = "cta";
                continue;
            }

            currentText.AppendLine(line);
        }

        // Add final segment
        if (currentText.Length > 0)
        {
            AddSegment(script, currentSection, currentText.ToString(), ref currentTime);
        }

        return script;
    }

    private void AddSegment(VideoScript script, string type, string text, ref int currentTime)
    {
        // Estimate duration based on word count (average speaking rate: 150 words/minute)
        var wordCount = text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        var duration = (int)Math.Ceiling(wordCount / 2.5); // ~150 words per minute

        var segment = new ScriptSegment
        {
            Type = type,
            Text = text.Trim(),
            StartTime = currentTime,
            Duration = duration,
            VisualCues = System.Text.RegularExpressions.Regex.Matches(text, @"\[([^\]]+)\]")
                .Select(m => m.Groups[1].Value)
                .ToList()
        };

        script.Segments.Add(segment);
        currentTime += duration;
    }

    private class OllamaResponse
    {
        public string? Response { get; set; }
    }
}
