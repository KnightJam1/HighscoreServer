using System.Net;
using System.Text.Json;
using HighscoreListener.Loggers;

namespace HighscoreListener
{
    public class Server : IServer
    {
        private readonly HttpListener _listener = new HttpListener();
        // private bool _isRunning;
        private Game data;
        static LoggerTerminal logger = new LoggerTerminal();

        public Server(string webAddress, Game data)
        {
            this.data = data;
            _listener = new HttpListener();
            _listener.Prefixes.Add(webAddress);
        }

        public void UpdateData(Game newData)
        {
            data = newData;
        }
        public async Task Start()
        {
            //_isRunning = true;
            _listener.Start();
            logger.Log("Now listening..."); // Consider listing prefixes
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
                while (!Program.isShuttingDown())
                {
                    var listenContext = await _listener.GetContextAsync();
                    _ = Task.Run(() => HandleRequest(listenContext));
                }
            }
            catch (HttpListenerException) when (!Program.isShuttingDown())
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