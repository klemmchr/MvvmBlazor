using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using Moq;
using MvvmBlazor.Components;
using MvvmBlazor.Internal.Bindings;
using MvvmBlazor.Internal.WeakEventListener;
using Shouldly;
using Xunit;

namespace MvvmBlazor.Tests.Components
{
    public class MvvmComponentBaseTests
    {
        [Fact]
        public void AddBinding_AddsBinding()
        {
            var viewModel = new TestViewModel();
            var serviceProvider = new Mock<IServiceProvider>();
            var binder = new Mock<IBinder>();
            serviceProvider.Setup(x => x.GetService(typeof(IBinder))).Returns(binder.Object)
                .Verifiable();
            binder.Setup(x => x.Bind(viewModel, y => y.TestProperty)).Returns("Test").Verifiable();
            binder.SetupSet(x => x.ValueChangedCallback = It.IsAny<Action<IBinding, EventArgs>>()).Verifiable();

            var component = new TestComponent(serviceProvider.Object);
            var res = component.AddBinding(viewModel, x => x.TestProperty);
            res.ShouldBe("Test");

            serviceProvider.Verify();
            binder.Verify();

            serviceProvider.VerifyNoOtherCalls();
        }

        [Fact]
        public void Bind_AddsBinding()
        {
            var viewModel = new TestViewModel();
            var serviceProvider = new Mock<IServiceProvider>();
            var binder = new Mock<IBinder>();
            serviceProvider.Setup(x => x.GetService(typeof(IBinder))).Returns(binder.Object)
                .Verifiable();

            var component = new Mock<MvvmComponentBase>(serviceProvider.Object);
            component.Setup(x => x.AddBinding(viewModel, y => y.TestProperty)).Returns("Test").Verifiable();

            var res = component.Object.Bind(viewModel, x => x.TestProperty);
            res.ShouldBe("Test");

            component.Verify();
        }

        internal class TestComponent : MvvmComponentBase
        {
            internal TestComponent(IServiceProvider serviceProvider) : base(serviceProvider) { }
            public Action BindingChangedAction { get; set; }

            internal override void BindingOnBindingValueChanged(object sender, EventArgs e)
            {
                BindingChangedAction?.Invoke();
            }
        }
    }
}