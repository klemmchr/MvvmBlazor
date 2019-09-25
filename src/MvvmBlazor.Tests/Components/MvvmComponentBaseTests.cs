using System;
using System.ComponentModel;
using System.Reflection;
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
        internal class TestComponent : MvvmComponentBase
        {
            internal TestComponent(IDependencyResolver dependencyResolver) : base(dependencyResolver) { }
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
            var dependencyResolver = new Mock<IDependencyResolver>();
            var wemf = new Mock<IWeakEventManagerFactory>();
            var wem = new Mock<IWeakEventManager>();
            var bindingFactory = new Mock<IBindingFactory>();
            var binding = new Mock<IBinding>();
            dependencyResolver.Setup(x => x.GetService<IWeakEventManagerFactory>()).Returns(wemf.Object).Verifiable();
            dependencyResolver.Setup(x => x.GetService<IBindingFactory>()).Returns(bindingFactory.Object).Verifiable();
            wemf.Setup(x => x.Create()).Returns(wem.Object).Verifiable();
            bindingFactory.Setup(x => x.Create(It.IsAny<INotifyPropertyChanged>(), It.IsAny<PropertyInfo>(),
                It.IsAny<IWeakEventManager>())).Returns(binding.Object).Verifiable();
            binding.Setup(x => x.GetValue()).Returns("Test").Verifiable();

            var component = new TestComponent(dependencyResolver.Object);
            var res = component.AddBinding(viewModel, x => x.TestProperty);
            res.ShouldBe("Test");

            dependencyResolver.Verify();
            wemf.Verify();
            bindingFactory.Verify();
            binding.Verify();
            binding.Verify(x => x.Initialize());
            wem.Verify(x => x.AddWeakEventListener(It.IsAny<IBinding>(), nameof(IBinding.BindingValueChanged),
                It.IsAny<Action<IBinding, EventArgs>>()));
            dependencyResolver.VerifyNoOtherCalls();
            wemf.VerifyNoOtherCalls();
            bindingFactory.VerifyNoOtherCalls();
            binding.VerifyNoOtherCalls();
            wem.VerifyNoOtherCalls();
        }

        [Fact]
        public void AddBinding_SkipsAddingBindingIfAlreadyExists()
        {
            var viewModel = new TestViewModel();
            var dependencyResolver = new Mock<IDependencyResolver>();
            var wemf = new Mock<IWeakEventManagerFactory>();
            var wem = new Mock<IWeakEventManager>();
            var bindingFactory = new Mock<IBindingFactory>();
            var binding = new Mock<IBinding>();
            dependencyResolver.Setup(x => x.GetService<IWeakEventManagerFactory>()).Returns(wemf.Object).Verifiable();
            dependencyResolver.Setup(x => x.GetService<IBindingFactory>()).Returns(bindingFactory.Object).Verifiable();
            wemf.Setup(x => x.Create()).Returns(wem.Object).Verifiable();
            bindingFactory.Setup(x => x.Create(It.IsAny<INotifyPropertyChanged>(), It.IsAny<PropertyInfo>(),
                It.IsAny<IWeakEventManager>())).Returns(binding.Object).Verifiable();
            binding.Setup(x => x.GetValue()).Returns("Test").Verifiable();

            var component = new TestComponent(dependencyResolver.Object);
            var res = component.AddBinding(viewModel, x => x.TestProperty);
            res.ShouldBe("Test");

            res = component.AddBinding(viewModel, x => x.TestProperty);
            res.ShouldBe("Test");

            dependencyResolver.Verify();
            wemf.Verify();
            bindingFactory.Verify();
            binding.Verify();
            binding.Verify(x => x.Initialize());
            wem.Verify(x => x.AddWeakEventListener(It.IsAny<IBinding>(), nameof(IBinding.BindingValueChanged),
                It.IsAny<Action<IBinding, EventArgs>>()));
            dependencyResolver.VerifyNoOtherCalls();
            wemf.VerifyNoOtherCalls();
            bindingFactory.VerifyNoOtherCalls();
            binding.VerifyNoOtherCalls();
            wem.VerifyNoOtherCalls();
        }

        [Fact]
        public void AddBinding_Throws_WhenBindingMemberIsAField()
        {
            var viewModel = new TestViewModel();
            var dependencyResolver = new Mock<IDependencyResolver>();
            var wemf = new Mock<IWeakEventManagerFactory>();
            var wem = new Mock<IWeakEventManager>();
            var bindingFactory = new Mock<IBindingFactory>();
            var binding = new Mock<IBinding>();
            dependencyResolver.Setup(x => x.GetService<IWeakEventManagerFactory>()).Returns(wemf.Object).Verifiable();
            dependencyResolver.Setup(x => x.GetService<IBindingFactory>()).Returns(bindingFactory.Object).Verifiable();
            wemf.Setup(x => x.Create()).Returns(wem.Object).Verifiable();
            bindingFactory.Setup(x => x.Create(It.IsAny<INotifyPropertyChanged>(), It.IsAny<PropertyInfo>(),
                It.IsAny<IWeakEventManager>())).Returns(binding.Object).Verifiable();

            var component = new TestComponent(dependencyResolver.Object);
            Should.Throw<BindingException>(() => component.AddBinding(viewModel, x => x._testProperty));

            dependencyResolver.Verify();
            wemf.Verify();
            dependencyResolver.VerifyNoOtherCalls();
            wemf.VerifyNoOtherCalls();
            bindingFactory.VerifyNoOtherCalls();
            binding.VerifyNoOtherCalls();
            wem.VerifyNoOtherCalls();
        }

        [Fact]
        public void AddBinding_Throws_WhenBindingMemberIsAMethod()
        {
            var viewModel = new TestViewModel();
            var dependencyResolver = new Mock<IDependencyResolver>();
            var wemf = new Mock<IWeakEventManagerFactory>();
            var wem = new Mock<IWeakEventManager>();
            var bindingFactory = new Mock<IBindingFactory>();
            var binding = new Mock<IBinding>();
            dependencyResolver.Setup(x => x.GetService<IWeakEventManagerFactory>()).Returns(wemf.Object).Verifiable();
            dependencyResolver.Setup(x => x.GetService<IBindingFactory>()).Returns(bindingFactory.Object).Verifiable();
            wemf.Setup(x => x.Create()).Returns(wem.Object).Verifiable();
            bindingFactory.Setup(x => x.Create(It.IsAny<INotifyPropertyChanged>(), It.IsAny<PropertyInfo>(),
                It.IsAny<IWeakEventManager>())).Returns(binding.Object).Verifiable();

            var component = new TestComponent(dependencyResolver.Object);
            Should.Throw<BindingException>(() => component.AddBinding(viewModel, x => x.ShouldRender()));

            dependencyResolver.Verify();
            wemf.Verify();
            dependencyResolver.VerifyNoOtherCalls();
            wemf.VerifyNoOtherCalls();
            bindingFactory.VerifyNoOtherCalls();
            binding.VerifyNoOtherCalls();
            wem.VerifyNoOtherCalls();
        }

        [Fact]
        public void AddBinding_Throws_WhenPropertyExpressionIsNull()
        {
            var viewModel = new TestViewModel();
            var dependencyResolver = new Mock<IDependencyResolver>();
            var wemf = new Mock<IWeakEventManagerFactory>();
            var wem = new Mock<IWeakEventManager>();
            var bindingFactory = new Mock<IBindingFactory>();
            var binding = new Mock<IBinding>();
            dependencyResolver.Setup(x => x.GetService<IWeakEventManagerFactory>()).Returns(wemf.Object).Verifiable();
            dependencyResolver.Setup(x => x.GetService<IBindingFactory>()).Returns(bindingFactory.Object).Verifiable();
            wemf.Setup(x => x.Create()).Returns(wem.Object).Verifiable();
            bindingFactory.Setup(x => x.Create(It.IsAny<INotifyPropertyChanged>(), It.IsAny<PropertyInfo>(),
                It.IsAny<IWeakEventManager>())).Returns(binding.Object).Verifiable();

            var component = new TestComponent(dependencyResolver.Object);
            Should.Throw<BindingException>(() => component.AddBinding<TestViewModel, string>(viewModel, null));

            dependencyResolver.Verify();
            wemf.Verify();
            dependencyResolver.VerifyNoOtherCalls();
            wemf.VerifyNoOtherCalls();
            bindingFactory.VerifyNoOtherCalls();
            binding.VerifyNoOtherCalls();
            wem.VerifyNoOtherCalls();
        }

        [Fact]
        public void AddBinding_Throws_WhenViewModelIsNull()
        {
            var viewModel = new TestViewModel();
            var dependencyResolver = new Mock<IDependencyResolver>();
            var wemf = new Mock<IWeakEventManagerFactory>();
            var wem = new Mock<IWeakEventManager>();
            var bindingFactory = new Mock<IBindingFactory>();
            var binding = new Mock<IBinding>();
            dependencyResolver.Setup(x => x.GetService<IWeakEventManagerFactory>()).Returns(wemf.Object).Verifiable();
            dependencyResolver.Setup(x => x.GetService<IBindingFactory>()).Returns(bindingFactory.Object).Verifiable();
            wemf.Setup(x => x.Create()).Returns(wem.Object).Verifiable();
            bindingFactory.Setup(x => x.Create(It.IsAny<INotifyPropertyChanged>(), It.IsAny<PropertyInfo>(),
                It.IsAny<IWeakEventManager>())).Returns(binding.Object).Verifiable();

            var component = new TestComponent(dependencyResolver.Object);
            Should.Throw<BindingException>(() =>
                component.AddBinding<TestViewModel, string>(null, x => x.TestProperty));

            dependencyResolver.Verify();
            wemf.Verify();
            dependencyResolver.VerifyNoOtherCalls();
            wemf.VerifyNoOtherCalls();
            bindingFactory.VerifyNoOtherCalls();
            binding.VerifyNoOtherCalls();
            wem.VerifyNoOtherCalls();
        }

        [Fact]
        public void Bind_AddsBinding()
        {
            var viewModel = new TestViewModel();
            var dependencyResolver = new Mock<IDependencyResolver>();
            var wemf = new Mock<IWeakEventManagerFactory>();
            var component = new Mock<MvvmComponentBase>(dependencyResolver.Object);
            dependencyResolver.Setup(x => x.GetService<IWeakEventManagerFactory>()).Returns(wemf.Object);
            component.Setup(x => x.AddBinding(viewModel, y => y.TestProperty)).Returns("Test").Verifiable();

            var res = component.Object.Bind(viewModel, x => x.TestProperty);
            res.ShouldBe("Test");

            component.Verify();
        }

        [Fact]
        public void Binding_BindingValueChanged_GetsInvoked()
        {
            var viewModel = new TestViewModel();
            var dependencyResolver = new Mock<IDependencyResolver>();
            var wemf = new Mock<IWeakEventManagerFactory>();
            var wem = new Mock<IWeakEventManager>();
            var bindingFactory = new Mock<IBindingFactory>();
            var binding = new Mock<IBinding>();
            dependencyResolver.Setup(x => x.GetService<IWeakEventManagerFactory>()).Returns(wemf.Object).Verifiable();
            dependencyResolver.Setup(x => x.GetService<IBindingFactory>()).Returns(bindingFactory.Object).Verifiable();
            wemf.Setup(x => x.Create()).Returns(wem.Object).Verifiable();
            wem.Setup(x =>
                    x.AddWeakEventListener(It.IsAny<IBinding>(), nameof(IBinding.BindingValueChanged),
                        It.IsAny<Action<IBinding, EventArgs>>()))
                .Callback<IBinding, string, Action<IBinding, EventArgs>>((b, e, a) =>
                    b.BindingValueChanged += (s, args) => a(b, args));
            bindingFactory.Setup(x => x.Create(It.IsAny<INotifyPropertyChanged>(), It.IsAny<PropertyInfo>(),
                It.IsAny<IWeakEventManager>())).Returns(binding.Object).Verifiable();
            binding.Setup(x => x.GetValue()).Returns("Test").Verifiable();

            var bindingChangedInvoked = false;
            var component = new TestComponent(dependencyResolver.Object);
            component.BindingChangedAction = () => bindingChangedInvoked = true;
            var res = component.AddBinding(viewModel, x => x.TestProperty);

            binding.Raise(x => x.BindingValueChanged += null, EventArgs.Empty);
            res.ShouldBe("Test");
            bindingChangedInvoked.ShouldBeTrue();

            dependencyResolver.Verify();
            wemf.Verify();
            bindingFactory.Verify();
            binding.Verify();
            binding.Verify(x => x.Initialize());
            wem.Verify(x => x.AddWeakEventListener(It.IsAny<IBinding>(), nameof(IBinding.BindingValueChanged),
                It.IsAny<Action<IBinding, EventArgs>>()));
            dependencyResolver.VerifyNoOtherCalls();
            wemf.VerifyNoOtherCalls();
            bindingFactory.VerifyNoOtherCalls();
            binding.VerifyNoOtherCalls();
            wem.VerifyNoOtherCalls();
        }

        [Fact]
        public void Dispose_RemovesEventListenersForBindings()
        {
            var viewModel = new TestViewModel();
            var dependencyResolver = new Mock<IDependencyResolver>();
            var wemf = new Mock<IWeakEventManagerFactory>();
            var wem = new Mock<IWeakEventManager>();
            var bindingFactory = new Mock<IBindingFactory>();
            var binding = new Mock<IBinding>();
            dependencyResolver.Setup(x => x.GetService<IWeakEventManagerFactory>()).Returns(wemf.Object).Verifiable();
            dependencyResolver.Setup(x => x.GetService<IBindingFactory>()).Returns(bindingFactory.Object).Verifiable();
            wemf.Setup(x => x.Create()).Returns(wem.Object).Verifiable();
            bindingFactory.Setup(x => x.Create(It.IsAny<INotifyPropertyChanged>(), It.IsAny<PropertyInfo>(),
                It.IsAny<IWeakEventManager>())).Returns(binding.Object).Verifiable();
            binding.Setup(x => x.GetValue()).Returns("Test").Verifiable();

            var component = new TestComponent(dependencyResolver.Object);
            component.AddBinding(viewModel, x => x.TestProperty);
            component.Dispose();

            dependencyResolver.Verify();
            wemf.Verify();
            bindingFactory.Verify();
            binding.Verify();
            binding.Verify(x => x.Initialize());
            binding.Verify(x => x.Dispose());
            wem.Verify(x => x.AddWeakEventListener(It.IsAny<IBinding>(), nameof(IBinding.BindingValueChanged),
                It.IsAny<Action<IBinding, EventArgs>>()));
            wem.Verify(x => x.RemoveWeakEventListener(It.IsAny<IBinding>()));
            dependencyResolver.VerifyNoOtherCalls();
            wemf.VerifyNoOtherCalls();
            bindingFactory.VerifyNoOtherCalls();
            binding.VerifyNoOtherCalls();
            wem.VerifyNoOtherCalls();
        }
    }
}