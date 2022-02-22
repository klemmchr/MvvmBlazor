namespace MvvmBlazor.CodeGenerators.NotifyPropertyChanged;

internal class NotifyPropertyChangedContext
{
    public FieldDeclarationSyntax Field { get; }
    public IFieldSymbol FieldSymbol { get; }

    public NotifyPropertyChangedContext(FieldDeclarationSyntax field, IFieldSymbol fieldSymbol)
    {
        Field = field;
        FieldSymbol = fieldSymbol;
    }
}