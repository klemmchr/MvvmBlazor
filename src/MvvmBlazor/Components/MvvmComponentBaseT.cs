using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MvvmBlazor.Internal.Parameters;
using MvvmBlazor.ViewModel;

namespace MvvmBlazor.Components
{
    public abstract class MvvmComponentBase<T> : MvvmComponentBase where T : ViewModelBase
    {
        private IViewModelParameterSetter _viewModelParameterSetter;

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public MvvmComponentBase()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {
        }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        internal MvvmComponentBase(IServiceProvider serviceProvider) : base(serviceProvider)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {
            SetBindingContext();
        }

        protected internal T BindingContext { get; set; }

        private void SetBindingContext()
        {
            BindingContext ??= ServiceProvider.GetRequiredService<T>();
        }

        private void SetParameters()
        {
            _viewModelParameterSetter ??= ServiceProvider.GetRequiredService<IViewModelParameterSetter>();
            _viewModelParameterSetter.ResolveAndSet(this, BindingContext);
        }

        protected internal TValue Bind<TValue>(Expression<Func<T, TValue>> property)
        {
            return AddBinding(BindingContext, property);
        }

        #region Lifecycle Methods

        /// <inheritdoc />
        protected sealed override void OnInitialized()
        {
            base.OnInitialized();
            SetBindingContext();
            SetParameters();
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
            SetParameters();
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
            await base.SetParametersAsync(parameters).ConfigureAwait(false);

            if (BindingContext != null)
                await BindingContext.SetParametersAsync(parameters).ConfigureAwait(false);
        }

        #endregion
    }
}