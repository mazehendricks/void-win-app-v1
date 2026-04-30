namespace VoidVideoGenerator.Services;

using System.Diagnostics;
using VoidVideoGenerator.Models;

/// <summary>
/// Voice generator using local Piper TTS
/// </summary>
public class PiperTTSService : IVoiceGeneratorService
{
    private readonly string _piperPath;
    private readonly string _modelPath;

    public PiperTTSService(string piperPath = "piper", string modelPath = "models/voice.onnx")
    {
        _piperPath = piperPath;
        _modelPath = modelPath;
    }

    public async Task<bool> IsAvailableAsync()
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _piperPath,
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

    public async Task<string> GenerateAudioAsync(string text, string outputPath, IProgress<string>? progress = null)
    {
        progress?.Report($"Generating audio: {Path.GetFileName(outputPath)}");

        // Ensure output directory exists
        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Clean text for TTS (remove visual cues)
        var cleanText = System.Text.RegularExpressions.Regex.Replace(text, @"\[([^\]]+)\]", "");

        // Create temp text file
        var tempTextFile = Path.GetTempFileName();
        await File.WriteAllTextAsync(tempTextFile, cleanText);

        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _piperPath,
                    Arguments = $"--model \"{_modelPath}\" --output_file \"{outputPath}\"",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            
            // Write text to stdin
            await process.StandardInput.WriteAsync(cleanText);
            process.StandardInput.Close();

            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                var error = await process.StandardError.ReadToEndAsync();
                throw new Exception($"Piper TTS failed: {error}");
            }

            progress?.Report($"Audio generated: {Path.GetFileName(outputPath)}");
            return outputPath;
        }
        finally
        {
            if (File.Exists(tempTextFile))
            {
                File.Delete(tempTextFile);
            }
        }
    }

    public async Task<List<string>> GenerateScriptAudioAsync(VideoScript script, string outputDirectory, IProgress<string>? progress = null)
    {
        Directory.CreateDirectory(outputDirectory);
        var audioFiles = new List<string>();

        for (int i = 0; i < script.Segments.Count; i++)
        {
            var segment = script.Segments[i];
            var outputPath = Path.Combine(outputDirectory, $"segment_{i:D3}_{segment.Type}.wav");
            
            progress?.Report($"Generating audio segment {i + 1}/{script.Segments.Count}");
            await GenerateAudioAsync(segment.Text, outputPath, progress);
            
            audioFiles.Add(outputPath);
        }

        return audioFiles;
    }
}
