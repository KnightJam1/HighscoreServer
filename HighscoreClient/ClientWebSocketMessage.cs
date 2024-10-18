namespace HighscoreClient;

public class ClientWebSocketMessage
{
    // Required:
    public required string Type {get; set;}
    // For POST:
    public string[]? Data {get; set;}
    // For GET:
    public string? LeaderboardName {get; set;}
    public int? NumberOfScores {get; set;}
    public int? Position {get; set;}
}