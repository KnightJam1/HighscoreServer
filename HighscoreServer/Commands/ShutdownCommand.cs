namespace HighscoreListener.Commands;

/// <summary>
/// Request for the server to shut down.
/// </summary>
public class ShutdownCommand : ICommand
{
    public string Name => "shutdown";
    public void Execute(CommandContext context, string[] args)
    {
        Program.RequestShutdown();

        context.DataService.Save(context.DefaultFileName, context.Data);
    }
}