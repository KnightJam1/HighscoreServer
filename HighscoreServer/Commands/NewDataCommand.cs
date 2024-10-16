namespace HighscoreServer.Commands;

/// <summary>
/// Creates new empty data.
/// </summary>
public class NewDataCommand : ICommand
{
    public string Name => "new";
    public int NumArgs => 0;
    public string HelpText => "Create a new set of leaderboards.";
    
    /// <exception cref="ArgumentException">Throws an exception if arguments are given.</exception>
    public void Execute(CommandContext context, string[] args)
    {
        // Check for a valid number of arguments.
        if (args.Length != NumArgs)
        {
            throw new ArgumentException($"Invalid number of arguments. Expected {NumArgs} but got {args.Length}.");
        }
        
        Console.WriteLine("Do you want to save current data before creating new data? (yes/no)");
        string response = Console.ReadLine() ?? "yes";
        if (response.Trim().ToLower() == "yes")
        {
            string saveName = "";
            while (saveName == "")
            {
                Console.WriteLine("What do you want to save your data as?");
                saveName = Console.ReadLine() ?? "";
            }
            context.Server.SaveData(saveName);
        }
        context.Server.NewData();
    }
}