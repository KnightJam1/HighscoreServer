﻿using HighscoreListener.Commands;
using HighscoreListener.DataServices;
using HighscoreListener.Loggers;

namespace HighscoreListener;

class Program
{
    // Needed here because it's used by two commands.
    static string _defaultDataDirectory = "SavedData";
    static string _defaultFileName = "data";
    static bool _shutdownRequested = false;

    // Passed into context.
    static Game _data = new Game();
    static Server _server = new Server("http://localhost:8080/",_data);
    static IDataService _dataService = new FileDataService(_defaultDataDirectory);
    static LoggerTerminal _logger = new LoggerTerminal();

    // Used only in main.
    static CommandFactory _factory = new CommandFactory();
    static Executor _executor = new Executor(_factory);

    

    static void Main()
    {
        // Start asynchronous server
        _ = _server.Start();

        // Load data when the server starts
        CommandContext context = new CommandContext(_dataService,_defaultDataDirectory,_defaultFileName,_data,_server,_logger);
        _executor.ExecuteCommand(context, "defaultLoad");

        //Console.WriteLine("Now Listening...\nType 'shutdown' to stop the server. Type 'help' to see a list of commands");

        // Start the command handling loop
        while (true)
        {
            string command = Console.ReadLine() ?? "";
            try
            {
                _executor.ExecuteCommand(context, command);
            }
            catch
            {
                Console.WriteLine($"Command {command} could not be performed."); // Replace later with a "not-a-command" command.
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

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static bool isShuttingDown()
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

    // Add more exceptions into class functions. Use Try Catch statements to have workarounds to the problem.
    // Change Null management to 
    // Exceptions should only be used for networking and filesystems if Possible.

    // SwitchCase to execute. Passed an instance of the class of the command to be executed.
    // Executer class is passed the command line and runs a command class.
    // Find a way to make 
    // Make a CommandBase class with a run function.

    // Move files to their own subfolders to organise. Utilise namespaces to make things easier.
    // Theres still some extra code ran after every load so consider abstracting it

    // Create a command context that has all of the objects.