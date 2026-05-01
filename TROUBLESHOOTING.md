# Troubleshooting Guide

## Quick Test Script

Before diving into troubleshooting, run the included test script:

```bash
# Windows
test-ollama.bat

# This will:
# 1. Check if Ollama is running
# 2. List installed models
# 3. Test generation with a simple prompt
# 4. Show you the response
```

If this test passes, your Ollama setup is correct and the issue is elsewhere.

## Common Issues and Solutions

### 1. LLM Request Timeout / "Waiting for LLM response..."

**Symptoms:**
- Application hangs at "Waiting for LLM response..."
- Eventually times out after 5 minutes
- **Ollama server shows no activity** (no logs, no CPU usage)

**Possible Causes & Solutions:**

#### A. Ollama is not running
```bash
# Check if Ollama is running
curl http://localhost:11434/api/tags

# If not running, start Ollama in a separate terminal
ollama serve
```

**Important:** Keep the `ollama serve` terminal window open while using the app.

#### B. Model not downloaded
```bash
# List available models
ollama list

# Download the model if missing (default: llama3.1)
ollama pull llama3.1

# Or use a smaller/faster model
ollama pull llama3.2
```

#### C. Model is too slow for your hardware
- Try a smaller model: `llama3.2:1b` or `phi3:mini`
- Increase timeout in [`OllamaScriptGenerator.cs`](src/Services/OllamaScriptGenerator.cs:22)
- Use GPU acceleration if available

#### D. Request not reaching Ollama (no server activity)

**Symptoms:**
- App says "Waiting for LLM response..."
- Ollama terminal shows NO activity at all
- Eventually times out

**Diagnosis:**
1. Run the test script: `test-ollama.bat`
2. If test script works but app doesn't, there's a configuration issue

**Solutions:**
1. **Check the config file** (`config.json`):
   ```json
   {
     "OllamaUrl": "http://localhost:11434",
     "OllamaModel": "llama3.1"
   }
   ```

2. **Verify Ollama URL in app settings:**
   - Open Settings tab
   - Check "Ollama URL" field
   - Should be: `http://localhost:11434` (default)
   - Click "Save Settings" after any changes

3. **Restart the application** after changing settings

4. **Check Windows Firewall:**
   - Ensure localhost connections are allowed
   - Try temporarily disabling firewall to test

5. **Try the diagnostic in the app:**
   - Go to "System Status" tab
   - Click "Check System Status"
   - Look at the "OLLAMA DIAGNOSTICS" section
   - It will tell you exactly what's wrong

#### E. Ollama running on different port/host
- Check your Ollama configuration
- Update the base URL in your config file or when initializing the service
- Default is `http://localhost:11434`

### 2. Script Generation Fails

**Symptoms:**
- "Failed to generate script from LLM" error
- Empty or malformed response

**Solutions:**

#### Check Ollama Response Format
```bash
# Test Ollama directly
curl http://localhost:11434/api/generate -d '{
  "model": "llama3.1",
  "prompt": "Write a short video script",
  "stream": false
}'
```

Expected response format:
```json
{
  "response": "Your script text here...",
  "model": "llama3.1",
  "done": true
}
```

#### Verify Model Works
```bash
# Interactive test
ollama run llama3.1 "Write a 30-second video script about AI"
```

### 3. Audio Generation Issues

**Symptoms:**
- "Piper TTS not available" error
- No audio files generated

**Solutions:**

#### A. Piper not installed
```bash
# Download Piper for your platform
# Windows: https://github.com/rhasspy/piper/releases
# Extract to a known location

# Test Piper
piper --version
```

#### B. Voice model not found
```bash
# Download a voice model
# Example: en_US-lessac-medium
# Place in: models/piper/

# Test voice generation
echo "Hello world" | piper --model models/piper/en_US-lessac-medium.onnx --output_file test.wav
```

### 4. Video Assembly Fails

**Symptoms:**
- "FFmpeg not available" error
- Video file not created

