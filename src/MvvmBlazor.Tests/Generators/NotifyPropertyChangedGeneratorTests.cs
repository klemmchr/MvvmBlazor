namespace MvvmBlazor.Tests.Generators;

public class NotifyPropertyChangedGeneratorTests
{
    [Fact]
    public void GeneratesError_WhenViewModelIsNotPartial()
    {
        var inputCompilation = CreateCompilation(
            @$"
                public class TestViewModel : MvvmBlazor.ViewModels.ViewModelBase
                {{
                    [{typeof(NotifyAttribute)}]
                    private bool _test;
                }}
            "
        );

        var generator = new NotifyPropertyChangedGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out _, out var diagnostics);
        diagnostics.ShouldNotBeEmpty();
        diagnostics.First().Id.ShouldBe("MVVMBLAZOR003");
    }

    [Fact]
    public void GeneratesError_WhenViewModelIsNotInheriting()
    {
        var inputCompilation = CreateCompilation(
            @$"
                public partial class TestViewModel
                {{
                    [{typeof(NotifyAttribute)}]
                    private bool _test;
                }}
            "
        );

        var generator = new NotifyPropertyChangedGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out _, out var diagnostics);
        diagnostics.ShouldNotBeEmpty();
        diagnostics.First().Id.ShouldBe("MVVMBLAZOR004");
    }

    private static Compilation CreateCompilation(string source)
    {
        return CSharpCompilation.Create(
            "compilation",
            new[] { CSharpSyntaxTree.ParseText(source) },
            new[]
            {
                MetadataReference.CreateFromFile(typeof(MvvmComponentBase).GetTypeInfo().Assembly.Location)
            },
            new CSharpCompilationOptions(OutputKind.ConsoleApplication)
        );
    }
}