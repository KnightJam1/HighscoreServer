using System.Collections.Concurrent;

namespace HighscoreServer.Loggers;

public class LoggerFile : LoggerBase
{
    private readonly ConcurrentQueue<string> _logQueue = new ConcurrentQueue<string>();
    private string _logFilePath = "";
    private bool _isSaving;
    
    public LoggerFile(string logFileDirectory)
    {
        Directory.CreateDirectory(logFileDirectory);
        SetNewLogFilePath(logFileDirectory);
        AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        Console.CancelKeyPress += OnCancelKeyPress;
    }
    
    private void SetNewLogFilePath(string directory)
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        _logFilePath = Path.Combine(directory, $"log_{timestamp}.txt");
    }
    
    public override void Log(string message, SeverityLevel severity = SeverityLevel.Info)
    {
        _logQueue.Enqueue($"[{DateTime.Now}] {severity}: {message}");
    }
    private async void OnProcessExit(object? sender, EventArgs e)
    {
        await SaveLogAsync("Process exiting normally.");
    }

    private async void OnUnhandledException(object? sender, UnhandledExceptionEventArgs e)
    {
        Log($"Unhandled exception: {e.ExceptionObject}");
        await SaveLogAsync("Process terminated due to unhandled exception.");
    }

    private async void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
    {
        Log("Process terminated via Ctrl+C.");
        await SaveLogAsync("Process terminated via Ctrl+C.");
        e.Cancel = true; // Prevent the process from terminating immediately
    }

    /// <summary>
    /// Save the 
    /// </summary>
    /// <param name="saveMessage"></param>
    public async Task SaveLogAsync(string saveMessage = "Saved log to file.")
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
                await writer.WriteLineAsync($"{DateTime.Now}: {saveMessage}");
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
