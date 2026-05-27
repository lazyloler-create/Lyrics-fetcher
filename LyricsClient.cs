using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class LyricsClient : ILyricsClient
{
    static ILogger logger = new Logger();
    private readonly HttpClient _http;

    public LyricsClient(HttpClient http)
    {
        _http = http ?? throw new ArgumentNullException(nameof(http));
        logger.LogInfo("LyricsClient initialized.");
    }

    public async Task<string?> GetLyricsAsync(string artist, string title)
    {
        if (string.IsNullOrWhiteSpace(artist)) {
            logger.LogError("Artist is required but was null or whitespace.\n");
            throw new ArgumentException("artist is required", nameof(artist));
            return null;
        }
        if (string.IsNullOrWhiteSpace(title)) {
            logger.LogError("Title is required but was null or whitespace.\n");
            throw new ArgumentException("title is required", nameof(title));
            return null;
        }

        string url = $"https://api.lyrics.ovh/v1/{Uri.EscapeDataString(artist)}/{Uri.EscapeDataString(title)}";
        using HttpResponseMessage response = await _http.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound) {
                logger.LogWarning($"Lyrics not found for '{artist} - {title}'\n"); 
                return null;
            }
            response.EnsureSuccessStatusCode();
            logger.LogInfo($"Success, code: {response.StatusCode}");
        }

        string result = await response.Content.ReadAsStringAsync();
        using JsonDocument doc = JsonDocument.Parse(result);

        if (!doc.RootElement.TryGetProperty("lyrics", out JsonElement lyricsElement))
        {
            logger.LogError($"Lyrics property not found for '{artist} - {title}'\n");
            return null;
        }
        logger.LogInfo($"Successfully fetched lyrics for '{artist} - {title}'\n");
        return lyricsElement.GetString() ?? string.Empty;
    }
}