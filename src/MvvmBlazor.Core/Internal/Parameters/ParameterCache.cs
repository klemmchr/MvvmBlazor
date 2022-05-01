using System.Collections.Concurrent;

namespace MvvmBlazor.Internal.Parameters;

internal interface IParameterCache
{
    ParameterInfo? Get(Type type);
    void Set(Type type, ParameterInfo info);
}

internal class ParameterCache : IParameterCache
{
    private readonly ConcurrentDictionary<Type, ParameterInfo> _cache = new();

    public ParameterInfo? Get(Type type)
    {
        return _cache.TryGetValue(type, out var info) ? info : null;
    }

    public void Set(Type type, ParameterInfo info)
    {
        ArgumentNullException.ThrowIfNull(info);
        _cache[type] = info;
    }
}