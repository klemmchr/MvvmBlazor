namespace MvvmBlazor.CodeGenerators.Extensions;

internal static class SymbolExtensions
{
    public static string GetMetadataName(this ISymbol symbol)
    {
        return string.Join(".", symbol.ContainingNamespace, symbol.MetadataName);
    }

    public static bool InheritsFrom(this ITypeSymbol symbol, ISymbol baseType)
    {
        if (symbol.BaseType is null)
        {
            return false;
        }

        if (symbol.BaseType.GetMetadataName() == baseType.GetMetadataName())
        {
            return true;
        }

        return symbol.BaseType.InheritsFrom(baseType);
    }
}