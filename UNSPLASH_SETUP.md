# Unsplash API Integration Guide

The Void Video Generator now supports automatic image generation using the Unsplash API. This feature allows you to automatically download relevant images based on the visual cues in your video script.

## Features

✅ **Automatic Image Generation** - Images are automatically downloaded based on visual cues from your script
✅ **Free API** - Unsplash offers a free tier with 50 requests per hour
✅ **High Quality** - Professional, royalty-free images
✅ **Smart Fallback** - Falls back to topic-based search if no visual cues are found

## Setup Instructions

### 1. Get Your Free Unsplash API Key

1. Go to [https://unsplash.com/developers](https://unsplash.com/developers)
2. Click "Register as a developer"
3. Create a new application
4. Copy your **Access Key** (not the Secret Key)

### 2. Configure in the Application

1. Open **Void Video Generator**
2. Go to the **Settings** tab
3. Find the **"Unsplash Image Generation"** section
4. Check **"Enable automatic image generation from Unsplash"**
5. Paste your **API Key** in the text field
6. Click **"Save Settings"**

## How It Works

### Priority System

The application uses images in this priority order:

1. **User-Provided Images** (highest priority)
   - If you manually add images via "Add Images" button, those are used first
   
2. **Unsplash API Images** (if enabled)
   - Automatically downloads images based on visual cues from the script
   - Example: If script contains `[mountain landscape]`, it searches for "mountain landscape"
   
3. **Placeholder Images** (fallback)
   - Black placeholder images if no other source is available

### Visual Cues

The script generator creates visual cues in `[brackets]`. For example:

```
Welcome to this amazing tutorial! [smiling person at computer]
Today we'll explore the mountains. [mountain landscape]
Let's get started! [hiking trail]
```

The Unsplash service will:
- Search for "smiling person at computer"
- Search for "mountain landscape"  
- Search for "hiking trail"
- Download one image for each cue

### If No Visual Cues

If the script doesn't contain visual cues, the system will:
- Use the video **topic** as the search query
- Download 3 images related to the topic

## Usage Examples

### Example 1: With Visual Cues

**Script:**
```
Hook: Did you know? [surprised face]
The ocean is full of mysteries [underwater scene]
Let's dive in! [scuba diver]
```

**Result:** 3 images downloaded (surprised face, underwater scene, scuba diver)

### Example 2: Without Visual Cues

**Topic:** "Healthy Cooking Tips"

**Result:** 3 images related to "Healthy Cooking Tips"

## API Limits

### Free Tier
- **50 requests per hour**
- **50,000 requests per month**
- Perfect for most users

### Rate Limiting
If you exceed the limit:
- The app will show a warning
- Fall back to placeholder images
- Wait an hour and try again

## Troubleshooting

### "Unsplash API key not configured"
- Make sure you've entered your API key in Settings
- Check that "Enable automatic image generation" is checked
- Click "Save Settings"

### "Failed to download image"
- Check your internet connection
- Verify your API key is correct
- Check if you've exceeded rate limits
- Try again in a few minutes

### Images Don't Match Content
- Improve your visual cues in the script
- Use more specific descriptions
- Example: Instead of `[person]`, use `[professional woman in office]`

## Best Practices

### 1. Write Clear Visual Cues
❌ Bad: `[thing]`, `[stuff]`, `[image]`
✅ Good: `[modern laptop on desk]`, `[sunset over ocean]`, `[happy family]`

### 2. Be Specific
❌ Bad: `[car]`
✅ Good: `[red sports car on highway]`

### 3. Use Relevant Keywords
- Think about what you want to see
- Use descriptive adjectives
- Include context when helpful

### 4. Limit Visual Cues
- Don't overuse visual cues (API limits)
- 5-10 cues per video is usually enough
- Images are distributed evenly throughout the video

## Combining Methods

You can mix and match:

1. **Add some manual images** for specific scenes
2. **Enable Unsplash** for additional variety
3. **Result:** Manual images + Unsplash images combined

## Privacy & Attribution

- Unsplash images are **royalty-free**
- No attribution required for free tier
- Images are downloaded to your local machine
- No data is sent to Unsplash except search queries

## Alternative: Manual Images

If you prefer full control:
1. Disable Unsplash in Settings
2. Use the "Add Images" button on the Generate tab
3. Select your own images (PNG, JPG, etc.)

## Support

For issues with:
- **Unsplash API**: Visit [Unsplash Help](https://help.unsplash.com/)
- **This Application**: Check TROUBLESHOOTING.md

---

**Note:** This feature is optional. The application works perfectly fine with manual images or placeholder visuals if you prefer not to use the Unsplash API.
