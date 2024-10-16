namespace HighscoreServer.Commands;

/// <summary>
/// Request for the server to shut down.
/// </summary>
public class ShutdownCommand : ICommand
{
    public string Name => "shutdown";
    public int NumArgs => 0;
    public string HelpText => "Requests the server shutdown.";
    public void Execute(CommandContext context, string[] args)
    {
        // Check for a valid number of arguments.
        if (args.Length != NumArgs)
        {
            throw new ArgumentException($"Invalid number of arguments. Expected {NumArgs} but got {args.Length}.");
        }
        
        context.Server.Stop();
    }
}