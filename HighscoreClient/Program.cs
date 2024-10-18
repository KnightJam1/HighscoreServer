using System.Text;
using System.Text.Json;

namespace HighscoreClient;

class Program
{
    static async Task Main()
    {
        var client = new WebsocketClient("8080");
        await client.OpenSessionAsync();
        await client.SendStringArrayAsync(["Hello, Server!"]);
        await client.ReceiveMessage();
        await client.CloseSessionAsync();
    }
}