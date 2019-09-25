using System;
using Microsoft.Extensions.DependencyInjection;

namespace MvvmBlazor.Components
{
    public interface IDependencyResolver
    {
        T GetService<T>();
    }

    internal class DependencyResolver : IDependencyResolver
    {
        private readonly IServiceProvider _serviceProvider;

        public DependencyResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public static IDependencyResolver? Default { get; internal set; }

        public T GetService<T>()
        {
            // Create a scope to be able to access scoped services
#pragma warning disable IDISP001
            using var scope = _serviceProvider.CreateScope();
#pragma warning restore IDISP001
            return scope.ServiceProvider.GetRequiredService<T>();
        }
    }
}