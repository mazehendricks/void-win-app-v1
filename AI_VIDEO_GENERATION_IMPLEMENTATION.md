# AI Video Generation Implementation Plan

## Overview
Transform Void Video Generator from image slideshow + voiceover to **actual AI-generated video content**.

## Current State
- ✅ Script generation (Ollama, OpenAI, Anthropic, Gemini)
- ✅ Voice synthesis (Piper TTS)
- ✅ Image fetching (Unsplash)
- ❌ **Missing: Actual video generation**

## Target State
- ✅ Generate real AI video clips from text prompts
- ✅ Support multiple AI video providers
- ✅ Local and cloud options
- ✅ Professional video output

---

## AI Video Generation Options

### Option 1: Cloud-Based APIs (Easiest, Best Quality)

#### A. **Runway ML Gen-3** ⭐⭐⭐ (Recommended)
- **Quality**: Excellent (industry-leading)
- **Cost**: $0.05/second (~$3 per minute)
- **Speed**: 30-60 seconds per 4-second clip
- **API**: REST API available
- **Features**: Text-to-video, image-to-video, motion control

```csharp
public class RunwayVideoService : IAIVideoGeneratorService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    
    public async Task<string> GenerateVideoAsync(VideoPrompt prompt)
    {
        var request = new
        {
            text_prompt = prompt.Description,
            duration = prompt.Duration, // 4-10 seconds
            aspect_ratio = prompt.AspectRatio, // "16:9", "9:16", "1:1"
            seed = prompt.Seed,
            motion_amount = prompt.MotionIntensity // 0-10
        };
        
        var response = await _httpClient.PostAsJsonAsync(
            "https://api.runwayml.com/v1/generate", 
            request
        );
        
        var result = await response.Content.ReadFromJsonAsync<RunwayResponse>();
        return await DownloadVideoAsync(result.VideoUrl);
    }
}
```

#### B. **Stability AI (Stable Video Diffusion)** ⭐⭐⭐
- **Quality**: Very good
- **Cost**: $0.04/second (~$2.40 per minute)
- **Speed**: 20-40 seconds per 4-second clip
- **API**: REST API available
- **Features**: Image-to-video, camera motion control

#### C. **Pika Labs** ⭐⭐
- **Quality**: Good
- **Cost**: $0.03/second (~$1.80 per minute)
- **Speed**: 15-30 seconds per 3-second clip
- **API**: Limited API access
- **Features**: Text-to-video, lip sync

#### D. **Luma AI (Dream Machine)** ⭐⭐⭐
- **Quality**: Excellent
- **Cost**: $0.06/second (~$3.60 per minute)
- **Speed**: 40-80 seconds per 5-second clip
- **API**: REST API available
- **Features**: Text-to-video, extend video, camera controls

---

### Option 2: Local/Open-Source (Free, Privacy)

#### A. **AnimateDiff** ⭐⭐⭐ (Recommended for Local)
- **Quality**: Good (depends on base model)
- **Cost**: Free (requires GPU)
- **Speed**: 2-5 minutes per 2-second clip (RTX 3060+)
- **Setup**: ComfyUI or Automatic1111 integration
- **Features**: Text-to-video, motion modules, LoRA support

```csharp
public class AnimateDiffService : IAIVideoGeneratorService
{
    private readonly string _comfyUIEndpoint = "http://localhost:8188";
    
    public async Task<string> GenerateVideoAsync(VideoPrompt prompt)
    {
        var workflow = new
        {
            prompt = prompt.Description,
            negative_prompt = prompt.NegativePrompt,
            num_frames = prompt.Duration * 8, // 8 fps
            motion_module = "mm_sd_v15_v2.ckpt",
            width = 512,
            height = 512,
            seed = prompt.Seed ?? -1
        };
        
        var response = await _httpClient.PostAsJsonAsync(
            $"{_comfyUIEndpoint}/prompt", 
            workflow
        );
        
        return await PollForCompletion(response);
    }
}
```

#### B. **Zeroscope** ⭐⭐
- **Quality**: Moderate
- **Cost**: Free (requires GPU)
- **Speed**: 1-3 minutes per 2-second clip
- **Setup**: Hugging Face Diffusers
- **Features**: Text-to-video, 576x320 resolution

#### C. **ModelScope** ⭐
- **Quality**: Basic
- **Cost**: Free
- **Speed**: 30-60 seconds per 2-second clip
- **Setup**: Hugging Face Diffusers
- **Features**: Text-to-video, Chinese-optimized

