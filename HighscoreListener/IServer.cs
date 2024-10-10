using System;
using System.Collections.Generic;
using System.Net;

namespace ServerSystem
{
    public interface IServer
    {
        //Listen
        Task ListenAsync();

        //Handle request
        Task HandleRequest(HttpListenerContext context);

    }
}