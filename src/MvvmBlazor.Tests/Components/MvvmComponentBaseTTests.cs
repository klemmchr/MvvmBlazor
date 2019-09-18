using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions.Interfaces;
using Moq;
using MvvmBlazor.Components;
using MvvmBlazor.Internal.Bindings;
using MvvmBlazor.Internal.WeakEventListener;
using MvvmBlazor.ViewModel;
using Shouldly;
using Xunit;

namespace MvvmBlazor.Tests.Components
{
    public class MvvmComponentBaseTTests
    {
        private (Mock<ViewModelBase> viewModel, Mock<IDependencyResolver> resolver) GetResolver()
        {
            var viewModel = new Mock<ViewModelBase>();
            var resolver = new Mock<IDependencyResolver>();
            var wemf = new Mock<IWeakEventManagerFactory>();
            resolver.Setup(x => x.GetService<ViewModelBase>()).Returns(viewModel.Object);
            resolver.Setup(x => x.GetService<IWeakEventManagerFactory>()).Returns(wemf.Object);

            return (viewModel, resolver);
        }

        [Fact]
        public void OnInitialized_CalledOnBindingContext()
        {
            var task = new Task(() => { });

            var (viewModel, resolver) = GetResolver();

            var component = new MockMvvmComponentBase(resolver.Object);
            component.Initialized();

            viewModel.Verify(x => x.OnInitialized());
            viewModel.VerifyNoOtherCalls();
        }

        [Fact]
        public void OnInitializedAsync_CalledOnBindingContext()
        {
            var task = new Task(() => { });

            var (viewModel, resolver) = GetResolver();
            viewModel.Setup(x => x.OnInitializedAsync()).Returns(task);

            var component = new MockMvvmComponentBase(resolver.Object);
            var res = component.InitializedAsync();
            res.ShouldBe(task);

            viewModel.Verify(x => x.OnInitializedAsync());
            viewModel.VerifyNoOtherCalls();
        }

        [Fact]
        public void OnParametersSet_CalledOnBindingContext()
        {
            var (viewModel, resolver) = GetResolver();

            var component = new MockMvvmComponentBase(resolver.Object);
            component.ParametersSet();

            viewModel.Verify(x => x.OnParametersSet());
            viewModel.VerifyNoOtherCalls();
        }

        [Fact]
        public void OnParametersSetAsync_CalledOnBindingContext()
        {
            var task = new Task(() => { });
            var (viewModel, resolver) = GetResolver();
            viewModel.Setup(x => x.OnParametersSetAsync()).Returns(task);

            var component = new MockMvvmComponentBase(resolver.Object);
            var res = component.ParametersSetAsync();
            res.ShouldBe(task);

            viewModel.Verify(x => x.OnParametersSetAsync());
            viewModel.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldRender_CalledOnBindingContext()
        {
            var (viewModel, resolver) = GetResolver();
            viewModel.Setup(x => x.ShouldRender()).Returns(true);

            var component = new MockMvvmComponentBase(resolver.Object);
            var res = component.Render();
            res.ShouldBe(true);

            viewModel.Verify(x => x.ShouldRender());
            viewModel.VerifyNoOtherCalls();
        }

        [Fact]
        public void AfterRender_CalledOnBindingContext()
        {
            var (viewModel, resolver) = GetResolver();
            
            var component = new MockMvvmComponentBase(resolver.Object);
            component.AfterRender(true);

            viewModel.Verify(x => x.OnAfterRender(true));
            viewModel.VerifyNoOtherCalls();
        }

        [Fact]
        public void AfterRenderAsync_CalledOnBindingContext()
        {
            var task = new Task(() => { });
            var (viewModel, resolver) = GetResolver();
            viewModel.Setup(x => x.OnAfterRenderAsync(It.IsAny<bool>())).Returns(task);

            var component = new MockMvvmComponentBase(resolver.Object);
            var res = component.AfterRenderAsync(true);
            res.ShouldBe(task);

            viewModel.Verify(x => x.OnAfterRenderAsync(true));
            viewModel.VerifyNoOtherCalls();
        }

        [Fact]
        public void SetsBindingContext()
        {
            var (viewModel, resolver) = GetResolver();
            var component = new MockMvvmComponentBase(resolver.Object);
            component.Context.ShouldBe(viewModel.Object);
        }

        [Fact]
        public void Bind_BindsBindingContext()
        {
            var viewModel = new TestViewModel();
            var resolver = new Mock<IDependencyResolver>();
            var wemf = new Mock<IWeakEventManagerFactory>();
            var wem = new Mock<IWeakEventManager>();
            var bindingFactory = new Mock<IBindingFactory>();
            var binding = new Mock<IBinding>();
            wemf.Setup(x => x.Create()).Returns(wem.Object);
            resolver.Setup(x => x.GetService<TestViewModel>()).Returns(viewModel);
            resolver.Setup(x => x.GetService<IWeakEventManagerFactory>()).Returns(wemf.Object);
            resolver.Setup(x => x.GetService<IBindingFactory>()).Returns(bindingFactory.Object);
            bindingFactory.Setup(x => x.Create(It.IsAny<INotifyPropertyChanged>(), It.IsAny<PropertyInfo>(),
                It.IsAny<IWeakEventManager>())).Returns(binding.Object);
            
            var component = new Mock<MvvmComponentBase<TestViewModel>>(resolver.Object);
            component.Object.Bind(x => x.TestProperty);

            component.Verify(x => x.AddBinding(viewModel, It.IsAny<Expression<Func<TestViewModel, string>>>()));
            component.VerifyNoOtherCalls();
        }

        private class MockMvvmComponentBase : MvvmComponentBase<ViewModelBase>
        {
            public MockMvvmComponentBase(IDependencyResolver dependencyResolver) : base(dependencyResolver) { }

            public ViewModelBase Context => BindingContext;

            public void Initialized()
            {
                OnInitialized();
            }

            public Task InitializedAsync()
            {
                return OnInitializedAsync();
            }

            public void ParametersSet()
            {
                OnParametersSet();
            }

            public Task ParametersSetAsync()
            {
                return OnParametersSetAsync();
            }

            public bool Render()
            {
                return ShouldRender();
            }

            public void AfterRender(bool firstRender)
            {
                OnAfterRender(firstRender);
            }

            public Task AfterRenderAsync(bool firstRender)
            {
                return OnAfterRenderAsync(firstRender);
            }

            public Task ParametersAsync(ParameterView parameters)
            {
                return SetParametersAsync(parameters);
            }
        }
    }
}