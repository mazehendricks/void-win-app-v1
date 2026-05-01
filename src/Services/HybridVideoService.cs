namespace VoidVideoGenerator.Services;

using System.Diagnostics;
using VoidVideoGenerator.Models;

/// <summary>
/// Hybrid video generation using keyframe generation + frame interpolation
/// More affordable alternative to full AI video generation
/// </summary>
public class HybridVideoService : IAIVideoGeneratorService
{
    private readonly HybridConfig _config;
    private readonly string _tempDir;

    public string ProviderName => "Hybrid (Keyframes + Interpolation)";
    public int MaxDuration => 30; // Can be longer since we're interpolating
    public bool SupportsImageToVideo => true;

    public HybridVideoService(HybridConfig config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _tempDir = Path.Combine(Path.GetTempPath(), "VoidVideoGenerator", "Hybrid");
        Directory.CreateDirectory(_tempDir);
    }

    public async Task<string> GenerateVideoAsync(VideoPrompt prompt, IProgress<int>? progress = null)
    {
        var jobId = Guid.NewGuid().ToString();
        var jobDir = Path.Combine(_tempDir, jobId);
        Directory.CreateDirectory(jobDir);

        try
        {
            progress?.Report(5);

            // Step 1: Generate keyframes using Stable Diffusion or similar
            var keyframes = await GenerateKeyframesAsync(prompt, jobDir, progress);
            
            progress?.Report(40);

            // Step 2: Interpolate between keyframes
            var interpolatedFrames = await InterpolateFramesAsync(keyframes, jobDir, progress);
            
            progress?.Report(80);

            // Step 3: Assemble video from frames
            var videoPath = await AssembleVideoAsync(interpolatedFrames, jobDir, prompt.FrameRate ?? _config.TargetFPS);
            
            progress?.Report(100);

            return videoPath;
        }
        catch
        {
            // Cleanup on error
            if (Directory.Exists(jobDir))
                Directory.Delete(jobDir, true);
            throw;
        }
    }

    public async Task<bool> IsAvailableAsync()
    {
        // Check if FFmpeg is available
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = "-version",
                    RedirectStandardOutput = true,
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

    public Task<VideoGenerationStatus> GetStatusAsync(string jobId)
    {
        // For hybrid service, we don't track async jobs
        return Task.FromResult(new VideoGenerationStatus
        {
            JobId = jobId,
            Status = "completed",
            Progress = 100
        });
    }

    public Task CancelGenerationAsync(string jobId)
    {
        // Not implemented for hybrid service
        return Task.CompletedTask;
    }

    private async Task<List<string>> GenerateKeyframesAsync(VideoPrompt prompt, string outputDir, IProgress<int>? progress)
    {
        var keyframes = new List<string>();
        var totalFrames = prompt.Duration * _config.TargetFPS;
        var numKeyframes = (int)Math.Ceiling((double)totalFrames / _config.KeyframeInterval);

        // If reference image provided, use it as first keyframe
        if (!string.IsNullOrEmpty(prompt.ReferenceImagePath) && File.Exists(prompt.ReferenceImagePath))
        {
            var firstKeyframe = Path.Combine(outputDir, "keyframe_000.png");
            File.Copy(prompt.ReferenceImagePath, firstKeyframe);
            keyframes.Add(firstKeyframe);
        }

        // Generate additional keyframes
        // NOTE: This is a placeholder - in production, you would integrate with
        // Stable Diffusion API or local installation
        for (int i = keyframes.Count; i < numKeyframes; i++)
        {
            var keyframePath = Path.Combine(outputDir, $"keyframe_{i:D3}.png");
            
            // TODO: Call Stable Diffusion to generate keyframe
            // For now, we'll create a placeholder
            await GeneratePlaceholderKeyframeAsync(prompt, keyframePath, i);
            
            keyframes.Add(keyframePath);
            
            // Update progress (5-40% range)
            var keyframeProgress = 5 + (int)((i / (double)numKeyframes) * 35);
            progress?.Report(keyframeProgress);
        }

        return keyframes;
    }

    private async Task GeneratePlaceholderKeyframeAsync(VideoPrompt prompt, string outputPath, int index)
    {
        // This is a placeholder - in production, integrate with Stable Diffusion
        // For now, create a simple colored image
        await Task.Run(() =>
        {
            using var bitmap = new System.Drawing.Bitmap(
                prompt.Width ?? 512,
                prompt.Height ?? 512
            );
            using var graphics = System.Drawing.Graphics.FromImage(bitmap);
            
            // Create gradient based on index
            var color = System.Drawing.Color.FromArgb(
                50 + (index * 20) % 200,
                100 + (index * 30) % 150,
                150 + (index * 40) % 100
            );
            
            graphics.Clear(color);
            
            // Add text
            using var font = new System.Drawing.Font("Arial", 24);
            using var brush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
            graphics.DrawString(
                $"Keyframe {index}\n{prompt.Description}",
                font,
                brush,
                new System.Drawing.PointF(10, 10)
            );
            
            bitmap.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);
        });
    }

