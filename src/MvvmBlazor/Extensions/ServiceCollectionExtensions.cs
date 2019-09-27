using Microsoft.Extensions.DependencyInjection;
using MvvmBlazor.Components;
using MvvmBlazor.Internal.Bindings;
using MvvmBlazor.Internal.WeakEventListener;

namespace MvvmBlazor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMvvm(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IWeakEventManagerFactory, WeakEventManagerFactory>();
            serviceCollection.AddSingleton<IBindingFactory, BindingFactory>();
            serviceCollection.AddHttpContextAccessor();

            return serviceCollection;
        }
    }
}