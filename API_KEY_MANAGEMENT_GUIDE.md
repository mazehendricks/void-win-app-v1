# API Key Management Guide

## Overview

The Void Video Generator application includes a **built-in API Key Management UI** that allows you to configure all API keys directly within the application, without manually editing configuration files.

## Accessing API Key Settings

1. Launch the application
2. Navigate to the **Settings** tab
3. All API key fields are organized in sections based on their purpose

## Available API Key Fields

### 🤖 AI Script Generator Section

Configure the AI service that will generate video scripts:

#### OpenAI
- **Field**: OpenAI API Key
- **Features**: Password-masked input, Test button
- **Model**: Configurable (default: gpt-4)
- **Usage**: Script generation using GPT models
- **Get Key**: https://platform.openai.com/api-keys

#### Anthropic Claude
- **Field**: Anthropic API Key
- **Features**: Password-masked input, Test button
- **Model**: Configurable (default: claude-3-5-sonnet-20241022)
- **Usage**: Script generation using Claude models
- **Get Key**: https://console.anthropic.com/

#### Google Gemini
- **Field**: Gemini API Key
- **Features**: Password-masked input, Test button
- **Model**: Configurable (default: gemini-1.5-pro)
- **Usage**: Script generation using Gemini models
- **Get Key**: https://makersuite.google.com/app/apikey

#### Ollama (Local)
- **Fields**: Ollama URL, Ollama Model
- **Features**: No API key required (runs locally)
- **Default URL**: http://localhost:11434
- **Default Model**: llama3.1
- **Usage**: Local AI script generation (free, private)

### 🖼️ Unsplash Image Generation (Optional)

- **Field**: Unsplash API Key
- **Features**: Enable/disable checkbox, auto-enables key field when checked
- **Usage**: Automatic image generation from Unsplash library
- **Get Key**: https://unsplash.com/developers
- **Note**: Free tier available with rate limits

### 🎬 AI Video Generation

Configure AI video generation services for creating animated videos:

#### Runway ML
- **Field**: API Key (shown when Runway ML is selected)
- **Features**: Password-masked input
- **Usage**: Cloud-based AI video generation
- **Get Key**: https://runwayml.com/

#### Luma AI
- **Field**: API Key (shown when Luma AI is selected)
- **Features**: Password-masked input
- **Usage**: Cloud-based AI video generation
- **Get Key**: https://lumalabs.ai/

#### AnimateDiff / Hybrid
- **No API Key Required**: These run locally
- **Note**: Requires local setup and GPU

## How to Add/Update API Keys

### Step-by-Step Instructions

1. **Open the Settings Tab**
   - Launch the application
   - Click on the "Settings" tab at the top

2. **Select Your AI Provider**
   - Choose from: Ollama (Local), OpenAI, Anthropic Claude, or Google Gemini
   - The relevant fields will be highlighted

3. **Enter Your API Key**
   - Click in the API Key field
   - Paste your API key (it will be masked with asterisks for security)
   - Optionally configure the model name

4. **Test the Connection (Optional)**
   - Click the "Test" button next to the API key field
   - The application will verify your API key is valid
   - You'll see a success or error message

5. **Configure Additional Services**
   - Enable Unsplash if you want automatic image generation
   - Select an AI Video provider if you want animated videos
   - Enter the corresponding API keys

6. **Save Your Settings**
   - Click the **"💾 Save All Settings"** button at the bottom
   - Your API keys will be saved to `config.json`
   - Services will be reinitialized with the new settings

## Security Features

### Password Masking
- All API key fields use password masking (asterisks or dots)
- Keys are hidden from view while typing
- Prevents shoulder-surfing and accidental exposure

### Local Storage
- API keys are stored in `config.json` in the application directory
- File is stored locally on your machine
- Not transmitted anywhere except to the respective API services

### Test Functionality
- Test buttons allow you to verify API keys without generating content
- Helps catch configuration errors early
- Provides immediate feedback on key validity

## Configuration File

When you save settings, they are written to `config.json`:

```json
{
  "AiProvider": "anthropic",
  "OpenAiApiKey": "sk-...",
  "OpenAiModel": "gpt-4",
  "AnthropicApiKey": "sk-ant-...",
  "AnthropicModel": "claude-3-5-sonnet-20241022",
  "GeminiApiKey": "AIza...",
  "GeminiModel": "gemini-1.5-pro",
  "UnsplashApiKey": "...",
  "UseUnsplashImages": true,
  "AIVideoGeneration": {
    "Provider": "RunwayML",
    "RunwayML": {
      "ApiKey": "..."
    },
    "LumaAI": {
      "ApiKey": "..."
    }
  }
}
```

## Keyboard Shortcuts

- **Ctrl+S**: Quick save settings from anywhere in the Settings tab

## Troubleshooting

### API Key Not Working

1. **Verify the Key Format**
   - OpenAI keys start with `sk-`
   - Anthropic keys start with `sk-ant-`
   - Gemini keys start with `AIza`

2. **Check Key Permissions**
   - Ensure the API key has the necessary permissions
   - Some keys may be restricted to specific models or features

3. **Test the Connection**
   - Use the "Test" button to verify the key
   - Check the Debug Console tab for detailed error messages

4. **Check Account Status**
   - Verify your API account is active
   - Ensure you have available credits/quota
   - Check for any billing issues

### Settings Not Saving

1. **Check File Permissions**
   - Ensure the application can write to `config.json`
   - Run as administrator if needed (Windows)

2. **Verify JSON Format**
   - If you manually edited `config.json`, ensure it's valid JSON
   - Delete the file to regenerate with defaults

3. **Check Debug Console**
   - Look for error messages in the Debug Console tab
   - Errors will indicate what went wrong

### API Key Disappeared

1. **Check config.json**
   - Open `config.json` in a text editor
   - Verify the key is present in the file

2. **Re-enter and Save**
   - If the key is missing, re-enter it in the UI
   - Click "Save All Settings"

## Best Practices

### Security
- ✅ Never share your `config.json` file
- ✅ Never commit API keys to version control
- ✅ Use environment-specific keys (dev vs production)
- ✅ Rotate keys periodically
- ❌ Don't screenshot the Settings tab with keys visible

### Cost Management
- Start with Ollama (free, local) for testing
- Use free tiers when available (Unsplash, etc.)
- Monitor API usage in your provider dashboards
- Set up billing alerts in your API accounts

### Performance
- Local AI (Ollama) is free but slower
- Cloud AI (OpenAI, Anthropic, Gemini) is faster but costs money
- Unsplash is optional - disable if you provide your own images
- AI Video generation is optional - images-only mode works great

## Related Documentation

- [`QUICKSTART.md`](QUICKSTART.md) - Getting started guide
- [`SETUP_GUIDE.md`](SETUP_GUIDE.md) - Initial setup instructions
- [`HOW_TO_UPDATE_API_KEYS.md`](HOW_TO_UPDATE_API_KEYS.md) - Manual configuration guide
- [`ONLINE_AI_SETUP.md`](ONLINE_AI_SETUP.md) - Cloud AI provider setup
- [`UNSPLASH_SETUP.md`](UNSPLASH_SETUP.md) - Unsplash integration guide
- [`AI_VIDEO_SETUP_GUIDE.md`](AI_VIDEO_SETUP_GUIDE.md) - AI video generation setup

## Summary

✅ **The application already has full API key management in the UI**
- All major API keys can be configured through the Settings tab
- Password masking protects sensitive information
- Test buttons verify API connectivity
- Save button persists all settings to config.json
- No manual file editing required

You can now manage all your API keys directly in the application without touching configuration files!
