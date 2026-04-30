@echo off
echo ================================
echo Void Video Generator
echo ================================
echo.
echo Starting application...
echo.

cd src
dotnet run

if %errorlevel% neq 0 (
    echo.
    echo ERROR: Application failed to start
    echo.
    echo Common issues:
    echo - .NET 8 SDK not installed
    echo - Build errors (run build.bat first)
    echo.
    pause
)
