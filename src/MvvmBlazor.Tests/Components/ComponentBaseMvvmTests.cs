using System.Threading.Tasks;
using Moq;
using MvvmBlazor.ViewModel;
using Xunit;
using Shouldly;

namespace MvvmBlazor.Tests.Components
{
    public class ComponentBaseMvvmTests
    {
        [Fact]
        public void Initialized_SetsBindingContext()
        {
            var bindingContext = new Mock<ViewModelBase>();
            
            var component = new MockComponentBaseMvvm(bindingContext.Object);
            component.Initialized();

            component.Context.ShouldBe(bindingContext.Object);

            bindingContext.Verify(x => x.OnInitialized());
            bindingContext.VerifyNoOtherCalls();
        }

        [Fact]
        public void OnInitializedAsync_CalledOnBindingContext()
        {
            var task = new Task(() => { });
            var bindingContext = new Mock<ViewModelBase>();
            bindingContext.Setup(x => x.OnInitializedAsync()).Returns(task);

            var component = new MockComponentBaseMvvm(bindingContext.Object);
            component.Initialized();
            var res = component.InitializedAsync();
            res.ShouldBe(task);

            bindingContext.Verify(x => x.OnInitialized());
            bindingContext.Verify(x => x.OnInitializedAsync());
            bindingContext.VerifyNoOtherCalls();
        }

        [Fact]
        public void OnParametersSet_CalledOnBindingContext()
        {
            var bindingContext = new Mock<ViewModelBase>();

            var component = new MockComponentBaseMvvm(bindingContext.Object);
            component.Initialized();
            component.ParametersSet();

            bindingContext.Verify(x => x.OnInitialized());
            bindingContext.Verify(x => x.OnParametersSet());
            bindingContext.VerifyNoOtherCalls();
        }

        [Fact]
        public void OnParametersSetAsync_CalledOnBindingContext()
        {
            var task = new Task(() => { });
            var bindingContext = new Mock<ViewModelBase>();
            bindingContext.Setup(x => x.OnParametersSetAsync()).Returns(task);

            var component = new MockComponentBaseMvvm(bindingContext.Object);
            component.Initialized();
            var res = component.ParametersSetAsync();
            res.ShouldBe(task);

            bindingContext.Verify(x => x.OnInitialized());
            bindingContext.Verify(x => x.OnParametersSetAsync());
            bindingContext.VerifyNoOtherCalls();
        }
    }
}