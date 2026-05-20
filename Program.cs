using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

//TODO: query history
//TODO: cache lyrics for n hours to reduce api calls and speed up repeated lookups


class Program
{
    static ILogger logger = new Logger();

    static string[] CleanTextLines(string lyrics)
    {
        if (lyrics.Contains("\\n") || lyrics.Contains("\\r\\n"))
        {
            lyrics = Regex.Unescape(lyrics);
        }

        lyrics = lyrics.Replace("\r\n", "\n").Replace("\r", "\n");
        logger.LogInfo("Text cleaned from newline charactes and split into lines \n");
        return lyrics.Split(new[] { '\n' }, StringSplitOptions.None);
    }

    static void PrintLines(string[] lines)
    {
        foreach (var line in lines)
        {
            logger.LogInfo("Printing lyrics...\n");
            Console.WriteLine(line);
        }
        logger.LogInfo("Program finished printing lyrics \n");
    }

    static void ExtractLyricsToFile(string artist, string title, string[] lines)
    {

        logger.LogInfo("User chose to extract lyrics \n");
        string fileName = $"{artist} - {title}.txt";
        File.WriteAllLines(fileName, lines);
        Console.WriteLine($"\nLyrics extracted to {fileName}");
        Console.WriteLine($"\nLyrics extracted to directory: {Directory.GetCurrentDirectory()}\\{fileName}");
        logger.LogInfo($"Lyrics extraced to {Directory.GetCurrentDirectory()}\\{fileName}");
    }

    static void queries(string query, string artist, string title)
    {
        logger.LogInfo("Saving user query");
        string queryName = query;
        string fileName = "queries.txt";
        File.AppendAllText(fileName, $"{queryName + '\n'} {artist} - {title}{Environment.NewLine}");
        logger.LogInfo("Saved user query to queries.txt \n");
        logger.LogInfo($"queries.txt file path: {Directory.GetCurrentDirectory()}\\{fileName} \n");
    }

    static async Task Main()
    {
        Console.WriteLine("Enter an artist: ");
        string? artist = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(artist))
        {
            Console.WriteLine("Artist name is required!");
            logger.LogError("User didn't provide artist name", new ArgumentException("Artist name is required!"));
            return;
        }

        Console.WriteLine("Enter a track: ");
        string? title = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(title))
        {
            Console.WriteLine("A track title is required!");
            logger.LogError("User did not provide a track title. \n", new ArgumentException("Track title is required!"));
            return;
        }

        DateTime dateTime = DateTime.Now;
        string formattedDateTime = dateTime.ToString("dd-MM-yyyy HH:mm:ss");
        queries(formattedDateTime, artist, title);

        // reuseable http client
        using var http = new HttpClient();
        ILyricsClient lyricsClient = new LyricsClient(http);

        Console.WriteLine($"Lyrics for {title} by {artist}: \n");
        logger.LogInfo($"Fetched lyrics for {title} by {artist} \n");

        try
        {
            string? lyrics = await lyricsClient.GetLyricsAsync(artist, title);

            if (lyrics is null)
            {
                Console.WriteLine("Lyrics not found.");
                logger.LogError($"Lyrics not found for {artist} - {title} \n", new Exception("Lyrics not found! \n"));
                return;
            }

            string[] lines = CleanTextLines(lyrics);
            PrintLines(lines);

            Console.WriteLine("\nWould you like to extract the lyrics to a plain text file? (y/n)");
            logger.LogInfo("Prompted user to extract lyrics to a plain text file \n"); 

            char choice = Console.ReadKey().KeyChar;
            if (choice == 'y' || choice == 'Y')
            {
                ExtractLyricsToFile(artist, title, lines);
            }
            else
            {
                Console.WriteLine("\nLyrics not extracted");
                logger.LogInfo("User chose not to extract lyrics \n");
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"\nRequest error: {ex.Message}");
            logger.LogError("HTTP request error while fetching lyrics \n", ex);
        }
        catch (System.Text.Json.JsonException ex)
        {
            Console.WriteLine($"\nJSON parse error: {ex.Message}");
            logger.LogError("JSON parse error while fetching lyrics \n", ex);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"\nInput error: {ex.Message}");
            logger.LogError("Input error while fetching lyrics \n", ex);
        }
    }
}