# Piper TTS Installation Fix

## Issue
The Piper TTS installation scripts were failing with a 404 error because they were trying to download from an outdated release version.

### Error Message
```
curl: (22) The requested URL returned error: 404
ERROR: Failed to download Piper after 3 attempts
URL: https://github.com/rhasspy/piper/releases/download/v1.2.0/piper_windows_amd64.zip
```

## Root Cause
The scripts were configured to download Piper version `v1.2.0`, which doesn't exist in the GitHub releases. The actual latest version is `2023.11.14-2`.

## Solution
Updated all three installation scripts with the correct version:

### Files Updated
1. **install-piper.bat** (Windows Batch)
2. **install-piper.ps1** (Windows PowerShell)
3. **install-piper.sh** (Linux/Codespaces)

### Changes Made
Changed the version from:
```
PIPER_VERSION="v1.2.0"
```

To:
```
PIPER_VERSION="2023.11.14-2"
```

## How to Use

### Windows (Batch Script)
```cmd
install-piper.bat
```

### Windows (PowerShell)
```powershell
powershell -ExecutionPolicy Bypass -File install-piper.ps1
```

### Linux/Codespaces
```bash
chmod +x install-piper.sh
./install-piper.sh
```

## What Gets Installed

### Windows
- **Piper executable**: `C:\Tools\piper\piper.exe`
- **Voice model**: `C:\Tools\piper\models\voice.onnx`
- **Model config**: `C:\Tools\piper\models\voice.onnx.json`

### Linux
- **Piper executable**: `~/.local/bin/piper` (or `/usr/local/bin/piper` if run with sudo)
- **Voice model**: `./models/voice.onnx`
- **Model config**: `./models/voice.onnx.json`

## Configuration
After installation, update your `config.json`:

```json
{
  "PiperPath": "C:\\Tools\\piper\\piper.exe",
  "PiperModelPath": "C:\\Tools\\piper\\models\\voice.onnx"
}
```

For Linux:
```json
{
  "PiperPath": "piper",
  "PiperModelPath": "models/voice.onnx"
}
```

## Verification
The installation scripts automatically test the installation by:
1. Running `piper --version` to verify the executable works
2. Generating a test audio file (`test.wav`) to verify voice synthesis

If both tests pass, the installation is successful.

## Additional Voice Models
The default installation includes the `en_US-lessac-medium` voice model. For more voice options, visit:
- https://github.com/rhasspy/piper/blob/master/VOICES.md
- https://huggingface.co/rhasspy/piper-voices

## Troubleshooting

### If Download Still Fails
1. Check your internet connection
2. Verify GitHub is accessible from your network
3. Try downloading manually from: https://github.com/rhasspy/piper/releases/download/2023.11.14-2/piper_windows_amd64.zip
4. Extract to `C:\Tools\piper` (Windows) or `~/.local/bin` (Linux)

### If Voice Model Download Fails
Download manually from HuggingFace:
- https://huggingface.co/rhasspy/piper-voices/resolve/main/en/en_US/lessac/medium/en_US-lessac-medium.onnx
- https://huggingface.co/rhasspy/piper-voices/resolve/main/en/en_US/lessac/medium/en_US-lessac-medium.onnx.json

## Related Files
- [`install-piper.bat`](install-piper.bat) - Windows batch installation script
- [`install-piper.ps1`](install-piper.ps1) - Windows PowerShell installation script
- [`install-piper.sh`](install-piper.sh) - Linux/Codespaces installation script
- [`SETUP_GUIDE.md`](SETUP_GUIDE.md) - General setup instructions
- [`RUNTIME_REQUIREMENTS.md`](RUNTIME_REQUIREMENTS.md) - Runtime dependencies

## Status
✅ **FIXED** - All installation scripts now use the correct Piper version (2023.11.14-2)
