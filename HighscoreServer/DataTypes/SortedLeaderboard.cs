using System.Diagnostics;

namespace HighscoreServer.DataTypes;

/// <summary>
/// A leaderboard with a list of entries, a format and a set of types to match the format.
/// The format and datatypes govern if a new entry can be added to the leaderboard so there is no mismatch in information.
/// </summary>
public class SortedLeaderboard
{
    // These must remain public for serialization to function properly.
    // Do not make private.
    public List<string[]> Entries { get; set; } = new List<string[]>();
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

        int index = Entries.BinarySearch(entry, new FirstValueComparer());
        if (index < 0) index = ~index; // Get the insertion point
        Entries.Insert(index, entry);

        if (Entries.Count > MaxEntries)
        {
            Entries.RemoveAt(0); // Remove the lowest
        }
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
    
    /// <summary>
    /// Returns a specified set of entries from the leaderboard.
    /// If before or after are larger than the available entries, this will still try to return the total number of entries by taking more from the other end.
    /// If before and after are both larger, then return as many as possible.
    /// </summary>
    /// <param name="pos">Specified 'centre' position.</param>
    /// <param name="before">Number of items before the specified position.</param>
    /// <param name="after">Number of items after.</param>
    /// <returns>List of entries.</returns>
    public List<string[]> GetTopN( int pos, int before, int after)
    {
        int startIndex = Math.Max(pos - before, 0); //Start with the items specified before the position, or 0
        int endIndex = Math.Min(pos + after, Entries.Count - 1);

        if (pos - before < 0) // If there are more items requested before the given position than there actually are, try to get more from after.
        {
            endIndex = Math.Min(pos + after - (pos - before), Entries.Count - 1);
        }

        if (pos + after > Entries.Count - 1) // If there are more items requested after the given position than there actually are, try to get more from before.
        {
            startIndex = Math.Max(pos - before - (pos + after - (Entries.Count - 1)), 0);
        }
        
        Debug.Assert(endIndex - startIndex + 1 <= before + 1 + after,$"Trying to send back more items than requested."); //Assert that there are never more items than requested.
        
        return Entries.GetRange(startIndex, endIndex - startIndex + 1);
    }
}