namespace HighscoreServer.Commands;

/// <summary>
/// Deletes a specified json file in the data directory.
/// </summary>
public class DeleteCommand : ICommand
{
    public string Name => "delete";
    public int NumArgs => 1;
    public string HelpText => "Deletes a file of saved data.";

    /// <param name="args">Name of the data to be deleted.
    /// If using a filesystem it doesn't matter if the filename has an extension or not.</param>
    /// <exception cref="ArgumentException">Throws an exception if not given an argument for the name of the file to be deleted.</exception>
    public void Execute(CommandContext context, string[] args)
    {
        // Check for a valid number of arguments.
        if (args.Length != NumArgs)
        {
            throw new ArgumentException($"Invalid number of arguments. Expected {NumArgs} but got {args.Length}.");
        }
        // Delete data.
        context.Server.DeleteData(args[0]);
    }
}