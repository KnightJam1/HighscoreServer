
using System.Diagnostics;

namespace HighscoreListener.Loggers;

public abstract class LoggerBase
{
    public enum SeverityLevel
    {
        Info, Warning, Error
    }

    protected readonly Dictionary<SeverityLevel, string> SeverityEmojis = new Dictionary<SeverityLevel, string>()
    {
        {SeverityLevel.Info,"💬"},
        {SeverityLevel.Warning,"⚠"},
        {SeverityLevel.Error,"🛑"}
    };
    protected SeverityLevel CurrentSeverity;
    public abstract void Log(string message, SeverityLevel severity);

    public void SetSeverityLevel(SeverityLevel severity)
    {
        Debug.Assert(SeverityEmojis.Count == Enum.GetNames(typeof(SeverityLevel)).Length, "Mismatch in number of severity levels and emojis.");
        CurrentSeverity = severity;
    }
    
    public LoggerBase(SeverityLevel severity = SeverityLevel.Info)
    {
        CurrentSeverity = severity;
    }
}