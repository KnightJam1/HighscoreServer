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
    public readonly Server Server;
    public readonly LoggerBase Logger;

    public CommandContext(Server server, LoggerBase logger)
    {
        this.Server = server;
        this.Logger = logger;
    }
}