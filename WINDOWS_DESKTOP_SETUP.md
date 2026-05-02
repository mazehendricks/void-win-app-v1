# Windows Desktop Setup Guide

## For Users Downloading from GitHub

This guide is for users who download the ZIP file from GitHub and want to run the application on their Windows desktop.

## Step 1: Download the Project

### Option A: Download ZIP
1. Go to https://github.com/mazehendricks/void-win-app-v1
2. Click the green **"Code"** button
3. Click **"Download ZIP"**
4. Extract the ZIP file to a folder (e.g., `C:\Projects\void-win-app-v1`)

### Option B: Clone with Git
```cmd
git clone https://github.com/mazehendricks/void-win-app-v1.git
cd void-win-app-v1
```

## Step 2: Install Prerequisites

### Install .NET 8 SDK

1. **Download .NET 8 SDK**
   - Go to: https://dotnet.microsoft.com/download/dotnet/8.0
   - Click **"Download .NET SDK x64"** (for 64-bit Windows)
   - Run the installer
   - Follow the installation wizard

2. **Verify Installation**
   - Open Command Prompt (Win+R, type `cmd`, press Enter)
   - Type: `dotnet --version`
   - You should see something like: `8.0.xxx`

**If you see an error**: .NET SDK is not installed correctly. Reinstall and restart your computer.

## Step 3: Build the Application

### Method 1: Using build.bat (Easiest)

1. Open the project folder in File Explorer
2. Double-click `build.bat`
3. Wait for the build to complete
4. Press any key to close the window

**Expected Output:**
```
================================
Void Video Generator - Build
================================

Restoring NuGet packages...
Building application...

================================
Build completed successfully!
================================

Output: src\bin\Release\net8.0-windows\VoidVideoGenerator.exe
```

### Method 2: Using Command Prompt

1. Open Command Prompt
2. Navigate to the project folder:
   ```cmd
   cd C:\Projects\void-win-app-v1
   ```
3. Run the build command:
   ```cmd
   dotnet build src\VoidVideoGenerator.csproj --configuration Release
   ```

### Method 3: Using PowerShell

1. Open PowerShell
2. Navigate to the project folder:
   ```powershell
   cd C:\Projects\void-win-app-v1
   ```
3. Run the build command:
   ```powershell
   dotnet build src\VoidVideoGenerator.csproj --configuration Release
   ```

## Step 4: Run the Application

### Option A: Using run.bat
1. Double-click `run.bat` in the project folder
2. The application window should open

