using System.Globalization;
using System.Text.Json;
using WeatherApi.Models;

namespace WeatherApi.Services
{
    public class WeatherInformationService : IWeatherInformationService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ILogger<IWeatherInformationService> _logger;

        public WeatherInformationService(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<IWeatherInformationService> logger)
        {
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient("WeatherApi");
            _logger = logger;
        }

        public async Task<WeatherInformation?> GetWeatherInfoAsync(string city, string country)
        {
            var openWeatherApiKey = _configuration.GetValue<string>("APIKey");

            if(string.IsNullOrEmpty(openWeatherApiKey))
            {
                throw new ArgumentNullException("APIKey", "The OpenWeather API key is missing");
            }

            var url = $"weather?q={city},{country}&appid={openWeatherApiKey}&units=metric";
            _logger.LogDebug("The retrieved API key from appSettings is: {API Key}", openWeatherApiKey);
            
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                throw new WeatherInformationServiceException(response.StatusCode, $"Error retrieving weather information for {city}: {response.StatusCode}");
            }
            var stringResult = await response.Content.ReadAsStringAsync();
            var rawWeatherData = JsonSerializer.Deserialize<WeatherInformation>(stringResult);

            var weather = rawWeatherData?.Weather?.FirstOrDefault();
            if (weather != null)
            {
                if (weather.Description != null)
                {
                    weather.Description = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(weather.Description.ToLower());
                }
                if (weather.Icon != null)
                {
                    weather.IconSource = $"http://openweathermap.org/img/wn/{weather.Icon}@2x.png";
                }
            }
            _logger.LogInformation("Weather Info: {rawWeatherData}", rawWeatherData);
            return rawWeatherData;  
        }
    }
}