namespace VoidVideoGenerator.Services;

using System.Diagnostics;
using System.Text;

/// <summary>
/// Service for adding captions/subtitles to videos
/// </summary>
public class VideoCaptionService
{
    private readonly string _ffmpegPath;
    private readonly CaptionStyle _defaultStyle;

    public VideoCaptionService(string ffmpegPath = "ffmpeg", CaptionStyle? defaultStyle = null)
    {
        _ffmpegPath = ffmpegPath;
        _defaultStyle = defaultStyle ?? new CaptionStyle();
    }

    /// <summary>
    /// Add captions to a video file
    /// </summary>
    public async Task<string> AddCaptionsToVideoAsync(
        string videoPath,
        List<TranscriptionSegment> segments,
        string outputPath,
        CaptionStyle? style = null,
        IProgress<string>? progress = null)
    {
        style ??= _defaultStyle;
        progress?.Report("Adding captions to video...");

        // Create SRT subtitle file
        var srtPath = Path.Combine(Path.GetTempPath(), $"captions_{Guid.NewGuid()}.srt");
        await CreateSrtFileAsync(segments, srtPath);

        try
        {
            // Build FFmpeg filter for captions
            var subtitlesFilter = BuildSubtitlesFilter(srtPath, style);

            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _ffmpegPath,
                    Arguments = $"-i \"{videoPath}\" -vf \"{subtitlesFilter}\" -c:a copy \"{outputPath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            progress?.Report($"FFmpeg command: {process.StartInfo.Arguments}");
            
            process.Start();
            
            var errorTask = process.StandardError.ReadToEndAsync();
            var outputTask = process.StandardOutput.ReadToEndAsync();
            
            await process.WaitForExitAsync();
            
            var error = await errorTask;

            if (process.ExitCode != 0)
            {
                throw new Exception($"FFmpeg caption overlay failed: {error}");
            }

            progress?.Report($"✓ Captions added successfully: {outputPath}");
            return outputPath;
        }
        finally
        {
            // Cleanup temp SRT file
            if (File.Exists(srtPath))
            {
                File.Delete(srtPath);
            }
        }
    }

    /// <summary>
    /// Add captions with word-level highlighting (more advanced)
    /// </summary>
    public async Task<string> AddAnimatedCaptionsAsync(
        string videoPath,
        List<TranscriptionSegment> segments,
        string outputPath,
        CaptionStyle? style = null,
        IProgress<string>? progress = null)
    {
        style ??= _defaultStyle;
        progress?.Report("Adding animated captions to video...");

        // Build drawtext filter for each segment
        var filterComplex = BuildAnimatedCaptionsFilter(segments, style);

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _ffmpegPath,
                Arguments = $"-i \"{videoPath}\" -filter_complex \"{filterComplex}\" -c:a copy \"{outputPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        progress?.Report("Rendering animated captions...");
        
        process.Start();
        
        var errorTask = process.StandardError.ReadToEndAsync();
        var outputTask = process.StandardOutput.ReadToEndAsync();
        
        await process.WaitForExitAsync();
        
        var error = await errorTask;

        if (process.ExitCode != 0)
        {
            throw new Exception($"FFmpeg animated captions failed: {error}");
        }

        progress?.Report($"✓ Animated captions added successfully: {outputPath}");
        return outputPath;
    }

    private async Task CreateSrtFileAsync(List<TranscriptionSegment> segments, string outputPath)
    {
        var srt = new StringBuilder();
        
        for (int i = 0; i < segments.Count; i++)
        {
            var segment = segments[i];
            srt.AppendLine($"{i + 1}");
            srt.AppendLine($"{FormatSrtTime(segment.StartTime)} --> {FormatSrtTime(segment.EndTime)}");
            srt.AppendLine(segment.Text);
            srt.AppendLine();
        }

        await File.WriteAllTextAsync(outputPath, srt.ToString());
    }

    private string FormatSrtTime(double seconds)
    {
        var ts = TimeSpan.FromSeconds(seconds);
        return $"{ts.Hours:D2}:{ts.Minutes:D2}:{ts.Seconds:D2},{ts.Milliseconds:D3}";
    }

