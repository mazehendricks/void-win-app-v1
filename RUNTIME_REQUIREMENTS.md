# Runtime Requirements & Installation Guide

## Current Status
✅ **Build Status**: All nullable reference warnings fixed - builds successfully with 0 errors and 0 warnings
❗ **Runtime Status**: Requires .NET 8 SDK to run on Windows

## System Requirements

### Operating System
- **Windows 10** or later (Windows 11 recommended)
- **Windows Forms** application - does not run on Linux or macOS

### .NET Runtime
- **.NET 8.0 SDK** (required)
- Download: https://dotnet.microsoft.com/download/dotnet/8.0

## Installation Steps

### 1. Install .NET 8 SDK

#### Option A: Direct Download
1. Visit https://dotnet.microsoft.com/download/dotnet/8.0
2. Download the **.NET 8.0 SDK** installer for Windows
3. Run the installer and follow the prompts
4. Restart your terminal/command prompt after installation

#### Option B: Using winget (Windows Package Manager)
```cmd
winget install Microsoft.DotNet.SDK.8
```

#### Option C: Using Chocolatey
```cmd
choco install dotnet-8.0-sdk
```

### 2. Verify Installation
Open a new command prompt and run:
```cmd
dotnet --version
```

You should see version 8.0.x displayed.

### 3. Build the Application
```cmd
build.bat
```

Or manually:
```cmd
cd src
dotnet build VoidVideoGenerator.csproj
```

### 4. Run the Application
```cmd
run.bat
```

Or manually:
```cmd
cd src
dotnet run --project VoidVideoGenerator.csproj
```

## Troubleshooting

### Error: "You must install or update .NET to run this application"
**Cause**: .NET 8.0 SDK is not installed or not in PATH

**Solution**:
1. Install .NET 8.0 SDK from https://dotnet.microsoft.com/download/dotnet/8.0
2. Restart your terminal
3. Verify with `dotnet --version`

### Error: "Framework 'Microsoft.NETCore.App', version '8.0.0' not found"
**Cause**: Wrong .NET version installed (e.g., .NET 9 or 10 instead of .NET 8)

**Solution**:
- Install .NET 8.0 SDK specifically (not just the runtime)
- Multiple .NET versions can coexist on the same machine

### Build Succeeds but Application Won't Start
**Cause**: Missing dependencies or configuration issues

**Solution**:
1. Run `dotnet restore` in the src directory
2. Check that `config.json` exists (copy from `config.example.json` if needed)
3. Ensure all required services are configured (Ollama, FFmpeg, etc.)

### Running on Linux/macOS
**Not Supported**: This is a Windows Forms application that requires Windows to run.

**Alternative**: Consider using Windows Subsystem for Linux (WSL) with WSLg for GUI support, though this is experimental.

## Project Configuration

### Target Framework
```xml
<TargetFramework>net8.0-windows</TargetFramework>
```

This specifies:
- **.NET 8.0**: The runtime version
- **-windows**: Windows-specific APIs (Windows Forms)

### Why .NET 8?
- Modern C# features (nullable reference types, pattern matching, etc.)
- Improved performance over older versions
- Long-term support (LTS) release
- Better Windows Forms designer support

## Upgrading to .NET 9 or 10 (Optional)

If you want to use a newer .NET version:

1. Edit `src/VoidVideoGenerator.csproj`
2. Change `<TargetFramework>net8.0-windows</TargetFramework>` to:
   - `net9.0-windows` for .NET 9
   - `net10.0-windows` for .NET 10

3. Update package references if needed
4. Rebuild: `dotnet build`

**Note**: .NET 8 is recommended as it's an LTS release with better stability.

## Development Environment

### Recommended IDEs
- **Visual Studio 2022** (Community, Professional, or Enterprise)
  - Best Windows Forms designer support
  - Integrated debugging
  - NuGet package management

- **Visual Studio Code** with C# extension
  - Lightweight alternative
  - Good for code editing
  - Limited Windows Forms designer support

- **JetBrains Rider**
  - Cross-platform .NET IDE
  - Excellent code analysis
  - Good Windows Forms support

## Additional Dependencies

### Required Services (for full functionality)
- **Ollama** (for local AI script generation)
- **FFmpeg** (for video assembly)
- **Piper TTS** (for voice generation)

See [`SETUP_GUIDE.md`](SETUP_GUIDE.md) for detailed service installation instructions.

## Summary

The nullable reference warnings have been successfully fixed. The application builds without errors. To run the application:

1. ✅ Install .NET 8.0 SDK on Windows
2. ✅ Run `build.bat` to build
3. ✅ Run `run.bat` to start the application

The error message you're seeing is expected behavior when .NET 8 is not installed - it's not a code issue, but a runtime environment requirement.
