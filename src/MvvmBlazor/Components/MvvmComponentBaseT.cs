using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MvvmBlazor.Bindings;
using MvvmBlazor.Extensions;
using MvvmBlazor.ViewModel;

namespace MvvmBlazor.Components
{
    public abstract class MvvmComponentBase<T> : MvvmComponentBase where T: ViewModelBase
    {
        private readonly IDependencyResolver _dependencyResolver;

        protected internal T BindingContext { get; set; }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        protected internal MvvmComponentBase(IDependencyResolver dependencyResolver)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {
            _dependencyResolver = dependencyResolver ?? throw new ArgumentNullException(nameof(dependencyResolver));
            SetBindingContext();
        }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        protected MvvmComponentBase()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {
            _dependencyResolver = DependencyResolver.Default ??
                throw new InvalidOperationException(
                    $"Mvvm blazor is uninitialized. Make sure to call '{nameof(ServiceCollectionExtensions.AddMvvm)}()' and '{nameof(ApplicationBuilderExtensions.UseMvvm)}()' in your Startup class.");
            SetBindingContext();
        }

        private void SetBindingContext()
        {
            BindingContext = _dependencyResolver.GetService<T>();
        }

        protected TValue Bind<TValue>(Expression<Func<T, TValue>> property)
        {
            return AddBinding(BindingContext, property);
        }
        
        #region Lifecycle Methods

        /// <inheritdoc />
        protected sealed override void OnInitialized()
        {
            BindingContext?.OnInitialized();
        }

        /// <inheritdoc />
        protected sealed override Task OnInitializedAsync()
        {
            return BindingContext?.OnInitializedAsync() ?? Task.CompletedTask;
        }

        /// <inheritdoc />
        protected sealed override void OnParametersSet()
        {
            BindingContext?.OnParametersSet();
        }

        /// <inheritdoc />
        protected sealed override Task OnParametersSetAsync()
        {
            return BindingContext?.OnParametersSetAsync() ?? Task.CompletedTask;
        }

        /// <inheritdoc />
        protected sealed override bool ShouldRender()
        {
            return BindingContext?.ShouldRender() ?? true;
        }

        /// <inheritdoc />
        protected sealed override void OnAfterRender(bool firstRender)
        {
            BindingContext?.OnAfterRender(firstRender);
        }
        
        /// <inheritdoc />
        protected sealed override Task OnAfterRenderAsync(bool firstRender)
        {
            return BindingContext?.OnAfterRenderAsync(firstRender) ?? Task.CompletedTask;
        }

        /// <inheritdoc />
        public sealed override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);

            if(BindingContext != null)
                await BindingContext.SetParametersAsync(parameters);
        }

        #endregion

        #region IDisposable Support
        
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                BindingContext?.Dispose();
            }
        }

        ~MvvmComponentBase()
        {
            Dispose(false);
        }
        #endregion
    }
}