using System.Net;

namespace HighscoreListener
{
    /// <summary>
    /// Interface for servers.
    /// </summary>
    public interface IServer
    {
        //Listen
        Task ListenAsync();

        //Handle request
        Task HandleRequest(HttpListenerContext context);

    }
}