using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MvvmBlazor.CodeGenerators
{
    internal class MvvmComponentClassContext
    {
        public ClassDeclarationSyntax ComponentClass { get; }
        public INamedTypeSymbol ComponentSymbol { get; }

        public MvvmComponentClassContext(ClassDeclarationSyntax componentClass, INamedTypeSymbol componentSymbol)
        {
            ComponentClass = componentClass;
            ComponentSymbol = componentSymbol;
        }
    }
}