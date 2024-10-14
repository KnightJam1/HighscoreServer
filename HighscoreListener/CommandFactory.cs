using System;
using System.Collections.Generic;
using System.Linq;

public class CommandFactory
{
    private readonly Dictionary<string, ICommand> _commands;

    public CommandFactory()
    {
        _commands = new Dictionary<string, ICommand>();

        // Automatically register all commands
        RegisterCommands();
    }

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