namespace MvvmBlazor.CodeGenerators.NotifyPropertyChanged;

internal class NotifyPropertyChangedSyntaxReceiver : ISyntaxContextReceiver
{
    public Dictionary<string, List<NotifyPropertyChangedContext>> Contexts { get; } = new();

    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        if (context.Node is not FieldDeclarationSyntax fds || fds.AttributeLists.Count == 0)
        {
            return;
        }

        foreach (var variable in fds.Declaration.Variables)
        {
            var symbol = context.SemanticModel.GetDeclaredSymbol(variable);
            if (symbol is not IFieldSymbol fieldSymbol)
            {
                continue;
            }

            var isDecoratedWithAttribute = fieldSymbol.GetAttributes()
                .Any(ad => ad.AttributeClass is { Name: "NotifyAttribute" });
            if (!isDecoratedWithAttribute)
            {
                return;
            }

            var className = fieldSymbol.ContainingType.Name;

            if (!Contexts.TryGetValue(className, out var fields))
            {
                fields = new List<NotifyPropertyChangedContext>();
                Contexts.Add(className, fields);
            }

            fields.Add(new NotifyPropertyChangedContext(fds, fieldSymbol));
        }
    }
}