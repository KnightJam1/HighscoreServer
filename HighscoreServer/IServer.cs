using System.Net;

namespace HighscoreServer
{
    /// <summary>
    /// Interface for servers.
    /// </summary>
    public interface IServer
    {
        //Listen
        Task ListenAsync();

        //Handle request
        //Task HandleRequest(HttpListenerContext context);

    }
}