using MvvmBlazor.Internal.WeakEventListener;
using Shouldly;
using Xunit;

namespace MvvmBlazor.Tests.Internal.WeakEventListener
{
    public class WeakEventManagerFactoryTests
    {
        [Fact]
        public void Create_ReturnsWeakEventManager()
        {
            var factory = new WeakEventManagerFactory();
            var res = factory.Create();
            res.ShouldNotBeNull();
        }
    }
}