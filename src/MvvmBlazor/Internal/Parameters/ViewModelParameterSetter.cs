namespace MvvmBlazor.Internal.Parameters;

internal interface IViewModelParameterSetter
{
    void ResolveAndSet(ComponentBase component, ViewModelBase viewModel);
}

internal class ViewModelParameterSetter : IViewModelParameterSetter
{
    private readonly IParameterResolver _parameterResolver;

    public ViewModelParameterSetter(IParameterResolver parameterResolver)
    {
        _parameterResolver = parameterResolver ?? throw new ArgumentNullException(nameof(parameterResolver));
    }

    public void ResolveAndSet(ComponentBase component, ViewModelBase viewModel)
    {
        if (component == null)
        {
            throw new ArgumentNullException(nameof(component));
        }

        if (viewModel == null)
        {
            throw new ArgumentNullException(nameof(viewModel));
        }

        var componentType = component.GetType();
        var viewModelType = viewModel.GetType();

        var parameterInfo = _parameterResolver.ResolveParameters(componentType, viewModelType);
        foreach (var (componentProperty, viewModelProperty) in parameterInfo.Parameters)
        {
            var value = componentProperty.GetValue(component);
            if (value != null && componentProperty.PropertyType != viewModelProperty.PropertyType)
            {
                var converter = TypeDescriptor.GetConverter(viewModelProperty.PropertyType);
                if (converter.CanConvertFrom(componentProperty.PropertyType))
                {
                    value = converter.ConvertTo(value, viewModelProperty.PropertyType);
                }
            }
            viewModelProperty.SetValue(viewModel, value);
        }
    }
}