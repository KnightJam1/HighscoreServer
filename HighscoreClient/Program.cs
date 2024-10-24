using System.Net;

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
        PostResult postResult = await client.PostEntry("gamemode1", ["8000", "Josh", "2024-10-22"]);
        if (postResult.IsSuccessful)
        {
            Console.WriteLine("Successfully sent an entry.");
        }
        else
        {
            Console.WriteLine($"Failed to send an entry: {postResult.StatusCode}");
        }
        
        // Can get a segment of the leaderboard.
        GetResult getResult = await client.GetLeaderboardScores("gamemode1", 7, 4, 5);
        
        // Display the list
        if (getResult.IsSuccessful)
        {
            List<string[]> leaderboard = getResult.Scores;
            
            for (int i = 0; i < leaderboard.Count; i++)
            {
                Console.WriteLine($"{leaderboard[i][0]}: {leaderboard[i][1]}");
            }
        }
        else
        {
            Console.WriteLine($"Get leaderboard failed: {getResult.StatusCode}");
        }

        // Wait for the user. This is here to test for concurrent sessions.
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
        Console.WriteLine();
        
        // Close session
        Console.WriteLine("About to close the connection.");
        SessionResult closeResult = await client.CloseSessionAsync();
        if (closeResult.IsSuccessful)
        {
            Console.WriteLine("Session closed successfully.");
            if (closeResult.StatusCode == "500")
            {
                Console.WriteLine("There was a problem somewhere, but the session closed anyway.");
            }
        }
        else
        {
            Console.WriteLine($"Session failed: {closeResult.StatusCode}");
        }
    }
}