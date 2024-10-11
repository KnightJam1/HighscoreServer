using System.Net;
using SaveLoadSystem;
using SaveLoadSystem;

public class LoadCommand : ICommand
{
    private readonly IDataService dataService;
    private readonly string _fileName;
    private readonly Game data;
    public string Name => "load";

    public LoadCommand(string defaultDataDirectory, string fileName, Game data)
    {
        dataService = new FileDataService(defaultDataDirectory);
        _fileName = fileName;
    }
    public void Execute(string[] args)
    {
        AskToSaveAndLoad(_fileName);
    }

    static void AskToSaveAndLoad(string filePath)
    {
        Console.WriteLine("Do you want to save current data before loading new data? (yes/no)");
        string response = Console.ReadLine() ?? "yes";
        if (response.Trim().ToLower() == "yes")
        {
            dataService.Save(filePath, data);
        }
        // Load new data. Only assign data if the new data is not null.
        // Null forgiving operator is used because data has only one assignment prior to this function.
        // Mentioned assignment does not allow data to be null
        Game? newData = dataService.Load(filePath);
        data ??= newData!;
        server.UpdateData(data);
    }
}