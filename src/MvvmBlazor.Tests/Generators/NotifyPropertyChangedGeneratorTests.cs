namespace MvvmBlazor.Tests.Generators;

public class NotifyPropertyChangedGeneratorTests
{
    [Fact]
    public void Generates_error_when_viewmodel_is_not_partial()
    {
        var inputCompilation = CreateCompilation(
            @$"
                public class TestViewModel : MvvmBlazor.ViewModel.ViewModelBase
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
    public void Generates_error_when_view_model_is_not_inheriting()
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

    [Fact]
    public void Generates_viewmodel()
    {
        var inputCompilation = CreateCompilation(
            @$"
                public partial class TestViewModel : MvvmBlazor.ViewModel.ViewModelBase
                {{
                    [{typeof(NotifyAttribute)}]
                    private bool _test;

                    public TestViewModel()
                    {{
                        Test = true;
                    }}
                }}
            "
        );

        var generator = new NotifyPropertyChangedGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out _, out var diagnostics);
        diagnostics.ShouldBeEmpty();
    }

    [Fact]
    public void Generates_viewmodel_for_abstract_class()
    {
        var inputCompilation = CreateCompilation(
            @$"
                public abstract partial class TestViewModel : MvvmBlazor.ViewModel.ViewModelBase
                {{
                    [{typeof(NotifyAttribute)}]
                    private bool _test;
                }}

                public TestViewModel()
                {{
                    Test = true;
                }}
            "
        );

        var generator = new NotifyPropertyChangedGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out _, out var diagnostics);
        diagnostics.ShouldBeEmpty();
    }

    [Fact]
    public void Generates_viewmodel_for_abstract_class_with_inheritance()
    {
        var inputCompilation = CreateCompilation(
            @$"
                public abstract partial class BaseViewModel : MvvmBlazor.ViewModel.ViewModelBase
                {{
                    [{typeof(NotifyAttribute)}]
                    private bool _foo;
                }}

                public abstract partial class TestViewModel : BaseViewModel
                {{
                    [{typeof(NotifyAttribute)}]
                    private bool _test;
                }}

                public TestViewModel()
                {{
                    Test = true;
                    Foo = true;
                }}
            "
        );

        var generator = new NotifyPropertyChangedGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out _, out var diagnostics);
        diagnostics.ShouldBeEmpty();
    }


    [Fact]
    public void Generates_viewmodel_for_generic_class()
    {
        var inputCompilation = CreateCompilation(
            @$"
                public abstract partial class TestViewModel<T> : MvvmBlazor.ViewModel.ViewModelBase
                {{
                    [{typeof(NotifyAttribute)}]
                    private bool _test;
                }}

                public TestViewModel()
                {{
                    Test = true;
                }}
            "
        );

        var generator = new NotifyPropertyChangedGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out _, out var diagnostics);
        diagnostics.ShouldBeEmpty();
    }

    [Fact]
    public void Generates_viewmodel_for_abstract_generic_class()
    {
        var inputCompilation = CreateCompilation(
            @$"
                public abstract partial class TestViewModel<T> : MvvmBlazor.ViewModel.ViewModelBase
                {{
                    [{typeof(NotifyAttribute)}]
                    private bool _test;
                }}

                public TestViewModel()
                {{
                    Test = true;
                }}
            "
        );

        var generator = new NotifyPropertyChangedGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out _, out var diagnostics);
        diagnostics.ShouldBeEmpty();
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