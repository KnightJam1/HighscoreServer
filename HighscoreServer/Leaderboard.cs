namespace HighscoreServer;

/// <summary>
/// A leaderboard with a list of entries, a format and a set of types to match the format.
/// The format and datatypes govern if a new entry can be added to the leaderboard so there is no mismatch in information.
/// </summary>
public class Leaderboard
{
    public List<string[]> Entries { get; set; } = new List<string[]>();
    public List<string> Format { get; private set; }
    public List<string> DataTypeNames { get; set; }

    public Leaderboard(List<string> format, List<string> dataTypeNames)
    {
        Format = format;
        DataTypeNames = dataTypeNames;
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

        Entries.Add(entry);
        return true;
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
}