namespace BlazorSample.ViewModels;

public partial class WeatherForecastsViewModel : ViewModelBase
{
    private readonly IWeatherForecastGetter _weatherForecastGetter;

    [Notify]
    private ObservableCollection<WeatherForecastViewModel>? _forecasts;

    public WeatherForecastsViewModel(IWeatherForecastGetter weatherForecastGetter)
    {
        _weatherForecastGetter = weatherForecastGetter;
    }

    public override async Task OnInitializedAsync()
    {
        // Simulate loading time
        await Task.Delay(1500);

        var forecastData = await _weatherForecastGetter.GetForecasts();
        Forecasts =
            new ObservableCollection<WeatherForecastViewModel>(
                forecastData.Select(x => new WeatherForecastViewModel(x))
            );
    }

    public void RandomizeData()
    {
        var random = new Random();

        foreach (var weatherForecastEntity in _forecasts!)
        {
            weatherForecastEntity.TemperatureC = random.Next(10, 40);
            weatherForecastEntity.TemperatureF = random.Next(50, 200);
        }
    }
}