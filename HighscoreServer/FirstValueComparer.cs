namespace HighscoreServer;

class FirstValueComparer : IComparer<string[]>
{
    public int Compare(string[] x, string[] y)
    {
        // Compare the first element as an integer
        return int.Parse(x[0]).CompareTo(int.Parse(y[0]));
    }
}