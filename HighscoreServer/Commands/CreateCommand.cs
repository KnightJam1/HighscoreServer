

namespace HighscoreServer.Commands;

/// <summary>
/// Command to create a new leaderboard to be added to a game.
/// </summary>
public class CreateCommand : ICommand
{
    public string Name => "create";
    public int NumArgs => 3;
    public string HelpText => "Create a new leaderboard. Requires name, items in entry, max entries. E.g. create gamemode1 3 1000.";
    
    /// <param name="args">Arguments should come in the form name, items in entry, max entries. E.g. 'gamemode1 3 1000'</param>
    public void Execute(CommandContext context, string[] args)
    {
        // Check for a valid number of arguments.
        if (args.Length != NumArgs)
        {
            throw new ArgumentException($"Invalid number of arguments. Expected {NumArgs} but got {args.Length}.");
        }
        
        // Check to see if args[0] is valid.
        if (args[0] == "")
        {
            throw new ArgumentException("You must provide a name!");
        }
        
        // Check to see if args[1] is valid.
        try
        {
            int.Parse(args[1]);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw new ArgumentException("Number of items for an entry must be an integer.");
        }
        if (int.Parse(args[1]) < 1)
        {
            throw new ArgumentException("Cannot have fewer than one items in an entry.");
        }
        
        // Check to see if args[2] is valid.
        try
        {
            int.Parse(args[2]);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw new ArgumentException("Maximum number of entries must be an integer.");
        }
        if (int.Parse(args[2]) < 1)
        {
            throw new ArgumentException("Cannot have fewer than one entry in the leaderboard.");
        }

        List<string> format = new List<string>();
        List<string> dataTypeNames = new List<string>();

        for (int i = 0; i < Convert.ToInt16(args[1]); i++)
        {
            if (i == 0)
            {
                Console.WriteLine($"Enter type for item 1 (int, string, datetime). This will be the value the highscores are ordered by.:");
            }
            Console.WriteLine($"Enter type for item {i + 1} (int, string, datetime):");
            string typeInput = Console.ReadLine() ?? "";

            switch (typeInput.ToLower())
            {
                case "string":
                case "int":
                case "datetime":
                    dataTypeNames.Add(typeInput.ToLower());
                    break;
                default:
                    Console.WriteLine($"Unknown type '{typeInput}'. Supported types are: string, int, datetime.");
                    return;
            }

            Console.WriteLine($"Enter name for item {i + 1}:");
            string nameInput = Console.ReadLine() ?? "";
            format.Add(nameInput);
        }

        context.Server.AddLeaderboard(args[0], format, dataTypeNames,int.Parse(args[2]));
        Console.WriteLine($"Leaderboard '{args[0]}' created with format: {string.Join(", ", format)}");
    }
}