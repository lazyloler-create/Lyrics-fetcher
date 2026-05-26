using System.Text.RegularExpressions;
using System.Http;

public class WordMeaningClient
{
    private readonly HttpClient _http = new HttpClient();
    string word = string.Empty;
    string url = $"https://freedictionaryapi.com/api/v1/entries/en/{word}";

    public async Task<string> GetWordMeaning(string word)
    {
        try {
            url = $"https://freedictionaryapi.com/api/v1/entries/en/{word}";
            string result = await _http.GetStringAsync(url);
            return result;
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Error fetching the meaning of {word}: {ex.Message}");
            return string.Empty;
        }
        catch(HttpRequestException ex)
        {
            Console.WriteLine($"HTTP request error while fetching the meaning of {word}: {ex.Message}");
            return string.Empty;
        }
    }
    string result = await GetWordMeaning(word);
    if(string.IsNullOrEmpty(result))
    {
        return $"Meaning for {word} failed to load!";
    }
    return result;
}