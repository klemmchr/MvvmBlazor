namespace MvvmBlazor.Tests.Internal.Parameters;

public class ParameterResolverTests : UnitTest
{
    public ParameterResolverTests(ITestOutputHelper outputHelper) : base(outputHelper) { }

    protected override void RegisterServices(IServiceCollection services)
    {
        services.Mock<IParameterCache>();
        services.Provide<ParameterResolver>();
    }

    [Fact]
    public void ResolveParameters_ignores_properties_without_attribute()
    {
        var resolver = Services.GetRequiredService<ParameterResolver>();

        var res = resolver.ResolveParameters(typeof(S3), typeof(S3));
        res.Parameters.Count.ShouldBe(1);
        res.Parameters.ElementAt(0).Key.Name.ShouldBe(nameof(S3.Test));
        res.Parameters.ElementAt(0).Key.PropertyType.ShouldBe(typeof(string));
    }

    [Fact]
    public void ResolveParameters_ignores_properties_without_public_setter()
    {
        var resolver = Services.GetRequiredService<ParameterResolver>();

        var res = resolver.ResolveParameters(typeof(S4), typeof(S4));
        res.Parameters.Count.ShouldBe(1);
        res.Parameters.ElementAt(0).Key.Name.ShouldBe(nameof(S4.Test));
        res.Parameters.ElementAt(0).Key.PropertyType.ShouldBe(typeof(string));
    }

    [Fact]
    public void ResolveParameters_resolves_multiple_parameters()
    {
        var resolver = Services.GetRequiredService<ParameterResolver>();

        var res = resolver.ResolveParameters(typeof(S2), typeof(S2));
        res.Parameters.Count.ShouldBe(2);
        res.Parameters.ElementAt(0).Key.Name.ShouldBe(nameof(S2.Foo));
        res.Parameters.ElementAt(0).Key.PropertyType.ShouldBe(typeof(int));
        res.Parameters.ElementAt(1).Key.Name.ShouldBe(nameof(S2.Test));
        res.Parameters.ElementAt(1).Key.PropertyType.ShouldBe(typeof(string));
    }

    [Fact]
    public void ResolveParameters_resolves_single_parameter()
    {
        var resolver = Services.GetRequiredService<ParameterResolver>();

        var res = resolver.ResolveParameters(typeof(S1), typeof(S1));
        res.Parameters.Count.ShouldBe(1);
        res.Parameters.ElementAt(0).Key.Name.ShouldBe(nameof(S1.Test));
        res.Parameters.ElementAt(0).Key.PropertyType.ShouldBe(typeof(string));
    }

    [Fact]
    public void ResolveParameters_resolves_cascading_parameter()
    {
        var resolver = Services.GetRequiredService<ParameterResolver>();

        var res = resolver.ResolveParameters(typeof(S5), typeof(S5));
        res.Parameters.Count.ShouldBe(1);
        res.Parameters.ElementAt(0).Key.Name.ShouldBe(nameof(S5.Test));
        res.Parameters.ElementAt(0).Key.PropertyType.ShouldBe(typeof(string));
    }

    private class S1
    {
        [Parameter] public string? Test { get; set; }
    }

    private class S2
    {
        [Parameter] public string? Test { get; set; }

        [Parameter] public int Foo { get; set; }
    }

    private class S3
    {
        [Parameter] public string? Test { get; set; }

        public int Foo { get; set; }
    }

    private class S4
    {
        [Parameter] public string? Test { get; set; }

        public int Foo { get; }
        public int Doo { get; private set; }
        public int Boo { get; internal set; }
        public int Loo { get; protected set; }
        public int Moo { get; protected internal set; }
    }

    private class S5
    {
        [CascadingParameter] public string? Test { get; set; }
    }
}