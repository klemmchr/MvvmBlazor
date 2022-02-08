using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MvvmBlazor.CodeGenerators
{
    internal class MvvmSyntaxReceiver : ISyntaxContextReceiver
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
                .Any(ad => ad.AttributeClass != null && ad.AttributeClass.Name == "MvvmComponentAttribute");
            if (isDecoratedWithAttribute)
            {
                ComponentClassContexts.Add(new MvvmComponentClassContext(cds, typeSymbol));
            }
        }
    }
}