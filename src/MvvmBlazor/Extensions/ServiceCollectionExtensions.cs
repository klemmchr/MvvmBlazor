using Microsoft.Extensions.DependencyInjection;
using MvvmBlazor.Components;

namespace MvvmBlazor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMvvm(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddSingleton<IDependencyResolver, DependencyResolver>();
        }
    }
}