---

### Option 3: Hybrid Approach ⭐⭐⭐ (Best Value)

**Strategy**: Generate keyframes with Stable Diffusion + Interpolate with AI

```csharp
public class HybridVideoService : IAIVideoGeneratorService
{
    private readonly IImageGeneratorService _imageGenerator;
    private readonly IFrameInterpolationService _interpolator;
    
    public async Task<string> GenerateVideoAsync(VideoPrompt prompt)
    {
        // 1. Generate keyframes (every 0.5 seconds)
        var keyframes = new List<string>();
        for (int i = 0; i < prompt.Duration * 2; i++)
        {
            var frame = await _imageGenerator.GenerateImageAsync(
                prompt.Description,
                seed: prompt.Seed + i
            );
            keyframes.Add(frame);
        }
        
        // 2. Interpolate between keyframes using FILM or RIFE
        var interpolatedFrames = await _interpolator.InterpolateAsync(
            keyframes,
            targetFps: 24
        );
        
        // 3. Assemble into video
        return await AssembleVideoFromFrames(interpolatedFrames);
    }
}
```

**Tools Needed**:
- **FILM** (Frame Interpolation for Large Motion) - Google's interpolation
- **RIFE** (Real-Time Intermediate Flow Estimation) - Fast interpolation
- **Stable Diffusion** - Keyframe generation

---

## Recommended Implementation Strategy

### Phase 1: Quick Win (Cloud API)
**Use Runway ML or Luma AI for immediate results**

**Pros**:
- ✅ Best quality immediately
- ✅ No local GPU required
- ✅ Fast implementation (1-2 days)
- ✅ Reliable results

**Cons**:
- ❌ Costs money per generation
- ❌ Requires internet
- ❌ Privacy concerns

### Phase 2: Local Option (AnimateDiff)
**Add AnimateDiff for users with GPUs**

**Pros**:
- ✅ Free after setup
- ✅ Complete privacy
- ✅ Unlimited generations
- ✅ Customizable

**Cons**:
- ❌ Requires powerful GPU (RTX 3060+ recommended)
- ❌ Slower generation
- ❌ More complex setup

### Phase 3: Hybrid Fallback
**Implement keyframe + interpolation for budget users**

---

## Implementation Code Structure

### 1. Core Interface

```csharp
public interface IAIVideoGeneratorService
{
    Task<string> GenerateVideoAsync(VideoPrompt prompt);
    Task<bool> IsAvailableAsync();
    Task<VideoGenerationStatus> GetStatusAsync(string jobId);
    Task CancelGenerationAsync(string jobId);
}

public class VideoPrompt
{
    public string Description { get; set; }
    public string NegativePrompt { get; set; }
    public int Duration { get; set; } // seconds
    public string AspectRatio { get; set; } // "16:9", "9:16", "1:1"
    public int? Seed { get; set; }
    public float MotionIntensity { get; set; } // 0-10
    public string Style { get; set; } // "cinematic", "realistic", "animated"
    public string CameraMotion { get; set; } // "static", "pan", "zoom"
    public string ReferenceImagePath { get; set; } // Optional
}

public class VideoGenerationStatus
{
    public string JobId { get; set; }
    public string Status { get; set; } // "queued", "processing", "completed", "failed"
    public int Progress { get; set; } // 0-100
    public string VideoUrl { get; set; }
    public string ErrorMessage { get; set; }
}
```

### 2. Runway ML Implementation

