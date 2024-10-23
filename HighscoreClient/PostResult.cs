namespace HighscoreClient;

public struct PostResult
{
    public bool IsSuccessful {get;}
    public string StatusCode {get;}

    public PostResult(bool isSuccessful = false, string statusCode = "")
    {
        IsSuccessful = isSuccessful;
        StatusCode = statusCode;
    }
}