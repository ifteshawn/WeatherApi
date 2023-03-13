using WeatherApi.Authentication;

namespace WeatherApi.Middleware
{
    public class ApiKeyMiddleware
    {
        //way to call the next thing in the middleware pipeline
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;


        public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<ApiKeyMiddleware> logger)
        {
            _next = next;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if(!context.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API Key missing.");
                return;
            }

            //declaring list of API  keys from appsettings
            var apiKey = _configuration.GetSection(AuthConstants.ApiKeySectionName).Get<List<string>>();
            _logger.LogDebug("Weather API Key values from appSettings: { apiKey }", apiKey);

            if (!apiKey.Contains(extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid API Key.");
                _logger.LogDebug("The extracted API key from request header is: {extractedApiKey}", extractedApiKey);
                return;
            }

            //RequestDelegate to pass down the context to the next request of the pipeline.
            await _next(context);

        }
    }
}
