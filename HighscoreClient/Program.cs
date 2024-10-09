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
            // Add key-value pairs
            var newEntry = new Dictionary<string, int>
            {
                { "key1", 1 },
                { "key2", 2 }
            };
            var jsonContent = new StringContent(JsonSerializer.Serialize(newEntry), Encoding.UTF8, "application/json");
            HttpResponseMessage postResponse = await client.PostAsync("http://localhost:8080/", jsonContent);
            postResponse.EnsureSuccessStatusCode();

            // Get the updated dictionary
            HttpResponseMessage getResponse = await client.GetAsync("http://localhost:8080/");
            getResponse.EnsureSuccessStatusCode();
            string responseBody = await getResponse.Content.ReadAsStringAsync();
            Console.WriteLine(responseBody);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("Request error: {0}", e.Message);
        }
    }
}