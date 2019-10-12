using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Moq;
using MvvmBlazor.Components;
using MvvmBlazor.Internal.Parameters;
using MvvmBlazor.ViewModel;
using Shouldly;
using Xunit;
using ParameterInfo = MvvmBlazor.Internal.Parameters.ParameterInfo;

namespace MvvmBlazor.Tests.Internal.Parameters
{
    public class ViewModelParameterSetterTests
    {
        private Mock<PropertyInfo> GenerateProperty(string propertyName)
        {
            var property = new Mock<PropertyInfo>();
            property.Setup(x => x.Name).Returns(propertyName).Verifiable();
            property.Setup(x => x.GetCustomAttributes(typeof(ParameterAttribute), false)).Returns(new object[] { new ParameterAttribute() });
            property.Setup(x => x.GetHashCode()).CallBase();
            return property;
        }

        [Fact]
        public void ValidatesParameters()
        {
            Should.Throw<ArgumentNullException>(() =>
                new ViewModelParameterSetter(null, new Mock<IParameterCache>().Object));
            Should.Throw<ArgumentNullException>(() =>
                new ViewModelParameterSetter(new Mock<IParameterResolver>().Object, null));
        }

        [Fact]
        public void ResolveAndSet_ValidatesParameters()
        {
            var resolver = new Mock<IParameterResolver>();
            var cache = new Mock<IParameterCache>();
            var setter = new ViewModelParameterSetter(resolver.Object, cache.Object);

            Should.Throw<ArgumentNullException>(() => setter.ResolveAndSet(null, new Mock<ViewModelBase>().Object));
            Should.Throw<ArgumentNullException>(() => setter.ResolveAndSet(new Mock<ComponentBase>().Object, null));
        }

        [Fact]
        public void ResolveAndSet_ResolvesParametersAndCachesResult()
        {
            var cp1 = GenerateProperty("p1");
            var componentProperties = new List<PropertyInfo> { cp1.Object };

            var vmp1 = GenerateProperty("p1");
            var viewModelProperties = new List<PropertyInfo> { vmp1.Object };

            var resolver = new Mock<IParameterResolver>();
            var cache = new Mock<IParameterCache>();
            var component = new Mock<ComponentBase>();
            var viewModel = new Mock<ViewModelBase>();
            cache.Setup(x => x.Get(It.Is<Type>(y => y.BaseType == typeof(ComponentBase)))).Returns((ParameterInfo) null).Verifiable();
            cache.Setup(x => x.Set(It.Is<Type>(y => y.BaseType == typeof(ComponentBase)), It.IsAny<ParameterInfo>())).Verifiable();
            resolver.Setup(x => x.ResolveParameters(It.Is<Type>(y => y.BaseType ==  typeof(ComponentBase)))).Returns(componentProperties).Verifiable();
            resolver.Setup(x => x.ResolveParameters(It.Is<Type>(y => y.BaseType == typeof(ViewModelBase)))).Returns(viewModelProperties).Verifiable();
            cp1.Setup(x => x.GetValue(component.Object, null)).Returns("foo").Verifiable();
            

            var setter = new ViewModelParameterSetter(resolver.Object, cache.Object);

            setter.ResolveAndSet(component.Object, viewModel.Object);

            cp1.Verify();
            cp1.Verify(x => x.GetHashCode());
            vmp1.Verify(x => x.SetValue(viewModel.Object, "foo", null));
            vmp1.Verify();
            cache.Verify();
            resolver.Verify();
            component.VerifyNoOtherCalls();
            viewModel.VerifyNoOtherCalls();
            cp1.VerifyNoOtherCalls();
            vmp1.VerifyNoOtherCalls();
            resolver.VerifyNoOtherCalls();
            cache.VerifyNoOtherCalls();
            component.VerifyNoOtherCalls();
            viewModel.VerifyNoOtherCalls();
        }
    }
}