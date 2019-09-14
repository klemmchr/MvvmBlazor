using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace MvvmBlazor.Components
{
    public interface IDependencyResolver {
        T GetService<T>();
    }

    internal class DependencyResolver : IDependencyResolver
    {
        private readonly IServiceProvider _serviceProvider;
        public static IDependencyResolver? Default { get; internal set; }

        public DependencyResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T GetService<T>()
        {
            return _serviceProvider.GetRequiredService<T>();
        }
    }
}
