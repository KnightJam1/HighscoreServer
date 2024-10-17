using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace HighscoreClient;

public class Client
{
    private readonly HttpClient _highscoreClient;
    private readonly string _serverUrl;
    private ClientWebSocket _webSocket;

    // Create a client
    public Client(string port)
    {
        _webSocket = new ClientWebSocket();
        _serverUrl = $"http://localhost:{port}";
    }

    // Open a session with the server
    public async Task OpenSessionAsync()
    {
        await _webSocket.ConnectAsync(new Uri(_serverUrl), CancellationToken.None);
        Console.WriteLine("Session opened with server.");
    }

    public async Task SendStringArrayAsync(string[] data)
    {
        var jsonData = JsonSerializer.Serialize(data);
        var buffer = Encoding.UTF8.GetBytes(jsonData);
        var segment = new ArraySegment<byte>(buffer);
        await _webSocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
        Console.WriteLine("Data sent successfully.");
    }

    public async Task CloseSessionAsync()
    {
        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        _webSocket.Dispose();
        Console.WriteLine("Session closed with server.");
    }

    public string[] RequestLeaderboard(string leaderboardId, int numberOfScores, int position)
    {
        throw new NotImplementedException();
    }
}