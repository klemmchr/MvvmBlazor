namespace MvvmBlazor.Internal.Bindings;

public interface IBinder
{
    Action<IBinding, EventArgs>? ValueChangedCallback { get; set; }

    TValue Bind<TViewModel, TValue>(TViewModel viewModel, Expression<Func<TViewModel, TValue>> propertyExpression)
        where TViewModel : ViewModelBase;
}

internal class Binder : IBinder, IDisposable
{
    private readonly IBindingFactory _bindingFactory;
    private readonly HashSet<IBinding> _bindings = new();
    private readonly IWeakEventManager _weakEventManager;
    private bool _isDisposed;

    public Binder(IBindingFactory bindingFactory, IWeakEventManager weakEventManager)
    {
        _bindingFactory = bindingFactory;
        _weakEventManager = weakEventManager;
    }

    public Action<IBinding, EventArgs>? ValueChangedCallback { get; set; }

    public TValue Bind<TViewModel, TValue>(
        TViewModel viewModel,
        Expression<Func<TViewModel, TValue>> propertyExpression) where TViewModel : ViewModelBase
    {
        ThrowIfDisposed();

        if (ValueChangedCallback is null)
        {
            throw new BindingException($"{nameof(ValueChangedCallback)} is null");
        }

        var propertyInfo = ValidateAndResolveBindingContext(viewModel, propertyExpression);

        var binding = _bindingFactory.Create(viewModel, propertyInfo, _weakEventManager);
        if (_bindings.Contains(binding))
        {
            return (TValue)binding.GetValue();
        }

        _weakEventManager.AddWeakEventListener(binding, nameof(Binding.BindingValueChanged), ValueChangedCallback);
        binding.Initialize();

        _bindings.Add(binding);

        return (TValue)binding.GetValue();
    }

    protected static PropertyInfo ValidateAndResolveBindingContext<TViewModel, TValue>(
        TViewModel viewModel,
        Expression<Func<TViewModel, TValue>> property) where TViewModel : ViewModelBase
    {
        if (viewModel is null)
        {
            throw new BindingException("ViewModelType is null");
        }

        if (property is null)
        {
            throw new BindingException("Property expression is null");
        }

        if (property.Body is not MemberExpression { Member: PropertyInfo p })
        {
            throw new BindingException("Binding member needs to be a property");
        }

        if (typeof(TViewModel).GetProperty(p.Name) is null)
        {
            throw new BindingException($"Cannot find property {p.Name} in type {viewModel.GetType().FullName}");
        }

        return p;
    }

    #region IDisposable

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            ThrowIfDisposed();

            _isDisposed = true;
            DisposeBindings();
        }
    }

    private void DisposeBindings()
    {
        foreach (var binding in _bindings)
        {
            _weakEventManager.RemoveWeakEventListener(binding);
            binding.Dispose();
        }
    }

    private void ThrowIfDisposed()
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException(nameof(Binder));
        }
    }

    ~Binder()
    {
        Dispose(false);
    }

    #endregion
}