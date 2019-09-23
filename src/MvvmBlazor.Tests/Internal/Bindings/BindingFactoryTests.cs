using System.ComponentModel;
using System.Reflection;
using Moq;
using MvvmBlazor.Internal.Bindings;
using MvvmBlazor.Internal.WeakEventListener;
using Shouldly;
using Xunit;

namespace MvvmBlazor.Tests.Internal.Bindings
{
    public class BindingFactoryTests
    {
        [Fact]
        public void Create_ReturnsBinding()
        {
            var source = new Mock<INotifyPropertyChanged>();
            var propertyInfo = new Mock<PropertyInfo>();
            var wem = new Mock<IWeakEventManager>();

            var factory = new BindingFactory();
            var res = factory.Create(source.Object, propertyInfo.Object, wem.Object);
            res.ShouldNotBeNull();
            res.PropertyInfo.ShouldBe(propertyInfo.Object);
            res.Source.ShouldBe(source.Object);
        }
    }
}