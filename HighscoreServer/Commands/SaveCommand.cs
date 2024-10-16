namespace HighscoreServer.Commands;

public class SaveCommand : ICommand
{
    public string Name => "save";
    public int NumArgs => 1;
    public string HelpText => "Save current set of leaderboards as a json file.";

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