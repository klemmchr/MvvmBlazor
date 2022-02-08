using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BlazorSample.Domain.Entities;
using BlazorSample.Domain.Services;

namespace BlazorClientsideSample.Client.Services;

public class WeatherForecastGetter : IWeatherForecastGetter
{
    private readonly HttpClient _httpClient;

    public WeatherForecastGetter(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<IEnumerable<WeatherForecastEntity>> GetForecasts()
    {
        return _httpClient.GetFromJsonAsync<IEnumerable<WeatherForecastEntity>>("WeatherForecast");
    }
}