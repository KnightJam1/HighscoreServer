using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ServerSystem
{
    public class Server : IServer
    {
        private HttpListener listener;
        private CancellationTokenSource cts;
        private Game data;

        public Server(HttpListener listener, CancellationTokenSource cts, Game data)
        {
            this.listener = listener;
            this.cts = cts;
            this.data = data;
        }

        public void UpdateData(Game newData)
        {
            data = newData;
        }

        public async Task ListenAsync()
        {
            try
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    var context = await listener.GetContextAsync();
                    _ = Task.Run(() => HandleRequest(context));
                }
            }
            catch (HttpListenerException) when (cts.Token.IsCancellationRequested)
            {
                // Expected exception when listener is stopped
            }
        }

        public async Task HandleRequest(HttpListenerContext context)
        {
            if (context.Request.HttpMethod == "POST")
            {
                using (var reader = new StreamReader(context.Request.InputStream))
                {
                    string requestBody = await reader.ReadToEndAsync();
                    var entry = JsonSerializer.Deserialize<KeyValuePair<string, string[]>>(requestBody);

                    if (data.AddEntry(entry.Key, entry.Value, out string message))
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        var response = JsonSerializer.Serialize(new { Message = message });
                        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(response);
                        context.Response.ContentLength64 = buffer.Length;
                        await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        var response = JsonSerializer.Serialize(new { Error = message });
                        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(response);
                        context.Response.ContentLength64 = buffer.Length;
                        await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                    }
                }
            }
            else if (context.Request.HttpMethod == "GET")
            {
                var responseString = JsonSerializer.Serialize(data.GetFormats());
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                context.Response.ContentLength64 = buffer.Length;

                using (var output = context.Response.OutputStream)
                {
                    await output.WriteAsync(buffer, 0, buffer.Length);
                }
            }
        }
    }
}