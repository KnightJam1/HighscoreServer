namespace HighscoreServer.Commands;

/// <summary>
/// Request for the server to shut down.
/// </summary>
public class ShutdownCommand : ICommand
{
    public string Name => "shutdown";
    public void Execute(CommandContext context, string[] args)
    {
        context.Server.Stop();
    }
}