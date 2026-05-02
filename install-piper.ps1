# Piper TTS Installation Script for Windows (PowerShell)
# This script downloads and installs Piper TTS with a default voice model
# Run with: powershell -ExecutionPolicy Bypass -File install-piper.ps1

$ErrorActionPreference = "Stop"

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Piper TTS Installation Script" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# Configuration
$PIPER_VERSION = "v1.2.0"
$PIPER_URL = "https://github.com/rhasspy/piper/releases/download/$PIPER_VERSION/piper_windows_amd64.zip"
$INSTALL_DIR = "C:\Tools\piper"
$MODELS_DIR = "$INSTALL_DIR\models"
$VOICE_MODEL = "en_US-lessac-medium"
$VOICE_BASE_URL = "https://huggingface.co/rhasspy/piper-voices/resolve/main/en/en_US/lessac/medium"
$TEMP_ZIP = "$env:TEMP\piper_$(Get-Random).zip"
$TEMP_EXTRACT = "$env:TEMP\piper_extract_$(Get-Random)"

# Function to download with retry
function Download-WithRetry {
    param(
        [string]$Url,
        [string]$OutputPath,
        [int]$MaxRetries = 3
    )
    
    $retryCount = 0
    $success = $false
    
    while (-not $success -and $retryCount -lt $MaxRetries) {
        $retryCount++
        Write-Host "Attempt $retryCount of $MaxRetries..." -ForegroundColor Yellow
        
        try {
            # Remove partial download if exists
            if (Test-Path $OutputPath) {
                Remove-Item $OutputPath -Force
            }
            
            # Download with progress
            $ProgressPreference = 'SilentlyContinue'
            Invoke-WebRequest -Uri $Url -OutFile $OutputPath -UseBasicParsing -TimeoutSec 300
            $ProgressPreference = 'Continue'
            
            # Verify file exists and has content
            if (Test-Path $OutputPath) {
                $fileSize = (Get-Item $OutputPath).Length
                if ($fileSize -gt 100000) {
                    Write-Host "Download successful ($fileSize bytes)" -ForegroundColor Green
                    $success = $true
                } else {
                    throw "Downloaded file is too small ($fileSize bytes)"
                }
            } else {
                throw "Downloaded file not found"
            }
        }
        catch {
            Write-Host "Download failed: $($_.Exception.Message)" -ForegroundColor Red
            
            if ($retryCount -lt $MaxRetries) {
                Write-Host "Retrying in 3 seconds..." -ForegroundColor Yellow
                Start-Sleep -Seconds 3
            }
        }
    }
    
    if (-not $success) {
        throw "Failed to download after $MaxRetries attempts"
    }
}

