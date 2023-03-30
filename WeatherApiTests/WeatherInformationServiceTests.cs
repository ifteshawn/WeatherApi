using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using WeatherApi.Models;
using WeatherApi.Services;

namespace WeatherApiTests
{
    public class WeatherInformationServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IConfigurationSection> _mockConfigurationSection;
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly Mock<ILogger<IWeatherInformationService>> _mockLogger;

        public WeatherInformationServiceTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfigurationSection = new Mock<IConfigurationSection>();
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockLogger = new Mock<ILogger<IWeatherInformationService>>();
            _mockConfigurationSection.Setup(x => x.Value).Returns("mock-api-key");
            _mockConfiguration.Setup(x => x.GetSection("APIKey")).Returns(_mockConfigurationSection.Object);        
        }

        [Fact]
        public async void TestGetWeatherInfoAsync()
        {
            // Arrange
            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\r\n  \"coord\": {\r\n    \"lon\": 10.99,\r\n    \"lat\": 44.34\r\n  },\r\n  \"weather\": [\r\n    {\r\n      \"id\": 501,\r\n      \"main\": \"Rain\",\r\n      \"description\": \"moderate rain\",\r\n      \"icon\": \"10d\"\r\n    }\r\n  ],\r\n  \"base\": \"stations\",\r\n  \"main\": {\r\n    \"temp\": 298.48,\r\n    \"feels_like\": 298.74,\r\n    \"temp_min\": 297.56,\r\n    \"temp_max\": 300.05,\r\n    \"pressure\": 1015,\r\n    \"humidity\": 64,\r\n    \"sea_level\": 1015,\r\n    \"grnd_level\": 933\r\n  },\r\n  \"visibility\": 10000,\r\n  \"wind\": {\r\n    \"speed\": 0.62,\r\n    \"deg\": 349,\r\n    \"gust\": 1.18\r\n  },\r\n  \"rain\": {\r\n    \"1h\": 3.16\r\n  },\r\n  \"clouds\": {\r\n    \"all\": 100\r\n  },\r\n  \"dt\": 1661870592,\r\n  \"sys\": {\r\n    \"type\": 2,\r\n    \"id\": 2075663,\r\n    \"country\": \"AU\",\r\n    \"sunrise\": 1661834187,\r\n    \"sunset\": 1661882248\r\n  },\r\n  \"timezone\": 7200,\r\n  \"id\": 3163858,\r\n  \"name\": \"Melbourne\",\r\n  \"cod\": 200\r\n}"),
                });

            var _mockHttpClient = new HttpClient(mockHandler.Object)
            {
                BaseAddress = new Uri("https://mock-test-uri/")
            };

            _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(_mockHttpClient);

            var _weatherService = new WeatherInformationService(_mockConfiguration.Object, _mockHttpClientFactory.Object, _mockLogger.Object);

            var expected = new WeatherInformation()
            {
                CityName = "Melbourne",
                Main = new Main() { Temperature = 298.48 },
                Sys = new Sys() { CountryCode = "AU" },
                Weather = new List<Weather>()
                {
                    new Weather()
                    {
                        Main = "rain", Description = "Moderate rain", Icon = "10d"
                    }
                },
            };

            // Act
            var result = await _weatherService.GetWeatherInfoAsync("Melbourne", "AU");

            // Assert
            Assert.IsType<WeatherInformation>(result);
            var weatherData = result as WeatherInformation;
            Assert.NotNull(weatherData);
            Assert.Equal(expected.CityName, result.CityName);
            Assert.Equal(expected.Main.Temperature, result.Main?.Temperature);
        }

        [Fact]
        public async Task TestGetWeatherInfoAsync_InvalidApiKey()
        {
            // Arrange
            _mockConfigurationSection.Setup(x => x.Value).Returns("");
            _mockConfiguration.Setup(x => x.GetSection("APIKey")).Returns(_mockConfigurationSection.Object);
            var _weatherService = new WeatherInformationService(_mockConfiguration.Object, _mockHttpClientFactory.Object, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await _weatherService.GetWeatherInfoAsync("Melbourne", "AU"));

        }

        [Theory]
        [InlineData("invalidCity", "AU", "Error retrieving weather information for invalidCity: NotFound")]
        [InlineData("Melbourne", "invalidCountry", "Error retrieving weather information for Melbourne: NotFound")]
        public async void TestGetWeatherInfoAsync_InvalidCity(string city, string countryCode, string expectedMessage)
        {
            // Arrange
            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound
                });

            var _mockHttpClient = new HttpClient(mockHandler.Object)
            {
                BaseAddress = new Uri("https://mock-test-uri/")
            };
            _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(_mockHttpClient);

            var _weatherService = new WeatherInformationService(_mockConfiguration.Object, _mockHttpClientFactory.Object, _mockLogger.Object);

            var expected = new WeatherInformationServiceException(HttpStatusCode.NotFound, expectedMessage);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<WeatherInformationServiceException>(
            async () => await _weatherService.GetWeatherInfoAsync(city, countryCode));
            Assert.Equal(expected.StatusCode, ex.StatusCode);
            Assert.Equal(expected.Message, ex.Message);
        }
    }
}
