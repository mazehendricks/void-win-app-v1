# How to Update API Keys in Void Video Generator

This guide shows you how to update your AI provider API keys directly in the application without manually editing config files.

## Quick Steps

### 1. Open the Settings Tab

Launch Void Video Generator and click on the **Settings** tab at the top of the window.

### 2. Select Your AI Provider

In the "AI Script Generator" section, use the dropdown to select your provider:
- **Ollama (Local)** - No API key needed, runs locally
- **OpenAI** - Requires OpenAI API key
- **Anthropic Claude** - Requires Anthropic API key
- **Google Gemini** - Requires Google AI API key

### 3. Enter Your API Key

Find the API key field for your selected provider and enter your key:

**For Anthropic:**
```
Anthropic API Key: [Enter your sk-ant-api03-... key here]
Anthropic Model: claude-3-5-sonnet-20241022
```

**For OpenAI:**
```
OpenAI API Key: [Enter your sk-... key here]
OpenAI Model: gpt-4
```

**For Gemini:**
```
Gemini API Key: [Enter your key here]
Gemini Model: gemini-1.5-pro
```

### 4. Test Your Connection (NEW!)

Click the **"Test"** button next to the API key field to verify your key works:

- ✓ **Success** - Your API key is valid and the service is available
- ✗ **Failed** - Check the error message for details

This helps you verify the key is correct before saving!

### 5. Save Your Settings

Click the **"Save All Settings"** button at the bottom of the Settings tab.

You'll see a confirmation message:
```
Settings saved successfully!

AI Provider: ANTHROPIC
Services have been reinitialized.

Go to System Status tab to verify all services are available.
```

### 6. Verify Everything Works

Go to the **System Status** tab and click **"Check System Status"** to confirm:

```
=== AI SCRIPT GENERATOR ===
Provider: ANTHROPIC
Status: ✓ Available  <-- Should show Available now!
Model: claude-3-5-sonnet-20241022
API Key: ✓ Configured
```

## Where to Get API Keys

### Anthropic (Claude)
1. Visit https://console.anthropic.com
2. Sign up or log in
3. Go to Settings → API Keys
4. Click "Create Key"
5. Copy the key (starts with `sk-ant-api03-`)

**Cost:** Pay-as-you-go, ~$3 per million input tokens

### OpenAI (GPT-4)
1. Visit https://platform.openai.com
2. Sign up or log in
3. Go to API Keys section
4. Click "Create new secret key"
5. Copy the key (starts with `sk-`)

**Cost:** Pay-as-you-go, varies by model

### Google Gemini
1. Visit https://makersuite.google.com/app/apikey
2. Sign in with Google account
3. Click "Create API Key"
4. Copy the key

**Cost:** Free tier available, then pay-as-you-go

### Ollama (Free Local Option)
No API key needed! Just:
1. Install Ollama from https://ollama.com
2. Run `ollama pull llama3.1`
3. Select "Ollama (Local)" in the app

**Cost:** Free! Runs on your computer

## Troubleshooting

### "Connection Test Failed"

**Check these:**
1. API key is copied correctly (no extra spaces)
2. You have internet connection
3. API key hasn't expired
4. You have available credits/quota

**Quick fix:**
- Click the **"Test"** button to see the specific error
- Check the console log for detailed error messages
- See [`ANTHROPIC_TROUBLESHOOTING.md`](ANTHROPIC_TROUBLESHOOTING.md) for Anthropic-specific help

### "Settings saved, but there was an issue initializing services"

This means the config was saved but the API key might be invalid.

**Fix:**
1. Go to System Status tab
2. Click "Check System Status"
3. Read the error message
4. Update the API key in Settings
5. Click "Test" to verify
6. Save again

### API Key Not Saving

**Make sure:**
- You clicked "Save All Settings" button
- The application has write permissions to `src/config.json`
- You're not running multiple instances of the app

### Password Characters (••••)

API key fields show dots for security. This is normal! Your key is still there.

**To verify your key:**
- Click the "Test" button - it will use the actual key
- Or check `src/config.json` file directly

## Switching Between Providers

You can easily switch between AI providers:

1. **Go to Settings tab**
2. **Select different provider** from dropdown
3. **Enter that provider's API key**
4. **Click "Test"** to verify
5. **Click "Save All Settings"**

The app will automatically use the selected provider for video generation.

## Best Practices

### Security
- ✓ Never share your API keys
- ✓ Don't commit config.json to public repositories
- ✓ Regenerate keys if accidentally exposed
- ✓ Use environment-specific keys (dev vs production)

### Cost Management
- ✓ Start with shorter videos to test
- ✓ Monitor your usage in provider dashboards
- ✓ Set up billing alerts
- ✓ Consider using Ollama (free) for testing

### Testing
- ✓ Always click "Test" before saving
- ✓ Check System Status after saving
- ✓ Generate a short test video to verify end-to-end

## Video Generation Workflow

Once your API key is configured:

1. **Go to Generate tab**
2. **Enter video details:**
   - Topic
   - Title
   - Duration
   - Keywords (optional)
3. **Click "Generate Video"**
4. **Wait for completion** (progress shown in status bar)
5. **Find your video** in the output folder

## Need Help?

### Documentation
- [`ANTHROPIC_TROUBLESHOOTING.md`](ANTHROPIC_TROUBLESHOOTING.md) - Anthropic-specific help
- [`ONLINE_AI_SETUP.md`](ONLINE_AI_SETUP.md) - Detailed setup for all providers
- [`TROUBLESHOOTING.md`](TROUBLESHOOTING.md) - General troubleshooting
- [`USER_GUIDE.md`](USER_GUIDE.md) - Complete user guide

### Test Scripts
Run these from command line to test API keys:
```bash
# Windows
test-anthropic.bat

# Linux/Mac
./test-anthropic.sh
```

### Support
- Check the console log in the app for detailed errors
- Use the "Test" button to diagnose connection issues
- See troubleshooting guides for specific error messages

## Summary

**To update API keys:**
1. Open Settings tab
2. Select AI provider
3. Enter API key
4. Click "Test" button ← NEW!
5. Click "Save All Settings"
6. Verify in System Status tab

**The "Test" button is your friend!** It will tell you immediately if your API key works before you save settings.
