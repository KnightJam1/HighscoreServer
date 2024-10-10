using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using SaveLoadSystem;

class Program
{
    static HttpListener listener = new HttpListener();
    static CancellationTokenSource cts = new CancellationTokenSource();
    static Game data = new Game();
    static string defaultDataDirectory = "savedData";
    static string defaultFileName = "data";
    static LoggerTerminal logger = new LoggerTerminal();
    static IDataService dataService = new FileDataService(defaultDataDirectory);

    static async Task Main()
    {
        // Load data when the server starts
        // LoadData(defaultFilePath);
        dataService.Load(defaultFileName);
        listener.Prefixes.Add("http://localhost:8080/");
        listener.Start();
        Console.WriteLine("Now Listening...\nType 'shutdown' to stop the server. Type 'help' to see a list of commands");

        // Start listening for HTTP requests
        var listenTask = ListenAsync();

        // Start the command handling loop
        while (true)
        {
            string command = Console.ReadLine() ?? "";

            // Do console commands. Switch statement looks at first word so arguments can be passed afterwards.
            var commandParts = command.Trim().ToLower().Split(' ', 3);
            switch (commandParts[0])
            {
                case "shutdown":
                    cts.Cancel();
                    listener.Stop();
                    dataService.Save(defaultFileName, data); // Save data on shutdown
                    break;
                case "status":
                    Console.WriteLine("Server is running...");
                    break;
                case "help":
                    Console.WriteLine("Console commands:\n\tshutdown\t\t- close the server.\n\tstatus\t\t\t- see the status of the server.\n\tload filename.json\t- load the specified file.");
                    break;
                case "load":
                    if (commandParts.Length == 2)
                    {
                        AskToSaveAndLoad(commandParts[1]);
                    }
                    if (commandParts.Length == 3)
                    {
                        Console.WriteLine("Too many arguments.");
                    }
                    else
                    {
                        Console.WriteLine("Please specify a file name.");
                    }
                    break;
                case "create":
                    if (commandParts.Length == 3)
                    {
                        CreateNewLeaderboard(commandParts[1], int.Parse(commandParts[2]));
                    }
                    else
                    {
                        Console.WriteLine("Please provide the leaderboard details. Example: create leaderboardName 3");
                    }
                    break;
                default:
                    Console.WriteLine("Unknown command. Type help to see possible commands.");
                    break;
            }

            if (cts.Token.IsCancellationRequested)
            {
                break;
            }
        }

        await listenTask;
        Console.WriteLine("Server has shut down.");
    }

    // Listen for a client to request something from the server
    static async Task ListenAsync()
    {
        try
        {
            while (!cts.Token.IsCancellationRequested)
            {
                var context = await listener.GetContextAsync();
                _ = Task.Run(() => HandleRequest(context));
            }
        }
        catch (HttpListenerException) when (cts.Token.IsCancellationRequested)
        {
            // Expected exception when listener is stopped
        }
    }

    // Handle the request, with functionality for POST
    static async Task HandleRequest(HttpListenerContext context)
    {
        if (context.Request.HttpMethod == "POST")
        {
            using (var reader = new StreamReader(context.Request.InputStream))
            {
                string requestBody = await reader.ReadToEndAsync();
                var entry = JsonSerializer.Deserialize<KeyValuePair<string, string[]>>(requestBody);

                if (data.AddEntry(entry.Key, entry.Value, out string message))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    var response = JsonSerializer.Serialize(new { Message = message });
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(response);
                    context.Response.ContentLength64 = buffer.Length;
                    await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    var response = JsonSerializer.Serialize(new { Error = message });
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(response);
                    context.Response.ContentLength64 = buffer.Length;
                    await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                }
            }
        }
        else if (context.Request.HttpMethod == "GET")
        {
            var responseString = JsonSerializer.Serialize(data.GetFormats());
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            context.Response.ContentLength64 = buffer.Length;

            using (var output = context.Response.OutputStream)
            {
                await output.WriteAsync(buffer, 0, buffer.Length);
            }
        }
    }

    // static void SaveData(string filePath)
    // {
    //     var jsonData = JsonSerializer.Serialize(data);
    //     File.WriteAllText(filePath, jsonData);
    //     Console.WriteLine($"Data saved to {filePath}.");
    // }

    // static void LoadData(string filePath)
    // {
    //     if (File.Exists(filePath))
    //     {
    //         var jsonData = File.ReadAllText(filePath);
    //         data = JsonSerializer.Deserialize<Game>(jsonData);
    //         Console.WriteLine($"Data loaded from {filePath}.");
    //     }
    //     else
    //     {
    //         Console.WriteLine($"File {filePath} could not be loaded as it does not exist.");
    //     }
    // }

    static void AskToSaveAndLoad(string filePath)
    {
        Console.WriteLine("Do you want to save current data before loading new data? (yes/no)");
        string response = Console.ReadLine() ?? "yes";
        if (response.Trim().ToLower() == "yes")
        {
            dataService.Save(defaultFileName, data);
        }
        data = dataService.Load(filePath) ?? data;
    }

    static void CreateNewLeaderboard(string name, int length)
    {
        List<string> format = new List<string>();
        List<string> dataTypeNames = new List<string>();

        for (int i = 0; i < length; i++)
        {
            Console.WriteLine($"Enter type for item {i + 1} (string, int, datetime):");
            string typeInput = Console.ReadLine() ?? "";

            switch (typeInput.ToLower())
            {
                case "string":
                case "int":
                case "datetime":
                    dataTypeNames.Add(typeInput.ToLower());
                    break;
                default:
                    Console.WriteLine($"Unknown type '{typeInput}'. Supported types are: string, int, datetime.");
                    return;
            }

            Console.WriteLine($"Enter name for item {i + 1}:");
            string nameInput = Console.ReadLine() ?? "";
            format.Add(nameInput);
        }

        data.AddLeaderboard(name, format, dataTypeNames);
        Console.WriteLine($"Leaderboard '{name}' created with format: {string.Join(", ", format)}");
    }
}
    
    // Look at SOLID. Class has one responsibility
    // Move functions to be methods of specific classes e.g. serialize/deserialize
    // Look for sorted arrays/dicts.
    // Allow/disallow administration through networking.
    // Server class. Put listener inside.
    // Class for writing out debug info. Logging class. Admin can determine if they want console, disk or no log. Severity levels: error, warn, info. Severity is an enum.
    // Push to external server.
    // Commit with every change.
    // Logger class should be an interface object. ILogger.
    // 