# Build Guide

## Overview

This guide explains how to build the Void Video Generator application on different platforms.

## Prerequisites

- **.NET 8 SDK** installed
  - Download: https://dotnet.microsoft.com/download/dotnet/8.0
  - Verify: `dotnet --version` (should show 8.0.x)

## Building on Windows

### Option 1: Using build.bat (Recommended)

```cmd
build.bat
```

This script will:
1. Check for the project file
2. Restore NuGet packages
3. Build in Release configuration
4. Display the output location

### Option 2: Using dotnet CLI

```cmd
cd src
dotnet restore VoidVideoGenerator.csproj
dotnet build VoidVideoGenerator.csproj --configuration Release
```

### Option 3: Using Visual Studio

1. Open `void-win-app-v1.sln` in Visual Studio
2. Select **Release** configuration
3. Press **Ctrl+Shift+B** or go to **Build > Build Solution**

## Building on Linux/macOS/Codespaces

**Note**: This is a Windows Forms application and cannot run on Linux/macOS, but you can still build it for deployment to Windows.

### Using dotnet CLI

```bash
# Restore dependencies
dotnet restore src/VoidVideoGenerator.csproj

# Build in Release mode
dotnet build src/VoidVideoGenerator.csproj --configuration Release

# Or build in Debug mode
dotnet build src/VoidVideoGenerator.csproj --configuration Debug
```

### Clean Build

```bash
# Clean previous build artifacts
dotnet clean src/VoidVideoGenerator.csproj

# Then build
dotnet build src/VoidVideoGenerator.csproj --configuration Release
```

## Build Output Locations

### Release Build
```
src/bin/Release/net8.0-windows/VoidVideoGenerator.dll
src/bin/Release/net8.0-windows/VoidVideoGenerator.exe (Windows only)
```

### Debug Build
```
src/bin/Debug/net8.0-windows/VoidVideoGenerator.dll
src/bin/Debug/net8.0-windows/VoidVideoGenerator.exe (Windows only)
```

## Build Configurations

### Release Configuration
- Optimized for performance
- No debug symbols
- Smaller file size
- Use for distribution

```bash
dotnet build src/VoidVideoGenerator.csproj --configuration Release
```

### Debug Configuration
- Includes debug symbols
- Easier to troubleshoot
- Larger file size
- Use for development

```bash
dotnet build src/VoidVideoGenerator.csproj --configuration Debug
```

## Troubleshooting

### Error: "Cannot find project file"

**Problem**: Running build command from wrong directory

**Solution**: 
```bash
# Make sure you're in the project root
cd /path/to/void-win-app-v1

# Then run build
dotnet build src/VoidVideoGenerator.csproj --configuration Release
```

### Error: ".NET SDK not found"

**Problem**: .NET 8 SDK not installed or not in PATH

**Solution**:
1. Download and install .NET 8 SDK from https://dotnet.microsoft.com/download/dotnet/8.0
2. Restart your terminal/command prompt
3. Verify: `dotnet --version`

### Error: "Package restore failed"

**Problem**: NuGet packages cannot be downloaded

**Solution**:
```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore packages
dotnet restore src/VoidVideoGenerator.csproj

# Try building again
dotnet build src/VoidVideoGenerator.csproj --configuration Release
```

### Error: "Build failed with compilation errors"

**Problem**: Code has syntax or compilation errors

**Solution**:
1. Check the error messages in the build output
2. Look for file names and line numbers
3. Fix the errors in the source code
4. Try building again

### Warning: "Platform not supported"

**Problem**: Trying to run Windows Forms app on Linux/macOS

**Note**: This is expected. The application is Windows-only but can be built on any platform. To run it:
1. Build on Linux/macOS/Codespaces
2. Copy the build output to a Windows machine
3. Run the `.exe` file on Windows

## Build Scripts

### Windows: build.bat

Located at: [`build.bat`](build.bat)

Features:
- Checks for project file
- Restores NuGet packages
- Builds in Release configuration
- Shows output location
- Pauses for user to see results

### Linux/macOS: Create build.sh (Optional)

You can create a shell script for convenience:

```bash
#!/bin/bash
echo "================================"
echo "Void Video Generator - Build"
echo "================================"
echo ""

if [ ! -f "src/VoidVideoGenerator.csproj" ]; then
    echo "ERROR: Cannot find src/VoidVideoGenerator.csproj"
    echo "Please run this script from the project root directory"
    exit 1
fi

echo "Restoring NuGet packages..."
dotnet restore src/VoidVideoGenerator.csproj

echo ""
echo "Building application..."
dotnet build src/VoidVideoGenerator.csproj --configuration Release

echo ""
echo "================================"
echo "Build completed!"
echo "================================"
echo ""
echo "Output: src/bin/Release/net8.0-windows/VoidVideoGenerator.dll"
```

Save as `build.sh` and make executable:
```bash
chmod +x build.sh
./build.sh
```

## Continuous Integration

### GitHub Actions Example

```yaml
name: Build

on: [push, pull_request]

jobs:
  build:
    runs-on: windows-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: 8.0.x
    
    - name: Restore dependencies
      run: dotnet restore src/VoidVideoGenerator.csproj
    
    - name: Build
      run: dotnet build src/VoidVideoGenerator.csproj --configuration Release --no-restore
    
    - name: Upload artifact
      uses: actions/upload-artifact@v3
      with:
        name: VoidVideoGenerator
        path: src/bin/Release/net8.0-windows/
```

## Publishing for Distribution

To create a standalone executable:

```bash
# Self-contained (includes .NET runtime)
dotnet publish src/VoidVideoGenerator.csproj \
  --configuration Release \
  --runtime win-x64 \
  --self-contained true \
  --output ./publish

# Framework-dependent (requires .NET 8 installed)
dotnet publish src/VoidVideoGenerator.csproj \
  --configuration Release \
  --runtime win-x64 \
  --self-contained false \
  --output ./publish
```

## Quick Reference

| Platform | Command |
|----------|---------|
| **Windows (Batch)** | `build.bat` |
| **Windows (CLI)** | `dotnet build src/VoidVideoGenerator.csproj --configuration Release` |
| **Linux/macOS** | `dotnet build src/VoidVideoGenerator.csproj --configuration Release` |
| **Visual Studio** | Ctrl+Shift+B |
| **Clean Build** | `dotnet clean src/VoidVideoGenerator.csproj` |
| **Restore Only** | `dotnet restore src/VoidVideoGenerator.csproj` |

## Related Documentation

- [`QUICKSTART.md`](QUICKSTART.md) - Getting started guide
- [`SETUP_GUIDE.md`](SETUP_GUIDE.md) - Initial setup instructions
- [`RUNTIME_REQUIREMENTS.md`](RUNTIME_REQUIREMENTS.md) - Runtime dependencies
- [`TROUBLESHOOTING.md`](TROUBLESHOOTING.md) - Common issues and solutions

## Summary

✅ **Build is working correctly**
- Successfully builds on Windows, Linux, and macOS
- No compilation errors or warnings
- Output: `src/bin/Release/net8.0-windows/VoidVideoGenerator.dll`
- Use `dotnet build` command on non-Windows platforms
- Use `build.bat` on Windows for convenience
