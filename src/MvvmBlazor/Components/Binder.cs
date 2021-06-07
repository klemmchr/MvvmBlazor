using MvvmBlazor.Internal.Bindings;
using MvvmBlazor.Internal.WeakEventListener;
using MvvmBlazor.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MvvmBlazor.Components
{
    public class Binder<TViewModel>
        where TViewModel: ViewModelBase
    {
        private readonly IBindingFactory _bindingFactory;
        private readonly IWeakEventManager _weakEventManager;
        private HashSet<IBinding> _bindings = new();

        public Binder(IBindingFactory bindingFactory, IWeakEventManager weakEventManager)
        {
            _bindingFactory = bindingFactory;
            _weakEventManager = weakEventManager;
        }

        public TValue Bind<TValue>(TViewModel viewModel,
            Expression<Func<TViewModel, TValue>> propertyExpression)
        { 
            var propertyInfo = ValidateAndResolveBindingContext(viewModel, propertyExpression);

            var binding = _bindingFactory.Create(viewModel, propertyInfo, _weakEventManager);
            if (_bindings.Contains(binding)) return (TValue) binding.GetValue();

            //_weakEventManager.AddWeakEventListener<IBinding, EventArgs>(binding, nameof(Binding.BindingValueChanged),
                //BindingOnBindingValueChanged);
            binding.Initialize();

            _bindings.Add(binding);

            return (TValue) binding.GetValue();
        }

        protected static PropertyInfo ValidateAndResolveBindingContext<TValue>(TViewModel viewModel,
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
    }
}
