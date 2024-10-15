using System.Diagnostics;
using System.Text.Json;
using HighscoreListener.DataServices;
using HighscoreListener.Loggers;

namespace HighscoreListener.Commands;

public class CommandContext
{
    public IDataService dataService;
    public string defaultDataDirectory;
    public string defaultFileName;
    public Game data;
    public Server server;
    public LoggerBase logger;

    public CommandContext(IDataService dataService, string defaultDataDirectory, string defaultFileName, Game data, Server server, LoggerBase logger)
    {
        this.dataService = dataService;
        this.defaultDataDirectory = defaultDataDirectory;
        this.defaultFileName = defaultFileName;
        this.data = data;
        this.server = server;
        this.logger = logger;
    }

    public void UpdateData(Game newData)
    {
        Debug.Assert(newData != null, "Something went wrong. Tried to update data to a null.");
        this.data = newData!;
        Console.WriteLine(JsonSerializer.Serialize(this.data));
        this.server.UpdateData(this.data);
    }
}