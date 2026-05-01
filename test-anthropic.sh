#!/bin/bash
# Test Anthropic API Connection
# This script helps diagnose Anthropic API issues

echo "========================================"
echo "Anthropic API Connection Test"
echo "========================================"
echo ""

# Check if config.json exists
if [ ! -f "src/config.json" ]; then
    echo "[ERROR] config.json not found!"
    echo "Please copy config.example.json to config.json and add your API key."
    echo ""
    exit 1
fi

echo "[1/4] Checking config.json..."
if grep -q "AnthropicApiKey" src/config.json; then
    echo "[OK] Config file found"
else
    echo "[ERROR] AnthropicApiKey not found in config.json"
    exit 1
fi

echo ""
echo "[2/4] Checking internet connectivity..."
if ping -c 1 8.8.8.8 &> /dev/null; then
    echo "[OK] Internet connection active"
else
    echo "[ERROR] No internet connection detected"
    exit 1
fi

echo ""
echo "[3/4] Checking Anthropic API endpoint..."
if curl -s -o /dev/null -w "%{http_code}" https://api.anthropic.com &> /dev/null; then
    echo "[OK] Can reach Anthropic API endpoint"
else
    echo "[WARNING] Could not reach api.anthropic.com"
    echo "This might be a firewall or network issue"
fi

echo ""
echo "[4/4] Testing API key..."
echo ""
echo "Please enter your Anthropic API key to test:"
echo "(or press Ctrl+C to cancel)"
read -p "API Key: " API_KEY

if [ -z "$API_KEY" ]; then
    echo "[ERROR] No API key provided"
    exit 1
fi

echo ""
echo "Testing API key with Anthropic..."
echo ""

# Test the API
RESPONSE=$(curl -s -X POST https://api.anthropic.com/v1/messages \
  -H "x-api-key: $API_KEY" \
  -H "anthropic-version: 2023-06-01" \
  -H "content-type: application/json" \
  -d '{
    "model": "claude-3-5-sonnet-20241022",
    "max_tokens": 10,
    "messages": [{"role": "user", "content": "Hi"}]
  }')

echo "$RESPONSE"

echo ""
echo ""
echo "========================================"
echo "Test Complete"
echo "========================================"
echo ""
echo "If you see a JSON response above with 'content', your API key works!"
echo "If you see an error, check the message for details."
echo ""
echo "Common issues:"
echo "- 'invalid_api_key': Your API key is incorrect"
echo "- 'rate_limit_error': You've hit your usage limit"
echo "- 'Could not resolve host': Network/firewall issue"
echo ""
