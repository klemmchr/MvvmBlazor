﻿using BlazorSample.Components.Pages;
using BlazorSample.Domain.Extensions;
using MatBlazor;
using Microsoft.Extensions.DependencyInjection;
using Index = BlazorSample.Components.Pages.Index;

namespace BlazorSample.Components.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddComponents(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddNavigationItem<Index>("Index", MatIconNames.Home);
        serviceCollection.AddNavigationItem<Counter>("Counter", MatIconNames.Add);
        serviceCollection.AddNavigationItem<WeatherForecasts>("Weather forecasts", MatIconNames.Cloud);
        serviceCollection.AddNavigationItem<Clock>("Clock", MatIconNames.Alarm);
        serviceCollection.AddNavigationItem<Parameters>("Parameters", MatIconNames.List);

        return serviceCollection;
    }
}