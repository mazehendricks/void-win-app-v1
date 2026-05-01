namespace VoidVideoGenerator.Services;

using VoidVideoGenerator.Models;

/// <summary>
/// Factory for creating AI video generator service instances
/// </summary>
public class AIVideoGeneratorFactory
{
    private readonly AIVideoConfig _config;

    public AIVideoGeneratorFactory(AIVideoConfig config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    /// <summary>
    /// Create an AI video generator based on the configured provider
    /// </summary>
    public IAIVideoGeneratorService CreateGenerator()
    {
        return _config.Provider.ToLowerInvariant() switch
        {
            "runwayml" => CreateRunwayMLGenerator(),
            "lumaai" => CreateLumaAIGenerator(),
            "animatediff" => CreateAnimateDiffGenerator(),
            "hybrid" => CreateHybridGenerator(),
            "none" => throw new InvalidOperationException("AI video generation is not enabled. Please select a provider in Settings."),
            _ => throw new NotSupportedException($"AI video provider '{_config.Provider}' is not supported")
        };
    }

    /// <summary>
    /// Create a specific generator by provider name
    /// </summary>
    public IAIVideoGeneratorService CreateGenerator(string providerName)
    {
        return providerName.ToLowerInvariant() switch
        {
            "runwayml" => CreateRunwayMLGenerator(),
            "lumaai" => CreateLumaAIGenerator(),
            "animatediff" => CreateAnimateDiffGenerator(),
            "hybrid" => CreateHybridGenerator(),
            _ => throw new NotSupportedException($"AI video provider '{providerName}' is not supported")
        };
    }

    /// <summary>
    /// Get all available providers
    /// </summary>
    public async Task<List<ProviderInfo>> GetAvailableProvidersAsync()
    {
        var providers = new List<ProviderInfo>();

        // Runway ML
        var runwayML = new ProviderInfo
        {
            Name = "RunwayML",
            DisplayName = "Runway ML Gen-3",
            Description = "Cloud-based, highest quality, supports image-to-video",
            Type = "Cloud",
            Cost = "$3.00 per minute",
            MaxDuration = 10,
            IsConfigured = !string.IsNullOrWhiteSpace(_config.RunwayML?.ApiKey)
        };
        
        if (runwayML.IsConfigured)
        {
            try
            {
                var service = CreateRunwayMLGenerator();
                runwayML.IsAvailable = await service.IsAvailableAsync();
            }
            catch
            {
                runwayML.IsAvailable = false;
            }
        }
        providers.Add(runwayML);

        // Luma AI
        var lumaAI = new ProviderInfo
        {
            Name = "LumaAI",
            DisplayName = "Luma AI Dream Machine",
            Description = "Cloud-based, excellent quality, camera controls",
            Type = "Cloud",
            Cost = "$3.60 per minute",
            MaxDuration = 5,
            IsConfigured = !string.IsNullOrWhiteSpace(_config.LumaAI?.ApiKey)
        };
        
        if (lumaAI.IsConfigured)
        {
            try
            {
                var service = CreateLumaAIGenerator();
                lumaAI.IsAvailable = await service.IsAvailableAsync();
            }
            catch
            {
                lumaAI.IsAvailable = false;
            }
        }
        providers.Add(lumaAI);

        // AnimateDiff
        var animateDiff = new ProviderInfo
        {
            Name = "AnimateDiff",
            DisplayName = "AnimateDiff (Local)",
            Description = "Local generation via ComfyUI, free but requires GPU",
            Type = "Local",
            Cost = "Free (requires RTX 3060+)",
            MaxDuration = 10,
            IsConfigured = !string.IsNullOrWhiteSpace(_config.AnimateDiff?.ComfyUIEndpoint)
        };
        
        if (animateDiff.IsConfigured)
        {
            try
            {
                var service = CreateAnimateDiffGenerator();
                animateDiff.IsAvailable = await service.IsAvailableAsync();
            }
            catch
            {
                animateDiff.IsAvailable = false;
            }
        }
        providers.Add(animateDiff);

        // Hybrid
        var hybrid = new ProviderInfo
        {
            Name = "Hybrid",
            DisplayName = "Hybrid (Keyframes + Interpolation)",
            Description = "Budget option using keyframe generation and interpolation",
            Type = "Local",
            Cost = "Free (requires FFmpeg)",
            MaxDuration = 30,
            IsConfigured = true
        };
        
        try
        {
            var service = CreateHybridGenerator();
            hybrid.IsAvailable = await service.IsAvailableAsync();
        }
        catch
        {
            hybrid.IsAvailable = false;
        }
        providers.Add(hybrid);

        return providers;
    }

    private RunwayMLVideoService CreateRunwayMLGenerator()
    {
        if (_config.RunwayML == null)
            throw new InvalidOperationException("Runway ML is not configured");
        
        return new RunwayMLVideoService(_config.RunwayML);
    }

    private LumaAIVideoService CreateLumaAIGenerator()
    {
        if (_config.LumaAI == null)
            throw new InvalidOperationException("Luma AI is not configured");
        
        return new LumaAIVideoService(_config.LumaAI);
    }

    private AnimateDiffVideoService CreateAnimateDiffGenerator()
    {
        if (_config.AnimateDiff == null)
            throw new InvalidOperationException("AnimateDiff is not configured");
        
        return new AnimateDiffVideoService(_config.AnimateDiff);
    }

    private HybridVideoService CreateHybridGenerator()
    {
        if (_config.Hybrid == null)
            _config.Hybrid = new HybridConfig();
        
        return new HybridVideoService(_config.Hybrid);
    }
}

/// <summary>
/// Information about an AI video provider
/// </summary>
public class ProviderInfo
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "Cloud" or "Local"
    public string Cost { get; set; } = string.Empty;
    public int MaxDuration { get; set; }
    public bool IsConfigured { get; set; }
    public bool IsAvailable { get; set; }
}
