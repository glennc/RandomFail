var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<RandomFailureMiddleware>();

var app = builder.Build();

// Configure the HTTP request pipeline.

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.UseMiddleware<RandomFailureMiddleware>();

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateTime.Now.AddDays(index),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public class RandomFailureMiddleware : IMiddleware
{
    private Random _rand;

    public RandomFailureMiddleware()
    {
        _rand = new Random();
    }

    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (_rand.NextDouble() >= 0.5)
        {
            throw new Exception("Computer says no.");
        }
        return next(context);
    }
}