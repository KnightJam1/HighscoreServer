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
    public class WebsocketServer
    {
        private readonly HttpListener _listener;
        private readonly SemaphoreSlim _semaphore;
        private readonly EncryptionHandler _encryptionHandler;
        static readonly LoggerTerminal Logger = new LoggerTerminal();
        
        private bool _isRunning;
        private int _activeRequests;
        private Game _data;
        
        static readonly IDataService DataService = new FileDataService("SavedData",".json");
        private const string DefaultDataDirectory = "SavedData";
        private const string DefaultFileName = "data";
        
        // Secret Int here. Must change. Make sure that the secret is the same for client and server.
        private const int ServerSecret = 1;
        private readonly Dictionary<WebSocket, byte[]> _clientKeys = new Dictionary<WebSocket, byte[]>();

        public WebsocketServer(string port)
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
            SaveData();
            Logger.Log("Saved current data to data.json.");
            Logger.Log("Shutting down the server...");
            _listener.Stop();
            Program.RequestShutdown();
        }

        /// <summary>
        /// Asynchronously listen for requests to start a websocket connection.
        /// </summary>
        public async Task ListenAsync()
        {
            try
            {
                while (_listener.IsListening)
                {
                    // Always listening for WebSocket requests
                    var context = await _listener.GetContextAsync();
                    if (context.Request.IsWebSocketRequest)
                    {
                        var webSocketContext = await context.AcceptWebSocketAsync(null);
                        await HandleWebsocketAsync(webSocketContext);
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
        /// Handle GET and POST requests from the client.
        /// </summary>
        /// <param name="context">The websocket context between server and client.</param>
        private async Task HandleWebsocketAsync(HttpListenerWebSocketContext context)
        {
            var webSocket = context.WebSocket;
            var buffer = new byte[1024 * 4];

            while (webSocket.State == WebSocketState.Open)
            {
                await _semaphore.WaitAsync();
                Interlocked.Increment(ref _activeRequests);

                try
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var parts = receivedMessage.Split(' ');

                    if (_clientKeys.ContainsKey(webSocket)) // Do if an encryption key has been shared between the server and client.
                    {
                        // Decrypt the message
                        byte[] key = _clientKeys[webSocket];
                        byte[] encryptedMessage = Convert.FromBase64String(parts[1]);
                        byte[] decryptedMessage = _encryptionHandler.Decrypt(encryptedMessage, key);

                        // Further processing based on decrypted message
                        Console.WriteLine("Decrypted Message: " + Encoding.UTF8.GetString(decryptedMessage));
                    }
                    else if (parts[0] == "secret") // Do if the client is confirming secret.
                    {
                        if (int.TryParse(parts[1], out int clientSecret) && clientSecret == ServerSecret)
                        {
                            // If the client has a matching secret, generate a new encryption key for the websocket and share with the client.
                            var key = _encryptionHandler.GenerateEncryptionKey();
                            _clientKeys[webSocket] = key;
                            await SendMessage(webSocket, Convert.ToBase64String(key));
                        }
                        else
                        {
                            await webSocket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Invalid secret", CancellationToken.None);
                        }
                    }
                }
                finally
                {
                    Interlocked.Decrement(ref _activeRequests);
                    _semaphore.Release();
                }
            }

            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        }
        
        private async Task SendMessage(WebSocket webSocket, string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
// await _semaphore.WaitAsync();
// Interlocked.Increment(ref _activeRequests);
//
// try
// {
//     var buffer = new byte[1024 * 4];
//     var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
//     var receivedMessage = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);
//     Console.WriteLine("Received: " + receivedMessage);
//
//     // Echo the message back
//     var serverMessage = "Server received: " + receivedMessage;
//     var bytes = System.Text.Encoding.UTF8.GetBytes(serverMessage);
//     await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
// }
// finally
// {
//     Interlocked.Decrement(ref _activeRequests);
//     _semaphore.Release();
//     await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
// }