

namespace HighscoreServer.Commands;

/// <summary>
/// Command to create a new leaderboard to be added to a game.
/// </summary>
public class CreateCommand : ICommand
{
    public string Name => "create";
    public int NumArgs => 4;
    public string HelpText => "Create a new leaderboard. Every leaderboad has an int score and string name. Requires a name, a number of extra details and a maximum number entries. E.g. create gamemode1 5 3 1000.";
    
    /// <param name="args">Arguments should come in the form name, max username length, extra details, max entries. E.g. 'gamemode1 5 3 1000'</param>
    public void Execute(CommandContext context, string[] args)
    {
        // Check for a valid number of arguments.
        if (args.Length != NumArgs)
        {
            throw new ArgumentException($"Invalid number of arguments. Expected {NumArgs} but got {args.Length}.");
        }
        
        // Check to see if args[0], the leaderboard name, is valid.
        if (args[0] == "")
        {
            throw new ArgumentException("You must provide a name!");
        }
        
        // Check to see if args[1], the number of items in an entry, is valid.
        try
        {
            int.Parse(args[1]);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw new ArgumentException("Username length must be an integer.");
        }
        if (int.Parse(args[1]) < 1)
        {
            throw new ArgumentException("Cannot have a username length less than 1.");
        }
        
        // Check to see if args[1], the number of items in an entry, is valid.
        try
        {
            int.Parse(args[2]);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw new ArgumentException("Number of items for an entry must be an integer.");
        }
        if (int.Parse(args[2]) < 1)
        {
            throw new ArgumentException("Cannot have fewer than one items in an entry.");
        }
        
        // Check to see if args[2], the upper limit of entries, is valid.
        try
        {
            int.Parse(args[3]);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw new ArgumentException("Maximum number of entries must be an integer.");
        }
        if (int.Parse(args[3]) < 1)
        {
            throw new ArgumentException("Cannot have fewer than one entry in the leaderboard.");
        }

        // format and dataTypeNames must include the pre set score and username items in an entry.
        List<string> format = ["score", "username"];
        List<string> dataTypeNames = ["int", "string"];

        for (int i = 0; i < Convert.ToInt16(args[1]); i++)
        {
            if (i == 0)
            {
                Console.WriteLine($"Enter type for item 1 (int, string, datetime):");
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

        context.Server.AddLeaderboard(args[0], format, dataTypeNames,int.Parse(args[3]),int.Parse(args[1]));
        Console.WriteLine($"Leaderboard '{args[0]}' created with format: {string.Join(", ", format)}");
    }
}