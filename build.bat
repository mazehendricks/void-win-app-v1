@echo off
echo ================================
echo Void Video Generator - Build
echo ================================
echo.

cd src

echo Restoring NuGet packages...
dotnet restore
if %errorlevel% neq 0 (
    echo ERROR: Failed to restore packages
    pause
    exit /b %errorlevel%
)

echo.
echo Building application...
dotnet build --configuration Release
if %errorlevel% neq 0 (
    echo ERROR: Build failed
    pause
    exit /b %errorlevel%
)

echo.
echo ================================
echo Build completed successfully!
echo ================================
echo.
echo To run the application:
echo   cd src
echo   dotnet run
echo.
echo Or use: run.bat
echo.
pause
