namespace HighscoreClient;

public struct PostResult
{
    public bool IsSuccessful {get;}
    public int Position {get;}
    public string StatusCode {get;}

    public PostResult(bool isSuccessful = false, int position = -1, string statusCode = "")
    {
        IsSuccessful = isSuccessful;
        Position = position;
        StatusCode = statusCode;
    }
}