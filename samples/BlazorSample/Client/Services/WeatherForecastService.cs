using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BlazorSample.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorSample.Client.Services
{
    public class WeatherForecastService
    {
        private readonly HttpClient _httpClient;

        public WeatherForecastService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<IEnumerable<WeatherForecast>> GetForecasts()
        {
            return _httpClient.GetJsonAsync<IEnumerable<WeatherForecast>>("WeatherForecast");
        }
    }
}