using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using WeatherApi.Controllers;
using WeatherApi.Models;
using WeatherApi.Services;

namespace WeatherApiTests
{
    public class WeatherInformationControllerTests
    {
        private readonly WeatherInformationController _controller;
        private readonly Mock<IConfiguration> _mockConfiguration = new Mock<IConfiguration>();
        private readonly Mock<ILogger<WeatherInformationController>> _mockLogger = new Mock<ILogger<WeatherInformationController>>();
        private readonly Mock<IWeatherInformationService> _mockWeatherInformationService = new Mock<IWeatherInformationService>();

        public WeatherInformationControllerTests()
        {
            _controller = new WeatherInformationController(_mockConfiguration.Object, _mockLogger.Object, _mockWeatherInformationService.Object);
        }

        [Fact]
        public async void TestGetWeatherInfoNonEmptyValues()
        {
            // Arrange
            var weatherInfo = new WeatherInformation()
            {
                CityName = "Melbourne",
                Main = new Main() { Temperature = 20.0 },
                Sys = new Sys() { CountryCode = "AU" },
                Weather = new List<Weather>()
                {
                    new Weather()
                    {
                        Main = "rain", Description = "Moderate rain", Icon = "10d"
                    }
                },
            };
            _mockWeatherInformationService.Setup(svc => svc.GetWeatherInfoAsync("Melbourne", "AU")).ReturnsAsync(weatherInfo);

            // Act
            var weatherResult = await _controller.GetWeatherInfo("Melbourne", "AU");

            // Assert
            Assert.Equal("Melbourne", weatherInfo.CityName);
            Assert.Equal(20.0, weatherInfo.Main.Temperature);
            Assert.Equal("rain", weatherInfo.Weather?.FirstOrDefault()?.Main);
            Assert.Equal("Moderate rain", weatherInfo.Weather?.FirstOrDefault()?.Description);
            Assert.Equal("10d", weatherInfo.Weather?.FirstOrDefault()?.Icon);
        }

        [Theory]
        [InlineData(null, null, "City and Country code are required.")]
        [InlineData("Melbourne", null, "Country code is required.")]
        [InlineData(null, "AU", "City is required.")]
        public async void TestGetWeatherInfoEmptyValues(string city, string countryCode, string expected)
        {
            // Act
            var result = await _controller.GetWeatherInfo(city, countryCode);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.Equal(expected, badRequestResult.Value);
        }
    }
}