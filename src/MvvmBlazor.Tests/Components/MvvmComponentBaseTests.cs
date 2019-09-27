using System;
using System.ComponentModel;
using System.Reflection;
using Moq;
using MvvmBlazor.Components;
using MvvmBlazor.Internal.Bindings;
using MvvmBlazor.Internal.WeakEventListener;
using MvvmBlazor.Tests.TestUtils;
using Shouldly;
using Xunit;

namespace MvvmBlazor.Tests.Components
{
    public class MvvmComponentBaseTests
    {
        internal class TestComponent : MvvmComponentBase
        {
            internal TestComponent(IServiceProvider serviceProvider) : base(serviceProvider) { }
            public Action BindingChangedAction { get; set; }

            internal override void BindingOnBindingValueChanged(object sender, EventArgs e)
            {
                BindingChangedAction?.Invoke();
            }
        }

        [Fact]
        public void AddBinding_AddsBinding()
        {
            var viewModel = new TestViewModel();
            var serviceProvider = new SafeMock<IServiceProvider>();
            var wemf = new SafeMock<IWeakEventManagerFactory>();
            var wem = new SafeMock<IWeakEventManager>();
            var bindingFactory = new SafeMock<IBindingFactory>();
            var binding = new SafeMock<IBinding>();
            serviceProvider.Setup(x => x.GetService(typeof(IWeakEventManagerFactory))).Returns(wemf.Object)
                .Verifiable();
            serviceProvider.Setup(x => x.GetService(typeof(IBindingFactory))).Returns(bindingFactory.Object)
                .Verifiable();
            wem.Setup(x => x.AddWeakEventListener(It.IsAny<IBinding>(), nameof(IBinding.BindingValueChanged),
                It.IsAny<Action<IBinding, EventArgs>>())).Verifiable();
            wemf.Setup(x => x.Create()).Returns(wem.Object).Verifiable();
            bindingFactory.Setup(x => x.Create(It.IsAny<INotifyPropertyChanged>(), It.IsAny<PropertyInfo>(),
                It.IsAny<IWeakEventManager>())).Returns(binding.Object).Verifiable();
            binding.Setup(x => x.GetValue()).Returns("Test").Verifiable();
            binding.Setup(x => x.Initialize());

            var component = new TestComponent(serviceProvider.Object);
            var res = component.AddBinding(viewModel, x => x.TestProperty);
            res.ShouldBe("Test");

            serviceProvider.Verify();
            wemf.Verify();
            wem.Verify();
            bindingFactory.Verify();
            binding.Verify();
            binding.Verify(x => x.Initialize());

            serviceProvider.VerifyNoOtherCalls();
            wemf.VerifyNoOtherCalls();
            bindingFactory.VerifyNoOtherCalls();
            binding.VerifyNoOtherCalls();
            wem.VerifyNoOtherCalls();
        }

        [Fact]
        public void AddBinding_SkipsAddingBindingIfAlreadyExists()
        {
            var viewModel = new TestViewModel();
            var serviceProvider = new SafeMock<IServiceProvider>();
            var wemf = new SafeMock<IWeakEventManagerFactory>();
            var wem = new SafeMock<IWeakEventManager>();
            var bindingFactory = new SafeMock<IBindingFactory>();
            var binding = new SafeMock<IBinding>();
            serviceProvider.Setup(x => x.GetService(typeof(IWeakEventManagerFactory))).Returns(wemf.Object)
                .Verifiable();
            serviceProvider.Setup(x => x.GetService(typeof(IBindingFactory))).Returns(bindingFactory.Object)
                .Verifiable();
            wemf.Setup(x => x.Create()).Returns(wem.Object).Verifiable();
            bindingFactory.Setup(x => x.Create(It.IsAny<INotifyPropertyChanged>(), It.IsAny<PropertyInfo>(),
                It.IsAny<IWeakEventManager>())).Returns(binding.Object).Verifiable();
            binding.Setup(x => x.GetValue()).Returns("Test").Verifiable();
            binding.Setup(x => x.Initialize()).Verifiable();
            wem.Setup(x => x.AddWeakEventListener(It.IsAny<IBinding>(), nameof(IBinding.BindingValueChanged),
                It.IsAny<Action<IBinding, EventArgs>>())).Verifiable();

            var component = new TestComponent(serviceProvider.Object);
            var res = component.AddBinding(viewModel, x => x.TestProperty);
            res.ShouldBe("Test");

            res = component.AddBinding(viewModel, x => x.TestProperty);
            res.ShouldBe("Test");

            serviceProvider.Verify();
            wemf.Verify();
            wem.Verify();
            bindingFactory.Verify();
            binding.Verify();
            binding.Verify();

            serviceProvider.VerifyNoOtherCalls();
            wemf.VerifyNoOtherCalls();
            bindingFactory.VerifyNoOtherCalls();
            binding.VerifyNoOtherCalls();
            wem.VerifyNoOtherCalls();
        }

