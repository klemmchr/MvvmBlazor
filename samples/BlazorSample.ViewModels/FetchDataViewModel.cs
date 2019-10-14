using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BlazorSample.Domain.Entities;
using BlazorSample.Domain.Services;
using MvvmBlazor.ViewModel;

namespace BlazorSample.ViewModels
{
    public class FetchDataViewModel : ViewModelBase
    {
        private readonly IWeatherForecastGetter _weatherForecastGetter;

        private ObservableCollection<WeatherForecastEntity> _forecasts;

        public FetchDataViewModel(IWeatherForecastGetter weatherForecastGetter)
        {
            _weatherForecastGetter = weatherForecastGetter;
        }

        public ObservableCollection<WeatherForecastEntity> Forecasts
        {
            get => _forecasts;
            set => Set(ref _forecasts, value, nameof(Forecasts));
        }

        public override async Task OnInitializedAsync()
        {
            await Task.Delay(1500);

            var forecastData = await _weatherForecastGetter.GetForecasts();
            _forecasts = new ObservableCollection<WeatherForecastEntity>(forecastData);
        }
    }
}