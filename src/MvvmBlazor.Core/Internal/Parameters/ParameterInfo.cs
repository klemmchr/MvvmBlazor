namespace MvvmBlazor.Internal.Parameters;

internal record ParameterInfo
{
    private readonly Dictionary<PropertyInfo, PropertyInfo> _parameters = new();

    public IReadOnlyDictionary<PropertyInfo, PropertyInfo> Parameters => _parameters;

    public ParameterInfo(IEnumerable<PropertyInfo> componentProperties, IEnumerable<PropertyInfo> viewModelProperties)
    {
        var viewModelPropDict = viewModelProperties.ToDictionary(x => x.Name);

        foreach (var componentProperty in componentProperties)
        {
            if (!viewModelPropDict.TryGetValue(componentProperty.Name, out var viewModelProperty))
            {
                continue;
            }

            _parameters.Add(componentProperty, viewModelProperty);
        }
    }
}