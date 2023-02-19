using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using WeatherApi.Models;
using Microsoft.Extensions.Options;

namespace WeatherApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherInformationController : ControllerBase
    {
        //DI to fetch configurations inside appsettings
        private readonly IConfiguration _configuration;
        public WeatherInformationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: api/WeatherInformation
        [HttpGet("[action]/{city}/{country}")]
        public async Task<IActionResult> getWeatherInfo(string city, string country)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri("https://api.openweathermap.org");
                    //Assigning the APIKey stored in appsettings.json to a variable
                    var openWeatherApiKey = _configuration.GetValue<string>("APIKey");
                    var response = await client.GetAsync($"/data/2.5/weather?q={city},{country}&appid={openWeatherApiKey}&units=metric");
                    response.EnsureSuccessStatusCode();

                    var stringResult = await response.Content.ReadAsStringAsync();
                    var rawWeatherData = JsonSerializer.Deserialize<WeatherInformation>(stringResult);

                    return Ok(new
                    {
                        name = rawWeatherData.name,
                        country = rawWeatherData.sys.country,
                        temp = rawWeatherData.main.temp,
                        //Description = string.Join(",", rawWeatherData.weather.Select(x => x.description)),
                        description = rawWeatherData.weather[0].description,
                        icon = rawWeatherData.weather[0].icon
                    });
                }
                catch (HttpRequestException httpRequestException)
                {
                    return BadRequest($"Error getting weather from OpenWeather: {httpRequestException.Message}");
                }
                
            }
            
        }

    }
}
