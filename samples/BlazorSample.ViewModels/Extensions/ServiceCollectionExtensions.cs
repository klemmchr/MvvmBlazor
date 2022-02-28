using BlazorSample.ViewModels.Navbar;

namespace BlazorSample.ViewModels.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddViewModels(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<WeatherForecastsViewModel>();
        serviceCollection.AddTransient<CounterViewModel>();
        serviceCollection.AddTransient<ClockViewModel>();
        serviceCollection.AddTransient<ParametersViewModel>();
        serviceCollection.AddScoped<NavbarViewModel>();
        serviceCollection.AddScoped<TypedParametersViewModel>();

        return serviceCollection;
    }
}