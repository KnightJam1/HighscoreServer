namespace HighscoreListener.Commands;

public class ShutdownCommand : ICommand
{
    public string Name => "shutdown";
    
    // public ShutdownCommand(HttpListener listener, IDataService dataService, string defaultFileName, Game data)
    // {
    //     this.listener = listener; // listener may be moved to Server class. When it is moved, this should become unneeded.

    //     this.dataService = dataService;
    //     this.defaultFileName = defaultFileName;
    //     this.data = data;
    // } 
    public void Execute(CommandContext context, string[] args) //Pass in context
    {
        //listener.Stop(); // These two should be replaced with something like server.stop(). The server should manage the cts and listener.
        Program.RequestShutdown();

        context.dataService.Save(context.defaultFileName, context.data);
        // If cts is removed, I need to return a flag or something to signify that the loop should
    }
}