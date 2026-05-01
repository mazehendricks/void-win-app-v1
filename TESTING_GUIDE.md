# Comprehensive Testing Guide - Void Video Generator

**Purpose:** Verify every feature works correctly before release

**Test Environment:** Windows 10/11, .NET 8.0

---

## 🧪 Pre-Test Setup Checklist

### Required Software:
- [ ] .NET 8.0 SDK installed
- [ ] Ollama installed and running
- [ ] FFmpeg installed and in PATH
- [ ] Piper TTS installed
- [ ] At least one Piper voice model downloaded

### Optional Software:
- [ ] OpenAI API key (for OpenAI testing)
- [ ] Anthropic API key (for Claude testing)
- [ ] Google Gemini API key (for Gemini testing)
- [ ] Unsplash API key (for image testing)

### Test Data:
- [ ] Sample images (5-10 JPG/PNG files)
- [ ] Test output directory created
- [ ] config.json backed up

---

## 📋 Test Categories

### 1. Installation & Startup Tests

#### Test 1.1: Fresh Installation
**Objective:** Verify app starts on first run

**Steps:**
1. Delete `config.json` if exists
2. Run `VoidVideoGenerator.exe`
3. Verify default config is created
4. Check all tabs load correctly

**Expected Results:**
- ✅ App starts without errors
- ✅ Default config.json created
- ✅ All 4 tabs visible (Generate, Settings, Status, Visual)
- ✅ No crash or freeze

**Status:** [ ] Pass [ ] Fail

**Notes:**
```
_______________________________________________________
```

---

#### Test 1.2: Configuration Loading
**Objective:** Verify config loads correctly

**Steps:**
1. Edit `config.json` with custom values
2. Restart application
3. Check Settings tab shows custom values

**Expected Results:**
- ✅ Custom values loaded correctly
- ✅ No errors in log
- ✅ All fields populated

**Status:** [ ] Pass [ ] Fail

**Notes:**
```
_______________________________________________________
```

---

### 2. AI Provider Tests

#### Test 2.1: Ollama (Local AI)
**Objective:** Verify Ollama integration works

**Prerequisites:**
- Ollama running on localhost:11434
- Model downloaded (e.g., llama3.1)

**Steps:**
1. Go to Settings tab
2. Select "Ollama" as AI Provider
3. Set Ollama URL: `http://localhost:11434`
4. Set Model: `llama3.1`
5. Click "Save All Settings"
6. Go to Status tab
7. Click "Check System Status"

**Expected Results:**
- ✅ Ollama shows as "Available"
- ✅ No connection errors
- ✅ Model name displayed correctly

**Status:** [ ] Pass [ ] Fail

**Notes:**
```
_______________________________________________________
```

---

#### Test 2.2: OpenAI Integration
**Objective:** Verify OpenAI API works

**Prerequisites:**
- Valid OpenAI API key

**Steps:**
1. Go to Settings tab
2. Select "OpenAI" as AI Provider
3. Enter API key
4. Set Model: `gpt-4`
5. Click "Save All Settings"
6. Go to Generate tab
7. Enter test title: "Test Video"
8. Enter topic: "Explain quantum computing in simple terms"
9. Click "Generate Video"

**Expected Results:**
- ✅ Script generation starts
- ✅ No API errors
- ✅ Script contains relevant content
- ✅ Visual cues generated

**Status:** [ ] Pass [ ] Fail

**Notes:**
```
_______________________________________________________
```

---

#### Test 2.3: Anthropic (Claude) Integration
**Objective:** Verify Claude API works

**Prerequisites:**
- Valid Anthropic API key

**Steps:**
1. Go to Settings tab
2. Select "Anthropic" as AI Provider
3. Enter API key
4. Set Model: `claude-3-5-sonnet-20241022`
5. Click "Save All Settings"
6. Generate a test video

**Expected Results:**
- ✅ Script generation works
- ✅ No API errors
- ✅ High-quality script output

**Status:** [ ] Pass [ ] Fail

**Notes:**
```
_______________________________________________________
```

---

#### Test 2.4: Google Gemini Integration
**Objective:** Verify Gemini API works

**Prerequisites:**
- Valid Gemini API key

**Steps:**
1. Go to Settings tab
2. Select "Gemini" as AI Provider
3. Enter API key
4. Set Model: `gemini-pro`
5. Click "Save All Settings"
6. Generate a test video

**Expected Results:**
- ✅ Script generation works
- ✅ No API errors
- ✅ Script quality acceptable

**Status:** [ ] Pass [ ] Fail

