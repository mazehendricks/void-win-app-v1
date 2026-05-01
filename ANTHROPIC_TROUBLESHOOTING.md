# Anthropic API Troubleshooting Guide

This guide helps you resolve issues with the Anthropic Claude API integration in Void Video Generator.

## Common Issues and Solutions

### ✗ Anthropic API Shows "Not Available"

If the system status shows Anthropic as "Not Available" even with a configured API key, try these solutions:

#### 1. Verify Your API Key

**Check if your API key is correct:**
- Go to https://console.anthropic.com/settings/keys
- Verify your API key is active and not expired
- Copy the key exactly (no extra spaces or characters)

**Update your config.json:**
```json
{
  "AiProvider": "anthropic",
  "AnthropicApiKey": "sk-ant-api03-your-actual-key-here",
  "AnthropicModel": "claude-3-5-sonnet-20241022"
}
```

#### 2. Check Internet Connection

The Anthropic API requires internet access:
- Test your connection: Open https://api.anthropic.com in a browser
- Check if you're behind a firewall or proxy
- Verify your network allows HTTPS connections to api.anthropic.com

#### 3. Verify API Service Status

Check if Anthropic's services are operational:
- Visit https://status.anthropic.com
- Check for any ongoing incidents or maintenance

#### 4. Test API Key Manually

Use curl or PowerShell to test your API key:

**Windows PowerShell:**
```powershell
$headers = @{
    "x-api-key" = "YOUR_API_KEY_HERE"
    "anthropic-version" = "2023-06-01"
    "content-type" = "application/json"
}

$body = @{
    model = "claude-3-5-sonnet-20241022"
    max_tokens = 10
    messages = @(
        @{
            role = "user"
            content = "Hi"
        }
    )
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://api.anthropic.com/v1/messages" -Method Post -Headers $headers -Body $body
```

**Linux/Mac (curl):**
```bash
curl https://api.anthropic.com/v1/messages \
  -H "x-api-key: YOUR_API_KEY_HERE" \
  -H "anthropic-version: 2023-06-01" \
  -H "content-type: application/json" \
  -d '{
    "model": "claude-3-5-sonnet-20241022",
    "max_tokens": 10,
    "messages": [{"role": "user", "content": "Hi"}]
  }'
```

**Expected Response:**
```json
{
  "id": "msg_...",
  "type": "message",
  "role": "assistant",
  "content": [{"type": "text", "text": "Hello"}],
  "model": "claude-3-5-sonnet-20241022",
  ...
}
```

#### 5. Common Error Messages

**"Invalid API Key"**
- Your API key is incorrect or has been revoked
- Generate a new key at https://console.anthropic.com/settings/keys

**"Rate limit exceeded"**
- You've hit your API usage limits
- Check your usage at https://console.anthropic.com/settings/usage
- Wait a few minutes or upgrade your plan

**"Network error"**
- Check your internet connection
- Verify firewall/proxy settings
- Try disabling VPN temporarily

**"Request timeout"**
- Your connection is too slow
- Try again with a better internet connection
- Check if your ISP is blocking the connection

#### 6. Configuration File Location

Make sure you're editing the correct config file:
- **Location:** `src/config.json` (in the same folder as the executable)
- **Not:** `src/config.example.json` (this is just a template)

If `config.json` doesn't exist:
1. Copy `config.example.json` to `config.json`
2. Edit `config.json` with your API key
3. Restart the application

#### 7. Restart the Application

After making changes:
1. Close Void Video Generator completely
2. Reopen the application
3. Go to the "System Status" tab
4. Click "Check System Status" to verify

## Getting an Anthropic API Key

If you don't have an API key yet:

1. **Sign up for Anthropic:**
   - Visit https://console.anthropic.com
   - Create an account or sign in

2. **Generate an API key:**
   - Go to Settings → API Keys
   - Click "Create Key"
   - Copy the key immediately (you won't see it again)

3. **Add credits (if needed):**
   - Go to Settings → Billing
   - Add payment method and credits
   - New accounts may get free trial credits

4. **Configure in Void Video Generator:**
   - Open `src/config.json`
   - Set `"AiProvider": "anthropic"`
   - Set `"AnthropicApiKey": "your-key-here"`
   - Save and restart the app

## Alternative AI Providers

If Anthropic isn't working, you can use other providers:

### Option 1: OpenAI (GPT-4)
```json
{
  "AiProvider": "openai",
  "OpenAiApiKey": "sk-your-openai-key",
  "OpenAiModel": "gpt-4"
}
```

### Option 2: Google Gemini
```json
{
  "AiProvider": "gemini",
  "GeminiApiKey": "your-gemini-key",
  "GeminiModel": "gemini-1.5-pro"
}
```

### Option 3: Local Ollama (Free, No API Key)
```json
{
  "AiProvider": "ollama",
  "OllamaUrl": "http://localhost:11434",
  "OllamaModel": "llama3.1"
}
```

See `ONLINE_AI_SETUP.md` for detailed setup instructions for each provider.

## Still Having Issues?

1. **Check the logs:**
   - Look at the console output in the application
   - Check for specific error messages

2. **Enable debug mode:**
   - Run the app from command line to see detailed output
   - Look for network errors or API responses

3. **Test with minimal request:**
   - Try generating a very short video (30 seconds)
   - This uses fewer API tokens and is faster to test

4. **Contact support:**
   - Open an issue on GitHub with:
     - Your error message
     - System status output
     - Steps you've already tried
   - **Never share your actual API key!**

## Quick Checklist

Before asking for help, verify:
- [ ] API key is correctly copied to `src/config.json`
- [ ] `AiProvider` is set to `"anthropic"` in config.json
- [ ] Internet connection is working
- [ ] https://status.anthropic.com shows no issues
- [ ] API key is valid (test with curl/PowerShell)
- [ ] Application has been restarted after config changes
- [ ] You have available API credits/quota

## Video Generation Requirements

To generate AI videos, you need:
1. **✓ Working AI Provider** (Anthropic, OpenAI, Gemini, or Ollama)
2. **✓ Piper TTS** (for voice generation)
3. **✓ FFmpeg** (for video assembly)
4. **✓ Images** (Unsplash API or your own images)

The status report shows which services are working. Focus on getting the AI provider working first, as it's required to generate the video script.
