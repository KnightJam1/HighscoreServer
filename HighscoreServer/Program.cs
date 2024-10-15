using HighscoreServer.Commands;
using HighscoreServer.DataServices;
using HighscoreServer.Loggers;

namespace HighscoreServer;

static class Program
{
    // Unnecessary. Move to server.
    static readonly string DefaultDataDirectory = "SavedData";
    static readonly string DefaultFileName = "data";
    
    // Necessary static property
    private static bool _shutdownRequested = false;

    // Passed into context.
    static Server _server = new Server("8080");
    static readonly LoggerTerminal Logger = new LoggerTerminal();

    // Used only in main.
    static readonly CommandFactory Factory = new CommandFactory();
    static readonly CommandProcessor CommandProcessor = new CommandProcessor(Factory);

    

    static void Main()
    {
        // Start asynchronous server
        _ = _server.Start();

        // Load data when the server starts
        CommandContext context = new CommandContext(DefaultDataDirectory,DefaultFileName,_server,Logger);
        CommandProcessor.ExecuteCommand(context, "defaultLoad");

        // Start the command handling loop
        while (true)
        {
            string command = Console.ReadLine() ?? "";
            try
            {
                CommandProcessor.ExecuteCommand(context, command);
            }
            catch (Exception ex)
            {
                Logger.Log($"{ex.GetType()}: {ex.Message}", LoggerBase.SeverityLevel.Error);
            }

            if (_shutdownRequested)
            {
                _server.Stop();
                break;
            }
        }

        Console.WriteLine("Server has shut down.");
    }

    /// <returns> return true when shutdown request is accepted </returns>
    public static bool RequestShutdown()
    {
        _shutdownRequested = true;
        return true; // The shutdown was accepted
    }

    /// <returns> returns whether a shutdown has been requested or not</returns>
    public static bool IsShuttingDown()
    {
        return _shutdownRequested;
    }

    //Make a hasShutdown command 
}

// Look at SOLID. Class has one responsibility
    // Look for sorted arrays/dicts.
    // Allow/disallow an attempt of administration by networking.
    // Move terminal management to it's own class? Make an interface so it can be done by web too?

    // DEFENSIVE PROGRAMMING
    // Remove as much static as possible
    // TDD Test Driven Development.
    // Add asserts to each function to test the input parameters. Handle incorrect options if possible and throw exceptions so they can be caught in the main code.