using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BlazorServersideSample.Data;
using MvvmBlazor.ViewModel;

namespace BlazorServersideSample.ViewModel
{
    public class FetchDataViewModel : ViewModelBase
    {
        private readonly WeatherForecastService _weatherForecastService;

        private ObservableCollection<WeatherForecast> _forecasts;

        public FetchDataViewModel(WeatherForecastService weatherForecastService)
        {
            _weatherForecastService = weatherForecastService;
        }

        public ObservableCollection<WeatherForecast> Forecasts
        {
            get => _forecasts;
            set => Set(ref _forecasts, value, nameof(Forecasts));
        }

        public override async Task OnInitializedAsync()
        {
            await Task.Delay(1500);

            var forecastData = await _weatherForecastService.GetForecastAsync(DateTime.Now);
            _forecasts = new ObservableCollection<WeatherForecast>(forecastData);
        }
    }
}