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

    public VideoGenerationPipeline(
        IScriptGeneratorService scriptGenerator,
        IVoiceGeneratorService voiceGenerator,
        IVideoAssemblyService videoAssembly)
    {
        _scriptGenerator = scriptGenerator;
        _voiceGenerator = voiceGenerator;
        _videoAssembly = videoAssembly;
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

            // Step 3: Prepare visuals (placeholder for now)
            progress?.Report("Step 3/4: Preparing visuals...");
            var visualsDirectory = Path.Combine(request.OutputPath, "visuals");
            Directory.CreateDirectory(visualsDirectory);
            
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

    private string SanitizeFileName(string fileName)
    {
        var invalid = Path.GetInvalidFileNameChars();
        return string.Join("_", fileName.Split(invalid, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
    }
}
