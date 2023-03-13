using WeatherApi.Models;

namespace WeatherApi.Services
{
    public interface IWeatherInformationService
    {
        Task<WeatherInformation?> GetWeatherInfoAsync(string city, string country);
    }
}
