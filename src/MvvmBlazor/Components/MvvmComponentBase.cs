using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using MvvmBlazor.Internal.Bindings;
using MvvmBlazor.Extensions;
using MvvmBlazor.Internal.WeakEventListener;
using MvvmBlazor.ViewModel;

namespace MvvmBlazor.Components
{
    public abstract class MvvmComponentBase : ComponentBase, IDisposable
    {
        private readonly HashSet<IBinding> _bindings = new HashSet<IBinding>();
        private IWeakEventManagerFactory _weakEventManagerFactory;
        private IBindingFactory _bindingFactory;
        private IWeakEventManager _weakEventManager;

        protected readonly IDependencyResolver DependencyResolver;

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        protected internal MvvmComponentBase(IDependencyResolver dependencyResolver)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {
            DependencyResolver = dependencyResolver ?? throw new ArgumentNullException(nameof(dependencyResolver));
            InitializeDependencies();
        }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        protected MvvmComponentBase()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {
            DependencyResolver = Components.DependencyResolver.Default ??
                                  throw new InvalidOperationException(
                                      $"Mvvm blazor is uninitialized. Make sure to call '{nameof(ServiceCollectionExtensions.AddMvvm)}()' and '{nameof(ApplicationBuilderExtensions.UseMvvm)}()' in your Startup class.");
            InitializeDependencies();
        }

        private void InitializeDependencies()
        {
            _weakEventManagerFactory = DependencyResolver.GetService<IWeakEventManagerFactory>();
            _bindingFactory = DependencyResolver.GetService<IBindingFactory>();
            _weakEventManager = _weakEventManagerFactory.Create();
        }

        protected internal TValue Bind<TViewModel, TValue>(TViewModel viewModel, Expression<Func<TViewModel, TValue>> property)
            where TViewModel : ViewModelBase
        {
            return AddBinding(viewModel, property);
        }

        public virtual TValue AddBinding<TViewModel, TValue>(TViewModel viewModel,
            Expression<Func<TViewModel, TValue>> property) where TViewModel : ViewModelBase
        {
            var propertyInfo = ValidateAndResolveBindingContext(viewModel, property);

            var binding = _bindingFactory.Create(viewModel, propertyInfo, _weakEventManagerFactory.Create());
            if (_bindings.Contains(binding)) return (TValue) binding.GetValue();

            _weakEventManager.AddWeakEventListener<IBinding, EventArgs>(binding, nameof(Binding.BindingValueChanged), BindingOnBindingValueChanged);
            binding.Initialize();

            _bindings.Add(binding);

            return (TValue) binding.GetValue();
        }

        internal virtual void BindingOnBindingValueChanged(object sender, EventArgs e)
        {
            InvokeAsync(StateHasChanged);
        }

        protected PropertyInfo ValidateAndResolveBindingContext<TViewModel, TValue>(ViewModelBase? viewModel,
            Expression<Func<TViewModel, TValue>> property)
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
                foreach (var binding in _bindings)
                {
                    _weakEventManager.RemoveWeakEventListener(binding);
                    binding.Dispose();
                }
        }

        ~MvvmComponentBase()
        {
            Dispose(false);
        }

        #endregion
    }
}