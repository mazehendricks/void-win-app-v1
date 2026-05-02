# Quick Fix Guide - Application Won't Start

## Problem
Error about missing .NET framework or version when trying to run the application.

## Root Cause
The application needs **.NET 8 Desktop Runtime** (not just the SDK) to run Windows Forms applications.

---

## Solution: Install .NET 8 Desktop Runtime

### Option 1: Direct Download (RECOMMENDED)
1. Go to: https://dotnet.microsoft.com/download/dotnet/8.0
2. Under ".NET Desktop Runtime 8.0.x", click **Download x64**
3. Run the installer
4. Restart your command prompt
5. Try running the app again

### Option 2: Using winget
```cmd
winget install Microsoft.DotNet.DesktopRuntime.8
```

### Option 3: Install Everything (SDK + Runtime)
```cmd
winget install Microsoft.DotNet.SDK.8
winget install Microsoft.DotNet.DesktopRuntime.8
```

---

## Verify Installation

After installing, verify with these commands:

```cmd
dotnet --list-runtimes
```

You should see:
```
Microsoft.NETCore.App 8.0.x [C:\Program Files\dotnet\shared\Microsoft.NETCore.App]
Microsoft.WindowsDesktop.App 8.0.x [C:\Program Files\dotnet\shared\Microsoft.WindowsDesktop.App]
```

The **Microsoft.WindowsDesktop.App 8.0.x** line is critical for Windows Forms apps!

---

## Alternative: Change Target Framework

If you can't install .NET 8, you can change the app to use .NET 9 or 10:

### Step 1: Edit the project file
Open `src/VoidVideoGenerator.csproj` and change line 5:

**From:**
```xml
<TargetFramework>net8.0-windows</TargetFramework>
```

**To:** (choose one)
```xml
<TargetFramework>net9.0-windows</TargetFramework>
```
or
```xml
<TargetFramework>net10.0-windows</TargetFramework>
```

### Step 2: Rebuild
```cmd
build.bat
```

### Step 3: Run
```cmd
run.bat
```

---

## Still Not Working?

### Check 1: Verify .NET Installation
```cmd
dotnet --info
```

Look for:
- .NET SDK version
- Runtime versions installed
- Base Path

### Check 2: Try Running Directly
```cmd
cd src\bin\Debug\net8.0-windows
VoidVideoGenerator.exe
```

This will show the actual error message.

### Check 3: Check Windows Version
The app requires:
- Windows 10 version 1809 or later
- Windows 11 (any version)

To check your Windows version:
```cmd
winver
```

---

## Common Error Messages

### "You must install or update .NET to run this application"
**Solution**: Install .NET 8 Desktop Runtime (see Option 1 above)

### "Framework 'Microsoft.WindowsDesktop.App', version '8.0.0' was not found"
**Solution**: Install .NET 8 Desktop Runtime specifically

### "The application requires .NET 8.0"
**Solution**: Either install .NET 8 or change target framework to match your installed version

---

## Quick Commands Summary

```cmd
# Install Desktop Runtime
winget install Microsoft.DotNet.DesktopRuntime.8

# Verify installation
dotnet --list-runtimes

# Rebuild application
build.bat

# Run application
run.bat
```

---

## Need More Help?

1. Share the output of: `dotnet --list-runtimes`
2. Share the output of: `dotnet --info`
3. Share the exact error message when running `run.bat`

This will help identify the specific issue!
