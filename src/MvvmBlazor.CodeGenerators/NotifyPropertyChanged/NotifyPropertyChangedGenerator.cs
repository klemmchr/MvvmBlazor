namespace MvvmBlazor.CodeGenerators.NotifyPropertyChanged;

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
    }

}