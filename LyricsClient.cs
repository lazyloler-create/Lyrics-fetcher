using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class LyricsClient : ILyricsClient
{
    private readonly HttpClient _http;

    public LyricsClient(HttpClient http)
    {
        _http = http ?? throw new ArgumentNullException(nameof(http));
    }

    public async Task<string?> GetLyricsAsync(string artist, string title)
    {
        if (string.IsNullOrWhiteSpace(artist)) throw new ArgumentException("artist is required", nameof(artist));
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("title is required", nameof(title));

        string url = $"https://api.lyrics.ovh/v1/{Uri.EscapeDataString(artist)}/{Uri.EscapeDataString(title)}";
        using HttpResponseMessage response = await _http.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
            response.EnsureSuccessStatusCode();
        }

        string result = await response.Content.ReadAsStringAsync();
        using JsonDocument doc = JsonDocument.Parse(result);

        if (!doc.RootElement.TryGetProperty("lyrics", out JsonElement lyricsElement))
        {
            return null;
        }

        return lyricsElement.GetString() ?? string.Empty;
    }
}