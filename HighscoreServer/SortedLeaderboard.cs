namespace HighscoreServer;

/// <summary>
/// A leaderboard with a list of entries, a format and a set of types to match the format.
/// The format and datatypes govern if a new entry can be added to the leaderboard so there is no mismatch in information.
/// </summary>
public class SortedLeaderboard
{
    // These must remain public for serialization to function properly.
    // Do not make private.
    public SortedSet<string[]> Entries { get; set; } = new SortedSet<string[]>(new FirstValueComparer());
    public List<string> Format { get; private set; }
    public List<string> DataTypeNames { get; set; }
    public int MaxEntries { get; set; }

    public SortedLeaderboard(List<string> format, List<string> dataTypeNames, int maxEntries)
    {
        Format = format;
        DataTypeNames = dataTypeNames;
        MaxEntries = maxEntries;
    }

    public bool AddEntry(string[] entry)
    {
        if (entry.Length != Format.Count)
        {
            throw new ArgumentException($"Invalid entry format. Expected {Format.Count} values: {string.Join(", ", Format)}");
        }

        for (int i = 0; i < entry.Length; i++)
        {
            if (!ValidateType(entry[i], DataTypeNames[i]))
            {
                throw new ArgumentException($"Invalid type for '{Format[i]}'. Expected {DataTypeNames[i]}.");
            }
        }

        if (Entries.Count >= MaxEntries)
        {
            if (new FirstValueComparer().Compare(entry, Entries.Min) > 0)
            {
                Entries.Remove(Entries.Min);
                Entries.Add(entry);
                return true;
            }
            else
            {
                //Inform the client that no entry was added.
                return false;
            }
        }
        else
        {
            Entries.Add(entry);
            return true;
        }
    }

    private bool ValidateType(string value, string typeName)
    {
        try
        {
            if (typeName == "int")
            {
                int.Parse(value);
            }
            else if (typeName == "datetime")
            {
                DateTime.Parse(value);
            }
            // Add other type checks as needed
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public List<string[]> GetTopN( int n)
    {
        var topN = new List<string[]>();
        foreach (var entry in Entries.Reverse())
        {
            if (topN.Count >= n)
            {
                break;
            }
            topN.Add(entry);
        }
        return topN;
    }

    private void RemoveLowest()
    {
        Entries.Remove(Entries.Min);
    }
}