using AspNetCoreRateLimit;
using WeatherApi.Middleware;
using WeatherApi.Services;

namespace WeatherApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add services to the container.
            // Add services related to client rate limitng
            services.AddOptions();
            services.AddMemoryCache();
            services.Configure<ClientRateLimitOptions>(Configuration.GetSection("ClientRateLimiting"));
            services.Configure<ClientRateLimitPolicies>(Configuration.GetSection("ClientRateLimitingPolicies"));
            services.AddInMemoryRateLimiting();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddScoped<IWeatherInformationService, WeatherInformationService>();

            services.AddHttpClient<WeatherInformationService>("WeatherApi", client =>
            {
                client.BaseAddress = new Uri("https://api.openweathermap.org/data/2.5/");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(options =>
            options.WithOrigins("http://localhost:3002")
            .AllowAnyMethod()
            .AllowAnyHeader());

            app.UseHttpsRedirection();

            //Middleware to check for APIKey in requests
            app.UseMiddleware<ApiKeyMiddleware>();

            app.UseClientRateLimiting();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}

