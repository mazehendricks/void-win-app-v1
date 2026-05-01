namespace VoidVideoGenerator.Services;

using VoidVideoGenerator.Models;

/// <summary>
/// Main pipeline that orchestrates the entire video generation process
/// </summary>
public class VideoGenerationPipeline
{
    private readonly IScriptGeneratorService _scriptGenerator;
    private readonly IVoiceGeneratorService _voiceGenerator;
    private readonly IVideoAssemblyService _videoAssembly;
    private readonly IAIVideoGeneratorService? _aiVideoGenerator;
    private readonly AIVideoConfig _aiVideoConfig;

    public VideoGenerationPipeline(
        IScriptGeneratorService scriptGenerator,
        IVoiceGeneratorService voiceGenerator,
        IVideoAssemblyService videoAssembly,
        IAIVideoGeneratorService? aiVideoGenerator = null,
        AIVideoConfig? aiVideoConfig = null)
    {
        _scriptGenerator = scriptGenerator;
        _voiceGenerator = voiceGenerator;
        _videoAssembly = videoAssembly;
        _aiVideoGenerator = aiVideoGenerator;
        _aiVideoConfig = aiVideoConfig ?? new AIVideoConfig();
    }

    /// <summary>
    /// Generate a complete video from a request
    /// </summary>
    public async Task<string> GenerateVideoAsync(VideoRequest request, IProgress<string>? progress = null)
    {
        try
        {
            // Step 1: Generate script
            progress?.Report("Step 1/4: Generating script...");
            var script = await _scriptGenerator.GenerateScriptAsync(request, progress);

            // Step 2: Generate audio
            progress?.Report("Step 2/4: Generating voice audio...");
            var audioDirectory = Path.Combine(request.OutputPath, "audio");
            Directory.CreateDirectory(audioDirectory);
            var audioFiles = await _voiceGenerator.GenerateScriptAudioAsync(script, audioDirectory, progress);

            // Step 3: Prepare visuals (AI video or images)
            progress?.Report("Step 3/4: Preparing visuals...");
            var visualsDirectory = Path.Combine(request.OutputPath, "visuals");
            Directory.CreateDirectory(visualsDirectory);
            
            List<string> visualAssets;
            
            if (_aiVideoConfig.Provider != "None" && _aiVideoGenerator != null)
            {
                // Generate AI video clips
                progress?.Report("Generating AI video clips...");
                visualAssets = await GenerateAIVideoClipsAsync(script, visualsDirectory, progress);
            }
            else
            {
                // Use provided images or placeholder
                progress?.Report("Using static images...");
                visualAssets = request.VisualImagePaths?.ToList() ?? new List<string>();
            }
            
            // Save script for reference
            var scriptPath = Path.Combine(request.OutputPath, "script.txt");
            await File.WriteAllTextAsync(scriptPath, script.FullText);
            progress?.Report($"Script saved to: {scriptPath}");

            // Step 4: Assemble video
            progress?.Report("Step 4/4: Assembling final video...");
            var videoPath = Path.Combine(request.OutputPath, $"{SanitizeFileName(request.Title)}.mp4");
            await _videoAssembly.CreateVideoFromScriptAsync(script, audioDirectory, visualsDirectory, videoPath, progress);

            progress?.Report($"✓ Video generation complete: {videoPath}");
            return videoPath;
        }
        catch (Exception ex)
        {
            progress?.Report($"✗ Error: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Check if all required services are available
    /// </summary>
    public async Task<Dictionary<string, bool>> CheckServicesAsync()
    {
        return new Dictionary<string, bool>
        {
            ["Ollama (LLM)"] = await _scriptGenerator.IsAvailableAsync(),
            ["Piper (TTS)"] = await _voiceGenerator.IsAvailableAsync(),
            ["FFmpeg"] = await _videoAssembly.IsAvailableAsync()
        };
    }

    private async Task<List<string>> GenerateAIVideoClipsAsync(
        VideoScript script,
        string outputDirectory,
        IProgress<string>? progress)
    {
        var clips = new List<string>();
        var totalSegments = script.Segments.Count;
        
        for (int i = 0; i < totalSegments; i++)
        {
            var segment = script.Segments[i];
            
            progress?.Report($"Generating AI video clip {i + 1}/{totalSegments}: {segment.VisualCue}");
            
            var prompt = new VideoPrompt
            {
                Description = segment.VisualCue,
                Duration = (int)Math.Ceiling(segment.Duration),
                AspectRatio = _aiVideoConfig.DefaultSettings.AspectRatio,
                Style = _aiVideoConfig.DefaultSettings.Style,
                MotionIntensity = _aiVideoConfig.DefaultSettings.MotionIntensity,
                NegativePrompt = _aiVideoConfig.DefaultSettings.NegativePrompt
            };
            
            try
            {
                var clipProgress = new Progress<int>(p => {
                    progress?.Report($"Clip {i + 1}/{totalSegments}: {p}% complete");
                });
                
                var clipPath = await _aiVideoGenerator!.GenerateVideoAsync(prompt, clipProgress);
                
                // Copy to output directory
                var outputPath = Path.Combine(outputDirectory, $"clip_{i:D3}.mp4");
                File.Copy(clipPath, outputPath, true);
                clips.Add(outputPath);
                
                progress?.Report($"✓ Clip {i + 1}/{totalSegments} generated successfully");
            }
            catch (Exception ex)
            {
                progress?.Report($"✗ Error generating clip {i + 1}: {ex.Message}");
                throw;
            }
        }
        
        return clips;
    }

    private string SanitizeFileName(string fileName)
    {
        var invalid = Path.GetInvalidFileNameChars();
        return string.Join("_", fileName.Split(invalid, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
    }
}
