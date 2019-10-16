using MvvmBlazor.ViewModel;

namespace BlazorSample.ViewModels.Navbar
{
    public class NavbarItemViewModel : ViewModelBase
    {
        public NavbarItemViewModel(string displayName, string template, string? icon)
        {
            DisplayName = displayName;
            Template = template;
            Icon = icon;
        }

        public string DisplayName { get; }

        public string Template { get; }

        public string? Icon { get; set; }

        private bool _isActive;

        public bool IsActive
        {
            get => _isActive;
            set => Set(ref _isActive, value, nameof(IsActive));
        }
    }
}