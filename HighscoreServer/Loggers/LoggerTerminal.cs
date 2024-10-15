
namespace HighscoreListener.Loggers;

public class LoggerTerminal:LoggerBase
{
    public override void Log(string message, SeverityLevel severity = SeverityLevel.Info)
    {
        if (severity >= CurrentSeverity)
        {
            Console.WriteLine($"{SeverityEmojis[severity]}: {message}");
        }
    }
}