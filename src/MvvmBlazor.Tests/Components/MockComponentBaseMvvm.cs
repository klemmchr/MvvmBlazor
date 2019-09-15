using System.Threading.Tasks;
using MvvmBlazor.Components;
using MvvmBlazor.ViewModel;

namespace MvvmBlazor.Tests.Components
{
    public class MockMvvmComponentBase : MvvmComponentBase<ViewModelBase>
    {
        public MockMvvmComponentBase(IDependencyResolver dependencyResolver) : base(dependencyResolver) { }

        public ViewModelBase Context => BindingContext;

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