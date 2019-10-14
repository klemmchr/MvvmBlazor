using Microsoft.Extensions.DependencyInjection;

namespace BlazorSample.ViewModels.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddViewModels(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<FetchDataViewModel>();
            serviceCollection.AddTransient<CounterViewModel>();
            serviceCollection.AddTransient<ClockViewModel>();
            serviceCollection.AddTransient<ParametersViewModel>();

            return serviceCollection;
        }
    }
}