namespace VoidVideoGenerator.Services;

using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using VoidVideoGenerator.Models;

/// <summary>
/// Runway ML Gen-3 video generation service
/// </summary>
public class RunwayMLVideoService : IAIVideoGeneratorService, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly RunwayMLConfig _config;
    private bool _disposed;

    public string ProviderName => "Runway ML Gen-3";
    public int MaxDuration => _config.MaxDuration;
    public bool SupportsImageToVideo => true;

    public RunwayMLVideoService(RunwayMLConfig config)
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
            throw new InvalidOperationException("Runway ML API key is not configured");

        // Validate duration
        var duration = Math.Min(prompt.Duration, MaxDuration);
        
        // Build request
        var request = new
        {
            text_prompt = prompt.Description,
            image_prompt = prompt.ReferenceImagePath,
            duration = duration,
            aspect_ratio = prompt.AspectRatio,
            seed = prompt.Seed,
            motion_amount = (int)Math.Round(prompt.MotionIntensity),
            style = prompt.Style,
            camera_motion = prompt.CameraMotion,
            model = _config.DefaultModel,
            negative_prompt = prompt.NegativePrompt
        };

        progress?.Report(5);

        // Submit generation request
        var response = await _httpClient.PostAsJsonAsync("/generations", request);
        
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Runway ML API error: {response.StatusCode} - {error}");
        }

        var result = await response.Content.ReadFromJsonAsync<RunwayMLResponse>();
        if (result == null || string.IsNullOrEmpty(result.Id))
            throw new InvalidOperationException("Invalid response from Runway ML API");

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
            var response = await _httpClient.GetAsync("/health");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<VideoGenerationStatus> GetStatusAsync(string jobId)
    {
        var response = await _httpClient.GetAsync($"/generations/{jobId}");
        
        if (!response.IsSuccessStatusCode)
        {
            return new VideoGenerationStatus
            {
                JobId = jobId,
                Status = "failed",
                ErrorMessage = $"Failed to get status: {response.StatusCode}"
            };
        }

        var result = await response.Content.ReadFromJsonAsync<RunwayMLStatusResponse>();
        
        return new VideoGenerationStatus
        {
            JobId = jobId,
            Status = MapStatus(result?.Status ?? "unknown"),
            Progress = result?.Progress ?? 0,
            VideoUrl = result?.Output?.Url,
            ErrorMessage = result?.Error,
            EstimatedTimeRemaining = result?.EstimatedTimeRemaining,
            Metadata = new Dictionary<string, object>
            {
                ["provider"] = "RunwayML",
                ["model"] = _config.DefaultModel
            }
        };
    }

    public async Task CancelGenerationAsync(string jobId)
    {
        await _httpClient.DeleteAsync($"/generations/{jobId}");
    }

    private async Task<string> PollForCompletionAsync(string jobId, IProgress<int>? progress)
    {
        var startTime = DateTime.UtcNow;
        var pollInterval = TimeSpan.FromSeconds(2);
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

            if (status.Status == "cancelled")
                throw new OperationCanceledException("Video generation was cancelled");

            await Task.Delay(pollInterval);
        }

        throw new TimeoutException($"Video generation timed out after {maxWaitTime.TotalSeconds} seconds");
    }

    private async Task<string> DownloadVideoAsync(string url)
    {
        var videoData = await _httpClient.GetByteArrayAsync(url);
        
        var outputDir = Path.Combine(Path.GetTempPath(), "VoidVideoGenerator", "RunwayML");
        Directory.CreateDirectory(outputDir);
        
        var outputPath = Path.Combine(outputDir, $"runway_{Guid.NewGuid()}.mp4");
        await File.WriteAllBytesAsync(outputPath, videoData);
        
        return outputPath;
    }

    private static string MapStatus(string runwayStatus)
    {
        return runwayStatus.ToLowerInvariant() switch
        {
            "pending" => "queued",
            "running" => "processing",
            "succeeded" => "completed",
            "failed" => "failed",
            "canceled" => "cancelled",
            _ => "queued"
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
    private class RunwayMLResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    private class RunwayMLStatusResponse
    {
        public string Status { get; set; } = string.Empty;
        public int Progress { get; set; }
        public RunwayMLOutput? Output { get; set; }
        public string? Error { get; set; }
        public int? EstimatedTimeRemaining { get; set; }
    }

    private class RunwayMLOutput
    {
        public string Url { get; set; } = string.Empty;
    }
}
