using BlazorSample.Domain.Entities;

namespace BlazorSample.Domain.Services;

public interface IWeatherForecastGetter
{
    Task<IEnumerable<WeatherForecastEntity>> GetForecasts();
}