using System;
using System.Net.Http;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        using HttpClient client = new HttpClient();
        Console.WriteLine("Enter an artist: ");
        string? artist = Console.ReadLine();
        string? title = Console.ReadLine();
        string url = $"https://api.lyrics.ovh/v1/{artist}/{title}";

        HttpResponseMessage response = await client.GetAsync(url);
        string result = await response.Content.ReadAsStringAsync();

        Console.WriteLine(result);
    }
}