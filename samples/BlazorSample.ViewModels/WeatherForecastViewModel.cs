using System;
using System.Collections.Generic;
using System.Text;
using BlazorSample.Domain.Entities;
using MvvmBlazor.ViewModel;

namespace BlazorSample.ViewModels
{
    public class WeatherForecastViewModel: ViewModelBase
    {
        public DateTime Date => _weatherForecastEntity.Date;
        public string? Summary => _weatherForecastEntity.Summary;

        private int _temperatureC;

        public int TemperatureC
        {
            get => _temperatureC;
            set => Set(ref _temperatureC, value, nameof(TemperatureC));
        }

        private int _temperatureF;

        public int TemperatureF
        {
            get => _temperatureF;
            set => Set(ref _temperatureF, value, nameof(TemperatureF));
        }

        private readonly WeatherForecastEntity _weatherForecastEntity;
        public WeatherForecastViewModel(WeatherForecastEntity weatherForecastEntity)
        {
            _weatherForecastEntity = weatherForecastEntity;
            TemperatureC = _weatherForecastEntity.TemperatureC;
            TemperatureF = _weatherForecastEntity.TemperatureF;
        }
    }
}
