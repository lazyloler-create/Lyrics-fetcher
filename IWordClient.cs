using System.Threading.Tasks;

public interface IWordClient
{
    public Task<string> GetRandomWord(string word);
}