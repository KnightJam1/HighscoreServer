using System.Text;
using System.Text.Json;

namespace HighscoreClient;

class Program
{
    static async Task Main()
    {
        var client = new WebsocketClient("8080");
        await client.OpenSessionAsync();
        
        var entry = new ClientWebSocketMessage
        {
            Type = "POST",
            LeaderboardName = "gamemode1",
            Data = ["1000","Josh","2024-10-01"]
        };
        var message = JsonSerializer.Serialize(entry);
        
        await client.SendEncryptedMessageAsync(message);
        Console.WriteLine("About to close the connection.");
        await client.CloseSessionAsync();
    }
}