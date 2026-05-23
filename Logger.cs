public class Logger : ILogger
{
    private readonly string logFolderPath;
    private readonly string logFilePath;

    public Logger()
    {
        logFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
        Directory.CreateDirectory(logFolderPath); // ensure folder exists
        logFilePath = Path.Combine(logFolderPath, "LyricsFetcher.log");
    }

    public void Log(string message)
    {
        string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
        try
        {
            File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to write to log file: {ex.Message}");
        }
    }

    public void LogError(string message, Exception ex)
    {
        string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - ERROR: {message} - Exception: {ex.Message}";
        try
        {
            File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
        }
        catch (Exception innerEx)
        {
            Console.WriteLine($"Failed to write to log file: {innerEx.Message}");
        }
    }

    public void LogInfo(string message)
    {
        Log($"INFO: {message}");
    }

    public void LogWarning(string message, Exception ex)
    {
        Log($"WARNING: {message} - Exception: {ex.Message}");
    }
}
