using System.Net;

namespace HighscoreListener
{
    public interface IServer
    {
        //Listen
        Task ListenAsync();

        //Handle request
        Task HandleRequest(HttpListenerContext context);

    }
}