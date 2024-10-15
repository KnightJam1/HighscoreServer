
using System.Diagnostics;

namespace HighscoreListener;

public abstract class LoggerBase
{
    public enum SeverityLevel
    {
        INFO, WARNING, ERROR
    }
    public Dictionary<SeverityLevel, string> severityEmojis = new Dictionary<SeverityLevel, string>()
    {
        {SeverityLevel.INFO,"💬"},
        {SeverityLevel.WARNING,"⚠"},
        {SeverityLevel.ERROR,"🛑"}
    };
    protected SeverityLevel currentSeverity;
    public abstract void Log(string message, SeverityLevel severity);

    public void SetSeverityLevel(SeverityLevel severity)
    {
        Debug.Assert(severityEmojis.Count == Enum.GetNames(typeof(SeverityLevel)).Length, "Mismatch in number of severity levels and emojis.");
        currentSeverity = severity;
    }
    
    public LoggerBase(SeverityLevel severity = SeverityLevel.INFO)
    {
        currentSeverity = severity;
    }
}