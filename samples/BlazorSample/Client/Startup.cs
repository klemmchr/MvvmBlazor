using BlazorSample.Client.Services;
using BlazorSample.Client.ViewModel;
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

            services.AddSingleton<WeatherForecastService>();

            // Register view models
            services.AddTransient<FetchDataViewModel>();
            services.AddTransient<CounterViewModel>();
            services.AddTransient<ClockViewModel>();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}