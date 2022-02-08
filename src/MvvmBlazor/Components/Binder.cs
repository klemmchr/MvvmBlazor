namespace MvvmBlazor.Components;

public interface IBinder : IDisposable
{
    Action<IBinding, EventArgs>? ValueChangedCallback { get; set; }

    TValue Bind<TViewModel, TValue>(TViewModel viewModel, Expression<Func<TViewModel, TValue>> propertyExpression)
        where TViewModel : ViewModelBase;
}

internal class Binder : IBinder
{
    private readonly IBindingFactory _bindingFactory;
    private readonly IWeakEventManager _weakEventManager;
    private HashSet<IBinding> _bindings = new();

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

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
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

        if (!(property.Body is MemberExpression m))
        {
            throw new BindingException("Binding member needs to be a property");
        }

        if (!(m.Member is PropertyInfo p))
        {
            throw new BindingException("Binding member needs to be a property");
        }

        if (typeof(TViewModel).GetProperty(p.Name) is null)
        {
            throw new BindingException($"Cannot find property {p.Name} in type {viewModel.GetType().FullName}");
        }

        return p;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_bindings is not null)
            {
                DisposeBindings();
                _bindings = null!;
            }
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

    ~Binder()
    {
        Dispose(false);
    }
}