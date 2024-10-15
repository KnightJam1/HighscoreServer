using System.Diagnostics;

namespace HighscoreServer.Commands;

/// <summary>
/// Command to create a new leaderboard to be added to a game.
/// </summary>
public class CreateCommand : ICommand
{
    public string Name => "create";
    
    /// <param name="args">Should be a string name and int16 number of entries. E.g. 'gamemode1 3'</param>
    public void Execute(CommandContext context, string[] args)
    {  
        // args[0] should be a name, args[1] should be the number of entries
        // Assert or catch if args[1] isnt int16

        Debug.Assert(args[0] != "", "Hey!");

        List<string> format = new List<string>();
        List<string> dataTypeNames = new List<string>();

        for (int i = 0; i < Convert.ToInt16(args[1]); i++)
        {
            Console.WriteLine($"Enter type for item {i + 1} (string, int, datetime):");
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

        context.Server.AddLeaderboard(args[0], format, dataTypeNames);
        Console.WriteLine($"Leaderboard '{args[0]}' created with format: {string.Join(", ", format)}");
    }
}