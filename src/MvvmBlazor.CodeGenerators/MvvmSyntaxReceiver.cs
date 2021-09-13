using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace MvvmBlazor.CodeGenerators
{
    class MvvmSyntaxReceiver : ISyntaxContextReceiver
    {
        public List<MvvmComponentClassContext> ComponentClassContexts { get; } = new();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is not ClassDeclarationSyntax cds || cds.AttributeLists.Count == 0)
                return;

            var symbol = context.SemanticModel.GetDeclaredSymbol(context.Node) as INamedTypeSymbol;
            if (symbol is not null && symbol.GetAttributes().Any(ad => ad.AttributeClass != null &&
                    ad.AttributeClass.ToDisplayString() == "MvvmBlazor.Components.MvvmComponentAttribute"))
            {
                ComponentClassContexts.Add(new MvvmComponentClassContext(cds, symbol));
            }

        }
    }
}
