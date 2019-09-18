using System.ComponentModel;
using System.Reflection;
using MvvmBlazor.Internal.WeakEventListener;

namespace MvvmBlazor.Internal.Bindings
{
    public interface IBindingFactory
    {
        IBinding Create(INotifyPropertyChanged source, PropertyInfo propertyInfo, IWeakEventManager weakEventManager);
    }

    internal class BindingFactory : IBindingFactory
    {
        public IBinding Create(INotifyPropertyChanged source, PropertyInfo propertyInfo,
            IWeakEventManager weakEventManager)
        {
            return new Binding(source, propertyInfo, weakEventManager);
        }
    }
}