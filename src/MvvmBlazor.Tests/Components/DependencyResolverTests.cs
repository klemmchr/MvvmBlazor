using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using MvvmBlazor.Components;
using Shouldly;
using Xunit;

namespace MvvmBlazor.Tests.Components
{
    public class DependencyResolverTests
    {
        [Fact]
        public void ValidatesParameters()
        {
            Should.Throw<ArgumentNullException>(() => new DependencyResolver(null));
        }

        [Fact]
        public void GetService_GetsServiceFromProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(x => x.GetService(typeof(object))).Returns(new object()).Verifiable();

            var resolver = new DependencyResolver(serviceProvider.Object);
            var res = resolver.GetService<object>();
            res.ShouldNotBeNull();

            serviceProvider.Verify();
            serviceProvider.VerifyNoOtherCalls();
        }

        [Fact]
        public void GetService_ThrowsException_WhenServiceIsUnregistered()
        {
            var serviceProvider = new Mock<IServiceProvider>();

            var resolver = new DependencyResolver(serviceProvider.Object);
            Should.Throw<InvalidOperationException>(() =>resolver.GetService<object>());
            
            serviceProvider.Verify(x => x.GetService(typeof(object)));
            serviceProvider.VerifyNoOtherCalls();
        }
    }
}
