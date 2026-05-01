namespace VoidVideoGenerator.Services;

using System.Diagnostics;
using System.Text.Json;

/// <summary>
/// Transcription service using OpenAI Whisper (local or API)
/// </summary>
public class WhisperTranscriptionService : ITranscriptionService
{
    private readonly string _whisperPath;
    private readonly string _modelPath;
    private readonly bool _useApi;
    private readonly string? _apiKey;

    /// <summary>
    /// Create Whisper transcription service
    /// </summary>
    /// <param name="whisperPath">Path to whisper executable (for local) or "api" for OpenAI API</param>
    /// <param name="modelPath">Path to model file (for local only)</param>
    /// <param name="apiKey">OpenAI API key (for API mode only)</param>
    public WhisperTranscriptionService(string whisperPath = "whisper", string modelPath = "", string? apiKey = null)
    {
        _whisperPath = whisperPath;
        _modelPath = modelPath;
        _useApi = whisperPath.ToLower() == "api";
        _apiKey = apiKey;
    }

    public async Task<bool> IsAvailableAsync()
    {
        if (_useApi)
        {
            return !string.IsNullOrEmpty(_apiKey);
        }

        try
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _whisperPath,
                    Arguments = "--version",
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

    public async Task<List<TranscriptionSegment>> TranscribeAsync(string audioOrVideoPath, IProgress<string>? progress = null)
    {
        if (_useApi)
        {
            return await TranscribeWithApiAsync(audioOrVideoPath, progress);
        }
        else
        {
            return await TranscribeLocallyAsync(audioOrVideoPath, progress);
        }
    }

    private async Task<List<TranscriptionSegment>> TranscribeLocallyAsync(string audioOrVideoPath, IProgress<string>? progress = null)
    {
        progress?.Report("Transcribing audio with Whisper...");

        var outputDir = Path.Combine(Path.GetTempPath(), $"whisper_{Guid.NewGuid()}");
        Directory.CreateDirectory(outputDir);

        try
        {
            var arguments = $"\"{audioOrVideoPath}\" --model {_modelPath} --output_dir \"{outputDir}\" --output_format json --word_timestamps True";

            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _whisperPath,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            
            var errorTask = process.StandardError.ReadToEndAsync();
            var outputTask = process.StandardOutput.ReadToEndAsync();
            
            await process.WaitForExitAsync();
            
            var error = await errorTask;

            if (process.ExitCode != 0)
            {
                throw new Exception($"Whisper transcription failed: {error}");
            }

            // Read the JSON output
            var jsonFile = Directory.GetFiles(outputDir, "*.json").FirstOrDefault();
            if (jsonFile == null)
            {
                throw new Exception("Whisper did not produce a JSON output file");
            }

            var jsonContent = await File.ReadAllTextAsync(jsonFile);
            var result = JsonSerializer.Deserialize<WhisperResult>(jsonContent);

            if (result?.Segments == null)
            {
                throw new Exception("Failed to parse Whisper output");
            }

            progress?.Report($"Transcription complete: {result.Segments.Count} segments");

            return result.Segments.Select(s => new TranscriptionSegment
            {
                Text = s.Text?.Trim() ?? "",
                StartTime = s.Start,
                EndTime = s.End
            }).ToList();
        }
        finally
        {
            // Cleanup temp directory
            try
            {
                if (Directory.Exists(outputDir))
                {
                    Directory.Delete(outputDir, true);
                }
            }
            catch { }
        }
    }

    private async Task<List<TranscriptionSegment>> TranscribeWithApiAsync(string audioOrVideoPath, IProgress<string>? progress = null)
    {
        progress?.Report("Transcribing audio with OpenAI Whisper API...");

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

        using var form = new MultipartFormDataContent();
        using var fileStream = File.OpenRead(audioOrVideoPath);
        using var fileContent = new StreamContent(fileStream);
        
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
        form.Add(fileContent, "file", Path.GetFileName(audioOrVideoPath));
        form.Add(new StringContent("whisper-1"), "model");
        form.Add(new StringContent("verbose_json"), "response_format");
        form.Add(new StringContent("true"), "timestamp_granularities[]");

        var response = await httpClient.PostAsync("https://api.openai.com/v1/audio/transcriptions", form);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"OpenAI Whisper API error: {response.StatusCode} - {responseContent}");
        }

        var result = JsonSerializer.Deserialize<WhisperResult>(responseContent);

        if (result?.Segments == null)
        {
            throw new Exception("Failed to parse Whisper API response");
        }

        progress?.Report($"Transcription complete: {result.Segments.Count} segments");

        return result.Segments.Select(s => new TranscriptionSegment
        {
            Text = s.Text?.Trim() ?? "",
            StartTime = s.Start,
            EndTime = s.End
        }).ToList();
    }

    private class WhisperResult
    {
        public List<WhisperSegment>? Segments { get; set; }
    }

    private class WhisperSegment
    {
        public string? Text { get; set; }
        public double Start { get; set; }
        public double End { get; set; }
    }
}
