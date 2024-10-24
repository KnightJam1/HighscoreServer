using System.Collections.Concurrent;

namespace HighscoreServer.Loggers;

public class LoggerFile : LoggerBase
{
    private readonly ConcurrentQueue<string> _logQueue = new ConcurrentQueue<string>();
    private readonly string _logFilePath;
    private bool _isSaving;
    
    public LoggerFile(string logFilePath)
    {
        _logFilePath = logFilePath;
        AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
    }
    
    public override void Log(string message, SeverityLevel severity = SeverityLevel.Info)
    {
        _logQueue.Enqueue($"[{DateTime.Now}] {severity}: {message}");
    }
    private async void OnProcessExit(object? sender, EventArgs e)
    {
        await SaveLogAsync();
    }

    public async Task SaveLogAsync()
    {
        if (_isSaving) return;

        _isSaving = true;
        try
        {
            using (var writer = new StreamWriter(_logFilePath, true))
            {
                while (_logQueue.TryDequeue(out var logMessage))
                {
                    await writer.WriteLineAsync(logMessage);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving log: {ex.Message}");
        }
        finally
        {
            _isSaving = false;
        }
    }
}