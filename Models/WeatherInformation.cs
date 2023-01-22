namespace WeatherApi.Models
{
    public class WeatherInformation
    {
        public string name { get; set; }
        public List<Weather> weather { get; set; }
        public Main main { get; set; }

    }

    public class Weather
    {
        public string main { get; set; }
        public string description { get; set; }
    }

    public class Main
    {
        public double temp { get; set; }
    } 

}
