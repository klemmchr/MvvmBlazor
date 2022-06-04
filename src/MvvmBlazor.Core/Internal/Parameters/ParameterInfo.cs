namespace MvvmBlazor.Internal.Parameters;

internal record ParameterInfo
{
    private readonly Dictionary<PropertyInfo, PropertyInfo> _parameters = new();

    public IReadOnlyDictionary<PropertyInfo, PropertyInfo> Parameters => _parameters;

    public ParameterInfo(IEnumerable<PropertyInfo> componentProperties, IEnumerable<PropertyInfo> viewModelProperties)
    {
        var componentPropertyDict = componentProperties.ToDictionary(x => x.Name);
        foreach (var viewModelProperty in viewModelProperties.OrderBy(x => x.Name))
        {
            if (!componentPropertyDict.TryGetValue(viewModelProperty.Name, out var componentProperty))
            {
                throw new ParameterException(
                    $"Failed to find matching component parameter {viewModelProperty.Name} for view model {viewModelProperty.DeclaringType!.FullName}"
                );
            }

            _parameters.Add(componentProperty, viewModelProperty);
        }
    }
}