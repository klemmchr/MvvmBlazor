namespace MvvmBlazor.Tests.Internal.Parameters;

public class ParameterResolverTests
{
    [Fact]
    public void ResolveParameters_IgnoresPropertiesWithoutAttribute()
    {
        var resolver = new ParameterResolver();

        var res = resolver.ResolveParameters(typeof(S3));
        res.Count().ShouldBe(1);
        res.ElementAt(0).Name.ShouldBe("Test");
        res.ElementAt(0).PropertyType.ShouldBe(typeof(string));
    }

    [Fact]
    public void ResolveParameters_IgnoresPropertiesWithoutPublicSetter()
    {
        var resolver = new ParameterResolver();

        var res = resolver.ResolveParameters(typeof(S4));
        res.Count().ShouldBe(1);
        res.ElementAt(0).Name.ShouldBe("Test");
        res.ElementAt(0).PropertyType.ShouldBe(typeof(string));
    }

    [Fact]
    public void ResolveParameters_ResolvesMultipleParameters()
    {
        var resolver = new ParameterResolver();

        var res = resolver.ResolveParameters(typeof(S2));
        res.Count().ShouldBe(2);
        res.ElementAt(0).Name.ShouldBe("Test");
        res.ElementAt(0).PropertyType.ShouldBe(typeof(string));
        res.ElementAt(1).Name.ShouldBe("Foo");
        res.ElementAt(1).PropertyType.ShouldBe(typeof(int));
    }

    [Fact]
    public void ResolveParameters_ResolvesSingleParameter()
    {
        var resolver = new ParameterResolver();

        var res = resolver.ResolveParameters(typeof(S1));
        res.Count().ShouldBe(1);
        res.ElementAt(0).Name.ShouldBe("Test");
        res.ElementAt(0).PropertyType.ShouldBe(typeof(string));
    }

    [Fact]
    public void ResolveParameters_ValidatesParameters()
    {
        var resolver = new ParameterResolver();
        Should.Throw<ArgumentNullException>(() => resolver.ResolveParameters(null));
    }

    private class S1
    {
        [Parameter] public string Test { get; set; }
    }

    private class S2
    {
        [Parameter] public string Test { get; set; }

        [Parameter] public int Foo { get; set; }
    }

    private class S3
    {
        [Parameter] public string Test { get; set; }

        public int Foo { get; set; }
    }

    private class S4
    {
        [Parameter] public string Test { get; set; }

        public int Foo { get; }
        public int Doo { get; private set; }
        public int Boo { get; internal set; }
        public int Loo { get; protected set; }
        public int Moo { get; protected internal set; }
    }
}