using System.Net;
using System.Text.Json;
using HighscoreListener.Loggers;

namespace HighscoreListener
{
    public class Server : IServer
    {
        private readonly HttpListener _listener;
        // private bool _isRunning;
        private Game _data;
        static readonly LoggerTerminal Logger = new LoggerTerminal();

        public Server(string webAddress, Game data)
        {
            this._data = data;
            _listener = new HttpListener();
            _listener.Prefixes.Add(webAddress);
        }

        public void UpdateData(Game newData)
        {
            _data = newData;
        }
        public async Task Start()
        {
            //_isRunning = true;
            _listener.Start();
            Logger.Log("Now Listening...\nType 'shutdown' to stop the server. Type 'help' to see a list of commands"); // Consider listing prefixes
            await ListenAsync();
        }

        public void Stop()
        {
            //_isRunning = false;
            _listener.Stop();
        }

        public async Task ListenAsync()
        {
            try
            {
                while (!Program.IsShuttingDown())
                {
                    var listenContext = await _listener.GetContextAsync();
                    _ = Task.Run(() => HandleRequest(listenContext));
                }
            }
            catch (HttpListenerException) when (!Program.IsShuttingDown())
            {
                // Expected exception when listener is stopped
            }
        }

        public async Task HandleRequest(HttpListenerContext context)
        {
            switch (context.Request.HttpMethod)
            {
                case "POST":
                {
                    using var reader = new StreamReader(context.Request.InputStream);
                    var requestBody = await reader.ReadToEndAsync();
                    var entry = JsonSerializer.Deserialize<KeyValuePair<string, string[]>>(requestBody);

                    if (_data.AddEntry(entry.Key, entry.Value, out string message))
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

                    break;
                }
                case "GET":
                {
                    var responseString = JsonSerializer.Serialize(_data.GetFormats());
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                    context.Response.ContentLength64 = buffer.Length;

                    using (var output = context.Response.OutputStream)
                    {
                        await output.WriteAsync(buffer, 0, buffer.Length);
                    }

                    break;
                }
            }
        }
    }
}