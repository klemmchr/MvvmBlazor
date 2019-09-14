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
    public abstract class MvvmComponentBase : ComponentBase, IDisposable
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

        ~MvvmComponentBase()
        {
            Dispose(false);
        }
        #endregion
    }
}