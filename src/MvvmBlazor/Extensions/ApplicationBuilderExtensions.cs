using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MvvmBlazor.Components;

namespace MvvmBlazor.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseMvvm(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null) throw new ArgumentNullException(nameof(applicationBuilder));
            
            var dependencyResolver = applicationBuilder.ApplicationServices.GetRequiredService<IDependencyResolver>();
            DependencyResolver.Default = dependencyResolver;

            return applicationBuilder;
        }
    }
}