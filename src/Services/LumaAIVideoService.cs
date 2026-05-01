namespace VoidVideoGenerator.Services;

using System.Net.Http;
using System.Net.Http.Json;
using VoidVideoGenerator.Models;

/// <summary>
/// Luma AI Dream Machine video generation service
/// </summary>
public class LumaAIVideoService : IAIVideoGeneratorService, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly LumaAIConfig _config;
    private bool _disposed;

    public string ProviderName => "Luma AI Dream Machine";
    public int MaxDuration => _config.MaxDuration;
    public bool SupportsImageToVideo => true;

    public LumaAIVideoService(LumaAIConfig config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_config.BaseUrl),
            Timeout = TimeSpan.FromSeconds(_config.TimeoutSeconds)
        };
        
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config.ApiKey}");
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    public async Task<string> GenerateVideoAsync(VideoPrompt prompt, IProgress<int>? progress = null)
    {
        if (string.IsNullOrWhiteSpace(_config.ApiKey))
            throw new InvalidOperationException("Luma AI API key is not configured");

        // Validate duration
        var duration = Math.Min(prompt.Duration, MaxDuration);
        
        // Build request
        var request = new
        {
            prompt = prompt.Description,
            keyframes = new
            {
                frame0 = new
                {
                    type = string.IsNullOrEmpty(prompt.ReferenceImagePath) ? "generation" : "image",
                    url = prompt.ReferenceImagePath
                }
            },
            aspect_ratio = prompt.AspectRatio,
            loop = false,
            extend_prompt = true
        };

        progress?.Report(5);

        // Submit generation request
        var response = await _httpClient.PostAsJsonAsync("/dream-machine/v1/generations", request);
        
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Luma AI API error: {response.StatusCode} - {error}");
        }

        var result = await response.Content.ReadFromJsonAsync<LumaAIResponse>();
        if (result == null || string.IsNullOrEmpty(result.Id))
            throw new InvalidOperationException("Invalid response from Luma AI API");

        progress?.Report(10);

        // Poll for completion
        var videoUrl = await PollForCompletionAsync(result.Id, progress);

        progress?.Report(90);

        // Download video
        var localPath = await DownloadVideoAsync(videoUrl);

        progress?.Report(100);

        return localPath;
    }

    public async Task<bool> IsAvailableAsync()
    {
        if (string.IsNullOrWhiteSpace(_config.ApiKey))
            return false;

        try
        {
            // Try to get account info as health check
            var response = await _httpClient.GetAsync("/dream-machine/v1/account");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<VideoGenerationStatus> GetStatusAsync(string jobId)
    {
        var response = await _httpClient.GetAsync($"/dream-machine/v1/generations/{jobId}");
        
        if (!response.IsSuccessStatusCode)
        {
            return new VideoGenerationStatus
            {
                JobId = jobId,
                Status = "failed",
                ErrorMessage = $"Failed to get status: {response.StatusCode}"
            };
        }

        var result = await response.Content.ReadFromJsonAsync<LumaAIStatusResponse>();
        
        return new VideoGenerationStatus
        {
            JobId = jobId,
            Status = MapStatus(result?.State ?? "unknown"),
            Progress = CalculateProgress(result?.State ?? "unknown"),
            VideoUrl = result?.Assets?.Video,
            ErrorMessage = result?.FailureReason,
            CreatedAt = result?.CreatedAt ?? DateTime.UtcNow,
            CompletedAt = result?.State == "completed" ? DateTime.UtcNow : null,
            Metadata = new Dictionary<string, object>
            {
                ["provider"] = "LumaAI",
                ["generation_id"] = jobId
            }
        };
    }

    public async Task CancelGenerationAsync(string jobId)
    {
        await _httpClient.DeleteAsync($"/dream-machine/v1/generations/{jobId}");
    }

    private async Task<string> PollForCompletionAsync(string jobId, IProgress<int>? progress)
    {
        var startTime = DateTime.UtcNow;
        var pollInterval = TimeSpan.FromSeconds(3);
        var maxWaitTime = TimeSpan.FromSeconds(_config.TimeoutSeconds);

        while (DateTime.UtcNow - startTime < maxWaitTime)
        {
            var status = await GetStatusAsync(jobId);

            // Update progress (10-90% range during polling)
            if (status.Progress > 0)
            {
                var scaledProgress = 10 + (int)(status.Progress * 0.8);
                progress?.Report(scaledProgress);
            }

            if (status.Status == "completed" && !string.IsNullOrEmpty(status.VideoUrl))
                return status.VideoUrl;

            if (status.Status == "failed")
                throw new Exception($"Video generation failed: {status.ErrorMessage}");

            await Task.Delay(pollInterval);
        }

        throw new TimeoutException($"Video generation timed out after {maxWaitTime.TotalSeconds} seconds");
    }

    private async Task<string> DownloadVideoAsync(string url)
    {
        var videoData = await _httpClient.GetByteArrayAsync(url);
        
        var outputDir = Path.Combine(Path.GetTempPath(), "VoidVideoGenerator", "LumaAI");
        Directory.CreateDirectory(outputDir);
        
        var outputPath = Path.Combine(outputDir, $"luma_{Guid.NewGuid()}.mp4");
        await File.WriteAllBytesAsync(outputPath, videoData);
        
        return outputPath;
    }

    private static string MapStatus(string lumaState)
    {
        return lumaState.ToLowerInvariant() switch
        {
            "queued" => "queued",
            "dreaming" => "processing",
            "completed" => "completed",
            "failed" => "failed",
            _ => "queued"
        };
    }

    private static int CalculateProgress(string lumaState)
    {
        return lumaState.ToLowerInvariant() switch
        {
            "queued" => 10,
            "dreaming" => 50,
            "completed" => 100,
            "failed" => 0,
            _ => 0
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

    // Response models
    private class LumaAIResponse
    {
        public string Id { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
    }

    private class LumaAIStatusResponse
    {
        public string State { get; set; } = string.Empty;
        public LumaAIAssets? Assets { get; set; }
        public string? FailureReason { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    private class LumaAIAssets
    {
        public string Video { get; set; } = string.Empty;
    }
}
