namespace HighscoreListener.Commands;

public class StatusCommand : ICommand
{
    public string Name => "status";
    public void Execute(CommandContext context, string[] args)
    {
        Console.WriteLine("The server is running.");
    }
}