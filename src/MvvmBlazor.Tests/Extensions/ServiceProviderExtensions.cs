// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

internal static class ServiceProviderExtensions
{
    public static Mock<T> GetMock<T>(this IServiceProvider provider) where T : class
    {
        return provider.GetRequiredService<Mock<T>>();
    }
}