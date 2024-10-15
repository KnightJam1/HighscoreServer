namespace HighscoreServer.Commands;

/// <summary>
/// Factory class that automatically registers commands.
/// </summary>
public class CommandFactory
{
    private readonly Dictionary<string, ICommand> _commands;

    public CommandFactory()
    {
        _commands = new Dictionary<string, ICommand>();

        // Automatically register all commands
        RegisterCommands();
    }

    /// <summary>
    /// Automatically registers commands.
    /// Creates an enumerable list of all command types, creates an instance of each and puts them in a dictionary.
    /// </summary>
    private void RegisterCommands()
    {
        var commandTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(ICommand).IsAssignableFrom(t) && !t.IsInterface);

        foreach (var type in commandTypes)
        {
            var commandInstance = (ICommand)Activator.CreateInstance(type)!;
            if (commandInstance != null){
                _commands[commandInstance.Name.ToLower()] = commandInstance;
            }
        }
    }

    /// <summary>
    /// Get a specific command
    /// </summary>
    /// <param name="commandName">Name of command</param>
    /// <returns>Returns a command of specified name if it exists.</returns>
    /// <exception cref="InvalidOperationException">Throws an exception if the named command doesn't exist.</exception>
    public ICommand GetCommand(string commandName)
    {
        if (_commands.ContainsKey(commandName.ToLower()))
        {
            return _commands[commandName.ToLower()];
        }
        // Console.WriteLine("Unknown command");
        throw new InvalidOperationException($"Command '{commandName}' does not exist.");
    }
}