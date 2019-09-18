using MvvmBlazor.ViewModel;

namespace MvvmBlazor.Tests.Components {
    public class TestViewModel : ViewModelBase
    {
        private string _testProperty;

        public string TestProperty
        {
            get => _testProperty;
            set => Set(ref _testProperty, value);
        }
    }
}