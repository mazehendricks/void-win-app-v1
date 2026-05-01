# Online AI Models Setup Guide

## Overview

Void Video Generator now supports multiple AI providers for script generation:

- **Ollama** (Local, Free) - Run AI models locally on your computer
- **OpenAI** (Cloud, Paid) - GPT-4, GPT-3.5-turbo, and other OpenAI models
- **Anthropic Claude** (Cloud, Paid) - Claude 3.5 Sonnet and other Claude models
- **Google Gemini** (Cloud, Paid/Free tier) - Gemini 1.5 Pro and other Gemini models

---

## Quick Start

1. Open the **Settings** tab in the application
2. Select your preferred **AI Provider** from the dropdown
3. Enter your API key (for cloud providers)
4. Configure the model name
5. Click **Save Settings**
6. Start generating videos!

---

## Provider Setup

### 1. Ollama (Local - Free)

**Pros:**
- ✅ Completely free
- ✅ Privacy - runs locally
- ✅ No API limits
- ✅ Works offline

**Cons:**
- ❌ Requires powerful hardware (8GB+ RAM recommended)
- ❌ Slower than cloud APIs
- ❌ Limited to available models

**Setup:**
1. Install Ollama from https://ollama.ai
2. Pull a model: `ollama pull llama3.1`
3. In the app settings:
   - Provider: **Ollama (Local)**
   - URL: `http://localhost:11434`
   - Model: `llama3.1` (or your preferred model)

**Recommended Models:**
- `llama3.1` - Best overall quality
- `llama3.1:70b` - Highest quality (requires 40GB+ RAM)
- `mistral` - Fast and efficient
- `phi3` - Lightweight option

---

### 2. OpenAI (Cloud - Paid)

**Pros:**
- ✅ Highest quality scripts
- ✅ Very fast
- ✅ Reliable and well-documented
- ✅ Multiple model options

**Cons:**
- ❌ Costs money per request
- ❌ Requires internet connection
- ❌ Data sent to OpenAI servers

**Setup:**
1. Create an account at https://platform.openai.com
2. Go to https://platform.openai.com/api-keys
3. Click "Create new secret key"
4. Copy your API key
5. In the app settings:
   - Provider: **OpenAI**
   - API Key: Paste your key
   - Model: `gpt-4` or `gpt-3.5-turbo`

**Recommended Models:**
- `gpt-4` - Best quality ($0.03/1K tokens)
- `gpt-4-turbo` - Fast and high quality ($0.01/1K tokens)
- `gpt-3.5-turbo` - Budget option ($0.0005/1K tokens)

**Pricing:**
- Typical 60-second video script: ~500-1000 tokens
- GPT-4: ~$0.03 per video
- GPT-3.5-turbo: ~$0.001 per video

---

### 3. Anthropic Claude (Cloud - Paid)

**Pros:**
- ✅ Excellent script quality
- ✅ Long context window
- ✅ Good at following instructions
- ✅ Competitive pricing

**Cons:**
- ❌ Costs money per request
- ❌ Requires internet connection
- ❌ Data sent to Anthropic servers

**Setup:**
1. Create an account at https://console.anthropic.com
2. Go to https://console.anthropic.com/settings/keys
3. Click "Create Key"
4. Copy your API key
5. In the app settings:
   - Provider: **Anthropic Claude**
   - API Key: Paste your key
   - Model: `claude-3-5-sonnet-20241022`

**Recommended Models:**
- `claude-3-5-sonnet-20241022` - Best overall ($3/MTok input, $15/MTok output)
- `claude-3-opus-20240229` - Highest quality ($15/MTok input, $75/MTok output)
- `claude-3-haiku-20240307` - Budget option ($0.25/MTok input, $1.25/MTok output)

**Pricing:**
- Typical 60-second video script: ~500-1000 tokens
- Claude 3.5 Sonnet: ~$0.02 per video
- Claude 3 Haiku: ~$0.002 per video

---

### 4. Google Gemini (Cloud - Free Tier Available)

**Pros:**
- ✅ Free tier available (60 requests/minute)
- ✅ Good quality scripts
- ✅ Fast response times
- ✅ Generous context window

**Cons:**
- ❌ Free tier has rate limits
- ❌ Requires internet connection
- ❌ Data sent to Google servers

**Setup:**
1. Go to https://makersuite.google.com/app/apikey
2. Click "Create API Key"
3. Copy your API key
4. In the app settings:
   - Provider: **Google Gemini**
   - API Key: Paste your key
   - Model: `gemini-1.5-pro`

**Recommended Models:**
- `gemini-1.5-pro` - Best quality (Free tier: 2 RPM, Paid: $7/MTok)
- `gemini-1.5-flash` - Fast and efficient (Free tier: 15 RPM, Paid: $0.35/MTok)
- `gemini-1.0-pro` - Budget option (Free tier: 60 RPM, Paid: $0.50/MTok)

**Free Tier Limits:**
- Gemini 1.5 Pro: 2 requests per minute
- Gemini 1.5 Flash: 15 requests per minute
- Gemini 1.0 Pro: 60 requests per minute

**Pricing (Paid):**
- Typical 60-second video script: ~500-1000 tokens
- Gemini 1.5 Pro: ~$0.007 per video
- Gemini 1.5 Flash: ~$0.0004 per video

---

## Comparison Table

