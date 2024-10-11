using System.Net;
using SaveLoadSystem;

public class ShutdownCommand : ICommand
{
    public string Name => "shutdown";
    private CancellationTokenSource cts;
    private HttpListener listener;
    private IDataService dataService;
    private string defaultFileName;
    private Game data;
    public ShutdownCommand(CancellationTokenSource cts, HttpListener listener, IDataService dataService, string defaultFileName, Game data)
    {
        this.cts = cts; // cts only really necessary here, remove from main code. Possibly the cts should be in Server?
        this.listener = listener; // listener may be moved to Server class. When it is moved, this should become unneeded.

        this.dataService = dataService;
        this.defaultFileName = defaultFileName;
        this.data = data;
    } 
    public void Execute()
    {
        cts.Cancel();
        listener.Stop(); // These two should be replaced with something like server.stop(). The server should manage the cts and listener.

        dataService.Save(defaultFileName, data);
        // If cts is removed, I need to return a flag or something to signify that the loop should
    }
}