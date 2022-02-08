using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using MvvmBlazor.CodeGenerators;
using MvvmBlazor.Components;
using Shouldly;
using Xunit;
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
                public class TestComponent {{}}
            "
        );

        var generator = new MvvmComponentGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);
        diagnostics.ShouldNotBeEmpty();
        diagnostics.First().Id.ShouldBe("MVVMBLAZOR001");
    }

    [Fact]
    public void GeneratesError_WhenComponentIsNotInheritingFromComponentBase()
    {
        var inputCompilation = CreateCompilation(
            @$"
                [{nameof(MvvmComponentAttribute)}]
                public partial class TestComponent {{}}
            "
        );

        var generator = new MvvmComponentGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);
        diagnostics.ShouldNotBeEmpty();
        diagnostics.First().Id.ShouldBe("MVVMBLAZOR002");
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