**Notes:**
```
_______________________________________________________
```

---

### 3. Voice Generation Tests

#### Test 3.1: Piper TTS Basic
**Objective:** Verify voice generation works

**Prerequisites:**
- Piper installed
- Voice model downloaded

**Steps:**
1. Go to Settings tab
2. Set Piper Path: `C:\path\to\piper.exe`
3. Set Model Path: `C:\path\to\model.onnx`
4. Click "Save All Settings"
5. Generate a short video (30 seconds)

**Expected Results:**
- ✅ Audio files generated in output/audio/
- ✅ Audio is clear and understandable
- ✅ No distortion or artifacts
- ✅ Proper timing/pacing

**Status:** [ ] Pass [ ] Fail

**Notes:**
```
_______________________________________________________
```

---

#### Test 3.2: Multiple Voice Models
**Objective:** Test different voice models

**Steps:**
1. Generate video with voice model A
2. Change to voice model B
3. Generate same script again
4. Compare audio quality

**Expected Results:**
- ✅ Both models work
- ✅ Different voice characteristics
- ✅ No errors switching models

**Status:** [ ] Pass [ ] Fail

**Notes:**
```
_______________________________________________________
```

---

### 4. Video Assembly Tests

#### Test 4.1: Basic Video Generation
**Objective:** Complete end-to-end video generation

**Steps:**
1. Go to Generate tab
2. Enter Title: "Test Video - Basic"
3. Enter Topic: "The benefits of exercise"
4. Set Duration: 60 seconds
5. Set Output Path
6. Click "Generate Video"
7. Wait for completion

**Expected Results:**
- ✅ Video file created (.mp4)
- ✅ Video plays correctly
- ✅ Audio synced with visuals
- ✅ No corruption or errors
- ✅ File size reasonable

**Status:** [ ] Pass [ ] Fail

**Video Path:**
```
_______________________________________________________
```

**File Size:** _______ MB

**Duration:** _______ seconds

---

#### Test 4.2: GPU Acceleration
**Objective:** Verify GPU encoding works

**Prerequisites:**
- NVIDIA GPU with NVENC support

**Steps:**
1. Go to Settings tab
2. Enable "Use GPU Acceleration"
3. Select GPU Encoder: `h264_nvenc`
4. Generate a video
5. Monitor GPU usage (Task Manager)

**Expected Results:**
- ✅ GPU usage increases during encoding
- ✅ Faster encoding than CPU
- ✅ Video quality maintained
- ✅ No encoding errors

**Status:** [ ] Pass [ ] Fail

**Encoding Time (GPU):** _______ seconds
**Encoding Time (CPU):** _______ seconds
**Speedup:** _______x

---

#### Test 4.3: Different Resolutions
**Objective:** Test various output resolutions

**Test Cases:**
| Resolution | Status | File Size | Notes |
|------------|--------|-----------|-------|
| 720p       | [ ]    | _____ MB  |       |
| 1080p      | [ ]    | _____ MB  |       |
| 1440p      | [ ]    | _____ MB  |       |
| 4K         | [ ]    | _____ MB  |       |

**Expected Results:**
- ✅ All resolutions generate successfully
- ✅ Quality scales appropriately
- ✅ File sizes increase with resolution

---

#### Test 4.4: Different Frame Rates
**Objective:** Test various frame rates

**Test Cases:**
| Frame Rate | Status | Smoothness | Notes |
|------------|--------|------------|-------|
| 24 fps     | [ ]    | [ ] Good   |       |
| 30 fps     | [ ]    | [ ] Good   |       |
| 60 fps     | [ ]    | [ ] Good   |       |

**Expected Results:**
- ✅ All frame rates work
- ✅ Higher FPS = smoother motion
- ✅ No frame drops

---

### 5. Visual Content Tests

#### Test 5.1: User-Provided Images
**Objective:** Test custom image input

**Steps:**
1. Go to Visual tab
2. Click "Add Images"
3. Select 5-10 images
4. Verify images appear in list
5. Generate video
6. Check images used in video

**Expected Results:**
- ✅ Images load correctly
- ✅ Images appear in video
- ✅ Proper timing/duration
- ✅ No distortion

**Status:** [ ] Pass [ ] Fail

---

#### Test 5.2: Unsplash Integration
**Objective:** Test automatic image sourcing

**Prerequisites:**
- Unsplash API key

**Steps:**
1. Go to Settings tab
2. Enable "Use Unsplash Images"
3. Enter Unsplash API key
4. Generate video WITHOUT adding custom images
5. Check visuals directory for downloaded images

