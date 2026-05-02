@echo off
REM Piper TTS Installation Script for Windows
REM This script downloads and installs Piper TTS with a default voice model

setlocal enabledelayedexpansion

echo =========================================
echo Piper TTS Installation Script
echo =========================================
echo.

REM Configuration
set PIPER_VERSION=2023.11.14-2
set PIPER_URL=https://github.com/rhasspy/piper/releases/download/%PIPER_VERSION%/piper_windows_amd64.zip
set INSTALL_DIR=C:\Tools\piper
set MODELS_DIR=%INSTALL_DIR%\models
set VOICE_MODEL=en_US-lessac-medium
set VOICE_BASE_URL=https://huggingface.co/rhasspy/piper-voices/resolve/main/en/en_US/lessac/medium

REM Check for curl (available in Windows 10+)
where curl >nul 2>nul
if %errorlevel% neq 0 (
    echo ERROR: curl not found. Please install curl or download manually.
    echo Download from: %PIPER_URL%
    pause
    exit /b 1
)

echo Step 1/5: Creating installation directory...
if not exist "%INSTALL_DIR%" mkdir "%INSTALL_DIR%"
if not exist "%MODELS_DIR%" mkdir "%MODELS_DIR%"

REM Clean up any previous failed downloads
if exist "%TEMP%\piper.zip" (
    echo Cleaning up previous download...
    del "%TEMP%\piper.zip"
)
if exist "%TEMP%\piper" (
    echo Cleaning up previous extraction...
    rmdir /S /Q "%TEMP%\piper"
)

echo.
echo Step 2/5: Downloading Piper TTS...
echo URL: %PIPER_URL%
echo This may take a few minutes depending on your connection...

REM Download with retry logic and better error handling
set MAX_RETRIES=3
set RETRY_COUNT=0

:download_retry
set /a RETRY_COUNT+=1
echo Attempt %RETRY_COUNT% of %MAX_RETRIES%...

REM Use curl with progress bar, follow redirects, and fail on HTTP errors
curl -L --fail --progress-bar -o "%TEMP%\piper.zip" "%PIPER_URL%"

if %errorlevel% neq 0 (
    echo WARNING: Download failed on attempt %RETRY_COUNT%
    if %RETRY_COUNT% lss %MAX_RETRIES% (
        echo Retrying in 3 seconds...
        timeout /t 3 /nobreak >nul
        if exist "%TEMP%\piper.zip" del "%TEMP%\piper.zip"
        goto download_retry
    ) else (
        echo ERROR: Failed to download Piper after %MAX_RETRIES% attempts
        echo.
        echo Possible solutions:
        echo 1. Check your internet connection
        echo 2. Try downloading manually from: %PIPER_URL%
        echo 3. Extract manually to: %INSTALL_DIR%
        pause
        exit /b 1
    )
)

REM Verify the downloaded file exists and has content
if not exist "%TEMP%\piper.zip" (
    echo ERROR: Downloaded file not found
    pause
    exit /b 1
)

REM Check file size (should be at least 1MB for Piper)
for %%A in ("%TEMP%\piper.zip") do set FILE_SIZE=%%~zA
if %FILE_SIZE% lss 1000000 (
    echo ERROR: Downloaded file is too small ^(%FILE_SIZE% bytes^)
    echo The download may be incomplete or corrupted
    echo.
    echo Please try one of these solutions:
    echo 1. Run this script again
    echo 2. Download manually from: %PIPER_URL%
    echo 3. Check if GitHub is accessible from your network
    del "%TEMP%\piper.zip"
    pause
    exit /b 1
)

echo Download successful ^(%FILE_SIZE% bytes^)

echo.
echo Step 3/5: Extracting Piper...
echo This may take a moment...

REM Try extraction with better error handling
powershell -NoProfile -ExecutionPolicy Bypass -Command "& { try { Add-Type -AssemblyName System.IO.Compression.FileSystem; [System.IO.Compression.ZipFile]::ExtractToDirectory('%TEMP%\piper.zip', '%TEMP%\piper'); exit 0 } catch { Write-Host 'ERROR: ' $_.Exception.Message; exit 1 } }"

