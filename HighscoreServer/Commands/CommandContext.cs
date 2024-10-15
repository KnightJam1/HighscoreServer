using System.Diagnostics;
using System.Text.Json;
using HighscoreListener.DataServices;
using HighscoreListener.Loggers;

namespace HighscoreListener.Commands;

public class CommandContext
{
    public readonly IDataService DataService;
    public string DefaultDataDirectory;
    public readonly string DefaultFileName;
    public Game Data;
    private readonly Server _server;
    public readonly LoggerBase Logger;

    public CommandContext(IDataService dataService, string defaultDataDirectory, string defaultFileName, Game data, Server server, LoggerBase logger)
    {
        this.DataService = dataService;
        this.DefaultDataDirectory = defaultDataDirectory;
        this.DefaultFileName = defaultFileName;
        this.Data = data;
        this._server = server;
        this.Logger = logger;
    }

    public void UpdateData(Game newData)
    {
        Debug.Assert(newData != null, "Something went wrong. Tried to update data to a null.");
        this.Data = newData!;
        Console.WriteLine(JsonSerializer.Serialize(this.Data));
        this._server.UpdateData(this.Data);
    }
}