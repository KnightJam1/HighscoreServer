using System.Net;
using SaveLoadSystem;

public class LoadCommand : ICommand
{
    public string Name => "load";
    public void Execute(CommandContext context, string[] args)
    {
        Console.WriteLine("Do you want to save current data before loading new data? (yes/no)");
        string response = Console.ReadLine() ?? "yes";
        if (response.Trim().ToLower() == "yes")
        {
            context.dataService.Save(context.defaultFileName, context.data);
        }
        // Load new data. Only assign data if the new data is not null.
        Game? newData = context.dataService.Load(args[0]);
        //context.data ??= newData!;
        //context.server.UpdateData(context.data);
        context.UpdateData(newData!);
    }
}