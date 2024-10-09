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
            // Add entries
            var newEntry = new string[][]
            {
                new string[] { "Alpha", "1000", "2024-10-01" },
                new string[] { "Bravo", "1500", "2024-10-02" },
                new string[] { "Charlie", "2000", "2024-10-03" },
                new string[] { "Delta", "2500", "2024-10-04" }
            };
            var jsonContent = new StringContent(JsonSerializer.Serialize(newEntry), Encoding.UTF8, "application/json");
            HttpResponseMessage postResponse = await client.PostAsync("http://localhost:8080/", jsonContent);
            postResponse.EnsureSuccessStatusCode();

            // Get the updated data
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