### Option B: Run the EXE directly
1. Navigate to: `src\bin\Release\net8.0-windows\`
2. Double-click `VoidVideoGenerator.exe`

### Option C: Using Command Prompt
```cmd
cd src
dotnet run
```

## Common Issues and Solutions

### Issue 1: "build.bat is not recognized" or "Nothing happens"

**Cause**: Windows security is blocking the batch file

**Solution**:
1. Right-click `build.bat`
2. Select **"Properties"**
3. If you see a security warning at the bottom, check **"Unblock"**
4. Click **"Apply"** then **"OK"**
5. Try running `build.bat` again

### Issue 2: ".NET SDK not found"

**Cause**: .NET 8 SDK is not installed or not in PATH

**Solution**:
1. Download and install .NET 8 SDK: https://dotnet.microsoft.com/download/dotnet/8.0
2. **Restart your computer** (important!)
3. Open Command Prompt and verify: `dotnet --version`
4. Try building again

### Issue 3: "Cannot find project file"

**Cause**: Running build.bat from the wrong location

**Solution**:
1. Make sure you extracted the ZIP file completely
2. Navigate to the root folder (where `build.bat` is located)
3. The folder structure should look like:
   ```
   void-win-app-v1/
   ├── build.bat
   ├── run.bat
   ├── src/
   │   ├── VoidVideoGenerator.csproj
   │   ├── MainForm.cs
   │   └── ...
   └── ...
   ```
4. Run `build.bat` from this root folder

### Issue 4: "NuGet package restore failed"

**Cause**: Internet connection issues or NuGet cache problems

**Solution**:
1. Check your internet connection
2. Clear NuGet cache:
   ```cmd
   dotnet nuget locals all --clear
   ```
3. Try building again:
   ```cmd
   build.bat
   ```

### Issue 5: "Access denied" or "Permission error"

**Cause**: Antivirus or Windows Defender blocking the build

**Solution**:
1. Temporarily disable antivirus
2. Add the project folder to antivirus exclusions
3. Run Command Prompt as Administrator:
   - Right-click Command Prompt
   - Select **"Run as administrator"**
   - Navigate to project folder
   - Run: `build.bat`

### Issue 6: "Build succeeded but no EXE file"

**Cause**: Looking in the wrong output folder

**Solution**:
The EXE is located at:
```
src\bin\Release\net8.0-windows\VoidVideoGenerator.exe
```

Or for Debug builds:
```
src\bin\Debug\net8.0-windows\VoidVideoGenerator.exe
```

### Issue 7: "Application won't start" or "Crashes immediately"

**Cause**: Missing dependencies or configuration

**Solution**:
1. Make sure .NET 8 Runtime is installed (comes with SDK)
2. Check if `config.json` exists in the same folder as the EXE
3. If not, copy `src\config.example.json` to `src\config.json`
4. Try running from Command Prompt to see error messages:
   ```cmd
   cd src\bin\Release\net8.0-windows
   VoidVideoGenerator.exe
   ```

## Step-by-Step First-Time Setup

### Complete Setup Process

1. **Download and Extract**
   ```
   - Download ZIP from GitHub
   - Extract to C:\Projects\void-win-app-v1
   - Unblock all files if prompted
   ```

2. **Install .NET 8 SDK**
   ```
   - Download from https://dotnet.microsoft.com/download/dotnet/8.0
   - Install
   - Restart computer
   - Verify: dotnet --version
   ```

3. **Build the Application**
   ```
   - Open project folder
   - Double-click build.bat
   - Wait for completion
   ```

4. **Configure the Application**
   ```
   - Copy src\config.example.json to src\config.json
   - Edit config.json with your API keys (or use the UI later)
   ```

5. **Run the Application**
   ```
   - Double-click run.bat
   - Or run src\bin\Release\net8.0-windows\VoidVideoGenerator.exe
   ```

6. **Configure API Keys in UI**
   ```
   - Open the application
   - Go to Settings tab
   - Enter your API keys
   - Click "Save All Settings"
   ```

## Alternative: Pre-built Release

If building from source is too complicated, you can request a pre-built release:

1. Go to the GitHub repository
2. Check the **"Releases"** section
3. Download the latest release ZIP
4. Extract and run `VoidVideoGenerator.exe`

**Note**: Pre-built releases may not be available yet. Building from source is recommended.

## Verifying Your Setup

### Check .NET Installation
```cmd
dotnet --version
```
Expected: `8.0.xxx`

### Check Project Structure
```cmd
dir src\VoidVideoGenerator.csproj
```
Expected: File should exist

### Check Build Output
```cmd
dir src\bin\Release\net8.0-windows\VoidVideoGenerator.exe
```
Expected: File should exist after building

## Getting Help

If you're still having issues:

1. **Check the error message carefully**
   - Take a screenshot
   - Note the exact error text

2. **Check existing documentation**
   - [`TROUBLESHOOTING.md`](TROUBLESHOOTING.md)
   - [`BUILD_GUIDE.md`](BUILD_GUIDE.md)
   - [`QUICKSTART.md`](QUICKSTART.md)

3. **Provide details when asking for help**
   - Windows version
   - .NET SDK version (`dotnet --version`)
   - Exact error message
   - What you've already tried

## Quick Command Reference

| Task | Command |
|------|---------|
| **Check .NET version** | `dotnet --version` |
| **Build (Release)** | `build.bat` or `dotnet build src\VoidVideoGenerator.csproj --configuration Release` |
| **Build (Debug)** | `dotnet build src\VoidVideoGenerator.csproj --configuration Debug` |
| **Run application** | `run.bat` or `dotnet run --project src\VoidVideoGenerator.csproj` |
| **Clean build** | `dotnet clean src\VoidVideoGenerator.csproj` |
| **Clear NuGet cache** | `dotnet nuget locals all --clear` |

## Next Steps

After successfully building and running:

1. **Install Piper TTS** (for voice generation)
   - Run `install-piper.bat`
   - Follow the prompts

2. **Install FFmpeg** (for video assembly)
   - Download from: https://ffmpeg.org/download.html
   - Add to PATH or specify path in Settings

3. **Configure API Keys**
   - Open application
   - Go to Settings tab
   - Enter your API keys
   - Click "Save All Settings"

4. **Generate your first video**
   - Go to Generate Video tab
   - Enter a title
   - Click "Generate Video"

## Summary

✅ **For Windows Desktop Users:**
1. Install .NET 8 SDK
2. Extract the ZIP file
3. Run `build.bat`
4. Run `run.bat` or the EXE directly
5. Configure API keys in the Settings tab

The application is now ready to use!
