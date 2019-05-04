using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MvvmBlazor.Bindings;
using MvvmBlazor.ViewModel;

namespace MvvmBlazor.Components
{
    public abstract class ComponentBaseMvvm : ComponentBase, IDisposable
    {
        private readonly HashSet<Binding> _bindings = new HashSet<Binding>();

        protected ViewModelBase BindingContext { get; set; }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        protected T Bind<T>(Expression<Func<T>> property)
        {
            return AddBinding(BindingContext, property);
        }

        protected T Bind<T>(ViewModelBase viewModel, Expression<Func<T>> property)
        {
            return AddBinding(viewModel, property);
        }

        public T AddBinding<T>(ViewModelBase viewModel, Expression<Func<T>> property)
        {
            var propertyInfo = ValidateAndResolveBindingContext(viewModel, property);

            var binding = new Binding(viewModel, propertyInfo);
            if (_bindings.Contains(binding)) return (T) binding.GetValue();

            binding.BindingValueChanged += BindingOnBindingValueChanged;
            binding.Initialize();

            _bindings.Add(binding);

            return (T) binding.GetValue();
        }

        private void BindingOnBindingValueChanged(object sender, EventArgs e)
        {
            StateHasChanged();
        }

        private PropertyInfo ValidateAndResolveBindingContext<T>(ViewModelBase viewModel, Expression<Func<T>> property)
        {
            if (viewModel is null)
                throw new BindingException("ViewModel is null");

            if (!(property.Body is MemberExpression m))
                throw new BindingException("Binding member needs to be a property");

            if (!(m.Member is PropertyInfo p))
                throw new BindingException("Binding member needs to be a property");

            if (p.DeclaringType != viewModel.GetType())
                throw new BindingException($"Cannot find property {p.Name} in type {viewModel.GetType().FullName}");

            return p;
        }

        ~ComponentBaseMvvm()
        {
            Dispose(false);
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

                BindingContext?.Dispose();
            }
        }

        protected virtual ViewModelBase SetBindingContext()
        {
            return null;
        }

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        /// </summary>
        protected sealed override void OnInit()
        {
            BindingContext = SetBindingContext();
            BindingContext?.OnInit();
        }

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        ///     Override this method if you will perform an asynchronous operation and
        ///     want the component to refresh when that operation is completed.
        /// </summary>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> representing any asynchronous operation.</returns>
        protected sealed override Task OnInitAsync()
        {
            return BindingContext?.OnInitAsync() ?? Task.CompletedTask;
        }

        /// <summary>
        ///     Method invoked when the component has received parameters from its parent in
        ///     the render tree, and the incoming values have been assigned to properties.
        /// </summary>
        protected sealed override void OnParametersSet()
        {
            BindingContext?.OnParametersSet();
        }

        /// <summary>
        ///     Method invoked when the component has received parameters from its parent in
        ///     the render tree, and the incoming values have been assigned to properties.
        /// </summary>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> representing any asynchronous operation.</returns>
        protected sealed override Task OnParametersSetAsync()
        {
            return BindingContext?.OnParametersSetAsync() ?? Task.CompletedTask;
        }

        /// <summary>
        ///     Returns a flag to indicate whether the component should render.
        /// </summary>
        /// <returns></returns>
        protected sealed override bool ShouldRender()
        {
            return BindingContext?.ShouldRender() ?? true;
        }

        /// <summary>
        ///     Method invoked after each time the component has been rendered.
        /// </summary>
        protected sealed override void OnAfterRender()
        {
            BindingContext?.OnAfterRenderAsync();
        }

        protected sealed override Task OnAfterRenderAsync()
        {
            return BindingContext?.OnAfterRenderAsync() ?? Task.CompletedTask;
        }
    }
}