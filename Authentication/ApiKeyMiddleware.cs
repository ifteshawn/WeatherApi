using WeatherApi.Authentication;

namespace WeatherApi.Middleware
{
    public class ApiKeyMiddleware
    {
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
            var apiKey = _configuration.GetSection(AuthConstants.ApiKeySectionName).Get<List<string>>();
            _logger.LogDebug("Weather API Key values from appSettings: { apiKey }", apiKey);

            if (!apiKey.Contains(extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid API Key.");
                _logger.LogDebug("The extracted API key from request header is: {extractedApiKey}", extractedApiKey);
                return;
            }
            await _next(context);

        }
    }
}
