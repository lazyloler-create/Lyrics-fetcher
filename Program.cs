using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

//TODO: query history
//TODO: validate artist and track title
//TODO: cache lyrics for n hours to reduce api calls and speed up repeated lookups


//FINISH THE LOGGER IMPLEMENTATION AND INTEGRATE IT INTO THE PROGRAM

class Program
{
    ILogger logger = new ILogger(); 


    static string[] CleanTextLines(string lyrics)
    {
        if (lyrics.Contains("\\n") || lyrics.Contains("\\r\\n"))
        {
            lyrics = Regex.Unescape(lyrics);
        }

        lyrics = lyrics.Replace("\r\n", "\n").Replace("\r", "\n");
        return lyrics.Split(new[] { '\n' }, StringSplitOptions.None);
    }

    static void PrintLines(string[] lines)
    {
        foreach (var line in lines)
        {
            Console.WriteLine(line);
        }
    }

    static async Task Main()
    {
        Console.WriteLine("Enter an artist: ");
        string? artist = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(artist))
        {
            Console.WriteLine("Artist name is required!");
            logger.LogError("User didn't provide artist name", new ArgumentException("Artist name is required!");
            return;
        }

        Console.WriteLine("Enter a track: ");
        string? title = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(title))
        {
            Console.WriteLine("A track title is required!");
            logger.LogError("User did not provide a track title.", new ArgumentException("Track title is required!"));
            return;
        }

        // reuseable http client
        using var http = new HttpClient();
        ILyricsClient lyricsClient = new LyricsClient(http);

        Console.WriteLine($"Lyrics for {title} by {artist}: \n");

        try
        {
            string? lyrics = await lyricsClient.GetLyricsAsync(artist, title);

            if (lyrics is null)
            {
                Console.WriteLine("Lyrics not found.");
                logger.LogError($"Lyrics not found for {artist} - {title}", new Exception("Lyrics not found!"));
                return;
            }

            string[] lines = CleanTextLines(lyrics);
            PrintLines(lines);

            Console.WriteLine("\nWould you like to extract the lyrics to a plain text file? (y/n)");
            char choice = Console.ReadKey().KeyChar;
            if (choice == 'y' || choice == 'Y')
            {
                string fileName = $"{artist} - {title}.txt";
                File.WriteAllLines(fileName, lines);
                Console.WriteLine($"\nLyrics extracted to {fileName}");
                Console.WriteLine($"\nLyrics extracted to directory: {Directory.GetCurrentDirectory()}");
            }
            else
            {
                Console.WriteLine("\nLyrics not extracted");
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"\nRequest error: {ex.Message}");
        }
        catch (System.Text.Json.JsonException ex)
        {
            Console.WriteLine($"\nJSON parse error: {ex.Message}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"\nInput error: {ex.Message}");
        }
    }
}