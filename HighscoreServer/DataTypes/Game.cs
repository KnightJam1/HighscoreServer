namespace HighscoreServer.DataTypes;

/// <summary>
/// A data type that represents a dictionary of leaderboards, keyed by a string representing the name of the leaderboard.
/// </summary>
public class Game
{
    public Dictionary<string, SortedLeaderboard> Leaderboards { get; init; } = new Dictionary<string, SortedLeaderboard>();

    public void AddLeaderboard(string name, List<string> format, List<string> dataTypeNames, int maxEntries)
    {
        if (!Leaderboards.ContainsKey(name))
        {
            Leaderboards[name] = new SortedLeaderboard(format, dataTypeNames, maxEntries);
        }
        // else throw an exception.
    }

    public bool AddEntry(string gameMode, string[] entry)
    {
        if (Leaderboards.ContainsKey(gameMode))
        {
            return Leaderboards[gameMode].AddEntry(entry);
        }
        else
        {
            throw new Exception($"Leaderboard for game mode '{gameMode}' does not exist.");
        }
    }

    public Dictionary<string, List<string>> GetFormats()
    {
        var formats = new Dictionary<string, List<string>>();
        foreach (var leaderboard in Leaderboards)
        {
            formats[leaderboard.Key] = leaderboard.Value.Format;
        }
        return formats;
    }

    public List<string[]> GetTopNFromLeaderboard(string leaderboardName, int topN)
    {
        return Leaderboards[leaderboardName].GetTopN(topN);
    }
}