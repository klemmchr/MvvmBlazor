namespace MvvmBlazor.CodeGenerators.Components;

internal class MvvmComponentSyntaxReceiver : ISyntaxContextReceiver
{
    public List<MvvmComponentClassContext> ComponentClassContexts { get; } = new();

    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        if (context.Node is not ClassDeclarationSyntax cds || cds.AttributeLists.Count == 0)
        {
            return;
        }

        var symbol = context.SemanticModel.GetDeclaredSymbol(context.Node);
        if (symbol is not INamedTypeSymbol typeSymbol)
        {
            return;
        }

        var isDecoratedWithAttribute = typeSymbol.GetAttributes()
            .Any(ad => ad.AttributeClass is { Name: "MvvmComponentAttribute" });
        if (!isDecoratedWithAttribute)
        {
            return;
        }

        ComponentClassContexts.Add(new MvvmComponentClassContext(cds, typeSymbol));
    }
}