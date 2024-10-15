namespace HighscoreServer.Commands;

/// <summary>
/// Processor used to execute commands.
/// </summary>
public class CommandProcessor
{
    private readonly CommandFactory _factory;

    public CommandProcessor(CommandFactory factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Execute the specified command.
    /// </summary>
    /// <param name="context">Context object that contains any objects a command might need.</param>
    /// <param name="input">Commandline input containing the command name and any additional arguments.</param>
    public void ExecuteCommand(CommandContext context, string input)
    {
        var parts = input.Split(' ');
        var commandName = parts[0];
        var args = parts.Length > 1 ? parts[1..] : Array.Empty<string>();

        var command = _factory.GetCommand(commandName);
        if (command != null)
        {
            command.Execute(context, args);
        }
        else
        {
            Console.WriteLine("Unknown command");
        }
    }
}