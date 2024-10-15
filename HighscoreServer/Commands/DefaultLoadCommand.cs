namespace HighscoreListener.Commands;

public class DefaultLoadCommand : ICommand
{
    public string Name => "defaultLoad";
    public void Execute(CommandContext context, string[] args)
    {
        // Load new data. Only assign data if the new data is not null.
        Game? newData = context.DataService.FirstTimeLoad(context.DefaultFileName);
        context.UpdateData(newData!);
    }
}