using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using HighscoreServer.DataServices;
using HighscoreServer.DataTypes;
using HighscoreServer.Loggers;

namespace HighscoreServer
{
    /// <summary>
    /// A server that communicates with the client over a websocket.
    /// Handles GET and POST commands.
    /// </summary>
    public class HighscoreServer
    {
        private readonly HttpListener _listener;
        private readonly SemaphoreSlim _semaphore;
        private readonly EncryptionHandler _encryptionHandler;
        static readonly LoggerFile Logger = new LoggerFile("Logs");
        
        private bool _isRunning;
        private int _activeRequests;
        private Game _data;
        
        // Change to allow user to specify their default directory.
        static readonly IDataService DataService = new FileDataService("SavedData",".json", Logger);
        //private const string DefaultDataDirectory = "SavedData";
        private const string DefaultFileName = "data";
        
        // Secret Int here. Must change. Make sure that the secret is the same for client and server.
        private const int ServerSecret = 1;
        private readonly Dictionary<WebSocket, byte[]> _clientKeys = new Dictionary<WebSocket, byte[]>();
        private List<WebSocket> _websockets = new List<WebSocket>();

        public HighscoreServer(string port)
        {
            _data = new Game();
            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://localhost:{port}/");
            _semaphore = new SemaphoreSlim(1, 1); // Allows only one request at a time
            _encryptionHandler = new EncryptionHandler();   
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
                Game newData = DataService.Load(defaultFileName);
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
            Logger.Log("Type 'shutdown' to stop the server. Type 'help' to see a list of commands");
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
            foreach (WebSocket ws in _websockets)
            {
                Logger.Log("Stopped a websocket session.");
                ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "410 Server shut down.",
                    CancellationToken.None);
            }
            SaveData();
            Logger.Log("Saved current data to data.json.");
            Logger.Log("Shutting down the server...");
            _listener.Stop();
            Program.RequestShutdown();
        }

        /// <summary>
        /// Asynchronously listen for requests to start a websocket connection.
        /// </summary>
        private async Task ListenAsync()
        {
            try
            {
                while (_listener.IsListening && _isRunning)
                {
                    // Always listening for WebSocket requests
                    var context = await _listener.GetContextAsync();
                    if (context.Request.IsWebSocketRequest)
                    {
                        var webSocketContext = await context.AcceptWebSocketAsync(null);
                        _ = HandleWebsocketAsync(webSocketContext);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                        context.Response.Close();
                    }
                }
            }
            catch (HttpListenerException) when (!Program.IsShuttingDown())
            {
                // Expected exception when listener is stopped
            }
        }

        /// <summary>
        /// Handle websockets.
        /// Handle requests from websockets if the encryption key has been established.
        /// Otherwise, create an encryption key if the secrets match.
        /// </summary>
        /// <param name="context">The websocket context between server and client.</param>
        private async Task HandleWebsocketAsync(HttpListenerWebSocketContext context)
        {
            var webSocket = context.WebSocket;
            var buffer = new byte[1024 * 4];

            while (webSocket.State == WebSocketState.Open && _isRunning)
            {
                try
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        //Close gracefully when the client stops the session.
                        if (_clientKeys.ContainsKey(webSocket))
                        {
                            _clientKeys.Remove(webSocket);
                        }
                        Logger.Log("Socket closed.");
                    }
                    var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var parts = receivedMessage.Split(' ');

                    if (_clientKeys.ContainsKey(webSocket)) // Do if an encryption key has been shared between the server and client.
                    {
                        // Decrypt the message
                        byte[] key = _clientKeys[webSocket];
                        byte[] encryptedMessage = Convert.FromBase64String(parts[0]);
                        byte[] decryptedMessage = _encryptionHandler.Decrypt(encryptedMessage, key);

                        // Handle the decrypted message
                        await HandleRequest(webSocket, Encoding.UTF8.GetString(decryptedMessage));
                    }
                    else if (parts[0] == "secret") // Do if the client is confirming secret.
                    {
                        if (int.TryParse(parts[1], out int clientSecret) && clientSecret == ServerSecret)
                        {
                            // If the client has a matching secret, generate a new encryption key for the websocket and share with the client.
                            var key = _encryptionHandler.GenerateEncryptionKey();
                            _clientKeys[webSocket] = key;
                            _websockets.Add(webSocket);
                            Logger.Log($"Session opened with new client.");
                            await SendMessageAsync(webSocket, Convert.ToBase64String(key));
                        }
                        else
                        {
                            await webSocket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "403 Invalid secret",
                                CancellationToken.None);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"{ex.GetType()}: {ex.Message}", LoggerBase.SeverityLevel.Error);
                }
            }

            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        }

        private async Task HandleRequest(WebSocket webSocket, string requestContent)
        {
            await _semaphore.WaitAsync();
            Interlocked.Increment(ref _activeRequests);
            try
            {
                //string serializedInfo = requestContent.Replace("\\u0022", "\"");
                //string serializedInfo = requestContent.Substring(1, requestContent.Length - 2);
                //Logger.Log($"Received request: {serializedInfo}");
                
                var info = JsonSerializer.Deserialize<string>(requestContent)!.Split(" ", 2);
                if (info == null)
                {
                    throw new Exception("Invalid request.");
                }
                Logger.Log($"Received a {info[0]} request.");
                switch (info[0])
                {
                    case "GET":
                    {
                        var parts = info[1].Split(' ');
                        var responseString = JsonSerializer.Serialize(_data.GetTopNFromLeaderboard(parts[0], int.Parse(parts[1]), int.Parse(parts[2]), int.Parse(parts[3])));
                        await SendEncryptedMessageAsync(webSocket, responseString);

                        break;
                    }
                    case "POST":
                    {
                        var parts = info[1].Split(' ', 2);
                        var result = _data.AddEntry(parts[0], parts[1].Split(" "));
                        Logger.Log("Added entry.");
                        var responseString = JsonSerializer.Serialize(result.Status);
                        await SendEncryptedMessageAsync(webSocket, responseString);
                        break;
                    }
                    default:
                    {
                        Logger.Log($"Could not handle request of type: {info[0]}.", LoggerBase.SeverityLevel.Warning);
                        break;
                    }
                }
                Logger.Log("Completed Request.");
            }
            finally
            {
                Interlocked.Decrement(ref _activeRequests);
                _semaphore.Release();
            }
        }
        
        private async Task SendMessageAsync(WebSocket webSocket, string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }
        
        private async Task SendEncryptedMessageAsync(WebSocket webSocket, string data)
        {
            byte[] encryptedData = _encryptionHandler.Encrypt(Encoding.UTF8.GetBytes(data), _clientKeys[webSocket]);

            string encryptedMessage = Convert.ToBase64String(encryptedData);
            await SendMessageAsync(webSocket, encryptedMessage);
        }
    }
}