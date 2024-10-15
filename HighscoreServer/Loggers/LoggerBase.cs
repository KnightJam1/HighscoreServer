
using System.Diagnostics;

namespace HighscoreServer.Loggers;

/// <summary>
/// Base abstract class for loggers. Use to create logs of important info.
/// Has three levels of severity, Info, Warning and Error.
/// </summary>
public abstract class LoggerBase
{
    public enum SeverityLevel
    {
        Info, Warning, Error
    }
    protected readonly string[] SeverityEmojis = ["💬", "⚠", "🛑"];
    protected SeverityLevel CurrentSeverity;
    public abstract void Log(string message, SeverityLevel severity);

    public void SetSeverityLevel(SeverityLevel severity)
    {
        Debug.Assert(SeverityEmojis.Length == Enum.GetNames(typeof(SeverityLevel)).Length, "Mismatch in number of severity levels and emojis.");
        CurrentSeverity = severity;
    }
    
    public LoggerBase(SeverityLevel severity = SeverityLevel.Info)
    {
        CurrentSeverity = severity;
    }
}