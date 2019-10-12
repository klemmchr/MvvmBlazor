using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Components;

namespace MvvmBlazor.Internal.Parameters
{
    internal interface IParameterResolver
    {
        IEnumerable<PropertyInfo> ResolveParameters(Type memberType);
    }

    internal class ParameterResolver : IParameterResolver
    {
        public IEnumerable<PropertyInfo> ResolveParameters(Type memberType)
        {
            var componentProperties = memberType.GetProperties();

            var resolvedComponentProperties = new List<PropertyInfo>();
            foreach (var componentProperty in componentProperties)
            {
                // Skip if property has no public setter
                if (!componentProperty.CanWrite)
                    continue;

                // If the property is marked as a parameter add it to the list
                var parameterAttribute = componentProperty.GetCustomAttribute<ParameterAttribute>();
                if (parameterAttribute != null)
                    resolvedComponentProperties.Add(componentProperty);
            }

            return resolvedComponentProperties;
        }
    }
}