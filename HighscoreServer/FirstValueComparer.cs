namespace HighscoreServer;

class FirstValueComparer : IComparer<string[]>
{
    public int Compare(string[]? x, string[]? y)
    {
        // Compare the first element as an integer
        if (x is not null && y is not null)
        {
            return int.Parse(y[0]).CompareTo(int.Parse(x[0])); // y first makes the list maintain itself in descending order, so the best highscore is first.
        }
        throw new Exception("Trying to compare null value.");
    }
}