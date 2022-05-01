namespace MvvmBlazor.ViewModel;

public abstract class ViewModelBase : INotifyPropertyChanged
{
    private readonly Dictionary<string, List<Func<object, Task>>> _subscriptions = new();

    public IServiceProvider RootServiceProvider { get; set; } = null!;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected bool Set<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            OnPropertyChanged(propertyName!);
            if (!_subscriptions.ContainsKey(propertyName!))
            {
                return true;
            }

            foreach (var action in _subscriptions[propertyName!]) action(value!);
            return true;
        }

        return false;
    }

    public virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        ArgumentNullException.ThrowIfNull(propertyName);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected void Subscribe<T>(Expression<Func<T>>? expression, Action<T> action)
    {
        SubscribeAsync(
            expression,
            arg =>
            {
                action(arg);
                return Task.CompletedTask;
            }
        );
    }

    protected void SubscribeAsync<T>(Expression<Func<T>>? property, Func<T, Task> func)
    {
        if (property is null)
        {
            throw new BindingException("Property cannot be null");
        }

        if (!(property.Body is MemberExpression m))
        {
            throw new BindingException("Subscription member must be a property");
        }

        if (!(m.Member is PropertyInfo propertyInfo))
        {
            throw new BindingException("Subscription member must be a property");
        }

        var propertyName = propertyInfo.Name;
        if (!_subscriptions.ContainsKey(propertyName))
        {
            _subscriptions[propertyName] = new List<Func<object, Task>>();
        }

        _subscriptions[propertyName].Add(async value => await func((T)value).ConfigureAwait(false));
    }

    #region Lifecycle Methods

    /// <summary>
    ///     Method invoked when the component is ready to start, having received its
    ///     initial parameters from its parent in the render tree.
    /// </summary>
    public virtual void OnInitialized() { }

    /// <summary>
    ///     Method invoked when the component is ready to start, having received its
    ///     initial parameters from its parent in the render tree.
    ///     Override this method if you will perform an asynchronous operation and
    ///     want the component to refresh when that operation is completed.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    public virtual Task OnInitializedAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Method invoked when the component has received parameters from its parent in
    ///     the render tree, and the incoming values have been assigned to properties.
    /// </summary>
    public virtual void OnParametersSet() { }

    /// <summary>
    ///     Method invoked when the component has received parameters from its parent in
    ///     the render tree, and the incoming values have been assigned to properties.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    public virtual Task OnParametersSetAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Notifies the component that its state has changed. When applicable, this will
    ///     cause the component to be re-rendered.
    /// </summary>
    protected void StateHasChanged()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
    }

    /// <summary>
    ///     Returns a flag to indicate whether the component should render.
    /// </summary>
    /// <returns></returns>
    public virtual bool ShouldRender()
    {
        return true;
    }

    /// <summary>
    ///     Method invoked after each time the component has been rendered.
    /// </summary>
    /// <param name="firstRender">
    ///     Set to <c>true</c> if this is the first time
    ///     <see cref="ComponentBase.OnAfterRender(bool)" /> has been invoked
    ///     on this component instance; otherwise <c>false</c>.
    /// </param>
    /// <remarks>
    ///     The <see cref="ComponentBase.OnAfterRender(bool)" /> and
    ///     <see cref="ComponentBase.OnAfterRenderAsync(bool)" /> lifecycle methods
    ///     are useful for performing interop, or interacting with values received from <c>@ref</c>.
    ///     Use the <paramref name="firstRender" /> parameter to ensure that initialization work is only performed
    ///     once.
    /// </remarks>
    public virtual void OnAfterRender(bool firstRender) { }

    /// <summary>
    ///     Method invoked after each time the component has been rendered. Note that the component does
    ///     not automatically re-render after the completion of any returned <see cref="Task" />,
    ///     because
    ///     that would cause an infinite render loop.
    /// </summary>
    /// <param name="firstRender">
    ///     Set to <c>true</c> if this is the first time
    ///     <see cref="ComponentBase.OnAfterRender(bool)" /> has been invoked
    ///     on this component instance; otherwise <c>false</c>.
    /// </param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    /// <remarks>
    ///     The <see cref="ComponentBase.OnAfterRender(bool)" /> and
    ///     <see cref="ComponentBase.OnAfterRenderAsync(bool)" /> lifecycle methods
    ///     are useful for performing interop, or interacting with values received from <c>@ref</c>.
    ///     Use the <paramref name="firstRender" /> parameter to ensure that initialization work is only performed
    ///     once.
    /// </remarks>
    public virtual Task OnAfterRenderAsync(bool firstRender)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Sets parameters supplied by the component's parent in the render tree.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <returns>
    ///     A <see cref="Task" /> that completes when the component has finished updating and
    ///     rendering itself.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         The
    ///         <see
    ///             cref="ComponentBase.SetParametersAsync(ParameterView)" />
    ///         method should be passed the entire set of parameter values each
    ///         time
    ///         <see
    ///             cref="ComponentBase.SetParametersAsync(ParameterView)" />
    ///         is called. It not required that the caller supply a parameter
    ///         value for all parameters that are logically understood by the component.
    ///     </para>
    ///     <para>
    ///         The default implementation of
    ///         <see
    ///             cref="ComponentBase.SetParametersAsync(ParameterView)" />
    ///         will set the value of each property
    ///         decorated with <see cref="ParameterAttribute" /> or
    ///         <see cref="CascadingParameterAttribute" /> that has
    ///         a corresponding value in the <see cref="ParameterView" />. Parameters that do
    ///         not have a corresponding value
    ///         will be unchanged.
    ///     </para>
    /// </remarks>
    public virtual Task SetParametersAsync(ParameterView parameters)
    {
        return Task.CompletedTask;
    }

    #endregion
}