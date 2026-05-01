namespace VoidVideoGenerator.Services;

using System.Diagnostics;
using VoidVideoGenerator.Models;

/// <summary>
/// Video assembly service using FFmpeg
/// </summary>
public class FFmpegVideoAssembly : IVideoAssemblyService
{
    private readonly string _ffmpegPath;
    private bool _useGpuAcceleration;
    private string _gpuEncoder;
    private VideoOutputSettings _outputSettings;

    public FFmpegVideoAssembly(string ffmpegPath = "ffmpeg", bool useGpuAcceleration = false, string gpuEncoder = "auto", VideoOutputSettings? outputSettings = null)
    {
        _ffmpegPath = ffmpegPath;
        _useGpuAcceleration = useGpuAcceleration;
        _gpuEncoder = gpuEncoder;
        _outputSettings = outputSettings ?? new VideoOutputSettings();
    }

    /// <summary>
    /// Set GPU acceleration settings
    /// </summary>
    public void SetGpuAcceleration(bool enabled, string encoder = "auto")
    {
        _useGpuAcceleration = enabled;
        _gpuEncoder = encoder;
    }

    /// <summary>
    /// Set video output settings
    /// </summary>
    public void SetOutputSettings(VideoOutputSettings settings)
    {
        _outputSettings = settings;
    }