**Expected Results:**
- ✅ Images downloaded from Unsplash
- ✅ Images relevant to topic
- ✅ Proper attribution saved
- ✅ No API errors

**Status:** [ ] Pass [ ] Fail

**Images Downloaded:** _______

---

#### Test 5.3: Ken Burns Effect
**Objective:** Test image animation

**Steps:**
1. Go to Settings tab
2. Enable "Ken Burns Effect"
3. Set Zoom Intensity: 1.2
4. Generate video with static images

**Expected Results:**
- ✅ Images have zoom/pan motion
- ✅ Smooth animation
- ✅ No jittering
- ✅ Natural movement

**Status:** [ ] Pass [ ] Fail

---

#### Test 5.4: Crossfade Transitions
**Objective:** Test scene transitions

**Steps:**
1. Go to Settings tab
2. Enable "Crossfade Transitions"
3. Set Transition Duration: 0.5 seconds
4. Generate video with multiple images

**Expected Results:**
- ✅ Smooth transitions between images
- ✅ No hard cuts
- ✅ Proper timing
- ✅ No black frames

**Status:** [ ] Pass [ ] Fail

---

### 6. Channel DNA Tests

#### Test 6.1: Custom Channel DNA
**Objective:** Verify personalization works

**Steps:**
1. Go to Generate tab
2. Fill in Channel DNA fields:
   - Niche: "Technology Reviews"
   - Persona: "Enthusiastic tech expert"
   - Tone: "Casual and friendly"
   - Audience: "Tech enthusiasts 18-35"
   - Style: "Fast-paced with humor"
3. Generate video
4. Review script for personality

**Expected Results:**
- ✅ Script reflects specified niche
- ✅ Tone matches guidelines
- ✅ Content appropriate for audience
- ✅ Style consistent throughout

**Status:** [ ] Pass [ ] Fail

---

### 7. Error Handling Tests

#### Test 7.1: Missing Dependencies
**Objective:** Verify graceful error handling

**Test Cases:**

**7.1a: Ollama Not Running**
- Stop Ollama service
- Try to generate video
- Expected: Clear error message, no crash
- Status: [ ] Pass [ ] Fail

**7.1b: FFmpeg Not Found**
- Rename FFmpeg executable
- Try to generate video
- Expected: Error message about FFmpeg
- Status: [ ] Pass [ ] Fail

**7.1c: Piper Not Found**
- Set invalid Piper path
- Try to generate video
- Expected: Error message about Piper
- Status: [ ] Pass [ ] Fail

**7.1d: Invalid API Key**
- Enter fake API key
- Try to generate video
- Expected: API authentication error
- Status: [ ] Pass [ ] Fail

---

#### Test 7.2: Invalid Input
**Objective:** Test input validation

**Test Cases:**

**7.2a: Empty Title**
- Leave title blank
- Click Generate
- Expected: Validation error, focus on title field
- Status: [ ] Pass [ ] Fail

**7.2b: Short Title**
- Enter "Hi" (< 5 chars)
- Click Generate
- Expected: Validation error
- Status: [ ] Pass [ ] Fail

**7.2c: Empty Topic**
- Leave topic blank
- Click Generate
- Expected: Validation error, focus on topic field
- Status: [ ] Pass [ ] Fail

**7.2d: Short Topic**
- Enter "Test" (< 10 chars)
- Click Generate
- Expected: Validation error
- Status: [ ] Pass [ ] Fail

**7.2e: Invalid Output Path**
- Set output to invalid path (e.g., "Z:\invalid\path")
- Click Generate
- Expected: Directory creation prompt or error
- Status: [ ] Pass [ ] Fail

---

#### Test 7.3: Resource Exhaustion
**Objective:** Test behavior under stress

**Test Cases:**

**7.3a: Disk Full**
- Generate video with very little disk space
- Expected: Clear error message about disk space
- Status: [ ] Pass [ ] Fail

**7.3b: Out of Memory**
- Generate very long video (10+ minutes)
- Monitor memory usage
- Expected: Completes or fails gracefully
- Status: [ ] Pass [ ] Fail

---

### 8. Configuration Tests

#### Test 8.1: Save/Load Settings
**Objective:** Verify settings persistence

**Steps:**
1. Change multiple settings
2. Click "Save All Settings"
3. Close application
4. Reopen application
5. Verify settings retained

**Expected Results:**
- ✅ All settings saved correctly
- ✅ Settings load on restart
- ✅ No data loss

