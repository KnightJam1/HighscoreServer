using System;
using System.Collections.Generic;

public class Game
{
    public Dictionary<string, Leaderboard> Leaderboards { get; set; } = new Dictionary<string, Leaderboard>();
    
    public void AddToLeaderboard(string gameMode, string[] entry)
    {
        if (!Leaderboards.ContainsKey(gameMode))
        {
            Leaderboards[gameMode] = new Leaderboard();
        }
        Leaderboards[gameMode].Entries.Add(entry);
    }
}
    