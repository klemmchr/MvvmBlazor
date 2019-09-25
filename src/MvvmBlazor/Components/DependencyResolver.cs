using System;
using Microsoft.AspNetCore.Http;
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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DependencyResolver(IServiceProvider serviceProvider, IHttpContextAccessor httpContextAccessor)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public static IDependencyResolver? Default { get; internal set; }

        public T GetService<T>()
        {
            // Use http context services if available
            if(_httpContextAccessor.HttpContext != null)
                return _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<T>();

            // Otherwise create a scope and resolve it
#pragma warning disable IDISP001
            using var scope = _serviceProvider.CreateScope();
#pragma warning restore IDISP001

            return scope.ServiceProvider.GetRequiredService<T>();
        }
    }
}