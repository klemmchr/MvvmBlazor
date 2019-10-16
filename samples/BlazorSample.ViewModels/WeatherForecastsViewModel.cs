using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BlazorSample.Domain.Entities;
using BlazorSample.Domain.Services;
using MvvmBlazor.ViewModel;

namespace BlazorSample.ViewModels
{
    public class WeatherForecastsViewModel : ViewModelBase
    {
        private readonly IWeatherForecastGetter _weatherForecastGetter;

        private ObservableCollection<WeatherForecastViewModel>? _forecasts;

        public WeatherForecastsViewModel(IWeatherForecastGetter weatherForecastGetter)
        {
            _weatherForecastGetter = weatherForecastGetter;
        }

        public ObservableCollection<WeatherForecastViewModel>? Forecasts
        {
            get => _forecasts;
            set => Set(ref _forecasts, value, nameof(Forecasts));
        }

        public override async Task OnInitializedAsync()
        {
            // Simulate loading time
            await Task.Delay(1500);

            var forecastData = await _weatherForecastGetter.GetForecasts();
            _forecasts = new ObservableCollection<WeatherForecastViewModel>(forecastData.Select(x => new WeatherForecastViewModel(x)));
        }

        public void RandomizeData()
        {
            var random = new Random();

            foreach (var weatherForecastEntity in _forecasts)
            {
                weatherForecastEntity.TemperatureC = random.Next(10, 40);
                weatherForecastEntity.TemperatureF = random.Next(50, 200);
            }
        }
    }
}