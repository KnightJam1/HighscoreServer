using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace HighscoreClient;

public class HighscoreClient
{
    private readonly string _serverUrl;
    private readonly EncryptionHandler _encryptionHandler;
    private readonly ClientWebSocket _webSocket;
    private bool _isConnected;
    
    // Secret Int here. Must change. Make sure that the secret is the same for client and server.
    private const int Secret = 1;
    private byte[] _encryptionKey = Array.Empty<byte>();

    // Create a client
    public HighscoreClient(string port)
    {
        _encryptionHandler = new EncryptionHandler();
        _webSocket = new ClientWebSocket();
        _serverUrl = $"ws://localhost:{port}";
        _isConnected = false;
    }

    // Open a session with the server
    public async Task<SessionResult> OpenSessionAsync()
    {
        try
        {
            await _webSocket.ConnectAsync(new Uri(_serverUrl), CancellationToken.None);
            Console.WriteLine("Session opened with server.");
            _isConnected = true;
        }
        catch (WebSocketException ex)
        {
            return new SessionResult(statusCode: WebStatusMap(ex));
        }

        // Send secret to the server
        await SendMessageAsync($"secret {Secret.ToString()}");
        
        // Recieve encryption key from the server
        try
        {
            string keyString = await ReceiveMessageAsync();
            _encryptionKey = Convert.FromBase64String(keyString);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return new SessionResult(statusCode: ex.Message.Split(" ")[0]);
        }
        // Console.WriteLine($"Encryption key received from server");
        return new SessionResult(isSuccessful: true, statusCode: "200");
    }

    public async Task CloseSessionAsync()
    {
        try
        {
            var getHandshake = ReceiveMessageAsync();
            var timeoutTask = Task.Delay(100);
            if (await Task.WhenAny(getHandshake, timeoutTask) == getHandshake)
            {
                throw new InvalidOperationException("The websocket is already closed.");
            }
            if (!_isConnected)
            {
                throw new InvalidOperationException("The websocket is not connected.");
            }
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            _webSocket.Dispose();
            Console.WriteLine("Session closed with server.");
            _isConnected = false;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            _webSocket.Dispose();
            _isConnected = false;
        }
    }

    public async Task<List<string[]>> GetLeaderboardScores(string leaderboardId, int position, int scoresBefore, int scoresAfter)
    {
        string message = $"GET {leaderboardId} {position} {scoresBefore} {scoresAfter}";
        var jsonMessage = JsonSerializer.Serialize(message);
        await SendEncryptedMessageAsync(message);
        return await ReceiveEncryptedMessageAsync();
    }

    public async Task PostEntry(string leaderboardId, string[] entry)
    {
        string message = $"POST {leaderboardId} {string.Join(" ", entry)}";
        var jsonMessage = JsonSerializer.Serialize(message);
        await SendEncryptedMessageAsync(message);
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
        if (result.MessageType == WebSocketMessageType.Close)
        {
            throw new InvalidOperationException(result.CloseStatusDescription);
        }
        return Encoding.UTF8.GetString(buffer, 0, result.Count);
    }
    
    public async Task SendEncryptedMessageAsync(string data)
    {
        string jsonData = JsonSerializer.Serialize(data);
        byte[] encryptedData = _encryptionHandler.Encrypt(Encoding.UTF8.GetBytes(jsonData), _encryptionKey);

        string encryptedMessage = Convert.ToBase64String(encryptedData);
        await SendMessageAsync(encryptedMessage);
    }
    
    public async Task<List<string[]>> ReceiveEncryptedMessageAsync()
    {
        string encryptedMessage = await ReceiveMessageAsync();
        byte[] encryptedData = Convert.FromBase64String(encryptedMessage);
        byte[] decryptedData = _encryptionHandler.Decrypt(encryptedData, _encryptionKey);

        string jsonData = Encoding.UTF8.GetString(decryptedData);
        List<string[]> data = JsonSerializer.Deserialize<List<string[]>>(jsonData)!;
        if (data == null)
        {
            throw new InvalidOperationException("The encrypted message is empty.");
        }
        return data;
    }

    private string WebStatusMap(WebSocketException ex)
    {
        string status;
        switch (ex.WebSocketErrorCode)
        {
            case WebSocketError.InvalidMessageType:
            case WebSocketError.HeaderError:
            case WebSocketError.InvalidState:
            case WebSocketError.NotAWebSocket:
                status = "400"; // Bad Request
                break;
            case WebSocketError.ConnectionClosedPrematurely:
                status = "408"; // Request Timeout
                break;
            case WebSocketError.UnsupportedProtocol:
                status = "426"; // Upgrade Required
                break;
            default:
                status = "500"; // Internal Server Error
                break;
        }
        return status;
    }
}