**Status:** [ ] Pass [ ] Fail

---

#### Test 8.2: Default Settings
**Objective:** Verify defaults are sensible

**Steps:**
1. Delete config.json
2. Start application
3. Review default values

**Expected Results:**
- ✅ Ollama selected by default
- ✅ Reasonable video settings
- ✅ Safe default paths

**Status:** [ ] Pass [ ] Fail

---

### 9. UI/UX Tests

#### Test 9.1: Keyboard Shortcuts
**Objective:** Verify shortcuts work

**Test Cases:**
| Shortcut | Action | Status |
|----------|--------|--------|
| Ctrl+G   | Generate Video | [ ] |
| F1       | Switch to Generate Tab | [ ] |
| F2       | Switch to Settings Tab | [ ] |
| F3       | Switch to Status Tab | [ ] |
| F4       | Switch to Visual Tab | [ ] |
| Ctrl+Q   | Quit Application | [ ] |

**Expected Results:**
- ✅ All shortcuts work
- ✅ No conflicts
- ✅ Responsive

---

#### Test 9.2: Progress Feedback
**Objective:** Verify user feedback during generation

**Steps:**
1. Start video generation
2. Observe progress bar
3. Read log messages
4. Monitor status updates

**Expected Results:**
- ✅ Progress bar updates smoothly
- ✅ Log messages clear and informative
- ✅ Current step always visible
- ✅ Percentage accurate

**Status:** [ ] Pass [ ] Fail

---

#### Test 9.3: Theme/Dark Mode
**Objective:** Verify UI appearance

**Steps:**
1. Check dark mode is applied
2. Verify all text is readable
3. Check button contrast
4. Verify no white backgrounds

**Expected Results:**
- ✅ Consistent dark theme
- ✅ Good contrast ratios
- ✅ No eye strain
- ✅ Professional appearance

**Status:** [ ] Pass [ ] Fail

---

### 10. Performance Tests

#### Test 10.1: Generation Speed
**Objective:** Measure performance

**Test Video:** 60-second video, 1080p, 30fps

**Measurements:**
| Step | Time (seconds) | Notes |
|------|----------------|-------|
| Script Generation | _____ | |
| Voice Generation | _____ | |
| Image Processing | _____ | |
| Video Assembly | _____ | |
| **Total** | _____ | |

**Expected Results:**
- ✅ Total time < 5 minutes
- ✅ No unnecessary delays
- ✅ Efficient resource usage

---

#### Test 10.2: Memory Usage
**Objective:** Monitor memory consumption

**Steps:**
1. Open Task Manager
2. Note baseline memory
3. Generate video
4. Monitor peak memory
5. Check for memory leaks

**Measurements:**
- Baseline: _____ MB
- Peak: _____ MB
- After completion: _____ MB

**Expected Results:**
- ✅ Peak < 2GB
- ✅ Memory released after generation
- ✅ No leaks

**Status:** [ ] Pass [ ] Fail

---

#### Test 10.3: CPU/GPU Usage
**Objective:** Verify efficient resource usage

**Steps:**
1. Monitor CPU usage during generation
2. Monitor GPU usage (if enabled)
3. Check for bottlenecks

**Measurements:**
- CPU Usage: _____% average
- GPU Usage: _____% average
- Encoding: [ ] CPU [ ] GPU

**Expected Results:**
- ✅ High utilization during encoding
- ✅ GPU used when enabled
- ✅ No idle periods

**Status:** [ ] Pass [ ] Fail

---

### 11. Output Quality Tests

#### Test 11.1: Video Quality
**Objective:** Verify output quality

**Checklist:**
- [ ] Video plays smoothly
- [ ] No pixelation or artifacts
- [ ] Colors accurate
- [ ] Proper aspect ratio
- [ ] No black bars (unless intended)

**Status:** [ ] Pass [ ] Fail

---

#### Test 11.2: Audio Quality
**Objective:** Verify audio quality

**Checklist:**
- [ ] Clear speech
- [ ] No distortion
- [ ] Proper volume level
- [ ] No clipping
- [ ] Synced with video

**Status:** [ ] Pass [ ] Fail

---

#### Test 11.3: Script Quality
**Objective:** Verify AI-generated content

**Checklist:**
- [ ] Relevant to topic
- [ ] Grammatically correct
- [ ] Logical flow
- [ ] Appropriate length
- [ ] Engaging content

**Status:** [ ] Pass [ ] Fail

---

### 12. Edge Cases

#### Test 12.1: Very Short Video
**Objective:** Test minimum duration

