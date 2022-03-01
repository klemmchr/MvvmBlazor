namespace MvvmBlazor.Tests.Components;

public class MvvmComponentBaseTests : UnitTest
{
    public MvvmComponentBaseTests(ITestOutputHelper outputHelper) : base(outputHelper) { }

    protected override void RegisterServices(IServiceCollection services)
    {
        var binder = services.StrictMock<IBinder>();
        services.AddSingleton<TestComponent>();

        binder.SetupSet(x => x.ValueChangedCallback = It.IsAny<Action<IBinding, EventArgs>>()).Verifiable();
    }

    [Fact]
    public void AddBinding_adds_Binding()
    {
        var viewModel = new TestViewModel();
        var binder = Services.GetMock<IBinder>();
        binder.Setup(x => x.Bind(viewModel, y => y.TestProperty)).Returns("Test").Verifiable();

        var component = Services.GetRequiredService<TestComponent>();
        var res = component.AddBinding(viewModel, x => x.TestProperty);
        res.ShouldBe("Test");

        binder.Verify();
    }

    [Fact]
    public void Bind_adds_Binding()
    {
        var viewModel = new TestViewModel();

        var component = new Mock<MvvmComponentBase>(Services.GetRequiredService<IServiceScopeFactory>());
        component.Setup(x => x.AddBinding(viewModel, y => y.TestProperty)).Returns("Test").Verifiable();

        var res = component.Object.Bind(viewModel, x => x.TestProperty);
        res.ShouldBe("Test");

        component.Verify();
    }

    internal class TestComponent : MvvmComponentBase
    {
        public Action? BindingChangedAction { get; set; }


        public TestComponent(IServiceProvider services) : base(services) { }

        internal override void BindingOnBindingValueChanged(object sender, EventArgs e)
        {
            BindingChangedAction?.Invoke();
        }
    }
}