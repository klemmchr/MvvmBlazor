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
            return _serviceProvider.GetRequiredService<T>();
        }
    }
}