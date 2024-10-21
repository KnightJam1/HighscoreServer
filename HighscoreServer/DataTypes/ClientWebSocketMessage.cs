namespace HighscoreServer.DataTypes;

public class ClientWebSocketMessage
{
    // Required:
    public required string Type {get; set;} = string.Empty;
    // For POST:
    public string[] Data {get; set;} = Array.Empty<string>();
    // For GET:
    public string LeaderboardName {get; set;} = string.Empty;
    //public int NumberOfScores { get; set; }
    public int ScoresBefore {get; set;} = 0;
    public int ScoresAfter {get; set;} = 0;
    public int Position { get; set; }
}