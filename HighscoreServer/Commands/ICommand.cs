namespace HighscoreListener.Commands;

/// <summary>
/// Interface for commands that can be called by the user.
/// </summary>
public interface ICommand
{
    /// <summary>
    /// What the command should be recognised as in the terminal.
    /// </summary>
    string Name { get; }
    
    /// <param name="context">Context object that contains any objects a command might need, such as the data service.</param>
    /// <param name="args">Some commands require extra arguments from the command line.
    /// Anything typed into the console after the name of the command is passed into the command here.</param>
    public void Execute(CommandContext context, string[] args);
}