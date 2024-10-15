namespace HighscoreListener.Commands;

public class CommandProcessor
{
    private readonly CommandFactory _factory;

    public CommandProcessor(CommandFactory factory)
    {
        _factory = factory;
    }

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