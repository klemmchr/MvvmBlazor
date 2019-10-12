using System;
using System.Collections.Generic;
using System.Reflection;
using Moq;
using MvvmBlazor.Internal.Parameters;
using Shouldly;
using Xunit;
using ParameterInfo = MvvmBlazor.Internal.Parameters.ParameterInfo;

namespace MvvmBlazor.Tests.Internal.Parameters
{
    public class ParameterCacheTests
    {
        [Fact]
        public void Get_ValidatesParameters()
        {
            var cache = new ParameterCache();
            Should.Throw<ArgumentNullException>(() => cache.Get(null));
        }

        [Fact]
        public void Set_ValidatesParameters()
        {
            var cache = new ParameterCache();
            Should.Throw<ArgumentNullException>(() =>
                cache.Set(null, new ParameterInfo(new List<PropertyInfo>(), new List<PropertyInfo>())));
            Should.Throw<ArgumentNullException>(() => cache.Set(typeof(ParameterCacheTests), null));
        }

        [Fact]
        public void Get_GetsCachedEntry()
        {
            var type = typeof(ParameterCacheTests);
            var parameterInfo = new ParameterInfo(new List<PropertyInfo>(), new List<PropertyInfo>());

            var cache = new ParameterCache();
            cache.Set(type, parameterInfo);

            cache.Get(type).ShouldBeSameAs(parameterInfo);
        }
    }
}