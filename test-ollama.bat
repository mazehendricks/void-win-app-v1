@echo off
echo ========================================
echo Ollama Connection Test
echo ========================================
echo.

echo [1/4] Checking if Ollama is running...
curl -s http://localhost:11434/api/tags >nul 2>&1
if %errorlevel% neq 0 (
    echo [FAIL] Cannot connect to Ollama at http://localhost:11434
    echo.
    echo Please make sure Ollama is running:
    echo   1. Open a new terminal
    echo   2. Run: ollama serve
    echo.
    pause
    exit /b 1
)
echo [PASS] Ollama is responding
echo.

echo [2/4] Listing installed models...
curl -s http://localhost:11434/api/tags
echo.
echo.

echo [3/4] Testing model generation (this may take a moment)...
echo Sending test prompt to llama3.1...
curl -s http://localhost:11434/api/generate -d "{\"model\":\"llama3.1\",\"prompt\":\"Say hello in 5 words\",\"stream\":false}" > test-response.json
if %errorlevel% neq 0 (
    echo [FAIL] Failed to generate response
    pause
    exit /b 1
)
echo [PASS] Generation successful
echo.

echo [4/4] Response preview:
type test-response.json
echo.
echo.

echo ========================================
echo All tests passed!
echo ========================================
echo.
echo Your Ollama installation is working correctly.
echo You can now use the Void Video Generator.
echo.

del test-response.json >nul 2>&1
pause
