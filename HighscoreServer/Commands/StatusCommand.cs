namespace HighscoreListener.Commands;

/// <summary>
/// Return the status of the server.
/// </summary>
public class StatusCommand : ICommand
{
    public string Name => "status";
    public void Execute(CommandContext context, string[] args)
    {
        Console.WriteLine("The server is running.");
    }
}