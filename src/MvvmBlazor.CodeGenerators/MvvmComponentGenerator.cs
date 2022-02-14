﻿namespace MvvmBlazor.CodeGenerators;

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

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxContextReceiver is not MvvmSyntaxReceiver syntaxReceiver ||
            syntaxReceiver.ComponentClassContexts.Count == 0)
        {
            return;
        }

        foreach (var componentClassContext in syntaxReceiver.ComponentClassContexts)
            ProcessComponent(context, componentClassContext);
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new MvvmSyntaxReceiver());
    }

    private void ProcessComponent(GeneratorExecutionContext context, MvvmComponentClassContext componentClassContext)
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

        if (componentClass.TypeParameterList is null || componentClass.TypeParameterList.Parameters.Count != 1)
        {
            throw new InvalidOperationException("Expected exactly one type parameter");
        }

        context.AddSource(componentClass.Identifier + "T.Generated.cs", genericComponentSourceText);
    }

    private static bool IsComponent(ITypeSymbol componentToCheck, ISymbol componentBaseType)
    {
        if (componentToCheck.BaseType is null)
        {
            return false;
        }

        if (componentToCheck.BaseType.GetMetadataName() == componentBaseType.GetMetadataName())
        {
            return true;
        }

        return IsComponent(componentToCheck.BaseType, componentBaseType);
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
    public partial class {componentClassName} : IDisposable
    {{
        private AsyncServiceScope? _scope;

        [Inject] IServiceScopeFactory ScopeFactory {{ get; set; }} = default!;
        [Inject] protected IServiceProvider RootServiceProvider {{ get; set; }} = default!;
        protected bool IsDisposed {{ get; private set; }}

        public IBinder Binder {{ get; private set; }} = null!;

        protected new IServiceProvider ScopedServices
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
            Binder = ScopedServices.GetRequiredService<IBinder>();
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
            }}
        }}

        protected virtual void Dispose(bool disposing)
        {{
        }}
    }}
}}
            ";
    }

    private static string GenerateGenericComponentCode(MvvmComponentClassContext componentClassContext)
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
    public abstract partial class {componentClassName}<T>
        where T : ViewModelBase
    {{
        private IViewModelParameterSetter? _viewModelParameterSetter;

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

            _viewModelParameterSetter ??= ScopedServices.GetRequiredService<IViewModelParameterSetter>();
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
            {{
                await BindingContext.SetParametersAsync(parameters).ConfigureAwait(false);
            }}
        }}
    }}
}}
            ";
    }
}