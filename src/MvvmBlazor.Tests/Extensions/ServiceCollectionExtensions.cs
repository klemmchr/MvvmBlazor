// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

internal static class ServiceCollectionExtensions
{
    public static Mock<T> Mock<T>(this IServiceCollection services, params object[] args) where T : class
    {
        var mock = new Mock<T>(args);
        services.AddSingleton(mock);
        services.AddSingleton(mock.Object);
        return mock;
    }

    public static Mock<T> StrictMock<T>(this IServiceCollection services) where T : class
    {
        var mock = new StrictMock<T>();
        services.AddSingleton<Mock<T>>(mock);
        services.AddSingleton(mock.Object);
        return mock;
    }

    public static IServiceCollection Provide<T>(this IServiceCollection services) where T : class
    {
        return services.AddSingleton<T>();
    }
}