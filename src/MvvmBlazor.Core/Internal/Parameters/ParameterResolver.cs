namespace MvvmBlazor.Internal.Parameters;

internal interface IParameterResolver
{
    ParameterInfo ResolveParameters(Type componentType, Type viewModelType);
}

internal class ParameterResolver : IParameterResolver
{
    private readonly IParameterCache _parameterCache;

    public ParameterResolver(IParameterCache parameterCache)
    {
        _parameterCache = parameterCache;
    }

    public ParameterInfo ResolveParameters(Type componentType, Type viewModelType)
    {
        var parameterInfo = _parameterCache.Get(componentType);
        if (parameterInfo is not null)
        {
            return parameterInfo;
        }

        var componentParameters = ResolveTypeParameters(componentType);
        var viewModelParameters = ResolveTypeParameters(viewModelType);

        parameterInfo = new ParameterInfo(componentParameters, viewModelParameters);
        _parameterCache.Set(componentType, parameterInfo);

        return parameterInfo;
    }

    private static IEnumerable<PropertyInfo> ResolveTypeParameters(Type memberType)
    {
        var componentProperties = memberType.GetProperties();

        var resolvedComponentProperties = new List<PropertyInfo>();
        foreach (var componentProperty in componentProperties)
        {
            // Skip if property has no public setter
            if (componentProperty.GetSetMethod() is null)
            {
                continue;
            }

            // If the property is marked as a parameter add it to the list
            ParameterAttribute? GetParameterAttribute() => componentProperty.GetCustomAttribute<ParameterAttribute>();
            CascadingParameterAttribute? GetCascadingParameterAttribute() => componentProperty.GetCustomAttribute<CascadingParameterAttribute>();
            if (GetParameterAttribute() != null || GetCascadingParameterAttribute() != null)
            {
                resolvedComponentProperties.Add(componentProperty);
            }
        }

        return resolvedComponentProperties;
    }
}