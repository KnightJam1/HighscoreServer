using System.Net;
using System.Text.Json;
using HighscoreServer.DataServices;
using HighscoreServer.Loggers;

namespace HighscoreServer
{
    /// <summary>
    /// A server that listens for requests from the client.
    /// Handles GET and POST commands.
    /// </summary>
    public class Server : IServer
    {
        private readonly HttpListener _listener;
        static readonly LoggerTerminal Logger = new LoggerTerminal();
        
        private bool _isRunning;
        private int _activeRequests;
        private Game _data;
        
        static readonly IDataService DataService = new FileDataService("SavedData",".json");
        private const string DefaultDataDirectory = "SavedData";
        private const string DefaultFileName = "data";

        public Server(string port)
        {
            _data = new Game();
            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://localhost:{port}/");
        }

        public void AddLeaderboard(string name, List<string> format, List<string> dataTypeNames, int maxEntries)
        {
            _data.AddLeaderboard(name, format, dataTypeNames, maxEntries);
        }
        
        public void InitialLoad() // Move functionality to LoadData? Pass an isInitial flag maybe
        {
            Game newData = DataService.InitialLoad(DefaultFileName);
            _data = newData;
        }

        public void LoadData(string defaultFileName)
        {
            try
            {
                Game newData = DataService.Load(defaultFileName)!;
                _data = newData;
            }
            catch (Exception ex)
            {
                Logger.Log($"{ex.GetType()}: {ex.Message}", LoggerBase.SeverityLevel.Error);
            }
        }

        public void NewData()
        {
            _data = new Game();
        }

        public void SaveData(string saveFileName = DefaultFileName)
        {
            try
            {
                DataService.Save(saveFileName, _data);
            }
            catch (Exception ex)
            {
                Logger.Log($"{ex.GetType()}: {ex.Message}", LoggerBase.SeverityLevel.Error);
            }
        }

        public void DeleteData(string fileName)
        {
            try
            {
                DataService.Delete(fileName);
            }
            catch (Exception ex)
            {
                Logger.Log($"{ex.GetType()}: {ex.Message}", LoggerBase.SeverityLevel.Error);
            }
        }
        
        public async Task Start()
        {
            _isRunning = true;
            _listener.Start();
            Logger.Log("Now Listening...");
            Logger.Log("Type 'shutdown' to stop the server. Type 'help' to see a list of commands"); // Consider listing prefixes
            await ListenAsync();
        }

        public void RequestStop()
        {
            _isRunning = false;
            Logger.Log("Waiting for active requests to stop...");
            while (_activeRequests > 0)
            {
                Thread.Sleep(100); // Small delay to prevent busy-waiting
            }
            SaveData();
            Logger.Log("Saved current data to data.json.");
            Logger.Log("Shutting down the server...");
            _listener.Stop();
            Program.RequestShutdown();
        }

        /// <summary>
        /// Asynchronously listen for requests from clients.
        /// </summary>
        public async Task ListenAsync()
        {
            try
            {
                while (_isRunning)
                {
                    var listenContext = await _listener.GetContextAsync();
                    Interlocked.Increment(ref _activeRequests);
                    _ = Task.Run(() => HandleRequest(listenContext));
                }
            }
            catch (HttpListenerException) when (!Program.IsShuttingDown())
            {
                // Expected exception when listener is stopped
            }
        }

        /// <summary>
        /// Handle GET and POST requests from the client.
        /// </summary>
        /// <param name="context">The context of the request.</param>
        public async Task HandleRequest(HttpListenerContext context)
        {
            try
            {
                switch (context.Request.HttpMethod)
                {
                    case "POST":
                    {
                        using var reader = new StreamReader(context.Request.InputStream);
                        var requestBody = await reader.ReadToEndAsync();
                        var entry = JsonSerializer.Deserialize<KeyValuePair<string, string[]>>(requestBody);

                        try // Switch to a try/catch
                        {
                            _data.AddEntry(entry.Key, entry.Value);
                            context.Response.StatusCode = (int)HttpStatusCode.OK;
                            var response = JsonSerializer.Serialize(new { Message = "Entry added successfully." });
                            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(response);
                            context.Response.ContentLength64 = buffer.Length;
                            await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                        }
                        catch (Exception ex)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            var response = JsonSerializer.Serialize(new { Error = ex.Message });
                            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(response);
                            context.Response.ContentLength64 = buffer.Length;
                            await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                        }

                        break;
                    }
                    case "GET":
                    {
                        var responseString = JsonSerializer.Serialize(_data.GetFormats());
                        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                        context.Response.ContentLength64 = buffer.Length;

                        using (var output = context.Response.OutputStream)
                        {
                            await output.WriteAsync(buffer, 0, buffer.Length);
                        }

                        break;
                    }
                }
            }
            finally
            {
                Interlocked.Decrement(ref _activeRequests);
            }
        }
    }
}