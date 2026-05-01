@echo off
echo ================================
echo Void Video Generator
echo ================================
echo.

REM Check if we're in the right directory
if not exist "src\VoidVideoGenerator.csproj" (
    echo ERROR: Cannot find src\VoidVideoGenerator.csproj
    echo Please run this script from the project root directory
    echo Current directory: %CD%
    pause
    exit /b 1
)

echo Starting application...
echo.

cd src
dotnet run --project VoidVideoGenerator.csproj

if %errorlevel% neq 0 (
    echo.
    echo ERROR: Application failed to start
    echo.
    echo Common issues:
    echo - .NET 8 SDK not installed (download from https://dotnet.microsoft.com/download/dotnet/8.0)
    echo - Build errors (run build.bat first)
    echo - Missing dependencies (run: dotnet restore)
    echo.
    echo Try running: build.bat
    echo.
    pause
)

cd ..
