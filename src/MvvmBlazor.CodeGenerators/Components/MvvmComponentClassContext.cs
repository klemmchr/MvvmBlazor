namespace MvvmBlazor.CodeGenerators.Components;

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