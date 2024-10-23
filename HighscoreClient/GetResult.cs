namespace HighscoreClient;

public struct GetResult
{
    public List<string[]> Scores { get;}
    public bool IsSuccessful { get;}
    public string StatusCode { get;}

    public GetResult(List<string[]>? scores = null, bool isSuccessful = false, string statusCode = "")
    {
        Scores = scores ?? [];
        IsSuccessful = isSuccessful;
        StatusCode = statusCode;
    }
}