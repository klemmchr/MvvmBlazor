using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MvvmBlazor.Bindings;
using MvvmBlazor.Extensions;
using MvvmBlazor.ViewModel;

namespace MvvmBlazor.Components
{
    public abstract class ComponentBaseMvvm : ComponentBase, IDisposable
    {
        private readonly HashSet<Binding> _bindings = new HashSet<Binding>();

        protected TValue Bind<TViewModel, TValue>(TViewModel viewModel, Expression<Func<TViewModel, TValue>> property) where TViewModel : ViewModelBase
        {
            return AddBinding(viewModel, property);
        }

        public TValue AddBinding<TViewModel, TValue>(TViewModel? viewModel, Expression<Func<TViewModel, TValue>> property) where TViewModel : ViewModelBase
        {
            var propertyInfo = ValidateAndResolveBindingContext(viewModel, property);

#pragma warning disable CS8604 // Possible null reference argument.
            var binding = new Binding(viewModel, propertyInfo);
#pragma warning restore CS8604 // Possible null reference argument.
            if (_bindings.Contains(binding)) return (TValue)binding.GetValue();

            binding.BindingValueChanged += BindingOnBindingValueChanged;
            binding.Initialize();

            _bindings.Add(binding);

            return (TValue)binding.GetValue();
        }

        private void BindingOnBindingValueChanged(object sender, EventArgs e)
        {
            InvokeAsync(StateHasChanged);
        }

        protected PropertyInfo ValidateAndResolveBindingContext<TViewModel, TValue>(ViewModelBase? viewModel, Expression<Func<TViewModel, TValue>> property)
        {
            if (viewModel is null)
                throw new BindingException("ViewModelType is null");

            if (!(property.Body is MemberExpression m))
                throw new BindingException("Binding member needs to be a property");

            if (!(m.Member is PropertyInfo p))
                throw new BindingException("Binding member needs to be a property");

            if (p.DeclaringType != viewModel.GetType())
                throw new BindingException($"Cannot find property {p.Name} in type {viewModel.GetType().FullName}");

            return p;
        }

        #region IDisposable Support
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var binding in _bindings)
                {
                    binding.BindingValueChanged -= BindingOnBindingValueChanged;
                    binding.Dispose();
                }
            }
        }

        ~ComponentBaseMvvm()
        {
            Dispose(false);
        }
        #endregion
    }

    public abstract class ComponentBaseMvvm<T> : ComponentBaseMvvm where T: ViewModelBase
    {
        private readonly IDependencyResolver _dependencyResolver;

        protected internal T BindingContext { get; set; }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        protected internal ComponentBaseMvvm(IDependencyResolver dependencyResolver)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {
            _dependencyResolver = dependencyResolver ?? throw new ArgumentNullException(nameof(dependencyResolver));
            SetBindingContext();
        }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        protected ComponentBaseMvvm()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {
            _dependencyResolver = DependencyResolver.Default ??
                throw new InvalidOperationException(
                    $"Mvvm blazor is uninitialized. Make sure to call '{nameof(ServiceCollectionExtensions.AddMvvm)}()' and '{nameof(ApplicationBuilderExtensions.UseMvvm)}()' in your Startup class.");
            SetBindingContext();
        }

        private void SetBindingContext()
        {
            BindingContext = _dependencyResolver.GetService<T>();
        }

        protected TValue Bind<TValue>(Expression<Func<T, TValue>> property)
        {
            return AddBinding(BindingContext, property);
        }
        
        #region Lifecycle Methods

        /// <inheritdoc />
        protected sealed override void OnInitialized()
        {
            BindingContext?.OnInitialized();
        }

        /// <inheritdoc />
        protected sealed override Task OnInitializedAsync()
        {
            return BindingContext?.OnInitializedAsync() ?? Task.CompletedTask;
        }

        /// <inheritdoc />
        protected sealed override void OnParametersSet()
        {
            BindingContext?.OnParametersSet();
        }

        /// <inheritdoc />
        protected sealed override Task OnParametersSetAsync()
        {
            return BindingContext?.OnParametersSetAsync() ?? Task.CompletedTask;
        }

        /// <inheritdoc />
        protected sealed override bool ShouldRender()
        {
            return BindingContext?.ShouldRender() ?? true;
        }

        /// <inheritdoc />
        protected sealed override void OnAfterRender(bool firstRender)
        {
            BindingContext?.OnAfterRender(firstRender);
        }
        
        /// <inheritdoc />
        protected sealed override Task OnAfterRenderAsync(bool firstRender)
        {
            return BindingContext?.OnAfterRenderAsync(firstRender) ?? Task.CompletedTask;
        }

        #endregion

        #region IDisposable Support
        
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                BindingContext?.Dispose();
            }
        }

        ~ComponentBaseMvvm()
        {
            Dispose(false);
        }
        #endregion
    }
}