using MvvmBlazor.CodeGenerators.Components;
using Binder = System.Reflection.Binder;

namespace MvvmBlazor.Tests.Generators;

public class MvvmComponentGeneratorTests
{
    [Fact]
    public void GeneratesError_WhenComponentIsNotPartial()
    {
        var inputCompilation = CreateCompilation(
            @$"
                [{nameof(MvvmComponentAttribute)}]
                public class TestComponent : Microsoft.AspNetCore.Components.OwningComponentBase {{}}
            "
        );

        var generator = new MvvmComponentGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out _, out var diagnostics);
        diagnostics.ShouldNotBeEmpty();
        diagnostics.First().Id.ShouldBe("MVVMBLAZOR001");
    }

    [Fact]
    public void GeneratesError_WhenComponentIsNotInheriting()
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
    public void GeneratesComponent_WithComponentBaseClass()
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
    public void GeneratesComponent_WithOwningComponentBaseClass()
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
    public void GeneratesGenericComponent()
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