        [Fact]
        public void AddBinding_Throws_WhenBindingMemberIsAField()
        {
            var viewModel = new TestViewModel();
            var serviceProvider = new SafeMock<IServiceProvider>();
            var wemf = new SafeMock<IWeakEventManagerFactory>();
            var wem = new SafeMock<IWeakEventManager>();
            var bindingFactory = new SafeMock<IBindingFactory>();
            var binding = new SafeMock<IBinding>();
            serviceProvider.Setup(x => x.GetService(typeof(IWeakEventManagerFactory))).Returns(wemf.Object)
                .Verifiable();
            serviceProvider.Setup(x => x.GetService(typeof(IBindingFactory))).Returns(bindingFactory.Object)
                .Verifiable();
            wemf.Setup(x => x.Create()).Returns(wem.Object).Verifiable();
            bindingFactory.Setup(x => x.Create(It.IsAny<INotifyPropertyChanged>(), It.IsAny<PropertyInfo>(),
                It.IsAny<IWeakEventManager>())).Returns(binding.Object).Verifiable();

            var component = new TestComponent(serviceProvider.Object);
            Should.Throw<BindingException>(() => component.AddBinding(viewModel, x => x._testProperty));

            serviceProvider.Verify();
            wemf.Verify();
            serviceProvider.VerifyNoOtherCalls();
            wemf.VerifyNoOtherCalls();
            bindingFactory.VerifyNoOtherCalls();
            binding.VerifyNoOtherCalls();
            wem.VerifyNoOtherCalls();
        }

        [Fact]
        public void AddBinding_Throws_WhenBindingMemberIsAMethod()
        {
            var viewModel = new TestViewModel();
            var serviceProvider = new SafeMock<IServiceProvider>();
            var wemf = new SafeMock<IWeakEventManagerFactory>();
            var wem = new SafeMock<IWeakEventManager>();
            var bindingFactory = new SafeMock<IBindingFactory>();
            var binding = new SafeMock<IBinding>();
            serviceProvider.Setup(x => x.GetService(typeof(IWeakEventManagerFactory))).Returns(wemf.Object)
                .Verifiable();
            serviceProvider.Setup(x => x.GetService(typeof(IBindingFactory))).Returns(bindingFactory.Object)
                .Verifiable();
            wemf.Setup(x => x.Create()).Returns(wem.Object).Verifiable();
            bindingFactory.Setup(x => x.Create(It.IsAny<INotifyPropertyChanged>(), It.IsAny<PropertyInfo>(),
                It.IsAny<IWeakEventManager>())).Returns(binding.Object).Verifiable();

            var component = new TestComponent(serviceProvider.Object);
            Should.Throw<BindingException>(() => component.AddBinding(viewModel, x => x.ShouldRender()));

            serviceProvider.Verify();
            wemf.Verify();
            serviceProvider.VerifyNoOtherCalls();
            wemf.VerifyNoOtherCalls();
            bindingFactory.VerifyNoOtherCalls();
            binding.VerifyNoOtherCalls();
            wem.VerifyNoOtherCalls();
        }

