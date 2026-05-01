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
    private static readonly System.Text.RegularExpressions.Regex VisualCueRegex =
        new System.Text.RegularExpressions.Regex(@"\[([^\]]+)\]", System.Text.RegularExpressions.RegexOptions.Compiled);

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
        var cleanText = VisualCueRegex.Replace(text, "");

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

            // Start reading error output asynchronously before starting the process
            var errorOutputBuilder = new System.Text.StringBuilder();
            var standardOutputBuilder = new System.Text.StringBuilder();

            try
            {
                process.Start();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to start Piper TTS. Make sure Piper is installed and the path '{_piperPath}' is correct. Error: {ex.Message}");
            }

            // Read output streams asynchronously to prevent deadlocks
            var errorTask = Task.Run(async () =>
            {
                while (!process.StandardError.EndOfStream)
                {
                    var line = await process.StandardError.ReadLineAsync();
                    if (line != null)
                    {
                        errorOutputBuilder.AppendLine(line);
                    }
                }
            });

            var outputTask = Task.Run(async () =>
            {
                while (!process.StandardOutput.EndOfStream)
                {
                    var line = await process.StandardOutput.ReadLineAsync();
                    if (line != null)
                    {
                        standardOutputBuilder.AppendLine(line);
                    }
                }
            });

            // Write text to stdin
            await process.StandardInput.WriteAsync(cleanText);
            process.StandardInput.Close();

            // Wait for process and output reading to complete
            await process.WaitForExitAsync();
            await Task.WhenAll(errorTask, outputTask);

            if (process.ExitCode != 0)
            {
                var errorOutput = errorOutputBuilder.ToString().Trim();
                var standardOutput = standardOutputBuilder.ToString().Trim();
                
                var errorMessage = !string.IsNullOrEmpty(errorOutput) ? errorOutput :
                                   !string.IsNullOrEmpty(standardOutput) ? standardOutput :
                                   $"Process exited with code {process.ExitCode}";
                
                // Check for common issues
                if (errorMessage.Contains("No such file") || errorMessage.Contains("not found"))
                {
                    throw new Exception($"Piper TTS model not found. Please ensure the model file exists at: {_modelPath}");
                }
                
                throw new Exception($"Piper TTS failed: {errorMessage}");
            }

            // Verify output file was created
            if (!File.Exists(outputPath))
            {
                throw new Exception($"Piper TTS completed but output file was not created: {outputPath}");
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
