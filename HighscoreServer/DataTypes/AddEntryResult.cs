namespace HighscoreServer.DataTypes;

public struct AddEntryResult
{
    public bool IsSuccessful { get;}
    public string Status { get;}

    public AddEntryResult(bool isSuccessful = false, string status = "")
    {
        IsSuccessful = isSuccessful;
        Status = status;
    }
}