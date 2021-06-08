using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using MvvmBlazor.Components;
using MvvmBlazor.CodeGenerators;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MvvmBlazor.Tests.Generators
{

    public class MvvmComponentGeneratorTests
    {
        [Fact]
        public void GeneratesError_WhenComponentIsNotPartial()
        {
            var inputCompilation = CreateCompilation(@$"
                [{nameof(MvvmComponentAttribute)}]
                public class TestComponent {{}}
            ");

            var generator = new MvvmComponentGenerator();
            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

            driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);
            diagnostics.ShouldNotBeEmpty();
            diagnostics.First().Id.ShouldBe("MVVMBLAZOR001");

            outputCompilation.SyntaxTrees.Any().ShouldBeFalse();
        }

        private static Compilation CreateCompilation(string source)
            => CSharpCompilation.Create("compilation",
                new[] { CSharpSyntaxTree.ParseText(source) },
                new[] { MetadataReference.CreateFromFile(typeof(System.Reflection.Binder).GetTypeInfo().Assembly.Location) },
                new CSharpCompilationOptions(OutputKind.ConsoleApplication));
    }
}
