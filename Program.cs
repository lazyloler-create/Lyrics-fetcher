using System.Text.Json;
using System.Text.RegularExpressions;
using System.IO;

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

    static void queries(string qDate, string artist, string title)
    {
        logger.LogInfo("Saving user query");
        string fileName = "queries.txt";
        File.AppendAllText(fileName, $"{qDate + '\n'} {artist} - {title}{Environment.NewLine}");
        logger.LogInfo("Saved user query to queries.txt \n");
        logger.LogInfo($"queries.txt file path: {Directory.GetCurrentDirectory()}\\{fileName} \n");
    }

    public class LyricsEntry
    {
        public string Artist { get; set; } = "";
        public string Title { get; set; } = "";
        public string[] Lyrics { get; set; } = Array.Empty<string>();
    }

    public class LyricsCache
    {
        private readonly string cacheFolder = "cache";

        public LyricsCache()
        {
            if (!Directory.Exists(cacheFolder))
            {
                Directory.CreateDirectory(cacheFolder);
            }
        }
        public void SaveLyrics(string[] lyrics, string artist, string title)
        {
        var obj = new LyricsEntry
        {
            Artist = artist,
            Title = title,
            Lyrics = lyrics
        };
        var options = new JsonSerializerOptions { WriteIndented = true };
        var jsonString = JsonSerializer.Serialize(obj, options);
        string file = $"{artist}-{title}".Replace(" ", "_") + ".json";
        string path = Path.Combine(cacheFolder, file);
        File.WriteAllText(path, jsonString);
        logger.LogInfo($"Lyrics saved to {file}.json in {Directory.GetCurrentDirectory}\\{file} \n");
        }

        public string[]? LoadLyrics(string artist, string title)
        {
            string file = $"{artist}-{title}".Replace(" ", "_") + ".json";
            string path = Path.Combine(cacheFolder, file);

            if (File.Exists(path))
            {
                var obj = JsonSerializer.Deserialize<LyricsEntry>(File.ReadAllText(path));
                return obj?.Lyrics;
            }
            return null;
        }
    }

    private async Task<string> WordOfTheProgram(HttpClient http)
    {
        try {
            IWordClient randomWord = new WordClient(http);
            IWordMeaningClient wordMeaningClient = new WordMeaningClient(http);
            string meaning = await wordMeaningClient.GetWordMeaning("");
            string word = await randomWord.GetRandomWord("");
            Console.WriteLine($"{word} - {meaning}");
        }
        catch (Exception ex) {
            Console.WriteLine($"Error fetching word of the day: {ex.Message}");
        }
        catch(HttpRequestException ex)
        {
            Console.WriteLine($"HTTP request error while fetching word of the day: {ex.Message}");
        }
    }

    static async Task Main()
    {
        Console.WriteLine("Enter an artist: ");
        string? artist = Console.ReadLine();

        Console.WriteLine("Enter a track: ");
        string? title = Console.ReadLine();

        DateTime dateTime = DateTime.Now;
        string formattedDateTime = dateTime.ToString("dd-MM-yyyy HH:mm:ss");
        queries(formattedDateTime, artist, title);

        // reuseable http client
        using var http = new HttpClient();
        ILyricsClient lyricsClient = new LyricsClient(http);

        Console.WriteLine("Random word of the day: ");
        await WordOfTheProgram(http);

        Console.WriteLine($"Lyrics for {title} by {artist}: \n");
        logger.LogInfo($"Fetched lyrics for {title} by {artist} \n");

        try
        {
            var cache = new LyricsCache();
            var cachedLyrics = cache.LoadLyrics(artist, title);
            
            if (cachedLyrics != null)
            {
                Console.WriteLine("Loaded lyrics from cache:");
                foreach (var line in cachedLyrics)
                Console.WriteLine(line);
                return;
            }

            string? lyrics = await lyricsClient.GetLyricsAsync(artist, title);

            if (lyrics is null)
            {
                Console.WriteLine("Lyrics not found.");
                logger.LogError($"Lyrics not found for {artist} - {title} \n", new Exception("Lyrics not found! \n"));
                return;
            }

            string[] lines = CleanTextLines(lyrics);

            
            cache.SaveLyrics(lines, artist, title);
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
        catch (HttpProtocolException)
        {
            Console.WriteLine("Http protocol failed while fetching lyrics.");
            logger.LogError("Http protocol failed while fetching lyrics \n", new HttpProtocolException("Http protocol failed while fetching lyrics \n"));
        }
    }
    return;
}