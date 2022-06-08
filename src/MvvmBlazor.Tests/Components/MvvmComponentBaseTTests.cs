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

    [Fact]
    public void AfterRender_called_On_binding_context()
    {
        var viewModel = Services.GetMock<ViewModelBase>();
        viewModel.Setup(x => x.OnAfterRender(true)).Verifiable();

        var component = Services.GetRequiredService<MockMvvmComponentBase>();
        component.AfterRender(true);

        viewModel.Verify();
    }

    [Fact]
    public async Task AfterRenderAsync_called_on_binding_context()
    {
        var task = Task.CompletedTask;
        var viewModel = Services.GetMock<ViewModelBase>();
        var component = Services.GetRequiredService<MockMvvmComponentBase>();

        viewModel.Setup(x => x.OnAfterRenderAsync(It.IsAny<bool>())).Returns(task).Verifiable();

        await component.AfterRenderAsync(true);

        viewModel.Verify();
    }

    [Fact]
    public void Bind_binds_binding_context()
    {
        var viewModel = Services.GetRequiredService<TestViewModel>();
        var binder = Services.GetMock<IBinder>();

        binder.Setup(x => x.Bind(viewModel, x => x.TestProperty));

        var component = new Mock<MvvmComponentBase<TestViewModel>>(Services.GetRequiredService<IServiceScopeFactory>());

        component.Setup(x => x.AddBinding(viewModel, x => x.TestProperty)).Verifiable();
        component.Object.Bind(x => x.TestProperty);

        component.Verify();
        component.VerifyNoOtherCalls();
    }

    [Fact]
    public void OnInitialized_called_on_binding_context()
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
    public async Task OnInitializedAsync_called_on_binding_context()
    {
        var task = Task.CompletedTask;

        var viewModel = Services.GetMock<ViewModelBase>();
        var component = Services.GetRequiredService<MockMvvmComponentBase>();

        viewModel.Setup(x => x.OnInitializedAsync()).Returns(task).Verifiable();

        await component.InitializedAsync();

        viewModel.Verify();
    }

    [Fact]
    public void OnParametersSet_sets_viewmodel_parameters()
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
    public async Task OnParametersSetAsync_called_on_binding_context()
    {
        var task = Task.CompletedTask;

        var viewModel = Services.GetMock<ViewModelBase>();
        var component = Services.GetRequiredService<MockMvvmComponentBase>();

        viewModel.Setup(x => x.OnParametersSetAsync()).Returns(task).Verifiable();

        await component.ParametersSetAsync();

        viewModel.Verify();
    }

    [Fact]
    public void Sets_binding_context()
    {
        var viewModel = Services.GetMock<ViewModelBase>();
        var component = Services.GetRequiredService<MockMvvmComponentBase>();

        component.Context.ShouldBe(viewModel.Object);
    }

    [Fact]
    public void ShouldRender_called_on_binding_context()
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
        public MockMvvmComponentBase(IServiceProvider services) : base(services) { }

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
    }
}