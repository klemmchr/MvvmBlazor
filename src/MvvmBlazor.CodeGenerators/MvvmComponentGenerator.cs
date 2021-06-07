using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using MvvmBlazor.CodeGenerators.Extensions;
using System.Diagnostics;
using System.Linq;

namespace MvvmBlazor.CodeGenerators
{
    [Generator]
    public class MvvmComponentGenerator : ISourceGenerator
    {
        private static readonly DiagnosticDescriptor ComponentNotPartialError = new DiagnosticDescriptor(id: "MVVMBLAZOR001",
                                                                                              title: "Component needs to be partial",
                                                                                              messageFormat: "Mvvm Component class '{0}' needs to be partial",
                                                                                              category: "MvvmBlazorGenerator",
                                                                                              DiagnosticSeverity.Error,
                                                                                              isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor ComponentWrongBaseClassError = new DiagnosticDescriptor(id: "MVVMBLAZOR002",
                                                                                              title: "Missing component base class",
                                                                                              messageFormat: "Mvvm Component class '{0}' needs to be assignable to '{1}'",
                                                                                              category: "MvvmBlazorGenerator",
                                                                                              DiagnosticSeverity.Error,
                                                                                              isEnabledByDefault: true);

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxContextReceiver is not MvvmSyntaxReceiver syntaxReceiver ||
                syntaxReceiver.ComponentClass is null ||
                syntaxReceiver.ComponentSymbol is null)
                return;

            var componentClass = syntaxReceiver.ComponentClass;
            if (!componentClass.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                context.ReportDiagnostic(Diagnostic.Create(ComponentNotPartialError, Location.Create(componentClass.SyntaxTree, TextSpan.FromBounds(componentClass.SpanStart,componentClass.SpanStart)), componentClass.Identifier));
                return;
            }

            var componentBaseType = context.Compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Components.ComponentBase");
            if(!IsComponent(syntaxReceiver.ComponentSymbol, componentBaseType!))
            {
                context.ReportDiagnostic(Diagnostic.Create(ComponentWrongBaseClassError, Location.Create(componentClass.SyntaxTree, TextSpan.FromBounds(componentClass.SpanStart,componentClass.SpanStart)), componentClass.Identifier, componentBaseType.GetMetadataName()));
                return;
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            if(!Debugger.IsAttached)
                Debugger.Launch();

            context.RegisterForSyntaxNotifications(() => new MvvmSyntaxReceiver());
        }

        private bool IsComponent(INamedTypeSymbol componentToCheck, INamedTypeSymbol componentBaseType)
        {
            if(componentToCheck.BaseType is null)
                return false;

            if(componentToCheck.BaseType.GetMetadataName() == componentBaseType.GetMetadataName())
                return true;
            
            return IsComponent(componentToCheck.BaseType, componentBaseType);
        }
    }
}
