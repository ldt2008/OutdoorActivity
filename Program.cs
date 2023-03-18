
using System.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace OutdoorActivity;
internal class Program
{
    static async Task Main(string[] args)

    {
        // Get the app settings from app.config
        var appSettings = ConfigurationManager.AppSettings;
        var weatherApi = appSettings["BaseAPI"];
        var accessKey = appSettings["AccessKey"];
        //Prompt zipcode
        Console.Write("Enter zipcode:");
        var zipcode = Console.ReadLine();

        //Call weather api
        StringBuilder apiUrl = new StringBuilder();
        apiUrl.Append(weatherApi);
        apiUrl.Append("?access_key=");
        apiUrl.Append(accessKey);
        apiUrl.Append("&query=");
        apiUrl.Append(zipcode);

        var result = await GetWeather(apiUrl.ToString());

        if(result != null && result.current != null){
            var current = result.current;
            var isRaining = current.weather_descriptions.Exists(s=> s.Equals("Raining", StringComparison.OrdinalIgnoreCase));
            Console.WriteLine("- Should I go outside?");
            if(isRaining){
                Console.WriteLine("No, you should not go outside. It's raining.");
            }
            else{
                Console.WriteLine("Yes, you can go outside. It's not raining.");
            }

            Console.WriteLine("- Should I wear sunscreen?");
            if(current.uv_index > 3){
                Console.WriteLine($"Yes, you should wear sunscreen. The UV index is high.");
            }
            else{
                Console.WriteLine($"No, you don't need to wear sunscreen. The UV index is low.");
            }

            Console.WriteLine("- Can I fly my kite?");
            if(!isRaining && current.wind_speed > 15)
            {
                Console.WriteLine("Yes, you can fly your kite. It's not raining and windy enough.");
            }
            else{
                Console.WriteLine("No, you cannot fly your kite. It's either raining or not windy enough.");
            }
        }
        else{
            Console.WriteLine("Zip code doesn't exist!");
        }
    }

    static async Task<Weather> GetWeather(string url){
        var client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await client.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();
        var serializer = JsonSerializer.Deserialize<Weather>(content);
        return serializer;
    }
}

public class Request
{
    public string type { get; set; }
    public string query { get; set; }
    public string language { get; set; }
    public string unit { get; set; }
}

public class Location
{
    public string name { get; set; }
    public string country { get; set; }
    public string region { get; set; }
    public string lat { get; set; }
    public string lon { get; set; }
    public string timezone_id { get; set; }
    public string localtime { get; set; }
    public int localtime_epoch { get; set; }
    public string utc_offset { get; set; }
}

public class Current
{
    public string observation_time { get; set; }
    public int temperature { get; set; }
    public int weather_code { get; set; }
    public List<string> weather_icons { get; set; }
    public List<string> weather_descriptions { get; set; }
    public int wind_speed { get; set; }
    public int wind_degree { get; set; }
    public string wind_dir { get; set; }
    public int pressure { get; set; }
    public int precip { get; set; }
    public int humidity { get; set; }
    public int cloudcover { get; set; }
    public int feelslike { get; set; }
    public int uv_index { get; set; }
    public int visibility { get; set; }
}

public class Weather
{
    public Request request { get; set; }
    public Location location { get; set; }
    public Current current { get; set; }
}