    public async Task<bool> IsAvailableAsync()
    {
        try
        {
            using var process = new Process
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

        // Handle single audio file case - no concatenation needed
        if (audioFiles.Count == 1)
        {
            File.Copy(audioFiles[0], outputPath, true);
            return;
        }

        // Create concat file list
        var concatFile = Path.Combine(Path.GetTempPath(), $"ffmpeg_audio_concat_{Guid.NewGuid()}.txt");
        // Use forward slashes for FFmpeg compatibility on all platforms
        var concatContent = string.Join("\n", audioFiles.Select(f => $"file '{f.Replace("\\", "/")}'"));
        await File.WriteAllTextAsync(concatFile, concatContent);

        try
        {
            using var process = new Process
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
            
            // Read output asynchronously to prevent deadlocks
            var errorTask = process.StandardError.ReadToEndAsync();
            var outputTask = process.StandardOutput.ReadToEndAsync();
            
            await process.WaitForExitAsync();
            
            var error = await errorTask;

            if (process.ExitCode != 0)
            {
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
        using var process = new Process
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
        if (imageFiles == null || imageFiles.Count == 0)
        {
            throw new ArgumentException("No image files provided for video creation", nameof(imageFiles));
        }

        var encoderType = _useGpuAcceleration ? "GPU" : "CPU";
        progress?.Report($"Creating video from images and audio using {encoderType} encoding...");

        // Check if animations are enabled
        if (_outputSettings.EnableKenBurnsEffect || _outputSettings.EnableCrossfadeTransitions)
        {
            await CreateAnimatedVideoAsync(imageFiles, audioPath, audioDuration, outputPath, progress);
        }
        else
        {
            await CreateStaticVideoAsync(imageFiles, audioPath, audioDuration, outputPath, progress);
        }
    }

    /// <summary>
    /// Create video with smooth animations (Ken Burns + crossfades)
    /// </summary>
    private async Task CreateAnimatedVideoAsync(
        List<string> imageFiles,
        string audioPath,
        double audioDuration,
        string outputPath,
        IProgress<string>? progress = null)
    {
        progress?.Report("Creating animated video with Ken Burns effects and crossfade transitions...");

        // Calculate duration per image (accounting for transitions)
        double transitionDuration = _outputSettings.EnableCrossfadeTransitions ? _outputSettings.TransitionDuration : 0;
        double effectiveDuration = audioDuration + (transitionDuration * (imageFiles.Count - 1));
        double durationPerImage = effectiveDuration / imageFiles.Count;

        // Build complex filter for animations
        var filterComplex = BuildAnimationFilterComplex(imageFiles, durationPerImage, transitionDuration);

        try
        {
            // Build FFmpeg arguments
            string videoCodec = GetVideoCodecArguments();
            string audioCodec = GetAudioCodecArguments();
            
            // Build input arguments
            var inputArgs = string.Join(" ", imageFiles.Select(img => $"-loop 1 -t {durationPerImage:F3} -i \"{img}\""));
            
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _ffmpegPath,
                    Arguments = $"{inputArgs} -i \"{audioPath}\" " +
                                $"-filter_complex \"{filterComplex}\" " +
                                $"-map \"[outv]\" -map {imageFiles.Count}:a " +
                                $"{videoCodec} {audioCodec} -pix_fmt yuv420p -r {_outputSettings.FrameRate} " +
                                $"-shortest \"{outputPath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            progress?.Report($"Applying smooth animations...");
            
            process.Start();
            
            // Read output asynchronously to prevent deadlocks
            var errorTask = process.StandardError.ReadToEndAsync();
            var outputTask = process.StandardOutput.ReadToEndAsync();
            
            await process.WaitForExitAsync();
            
            var error = await errorTask;
            var output = await outputTask;

            if (process.ExitCode != 0)
            {
                throw new Exception($"FFmpeg animated video creation failed: {error}");
            }
        }
        catch (Exception ex)
        {
            progress?.Report($"Animation failed, falling back to static video: {ex.Message}");
            await CreateStaticVideoAsync(imageFiles, audioPath, audioDuration, outputPath, progress);
        }
    }

    /// <summary>
    /// Create static video (original method, fallback)
    /// </summary>
    private async Task CreateStaticVideoAsync(
        List<string> imageFiles,
        string audioPath,
        double audioDuration,
        string outputPath,
        IProgress<string>? progress = null)
    {
        progress?.Report("Creating static video...");

        // Calculate duration per image
        var durationPerImage = audioDuration / imageFiles.Count;

        // Create input file list with durations
        var inputFile = Path.Combine(Path.GetTempPath(), $"ffmpeg_concat_{Guid.NewGuid()}.txt");
        var inputContent = new System.Text.StringBuilder();
        
        foreach (var image in imageFiles)
        {
            // Use forward slashes for FFmpeg compatibility on all platforms
            var imagePath = image.Replace("\\", "/");
            inputContent.AppendLine($"file '{imagePath}'");
            inputContent.AppendLine($"duration {durationPerImage:F6}");
        }
        // Add last image again for proper ending
        if (imageFiles.Count > 0)
        {
            var lastImagePath = imageFiles[^1].Replace("\\", "/");
            inputContent.AppendLine($"file '{lastImagePath}'");
        }

        await File.WriteAllTextAsync(inputFile, inputContent.ToString());

        try
        {
            // Build FFmpeg arguments based on settings
            string videoCodec = GetVideoCodecArguments();
            string audioCodec = GetAudioCodecArguments();
            string scaleFilter = GetScaleFilter();
            
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _ffmpegPath,
                    Arguments = $"-f concat -safe 0 -i \"{inputFile}\" -i \"{audioPath}\" " +
                                $"{scaleFilter}{videoCodec} {audioCodec} -pix_fmt yuv420p -r {_outputSettings.FrameRate} " +
                                $"-shortest \"{outputPath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            
            process.Start();
            
            // Read output asynchronously to prevent deadlocks
            var errorTask = process.StandardError.ReadToEndAsync();
            var outputTask = process.StandardOutput.ReadToEndAsync();
            
            await process.WaitForExitAsync();
            
            var error = await errorTask;
            var output = await outputTask;

            if (process.ExitCode != 0)
            {
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

    /// <summary>
    /// Build FFmpeg filter complex for Ken Burns effect and crossfade transitions
    /// </summary>
    private string BuildAnimationFilterComplex(List<string> imageFiles, double durationPerImage, double transitionDuration)
    {
        var filters = new System.Text.StringBuilder();
        int width = _outputSettings.Width;
        int height = _outputSettings.Height;
        double zoomIntensity = _outputSettings.ZoomIntensity;
        int fps = _outputSettings.FrameRate;

        // Apply Ken Burns effect to each image
        for (int i = 0; i < imageFiles.Count; i++)
        {
            if (_outputSettings.EnableKenBurnsEffect)
            {
                // Alternate between zoom-in and zoom-out for variety
                bool zoomIn = i % 2 == 0;
                double startZoom = zoomIn ? 1.0 : zoomIntensity;
                double endZoom = zoomIn ? zoomIntensity : 1.0;
                
                // Calculate zoom parameters
                int totalFrames = (int)(durationPerImage * fps);
                double zoomStep = (endZoom - startZoom) / totalFrames;
                
                // Ken Burns effect: scale + pan
                filters.Append($"[{i}:v]scale={width * 2}:{height * 2},");
                filters.Append($"zoompan=z='if(lte(zoom,1.0),{startZoom},{startZoom}+({zoomStep}*on))':d={totalFrames}:x='iw/2-(iw/zoom/2)':y='ih/2-(ih/zoom/2)':s={width}x{height}:fps={fps}");
                filters.Append($"[v{i}];");
            }
            else
            {
                // Just scale to proper size
                filters.Append($"[{i}:v]scale={width}:{height}:force_original_aspect_ratio=decrease,pad={width}:{height}:(ow-iw)/2:(oh-ih)/2[v{i}];");
            }
        }

        // Apply crossfade transitions between images
        if (_outputSettings.EnableCrossfadeTransitions && imageFiles.Count > 1)
        {
            // First image
            filters.Append($"[v0]");
            
            // Chain crossfades
            for (int i = 1; i < imageFiles.Count; i++)
            {
                double offset = (durationPerImage * i) - transitionDuration;
                if (i == 1)
                {
                    filters.Append($"[v{i}]xfade=transition=fade:duration={transitionDuration:F3}:offset={offset:F3}");
                }
                else
                {
                    filters.Append($"[v{i}]xfade=transition=fade:duration={transitionDuration:F3}:offset={offset:F3}");
                }
                
                if (i < imageFiles.Count - 1)
                {
                    filters.Append($"[vx{i}];[vx{i}]");
                }
            }
            filters.Append("[outv]");
        }
        else
        {
            // No transitions, just concatenate
            filters.Append(string.Join("", Enumerable.Range(0, imageFiles.Count).Select(i => $"[v{i}]")));
            filters.Append($"concat=n={imageFiles.Count}:v=1:a=0[outv]");
        }

        return filters.ToString();
    }

    /// <summary>
    /// Get video codec arguments based on GPU/CPU settings and output settings
    /// </summary>
    private string GetVideoCodecArguments()
    {
        _outputSettings ??= new VideoOutputSettings(); // Null safety
        int bitrate = _outputSettings.VideoBitrate;
        int maxrate = (int)(bitrate * 1.5);
        
        if (!_useGpuAcceleration)
        {
            // CPU encoding with libx264
            return $"-c:v libx264 -tune stillimage -preset medium -b:v {bitrate}k -maxrate {maxrate}k -bufsize {bitrate * 2}k";
        }

        // GPU encoding - determine encoder type
        string encoder = DetermineGpuEncoder();
        
        return encoder switch
        {
            "h264_nvenc" => $"-c:v h264_nvenc -preset p4 -tune hq -rc vbr -cq 23 -b:v {bitrate}k -maxrate {maxrate}k",
            "h264_amf" => $"-c:v h264_amf -quality quality -rc vbr_latency -qp_i 23 -qp_p 23 -b:v {bitrate}k",
            "h264_qsv" => $"-c:v h264_qsv -preset medium -global_quality 23 -b:v {bitrate}k",
            "h264_videotoolbox" => $"-c:v h264_videotoolbox -b:v {bitrate}k",
            _ => $"-c:v libx264 -tune stillimage -preset medium -b:v {bitrate}k -maxrate {maxrate}k" // Fallback to CPU
        };
    }

    /// <summary>
    /// Get audio codec arguments based on output settings
    /// </summary>
    private string GetAudioCodecArguments()
    {
        _outputSettings ??= new VideoOutputSettings(); // Null safety
        int channels = (_outputSettings.AudioChannels?.ToLower() ?? "stereo") == "mono" ? 1 : 2;
        return $"-c:a aac -b:a {_outputSettings.AudioBitrate}k -ar {_outputSettings.AudioSampleRate} -ac {channels}";
    }

    /// <summary>
    /// Get scale filter for resolution
    /// </summary>
    private string GetScaleFilter()
    {
        _outputSettings ??= new VideoOutputSettings(); // Null safety
        
        // Only add scale filter if resolution is not 1920x1080 (default)
        if (_outputSettings.Width != 1920 || _outputSettings.Height != 1080)
        {
            return $"-vf scale={_outputSettings.Width}:{_outputSettings.Height} ";
        }
        return "";
    }

    /// <summary>
    /// Determine which GPU encoder to use
    /// </summary>
    private string DetermineGpuEncoder()
    {
        if (_gpuEncoder.ToLower() == "auto")
        {
            // Try to detect available GPU encoder
            // Priority: NVIDIA > AMD > Intel > Apple
            if (IsEncoderAvailable("h264_nvenc")) return "h264_nvenc";
            if (IsEncoderAvailable("h264_amf")) return "h264_amf";
            if (IsEncoderAvailable("h264_qsv")) return "h264_qsv";
            if (IsEncoderAvailable("h264_videotoolbox")) return "h264_videotoolbox";
            
            return "libx264"; // Fallback to CPU
        }

        // Use specified encoder
        return _gpuEncoder.ToLower() switch
        {
            "nvidia" or "nvenc" => "h264_nvenc",
            "amd" or "amf" => "h264_amf",
            "intel" or "qsv" => "h264_qsv",
            "apple" or "videotoolbox" => "h264_videotoolbox",
            _ => "libx264"
        };
    }

    /// <summary>
    /// Check if a specific encoder is available in FFmpeg
    /// </summary>
    private bool IsEncoderAvailable(string encoderName)
    {
        try
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _ffmpegPath,
                    Arguments = "-encoders",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return output.Contains(encoderName);
        }
        catch
        {
            return false;
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

        // Create a simple black placeholder image using FFmpeg with configured resolution
        var placeholderPath = Path.Combine(outputDirectory, "placeholder.png");
        var resolution = $"{_outputSettings.Width}x{_outputSettings.Height}";
        
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _ffmpegPath,
                Arguments = $"-f lavfi -i color=c=black:s={resolution}:d=1 -frames:v 1 \"{placeholderPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        
        // Read output asynchronously to prevent deadlocks
        var errorTask = process.StandardError.ReadToEndAsync();
        var outputTask = process.StandardOutput.ReadToEndAsync();
        
        await process.WaitForExitAsync();
        
        var error = await errorTask;

        if (process.ExitCode != 0 || !File.Exists(placeholderPath))
        {
            throw new Exception($"Failed to create placeholder image. Error: {error}. Please provide images in the visuals directory.");
        }
        
        imageFiles.Add(placeholderPath);
        
        return imageFiles;
    }
}
