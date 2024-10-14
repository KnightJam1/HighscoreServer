using System.Net;
using SaveLoadSystem;

public class HelpCommand : ICommand
{
    public string Name => "help";
    public void Execute(CommandContext context, string[] args)
    {
        Console.WriteLine("Console commands:\n\tshutdown\t\t- close the server.\n\tstatus\t\t\t- see the status of the server.\n\tload filename.json\t- load the specified file.");
    }
}