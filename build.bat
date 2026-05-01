@echo off
echo ================================
echo Void Video Generator - Build
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

cd src

echo Restoring NuGet packages...
dotnet restore VoidVideoGenerator.csproj
if %errorlevel% neq 0 (
    echo ERROR: Failed to restore packages
    cd ..
    pause
    exit /b %errorlevel%
)

echo.
echo Building application...
dotnet build VoidVideoGenerator.csproj --configuration Release
if %errorlevel% neq 0 (
    echo ERROR: Build failed
    cd ..
    pause
    exit /b %errorlevel%
)

cd ..

echo.
echo ================================
echo Build completed successfully!
echo ================================
echo.
echo Output: src\bin\Release\net8.0\VoidVideoGenerator.exe
echo.
echo To run the application:
echo   1. Use: run.bat
echo   2. Or: cd src ^&^& dotnet run
echo   3. Or run the exe directly from bin folder
echo.
pause
