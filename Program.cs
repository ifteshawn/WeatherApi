using AspNetCoreRateLimit;
using WeatherApi.Middleware;

var builder = WebApplication.CreateBuilder(args);


//logging configuration
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container.
// Add services related to client rate limitng
builder.Services.AddOptions();
builder.Services.AddMemoryCache();
builder.Services.Configure<ClientRateLimitOptions>(builder.Configuration.GetSection("ClientRateLimiting"));
builder.Services.Configure<ClientRateLimitPolicies>(builder.Configuration.GetSection("ClientRateLimitingPolicies"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
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

app.UseAuthorization();

app.MapControllers();

app.Run();
