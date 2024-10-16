namespace HighscoreServer.Commands;

/// <summary>
/// Saves current data to a json file named by the argument passed.
/// </summary>
public class SaveCommand : ICommand
{
    public string Name => "save";
    public int NumArgs => 1;
    public string HelpText => "Save current set of leaderboards. Requires a name for the json file.";


    /// <param name="args">Filename where the data should be saved.</param>
    /// <exception cref="ArgumentException">Throws an exception if there isn't an argument for the file name.</exception>
    public void Execute(CommandContext context, string[] args)
    {
        // Check for a valid number of arguments.
        if (args.Length != NumArgs)
        {
            throw new ArgumentException($"Invalid number of arguments. Expected {NumArgs} but got {args.Length}.");
        }
        
        context.Server.SaveData(args[0]);
    }
}