**Solutions:**

#### A. FFmpeg not installed
```bash
# Windows: Download from https://ffmpeg.org/download.html
# Add to PATH or specify full path in config

# Test FFmpeg
ffmpeg -version
```

#### B. Missing audio/visual files
- Check the output directory for audio files
- Verify script.txt was generated
- Check logs for earlier errors

### 5. Performance Issues

**Optimization Tips:**

1. **Use a faster LLM model:**
   ```bash
   ollama pull llama3.2:1b  # Smaller, faster
   ```

2. **Enable GPU acceleration:**
   - Ensure CUDA/ROCm is installed for Ollama
   - Check GPU usage: `nvidia-smi` (NVIDIA) or `rocm-smi` (AMD)

3. **Reduce script length:**
   - Lower target duration (30-60 seconds instead of 120+)
   - Shorter scripts = faster generation

4. **Parallel processing:**
   - The pipeline already processes steps sequentially
   - Audio generation for segments could be parallelized (future enhancement)

### 6. Configuration Issues

**Check your config file:**

```json
{
  "OllamaBaseUrl": "http://localhost:11434",
  "OllamaModel": "llama3.1",
  "PiperExecutablePath": "C:\\path\\to\\piper.exe",
  "PiperVoiceModel": "en_US-lessac-medium",
  "FFmpegPath": "ffmpeg",
  "DefaultOutputPath": "C:\\Users\\YourName\\Videos\\Void Gen Output"
}
```

**Common mistakes:**
- Wrong path separators (use `\\` in JSON for Windows paths)
- Incorrect Ollama URL (check port number)
- Missing voice model files
- FFmpeg not in PATH

### 7. Debugging Tips

#### Enable Verbose Logging
The application reports progress through the UI. Watch for:
- "Generating script with local LLM..."
- "Received response (X chars), parsing..."
- "Script generated successfully (X chars)"

#### Check Output Directory
After each run, check:
```
Output Directory/
├── script.txt          # Generated script
├── audio/              # Audio segments
│   ├── segment_0.wav
│   └── segment_1.wav
└── visuals/            # Visual assets (placeholder)
```

#### Test Components Individually

**Test Ollama:**
```bash
curl http://localhost:11434/api/generate -d '{
  "model": "llama3.1",
  "prompt": "Test",
  "stream": false
}'
```

**Test Piper:**
```bash
echo "Test" | piper --model path/to/model.onnx --output_file test.wav
```

**Test FFmpeg:**
```bash
ffmpeg -version
```

### 8. Getting Help

If you're still experiencing issues:

1. **Check the logs** - Look for error messages in the application output
2. **Verify all dependencies** - Ollama, Piper, FFmpeg all working independently
3. **Test with minimal settings** - Use smallest model, shortest duration
4. **Check system resources** - Ensure enough RAM/disk space
5. **Review recent changes** - Did you update any components?

### Quick Diagnostic Checklist

- [ ] Ollama is running (`curl http://localhost:11434/api/tags`)
- [ ] Model is downloaded (`ollama list`)
- [ ] Piper is installed and accessible
- [ ] Voice model files exist
- [ ] FFmpeg is installed and in PATH
- [ ] Config file has correct paths
- [ ] Output directory is writable
- [ ] Sufficient disk space available
- [ ] No firewall blocking localhost connections

### Performance Benchmarks

Typical generation times (varies by hardware):

| Component | Fast (GPU) | Medium (CPU) | Slow (Old CPU) |
|-----------|-----------|--------------|----------------|
| Script (60s video) | 5-15s | 30-60s | 2-5 min |
| Audio generation | 2-5s | 5-10s | 10-20s |
| Video assembly | 5-10s | 10-20s | 20-40s |
| **Total** | **15-30s** | **45-90s** | **3-6 min** |

If your times are significantly longer, consider:
- Using a smaller LLM model
- Reducing video duration
- Checking for background processes consuming resources