```csharp
public class RunwayMLVideoService : IAIVideoGeneratorService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private const string BaseUrl = "https://api.runwayml.com/v1";
    
    public RunwayMLVideoService(string apiKey)
    {
        _apiKey = apiKey;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
    }
    
    public async Task<string> GenerateVideoAsync(VideoPrompt prompt)
    {
        // 1. Submit generation request
        var request = new
        {
            text_prompt = prompt.Description,
            duration = Math.Min(prompt.Duration, 10), // Max 10 seconds
            aspect_ratio = prompt.AspectRatio ?? "16:9",
            seed = prompt.Seed,
            motion_amount = (int)prompt.MotionIntensity,
            style = prompt.Style ?? "realistic"
        };
        
        var response = await _httpClient.PostAsJsonAsync(
            $"{BaseUrl}/generate",
            request
        );
        
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<RunwayResponse>();
        
        // 2. Poll for completion
        var videoUrl = await PollForCompletionAsync(result.JobId);
        
        // 3. Download video
        var localPath = await DownloadVideoAsync(videoUrl);
        
        return localPath;
    }
    
    private async Task<string> PollForCompletionAsync(string jobId)
    {
        while (true)
        {
            var status = await GetStatusAsync(jobId);
            
            if (status.Status == "completed")
                return status.VideoUrl;
            
            if (status.Status == "failed")
                throw new Exception($"Generation failed: {status.ErrorMessage}");
            
            await Task.Delay(2000); // Poll every 2 seconds
        }
    }
    
    public async Task<VideoGenerationStatus> GetStatusAsync(string jobId)
    {
        var response = await _httpClient.GetAsync($"{BaseUrl}/status/{jobId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<VideoGenerationStatus>();
    }
    
    private async Task<string> DownloadVideoAsync(string url)
    {
        var videoData = await _httpClient.GetByteArrayAsync(url);
        var outputPath = Path.Combine(
            Path.GetTempPath(),
            $"runway_{Guid.NewGuid()}.mp4"
        );
        await File.WriteAllBytesAsync(outputPath, videoData);
        return outputPath;
    }
    
    public async Task<bool> IsAvailableAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/health");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task CancelGenerationAsync(string jobId)
    {
        await _httpClient.DeleteAsync($"{BaseUrl}/jobs/{jobId}");
    }
}

public class RunwayResponse
{
    public string JobId { get; set; }
    public string Status { get; set; }
}
```

### 3. AnimateDiff Local Implementation

```csharp
public class AnimateDiffService : IAIVideoGeneratorService
{
    private readonly string _comfyUIEndpoint;
    private readonly HttpClient _httpClient;
    
    public AnimateDiffService(string comfyUIEndpoint = "http://localhost:8188")
    {
        _comfyUIEndpoint = comfyUIEndpoint;
        _httpClient = new HttpClient();
    }
    
    public async Task<string> GenerateVideoAsync(VideoPrompt prompt)
    {
        // ComfyUI workflow for AnimateDiff
        var workflow = CreateAnimateDiffWorkflow(prompt);
        
        var response = await _httpClient.PostAsJsonAsync(
            $"{_comfyUIEndpoint}/prompt",
            new { prompt = workflow }
        );
        
        var result = await response.Content.ReadFromJsonAsync<ComfyUIResponse>();
        
        // Wait for completion
        var outputPath = await WaitForOutputAsync(result.PromptId);
        
        return outputPath;
    }
    
    private object CreateAnimateDiffWorkflow(VideoPrompt prompt)
    {
        // This is a simplified workflow - actual ComfyUI workflows are more complex
        return new
        {
            // Load checkpoint
            checkpoint_loader = new
            {
                class_type = "CheckpointLoaderSimple",
                inputs = new { ckpt_name = "realisticVisionV51_v51VAE.safetensors" }
            },
            
            // AnimateDiff loader
            animatediff_loader = new
            {
                class_type = "AnimateDiffLoader",
                inputs = new { model_name = "mm_sd_v15_v2.ckpt" }
            },
            
            // Positive prompt
            positive_prompt = new
            {
                class_type = "CLIPTextEncode",
                inputs = new { text = prompt.Description }
            },
            
            // Negative prompt
            negative_prompt = new
            {
                class_type = "CLIPTextEncode",
                inputs = new { text = prompt.NegativePrompt ?? "blurry, low quality" }
            },
            
            // KSampler
            sampler = new
            {
                class_type = "KSampler",
                inputs = new
                {
                    seed = prompt.Seed ?? new Random().Next(),
                    steps = 20,
                    cfg = 7.5,
                    sampler_name = "euler_ancestral",
                    scheduler = "normal",
                    denoise = 1.0
                }
            },
            
            // Video output
            video_output = new
            {
                class_type = "VHS_VideoCombine",
                inputs = new
                {
                    frame_rate = 8,
                    format = "video/h264-mp4"
                }
            }
        };
    }
    
    public async Task<bool> IsAvailableAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_comfyUIEndpoint}/system_stats");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
```

### 4. Updated Video Generation Pipeline

