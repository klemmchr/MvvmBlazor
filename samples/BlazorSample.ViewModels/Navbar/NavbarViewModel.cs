using System.Collections.ObjectModel;
using System.Linq;
using BlazorSample.Domain.Services.Navbar;
using Microsoft.AspNetCore.Components;
using MvvmBlazor.ViewModel;

namespace BlazorSample.ViewModels.Navbar
{
    public class NavbarViewModel : ViewModelBase
    {
        private readonly NavigationManager _navigationManager;
        public ObservableCollection<NavbarItemViewModel> NavbarItems { get; }

        private bool _isMenuOpen = true;
        public bool IsMenuOpen
        {
            get => _isMenuOpen;
            set => Set(ref _isMenuOpen, value, nameof(IsMenuOpen));
        }

        public NavbarViewModel(INavbarService navbarService, NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
            navigationManager.LocationChanged += (_, __) => UpdateActiveItem();
            NavbarItems = new ObservableCollection<NavbarItemViewModel>(
                navbarService.NavbarItems.Select(x => new NavbarItemViewModel(x.DisplayName, x.Template!, x.Icon)));
            UpdateActiveItem();
        }
        
        public void ToggleMenu()
        {
            IsMenuOpen = !IsMenuOpen;
        }

        private void UpdateActiveItem()
        {
            var relativePath = _navigationManager.ToBaseRelativePath(_navigationManager.Uri);

            foreach (var navbarItem in NavbarItems)
            {
                if (string.IsNullOrEmpty(relativePath))
                {
                    navbarItem.IsActive = navbarItem.Template == "/";
                }
                else
                {
                    navbarItem.IsActive = navbarItem.Template.StartsWith("/" + relativePath);
                }
            }
        }
    }
}