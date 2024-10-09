using System;
using System.Collections.Generic;

public class Game
{
    public Dictionary<string, Leaderboard> Leaderboards { get; set; } = new Dictionary<string, Leaderboard>();

    public void AddLeaderboard(string name, List<string> format)
    {
        if (!Leaderboards.ContainsKey(name))
        {
            Leaderboards[name] = new Leaderboard(format);
        }
    }

    public bool AddEntry(string gameMode, string[] entry, out string message)
    {
        if (Leaderboards.ContainsKey(gameMode))
        {
            return Leaderboards[gameMode].AddEntry(entry, out message);
        }
        else
        {
            message = $"Leaderboard for game mode '{gameMode}' does not exist.";
            return false;
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
}