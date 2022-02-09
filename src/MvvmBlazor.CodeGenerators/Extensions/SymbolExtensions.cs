namespace MvvmBlazor.CodeGenerators.Extensions;

internal static class SymbolExtensions
{
    public static string GetMetadataName(this ISymbol symbol)
    {
        return string.Join(".", symbol.ContainingNamespace, symbol.MetadataName);
    }
}