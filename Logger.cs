using System;
using System.IO;

public class Logger : ILogger
{
    public void Log(string message)
    {
        string logFilePath = "LyricsFetcher.log";
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
        string logFilePath = "LyricsFetcher.log";
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