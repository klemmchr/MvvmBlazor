using ParameterInfo = MvvmBlazor.Internal.Parameters.ParameterInfo;

namespace MvvmBlazor.Tests.Internal.Parameters;

public class ParameterCacheTests
{
    [Fact]
    public void Get_gets_cached_entry()
    {
        var type = typeof(ParameterCacheTests);
        var parameterInfo = new ParameterInfo(new List<PropertyInfo>(), new List<PropertyInfo>());

        var cache = new ParameterCache();
        cache.Set(type, parameterInfo);

        cache.Get(type).ShouldBeSameAs(parameterInfo);
    }
}