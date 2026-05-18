using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.RegularExpressions;

//TODO: prompt user to extract lyrics to plain text
//TODO: query history
//TODO: logger
//TODO: validate artist and track title
//TODO: cache lyrics for n hours to reduce api calls and speed up repeated lookups

class Program
{
    static async Task Main()
    {
        using HttpClient client = new HttpClient();
        Console.WriteLine("Enter an artist: ");
        string? artist = Console.ReadLine();

        Console.WriteLine("Enter a track: ");
        string? title = Console.ReadLine();
        string url = $"https://api.lyrics.ovh/v1/{artist}/{title}";

        if(string.IsNullOrWhiteSpace(artist) || string.IsNullOrWhiteSpace(title))
        {
            Console.WriteLine("An artist or track title is required!");
            return;
        }

        Console.WriteLine($"Lyrics for {title} by {artist}: \n");
        try
        {
            HttpResponseMessage response = await client.GetAsync(url);
            string result = await response.Content.ReadAsStringAsync();
            using JsonDocument doc = JsonDocument.Parse(result);
            if (!doc.RootElement.TryGetProperty("lyrics", out JsonElement lyricsElement)) //tries to load lyrics from JsonDocument
            {
                Console.WriteLine("Lyrics not found in response");
                return;
            }
            string lyrics = lyricsElement.GetString() ?? string.Empty;

            if(lyrics.Contains("\\n") || lyrics.Contains("\\r\\n"))
            {
                lyrics = Regex.Unescape(lyrics);
            }
            
            lyrics = lyrics.Replace("\r\n", "\n").Replace("\r", "\n");
            string[] lines = lyrics.Split(new[] { '\n' }, StringSplitOptions.None);

            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"JSON parse error: {ex.Message}");
        }
    }
}