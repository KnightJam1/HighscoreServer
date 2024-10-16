namespace HighscoreServer.Commands;
/// <summary>
/// Displays the possible commands in the console.
/// </summary>
public class HelpCommand : ICommand
{
    public string Name => "help";
    public int NumArgs => 0;
    public string HelpText => "Lists all help text.";
    private readonly IEnumerable<ICommand> _commands;
    
    public HelpCommand(IEnumerable<ICommand> commands)
    {
        _commands = commands;
    }
    
    public void Execute(CommandContext context, string[] args)
    {
        // Check for a valid number of arguments.
        if (args.Length != NumArgs)
        {
            throw new ArgumentException($"Invalid number of arguments. Expected {NumArgs} but got {args.Length}.");
        }
        
        Console.WriteLine("Available commands:");
        foreach (var command in _commands)
        {
            Console.WriteLine($"{command.Name} (Args: {command.NumArgs}): {command.HelpText}");
        }
    }
}