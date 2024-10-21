using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace HighscoreClient;

public class WebsocketClient
{
    //private readonly HttpClient _highscoreClient;
    private readonly string _serverUrl;
    private readonly EncryptionHandler _encryptionHandler;
    private ClientWebSocket _webSocket;
    private bool _isConnected;
    
    // Secret Int here. Must change. Make sure that the secret is the same for client and server.
    private const int Secret = 1;
    private byte[] _encryptionKey;

    // Create a client
    public WebsocketClient(string port)
    {
        _encryptionHandler = new EncryptionHandler();
        _webSocket = new ClientWebSocket();
        _serverUrl = $"ws://localhost:{port}";
        _isConnected = false;
        //_highscoreClient = new HttpClient();
    }

    // Open a session with the server
    public async Task OpenSessionAsync()
    {
        await _webSocket.ConnectAsync(new Uri(_serverUrl), CancellationToken.None);
        Console.WriteLine("Session opened with server.");
        _isConnected = true;

        // Send secret to the server
        await SendMessageAsync($"secret {Secret.ToString()}");
        
        // Recieve encryption key from the server
        string keyString = await ReceiveMessageAsync();
        _encryptionKey = Convert.FromBase64String(keyString);
        Console.WriteLine($"Encryption key received from server");
    }

    public async Task SendStringArrayAsync(string[] data)
    {
        var message = new ClientWebSocketMessage
        {
            Type = "POST",
            Data = data
        };
        var jsonMessage = JsonSerializer.Serialize(message);
        var buffer = Encoding.UTF8.GetBytes(jsonMessage);
        var segment = new ArraySegment<byte>(buffer);
        await _webSocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
        Console.WriteLine("Data sent successfully.");
    }

    public async Task CloseSessionAsync()
    {
        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        _webSocket.Dispose();
        Console.WriteLine("Session closed with server.");
        _isConnected = true;
    }

    public async Task RequestLeaderboard(string leaderboardId, int numberOfScores, int position)
    {
        var message = new ClientWebSocketMessage
        {
            Type = "GET",
            LeaderboardName = leaderboardId,
            NumberOfScores = numberOfScores,
            Position = position
        };
        var jsonMessage = JsonSerializer.Serialize(message);
        var buffer = Encoding.UTF8.GetBytes(jsonMessage);
        var segment = new ArraySegment<byte>(buffer);
        await _webSocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
        Console.WriteLine("Data sent successfully.");
    }
    
    private async Task SendMessageAsync(string message)
    {
        var buffer = Encoding.UTF8.GetBytes(message);
        await _webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
    }
    
    private async Task<string> ReceiveMessageAsync()
    {
        var buffer = new byte[1024 * 4];
        var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        return Encoding.UTF8.GetString(buffer, 0, result.Count);
    }
    
    public async Task SendEncryptedMessageAsync(string data)
    {
        string jsonData = JsonSerializer.Serialize(data);
        byte[] encryptedData = _encryptionHandler.Encrypt(Encoding.UTF8.GetBytes(jsonData), _encryptionKey);

        string encryptedMessage = Convert.ToBase64String(encryptedData);
        await SendMessageAsync(encryptedMessage);
    }
    
    public async Task<string[]> ReceiveEncryptedMessageAsync()
    {
        string encryptedMessage = await ReceiveMessageAsync();
        byte[] encryptedData = Convert.FromBase64String(encryptedMessage);
        byte[] decryptedData = _encryptionHandler.Decrypt(encryptedData, _encryptionKey);

        string jsonData = Encoding.UTF8.GetString(decryptedData);
        return JsonSerializer.Deserialize<string[]>(jsonData);
    }
}