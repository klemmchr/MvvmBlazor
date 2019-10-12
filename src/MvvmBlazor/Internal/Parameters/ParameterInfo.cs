using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MvvmBlazor.Internal.Parameters
{
    internal class ParameterInfo
    {
        private readonly Dictionary<PropertyInfo, PropertyInfo> _parameters =
            new Dictionary<PropertyInfo, PropertyInfo>();

        public ParameterInfo(IEnumerable<PropertyInfo> componentProperties,
            IEnumerable<PropertyInfo> viewModelProperties)
        {
            if (componentProperties == null) throw new ArgumentNullException(nameof(componentProperties));
            if (viewModelProperties == null) throw new ArgumentNullException(nameof(viewModelProperties));

            var viewModelPropDict = viewModelProperties.ToDictionary(x => x.Name);

            foreach (var componentProperty in componentProperties)
            {
                if (!viewModelPropDict.TryGetValue(componentProperty.Name, out var viewModelProperty))
                    continue;

                _parameters.Add(componentProperty, viewModelProperty);
            }
        }

        public IReadOnlyDictionary<PropertyInfo, PropertyInfo> Parameters => _parameters;
    }
}