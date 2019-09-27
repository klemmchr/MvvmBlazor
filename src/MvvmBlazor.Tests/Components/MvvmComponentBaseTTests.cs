using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
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
        private (Mock<ViewModelBase> viewModel, Mock<IServiceProvider> serviceProvider) GetServiceProvider()
        {
            var viewModel = new Mock<ViewModelBase>();
            var serviceProvider = new Mock<IServiceProvider>();
            var wemf = new Mock<IWeakEventManagerFactory>();
            var bindingFactory = new Mock<IBindingFactory>();
            serviceProvider.Setup(x => x.GetService(typeof(ViewModelBase))).Returns(viewModel.Object);
            serviceProvider.Setup(x => x.GetService(typeof(IWeakEventManagerFactory))).Returns(wemf.Object);
            serviceProvider.Setup(x => x.GetService(typeof(IWeakEventManagerFactory))).Returns(wemf.Object);
            serviceProvider.Setup(x => x.GetService(typeof(IBindingFactory))).Returns(bindingFactory.Object);

            return (viewModel, serviceProvider);
        }

        private class MockMvvmComponentBase : MvvmComponentBase<ViewModelBase>
        {
            public MockMvvmComponentBase(IServiceProvider serviceProvider) : base(serviceProvider) { }

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

        [Fact]
        public void AfterRender_CalledOnBindingContext()
        {
            var (viewModel, serviceProvider) = GetServiceProvider();

            var component = new MockMvvmComponentBase(serviceProvider.Object);
            component.AfterRender(true);

            viewModel.Verify(x => x.OnAfterRender(true));
            viewModel.VerifyNoOtherCalls();
        }

        [Fact]
        public void AfterRenderAsync_CalledOnBindingContext()
        {
            var task = new Task(() => { });
            var (viewModel, serviceProvider) = GetServiceProvider();
            viewModel.Setup(x => x.OnAfterRenderAsync(It.IsAny<bool>())).Returns(task);


            var component = new MockMvvmComponentBase(serviceProvider.Object);
            var res = component.AfterRenderAsync(true);
            res.ShouldBe(task);

            viewModel.Verify(x => x.OnAfterRenderAsync(true));
            viewModel.VerifyNoOtherCalls();
        }

        [Fact]
        public void Bind_BindsBindingContext()
        {
            var viewModel = new TestViewModel();
            var serviceProvider = new Mock<IServiceProvider>();
            var wemf = new Mock<IWeakEventManagerFactory>();
            var wem = new Mock<IWeakEventManager>();
            var bindingFactory = new Mock<IBindingFactory>();
            var binding = new Mock<IBinding>();
            wemf.Setup(x => x.Create()).Returns(wem.Object);
            serviceProvider.Setup(x => x.GetService(typeof(TestViewModel))).Returns(viewModel);
            serviceProvider.Setup(x => x.GetService(typeof(IWeakEventManagerFactory))).Returns(wemf.Object);
            serviceProvider.Setup(x => x.GetService(typeof(IBindingFactory))).Returns(bindingFactory.Object);
            bindingFactory.Setup(x => x.Create(It.IsAny<INotifyPropertyChanged>(), It.IsAny<PropertyInfo>(),
                It.IsAny<IWeakEventManager>())).Returns(binding.Object);

            var component = new Mock<MvvmComponentBase<TestViewModel>>(serviceProvider.Object);
            component.Object.Bind(x => x.TestProperty);

            component.Verify(x => x.AddBinding(viewModel, It.IsAny<Expression<Func<TestViewModel, string>>>()));
            component.VerifyNoOtherCalls();
        }

        [Fact]
        public void Dispose_DisposesBindingContext()
        {
            var viewModel = new TestViewModel();
            var serviceProvider = new Mock<IServiceProvider>();
            var wemf = new Mock<IWeakEventManagerFactory>();
            var wem = new Mock<IWeakEventManager>();
            var bindingFactory = new Mock<IBindingFactory>();
            var binding = new Mock<IBinding>();
            wemf.Setup(x => x.Create()).Returns(wem.Object);
            serviceProvider.Setup(x => x.GetService(typeof(TestViewModel))).Returns(viewModel);
            serviceProvider.Setup(x => x.GetService(typeof(IWeakEventManagerFactory))).Returns(wemf.Object);
            serviceProvider.Setup(x => x.GetService(typeof(IBindingFactory))).Returns(bindingFactory.Object);
            bindingFactory.Setup(x => x.Create(It.IsAny<INotifyPropertyChanged>(), It.IsAny<PropertyInfo>(),
                It.IsAny<IWeakEventManager>())).Returns(binding.Object);

            var component = new Mock<MvvmComponentBase<TestViewModel>>(serviceProvider.Object);
            component.Object.Bind(x => x.TestProperty);

            component.Verify(x => x.AddBinding(viewModel, It.IsAny<Expression<Func<TestViewModel, string>>>()));
            component.VerifyNoOtherCalls();
        }

        [Fact]
        public void OnInitialized_CalledOnBindingContext()
        {
            var task = new Task(() => { });

            var (viewModel, serviceProvider) = GetServiceProvider();

            var component = new MockMvvmComponentBase(serviceProvider.Object);
            component.Initialized();

            viewModel.Verify(x => x.OnInitialized());
            viewModel.VerifyNoOtherCalls();
        }

        [Fact]
        public void OnInitializedAsync_CalledOnBindingContext()
        {
            var task = new Task(() => { });

            var (viewModel, serviceProvider) = GetServiceProvider();
            viewModel.Setup(x => x.OnInitializedAsync()).Returns(task);

            var component = new MockMvvmComponentBase(serviceProvider.Object);
            var res = component.InitializedAsync();
            res.ShouldBe(task);

            viewModel.Verify(x => x.OnInitializedAsync());
            viewModel.VerifyNoOtherCalls();
        }

        [Fact]
        public void OnParametersSet_CalledOnBindingContext()
        {
            var (viewModel, serviceProvider) = GetServiceProvider();

            var component = new MockMvvmComponentBase(serviceProvider.Object);
            component.ParametersSet();

            viewModel.Verify(x => x.OnParametersSet());
            viewModel.VerifyNoOtherCalls();
        }

        [Fact]
        public void OnParametersSetAsync_CalledOnBindingContext()
        {
            var task = new Task(() => { });
            var (viewModel, serviceProvider) = GetServiceProvider();
            viewModel.Setup(x => x.OnParametersSetAsync()).Returns(task);

            var component = new MockMvvmComponentBase(serviceProvider.Object);
            var res = component.ParametersSetAsync();
            res.ShouldBe(task);

            viewModel.Verify(x => x.OnParametersSetAsync());
            viewModel.VerifyNoOtherCalls();
        }

        [Fact]
        public void SetsBindingContext()
        {
            var (viewModel, serviceProvider) = GetServiceProvider();
            var component = new MockMvvmComponentBase(serviceProvider.Object);
            component.Context.ShouldBe(viewModel.Object);
        }

        [Fact]
        public void ShouldRender_CalledOnBindingContext()
        {
            var (viewModel, serviceProvider) = GetServiceProvider();
            viewModel.Setup(x => x.ShouldRender()).Returns(true);

            var component = new MockMvvmComponentBase(serviceProvider.Object);
            var res = component.Render();
            res.ShouldBe(true);

            viewModel.Verify(x => x.ShouldRender());
            viewModel.VerifyNoOtherCalls();
        }
    }
}