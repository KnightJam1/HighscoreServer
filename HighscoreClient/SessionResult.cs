namespace HighscoreClient;

public struct SessionResult
{
    public bool IsSuccessful { get; }
    public string StatusCode { get; }

    public SessionResult(bool isSuccessful = false, string statusCode = "")
    {
        IsSuccessful = isSuccessful;
        StatusCode = statusCode;
    }
}