    private string BuildSubtitlesFilter(string srtPath, CaptionStyle style)
    {
        // Escape special characters for FFmpeg
        var escapedPath = srtPath.Replace("\\", "\\\\").Replace(":", "\\:");
        
        var filter = $"subtitles='{escapedPath}':force_style='";
        filter += $"FontName={style.FontName},";
        filter += $"FontSize={style.FontSize},";
        filter += $"PrimaryColour={ConvertColorToAss(style.TextColor)},";
        filter += $"OutlineColour={ConvertColorToAss(style.OutlineColor)},";
        filter += $"BackColour={ConvertColorToAss(style.BackgroundColor)},";
        filter += $"Outline={style.OutlineWidth},";
        filter += $"Shadow={style.ShadowDepth},";
        filter += $"Bold={style.Bold},";
        filter += $"Alignment={style.Alignment}";
        filter += "'";

        return filter;
    }

    private string BuildAnimatedCaptionsFilter(List<TranscriptionSegment> segments, CaptionStyle style)
    {
        var filters = new List<string>();
        
        for (int i = 0; i < segments.Count; i++)
        {
            var segment = segments[i];
            var text = segment.Text.Replace("'", "\\'").Replace(":", "\\:");
            
            var drawtext = $"drawtext=text='{text}':";
            drawtext += $"fontfile={style.FontName}:";
            drawtext += $"fontsize={style.FontSize}:";
            drawtext += $"fontcolor={style.TextColor}:";
            drawtext += $"borderw={style.OutlineWidth}:";
            drawtext += $"bordercolor={style.OutlineColor}:";
            drawtext += $"x=(w-text_w)/2:";
            drawtext += $"y=h-{style.BottomMargin}-text_h:";
            drawtext += $"enable='between(t,{segment.StartTime},{segment.EndTime})'";
            
            filters.Add(drawtext);
        }

        return string.Join(",", filters);
    }

    private string ConvertColorToAss(string hexColor)
    {
        // Convert #RRGGBB to &HAABBGGRR (ASS format)
        if (hexColor.StartsWith("#") && hexColor.Length == 7)
        {
            var r = hexColor.Substring(1, 2);
            var g = hexColor.Substring(3, 2);
            var b = hexColor.Substring(5, 2);
            return $"&H00{b}{g}{r}";
        }
        return "&H00FFFFFF"; // Default white
    }
}

/// <summary>
/// Caption styling options
/// </summary>
public class CaptionStyle
{
    public string FontName { get; set; } = "Arial";
    public int FontSize { get; set; } = 24;
    public string TextColor { get; set; } = "#FFFFFF"; // White
    public string OutlineColor { get; set; } = "#000000"; // Black
    public string BackgroundColor { get; set; } = "#00000080"; // Semi-transparent black
    public int OutlineWidth { get; set; } = 2;
    public int ShadowDepth { get; set; } = 1;
    public int Bold { get; set; } = 1; // 0 = normal, 1 = bold
    public int Alignment { get; set; } = 2; // 2 = bottom center
    public int BottomMargin { get; set; } = 50; // pixels from bottom
    
    // Preset styles
    public static CaptionStyle YouTube => new CaptionStyle
    {
        FontName = "Arial",
        FontSize = 28,
        TextColor = "#FFFFFF",
        OutlineColor = "#000000",
        OutlineWidth = 3,
        Bold = 1,
        Alignment = 2,
        BottomMargin = 60
    };

    public static CaptionStyle TikTok => new CaptionStyle
    {
        FontName = "Arial",
        FontSize = 32,
        TextColor = "#FFFFFF",
        OutlineColor = "#000000",
        OutlineWidth = 4,
        Bold = 1,
        Alignment = 5, // Center
        BottomMargin = 200
    };

    public static CaptionStyle Minimal => new CaptionStyle
    {
        FontName = "Arial",
        FontSize = 20,
        TextColor = "#FFFFFF",
        OutlineColor = "#000000",
        OutlineWidth = 1,
        Bold = 0,
        Alignment = 2,
        BottomMargin = 40
    };
}
