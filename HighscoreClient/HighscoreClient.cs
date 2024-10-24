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

    /// <summary>
    /// Open a session with the server.
    /// </summary>
    /// <returns>
    /// Returns a SessionResult that says if the websocket successfully connected.
    /// If the connection was not successful the SessionResult will have a status code to indicate what went wrong.
    /// 200 - success
    /// 400 - bad request
    /// 403 - incorrect secret
    /// 408 - request timeout
    /// 426 - upgrade required
    /// 500 - internal server error
    /// </returns>
    public async Task<SessionResult> OpenSessionAsync()
    {
        try
        {
            await _webSocket.ConnectAsync(new Uri(_serverUrl), CancellationToken.None);
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
            return new SessionResult(statusCode: ex.Message.Split(" ")[0]);
        }
        return new SessionResult(isSuccessful: true, statusCode: "200");
    }

    /// <summary>
    /// Close the current session with the server.
    /// </summary>
    /// <returns>
    /// Returns a SessionResult that says if the websocket successfully disconnected.
    /// The SessionResult will have a status code to indicate how the session closed and if there was a problem.
    /// 200 - Successful closure
    /// 404 - No session was started, and thus cannot be closed.
    /// 410 - Session already closed by  server.
    /// 500 - Error with closure
    /// </returns>
    public async Task<SessionResult> CloseSessionAsync()
    {
        try
        {
            var getHandshake = ReceiveMessageAsync();
            var timeoutTask = Task.Delay(100);
            if (await Task.WhenAny(getHandshake, timeoutTask) == getHandshake)
            {
                return new SessionResult(isSuccessful: true,statusCode: "410"); // Session already closed by server.
            }
            if (!_isConnected)
            {
                return new SessionResult(statusCode: "404"); // No started session.
            }
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            _webSocket.Dispose();
            _isConnected = false;
            return new SessionResult(isSuccessful: true, statusCode: "200"); // Successful closure of session.
        }
        catch (Exception ex)
        {
            _webSocket.Dispose();
            _isConnected = false;
            return new SessionResult(isSuccessful: true, statusCode: "500"); // An error occured trying to close the session, but the session has been closed.
        }
    }

    /// <summary>
    /// Get a section from a leaderboard.
    /// </summary>
    /// <param name="leaderboardId">Name of the leaderboard being accessed.</param>
    /// <param name="position">The position of the main entry to look at.</param>
    /// <param name="scoresBefore">The number of positions before the entry that should be returned.</param>
    /// <param name="scoresAfter">The number of positions after the entry that should be returned.</param>
    /// <returns>
    /// Returns a GetResult struct that holds the returned section of the leaderboard, a bool saying if the get was successful and a status code to identify problems.
    /// 200 - successful get
    /// 500 - error trying to get
    /// </returns>
    public async Task<GetResult> GetLeaderboardScores(string leaderboardId, int position, int scoresBefore, int scoresAfter)
    {
        string message = $"GET {leaderboardId} {position} {scoresBefore} {scoresAfter}";
        await SendEncryptedMessageAsync(message);
        try
        {
            var scores = await ReceiveEncryptedMessageAsync();
            return new GetResult(scores:scores, isSuccessful:true, statusCode:"200");
        }
        catch (Exception ex)
        {
            return new GetResult(statusCode: "500");
        }
    }

    /// <summary>
    /// Post an entry to the specified leaderboard.
    /// </summary>
    /// <param name="leaderboardId">The leaderboard the entry should be added to.</param>
    /// <param name="entry">The entry to be added.</param>
    /// <returns>
    /// Returns a PostResult struct containing a bool for success, a bool to return if the entry qualified and a status code to identify problems.
    /// 200 - success
    /// 404 - could not find the specified leaderboard
    /// 500 - error
    /// </returns>
    public async Task<PostResult> PostEntry(string leaderboardId, string[] entry)
    {
        string message = $"POST {leaderboardId} {string.Join(" ", entry)}";
        await SendEncryptedMessageAsync(message);
        string status = await ReceiveEncryptedResponseAsync();
        if (status == "TooLow")
        {
            return new PostResult(isSuccessful:true, scoreTooLow:true, statusCode:"200"); // Success but the entry did not make it onto the leaderboard
        }
        else if (status == "200")
        {
            return new PostResult(isSuccessful:true, statusCode:"200"); // Success and the entry made it onto the leaderboard
        }
        return new PostResult(statusCode:status.Split(" ")[0]); // Faliure
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
    
    private async Task SendEncryptedMessageAsync(string data)
    {
        string jsonData = JsonSerializer.Serialize(data);
        byte[] encryptedData = _encryptionHandler.Encrypt(Encoding.UTF8.GetBytes(jsonData), _encryptionKey);

        string encryptedMessage = Convert.ToBase64String(encryptedData);
        await SendMessageAsync(encryptedMessage);
    }
    
    private async Task<List<string[]>> ReceiveEncryptedMessageAsync()
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

    private async Task<string> ReceiveEncryptedResponseAsync()
    {
        string encryptedMessage = await ReceiveMessageAsync();
        byte[] encryptedData = Convert.FromBase64String(encryptedMessage);
        byte[] decryptedData = _encryptionHandler.Decrypt(encryptedData, _encryptionKey);

        string jsonData = Encoding.UTF8.GetString(decryptedData);
        string message = JsonSerializer.Deserialize<string>(jsonData)!;
        if (message == null)
        {
            throw new InvalidOperationException("The encrypted message is empty.");
        }
        return message;
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