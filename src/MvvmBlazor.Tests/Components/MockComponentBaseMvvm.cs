using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MvvmBlazor.Components;
using MvvmBlazor.ViewModel;

namespace MvvmBlazor.Tests.Components
{
    public class MockMvvmComponentBase: MvvmComponentBase
    {
        public ViewModelBase Context => BindingContext;

        private readonly ViewModelBase _viewModel;

        public MockMvvmComponentBase(ViewModelBase viewModel)
        {
            _viewModel = viewModel;
        }

        protected override ViewModelBase SetBindingContext()
        {
            return _viewModel;
        }

        public void Initialized()
        {
            OnInitialized();
        }

        public Task InitializedAsync()
        {
            return OnInitializedAsync();
        }

        public void ParametersSet()
        {
            OnParametersSet();
        }

        public Task ParametersSetAsync()
        {
            return OnParametersSetAsync();
        }

        public bool Render()
        {
            return ShouldRender();
        }

        public void AfterRender(bool firstRender)
        {
            OnAfterRender(firstRender);
        }

        public Task AfterRenderAsync(bool firstRender)
        {
            return OnAfterRenderAsync(firstRender);
        }
    }
}
