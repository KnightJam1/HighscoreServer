
namespace HighscoreListener.Loggers;

public class LoggerTerminal:LoggerBase
{
    public override void Log(string message, SeverityLevel severity = SeverityLevel.INFO)
    {
        if (severity >= currentSeverity)
        {
            Console.WriteLine($"{severityEmojis[severity]}: {message}");
        }
    }
}