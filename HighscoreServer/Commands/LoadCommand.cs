using HighscoreServer.Loggers;

namespace HighscoreServer.Commands;

/// <summary>
/// Load new data. Asks user if they wish to save the current data before loading the new data.
/// </summary>
public class LoadCommand : ICommand
{
    public string Name => "load";
    public int NumArgs => 1;
    public string HelpText => "Loads a set of leaderboards. Requires the name of the saved data, with or without an extension. E.g.: Load savedLeaderboards.";

    /// <param name="args">Name of the data to be loaded.
    /// If using a filesystem it doesn't matter if the filename has an extension or not.</param>
    public void Execute(CommandContext context, string[] args)
    {
        // Check for a valid number of arguments.
        if (args.Length != NumArgs)
        {
            throw new ArgumentException($"Invalid number of arguments. Expected {NumArgs} but got {args.Length}.");
        }
        
        Console.WriteLine("Do you want to save current data before loading new data? (yes/no)");
        string response = Console.ReadLine() ?? "yes";
        if (response.Trim().ToLower() == "yes")
        {
            context.Server.SaveData(); // Add functionality to recieve a response from the server about success
        }
        // Load new data. Only assign data if the new data is not null.
        context.Server.LoadData(args[0]);
    }
}