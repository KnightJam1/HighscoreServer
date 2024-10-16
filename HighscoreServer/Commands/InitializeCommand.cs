namespace HighscoreServer.Commands;

/// <summary>
/// Command to run the initial load at the start of the program.
/// Loads the default file, or new data if it cannot be found.
/// Different from the load command as it should not ask the user if they wish to save beforehand.
/// </summary>
public class InitializeCommand : ICommand
{
    public string Name => "initialize";
    public int NumArgs => 0;
    public string HelpText => "Load default data. Use only at the start as old data will not be saved.";
    public void Execute(CommandContext context, string[] args)
    {
        // Load new data. Only assign data if the new data is not null.
        context.Server.InitialLoad();
    }
}