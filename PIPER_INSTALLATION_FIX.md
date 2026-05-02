# Piper TTS Installation Fix Guide

## Problem
You encountered the error: `Central Directory corrupt` when trying to install Piper TTS using the `install-piper.bat` script.

## Root Cause
This error occurs when:
1. The ZIP file download was interrupted or incomplete
2. Network issues caused file corruption during download
3. The GitHub release server had temporary issues
4. Antivirus software interfered with the download

## Solutions

### Solution 1: Use the Improved PowerShell Script (Recommended)
I've created an improved PowerShell script with better error handling and retry logic:

```powershell
powershell -ExecutionPolicy Bypass -File install-piper.ps1
```

**Benefits:**
- Automatic retry on download failure (up to 3 attempts)
- ZIP file validation before extraction
- Better error messages and troubleshooting guidance
- Automatic cleanup of corrupted files
- Progress indicators

### Solution 2: Use the Updated Batch Script
The updated `install-piper.bat` now includes:
- Download retry logic
- File size validation
- Better ZIP extraction method
- Automatic cleanup of corrupted downloads

Simply run:
```batch
install-piper.bat
```

### Solution 3: Manual Installation
If both scripts fail, install manually:

#### Step 1: Download Piper
1. Go to: https://github.com/rhasspy/piper/releases/tag/v1.2.0
2. Download: `piper_windows_amd64.zip`
3. Save to a location you can find (e.g., Downloads folder)

#### Step 2: Verify Download
- Check the file size (should be ~10-20 MB)
- If it's very small (< 1 MB), the download failed - try again

#### Step 3: Extract Files
1. Right-click the ZIP file
2. Select "Extract All..."
3. Choose destination: `C:\Tools\piper`
4. Click "Extract"

**If extraction fails:**
- Try using 7-Zip or WinRAR instead of Windows built-in extractor
- Re-download the file (it may be corrupted)

#### Step 4: Download Voice Model
1. Create folder: `C:\Tools\piper\models`
2. Download these files:
   - https://huggingface.co/rhasspy/piper-voices/resolve/main/en/en_US/lessac/medium/en_US-lessac-medium.onnx
   - https://huggingface.co/rhasspy/piper-voices/resolve/main/en/en_US/lessac/medium/en_US-lessac-medium.onnx.json
3. Save both files to `C:\Tools\piper\models\` and rename:
   - `en_US-lessac-medium.onnx` → `voice.onnx`
   - `en_US-lessac-medium.onnx.json` → `voice.onnx.json`

#### Step 5: Test Installation
Open Command Prompt and run:
```batch
C:\Tools\piper\piper.exe --version
```

If successful, test voice generation:
```batch
echo Hello world | C:\Tools\piper\piper.exe --model C:\Tools\piper\models\voice.onnx --output_file test.wav
```

### Solution 4: Clean Up and Retry
If you've tried before and it failed:

1. **Delete corrupted files:**
   ```batch
   del %TEMP%\piper.zip
   rmdir /S /Q %TEMP%\piper
   ```

2. **Clear browser/download cache** (if downloading manually)

3. **Temporarily disable antivirus** (it may be blocking the download)

4. **Try a different network** (if possible)

5. **Run the script again:**
   ```batch
   install-piper.bat
   ```
   or
   ```powershell
   powershell -ExecutionPolicy Bypass -File install-piper.ps1
   ```

## Configuration

After successful installation, update your `config.json`:

```json
{
  "PiperPath": "C:\\Tools\\piper\\piper.exe",
  "PiperModelPath": "C:\\Tools\\piper\\models\\voice.onnx"
}
```

## Alternative: Use Different TTS Service

If Piper installation continues to fail, you can use alternative TTS services:

### Option 1: Windows Built-in TTS
Update your config.json:
```json
{
  "VoiceGenerator": "Windows",
  "UseWindowsTTS": true
}
```

### Option 2: Online TTS Services
- Google Cloud Text-to-Speech
- Amazon Polly
- Microsoft Azure Speech

## Troubleshooting Common Issues

### Issue: "Access Denied" Error
**Solution:** Run the script as Administrator
- Right-click `install-piper.bat` or `install-piper.ps1`
- Select "Run as administrator"

### Issue: "curl not found"
**Solution:** Update Windows or install curl
- Windows 10 (1803+) and Windows 11 include curl
- Or download from: https://curl.se/windows/

### Issue: PowerShell Execution Policy Error
**Solution:** Run with bypass flag
```powershell
powershell -ExecutionPolicy Bypass -File install-piper.ps1
```

### Issue: Slow Download Speed
**Solution:** 
- Use a wired connection instead of WiFi
- Try downloading during off-peak hours
- Use a VPN if GitHub is throttled in your region

### Issue: Antivirus Blocking
**Solution:**
- Temporarily disable antivirus
- Add exception for `C:\Tools\piper`
- Add exception for the download scripts

## Getting Help

If you continue to have issues:

1. **Check the error message carefully** - it often contains the solution
2. **Try the PowerShell script** - it has better error handling
3. **Install manually** - follow Solution 3 above
4. **Check GitHub Issues**: https://github.com/rhasspy/piper/issues
5. **Use alternative TTS** - Windows TTS or online services

## What Changed in the Fixed Scripts

### install-piper.bat
- ✅ Added download retry logic (3 attempts)
- ✅ Added file size validation
- ✅ Improved ZIP extraction method
- ✅ Better error messages
- ✅ Automatic cleanup of corrupted files

### install-piper.ps1 (New)
- ✅ PowerShell-native implementation
- ✅ ZIP validation before extraction
- ✅ Robust error handling
- ✅ Progress indicators
- ✅ Colored output for better readability
- ✅ Automatic retry with exponential backoff

## Quick Start

**Recommended approach:**

1. Open PowerShell as Administrator
2. Navigate to your project directory:
   ```powershell
   cd C:\path\to\void-win-app-v1
   ```
3. Run the PowerShell script:
   ```powershell
   .\install-piper.ps1
   ```
4. Wait for installation to complete
5. Update your `config.json` with the paths shown
6. Test your application

If PowerShell script fails, try the batch script:
```batch
install-piper.bat
```

If both fail, follow the manual installation steps in Solution 3.

## Success Indicators

You'll know the installation succeeded when:
- ✅ `C:\Tools\piper\piper.exe` exists
- ✅ `C:\Tools\piper\models\voice.onnx` exists
- ✅ Running `piper.exe --version` shows version info
- ✅ A test.wav file is created successfully

## Next Steps After Installation

1. ✅ Update `config.json` with Piper paths
2. ✅ Test voice generation in your application
3. ✅ (Optional) Download additional voice models from:
   https://github.com/rhasspy/piper/blob/master/VOICES.md
4. ✅ (Optional) Add Piper to your system PATH for easier access

---

**Note:** The improved scripts automatically handle most common issues. If you still encounter problems, the manual installation method (Solution 3) is the most reliable fallback option.
