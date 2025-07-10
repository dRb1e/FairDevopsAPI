using Microsoft.OpenApi.Models;
using FairDevopsAPI.Services;
using FairDevopsAPI.Models;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Fair DevOps API", 
        Version = "v1",
        Description = "Fair Teknoloji DevOps Assessment API - CRUD Operations with Monitoring"
    });
});

// Add health checks for DevOps monitoring
builder.Services.AddHealthChecks();

// Add WeatherService for CRUD operations
builder.Services.AddSingleton<WeatherService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

// Prometheus metrics endpoint
app.UseMetricServer();
app.UseHttpMetrics();

app.UseHttpsRedirection();

// Health check endpoint for DevOps monitoring (Kubernetes için)
app.MapHealthChecks("/health");

// Health check endpoint for Swagger (Swagger UI'da görünür)
app.MapGet("/health/swagger", () => Results.Ok(new { status = "Healthy" }))
   .WithName("HealthCheckSwagger")
   .WithOpenApi();

// Prometheus metrics endpoint
app.MapGet("/metrics", () => Results.Ok("Metrics endpoint available at /metrics"))
   .WithName("MetricsEndpoint")
   .WithOpenApi();

// CRUD Endpoints for WeatherForecast

// GET /api/weather - Get all weather forecasts
app.MapGet("/api/weather", (WeatherService weatherService) =>
{
    // Increment request counter
    Metrics.CreateCounter("weather_requests_total", "Total number of weather requests", new CounterConfiguration
    {
        LabelNames = new[] { "endpoint" }
    }).WithLabels("get_all").Inc();
    
    return Results.Ok(weatherService.GetAll());
})
.WithName("GetAllWeatherForecasts")
.WithOpenApi();

// GET /api/weather/{id} - Get weather forecast by ID
app.MapGet("/api/weather/{id}", (int id, WeatherService weatherService) =>
{
    // Increment request counter
    Metrics.CreateCounter("weather_requests_total", "Total number of weather requests", new CounterConfiguration
    {
        LabelNames = new[] { "endpoint" }
    }).WithLabels("get_by_id").Inc();
    
    var forecast = weatherService.GetById(id);
    return forecast != null ? Results.Ok(forecast) : Results.NotFound();
})
.WithName("GetWeatherForecastById")
.WithOpenApi();

// POST /api/weather - Create new weather forecast
app.MapPost("/api/weather", (WeatherForecast forecast, WeatherService weatherService) =>
{
    // Increment request counter
    Metrics.CreateCounter("weather_requests_total", "Total number of weather requests", new CounterConfiguration
    {
        LabelNames = new[] { "endpoint" }
    }).WithLabels("create").Inc();
    
    var newForecast = weatherService.Create(forecast);
    return Results.Created($"/api/weather/{newForecast.Id}", newForecast);
})
.WithName("CreateWeatherForecast")
.WithOpenApi();

// PUT /api/weather/{id} - Update weather forecast
app.MapPut("/api/weather/{id}", (int id, WeatherForecast forecast, WeatherService weatherService) =>
{
    // Increment request counter
    Metrics.CreateCounter("weather_requests_total", "Total number of weather requests", new CounterConfiguration
    {
        LabelNames = new[] { "endpoint" }
    }).WithLabels("update").Inc();
    
    var updatedForecast = weatherService.Update(id, forecast);
    return updatedForecast != null ? Results.Ok(updatedForecast) : Results.NotFound();
})
.WithName("UpdateWeatherForecast")
.WithOpenApi();

// DELETE /api/weather/{id} - Delete weather forecast
app.MapDelete("/api/weather/{id}", (int id, WeatherService weatherService) =>
{
    // Increment request counter
    Metrics.CreateCounter("weather_requests_total", "Total number of weather requests", new CounterConfiguration
    {
        LabelNames = new[] { "endpoint" }
    }).WithLabels("delete").Inc();
    
    var deleted = weatherService.Delete(id);
    return deleted ? Results.NoContent() : Results.NotFound();
})
.WithName("DeleteWeatherForecast")
.WithOpenApi();

app.Run();
