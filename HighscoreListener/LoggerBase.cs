
public abstract class LoggerBase
{
    public enum SeverityLevel
    {
        INFO, WARNING, ERROR
    }
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