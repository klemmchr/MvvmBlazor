using MvvmBlazor.ViewModel;

namespace MvvmBlazor.Tests.Components
{
    public class TestViewModel : ViewModelBase
    {
        public string _testProperty;

        public string TestProperty
        {
            get => _testProperty;
            set => Set(ref _testProperty, value);
        }
    }
}