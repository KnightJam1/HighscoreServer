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

    public AddEntryResult AddEntry(string gameMode, string[] entry)
    {
        if (Leaderboards.ContainsKey(gameMode))
        {
            try
            {
                int position = Leaderboards[gameMode].AddEntry(entry);
                return new AddEntryResult(isSuccessful: true, position: position, status:"200");
            }
            catch (Exception ex)
            {
                return new AddEntryResult(status:$"500 {ex.Message}");
            }
        }
        else
        {
            //throw new Exception($"Leaderboard for game mode '{gameMode}' does not exist.");
            return new AddEntryResult(status:"404 Leaderboard not found.");
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

    public List<string[]> GetTopNFromLeaderboard(string leaderboardName, int position, int scoresBefore, int scoresAfter)
    {
        return Leaderboards[leaderboardName].GetTopN(position, scoresBefore, scoresAfter);
    }
}