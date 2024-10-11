using System.Net;
using SaveLoadSystem;

public class StatusCommand : ICommand
{
    public string Name => "status";
    public void Execute()
    {
        Console.WriteLine("The server is running.");
    }
}