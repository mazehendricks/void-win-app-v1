@echo off
REM Piper TTS Installation Script for Windows
REM This script downloads and installs Piper TTS with a default voice model

setlocal enabledelayedexpansion

echo =========================================
echo Piper TTS Installation Script
echo =========================================
echo.

REM Configuration
set PIPER_VERSION=v1.2.0
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

echo.
echo Step 2/5: Downloading Piper TTS...
echo URL: %PIPER_URL%
curl -L -o "%TEMP%\piper.zip" "%PIPER_URL%"
if %errorlevel% neq 0 (
    echo ERROR: Failed to download Piper
    pause
    exit /b 1
)

echo.
echo Step 3/5: Extracting Piper...
powershell -command "Expand-Archive -Path '%TEMP%\piper.zip' -DestinationPath '%TEMP%\piper' -Force"
if %errorlevel% neq 0 (
    echo ERROR: Failed to extract Piper
    pause
    exit /b 1
)

echo.
echo Step 4/5: Installing Piper to %INSTALL_DIR%...
xcopy /Y /E "%TEMP%\piper\*" "%INSTALL_DIR%\"

REM Cleanup
del "%TEMP%\piper.zip"
rmdir /S /Q "%TEMP%\piper"

echo.
echo Step 5/5: Downloading voice model (%VOICE_MODEL%)...
echo   - Downloading .onnx file...
curl -L -o "%MODELS_DIR%\voice.onnx" "%VOICE_BASE_URL%/%VOICE_MODEL%.onnx"
if %errorlevel% neq 0 (
    echo ERROR: Failed to download voice model
    pause
    exit /b 1
)

echo   - Downloading .onnx.json file...
curl -L -o "%MODELS_DIR%\voice.onnx.json" "%VOICE_BASE_URL%/%VOICE_MODEL%.onnx.json"
if %errorlevel% neq 0 (
    echo ERROR: Failed to download voice model config
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
