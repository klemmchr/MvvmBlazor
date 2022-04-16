namespace MvvmBlazor.Tests.Abstractions;

public abstract class UnitTest : IDisposable
{
    private readonly ServiceProvider _services;
    protected IServiceProvider Services => _services;

    protected UnitTest(ITestOutputHelper outputHelper)
    {
        _services = BuildProvider(outputHelper);
    }

    private ServiceProvider BuildProvider(ITestOutputHelper testOutputHelper)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(testOutputHelper);

        RegisterServices(serviceCollection);

        return serviceCollection.BuildServiceProvider();
    }

    protected virtual void RegisterServices(IServiceCollection services) { }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _services.Dispose();
        }
    }
}