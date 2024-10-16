namespace HighscoreServer.Commands;

/// <summary>
/// Return the status of the server.
/// </summary>
public class StatusCommand : ICommand
{
    public string Name => "status";
    public int NumArgs => 0;
    public string HelpText => "Returns the status of the server.";
    public void Execute(CommandContext context, string[] args)
    {
        // Check for a valid number of arguments.
        if (args.Length != NumArgs)
        {
            throw new ArgumentException($"Invalid number of arguments. Expected {NumArgs} but got {args.Length}.");
        }
        
        Console.WriteLine("The server is running.");
    }
}