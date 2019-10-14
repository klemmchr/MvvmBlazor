using BlazorSample.Client.Services;
using BlazorSample.Domain.Services;
using BlazorSample.ViewModels.Extensions;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using MvvmBlazor.Extensions;

namespace BlazorSample.Client
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Add mvvm to client
            services.AddMvvm();

            // Register view models
            services.AddViewModels();

            services.AddSingleton<IWeatherForecastGetter, WeatherForecastGetter>();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}