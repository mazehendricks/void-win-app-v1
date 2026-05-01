@echo off
echo ========================================
echo Whisper Installation Script
echo ========================================
echo.

REM Check if Python is installed
python --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: Python is not installed or not in PATH
    echo.
    echo Please install Python 3.8 or later from:
    echo https://www.python.org/downloads/
    echo.
    echo Make sure to check "Add Python to PATH" during installation
    pause
    exit /b 1
)

echo Python found:
python --version
echo.

REM Check if pip is available
pip --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: pip is not installed
    echo.
    echo Please reinstall Python with pip included
    pause
    exit /b 1
)

echo pip found:
pip --version
echo.

echo Installing OpenAI Whisper...
echo This may take a few minutes...
echo.

pip install -U openai-whisper

if %errorlevel% neq 0 (
    echo.
    echo ERROR: Failed to install Whisper
    echo.
    echo Try running this command manually:
    echo pip install -U openai-whisper
    pause
    exit /b 1
)

echo.
echo ========================================
echo Whisper installed successfully!
echo ========================================
echo.
echo You can now use the caption feature in the application.
echo.
echo Note: First run may download model files (100-1500MB depending on model)
echo.
pause
