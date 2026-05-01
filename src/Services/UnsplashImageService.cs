namespace VoidVideoGenerator.Services;

using System.Text.Json;
using VoidVideoGenerator.Models;

/// <summary>
/// Service to download images from Unsplash API based on search queries
/// </summary>
public class UnsplashImageService
{
    private readonly HttpClient _httpClient;
    private readonly string _accessKey;
    private const string BaseUrl = "https://api.unsplash.com";

    public UnsplashImageService(string accessKey = "")
    {
        _accessKey = accessKey;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("Accept-Version", "v1");
        if (!string.IsNullOrEmpty(_accessKey))
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Client-ID {_accessKey}");
        }
    }

    /// <summary>
    /// Check if Unsplash API is configured and available
    /// </summary>
    public async Task<bool> IsAvailableAsync()
    {
        if (string.IsNullOrEmpty(_accessKey))
        {
            return false;
        }

        try
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/photos/random?count=1");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Download images based on visual cues from the script
    /// </summary>
    public async Task<List<string>> DownloadImagesFromCuesAsync(
        List<string> visualCues, 
        string outputDirectory, 
        int imagesPerCue = 1,
        IProgress<string>? progress = null)
    {
        if (string.IsNullOrEmpty(_accessKey))
        {
            throw new Exception("Unsplash API key not configured. Please add your API key in Settings.");
        }

        Directory.CreateDirectory(outputDirectory);
        var downloadedImages = new List<string>();
        int imageIndex = 0;

        progress?.Report($"Downloading images from Unsplash for {visualCues.Count} visual cues...");

        foreach (var cue in visualCues)
        {
            try
            {
                // Clean up the cue for search
                var searchQuery = CleanSearchQuery(cue);
                progress?.Report($"Searching Unsplash for: {searchQuery}");

                var images = await SearchImagesAsync(searchQuery, imagesPerCue);
                
                foreach (var imageUrl in images)
                {
                    var imagePath = Path.Combine(outputDirectory, $"unsplash_{imageIndex:D3}.jpg");
                    await DownloadImageAsync(imageUrl, imagePath);
                    downloadedImages.Add(imagePath);
                    imageIndex++;
                    progress?.Report($"Downloaded image {imageIndex}: {Path.GetFileName(imagePath)}");
                }
            }
            catch (Exception ex)
            {
                progress?.Report($"Warning: Failed to download image for '{cue}': {ex.Message}");
            }
        }

        progress?.Report($"✓ Downloaded {downloadedImages.Count} images from Unsplash");
        return downloadedImages;
    }

    /// <summary>
    /// Search for images on Unsplash
    /// </summary>
    private async Task<List<string>> SearchImagesAsync(string query, int count = 1)
    {
        var imageUrls = new List<string>();

        try
        {
            var url = $"{BaseUrl}/search/photos?query={Uri.EscapeDataString(query)}&per_page={count}&orientation=landscape";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<UnsplashSearchResponse>(json, options);

            if (result?.Results != null)
            {
                foreach (var photo in result.Results.Take(count))
                {
                    // Use regular quality for faster downloads
                    var imageUrl = photo.Urls?.Regular ?? photo.Urls?.Full ?? "";
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        imageUrls.Add(imageUrl);
                    }
                }
            }

            // If no results, try a random image
            if (imageUrls.Count == 0)
            {
                var randomUrl = await GetRandomImageAsync();
                if (!string.IsNullOrEmpty(randomUrl))
                {
                    imageUrls.Add(randomUrl);
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to search Unsplash: {ex.Message}");
        }

        return imageUrls;
    }

    /// <summary>
    /// Get a random image from Unsplash
    /// </summary>
    private async Task<string> GetRandomImageAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/photos/random?orientation=landscape");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var photo = JsonSerializer.Deserialize<UnsplashPhoto>(json, options);

            return photo?.Urls?.Regular ?? "";
        }
        catch
        {
            return "";
        }
    }

    /// <summary>
    /// Download an image from URL to local file
    /// </summary>
    private async Task DownloadImageAsync(string imageUrl, string outputPath)
    {
        if (string.IsNullOrEmpty(imageUrl))
        {
            throw new Exception("Invalid image URL");
        }

        var imageData = await _httpClient.GetByteArrayAsync(imageUrl);
        await File.WriteAllBytesAsync(outputPath, imageData);
    }

    /// <summary>
    /// Clean up visual cue text for better search results
    /// </summary>
    private string CleanSearchQuery(string cue)
    {
        // Remove common filler words and clean up the query
        var query = cue.Trim()
            .Replace("show ", "", StringComparison.OrdinalIgnoreCase)
            .Replace("display ", "", StringComparison.OrdinalIgnoreCase)
            .Replace("image of ", "", StringComparison.OrdinalIgnoreCase)
            .Replace("picture of ", "", StringComparison.OrdinalIgnoreCase)
            .Replace("video of ", "", StringComparison.OrdinalIgnoreCase);

        // Limit length for API
        if (query.Length > 50)
        {
            query = query.Substring(0, 50);
        }

        return query;
    }

    // JSON response models
    private class UnsplashSearchResponse
    {
        public List<UnsplashPhoto>? Results { get; set; }
    }

    private class UnsplashPhoto
    {
        public string? Id { get; set; }
        public UnsplashUrls? Urls { get; set; }
    }

    private class UnsplashUrls
    {
        public string? Raw { get; set; }
        public string? Full { get; set; }
        public string? Regular { get; set; }
        public string? Small { get; set; }
        public string? Thumb { get; set; }
    }
}
