using System.Text.RegularExpressions;
using System.Http;

public class WordClient
{
    static ILogger logger = new Logger();
    private readonly HttpClient _http = new HttpClient();
    private readonly HttpClient _randomWord = new HttpClient();
    string word = string.Empty;
    string url = string.Empty;

    Random rnd = new Random();
    int difficulty = rnd.Next(1, 6);

    public async Task<string> GetRandomWord(string word)
    {
        try {
            logger.LogInfo($"Trying to fetch a random word \n");
            url = $"https://random-word-api.herokuapp.com/word?diff={difficulty}";
            word = await _randomWord.GetStringAsync(url);

            return word;
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Error fetching random word: {ex.Message}");
            logger.LogError($"Failed to fetch a random word \n{ex.Message}");
            return string.Empty;
        }
        catch(HttpRequestException ex)
        {
            Console.WriteLine($"HTTP request error while fetching random word: {ex.Message}");
            logger.LogError($"HTTP request error while fetching a random word: {ex.Message}");
            return string.Empty;
        }
    }

    string result = await GetRandomWord(word);
    if(string.IsNullOrEmpty(result))
    {
        logger.LogWarning($"Failed to fetch a random word.\n");
        return "Word failed to load!";
    }
    logger.LogInfo($"Successfully fetched a random word: '{result}' \n");
    return result;
}