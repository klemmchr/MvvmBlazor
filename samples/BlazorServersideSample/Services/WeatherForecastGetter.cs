using BlazorSample.Domain.Entities;
using BlazorSample.Domain.Services;

namespace BlazorServersideSample.Services;

public class WeatherForecastGetter : IWeatherForecastGetter
{
    private static readonly string[] Summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public Task<IEnumerable<WeatherForecastEntity>> GetForecasts()
    {
        var rng = new Random();
        return Task.FromResult(
            Enumerable.Range(1, 5)
                .Select(
                    index => new WeatherForecastEntity
                    {
                        Date = DateTime.Now.AddDays(index),
                        TemperatureC = rng.Next(-20, 55),
                        Summary = Summaries[rng.Next(Summaries.Length)]
                    }
                )
        );
    }
}