using System;
using System.Threading.Tasks;
using BlazorSample.Data;
using MvvmBlazor.Collections;
using MvvmBlazor.ViewModel;

namespace BlazorSample.ViewModel
{
    public class FetchDataViewModel : ViewModelBase
    {
        private readonly WeatherForecastService _weatherForecastService;

        private ObservableCollection<WeatherForecast> _forecasts = new ObservableCollection<WeatherForecast>();

        public FetchDataViewModel(WeatherForecastService weatherForecastService)
        {
            _weatherForecastService = weatherForecastService;
        }

        public ObservableCollection<WeatherForecast> Forecasts
        {
            get => _forecasts;
            set => Set(ref _forecasts, value, nameof(Forecasts));
        }

        public override async Task OnInitAsync()
        {
            Forecasts.AddRange(await _weatherForecastService.GetForecastAsync(DateTime.Now));
        }
    }
}