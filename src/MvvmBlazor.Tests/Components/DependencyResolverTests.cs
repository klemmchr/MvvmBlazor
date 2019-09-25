using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MvvmBlazor.Components;
using Shouldly;
using Xunit;

namespace MvvmBlazor.Tests.Components
{
    public class DependencyResolverTests
    {
        [Fact]
        public void GetService_GetsServiceFromProvider()
        {
            var scope = new Mock<IServiceScope>();
            var scopeFactory = new Mock<IServiceScopeFactory>();
            var serviceProvider = new Mock<IServiceProvider>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            scopeFactory.Setup(x => x.CreateScope()).Returns(scope.Object).Verifiable();
            scope.SetupGet(x => x.ServiceProvider).Returns(serviceProvider.Object).Verifiable();
            serviceProvider.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(scopeFactory.Object)
                .Verifiable();
            serviceProvider.Setup(x => x.GetService(typeof(object))).Returns(new object()).Verifiable();

            var resolver = new DependencyResolver(serviceProvider.Object, contextAccessor.Object);
            var res = resolver.GetService<object>();
            res.ShouldNotBeNull();

            scope.Verify();
            scopeFactory.Verify();
            scope.Verify(x => x.Dispose());
            serviceProvider.Verify();
            contextAccessor.VerifyGet(x => x.HttpContext);

            contextAccessor.VerifyNoOtherCalls();
            serviceProvider.VerifyNoOtherCalls();
            scopeFactory.VerifyNoOtherCalls();
            scope.VerifyNoOtherCalls();
        }

        [Fact]
        public void GetService_ThrowsException_WhenHttpContextServiceIsUnregistered()
        {
            var scope = new Mock<IServiceScope>();
            var scopeFactory = new Mock<IServiceScopeFactory>();
            var serviceProvider = new Mock<IServiceProvider>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var httpContext = new Mock<HttpContext>();
            serviceProvider.Setup(x => x.GetService(typeof(object))).Returns((object) null).Verifiable();
            contextAccessor.SetupGet(x => x.HttpContext).Returns(httpContext.Object);
            httpContext.SetupGet(x => x.RequestServices).Returns(serviceProvider.Object);

            var resolver = new DependencyResolver(serviceProvider.Object, contextAccessor.Object);
            Should.Throw<InvalidOperationException>(() => resolver.GetService<object>());

            serviceProvider.Verify();
            contextAccessor.VerifyGet(x => x.HttpContext);
            httpContext.VerifyGet(x => x.RequestServices);

            contextAccessor.VerifyNoOtherCalls();
            serviceProvider.VerifyNoOtherCalls();
            scopeFactory.VerifyNoOtherCalls();
            scope.VerifyNoOtherCalls();
        }

        [Fact]
        public void GetService_ThrowsException_WhenServiceProviderServiceIsUnregistered()
        {
            var scope = new Mock<IServiceScope>();
            var scopeFactory = new Mock<IServiceScopeFactory>();
            var serviceProvider = new Mock<IServiceProvider>();
            var scopeServiceProvider = new Mock<IServiceProvider>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            scopeFactory.Setup(x => x.CreateScope()).Returns(scope.Object).Verifiable();
            scope.SetupGet(x => x.ServiceProvider).Returns(scopeServiceProvider.Object).Verifiable();
            serviceProvider.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(scopeFactory.Object)
                .Verifiable();
            scopeServiceProvider.Setup(x => x.GetService(typeof(object))).Returns((object) null).Verifiable();

            var resolver = new DependencyResolver(serviceProvider.Object, contextAccessor.Object);
            Should.Throw<InvalidOperationException>(() => resolver.GetService<object>());

            scope.Verify();
            scopeFactory.Verify();
            scope.Verify(x => x.Dispose());
            serviceProvider.Verify();
            scopeServiceProvider.Verify();
            contextAccessor.VerifyGet(x => x.HttpContext);

            contextAccessor.VerifyNoOtherCalls();
            serviceProvider.VerifyNoOtherCalls();
            scopeFactory.VerifyNoOtherCalls();
            scope.VerifyNoOtherCalls();
            scopeServiceProvider.VerifyNoOtherCalls();
        }

        [Fact]
        public void ValidatesParameters()
        {
            Should.Throw<ArgumentNullException>(() =>
                new DependencyResolver(null, new Mock<IHttpContextAccessor>().Object));
            Should.Throw<ArgumentNullException>(() =>
                new DependencyResolver(new Mock<IServiceProvider>().Object, null));
        }
    }
}