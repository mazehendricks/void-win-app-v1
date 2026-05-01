@echo off
REM Test Anthropic API Connection
REM This script helps diagnose Anthropic API issues

echo ========================================
echo Anthropic API Connection Test
echo ========================================
echo.

REM Check if config.json exists
if not exist "src\config.json" (
    echo [ERROR] config.json not found!
    echo Please copy config.example.json to config.json and add your API key.
    echo.
    pause
    exit /b 1
)

echo [1/4] Checking config.json...
findstr /C:"AnthropicApiKey" src\config.json >nul
if errorlevel 1 (
    echo [ERROR] AnthropicApiKey not found in config.json
    pause
    exit /b 1
)
echo [OK] Config file found

echo.
echo [2/4] Checking internet connectivity...
ping -n 1 8.8.8.8 >nul 2>&1
if errorlevel 1 (
    echo [ERROR] No internet connection detected
    pause
    exit /b 1
)
echo [OK] Internet connection active

echo.
echo [3/4] Checking Anthropic API endpoint...
curl -s -o nul -w "%%{http_code}" https://api.anthropic.com >nul 2>&1
if errorlevel 1 (
    echo [WARNING] Could not reach api.anthropic.com
    echo This might be a firewall or network issue
) else (
    echo [OK] Can reach Anthropic API endpoint
)

echo.
echo [4/4] Testing API key...
echo.
echo Please enter your Anthropic API key to test:
echo (or press Ctrl+C to cancel)
set /p API_KEY="API Key: "

if "%API_KEY%"=="" (
    echo [ERROR] No API key provided
    pause
    exit /b 1
)

echo.
echo Testing API key with Anthropic...
echo.

REM Create temporary test file
echo {"model":"claude-3-5-sonnet-20241022","max_tokens":10,"messages":[{"role":"user","content":"Hi"}]} > temp_test.json

curl -s -X POST https://api.anthropic.com/v1/messages ^
  -H "x-api-key: %API_KEY%" ^
  -H "anthropic-version: 2023-06-01" ^
  -H "content-type: application/json" ^
  -d @temp_test.json

del temp_test.json

echo.
echo.
echo ========================================
echo Test Complete
echo ========================================
echo.
echo If you see a JSON response above with "content", your API key works!
echo If you see an error, check the message for details.
echo.
echo Common issues:
echo - "invalid_api_key": Your API key is incorrect
echo - "rate_limit_error": You've hit your usage limit
echo - "Could not resolve host": Network/firewall issue
echo.
pause
