using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using MvvmBlazor.CodeGenerators.Extensions;
using System.Linq;
using System.Text;

namespace MvvmBlazor.CodeGenerators
{
    [Generator]
    public class MvvmComponentGenerator : ISourceGenerator
    {
        private static readonly DiagnosticDescriptor ComponentNotPartialError = new DiagnosticDescriptor(
            id: "MVVMBLAZOR001",
            title: "Component needs to be partial",
            messageFormat: "Mvvm Component class '{0}' needs to be partial",
            category: "MvvmBlazorGenerator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true
        );

        private static readonly DiagnosticDescriptor ComponentWrongBaseClassError = new DiagnosticDescriptor(
            id: "MVVMBLAZOR002",
            title: "Missing component base class",
            messageFormat: "Mvvm Component class '{0}' needs to be assignable to '{1}'",
            category: "MvvmBlazorGenerator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true
        );

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxContextReceiver is not MvvmSyntaxReceiver syntaxReceiver ||
                syntaxReceiver.ComponentClassContexts.Count == 0)
                return;

            foreach (var componentClassContext in syntaxReceiver.ComponentClassContexts)
            {
                ProcessComponent(context, componentClassContext);
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new MvvmSyntaxReceiver());
        }

        private void ProcessComponent(
            GeneratorExecutionContext context,
            MvvmComponentClassContext componentClassContext)
        {
            var componentClass = componentClassContext.ComponentClass;
            var isPartial = componentClass.Modifiers.Any(SyntaxKind.PartialKeyword);
            if (!isPartial)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        ComponentNotPartialError,
                        Location.Create(
                            componentClass.SyntaxTree,
                            TextSpan.FromBounds(componentClass.SpanStart, componentClass.SpanStart)
                        ),
                        componentClass.Identifier
                    )
                );
                return;
            }

            var componentBaseType =
                context.Compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Components.ComponentBase")!;
            if (!IsComponent(componentClassContext.ComponentSymbol, componentBaseType))
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        ComponentWrongBaseClassError,
                        Location.Create(
                            componentClass.SyntaxTree,
                            TextSpan.FromBounds(componentClass.SpanStart, componentClass.SpanStart)
                        ),
                        componentClass.Identifier,
                        componentBaseType.GetMetadataName()
                    )
                );
                return;
            }

            if (componentClass.TypeParameterList is null || componentClass.TypeParameterList.Parameters.Count == 0)
            {
                AddComponent(context, componentClassContext, componentClass);
                return;
            }

            AddGenericComponent(context, componentClassContext, componentClass);
        }

        private static void AddComponent(
            GeneratorExecutionContext context,
            MvvmComponentClassContext componentClassContext,
            BaseTypeDeclarationSyntax componentClass)
        {
            var componentSourceText = SourceText.From(GenerateComponentCode(componentClassContext), Encoding.UTF8);
            context.AddSource(componentClass.Identifier + ".Generated.cs", componentSourceText);
        }

        private static void AddGenericComponent(
            GeneratorExecutionContext context,
            MvvmComponentClassContext componentClassContext,
            TypeDeclarationSyntax componentClass)
        {
            var genericComponentSourceText = SourceText.From(
                GenerateGenericComponentCode(componentClassContext),
                Encoding.UTF8
            );

            var genericPostFix =
                componentClass.TypeParameterList is null || componentClass.TypeParameterList.Parameters.Count == 0
                    ? "T"
                    : string.Join(
                        ",",
                        Enumerable.Range(0, componentClass.TypeParameterList.Parameters.Count).Select(i => $"T{i}")
                    );

            context.AddSource(
                componentClass.Identifier + $"{{{genericPostFix}}}.Generated.cs",
                genericComponentSourceText
            );
        }

        private static bool IsComponent(ITypeSymbol componentToCheck, ISymbol componentBaseType)
        {
            if (componentToCheck.BaseType is null) return false;

            if (componentToCheck.BaseType.GetMetadataName() == componentBaseType.GetMetadataName()) return true;

            return IsComponent(componentToCheck.BaseType, componentBaseType);
        }

        private static string GenerateComponentCode(MvvmComponentClassContext componentClassContext)
        {
            var componentNamespace = componentClassContext.ComponentSymbol.ContainingNamespace;
            var componentClassName = componentClassContext.ComponentClass.Identifier;
            var baseType = componentClassContext.ComponentSymbol.BaseType!.GetMetadataName();

            return $@"
                using System;
                using System.Linq.Expressions;
                using Microsoft.AspNetCore.Components;
                using Microsoft.Extensions.DependencyInjection;
                using MvvmBlazor.Components;
                using MvvmBlazor.ViewModel;

                namespace {componentNamespace}
                {{
                    public partial class {componentClassName}:
                        {baseType}
                    {{
                        public IBinder Binder {{ get; private set; }} = null!;

                        protected internal {componentClassName}(IServiceProvider serviceProvider)
                        {{
                            ServiceProvider = serviceProvider;
                            InitializeDependencies();
                        }}

                        protected {componentClassName}() {{}}

                        [Inject] protected IServiceProvider ServiceProvider {{ get; set; }} = null!;

                        private void InitializeDependencies()
                        {{
                            Binder = ServiceProvider.GetRequiredService<IBinder>();
                            Binder.ValueChangedCallback = BindingOnBindingValueChanged;
                        }}

                        protected internal TValue Bind<TViewModel, TValue>(TViewModel viewModel,
                            Expression<Func<TViewModel, TValue>> property)
                            where TViewModel : ViewModelBase
                        {{
                            return AddBinding(viewModel, property);
                        }}

                        public virtual TValue AddBinding<TViewModel, TValue>(TViewModel viewModel,
                            Expression<Func<TViewModel, TValue>> propertyExpression) where TViewModel : ViewModelBase
                        {{
                            return Binder.Bind(viewModel, propertyExpression);
                        }}

                        protected override void OnInitialized()
                        {{
                            base.OnInitialized();
                            InitializeDependencies();
                        }}

                        internal virtual void BindingOnBindingValueChanged(object sender, EventArgs e)
                        {{
                            InvokeAsync(StateHasChanged);
                        }}
                    }}
                }}
            ";
        }

        private static string GenerateGenericComponentCode(MvvmComponentClassContext componentClassContext)
        {
            return $@"
                using System;
                using System.Linq.Expressions;
                using System.Threading.Tasks;
                using Microsoft.AspNetCore.Components;
                using Microsoft.Extensions.DependencyInjection;
                using MvvmBlazor.Internal.Parameters;
                using MvvmBlazor.ViewModel;

                namespace {componentClassContext.ComponentSymbol!.ContainingNamespace}
                {{
                    public abstract partial class {componentClassContext.ComponentClass!.Identifier}<T> :
                        {componentClassContext.ComponentSymbol!.BaseType!.GetMetadataName()}
                        where T : ViewModelBase
                    {{
                        private IViewModelParameterSetter? _viewModelParameterSetter;

                        protected internal {componentClassContext.ComponentClass!.Identifier}(IServiceProvider serviceProvider) : base(serviceProvider)
                        {{
                            SetBindingContext();
                        }}

                        protected {componentClassContext.ComponentClass!.Identifier}() {{}}

                        protected T? BindingContext {{ get; set; }}

                        private void SetBindingContext()
                        {{
                            BindingContext ??= ServiceProvider.GetRequiredService<T>();
                        }}

                        private void SetParameters()
                        {{
                            if (BindingContext is null)
                                throw new InvalidOperationException($""{{nameof(BindingContext)}} is not set"");

                            _viewModelParameterSetter ??= ServiceProvider.GetRequiredService<IViewModelParameterSetter>();
                            _viewModelParameterSetter.ResolveAndSet(this, BindingContext);
                        }}

                        protected internal TValue Bind<TValue>(Expression<Func<T, TValue>> property)
                        {{
                            if (BindingContext is null)
                                throw new InvalidOperationException($""{{nameof(BindingContext)}} is not set"");

                            return AddBinding(BindingContext, property);
                        }}

                        /// <inheritdoc />
                        protected override void OnInitialized()
                        {{
                            base.OnInitialized();
                            SetBindingContext();
                            SetParameters();
                            BindingContext?.OnInitialized();
                        }}

                        /// <inheritdoc />
                        protected override Task OnInitializedAsync()
                        {{
                            return BindingContext?.OnInitializedAsync() ?? Task.CompletedTask;
                        }}

                        /// <inheritdoc />
                        protected override void OnParametersSet()
                        {{
                            SetParameters();
                            BindingContext?.OnParametersSet();
                        }}

                        /// <inheritdoc />
                        protected override Task OnParametersSetAsync()
                        {{
                            return BindingContext?.OnParametersSetAsync() ?? Task.CompletedTask;
                        }}

                        /// <inheritdoc />
                        protected override bool ShouldRender()
                        {{
                            return BindingContext?.ShouldRender() ?? true;
                        }}

                        /// <inheritdoc />
                        protected override void OnAfterRender(bool firstRender)
                        {{
                            BindingContext?.OnAfterRender(firstRender);
                        }}

                        /// <inheritdoc />
                        protected override Task OnAfterRenderAsync(bool firstRender)
                        {{
                            return BindingContext?.OnAfterRenderAsync(firstRender) ?? Task.CompletedTask;
                        }}

                        /// <inheritdoc />
                        public override async Task SetParametersAsync(ParameterView parameters)
                        {{
                            await base.SetParametersAsync(parameters).ConfigureAwait(false);

                            if (BindingContext != null)
                                await BindingContext.SetParametersAsync(parameters).ConfigureAwait(false);
                        }}
                    }}
                }}
            ";
        }
    }
}