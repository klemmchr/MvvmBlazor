namespace MvvmBlazor.CodeGenerators.Components;

[Generator]
public class MvvmComponentGenerator : ISourceGenerator
{
    private static readonly DiagnosticDescriptor ComponentNotPartialError = new(
        "MVVMBLAZOR001",
        "Component needs to be partial",
        "Mvvm Component class '{0}' needs to be partial",
        "MvvmBlazorGenerator",
        DiagnosticSeverity.Error,
        true
    );

    private static readonly DiagnosticDescriptor ComponentWrongBaseClassError = new(
        "MVVMBLAZOR002",
        "Missing component base class",
        "Mvvm Component class '{0}' needs to be assignable to '{1}'",
        "MvvmBlazorGenerator",
        DiagnosticSeverity.Error,
        true
    );

    private static readonly DiagnosticDescriptor ComponentWrongTypeParameterError = new(
        "MVVMBLAZOR005",
        "Wrong type parameter",
        "Mvvm Component class '{0}' needs to have exactly one type parameter named '{1}'",
        "MvvmBlazorGenerator",
        DiagnosticSeverity.Error,
        true
    );


    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxContextReceiver is not MvvmComponentSyntaxReceiver syntaxReceiver ||
            syntaxReceiver.ComponentClassContexts.Count == 0)
        {
            return;
        }

        foreach (var componentClassContext in syntaxReceiver.ComponentClassContexts)
            ProcessComponent(context, componentClassContext);
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new MvvmComponentSyntaxReceiver());
    }

    private static void ProcessComponent(
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
        if (!componentClassContext.ComponentSymbol.InheritsFrom(componentBaseType))
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
        context.AddSource(componentClass.Identifier + ".g.cs", componentSourceText);
    }

    private static void AddGenericComponent(
        GeneratorExecutionContext context,
        MvvmComponentClassContext componentClassContext,
        TypeDeclarationSyntax componentClass)
    {
        const string typeParameterName = "T";
        var genericComponentSourceText = SourceText.From(
            GenerateGenericComponentCode(componentClassContext, typeParameterName),
            Encoding.UTF8
        );

        var typeParameterList = componentClass.TypeParameterList;
        if (typeParameterList is null || typeParameterList.Parameters.Count != 1 || typeParameterList.Parameters[0].Identifier.ValueText != typeParameterName)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    ComponentWrongTypeParameterError,
                    Location.Create(
                        componentClass.SyntaxTree,
                        TextSpan.FromBounds(componentClass.SpanStart, componentClass.SpanStart)
                    ),
                    componentClass.Identifier,
                    typeParameterName
                )
            );
        }

        context.AddSource(componentClass.Identifier + "T.g.cs", genericComponentSourceText);
    }

    private static string GenerateComponentCode(MvvmComponentClassContext componentClassContext)
    {
        var componentNamespace = componentClassContext.ComponentSymbol.ContainingNamespace;
        var componentClassName = componentClassContext.ComponentClass.Identifier;

        return $@"
#nullable enable
using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MvvmBlazor.Components;
using MvvmBlazor.ViewModel;

namespace {componentNamespace}
{{
    partial class {componentClassName} : IDisposable, IAsyncDisposable
    {{
        private AsyncServiceScope? _scope;

        [Inject] IServiceScopeFactory ScopeFactory {{ get; set; }} = default!;
        [Inject] protected IServiceProvider RootServiceProvider {{ get; set; }} = default!;
        protected bool IsDisposed {{ get; private set; }}

        public MvvmBlazor.Internal.Bindings.IBinder Binder {{ get; private set; }} = null!;

#pragma warning disable CS0109
        protected new IServiceProvider ScopedServices
#pragma warning restore CS0109
        {{
            get
            {{
                if (ScopeFactory == null)
                {{
                    throw new InvalidOperationException(""Services cannot be accessed before the component is initialized."");
                }}

                if (IsDisposed)
                {{
                    throw new ObjectDisposedException(GetType().Name);
                }}

                _scope ??= ScopeFactory.CreateAsyncScope();
                return _scope.Value.ServiceProvider;
            }}
        }}

#pragma warning disable CS8618
        protected internal {componentClassName}(IServiceProvider services)
#pragma warning restore CS8618
        {{
            RootServiceProvider = services;
            ScopeFactory = services.GetRequiredService<IServiceScopeFactory>();
            InitializeDependencies();
        }}

#pragma warning disable CS8618
        protected {componentClassName}()
#pragma warning restore CS8618
        {{
        }}

        private void InitializeDependencies()
        {{
            Binder = ScopedServices.GetRequiredService<MvvmBlazor.Internal.Bindings.IBinder>();
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

        public void Dispose()
        {{
            if (!IsDisposed)
            {{
                _scope?.Dispose();
                _scope = null;
                Dispose(true);
                GC.SuppressFinalize(this);
                IsDisposed = true;
            }}
        }}

        protected virtual void Dispose(bool disposing)
        {{
        }}

        public async ValueTask DisposeAsync()
        {{
            if (!IsDisposed)
            {{
                if (_scope is not null)
                {{
                    await _scope.Value.DisposeAsync();
                    _scope = null;
                }}

                await DisposeAsyncCore();
                Dispose(false);
                GC.SuppressFinalize(this);
                IsDisposed = true;
            }}
        }}

        protected virtual ValueTask DisposeAsyncCore()
        {{
            return ValueTask.CompletedTask;
        }}
    }}
}}
            ";
    }

    private static string GenerateGenericComponentCode(MvvmComponentClassContext componentClassContext, string typeParameterName)
    {
        var componentNamespace = componentClassContext.ComponentSymbol.ContainingNamespace;
        var componentClassName = componentClassContext.ComponentClass.Identifier;

        return $@"
#nullable enable
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MvvmBlazor.Internal.Parameters;
using MvvmBlazor.ViewModel;

namespace {componentNamespace}
{{
    partial class {componentClassName}<{typeParameterName}>
        where T : ViewModelBase
    {{
        private MvvmBlazor.Internal.Parameters.IViewModelParameterSetter? _viewModelParameterSetter;

#pragma warning disable CS8618
        protected internal {componentClassName}(IServiceProvider serviceProvider) : base(serviceProvider)
#pragma warning restore CS8618
        {{
            SetBindingContext();
        }}

#pragma warning disable CS8618
        protected {componentClassName}()
#pragma warning restore CS8618
        {{
        }}

        protected T BindingContext {{ get; set; }}

        private void SetBindingContext()
        {{
            BindingContext ??= ScopedServices.GetRequiredService<T>();
            BindingContext.RootServiceProvider = RootServiceProvider;
        }}

        private void SetParameters()
        {{
            if (IsDisposed)
                return;

            if (BindingContext is null)
                throw new InvalidOperationException($""{{nameof(BindingContext)}} is not set"");

            _viewModelParameterSetter ??= ScopedServices.GetRequiredService<MvvmBlazor.Internal.Parameters.IViewModelParameterSetter>();
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
            SetBindingContext();
            base.OnInitialized();
            BindingContext?.OnInitialized();
        }}

        /// <inheritdoc />
        protected override async Task OnInitializedAsync()
        {{
            await base.OnInitializedAsync();
            await BindingContext!.OnInitializedAsync();
        }}

        /// <inheritdoc />
        protected override void OnParametersSet()
        {{
            SetParameters();
            base.OnParametersSet();
            BindingContext?.OnParametersSet();
        }}

        /// <inheritdoc />
        protected override async Task OnParametersSetAsync()
        {{
            await base.OnParametersSetAsync();
            await BindingContext.OnParametersSetAsync();
        }}

        /// <inheritdoc />
        protected override bool ShouldRender()
        {{
            return BindingContext!.ShouldRender();
        }}

        /// <inheritdoc />
        protected override void OnAfterRender(bool firstRender)
        {{
            base.OnAfterRender(firstRender);
            BindingContext!.OnAfterRender(firstRender);
        }}

        /// <inheritdoc />
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {{
            await base.OnAfterRenderAsync(firstRender);
            await BindingContext!.OnAfterRenderAsync(firstRender);
        }}

        /// <inheritdoc />
        public override async Task SetParametersAsync(ParameterView parameters)
        {{
            await base.SetParametersAsync(parameters);

            if (BindingContext != null)
            {{
                await BindingContext.SetParametersAsync(parameters);
            }}
        }}
    }}
}}
            ";
    }
}
