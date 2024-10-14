using System.Net;
using SaveLoadSystem;

public class DefaultLoadCommand : ICommand
{
    public string Name => "defaultLoad";
    public void Execute(CommandContext context, string[] args)
    {
        // Load new data. Only assign data if the new data is not null.
        Game? newData = context.dataService.FirstTimeLoad(context.defaultFileName);
        context.UpdateData(newData!);
    }
}