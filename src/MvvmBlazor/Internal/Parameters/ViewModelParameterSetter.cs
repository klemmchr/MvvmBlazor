using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using MvvmBlazor.ViewModel;

namespace MvvmBlazor.Internal.Parameters
{
    internal interface IViewModelParameterSetter
    {
        void ResolveAndSet(ComponentBase component, ViewModelBase viewModel);
    }

    internal class ViewModelParameterSetter : IViewModelParameterSetter
    {
        private readonly IParameterResolver _parameterResolver;
        private readonly IParameterCache _parameterCache;

        public ViewModelParameterSetter(IParameterResolver parameterResolver, IParameterCache parameterCache)
        {
            _parameterResolver = parameterResolver ?? throw new ArgumentNullException(nameof(parameterResolver));
            _parameterCache = parameterCache ?? throw new ArgumentNullException(nameof(parameterCache));
        }

        public void ResolveAndSet(ComponentBase component, ViewModelBase viewModel)
        {
            if (component == null) throw new ArgumentNullException(nameof(component));
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            var componentType = component.GetType();

            var parameterInfo = _parameterCache.Get(componentType);
            if (parameterInfo == null)
            {
                var componentParameters = _parameterResolver.ResolveParameters(componentType);
                var viewModelParameters = _parameterResolver.ResolveParameters(viewModel.GetType());
                parameterInfo = new ParameterInfo(componentParameters, viewModelParameters);
                _parameterCache.Set(componentType, parameterInfo);
            }

            foreach ((PropertyInfo componentProperty, PropertyInfo viewModelProperty) in parameterInfo.Parameters)
            {
                var value = componentProperty.GetValue(component);
                viewModelProperty.SetValue(viewModel, value);
            }
        }
    }
}