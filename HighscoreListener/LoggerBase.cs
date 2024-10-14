
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
        currentSeverity = severity;
    }
    
    public LoggerBase(SeverityLevel severity = SeverityLevel.INFO)
    {
        currentSeverity = severity;
    }
}