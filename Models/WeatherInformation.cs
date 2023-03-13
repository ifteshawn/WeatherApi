using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace WeatherApi.Models
{
    public class WeatherInformation
    {
        [JsonPropertyName("name")]
        public string? CityName { get; set; }

        [JsonPropertyName("weather")]
        public List<Weather>? Weather { get; set; }

        [JsonPropertyName("sys")]
        public Sys? Sys { get; set; }

        [JsonPropertyName("main")]
        public Main? Main { get; set; }
    }

    public class Weather
    {
        [JsonPropertyName("main")]
        public string? Main { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("icon")]
        public string? Icon { get; set; }
    }

    public class Sys
    {
        [JsonPropertyName("country")]
        public string? CountryCode { get; set; }
    }

    public class Main
    {
        [JsonPropertyName("temp")]
        public double? Temperature { get; set; }
    }

}
