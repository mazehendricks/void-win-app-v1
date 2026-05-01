namespace VoidVideoGenerator.Services;

using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using VoidVideoGenerator.Models;

/// <summary>
/// AnimateDiff local video generation service via ComfyUI
/// </summary>
public class AnimateDiffVideoService : IAIVideoGeneratorService, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly AnimateDiffConfig _config;
    private bool _disposed;

    public string ProviderName => "AnimateDiff (Local)";
    public int MaxDuration => 10; // Limited by VRAM
    public bool SupportsImageToVideo => true;

    public AnimateDiffVideoService(AnimateDiffConfig config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_config.ComfyUIEndpoint),
            Timeout = TimeSpan.FromSeconds(_config.TimeoutSeconds)
        };
    }

    public async Task<string> GenerateVideoAsync(VideoPrompt prompt, IProgress<int>? progress = null)
    {
        // Check if ComfyUI is running
        if (!await IsAvailableAsync())
            throw new InvalidOperationException("ComfyUI is not running. Please start ComfyUI first.");

        progress?.Report(5);

        // Create workflow
        var workflow = CreateAnimateDiffWorkflow(prompt);
        
        // Submit to ComfyUI
        var request = new { prompt = workflow, client_id = Guid.NewGuid().ToString() };
        var response = await _httpClient.PostAsJsonAsync("/prompt", request);
        
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"ComfyUI API error: {response.StatusCode} - {error}");
        }

        var result = await response.Content.ReadFromJsonAsync<ComfyUIResponse>();
        if (result == null || string.IsNullOrEmpty(result.PromptId))
            throw new InvalidOperationException("Invalid response from ComfyUI");

        progress?.Report(10);

        // Wait for completion
        var outputPath = await WaitForOutputAsync(result.PromptId, progress);

        progress?.Report(100);

        return outputPath;
    }

    public async Task<bool> IsAvailableAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/system_stats");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<VideoGenerationStatus> GetStatusAsync(string jobId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/history/{jobId}");
            
            if (!response.IsSuccessStatusCode)
            {
                return new VideoGenerationStatus
                {
                    JobId = jobId,
                    Status = "failed",
                    ErrorMessage = "Failed to get status from ComfyUI"
                };
            }

            var history = await response.Content.ReadFromJsonAsync<Dictionary<string, ComfyUIHistoryItem>>();
            
            if (history != null && history.TryGetValue(jobId, out var item))
            {
                var status = item.Status?.StatusStr ?? "unknown";
                var isCompleted = item.Outputs != null && item.Outputs.Count > 0;
                
                return new VideoGenerationStatus
                {
                    JobId = jobId,
                    Status = isCompleted ? "completed" : (status == "error" ? "failed" : "processing"),
                    Progress = isCompleted ? 100 : 50,
                    Metadata = new Dictionary<string, object>
                    {
                        ["provider"] = "AnimateDiff",
                        ["comfyui_endpoint"] = _config.ComfyUIEndpoint
                    }
                };
            }

            return new VideoGenerationStatus
            {
                JobId = jobId,
                Status = "queued",
                Progress = 0
            };
        }
        catch (Exception ex)
        {
            return new VideoGenerationStatus
            {
                JobId = jobId,
                Status = "failed",
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task CancelGenerationAsync(string jobId)
    {
        await _httpClient.PostAsync("/interrupt", null);
    }

    private Dictionary<string, object> CreateAnimateDiffWorkflow(VideoPrompt prompt)
    {
        var numFrames = prompt.Duration * 8; // 8 FPS for AnimateDiff
        var seed = prompt.Seed ?? new Random().Next(0, int.MaxValue);

        // Simplified workflow - in production, this would be much more complex
        var workflow = new Dictionary<string, object>
        {
            ["1"] = new
            {
                inputs = new
                {
                    ckpt_name = Path.GetFileName(_config.CheckpointPath)
                },
                class_type = "CheckpointLoaderSimple"
            },
            ["2"] = new
            {
                inputs = new
                {
                    model_name = Path.GetFileName(_config.ModelPath)
                },
                class_type = "AnimateDiffLoader"
            },
            ["3"] = new
            {
                inputs = new
                {
                    text = prompt.Description,
                    clip = new[] { "1", 1 }
                },
                class_type = "CLIPTextEncode"
            },
            ["4"] = new
            {
                inputs = new
                {
                    text = prompt.NegativePrompt ?? "blurry, low quality, distorted",
                    clip = new[] { "1", 1 }
                },
                class_type = "CLIPTextEncode"
            },
            ["5"] = new
            {
                inputs = new
                {
                    seed = seed,
                    steps = _config.Steps,
                    cfg = _config.CFG,
                    sampler_name = _config.Sampler,
                    scheduler = "normal",
                    denoise = 1.0,
                    model = new[] { "2", 0 },
                    positive = new[] { "3", 0 },
                    negative = new[] { "4", 0 },
                    latent_image = new[] { "6", 0 }
                },
                class_type = "KSampler"
            },
            ["6"] = new
            {
                inputs = new
                {
                    width = prompt.Width ?? 512,
                    height = prompt.Height ?? 512,
                    batch_size = numFrames
                },
                class_type = "EmptyLatentImage"
            },
            ["7"] = new
            {
                inputs = new
                {
                    samples = new[] { "5", 0 },
                    vae = new[] { "1", 2 }
                },
                class_type = "VAEDecode"
            },
            ["8"] = new
            {
                inputs = new
                {
                    frame_rate = prompt.FrameRate ?? 8,
                    loop_count = 0,
                    filename_prefix = "AnimateDiff",
                    format = "video/h264-mp4",
                    images = new[] { "7", 0 }
                },
                class_type = "VHS_VideoCombine"
            }
        };

        return workflow;
    }

    private async Task<string> WaitForOutputAsync(string promptId, IProgress<int>? progress)
    {
        var startTime = DateTime.UtcNow;
        var pollInterval = TimeSpan.FromSeconds(2);
        var maxWaitTime = TimeSpan.FromSeconds(_config.TimeoutSeconds);

        while (DateTime.UtcNow - startTime < maxWaitTime)
        {
            var status = await GetStatusAsync(promptId);

            // Update progress
            if (status.Progress > 0)
            {
                var scaledProgress = 10 + (int)(status.Progress * 0.9);
                progress?.Report(scaledProgress);
            }

            if (status.Status == "completed")
            {
                // ComfyUI typically saves to its output directory
                // This assumes ComfyUI is running from its installation directory
                // Users may need to configure this path in the config
                var comfyUIDir = Environment.GetEnvironmentVariable("COMFYUI_DIR") ??
                                 Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "ComfyUI");
                var outputDir = Path.Combine(comfyUIDir, "output");
                
                if (!Directory.Exists(outputDir))
                {
                    // Fallback: try common locations
                    var fallbackDirs = new[]
                    {
                        Path.Combine(Environment.CurrentDirectory, "ComfyUI", "output"),
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Documents", "ComfyUI", "output"),
                        "C:\\ComfyUI\\output"
                    };
                    
                    outputDir = fallbackDirs.FirstOrDefault(Directory.Exists) ?? outputDir;
                }
                
                if (Directory.Exists(outputDir))
                {
                    // Find the most recent video file
                    var videoFiles = Directory.GetFiles(outputDir, "AnimateDiff*.mp4")
                        .OrderByDescending(f => File.GetCreationTime(f))
                        .ToList();

                    if (videoFiles.Any())
                        return videoFiles.First();
                }

                throw new FileNotFoundException($"Generated video file not found. Please check ComfyUI output directory: {outputDir}");
            }

            if (status.Status == "failed")
                throw new Exception($"Video generation failed: {status.ErrorMessage}");

            await Task.Delay(pollInterval);
        }

        throw new TimeoutException($"Video generation timed out after {maxWaitTime.TotalSeconds} seconds");
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
    private class ComfyUIResponse
    {
        public string PromptId { get; set; } = string.Empty;
    }

    private class ComfyUIHistoryItem
    {
        public ComfyUIStatus? Status { get; set; }
        public Dictionary<string, object>? Outputs { get; set; }
    }

    private class ComfyUIStatus
    {
        public string StatusStr { get; set; } = string.Empty;
        public bool Completed { get; set; }
    }
}