if %errorlevel% neq 0 (
    echo ERROR: Failed to extract Piper ZIP file
    echo The downloaded file may be corrupted
    echo.
    echo Troubleshooting steps:
    echo 1. Delete the corrupted file: %TEMP%\piper.zip
    echo 2. Run this script again to re-download
    echo 3. Or download manually from: %PIPER_URL%
    echo    Then extract to: %INSTALL_DIR%
    echo.
    echo Cleaning up corrupted download...
    if exist "%TEMP%\piper.zip" del "%TEMP%\piper.zip"
    pause
    exit /b 1
)

echo Extraction successful!

echo.
echo Step 4/5: Installing Piper to %INSTALL_DIR%...
xcopy /Y /E "%TEMP%\piper\*" "%INSTALL_DIR%\"
if %errorlevel% neq 0 (
    echo ERROR: Failed to copy files to installation directory
    pause
    exit /b 1
)

REM Cleanup
echo Cleaning up temporary files...
if exist "%TEMP%\piper.zip" del "%TEMP%\piper.zip"
if exist "%TEMP%\piper" rmdir /S /Q "%TEMP%\piper"

echo.
echo Step 5/5: Downloading voice model (%VOICE_MODEL%)...
echo   - Downloading .onnx file...
curl -L --fail --progress-bar -o "%MODELS_DIR%\voice.onnx" "%VOICE_BASE_URL%/%VOICE_MODEL%.onnx"
if %errorlevel% neq 0 (
    echo ERROR: Failed to download voice model
    echo You can download it manually from:
    echo %VOICE_BASE_URL%/%VOICE_MODEL%.onnx
    pause
    exit /b 1
)

echo   - Downloading .onnx.json file...
curl -L --fail --progress-bar -o "%MODELS_DIR%\voice.onnx.json" "%VOICE_BASE_URL%/%VOICE_MODEL%.onnx.json"
if %errorlevel% neq 0 (
    echo ERROR: Failed to download voice model config
    echo You can download it manually from:
    echo %VOICE_BASE_URL%/%VOICE_MODEL%.onnx.json
    pause
    exit /b 1
)

echo.
echo =========================================
echo Installation Complete!
echo =========================================
echo.
echo Piper installed to: %INSTALL_DIR%\piper.exe
echo Voice model: %MODELS_DIR%\voice.onnx
echo.

REM Test Piper
echo Testing installation...
echo.
"%INSTALL_DIR%\piper.exe" --version
if %errorlevel% neq 0 (
    echo ERROR: Piper installation test failed
    echo The executable may not have been installed correctly
    pause
    exit /b 1
)

echo.
echo Testing voice generation...
echo Hello! This is a test of the Piper text to speech system. | "%INSTALL_DIR%\piper.exe" --model "%MODELS_DIR%\voice.onnx" --output_file test.wav

if exist test.wav (
    echo SUCCESS: Test audio file created: test.wav
    echo You can play it with Windows Media Player or any audio player
) else (
    echo ERROR: Failed to create test audio file
    pause
    exit /b 1
)

echo.
echo =========================================
echo Next Steps:
echo =========================================
echo 1. Add Piper to your PATH (optional):
echo    - Press Win + X, select System
echo    - Click "Advanced system settings"
echo    - Click "Environment Variables"
echo    - Under "System variables", find "Path"
echo    - Click "Edit" -^> "New"
echo    - Add: %INSTALL_DIR%
echo    - Click OK on all dialogs
echo.
echo 2. Update your config.json:
echo    {
echo      "PiperPath": "%INSTALL_DIR%\\piper.exe",
echo      "PiperModelPath": "%MODELS_DIR%\\voice.onnx"
echo    }
echo.
echo 3. Run your application and test audio generation
echo.
echo For more voice models, visit:
echo https://github.com/rhasspy/piper/blob/master/VOICES.md
echo.
echo =========================================

pause