# Function to extract ZIP with validation
function Extract-ZipSafely {
    param(
        [string]$ZipPath,
        [string]$DestinationPath
    )
    
    Write-Host "Validating ZIP file..." -ForegroundColor Yellow
    
    try {
        # Test if ZIP is valid
        Add-Type -AssemblyName System.IO.Compression.FileSystem
        $zip = [System.IO.Compression.ZipFile]::OpenRead($ZipPath)
        $entryCount = $zip.Entries.Count
        $zip.Dispose()
        
        Write-Host "ZIP file is valid ($entryCount entries)" -ForegroundColor Green
        
        # Extract
        Write-Host "Extracting files..." -ForegroundColor Yellow
        [System.IO.Compression.ZipFile]::ExtractToDirectory($ZipPath, $DestinationPath)
        Write-Host "Extraction successful!" -ForegroundColor Green
        
        return $true
    }
    catch {
        Write-Host "ZIP validation/extraction failed: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

try {
    # Step 1: Create directories
    Write-Host "Step 1/5: Creating installation directory..." -ForegroundColor Cyan
    if (-not (Test-Path $INSTALL_DIR)) {
        New-Item -ItemType Directory -Path $INSTALL_DIR -Force | Out-Null
    }
    if (-not (Test-Path $MODELS_DIR)) {
        New-Item -ItemType Directory -Path $MODELS_DIR -Force | Out-Null
    }
    Write-Host "Directories created successfully" -ForegroundColor Green
    Write-Host ""
    
    # Step 2: Download Piper
    Write-Host "Step 2/5: Downloading Piper TTS..." -ForegroundColor Cyan
    Write-Host "URL: $PIPER_URL" -ForegroundColor Gray
    Write-Host "This may take a few minutes depending on your connection..." -ForegroundColor Gray
    
    Download-WithRetry -Url $PIPER_URL -OutputPath $TEMP_ZIP -MaxRetries 3
    Write-Host ""
    
    # Step 3: Extract Piper
    Write-Host "Step 3/5: Extracting Piper..." -ForegroundColor Cyan
    
    $extractSuccess = Extract-ZipSafely -ZipPath $TEMP_ZIP -DestinationPath $TEMP_EXTRACT
    
    if (-not $extractSuccess) {
        Write-Host ""
        Write-Host "ERROR: Failed to extract ZIP file" -ForegroundColor Red
        Write-Host "The downloaded file may be corrupted." -ForegroundColor Yellow
        Write-Host ""
        Write-Host "Troubleshooting steps:" -ForegroundColor Yellow
        Write-Host "1. Run this script again to re-download" -ForegroundColor White
        Write-Host "2. Check your internet connection" -ForegroundColor White
        Write-Host "3. Try downloading manually from: $PIPER_URL" -ForegroundColor White
        Write-Host "   Then extract to: $INSTALL_DIR" -ForegroundColor White
        Write-Host ""
        
        # Cleanup
        if (Test-Path $TEMP_ZIP) {
            Remove-Item $TEMP_ZIP -Force
        }
        
        throw "ZIP extraction failed"
    }
    Write-Host ""
    
    # Step 4: Install Piper
    Write-Host "Step 4/5: Installing Piper to $INSTALL_DIR..." -ForegroundColor Cyan
    
    # Copy all files from extracted directory to install directory
    Get-ChildItem -Path $TEMP_EXTRACT -Recurse | ForEach-Object {
        $targetPath = $_.FullName.Replace($TEMP_EXTRACT, $INSTALL_DIR)
        if ($_.PSIsContainer) {
            if (-not (Test-Path $targetPath)) {
                New-Item -ItemType Directory -Path $targetPath -Force | Out-Null
            }
        } else {
            Copy-Item $_.FullName -Destination $targetPath -Force
        }
    }
    
    Write-Host "Installation successful!" -ForegroundColor Green
    
    # Cleanup
    Write-Host "Cleaning up temporary files..." -ForegroundColor Gray
    if (Test-Path $TEMP_ZIP) {
        Remove-Item $TEMP_ZIP -Force
    }
    if (Test-Path $TEMP_EXTRACT) {
        Remove-Item $TEMP_EXTRACT -Recurse -Force
    }
    Write-Host ""
    
    # Step 5: Download voice model
    Write-Host "Step 5/5: Downloading voice model ($VOICE_MODEL)..." -ForegroundColor Cyan
    
    Write-Host "  - Downloading .onnx file..." -ForegroundColor Gray
    $onnxPath = "$MODELS_DIR\voice.onnx"
    Download-WithRetry -Url "$VOICE_BASE_URL/$VOICE_MODEL.onnx" -OutputPath $onnxPath -MaxRetries 3
    
    Write-Host "  - Downloading .onnx.json file..." -ForegroundColor Gray
    $jsonPath = "$MODELS_DIR\voice.onnx.json"
    Download-WithRetry -Url "$VOICE_BASE_URL/$VOICE_MODEL.onnx.json" -OutputPath $jsonPath -MaxRetries 3
    
    Write-Host ""
    Write-Host "=========================================" -ForegroundColor Green
    Write-Host "Installation Complete!" -ForegroundColor Green
    Write-Host "=========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Piper installed to: $INSTALL_DIR\piper.exe" -ForegroundColor White
    Write-Host "Voice model: $MODELS_DIR\voice.onnx" -ForegroundColor White
    Write-Host ""
    
    # Test Piper
    Write-Host "Testing installation..." -ForegroundColor Cyan
    $piperExe = "$INSTALL_DIR\piper.exe"
    
    if (Test-Path $piperExe) {
        & $piperExe --version
        
        Write-Host ""
        Write-Host "Testing voice generation..." -ForegroundColor Cyan
        $testText = "Hello! This is a test of the Piper text to speech system."
        $testText | & $piperExe --model "$MODELS_DIR\voice.onnx" --output_file "test.wav"
        
        if (Test-Path "test.wav") {
            Write-Host "SUCCESS: Test audio file created: test.wav" -ForegroundColor Green
            Write-Host "You can play it with Windows Media Player or any audio player" -ForegroundColor White
        } else {
            Write-Host "WARNING: Failed to create test audio file" -ForegroundColor Yellow
        }
    } else {
        Write-Host "WARNING: Piper executable not found at expected location" -ForegroundColor Yellow
    }
    
    Write-Host ""
    Write-Host "=========================================" -ForegroundColor Cyan
    Write-Host "Next Steps:" -ForegroundColor Cyan
    Write-Host "=========================================" -ForegroundColor Cyan
    Write-Host "1. Add Piper to your PATH (optional):" -ForegroundColor White
    Write-Host "   - Press Win + X, select System" -ForegroundColor Gray
    Write-Host "   - Click 'Advanced system settings'" -ForegroundColor Gray
    Write-Host "   - Click 'Environment Variables'" -ForegroundColor Gray
    Write-Host "   - Under 'System variables', find 'Path'" -ForegroundColor Gray
    Write-Host "   - Click 'Edit' -> 'New'" -ForegroundColor Gray
    Write-Host "   - Add: $INSTALL_DIR" -ForegroundColor Gray
    Write-Host "   - Click OK on all dialogs" -ForegroundColor Gray
    Write-Host ""
    Write-Host "2. Update your config.json:" -ForegroundColor White
    Write-Host '   {' -ForegroundColor Gray
    Write-Host "     `"PiperPath`": `"$INSTALL_DIR\\piper.exe`"," -ForegroundColor Gray
    Write-Host "     `"PiperModelPath`": `"$MODELS_DIR\\voice.onnx`"" -ForegroundColor Gray
    Write-Host '   }' -ForegroundColor Gray
    Write-Host ""
    Write-Host "3. Run your application and test audio generation" -ForegroundColor White
    Write-Host ""
    Write-Host "For more voice models, visit:" -ForegroundColor White
    Write-Host "https://github.com/rhasspy/piper/blob/master/VOICES.md" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "=========================================" -ForegroundColor Cyan
}
catch {
    Write-Host ""
    Write-Host "=========================================" -ForegroundColor Red
    Write-Host "Installation Failed!" -ForegroundColor Red
    Write-Host "=========================================" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please check the error message above and try again." -ForegroundColor Yellow
    Write-Host "If the problem persists, you can:" -ForegroundColor Yellow
    Write-Host "1. Download Piper manually from: $PIPER_URL" -ForegroundColor White
    Write-Host "2. Extract to: $INSTALL_DIR" -ForegroundColor White
    Write-Host "3. Download voice models from: $VOICE_BASE_URL" -ForegroundColor White
    Write-Host ""
    
    # Cleanup on error
    if (Test-Path $TEMP_ZIP) {
        Remove-Item $TEMP_ZIP -Force -ErrorAction SilentlyContinue
    }
    if (Test-Path $TEMP_EXTRACT) {
        Remove-Item $TEMP_EXTRACT -Recurse -Force -ErrorAction SilentlyContinue
    }
    
    exit 1
}

Write-Host "Press any key to continue..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
