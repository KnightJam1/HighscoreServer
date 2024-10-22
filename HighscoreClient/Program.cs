using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace HighscoreClient;

class Program
{
    static async Task Main()
    {
        var client = new HighscoreClient("8080");
        SessionResult result = await client.OpenSessionAsync();
        if (result.IsSuccessful)
        {
            Console.WriteLine("Session started successfully.");
        }
        else
        {
            Console.WriteLine($"Session failed: {result.StatusCode}");
        }
        
        var entry = new ClientWebSocketMessage
        {
            Type = "POST",
            LeaderboardName = "gamemode1",
            Data = ["1000","Josh","2024-10-01"]
        };
        var message = JsonSerializer.Serialize(entry);
        
        await client.SendEncryptedMessageAsync(message);

        var get10Request = new ClientWebSocketMessage
        {
            Type = "GET",
            LeaderboardName = "gamemode1",
            ScoresBefore = 4,
            ScoresAfter = 5,
            Position = 20
        };
        message = JsonSerializer.Serialize(get10Request);
        
        await client.SendEncryptedMessageAsync(message);
        List<string[]> leaderboard = await client.ReceiveEncryptedMessageAsync();
        
        for (int i = 0; i < leaderboard.Count; i++)
        {
            Console.WriteLine($"{leaderboard[i][0]}: {leaderboard[i][1]}");
        }

        Console.ReadLine();
        
        Console.WriteLine("About to close the connection.");
        await client.CloseSessionAsync();
    }
}