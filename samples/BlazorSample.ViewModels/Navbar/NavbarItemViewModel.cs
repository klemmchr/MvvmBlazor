using MvvmBlazor.ViewModel;

namespace BlazorSample.ViewModels.Navbar
{
    public class NavbarItemViewModel : ViewModelBase
    {
        private string _displayName;

        private bool _isActive;

        private string _template;

        public NavbarItemViewModel(string displayName, string template)
        {
            _displayName = displayName;
            _template = template;
        }

        public string DisplayName
        {
            get => _displayName;
            set => Set(ref _displayName, value, nameof(DisplayName));
        }

        public string Template
        {
            get => _template;
            set => Set(ref _template, value, nameof(Template));
        }

        public bool IsActive
        {
            get => _isActive;
            set => Set(ref _isActive, value, nameof(IsActive));
        }
    }
}