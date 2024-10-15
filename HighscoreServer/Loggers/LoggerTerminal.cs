
namespace HighscoreListener.Loggers;

/// <summary>
/// A logger that outputs logs to the terminal.
/// </summary>
public class LoggerTerminal:LoggerBase
{
    public override void Log(string message, SeverityLevel severity = SeverityLevel.Info)
    {
        if (severity >= CurrentSeverity)
        {
            Console.WriteLine($"{SeverityEmojis[(int)severity]}: {message}");
        }
    }
}