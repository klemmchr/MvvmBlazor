namespace MvvmBlazor.Internal.Bindings;

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
    private bool _isCollection;

    public Binding(INotifyPropertyChanged source, PropertyInfo propertyInfo, IWeakEventManager weakEventManager)
    {
        _weakEventManager = weakEventManager;
        Source = source;
        PropertyInfo = propertyInfo;
    }

    public INotifyPropertyChanged Source { get; }
    public PropertyInfo PropertyInfo { get; }

    public event EventHandler? BindingValueChanged;

    public void Initialize()
    {
        _isCollection = typeof(INotifyCollectionChanged).IsAssignableFrom(PropertyInfo.PropertyType);
        _weakEventManager.AddWeakEventListener(Source, SourceOnPropertyChanged);
        AddCollectionBindings();
    }

    public object GetValue()
    {
        return PropertyInfo.GetValue(Source, null)!;
    }

    private void AddCollectionBindings()
    {
        if (!_isCollection || GetValue() is not INotifyCollectionChanged collection)
        {
            return;
        }

        _weakEventManager.AddWeakEventListener(collection, CollectionOnCollectionChanged);
        _boundCollection = collection;
    }

    private void SourceOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is null)
        {
            BindingValueChanged?.Invoke(this, EventArgs.Empty);
            return;
        }

        // This should just listen to the bindings property
        if (e.PropertyName != PropertyInfo.Name)
        {
            return;
        }

        if (_isCollection)
        {
            // If our binding is a collection binding we need to remove the event
            // and reinitialize the collection bindings
            if (_boundCollection != null)
            {
                _weakEventManager.RemoveWeakEventListener(_boundCollection);
            }

            AddCollectionBindings();
        }


        BindingValueChanged?.Invoke(this, EventArgs.Empty);
    }

    private void CollectionOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        BindingValueChanged?.Invoke(this, EventArgs.Empty);
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
        {
            if (_boundCollection != null)
            {
                _weakEventManager.RemoveWeakEventListener(_boundCollection);
            }

            _weakEventManager.RemoveWeakEventListener(Source);
        }
    }

    #endregion

    #region Base overrides

    public override string ToString()
    {
        return $"{PropertyInfo?.DeclaringType?.Name}.{PropertyInfo?.Name}";
    }

    public override bool Equals(object? obj)
    {
        return obj is Binding b && ReferenceEquals(b.Source, Source) && b.PropertyInfo.Name == PropertyInfo.Name;
    }

    public override int GetHashCode()
    {
        var hash = 13;
        hash = hash * 7 + Source.GetHashCode();
        hash = hash * 7 + PropertyInfo.Name.GetHashCode(StringComparison.InvariantCulture);

        return hash;
    }

    #endregion
}