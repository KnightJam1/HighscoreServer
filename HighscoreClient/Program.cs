using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        using HttpClient client = new HttpClient();
        try
        {
            // Query for formats
            HttpResponseMessage getFormatsResponse = await client.GetAsync("http://localhost:8080/");
            getFormatsResponse.EnsureSuccessStatusCode();
            string formatsResponseBody = await getFormatsResponse.Content.ReadAsStringAsync();
            Console.WriteLine("Formats: " + formatsResponseBody);

            // Add a single entry
            var entry = new KeyValuePair<string, string[]>("gamemode1", new string[] { "Josh", "1000", "2024-10-01"});
            var jsonContent = new StringContent(JsonSerializer.Serialize(entry), Encoding.UTF8, "application/json");
            HttpResponseMessage postResponse = await client.PostAsync("http://localhost:8080/", jsonContent);
            postResponse.EnsureSuccessStatusCode();
            string postResponseBody = await postResponse.Content.ReadAsStringAsync();
            Console.WriteLine("Post Response: " + postResponseBody);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("Request error: {0}", e.Message);
        }
    }
}