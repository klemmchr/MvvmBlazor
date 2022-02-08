using System;

using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace MvvmBlazor.Tests.Components;

public class MvvmComponentBaseTTests : UnitTest
{
    public MvvmComponentBaseTTests(ITestOutputHelper outputHelper) : base(outputHelper) { }

    protected override void RegisterServices(IServiceCollection services)
    {
        var binder = services.StrictMock<IBinder>();
        services.StrictMock<ViewModelBase>();
        services.StrictMock<IViewModelParameterSetter>();
        services.AddSingleton<MockMvvmComponentBase>();
        services.AddSingleton<TestViewModel>();

        binder.SetupSet(x => x.ValueChangedCallback = It.IsAny<Action<IBinding, EventArgs>>()).Verifiable();
    }

    private (Mock<ViewModelBase> viewModel, Mock<IServiceProvider> serviceProvider) GetServiceProvider()
    {
        var viewModel = new Mock<ViewModelBase>();
        var serviceProvider = new Mock<IServiceProvider>();
        var bindingFactory = new Mock<IBindingFactory>();
        var viewModelParameterSetter = new Mock<IViewModelParameterSetter>();
        serviceProvider.Setup(x => x.GetService(typeof(ViewModelBase))).Returns(viewModel.Object);
        serviceProvider.Setup(x => x.GetService(typeof(IBindingFactory))).Returns(bindingFactory.Object);
        serviceProvider.Setup(x => x.GetService(typeof(IViewModelParameterSetter)))
            .Returns(viewModelParameterSetter.Object);
        serviceProvider.Setup(x => x.GetService(typeof(Mock<IViewModelParameterSetter>)))
            .Returns(viewModelParameterSetter);


        return (viewModel, serviceProvider);
    }

    [Fact]
    public void AfterRender_CalledOnBindingContext()
    {
        var viewModel = Services.GetMock<ViewModelBase>();
        viewModel.Setup(x => x.OnAfterRender(true)).Verifiable();

        var component = Services.GetRequiredService<MockMvvmComponentBase>();
        component.AfterRender(true);

        viewModel.Verify();
    }

    [Fact]
    public void AfterRenderAsync_CalledOnBindingContext()
    {
        var task = new Task(() => { });
        var viewModel = Services.GetMock<ViewModelBase>();
        var component = Services.GetRequiredService<MockMvvmComponentBase>();

        viewModel.Setup(x => x.OnAfterRenderAsync(It.IsAny<bool>())).Returns(task).Verifiable();

        var res = component.AfterRenderAsync(true);
        res.ShouldBe(task);

        viewModel.Verify();
    }

    [Fact]
    public void Bind_BindsBindingContext()
    {
        var viewModel = Services.GetRequiredService<TestViewModel>();
        var binder = Services.GetMock<IBinder>();

        binder.Setup(x => x.Bind(viewModel, x => x.TestProperty));

        var component = new Mock<MvvmComponentBase<TestViewModel>>(Services);

        component.Setup(x => x.AddBinding(viewModel, x => x.TestProperty)).Verifiable();
        component.Object.Bind(x => x.TestProperty);

        component.Verify();
        component.VerifyNoOtherCalls();
    }

    [Fact]
    public void OnInitialized_CalledOnBindingContext()
    {
        var viewModel = Services.GetMock<ViewModelBase>();
        var viewModelParameterSetter = Services.GetMock<IViewModelParameterSetter>();
        var component = Services.GetRequiredService<MockMvvmComponentBase>();

        viewModel.Setup(x => x.OnInitialized()).Verifiable();
        viewModelParameterSetter.Setup(x => x.ResolveAndSet(component, viewModel.Object)).Verifiable();

        component.Initialized();

        viewModel.Verify();
    }

    [Fact]
    public void OnInitializedAsync_CalledOnBindingContext()
    {
        var task = new Task(() => { });

        var viewModel = Services.GetMock<ViewModelBase>();
        var component = Services.GetRequiredService<MockMvvmComponentBase>();

        viewModel.Setup(x => x.OnInitializedAsync()).Returns(task).Verifiable();

        var res = component.InitializedAsync();
        res.ShouldBe(task);

        viewModel.Verify();
    }

    [Fact]
    public void OnParametersSet_SetsViewModelParameters()
    {
        var viewModel = Services.GetMock<ViewModelBase>();
        var viewModelParameterSetter = Services.GetMock<IViewModelParameterSetter>();
        var component = Services.GetRequiredService<MockMvvmComponentBase>();

        viewModel.Setup(x => x.OnParametersSet()).Verifiable();
        viewModelParameterSetter.Setup(x => x.ResolveAndSet(component, viewModel.Object)).Verifiable();

        component.ParametersSet();

        viewModelParameterSetter.Verify();
    }

    [Fact]
    public void OnParametersSetAsync_CalledOnBindingContext()
    {
        var task = new Task(() => { });

        var viewModel = Services.GetMock<ViewModelBase>();
        var component = Services.GetRequiredService<MockMvvmComponentBase>();

        viewModel.Setup(x => x.OnParametersSetAsync()).Returns(task).Verifiable();

        var res = component.ParametersSetAsync();
        res.ShouldBe(task);

        viewModel.Verify();
    }

    [Fact]
    public void SetsBindingContext()
    {
        var viewModel = Services.GetMock<ViewModelBase>();
        var component = Services.GetRequiredService<MockMvvmComponentBase>();

        component.Context.ShouldBe(viewModel.Object);
    }

    [Fact]
    public void ShouldRender_CalledOnBindingContext()
    {
        var viewModel = Services.GetMock<ViewModelBase>();
        var component = Services.GetRequiredService<MockMvvmComponentBase>();

        viewModel.Setup(x => x.ShouldRender()).Returns(true).Verifiable();

        var res = component.Render();
        res.ShouldBe(true);

        viewModel.Verify();
    }

    private class MockMvvmComponentBase : MvvmComponentBase<ViewModelBase>
    {
        public ViewModelBase Context => BindingContext;
        public MockMvvmComponentBase(IServiceProvider serviceProvider) : base(serviceProvider) { }

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