using System;
using System.Collections.Generic;

public class Leaderboard
{
    public List<string[]> Entries { get; set; } = new List<string[]>();
    public List<string> Format { get; private set; }
    public List<string> DataTypeNames { get; private set; }

    public Leaderboard(List<string> format, List<string> dataTypeNames)
    {
        Format = format;
        DataTypeNames = dataTypeNames;
    }

    public bool AddEntry(string[] entry, out string message)
    {
        if (entry.Length != Format.Count)
        {
            message = $"Invalid entry format. Expected {Format.Count} values: {string.Join(", ", Format)}";
            return false;
        }

        for (int i = 0; i < entry.Length; i++)
        {
            if (!ValidateType(entry[i], DataTypeNames[i]))
            {
                message = $"Invalid type for '{Format[i]}'. Expected {DataTypeNames[i]}.";
                return false;
            }
        }

        Entries.Add(entry);
        message = "Entry added successfully.";
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