**Steps:**
1. Set duration to 10 seconds
2. Generate video

**Expected Results:**
- ✅ Video generates successfully
- ✅ Proper pacing
- ✅ No errors

**Status:** [ ] Pass [ ] Fail

---

#### Test 12.2: Very Long Video
**Objective:** Test maximum duration

**Steps:**
1. Set duration to 600 seconds (10 minutes)
2. Generate video

**Expected Results:**
- ✅ Video generates successfully
- ✅ No memory issues
- ✅ Proper file size

**Status:** [ ] Pass [ ] Fail

---

#### Test 12.3: Special Characters
**Objective:** Test filename handling

**Steps:**
1. Enter title with special chars: "Test: Video #1 (2024)"
2. Generate video
3. Check filename

**Expected Results:**
- ✅ Special chars sanitized
- ✅ File created successfully
- ✅ No path errors

**Status:** [ ] Pass [ ] Fail

---

#### Test 12.4: Unicode Characters
**Objective:** Test international characters

**Steps:**
1. Enter title: "测试视频 - Тест - テスト"
2. Generate video

**Expected Results:**
- ✅ Unicode handled correctly
- ✅ File created
- ✅ Script displays properly

**Status:** [ ] Pass [ ] Fail

---

### 13. Integration Tests

#### Test 13.1: Full Workflow
**Objective:** Complete realistic workflow

**Scenario:** Create a YouTube video about "How to Start a Podcast"

**Steps:**
1. Configure Ollama as AI provider
2. Set Channel DNA for podcast niche
3. Add 5 custom images
4. Set 1080p, 30fps output
5. Enable GPU acceleration
6. Enable Ken Burns effect
7. Generate video
8. Review output

**Expected Results:**
- ✅ All steps complete without errors
- ✅ High-quality output
- ✅ Ready for upload
- ✅ Professional appearance

**Status:** [ ] Pass [ ] Fail

**Output Quality (1-10):** _____

---

#### Test 13.2: Multiple Videos
**Objective:** Test batch generation

**Steps:**
1. Generate video A
2. Immediately generate video B
3. Generate video C

**Expected Results:**
- ✅ All videos generate successfully
- ✅ No interference between generations
- ✅ Separate output directories
- ✅ No file conflicts

**Status:** [ ] Pass [ ] Fail

---

### 14. Regression Tests

#### Test 14.1: Previous Bug Fixes
**Objective:** Verify bugs stay fixed

**Test Cases:**
- [ ] ThemeColors reference (fixed 2026-05-01)
- [ ] License code removed (fixed 2026-05-01)
- [ ] No orphaned references

**Status:** [ ] Pass [ ] Fail

---

## 📊 Test Summary

### Overall Results

**Total Tests:** _____
**Passed:** _____
**Failed:** _____
**Skipped:** _____

**Pass Rate:** _____%

### Critical Issues Found:
```
1. _______________________________________________________
2. _______________________________________________________
3. _______________________________________________________
```

### Minor Issues Found:
```
1. _______________________________________________________
2. _______________________________________________________
3. _______________________________________________________
```

### Recommendations:
```
1. _______________________________________________________
2. _______________________________________________________
3. _______________________________________________________
```

---

## ✅ Release Checklist

Before releasing to production:

- [ ] All critical tests pass
- [ ] No known critical bugs
- [ ] Documentation updated
- [ ] README.md accurate
- [ ] Example videos created
- [ ] Installation guide tested
- [ ] User guide reviewed
- [ ] License information correct
- [ ] Version number updated
- [ ] Release notes written

---

## 📝 Test Log Template

**Date:** _______________
**Tester:** _______________
**Version:** _______________
**Environment:** _______________

**Test Session Notes:**
```
_______________________________________________________
_______________________________________________________
_______________________________________________________
_______________________________________________________
_______________________________________________________
```

---

## 🎯 Automated Testing (Future)

### Unit Tests to Implement:
- [ ] Configuration loading/saving
- [ ] File name sanitization
- [ ] Script parsing
- [ ] Audio file handling
- [ ] Video assembly logic

### Integration Tests to Implement:
- [ ] AI provider connections
- [ ] FFmpeg integration
- [ ] Piper TTS integration
- [ ] File I/O operations

### Performance Tests to Implement:
- [ ] Memory leak detection
- [ ] CPU usage profiling
- [ ] GPU utilization monitoring
- [ ] Generation speed benchmarks

---

**This testing guide ensures every feature of Void Video Generator works correctly and provides a professional, bug-free experience for users.**
