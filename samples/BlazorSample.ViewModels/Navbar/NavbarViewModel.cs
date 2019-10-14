using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MvvmBlazor.ViewModel;

namespace BlazorSample.ViewModels.Navbar
{
    public class NavbarViewModel : ViewModelBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private bool _isMenuOpen;

        public ObservableCollection<NavbarItemViewModel> NavbarItems { get; }
        public bool IsMenuOpen
        {
            get => _isMenuOpen;
            set => Set(ref _isMenuOpen, value, nameof(IsMenuOpen));
        }

        public NavbarViewModel(INavbarService navbarService,
            INavigationService navigationService,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            NavbarItems = new ObservableCollection<NavbarItemViewModel>(
                navbarService.NavbarItems.Select(x => new NavbarItemViewModel(x.DisplayName, x.Template)));
        }

        public Task Logout()
        {
            return _httpContextAccessor.HttpContext.SignOutAsync();
        }
        
        public void ToggleMenu()
        {
            IsMenuOpen = !IsMenuOpen;
        }
    }
}