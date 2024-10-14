using System.Net;
using SaveLoadSystem;
using ServerSystem;

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
}