```csharp
public class AIVideoGenerationPipeline
{
    private readonly IScriptGeneratorService _scriptGenerator;
    private readonly IAIVideoGeneratorService _videoGenerator;
    private readonly IVoiceGeneratorService _voiceGenerator;
    private readonly IVideoAssemblyService _videoAssembler;
    
    public async Task<string> GenerateVideoAsync(VideoRequest request)
    {
        // 1. Generate script
        var script = await _scriptGenerator.GenerateScriptAsync(request);
        
        // 2. Generate video clips for each scene
        var videoClips = new List<string>();
        foreach (var segment in script.Segments)
        {
            var videoPrompt = new VideoPrompt
            {
                Description = segment.VisualCue,
                Duration = (int)segment.Duration,
                AspectRatio = request.AspectRatio ?? "16:9",
                Style = request.VideoStyle ?? "realistic",
                MotionIntensity = request.MotionIntensity ?? 5
            };
            
            var clip = await _videoGenerator.GenerateVideoAsync(videoPrompt);
            videoClips.Add(clip);
        }
        
        // 3. Generate voiceover
        var audioPath = await _voiceGenerator.GenerateVoiceAsync(script);
        
        // 4. Combine clips and sync with audio
        var finalVideo = await _videoAssembler.AssembleVideoAsync(
            videoClips,
            audioPath,
            request.OutputSettings
        );
        
        return finalVideo;
    }
}
```

---

## Configuration Updates

### Add to config.json

```json
{
  "AIVideoGeneration": {
    "Provider": "RunwayML",  // "RunwayML", "LumaAI", "AnimateDiff", "Hybrid"
    "RunwayML": {
      "ApiKey": "your-runway-api-key",
      "MaxDuration": 10,
      "DefaultStyle": "realistic"
    },
    "LumaAI": {
      "ApiKey": "your-luma-api-key",
      "MaxDuration": 5
    },
    "AnimateDiff": {
      "ComfyUIEndpoint": "http://localhost:8188",
      "ModelPath": "models/animatediff/mm_sd_v15_v2.ckpt",
      "CheckpointPath": "models/checkpoints/realisticVisionV51.safetensors"
    },
    "DefaultSettings": {
      "AspectRatio": "16:9",
      "MotionIntensity": 5,
      "Style": "realistic",
      "NegativePrompt": "blurry, low quality, distorted, watermark"
    }
  }
}
```

---

## UI Updates Needed

### Settings Tab - Add AI Video Section

```csharp
// Add to MainForm.Designer.cs
var grpAIVideo = new GroupBox {
    Text = "AI Video Generation",
    Location = new Point(10, yPos),
    Size = new Size(900, 180)
};

var lblProvider = new Label {
    Text = "Video Provider:",
    Location = new Point(10, 25)
};

var cmbProvider = new ComboBox {
    Location = new Point(120, 22),
    Size = new Size(200, 25),
    DropDownStyle = ComboBoxStyle.DropDownList
};
cmbProvider.Items.AddRange(new[] { 
    "Runway ML (Cloud)", 
    "Luma AI (Cloud)", 
    "AnimateDiff (Local)",
    "Hybrid (Local+Interpolation)"
});

var lblApiKey = new Label {
    Text = "API Key:",
    Location = new Point(10, 55)
};

var txtVideoApiKey = new TextBox {
    Location = new Point(120, 52),
    Size = new Size(400, 25),
    PasswordChar = '*'
};

// Motion intensity slider
var lblMotion = new Label {
    Text = "Motion Intensity:",
    Location = new Point(10, 85)
};

var trackMotion = new TrackBar {
    Location = new Point(120, 82),
    Size = new Size(300, 45),
    Minimum = 0,
    Maximum = 10,
    Value = 5
};
```

---

## Cost Estimation

### Cloud Providers (per 60-second video):
- **Runway ML**: ~$3.00
- **Luma AI**: ~$3.60
- **Pika Labs**: ~$1.80
- **Stability AI**: ~$2.40

### Local (AnimateDiff):
- **Cost**: $0 (electricity only)
- **Time**: 10-30 minutes (depending on GPU)
- **GPU Required**: RTX 3060 or better

---

## Next Steps

1. **Choose Provider**: Start with Runway ML for best quality
2. **Implement Service**: Create RunwayMLVideoService
3. **Update Pipeline**: Integrate into VideoGenerationPipeline
4. **Update UI**: Add provider selection and settings
5. **Test**: Generate sample videos
6. **Add Local Option**: Implement AnimateDiff for GPU users

---

## Recommendation

**Start with Runway ML** because:
- ✅ Best quality results
- ✅ Fastest implementation
- ✅ Most reliable
- ✅ Good documentation
- ✅ Reasonable pricing

**Then add AnimateDiff** for users who want:
- ✅ Free unlimited generations
- ✅ Complete privacy
- ✅ Offline capability
- ✅ Full control

This gives users choice between quality/speed (cloud) and cost/privacy (local).
