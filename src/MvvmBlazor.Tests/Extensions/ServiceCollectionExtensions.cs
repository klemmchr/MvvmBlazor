using Microsoft.Extensions.DependencyInjection;
using Moq;
using MvvmBlazor.Tests.Abstractions;

namespace MvvmBlazor.Tests.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static Mock<T> Mock<T>(this IServiceCollection services, params object[] args)
            where T : class
        {
            var mock = new Mock<T>(args);
            services.AddSingleton(mock);
            services.AddSingleton(mock.Object);
            return mock;
        }

        public static Mock<T> StrictMock<T>(this IServiceCollection services)
            where T : class
        {
            var mock = new StrictMock<T>();
            services.AddSingleton<Mock<T>>(mock);
            services.AddSingleton(mock.Object);
            return mock;
        }

    }
}