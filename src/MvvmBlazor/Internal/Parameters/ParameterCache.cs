using System;
using System.Collections.Generic;
using System.Text;

namespace MvvmBlazor.Internal.Parameters
{
    internal interface IParameterCache
    {
        ParameterInfo? Get(Type type);
        void Set(Type type, ParameterInfo info);
    }

    internal class ParameterCache : IParameterCache
    {
        private readonly Dictionary<Type, ParameterInfo> _cache = new Dictionary<Type, ParameterInfo>();

        public ParameterInfo? Get(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return _cache.TryGetValue(type, out var info) ? info : null;
        }
        
        public void Set(Type type, ParameterInfo info)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            _cache[type] = info ?? throw new ArgumentNullException(nameof(info));
        }
    }
}