namespace MvvmBlazor.CodeGenerators.NotifyPropertyChanged;

[Generator]
public class NotifyPropertyChangedGenerator : ISourceGenerator
{
    private static readonly DiagnosticDescriptor ViewModelNotPartialError = new(
        "MVVMBLAZOR003",
        "View model needs to be partial",
        "View model class '{0}' needs to be partial",
        "MvvmBlazorGenerator",
        DiagnosticSeverity.Error,
        true
    );

    private static readonly DiagnosticDescriptor ViewModelMissingBaseClass = new(
        "MVVMBLAZOR004",
        "Missing view model base class",
        "View model class '{0}' needs to be assignable to '{1}'",
        "MvvmBlazorGenerator",
        DiagnosticSeverity.Error,
        true
    );

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new NotifyPropertyChangedSyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxContextReceiver is not NotifyPropertyChangedSyntaxReceiver syntaxReceiver ||
            syntaxReceiver.Contexts.Count == 0)
        {
            return;
        }

        foreach (var fieldContexts in syntaxReceiver.Contexts)
        {
            ProcessViewModel(context, fieldContexts.Value);
        }
    }

    private void ProcessViewModel(
        GeneratorExecutionContext context,
        IReadOnlyCollection<NotifyPropertyChangedContext> fieldContexts)
    {
        var viewModelClass = fieldContexts.First().Field.Ancestors().OfType<ClassDeclarationSyntax>().First();
        var isPartial = viewModelClass.Modifiers.Any(SyntaxKind.PartialKeyword);
        if (!isPartial)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    ViewModelNotPartialError,
                    Location.Create(
                        viewModelClass.SyntaxTree,
                        TextSpan.FromBounds(viewModelClass.SpanStart, viewModelClass.SpanStart)
                    ),
                    viewModelClass.Identifier
                )
            );
            return;
        }

        var viewModelType = fieldContexts.First().FieldSymbol.ContainingType;
        var viewModelBaseType =
            context.Compilation.GetTypeByMetadataName("MvvmBlazor.ViewModel.ViewModelBase")!;
        if (!viewModelType.InheritsFrom(viewModelBaseType))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    ViewModelMissingBaseClass,
                    Location.Create(
                        viewModelClass.SyntaxTree,
                        TextSpan.FromBounds(viewModelClass.SpanStart, viewModelClass.SpanStart)
                    ),
                    viewModelClass.Identifier
                )
            );
            return;
        }

        AddViewModel(context, fieldContexts, viewModelType, viewModelClass);
    }

    private static void AddViewModel(
        GeneratorExecutionContext context,
        IReadOnlyCollection<NotifyPropertyChangedContext> fieldContexts,
        INamedTypeSymbol viewModelType,
        ClassDeclarationSyntax viewModelClass)
    {
        var viewModelSourceText = SourceText.From(GenerateViewModelCode(fieldContexts, viewModelType, viewModelClass), Encoding.UTF8);
        context.AddSource(viewModelClass.Identifier + ".Generated.cs", viewModelSourceText);
    }

    private static string GenerateViewModelCode(IReadOnlyCollection<NotifyPropertyChangedContext> fieldContexts, INamedTypeSymbol viewModelType, ClassDeclarationSyntax viewModelClass)
    {
        var viewModelNamespace = viewModelType.ContainingNamespace;
        var viewModelClassName = viewModelClass.Identifier;

        var sb = new StringBuilder();

        sb.AppendLine("#nullable enable");
        sb.AppendLine();
        sb.AppendLineFormat("namespace {0}", viewModelNamespace);
        sb.AppendLine("{");
        sb.AppendLineFormat("    partial class {0}", viewModelClassName);
        sb.AppendLine("    {");

        foreach (var fieldContext in fieldContexts)
        {
            var propertyType = fieldContext.FieldSymbol.Type;
            var fieldName = fieldContext.FieldSymbol.Name;
            var propertyName = GetPropertyName(fieldName);
            sb.AppendLineFormat("        public {0} {1}", propertyType, propertyName);
            sb.AppendLine("        {");
            sb.AppendLineFormat("            get => {0};", fieldName);
            sb.AppendLineFormat("            set => Set(ref {0}, value, \"{1}\");", fieldName, propertyName);
            sb.AppendLine("        }");
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }

    private static string GetPropertyName(string fieldName)
    {
        var fieldNameWithoutUnderscore = fieldName.TrimStart('_');
        var firstChar = fieldNameWithoutUnderscore.Substring(0, 1);

        return firstChar.ToUpperInvariant() + fieldNameWithoutUnderscore.Substring(1);
    }
}


