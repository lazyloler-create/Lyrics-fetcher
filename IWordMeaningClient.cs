using System.Threading.Tasks;

public interface IWordMeaningClient
{
    Task<string> GetWordMeaning(string word);
}