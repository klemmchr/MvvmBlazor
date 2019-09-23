using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BlazorSample.Client.Services;
using BlazorSample.Shared;
using MvvmBlazor.ViewModel;

namespace BlazorSample.Client.ViewModel
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

            var forecastData = await _weatherForecastService.GetForecasts();
            _forecasts = new ObservableCollection<WeatherForecast>(forecastData);
        }
    }
}