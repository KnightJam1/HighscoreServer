using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static HttpListener listener = new HttpListener();
    static CancellationTokenSource cts = new CancellationTokenSource();
    static Dictionary<string, int> data = new Dictionary<string, int>();
    static string defaultFilePath = "data.json";

    static async Task Main()
    {
        // Load data when the server starts
        LoadData(defaultFilePath);
        listener.Prefixes.Add("http://localhost:8080/");
        listener.Start();
        Console.WriteLine("Listening... Type 'shutdown' to stop the server.");

        // Start listening for HTTP requests
        var listenTask = ListenAsync();

        // Start the command handling loop
        while (true)
        {
            string command = Console.ReadLine();
            if (command == null) // Ignore nulls from the console
            {
                continue;
            }

            switch (command.Trim().ToLower())
            {
                case "shutdown":
                    cts.Cancel();
                    listener.Stop();
                    SaveData(defaultFilePath); // Save data on shutdown
                    break;
                case "status":
                    Console.WriteLine("Server is running...");
                    break;
                case "help":
                    Console.WriteLine("Type shutdown to close the server\nType status to see the status of the server.");
                    break;
                case "load":
                    if (commandParts.Length == 2)
                    {
                        AskToSaveAndLoad(commandParts[1]);
                    }
                    else
                    {
                        Console.WriteLine("Please specify a file name.");
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
            using (var reader = new System.IO.StreamReader(context.Request.InputStream))
            {
                string requestBody = await reader.ReadToEndAsync();
                var newEntry = JsonSerializer.Deserialize<Dictionary<string, int>>(requestBody);
                foreach (var item in newEntry)
                {
                    data[item.Key] = item.Value;
                }
            }
        }

        string responseString = JsonSerializer.Serialize(data);
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
        context.Response.ContentLength64 = buffer.Length;

        using (var output = context.Response.OutputStream)
        {
            await output.WriteAsync(buffer, 0, buffer.Length);
        }
    }

    static void SaveData(string filePath)
    {
        var jsonData = JsonSerializer.Serialize(data);
        File.WriteAllText(filePath, jsonData);
        Console.WriteLine($"Data saved to {filePath}.");
    }

    static void LoadData(string filePath)
    {
        if (File.Exists(filePath))
        {
            var jsonData = File.ReadAllText(filePath);
            data = JsonSerializer.Deserialize<Dictionary<string, int>>(jsonData);
            Console.WriteLine($"Data loaded from {filePath}.");
        }
        else
        {
            Console.WriteLine($"File {filePath} could not be loaded as it does not exist.");
        }
    }

    static void AskToSaveAndLoad(string filePath)
    {
        Console.WriteLine("Do you want to save current data before loading new data? (yes/no)");
        string response = Console.ReadLine();
        if (response.Trim().ToLower() == "yes")
        {
            SaveData(defaultFilePath);
        }
        LoadData(filePath);
    }
}
    