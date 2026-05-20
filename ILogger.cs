using System.IO;

public interface ILogger
{
    void Log(string message);
    void LogError(string message, Exception ex);
    void LogInfo(string message);
    void LogWarning(string message);
}