        [Fact]
        public void AddBinding_Throws_WhenPropertyExpressionIsNull()
        {
            var viewModel = new TestViewModel();
            var serviceProvider = new SafeMock<IServiceProvider>();
            var wemf = new SafeMock<IWeakEventManagerFactory>();
            var wem = new SafeMock<IWeakEventManager>();
            var bindingFactory = new SafeMock<IBindingFactory>();
            var binding = new SafeMock<IBinding>();
            serviceProvider.Setup(x => x.GetService(typeof(IWeakEventManagerFactory))).Returns(wemf.Object)
                .Verifiable();
            serviceProvider.Setup(x => x.GetService(typeof(IBindingFactory))).Returns(bindingFactory.Object)
                .Verifiable();
            wemf.Setup(x => x.Create()).Returns(wem.Object).Verifiable();
            bindingFactory.Setup(x => x.Create(It.IsAny<INotifyPropertyChanged>(), It.IsAny<PropertyInfo>(),
                It.IsAny<IWeakEventManager>())).Returns(binding.Object).Verifiable();

            var component = new TestComponent(serviceProvider.Object);
            Should.Throw<BindingException>(() => component.AddBinding<TestViewModel, string>(viewModel, null));

            serviceProvider.Verify();
            wemf.Verify();
            serviceProvider.VerifyNoOtherCalls();
            wemf.VerifyNoOtherCalls();
            bindingFactory.VerifyNoOtherCalls();
            binding.VerifyNoOtherCalls();
            wem.VerifyNoOtherCalls();
        }

        [Fact]
        public void AddBinding_Throws_WhenViewModelIsNull()
        {
            var viewModel = new TestViewModel();
            var serviceProvider = new SafeMock<IServiceProvider>();
            var wemf = new SafeMock<IWeakEventManagerFactory>();
            var wem = new SafeMock<IWeakEventManager>();
            var bindingFactory = new SafeMock<IBindingFactory>();
            var binding = new SafeMock<IBinding>();
            serviceProvider.Setup(x => x.GetService(typeof(IWeakEventManagerFactory))).Returns(wemf.Object)
                .Verifiable();
            serviceProvider.Setup(x => x.GetService(typeof(IBindingFactory))).Returns(bindingFactory.Object)
                .Verifiable();
            wemf.Setup(x => x.Create()).Returns(wem.Object).Verifiable();
            bindingFactory.Setup(x => x.Create(It.IsAny<INotifyPropertyChanged>(), It.IsAny<PropertyInfo>(),
                It.IsAny<IWeakEventManager>())).Returns(binding.Object).Verifiable();

            var component = new TestComponent(serviceProvider.Object);
            Should.Throw<BindingException>(() =>
                component.AddBinding<TestViewModel, string>(null, x => x.TestProperty));

            serviceProvider.Verify();
            wemf.Verify();
            serviceProvider.VerifyNoOtherCalls();
            wemf.VerifyNoOtherCalls();
            bindingFactory.VerifyNoOtherCalls();
            binding.VerifyNoOtherCalls();
            wem.VerifyNoOtherCalls();
        }

        [Fact]
        public void Bind_AddsBinding()
        {
            var viewModel = new TestViewModel();
            var serviceProvider = new SafeMock<IServiceProvider>();
            var wemf = new SafeMock<IWeakEventManagerFactory>();
            var wem = new SafeMock<IWeakEventManager>();
            var bindingFactory = new SafeMock<IBindingFactory>();
            var component = new SafeMock<MvvmComponentBase>(serviceProvider.Object);
            serviceProvider.Setup(x => x.GetService(typeof(IBindingFactory))).Returns(bindingFactory.Object);
            serviceProvider.Setup(x => x.GetService(typeof(IWeakEventManagerFactory))).Returns(wemf.Object);
            component.Setup(x => x.AddBinding(viewModel, y => y.TestProperty)).Returns("Test").Verifiable();
            wemf.Setup(x => x.Create()).Returns(wem.Object).Verifiable();

            var res = component.Object.Bind(viewModel, x => x.TestProperty);
            res.ShouldBe("Test");

            component.Verify();
        }

