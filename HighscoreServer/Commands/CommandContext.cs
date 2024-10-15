using System.Diagnostics;
using System.Text.Json;
using HighscoreServer.DataServices;
using HighscoreServer.Loggers;

namespace HighscoreServer.Commands;

/// <summary>
/// Contains all of the relevant and necessary objects that commands need access to.
/// </summary>
public class CommandContext
{
    public string DefaultDataDirectory;
    public readonly string DefaultFileName;
    public readonly Server Server;
    public readonly LoggerBase Logger;

    public CommandContext(string defaultDataDirectory, string defaultFileName, Server server, LoggerBase logger)
    {
        this.DefaultDataDirectory = defaultDataDirectory;
        this.DefaultFileName = defaultFileName;
        this.Server = server;
        this.Logger = logger;
    }
}