using System.Net;

namespace WeatherApi.Models
{
    public class WeatherInformationServiceException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public WeatherInformationServiceException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
