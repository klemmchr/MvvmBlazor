using System;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace MvvmBlazor.Tests.Abstractions
{
    public abstract class UnitTest
    {
        protected IServiceProvider Services { get; }

        protected UnitTest(ITestOutputHelper outputHelper)
        {
            Services = BuildProvider(outputHelper);
        }

        private IServiceProvider BuildProvider(ITestOutputHelper testOutputHelper)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(testOutputHelper);

            RegisterServices(serviceCollection);

            return serviceCollection.BuildServiceProvider();
        }

        protected virtual void RegisterServices(IServiceCollection services)
        {

        }
    }
}