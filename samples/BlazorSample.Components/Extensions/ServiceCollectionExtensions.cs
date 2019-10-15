using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using BlazorSample.Components.Pages;
using BlazorSample.Domain.Extensions;
using MatBlazor;
using Index = BlazorSample.Components.Pages.Index;

namespace BlazorSample.Components.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddComponents(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddNavigationItem<Index>("Index", MatIconNames.Home);
            serviceCollection.AddNavigationItem<Counter>("Counter", MatIconNames.Add);
            serviceCollection.AddNavigationItem<FetchData>("FetchData", MatIconNames.Cloud_download);
            serviceCollection.AddNavigationItem<Clock>("Clock", MatIconNames.Alarm);
            serviceCollection.AddNavigationItem<Parameters>("Parameters", MatIconNames.List);

            serviceCollection.AddScoped<HttpClient>();
            return serviceCollection;
        }
    }
}
