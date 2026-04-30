namespace VoidVideoGenerator.Services;

using System.Diagnostics;
using VoidVideoGenerator.Models;

/// <summary>
/// Video assembly service using FFmpeg
/// </summary>
public class FFmpegVideoAssembly : IVideoAssemblyService
{
    private readonly string _ffmpegPath;

    public FFmpegVideoAssembly(string ffmpegPath = "ffmpeg")
    {
        _ffmpegPath = ffmpegPath;
    }

    public async Task<bool> IsAvailableAsync()
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _ffmpegPath,
                    Arguments = "-version",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            await process.WaitForExitAsync();
            return process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<string> AssembleVideoAsync(
        List<string> audioFiles,
        List<string> imageFiles,
        string outputPath,
        IProgress<string>? progress = null)
    {
        progress?.Report("Assembling video with FFmpeg...");

        // Ensure output directory exists
        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Concatenate audio files first
        var concatenatedAudio = Path.Combine(Path.GetTempPath(), $"audio_{Guid.NewGuid()}.wav");
        await ConcatenateAudioFilesAsync(audioFiles, concatenatedAudio, progress);

        // Get audio duration
        var audioDuration = await GetAudioDurationAsync(concatenatedAudio);

        // Create video from images with audio
        await CreateVideoFromImagesAsync(imageFiles, concatenatedAudio, audioDuration, outputPath, progress);

        // Cleanup temp file
        if (File.Exists(concatenatedAudio))
        {
            File.Delete(concatenatedAudio);
        }

        progress?.Report($"Video created: {outputPath}");
        return outputPath;
    }

    public async Task<string> CreateVideoFromScriptAsync(
        VideoScript script,
        string audioDirectory,
        string visualsDirectory,
        string outputPath,
        IProgress<string>? progress = null)
    {
        // Collect audio files
        var audioFiles = Directory.GetFiles(audioDirectory, "*.wav")
            .OrderBy(f => f)
            .ToList();

        // Collect image files
        var imageFiles = Directory.GetFiles(visualsDirectory, "*.png")
            .Concat(Directory.GetFiles(visualsDirectory, "*.jpg"))
            .OrderBy(f => f)
            .ToList();

        // If no images, create placeholder images
        if (imageFiles.Count == 0)
        {
            progress?.Report("No images found, creating placeholder visuals...");
            imageFiles = await CreatePlaceholderImagesAsync(script, visualsDirectory);
        }

        return await AssembleVideoAsync(audioFiles, imageFiles, outputPath, progress);
    }

    private async Task ConcatenateAudioFilesAsync(List<string> audioFiles, string outputPath, IProgress<string>? progress = null)
    {
        progress?.Report("Concatenating audio files...");

        // Create concat file list
        var concatFile = Path.GetTempFileName();
        var concatContent = string.Join("\n", audioFiles.Select(f => $"file '{f}'"));
        await File.WriteAllTextAsync(concatFile, concatContent);

        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _ffmpegPath,
                    Arguments = $"-f concat -safe 0 -i \"{concatFile}\" -c copy \"{outputPath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                var error = await process.StandardError.ReadToEndAsync();
                throw new Exception($"FFmpeg audio concatenation failed: {error}");
            }
        }
        finally
        {
            if (File.Exists(concatFile))
            {
                File.Delete(concatFile);
            }
        }
    }

    private async Task<double> GetAudioDurationAsync(string audioPath)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _ffmpegPath,
                Arguments = $"-i \"{audioPath}\" -f null -",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        var output = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        // Parse duration from FFmpeg output
        var match = System.Text.RegularExpressions.Regex.Match(output, @"Duration: (\d{2}):(\d{2}):(\d{2}\.\d{2})");
        if (match.Success)
        {
            var hours = int.Parse(match.Groups[1].Value);
            var minutes = int.Parse(match.Groups[2].Value);
            var seconds = double.Parse(match.Groups[3].Value);
            return hours * 3600 + minutes * 60 + seconds;
        }

        return 60; // Default fallback
    }

    private async Task CreateVideoFromImagesAsync(
        List<string> imageFiles,
        string audioPath,
        double audioDuration,
        string outputPath,
        IProgress<string>? progress = null)
    {
        progress?.Report("Creating video from images and audio...");

        // Calculate duration per image
        var durationPerImage = audioDuration / imageFiles.Count;

        // Create input file list with durations
        var inputFile = Path.GetTempFileName();
        var inputContent = new System.Text.StringBuilder();
        
        foreach (var image in imageFiles)
        {
            inputContent.AppendLine($"file '{image}'");
            inputContent.AppendLine($"duration {durationPerImage}");
        }
        // Add last image again for proper ending
        if (imageFiles.Count > 0)
        {
            inputContent.AppendLine($"file '{imageFiles[^1]}'");
        }

        await File.WriteAllTextAsync(inputFile, inputContent.ToString());

        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _ffmpegPath,
                    Arguments = $"-f concat -safe 0 -i \"{inputFile}\" -i \"{audioPath}\" " +
                               $"-c:v libx264 -tune stillimage -c:a aac -b:a 192k -pix_fmt yuv420p " +
                               $"-shortest \"{outputPath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                var error = await process.StandardError.ReadToEndAsync();
                throw new Exception($"FFmpeg video creation failed: {error}");
            }
        }
        finally
        {
            if (File.Exists(inputFile))
            {
                File.Delete(inputFile);
            }
        }
    }

    private async Task<List<string>> CreatePlaceholderImagesAsync(VideoScript script, string outputDirectory)
    {
        // This is a placeholder - in a real implementation, you would:
        // 1. Use Stable Diffusion to generate images based on visual cues
        // 2. Download stock images from free APIs
        // 3. Create simple text overlays
        
        Directory.CreateDirectory(outputDirectory);
        var imageFiles = new List<string>();

        // For now, just return empty list - user will need to provide images
        // or integrate with Stable Diffusion
        
        return imageFiles;
    }
}
