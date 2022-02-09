using Binder = MvvmBlazor.Internal.Bindings.Binder;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMvvm(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IBindingFactory, BindingFactory>();
        serviceCollection.AddSingleton<IParameterResolver, ParameterResolver>();
        serviceCollection.AddSingleton<IParameterCache, ParameterCache>();
        serviceCollection.AddSingleton<IViewModelParameterSetter, ViewModelParameterSetter>();
        serviceCollection.AddTransient<IWeakEventManager, WeakEventManager>();
        serviceCollection.AddTransient<IBinder, Binder>();

        return serviceCollection;
    }
}