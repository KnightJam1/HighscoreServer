namespace HighscoreServer.Commands;

/// <summary>
/// Command to run the initial load at the start of the program.
/// Loads the default file, or new data if it cannot be found.
/// </summary>
public class DefaultLoadCommand : ICommand
{
    public string Name => "defaultLoad";
    public void Execute(CommandContext context, string[] args)
    {
        // Load new data. Only assign data if the new data is not null.
        //Game? newData = context.Server.DataService.FirstTimeLoad(context.DefaultFileName);
        //context.UpdateData(newData);
        context.Server.InitialLoad(args[0]);
    }
}