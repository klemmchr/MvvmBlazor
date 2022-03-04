using Binder = System.Reflection.Binder;

namespace MvvmBlazor.Tests.Generators;

public class MvvmComponentGeneratorTests
{
    [Fact]
    public void Generates_error_when_component_is_not_partial()
    {
        var inputCompilation = CreateCompilation(
            @$"
                [{nameof(MvvmComponentAttribute)}]
                public class TestComponent : Microsoft.AspNetCore.Components.ComponentBase {{}}
            "
        );

        var generator = new MvvmComponentGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out _, out var diagnostics);
        diagnostics.ShouldNotBeEmpty();
        diagnostics.First().Id.ShouldBe("MVVMBLAZOR001");
    }

    [Fact]
    public void Generates_error_when_component_is_not_inheriting()
    {
        var inputCompilation = CreateCompilation(
            @$"
                [{nameof(MvvmComponentAttribute)}]
                public partial class TestComponent {{}}
            "
        );

        var generator = new MvvmComponentGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out _, out var diagnostics);
        diagnostics.ShouldNotBeEmpty();
        diagnostics.First().Id.ShouldBe("MVVMBLAZOR002");
    }

    [Fact]
    public void Generates_error_when_generic_type_parameter_count_is_wrong()
    {
        var inputCompilation = CreateCompilation(
            @$"
                [{nameof(MvvmComponentAttribute)}]
                public partial class TestComponent<T, T1> : Microsoft.AspNetCore.Components.ComponentBase {{}}
            "
        );

        var generator = new MvvmComponentGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out _, out var diagnostics);
        diagnostics.ShouldNotBeEmpty();
        diagnostics.First().Id.ShouldBe("MVVMBLAZOR005");
    }

    [Fact]
    public void Generates_error_when_generic_type_parameter_name_is_wrong()
    {
        var inputCompilation = CreateCompilation(
            @$"
                [{nameof(MvvmComponentAttribute)}]
                public partial class TestComponent<T1> : Microsoft.AspNetCore.Components.ComponentBase {{}}
            "
        );

        var generator = new MvvmComponentGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out _, out var diagnostics);
        diagnostics.ShouldNotBeEmpty();
        diagnostics.First().Id.ShouldBe("MVVMBLAZOR005");
    }

    [Fact]
    public void Generates_component_with_component_base_class()
    {
        var inputCompilation = CreateCompilation(
            @$"
                [{nameof(MvvmComponentAttribute)}]
                public partial class TestComponent : Microsoft.AspNetCore.Components.ComponentBase {{}}
            "
        );

        var generator = new MvvmComponentGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out _, out var diagnostics);
        diagnostics.ShouldBeEmpty();
    }

    [Fact]
    public void Generates_component_with_owning_component_base_class()
    {
        var inputCompilation = CreateCompilation(
            @$"
                [{nameof(MvvmComponentAttribute)}]
                public partial class TestComponent : Microsoft.AspNetCore.Components.OwningComponentBase {{}}
            "
        );

        var generator = new MvvmComponentGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out _, out var diagnostics);
        diagnostics.ShouldBeEmpty();
    }

    [Fact]
    public void Generates_generic_component()
    {
        var inputCompilation = CreateCompilation(
            @$"
                [{nameof(MvvmComponentAttribute)}]
                public partial class TestComponent<T> : Microsoft.AspNetCore.Components.OwningComponentBase {{}}
            "
        );

        var generator = new MvvmComponentGenerator();
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
                MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ComponentBase).GetTypeInfo().Assembly.Location)
            },
            new CSharpCompilationOptions(OutputKind.ConsoleApplication)
        );
    }
}