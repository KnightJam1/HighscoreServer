namespace HighscoreClient;

public struct PostResult
{
    public bool IsSuccessful {get;}
    public bool ScoreTooLow {get;}
    public string StatusCode {get;}

    public PostResult(bool isSuccessful = false, bool scoreTooLow = false, string statusCode = "")
    {
        IsSuccessful = isSuccessful;
        ScoreTooLow = scoreTooLow;
        StatusCode = statusCode;
    }
}