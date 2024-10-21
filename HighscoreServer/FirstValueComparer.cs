namespace HighscoreServer;

class FirstValueComparer : IComparer<string[]>
{
    public int Compare(string[]? x, string[]? y)
    {
        // Compare the first element as an integer
        if (x is not null && y is not null)
        {
            return int.Parse(x[0]).CompareTo(int.Parse(y[0]));
        }
        throw new Exception("Trying to compare null value.");
    }
}