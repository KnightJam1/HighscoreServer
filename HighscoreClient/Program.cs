namespace HighscoreClient;

class Program
{
    static async Task Main()
    {
        // Create a Client
        var client = new HighscoreClient("8080");
        
        // Open a session
        SessionResult result = await client.OpenSessionAsync();
        
        // Do specific things depending on the status
        if (result.IsSuccessful)
        {
            Console.WriteLine("Session started successfully.");
        }
        else
        {
            Console.WriteLine($"Session failed: {result.StatusCode}");
        }

        // Can post an entry to an existing leaderboard.
        await client.PostEntry("gamemode1", ["1004", "Josh", "2024-10-22"]);
        
        // Can get a segment of the leaderboard.
        List<string[]> leaderboard = await client.GetLeaderboardScores("gamemode1", 7, 4, 5);
        
        // Display the list
        for (int i = 0; i < leaderboard.Count; i++)
        {
            Console.WriteLine($"{leaderboard[i][0]}: {leaderboard[i][1]}");
        }

        // Wait for the user. This is here to test for concurrent sessions.
        Console.Write("Press any key to continue...");
        Console.ReadKey();
        
        // Close session
        Console.WriteLine("About to close the connection.");
        await client.CloseSessionAsync();
    }
}