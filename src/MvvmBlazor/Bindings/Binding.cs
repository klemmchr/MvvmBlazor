using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;

namespace MvvmBlazor.Bindings
{
    internal class Binding : IDisposable
    {
        private INotifyCollectionChanged _boundCollection;

        public Binding(INotifyPropertyChanged source, PropertyInfo propertyInfo)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            PropertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
        }

        public INotifyPropertyChanged Source { get; }
        public PropertyInfo PropertyInfo { get; }

        public void Dispose()
        {
            Dispose(true);
        }

        public event EventHandler BindingValueChanged;

        public void Initialize()
        {
            Source.PropertyChanged += SourceOnPropertyChanged;
            AddCollectionBindings();
        }

        public object GetValue()
        {
            return PropertyInfo.GetValue(Source);
        }

        private void AddCollectionBindings()
        {
            var value = GetValue();
            if (value is INotifyCollectionChanged collection)
            {
                collection.CollectionChanged += CollectionOnCollectionChanged;
                _boundCollection = collection;
            }
        }

        private void SourceOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == PropertyInfo.Name)
            {
                if (_boundCollection != null)
                {
                    _boundCollection.CollectionChanged -= CollectionOnCollectionChanged;
                    AddCollectionBindings();
                }

                BindingValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void CollectionOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            BindingValueChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_boundCollection != null)
                    _boundCollection.CollectionChanged -= CollectionOnCollectionChanged;

                Source.PropertyChanged -= SourceOnPropertyChanged;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is Binding b && b.Source == Source && b.PropertyInfo.Name == PropertyInfo.Name;
        }

        public override int GetHashCode()
        {
            var hash = 13;
            hash = hash * 7 + Source.GetHashCode();
            hash = hash * 7 + PropertyInfo.Name.GetHashCode();

            return hash;
        }
    }
}