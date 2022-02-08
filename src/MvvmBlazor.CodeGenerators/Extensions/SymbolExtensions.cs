using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace MvvmBlazor.CodeGenerators.Extensions
{
    internal static class SymbolExtensions
    {
        public static string GetMetadataName(this ISymbol symbol) =>
            string.Join(".", symbol.ContainingNamespace, symbol.MetadataName);
    }
}