| Provider | Cost/Video | Quality | Speed | Privacy | Free Tier |
|----------|-----------|---------|-------|---------|-----------|
| **Ollama** | Free | Good | Slow | ✅ Local | ✅ Unlimited |
| **OpenAI GPT-4** | ~$0.03 | Excellent | Fast | ❌ Cloud | ❌ No |
| **OpenAI GPT-3.5** | ~$0.001 | Good | Very Fast | ❌ Cloud | ❌ No |
| **Claude 3.5 Sonnet** | ~$0.02 | Excellent | Fast | ❌ Cloud | ❌ No |
| **Claude 3 Haiku** | ~$0.002 | Good | Very Fast | ❌ Cloud | ❌ No |
| **Gemini 1.5 Pro** | Free/~$0.007 | Very Good | Fast | ❌ Cloud | ✅ 2 RPM |
| **Gemini 1.5 Flash** | Free/~$0.0004 | Good | Very Fast | ❌ Cloud | ✅ 15 RPM |

---

## Recommendations

### For Privacy-Conscious Users
**Use Ollama** - All processing happens locally on your computer.

### For Best Quality
**Use OpenAI GPT-4 or Claude 3.5 Sonnet** - These produce the highest quality, most engaging scripts.

### For Budget Users
**Use Google Gemini (Free Tier)** - Excellent quality with generous free tier limits.

### For High Volume
**Use Ollama or Gemini 1.5 Flash** - No rate limits (Ollama) or high limits (Gemini).

### For Occasional Use
**Use OpenAI GPT-3.5-turbo** - Very cheap and fast for occasional video generation.

---

## Configuration in Application

### Via Settings Tab (Recommended)

1. Open the application
2. Go to **Settings** tab
3. Select **AI Provider** from dropdown
4. Fill in the relevant fields:
   - **Ollama**: URL and Model
   - **OpenAI**: API Key and Model
   - **Anthropic**: API Key and Model
   - **Gemini**: API Key and Model
5. Click **Save Settings**

### Via config.json (Advanced)

Edit `config.json` in the application directory:

```json
{
  "AiProvider": "openai",
  "OllamaUrl": "http://localhost:11434",
  "OllamaModel": "llama3.1",
  "OpenAiApiKey": "sk-your-key-here",
  "OpenAiModel": "gpt-4",
  "AnthropicApiKey": "sk-ant-your-key-here",
  "AnthropicModel": "claude-3-5-sonnet-20241022",
  "GeminiApiKey": "your-key-here",
  "GeminiModel": "gemini-1.5-pro"
}
```

---

## Troubleshooting

### "API key is not configured"
- Make sure you've entered your API key in the Settings tab
- Click "Save Settings" after entering the key
- Restart the application if needed

### "Failed to generate script"
- **OpenAI**: Check your API key is valid and has credits
- **Anthropic**: Verify your API key and account status
- **Gemini**: Ensure you haven't exceeded rate limits
- **Ollama**: Make sure Ollama is running (`ollama serve`)

### Rate Limit Errors
- **Gemini Free Tier**: Wait a minute between requests
- **OpenAI/Anthropic**: Upgrade your plan or wait for rate limit reset
- **Ollama**: No rate limits

### Poor Script Quality
- Try a different model (e.g., GPT-4 instead of GPT-3.5)
- Adjust your Channel DNA settings for better prompts
- Provide more detailed topic descriptions

---

## Security Best Practices

### Protecting Your API Keys

1. **Never share your config.json file** - It contains your API keys
2. **Add config.json to .gitignore** - Don't commit it to version control
3. **Use environment variables** (advanced) - Store keys separately
4. **Rotate keys regularly** - Generate new keys periodically
5. **Monitor usage** - Check your provider dashboards for unexpected usage

### API Key Storage

The application stores API keys in `config.json` in plain text. For production use:
- Keep config.json in a secure location
- Set appropriate file permissions (read-only for your user)
- Consider encrypting the file or using a secrets manager

---

## Cost Management

### Tips to Reduce Costs

1. **Use Ollama for testing** - Perfect scripts locally before using paid APIs
2. **Choose cheaper models** - GPT-3.5 or Claude Haiku for simple videos
3. **Batch your work** - Generate multiple videos in one session
4. **Monitor usage** - Set up billing alerts in your provider dashboard
5. **Use free tiers** - Gemini offers generous free tier limits

### Estimated Monthly Costs

**Light Use (10 videos/month):**
- Ollama: Free
- GPT-3.5: ~$0.01
- Gemini Free: Free
- Claude Haiku: ~$0.02

**Medium Use (100 videos/month):**
- Ollama: Free
- GPT-3.5: ~$0.10
- Gemini Pro: ~$0.70
- Claude Sonnet: ~$2.00
- GPT-4: ~$3.00

**Heavy Use (1000 videos/month):**
- Ollama: Free
- GPT-3.5: ~$1.00
- Gemini Flash: ~$0.40
- Claude Haiku: ~$2.00
- GPT-4: ~$30.00

---

## Support

For more help:
- Check the main [README.md](README.md)
- See [TROUBLESHOOTING.md](TROUBLESHOOTING.md)
- Visit provider documentation:
  - OpenAI: https://platform.openai.com/docs
  - Anthropic: https://docs.anthropic.com
  - Google Gemini: https://ai.google.dev/docs
  - Ollama: https://ollama.ai/docs
