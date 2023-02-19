using WeatherApi.Authentication;

namespace WeatherApi.Middleware
{
    public class ApiKeyMiddleware
    {
        //way to call the next thing in the middleware pipeline
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
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

            if (!apiKey.Contains(extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid API Key.");
                return;
            }

            //RequestDelegate to pass down the context to the next request of the pipeline.
            await _next(context);

        }
    }
}
