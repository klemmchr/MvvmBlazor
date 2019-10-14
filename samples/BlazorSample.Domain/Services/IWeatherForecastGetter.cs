using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorSample.Domain.Entities;

namespace BlazorSample.Domain.Services
{
    public interface IWeatherForecastGetter
    {
        Task<IEnumerable<WeatherForecastEntity>> GetForecasts();
    }
}