using System.Threading.Tasks;
using Moq;
using MvvmBlazor.Components;
using MvvmBlazor.ViewModel;
using Shouldly;
using Xunit;

namespace MvvmBlazor.Tests.Components
{
    public class ComponentBaseMvvmTests
    {
        private (Mock<ViewModelBase> viewModel, Mock<IDependencyResolver> resolver) GetResolver()
        {
            var viewModel = new Mock<ViewModelBase>();
            var resolver = new Mock<IDependencyResolver>();
            resolver.Setup(x => x.GetService<ViewModelBase>()).Returns(viewModel.Object);

            return (viewModel, resolver);
        }

        [Fact]
        public void OnInitializedAsync_CalledOnBindingContext()
        {
            var task = new Task(() => { });

            var (viewModel, resolver) = GetResolver();
            viewModel.Setup(x => x.OnInitializedAsync()).Returns(task);

            var component = new MockMvvmComponentBase(resolver.Object);
            component.Initialized();
            var res = component.InitializedAsync();
            res.ShouldBe(task);

            viewModel.Verify(x => x.OnInitialized());
            viewModel.Verify(x => x.OnInitializedAsync());
            viewModel.VerifyNoOtherCalls();
        }

        [Fact]
        public void OnParametersSet_CalledOnBindingContext()
        {
            var (viewModel, resolver) = GetResolver();

            var component = new MockMvvmComponentBase(resolver.Object);
            component.Initialized();
            component.ParametersSet();

            viewModel.Verify(x => x.OnInitialized());
            viewModel.Verify(x => x.OnParametersSet());
            viewModel.VerifyNoOtherCalls();
        }

        [Fact]
        public void OnParametersSetAsync_CalledOnBindingContext()
        {
            var task = new Task(() => { });
            var (viewModel, resolver) = GetResolver();
            viewModel.Setup(x => x.OnParametersSetAsync()).Returns(task);

            var component = new MockMvvmComponentBase(resolver.Object);
            component.Initialized();
            var res = component.ParametersSetAsync();
            res.ShouldBe(task);

            viewModel.Verify(x => x.OnInitialized());
            viewModel.Verify(x => x.OnParametersSetAsync());
            viewModel.VerifyNoOtherCalls();
        }
    }
}