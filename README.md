# WeatherApi
This Weather API is a service developed using C#, Visual Studio, and .Net 6. 
This API interacts with the frontend weather-app client by exposing an endpoint that takes city and country as parameters in the client request. It is API key authenticated, and looks for API keys in the incoming requests. It also uses client rate limiting based on the API key. It allows 5 requests per hour for each API key to prevent excessive requests. Based on the request received and validations on the parameters and API key, this service calls Open Weather API to fetch weather information with its own API key. This API key is stored in the appsettings file. It also stores its own API keys for clients in the appsettings file. 

### Installation:

Install Visual Studio 2022 or later version.
Clone the project from Github repository in Visual Studio
Restore the NuGet packages used in the project.
Build the project.

### Configuration:

Open the appsettings.json file in the project.
Open weather API key is stored as "APIKey".
Backend API keys are stored as "xApiKeys" in "Authentication"
Modify the rate limiting setting if needed.

### Usage:

The backend exposes a REST API that can be used by clients to fetch weather information.
Clients should include their API key in the request headers as "x-api-key"
The backend will check the rate limiting before making the request to the third-party API.
If the rate limit is exceeded, the backend will return an error response.
If the rate limit is not exceeded, the backend will make the request to the Open Weather API and return the weather information to the client.

### Testing:

To test the backend, run the project in Visual Studio.
Use Postman to send GET request to the API endpoints.
Include the API key in the request headers as "x-api-key".

### API endpoint:

https://localhost:7061/api/WeatherInformation/getWeatherInfo/{city}/{countryCode}
Example: https://localhost:7061/api/WeatherInformation/getWeatherInfo/Melbourne/AU

### Example response returned by this service:

{
  name: "Melbourne",
  weather: [
    {
      main: "Clouds",
      description: "Broken Clouds",
      icon: "04d",
      iconSource: "http://openweathermap.org/img/wn/04d@2x.png",
    },
  ],
  sys: {
    country: "AU",
  },
  main: {
    temp: 19.16,
  },
};

### Note:
The code repo for the front end is here: https://github.com/ifteshawn/weather-app-ui
Please follow the instructions on how to install and run the front end in the front end repo to run it. 
