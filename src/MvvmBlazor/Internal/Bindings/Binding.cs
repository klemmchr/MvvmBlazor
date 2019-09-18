using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using MvvmBlazor.Internal.WeakEventListener;

namespace MvvmBlazor.Internal.Bindings
{
    public interface IBinding : IDisposable
    {
        INotifyPropertyChanged Source { get; }
        PropertyInfo PropertyInfo { get; }
        event EventHandler? BindingValueChanged;
        void Initialize();
        object GetValue();
    }

    internal class Binding : IBinding
    {
        private readonly IWeakEventManager _weakEventManager;
        private INotifyCollectionChanged? _boundCollection;

        public Binding(INotifyPropertyChanged source, PropertyInfo propertyInfo, IWeakEventManager weakEventManager)
        {
            _weakEventManager = weakEventManager ?? throw new ArgumentNullException(nameof(weakEventManager));
            Source = source ?? throw new ArgumentNullException(nameof(source));
            PropertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
        }

        public INotifyPropertyChanged Source { get; }
        public PropertyInfo PropertyInfo { get; }

        

        public event EventHandler? BindingValueChanged;

        public void Initialize()
        {
            _weakEventManager.AddWeakEventListener(Source, SourceOnPropertyChanged);
            AddCollectionBindings();
        }

        public object GetValue()
        {
            return PropertyInfo.GetValue(Source);
        }

        private void AddCollectionBindings()
        {
            if (typeof(INotifyCollectionChanged).IsAssignableFrom(PropertyInfo.ReflectedType))
            {
                var collection = (INotifyCollectionChanged) GetValue();
                _weakEventManager.AddWeakEventListener(collection, CollectionOnCollectionChanged);
                _boundCollection = collection;
            }
        }

        private void SourceOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Check if property name of event matches the property name of the binding
            if (e.PropertyName == PropertyInfo.Name)
            {
                // If our binding is a collection binding we need to remove the event
                // and reinitialize the collection bindings
                if (_boundCollection != null)
                {
                    _weakEventManager.RemoveWeakEventListener(_boundCollection);
                    AddCollectionBindings();
                }

                BindingValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void CollectionOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            BindingValueChanged?.Invoke(this, EventArgs.Empty);
        }

        #region IDisposable Support
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_boundCollection != null)
                    _weakEventManager.RemoveWeakEventListener(_boundCollection);

                _weakEventManager.RemoveWeakEventListener(Source);
            }
        }
        #endregion

        #region Base overrides

        public override string ToString()
        {
            return $"{PropertyInfo?.DeclaringType?.Name}.{PropertyInfo?.Name}";
        }

        public override bool Equals(object obj)
        {
            return obj is Binding b && ReferenceEquals(b.Source, Source) && b.PropertyInfo.Name == PropertyInfo.Name;
        }

        public override int GetHashCode()
        {
            var hash = 13;
            hash = hash * 7 + Source.GetHashCode();
            hash = hash * 7 + PropertyInfo.Name.GetHashCode();

            return hash;
        }
        #endregion
    }
}