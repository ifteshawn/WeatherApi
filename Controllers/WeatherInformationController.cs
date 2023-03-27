using Microsoft.AspNetCore.Mvc;
using WeatherApi.Models;
using WeatherApi.Services;

namespace WeatherApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherInformationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<WeatherInformationController> _logger;
        private readonly IWeatherInformationService _weatherInformationService;

        public WeatherInformationController(IConfiguration configuration, ILogger<WeatherInformationController> logger, IWeatherInformationService weatherInformationService)
        {
            _configuration = configuration;
            _logger = logger;
            _weatherInformationService = weatherInformationService;
        }

        // GET: api/WeatherInformation
        //[HttpGet("[action]/{city}/{country}")]
        [HttpGet("[action]/{city:length(1,50)}/{country:length(2)}")]
        public async Task<IActionResult> GetWeatherInfo(string city, string country)
        {
            _logger.LogInformation($"Fetching weather data for location {city}, {country}");

            if (string.IsNullOrEmpty(city) && string.IsNullOrEmpty(country))
            {
                return BadRequest("City and Country code are required.");
            }
            if (string.IsNullOrEmpty(city))
            {
                return BadRequest("City is required.");
            }
            if (string.IsNullOrEmpty(country))
            {
                return BadRequest("Country code is required.");
            }

            try
            {
                var weatherInformation = await _weatherInformationService.GetWeatherInfoAsync(city, country);
                _logger.LogInformation($"Temperature in {city}: {weatherInformation?.Main?.Temperature}°C");
                _logger.LogInformation($"Weather in {city}: {weatherInformation?.Weather?.FirstOrDefault()?.Main}");
                return Ok(weatherInformation);
            }
            catch (WeatherInformationServiceException ex)
            {
                _logger.LogError("Could not fetch weather data for {city} because: {Message}", city, ex.Message);
                var problemDetails = new ProblemDetails
                    {
                        Title = "An error occurred while retrieving weather data.",
                        Detail = ex.Message,
                        Status = (int)ex.StatusCode
                    };
                return StatusCode((int)ex.StatusCode, problemDetails);
            }   
        }
    }
}
