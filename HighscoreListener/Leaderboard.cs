using System;
using System.Collections.Generic;

public class Leaderboard
{
    public List<string[]> Entries { get; set; } = new List<string[]>();
    public List<string> Format { get; private set; }

    public Leaderboard(List<string> format)
    {
        Format = format;
    }

    public bool AddEntry(string[] entry, out string message)
    {
        if (entry.Length == Format.Count)
        {
            Entries.Add(entry);
            message = "Entry added successfully.";
            return true;
        }
        else
        {
            message = $"Invalid entry format. Expected {Format.Count} values: {string.Join(", ", Format)}";
            return false;
        }
    }
}