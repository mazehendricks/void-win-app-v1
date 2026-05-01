#!/bin/bash

echo "========================================"
echo "Whisper Installation Script"
echo "========================================"
echo ""

# Check if Python is installed
if ! command -v python3 &> /dev/null; then
    echo "ERROR: Python 3 is not installed"
    echo ""
    echo "Please install Python 3.8 or later:"
    echo "  Ubuntu/Debian: sudo apt install python3 python3-pip"
    echo "  macOS: brew install python3"
    echo "  Fedora: sudo dnf install python3 python3-pip"
    exit 1
fi

echo "Python found:"
python3 --version
echo ""

# Check if pip is available
if ! command -v pip3 &> /dev/null; then
    echo "ERROR: pip3 is not installed"
    echo ""
    echo "Please install pip:"
    echo "  Ubuntu/Debian: sudo apt install python3-pip"
    echo "  macOS: python3 -m ensurepip"
    echo "  Fedora: sudo dnf install python3-pip"
    exit 1
fi

echo "pip found:"
pip3 --version
echo ""

echo "Installing OpenAI Whisper..."
echo "This may take a few minutes..."
echo ""

pip3 install -U openai-whisper

if [ $? -ne 0 ]; then
    echo ""
    echo "ERROR: Failed to install Whisper"
    echo ""
    echo "Try running this command manually:"
    echo "pip3 install -U openai-whisper"
    exit 1
fi

echo ""
echo "========================================"
echo "Whisper installed successfully!"
echo "========================================"
echo ""
echo "You can now use the caption feature in the application."
echo ""
echo "Note: First run may download model files (100-1500MB depending on model)"
echo ""
