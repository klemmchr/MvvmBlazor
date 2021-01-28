using BlazorClientsideSample.Client.Services;
using BlazorSample.Components.Extensions;
using BlazorSample.Domain.Extensions;
using BlazorSample.Domain.Services;
using BlazorSample.ViewModels.Extensions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MvvmBlazor.Extensions;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorClientsideSample.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.RootComponents.Add<App>("#app");

            builder.Services.AddMvvm();
            builder.Services.AddDomain().AddComponents().AddViewModels();

            builder.Services.AddScoped(
                sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddSingleton<IWeatherForecastGetter, WeatherForecastGetter>();

            await builder.Build().RunAsync();
        }
    }
}