        [Fact]
        public void Binding_BindingValueChanged_GetsInvoked()
        {
            var viewModel = new TestViewModel();
            var serviceProvider = new SafeMock<IServiceProvider>();
            var wemf = new SafeMock<IWeakEventManagerFactory>();
            var wem = new SafeMock<IWeakEventManager>();
            var bindingFactory = new SafeMock<IBindingFactory>();
            var binding = new SafeMock<IBinding>();
            serviceProvider.Setup(x => x.GetService(typeof(IWeakEventManagerFactory))).Returns(wemf.Object)
                .Verifiable();
            serviceProvider.Setup(x => x.GetService(typeof(IBindingFactory))).Returns(bindingFactory.Object)
                .Verifiable();
            wemf.Setup(x => x.Create()).Returns(wem.Object).Verifiable();
            wem.Setup(x =>
                    x.AddWeakEventListener(It.IsAny<IBinding>(), nameof(IBinding.BindingValueChanged),
                        It.IsAny<Action<IBinding, EventArgs>>()))
                .Callback<IBinding, string, Action<IBinding, EventArgs>>((b, e, a) =>
                    b.BindingValueChanged += (s, args) => a(b, args)).Verifiable();
            bindingFactory.Setup(x => x.Create(It.IsAny<INotifyPropertyChanged>(), It.IsAny<PropertyInfo>(),
                It.IsAny<IWeakEventManager>())).Returns(binding.Object).Verifiable();
            binding.Setup(x => x.GetValue()).Returns("Test").Verifiable();
            binding.Setup(x => x.Initialize()).Verifiable();

            var bindingChangedInvoked = false;
            var component = new TestComponent(serviceProvider.Object)
            {
                BindingChangedAction = () => bindingChangedInvoked = true
            };
            var res = component.AddBinding(viewModel, x => x.TestProperty);

            binding.Raise(x => x.BindingValueChanged += null, EventArgs.Empty);
            res.ShouldBe("Test");
            bindingChangedInvoked.ShouldBeTrue();

            serviceProvider.Verify();
            wemf.Verify();
            bindingFactory.Verify();
            binding.Verify();
            wem.Verify();

            serviceProvider.VerifyNoOtherCalls();
            wemf.VerifyNoOtherCalls();
            bindingFactory.VerifyNoOtherCalls();
            binding.VerifyNoOtherCalls();
            wem.VerifyNoOtherCalls();
        }

        [Fact]
        public void Dispose_RemovesEventListenersForBindings()
        {
            var viewModel = new TestViewModel();
            var serviceProvider = new SafeMock<IServiceProvider>();
            var wemf = new SafeMock<IWeakEventManagerFactory>();
            var wem = new SafeMock<IWeakEventManager>();
            var bindingFactory = new SafeMock<IBindingFactory>();
            var binding = new SafeMock<IBinding>();
            serviceProvider.Setup(x => x.GetService(typeof(IWeakEventManagerFactory))).Returns(wemf.Object)
                .Verifiable();
            serviceProvider.Setup(x => x.GetService(typeof(IBindingFactory))).Returns(bindingFactory.Object)
                .Verifiable();
            wemf.Setup(x => x.Create()).Returns(wem.Object).Verifiable();
            wem.Setup(x => x.AddWeakEventListener(It.IsAny<IBinding>(), nameof(IBinding.BindingValueChanged),
                It.IsAny<Action<IBinding, EventArgs>>())).Verifiable();
            wem.Setup(x => x.RemoveWeakEventListener(It.IsAny<IBinding>())).Verifiable();
            bindingFactory.Setup(x => x.Create(It.IsAny<INotifyPropertyChanged>(), It.IsAny<PropertyInfo>(),
                It.IsAny<IWeakEventManager>())).Returns(binding.Object).Verifiable();
            binding.Setup(x => x.GetValue()).Returns("Test").Verifiable();
            binding.Setup(x => x.Initialize()).Verifiable();
            binding.Setup(x => x.Dispose()).Verifiable();
            
            var component = new TestComponent(serviceProvider.Object);
            component.AddBinding(viewModel, x => x.TestProperty);
            component.Dispose();
            
            serviceProvider.Verify();
            wemf.Verify();
            bindingFactory.Verify();
            binding.Verify();
            wem.Verify();
            
            serviceProvider.VerifyNoOtherCalls();
            wemf.VerifyNoOtherCalls();
            bindingFactory.VerifyNoOtherCalls();
            binding.VerifyNoOtherCalls();
            wem.VerifyNoOtherCalls();
        }
    }
}