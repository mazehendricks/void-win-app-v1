namespace VoidVideoGenerator.Services;

using System.Text.Json;
using System.Text;
using VoidVideoGenerator.Models;

/// <summary>
/// Script generator using local Ollama LLM
/// </summary>
public class OllamaScriptGenerator : IScriptGeneratorService, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly string _model;
    private bool _disposed = false;
    private static readonly System.Text.RegularExpressions.Regex VisualCueRegex =
        new System.Text.RegularExpressions.Regex(@"\[([^\]]+)\]", System.Text.RegularExpressions.RegexOptions.Compiled);

    public OllamaScriptGenerator(string baseUrl = "http://localhost:11434", string model = "llama3.1")
    {
        _baseUrl = baseUrl;
        _model = model;
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromMinutes(5)
        };
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _httpClient?.Dispose();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }

    public async Task<bool> IsAvailableAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/tags");
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            // Verify the model is available
            var responseJson = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<OllamaTagsResponse>(responseJson, options);
            
            // Check if our model is in the list
            var modelAvailable = result?.Models?.Any(m => m.Name?.StartsWith(_model) == true) ?? false;
            
            return modelAvailable;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Get detailed status information for diagnostics
    /// </summary>
    public async Task<string> GetDiagnosticInfoAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/tags");
            if (!response.IsSuccessStatusCode)
            {
                return $"❌ Ollama not responding at {_baseUrl} (Status: {response.StatusCode})";
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<OllamaTagsResponse>(responseJson, options);

            if (result?.Models == null || !result.Models.Any())
            {
                return $"⚠️ Ollama is running but no models are installed.\nRun: ollama pull {_model}";
            }

            var modelNames = string.Join(", ", result.Models.Select(m => m.Name));
            var modelAvailable = result.Models.Any(m => m.Name?.StartsWith(_model) == true);

            if (modelAvailable)
            {
                return $"✓ Ollama is ready\n  URL: {_baseUrl}\n  Model: {_model}\n  Available models: {modelNames}";
            }
            else
            {
                return $"⚠️ Model '{_model}' not found\n  Available models: {modelNames}\n  Run: ollama pull {_model}";
            }
        }
        catch (HttpRequestException ex)
        {
            return $"❌ Cannot connect to Ollama at {_baseUrl}\n  Error: {ex.Message}\n  Make sure Ollama is running: ollama serve";
        }
        catch (Exception ex)
        {
            return $"❌ Error checking Ollama: {ex.Message}";
        }
    }

    public async Task<VideoScript> GenerateScriptAsync(VideoRequest request, IProgress<string>? progress = null)
    {
        progress?.Report("Generating script with local LLM...");
        
        // Pre-flight check: Verify Ollama is accessible
        progress?.Report($"Checking Ollama connection at {_baseUrl}...");
        try
        {
            var testResponse = await _httpClient.GetAsync($"{_baseUrl}/api/tags");
            if (!testResponse.IsSuccessStatusCode)
            {
                throw new Exception($"Ollama is not responding properly at {_baseUrl}. Status: {testResponse.StatusCode}");
            }
            progress?.Report("✓ Ollama connection verified");
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Cannot connect to Ollama at {_baseUrl}. Make sure Ollama is running (ollama serve). Error: {ex.Message}");
        }
        catch (TaskCanceledException)
        {
            throw new Exception($"Connection to Ollama at {_baseUrl} timed out. Is Ollama running?");
        }

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

        progress?.Report($"Sending request to Ollama (model: {_model})...");
        progress?.Report($"Prompt length: {json.Length} bytes");
        progress?.Report("Waiting for LLM response (this may take 1-5 minutes)...");
        
        OllamaResponse result;
        try
        {
            var response = await _httpClient.PostAsync($"{_baseUrl}/api/generate", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            progress?.Report($"Received response ({responseJson.Length} chars), parsing...");
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            result = JsonSerializer.Deserialize<OllamaResponse>(responseJson, options)!;

            if (result?.Response == null)
            {
                throw new Exception($"Failed to generate script from LLM. Response: {responseJson.Substring(0, Math.Min(200, responseJson.Length))}");
            }
            
            progress?.Report($"Script generated successfully ({result.Response.Length} chars)");
        }
        catch (TaskCanceledException)
        {
            throw new Exception("LLM request timed out. The model might be too slow or not responding.");
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Failed to connect to Ollama at {_baseUrl}: {ex.Message}");
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
        var visualCues = VisualCueRegex.Matches(scriptText);
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
            VisualCues = VisualCueRegex.Matches(text)
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

    private class OllamaTagsResponse
    {
        public List<OllamaModel>? Models { get; set; }
    }

    private class OllamaModel
    {
        public string? Name { get; set; }
        public long Size { get; set; }
    }
}