    private async Task<List<string>> InterpolateFramesAsync(List<string> keyframes, string outputDir, IProgress<int>? progress)
    {
        var allFrames = new List<string>();
        var frameIndex = 0;

        for (int i = 0; i < keyframes.Count - 1; i++)
        {
            var frame1 = keyframes[i];
            var frame2 = keyframes[i + 1];

            // Add first keyframe
            var keyframeCopy = Path.Combine(outputDir, $"frame_{frameIndex:D6}.png");
            File.Copy(frame1, keyframeCopy, true);
            allFrames.Add(keyframeCopy);
            frameIndex++;

            // Interpolate frames between keyframes
            var interpolatedFrames = await InterpolateBetweenFramesAsync(
                frame1,
                frame2,
                _config.KeyframeInterval - 1,
                outputDir,
                frameIndex
            );

            allFrames.AddRange(interpolatedFrames);
            frameIndex += interpolatedFrames.Count;

            // Update progress (40-80% range)
            var interpProgress = 40 + (int)(((i + 1) / (double)(keyframes.Count - 1)) * 40);
            progress?.Report(interpProgress);
        }

        // Add last keyframe
        var lastFrame = Path.Combine(outputDir, $"frame_{frameIndex:D6}.png");
        File.Copy(keyframes[^1], lastFrame, true);
        allFrames.Add(lastFrame);

        return allFrames;
    }

    private async Task<List<string>> InterpolateBetweenFramesAsync(
        string frame1Path,
        string frame2Path,
        int numFrames,
        string outputDir,
        int startIndex)
    {
        var interpolatedFrames = new List<string>();

        if (_config.InterpolationMethod == "RIFE")
        {
            // Use RIFE for interpolation
            interpolatedFrames = await InterpolateWithRIFEAsync(
                frame1Path,
                frame2Path,
                numFrames,
                outputDir,
                startIndex
            );
        }
        else
        {
            // Fallback to simple linear interpolation
            interpolatedFrames = await SimpleLinearInterpolationAsync(
                frame1Path,
                frame2Path,
                numFrames,
                outputDir,
                startIndex
            );
        }

        return interpolatedFrames;
    }

    private async Task<List<string>> InterpolateWithRIFEAsync(
        string frame1,
        string frame2,
        int numFrames,
        string outputDir,
        int startIndex)
    {
        // TODO: Integrate with RIFE (Real-Time Intermediate Flow Estimation)
        // For now, fall back to simple interpolation
        return await SimpleLinearInterpolationAsync(frame1, frame2, numFrames, outputDir, startIndex);
    }

    private async Task<List<string>> SimpleLinearInterpolationAsync(
        string frame1Path,
        string frame2Path,
        int numFrames,
        string outputDir,
        int startIndex)
    {
        var frames = new List<string>();

        await Task.Run(() =>
        {
            using var img1 = System.Drawing.Image.FromFile(frame1Path);
            using var img2 = System.Drawing.Image.FromFile(frame2Path);
            using var bmp1 = new System.Drawing.Bitmap(img1);
            using var bmp2 = new System.Drawing.Bitmap(img2);

            for (int i = 0; i < numFrames; i++)
            {
                var alpha = (i + 1) / (double)(numFrames + 1);
                var outputPath = Path.Combine(outputDir, $"frame_{startIndex + i:D6}.png");

                using var interpolated = new System.Drawing.Bitmap(bmp1.Width, bmp1.Height);
                
                for (int y = 0; y < bmp1.Height; y++)
                {
                    for (int x = 0; x < bmp1.Width; x++)
                    {
                        var c1 = bmp1.GetPixel(x, y);
                        var c2 = bmp2.GetPixel(x, y);

                        var r = (int)(c1.R * (1 - alpha) + c2.R * alpha);
                        var g = (int)(c1.G * (1 - alpha) + c2.G * alpha);
                        var b = (int)(c1.B * (1 - alpha) + c2.B * alpha);

                        interpolated.SetPixel(x, y, System.Drawing.Color.FromArgb(r, g, b));
                    }
                }

                interpolated.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);
                frames.Add(outputPath);
            }
        });

        return frames;
    }

    private async Task<string> AssembleVideoAsync(List<string> frames, string outputDir, int fps)
    {
        var outputPath = Path.Combine(outputDir, "output.mp4");
        var framePattern = Path.Combine(outputDir, "frame_%06d.png");

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-framerate {fps} -i \"{framePattern}\" -c:v libx264 -pix_fmt yuv420p -y \"{outputPath}\"",
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
            throw new Exception($"FFmpeg failed: {error}");
        }

        return outputPath;
    }
}
