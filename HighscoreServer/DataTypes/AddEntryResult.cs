namespace HighscoreServer.DataTypes;

public struct AddEntryResult
{
    public bool IsSuccessful { get; }
    public int Position { get; }
    public string Status { get; }

    public AddEntryResult(bool isSuccessful = false, int position = -1, string status = "")
    {
        IsSuccessful = isSuccessful;
        Position = position;
        Status = status;
    }
}