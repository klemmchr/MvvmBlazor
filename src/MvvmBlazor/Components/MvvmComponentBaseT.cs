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
        private IViewModelParameterSetter? _viewModelParameterSetter;
        
        internal MvvmComponentBase(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            SetBindingContext();
        }

        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once PublicConstructorInAbstractClass
        public MvvmComponentBase() {}

        protected internal T BindingContext { get; set; } = null!;

        private void SetBindingContext()
        {
            // ReSharper disable once ConstantNullCoalescingCondition
            BindingContext ??= ServiceProvider.GetRequiredService<T>();
        }

        private void SetParameters()
        {
            if (BindingContext is null)
                throw new InvalidOperationException($"{nameof(BindingContext)} is not set");

            _viewModelParameterSetter ??= ServiceProvider.GetRequiredService<IViewModelParameterSetter>();
            _viewModelParameterSetter.ResolveAndSet(this, BindingContext);
        }

        protected internal TValue Bind<TValue>(Expression<Func<T, TValue>> property)
        {
            if (BindingContext is null)
                throw new InvalidOperationException($"{nameof(BindingContext)} is not set");

            return AddBinding(BindingContext, property);
        }

        #region Lifecycle Methods

        /// <inheritdoc />
        protected override void OnInitialized()
        {
            base.OnInitialized();
            SetBindingContext();
            SetParameters();
            BindingContext?.OnInitialized();
        }

        /// <inheritdoc />
        protected override Task OnInitializedAsync()
        {
            return BindingContext?.OnInitializedAsync() ?? Task.CompletedTask;
        }

        /// <inheritdoc />
        protected override void OnParametersSet()
        {
            SetParameters();
            BindingContext?.OnParametersSet();
        }

        /// <inheritdoc />
        protected override Task OnParametersSetAsync()
        {
            return BindingContext?.OnParametersSetAsync() ?? Task.CompletedTask;
        }

        /// <inheritdoc />
        protected override bool ShouldRender()
        {
            return BindingContext?.ShouldRender() ?? true;
        }

        /// <inheritdoc />
        protected override void OnAfterRender(bool firstRender)
        {
            BindingContext?.OnAfterRender(firstRender);
        }

        /// <inheritdoc />
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            return BindingContext?.OnAfterRenderAsync(firstRender) ?? Task.CompletedTask;
        }

        /// <inheritdoc />
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters).ConfigureAwait(false);

            if (BindingContext != null)
                await BindingContext.SetParametersAsync(parameters).ConfigureAwait(false);
        }

        #endregion
    }
}