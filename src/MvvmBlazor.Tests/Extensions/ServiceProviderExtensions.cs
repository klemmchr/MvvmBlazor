using System;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace MvvmBlazor.Tests.Extensions
{
    internal static class ServiceProviderExtensions
    {
        public static Mock<T> GetMock<T>(this IServiceProvider provider) where T : class
        {
            return provider.GetRequiredService<Mock<T>>();
        }
    }
}