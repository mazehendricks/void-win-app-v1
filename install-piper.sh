#!/bin/bash
# Piper TTS Installation Script for Linux/Codespaces
# This script downloads and installs Piper TTS with a default voice model

set -e

echo "========================================="
echo "Piper TTS Installation Script"
echo "========================================="
echo ""

# Configuration
PIPER_VERSION="v1.2.0"
PIPER_URL="https://github.com/rhasspy/piper/releases/download/${PIPER_VERSION}/piper_amd64.tar.gz"
INSTALL_DIR="/usr/local/bin"
MODELS_DIR="./models"
VOICE_MODEL="en_US-lessac-medium"
VOICE_BASE_URL="https://huggingface.co/rhasspy/piper-voices/resolve/main/en/en_US/lessac/medium"

# Check if running with sudo for system-wide install
if [ "$EUID" -ne 0 ]; then 
    echo "Note: Not running as root. Will install to local directory."
    INSTALL_DIR="$HOME/.local/bin"
    mkdir -p "$INSTALL_DIR"
    
    # Add to PATH if not already there
    if [[ ":$PATH:" != *":$INSTALL_DIR:"* ]]; then
        echo "Adding $INSTALL_DIR to PATH..."
        echo 'export PATH="$HOME/.local/bin:$PATH"' >> ~/.bashrc
        export PATH="$HOME/.local/bin:$PATH"
    fi
fi

echo "Step 1/4: Downloading Piper TTS..."
echo "URL: $PIPER_URL"
wget -q --show-progress "$PIPER_URL" -O piper.tar.gz

echo ""
echo "Step 2/4: Extracting Piper..."
tar -xzf piper.tar.gz

echo ""
echo "Step 3/4: Installing Piper to $INSTALL_DIR..."
if [ "$EUID" -eq 0 ]; then
    mv piper/piper "$INSTALL_DIR/"
else
    cp piper/piper "$INSTALL_DIR/"
fi
chmod +x "$INSTALL_DIR/piper"

# Cleanup
rm -rf piper piper.tar.gz

echo ""
echo "Step 4/4: Downloading voice model ($VOICE_MODEL)..."
mkdir -p "$MODELS_DIR"

echo "  - Downloading .onnx file..."
wget -q --show-progress "${VOICE_BASE_URL}/${VOICE_MODEL}.onnx" -O "${MODELS_DIR}/voice.onnx"

echo "  - Downloading .onnx.json file..."
wget -q --show-progress "${VOICE_BASE_URL}/${VOICE_MODEL}.onnx.json" -O "${MODELS_DIR}/voice.onnx.json"

echo ""
echo "========================================="
echo "✓ Installation Complete!"
echo "========================================="
echo ""
echo "Piper installed to: $INSTALL_DIR/piper"
echo "Voice model: $MODELS_DIR/voice.onnx"
echo ""
echo "Testing installation..."
echo ""

# Test Piper
if command -v piper &> /dev/null; then
    echo "✓ Piper is in PATH"
    piper --version
    echo ""
    
    # Test voice generation
    echo "Testing voice generation..."
    echo "Hello! This is a test of the Piper text to speech system." | piper --model "${MODELS_DIR}/voice.onnx" --output_file test.wav
    
    if [ -f "test.wav" ]; then
        echo "✓ Test audio file created: test.wav"
        echo ""
        echo "You can play it with: aplay test.wav (Linux) or your media player"
    else
        echo "✗ Failed to create test audio file"
        exit 1
    fi
else
    echo "⚠ Piper installed but not in PATH yet"
    echo "Please run: source ~/.bashrc"
    echo "Or restart your terminal"
fi

echo ""
echo "========================================="
echo "Next Steps:"
echo "========================================="
echo "1. Update your config.json:"
echo "   {"
echo "     \"PiperPath\": \"piper\","
echo "     \"PiperModelPath\": \"models/voice.onnx\""
echo "   }"
echo ""
echo "2. Run your application and test audio generation"
echo ""
echo "For more voice models, visit:"
echo "https://github.com/rhasspy/piper/blob/master/VOICES.md"
echo ""
