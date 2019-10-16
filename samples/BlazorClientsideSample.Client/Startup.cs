using BlazorClientsideSample.Client.Services;
using BlazorSample.Components.Extensions;
using BlazorSample.Domain.Extensions;
using BlazorSample.Domain.Services;
using BlazorSample.ViewModels.Extensions;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using MvvmBlazor.Extensions;

namespace BlazorClientsideSample.Client
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Add mvvm to client
            services.AddMvvm();

            services.AddDomain().AddComponents().AddViewModels();

            services.AddSingleton<IWeatherForecastGetter, WeatherForecastGetter>();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}