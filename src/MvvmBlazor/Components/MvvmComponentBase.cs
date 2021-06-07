using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MvvmBlazor.Internal.Bindings;
using MvvmBlazor.Internal.WeakEventListener;
using MvvmBlazor.ViewModel;

namespace MvvmBlazor.Components
{
    public abstract class MvvmComponentBase : ComponentBase, IDisposable, IAsyncDisposable
    {
        private IBindingFactory _bindingFactory = null!;
        private HashSet<IBinding> _bindings = new();
        private IWeakEventManager _weakEventManager = null!;
        private IWeakEventManagerFactory _weakEventManagerFactory = null!;

        protected internal MvvmComponentBase(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            InitializeDependencies();
        }

        // ReSharper disable once UnusedMember.Global
        protected MvvmComponentBase() {}

        [Inject] protected IServiceProvider ServiceProvider { get; set; } = null!;

        private void InitializeDependencies()
        {
            _weakEventManagerFactory = ServiceProvider.GetRequiredService<IWeakEventManagerFactory>();
            _bindingFactory = ServiceProvider.GetRequiredService<IBindingFactory>();
            _weakEventManager = _weakEventManagerFactory.Create();
        }

        protected internal TValue Bind<TViewModel, TValue>(TViewModel viewModel,
            Expression<Func<TViewModel, TValue>> property)
            where TViewModel : ViewModelBase
        {
            return AddBinding(viewModel, property);
        }

        public virtual TValue AddBinding<TViewModel, TValue>(TViewModel viewModel,
            Expression<Func<TViewModel, TValue>> propertyExpression) where TViewModel : ViewModelBase
        { 
            var propertyInfo = ValidateAndResolveBindingContext(viewModel, propertyExpression);

            var binding = _bindingFactory.Create(viewModel, propertyInfo, _weakEventManagerFactory.Create());
            if (_bindings.Contains(binding)) return (TValue) binding.GetValue();

            _weakEventManager.AddWeakEventListener<IBinding, EventArgs>(binding, nameof(Binding.BindingValueChanged),
                BindingOnBindingValueChanged);
            binding.Initialize();

            _bindings.Add(binding);

            return (TValue) binding.GetValue();
        }

        protected override void OnInitialized()
        {
            InitializeDependencies();
        }

        internal virtual void BindingOnBindingValueChanged(object sender, EventArgs e)
        {
            InvokeAsync(StateHasChanged);
        }

        protected static PropertyInfo ValidateAndResolveBindingContext<TViewModel, TValue>(TViewModel viewModel,
            Expression<Func<TViewModel, TValue>> property)
        {
            if (viewModel is null)
                throw new BindingException("ViewModelType is null");

            if (property is null)
                throw new BindingException("Property expression is null");

            if (!(property.Body is MemberExpression m))
                throw new BindingException("Binding member needs to be a property");

            if (!(m.Member is PropertyInfo p))
                throw new BindingException("Binding member needs to be a property");

            if (typeof(TViewModel).GetProperty(p.Name) is null)
                throw new BindingException($"Cannot find property {p.Name} in type {viewModel.GetType().FullName}");

            return p;
        }

        #region IDisposable Support

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                if (_bindings is not null!)
                {
                    DisposeBindings();

                    _bindings = null!;
                }
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();

            Dispose(false);
            GC.SuppressFinalize(this);
        }

        protected virtual ValueTask DisposeAsyncCore()
        {
            if (_bindings is not null!)
            {
                DisposeBindings();
                _bindings = null!;
            }

            return default;
        }

        private void DisposeBindings()
        {
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