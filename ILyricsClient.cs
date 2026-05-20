using System.Threading.Tasks;

public interface ILyricsClient
{
    Task<string?> GetLyricsAsync(string artist, string title);
}