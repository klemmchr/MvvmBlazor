using System;
using System.Collections.ObjectModel;
using System.Linq;
using BlazorSample.Domain.Services.Navbar;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MvvmBlazor.ViewModel;

namespace BlazorSample.ViewModels.Navbar;

public class NavbarViewModel : ViewModelBase
{
    private bool _isMenuOpen = true;

    public ObservableCollection<NavbarItemViewModel> NavbarItems { get; }

    public bool IsMenuOpen
    {
        get => _isMenuOpen;
        set => Set(ref _isMenuOpen, value, nameof(IsMenuOpen));
    }

    public NavbarViewModel(INavbarService navbarService)
    {
        NavbarItems = new ObservableCollection<NavbarItemViewModel>(
            navbarService.NavbarItems.Select(x => new NavbarItemViewModel(x.DisplayName, x.Template!, x.Icon))
        );
    }

    public void ToggleMenu()
    {
        IsMenuOpen = !IsMenuOpen;
    }

    private void UpdateActiveItem(NavigationManager navigationManager)
    {
        var relativePath = navigationManager.ToBaseRelativePath(navigationManager.Uri);

        foreach (var navbarItem in NavbarItems)
            if (string.IsNullOrEmpty(relativePath))
            {
                navbarItem.IsActive = navbarItem.Template == "/";
            }
            else
            {
                navbarItem.IsActive = navbarItem.Template.StartsWith("/" + relativePath);
            }
    }

    public override void OnInitialized()
    {
        var navigationManager = RootServiceProvider.GetRequiredService<NavigationManager>();
        navigationManager.LocationChanged += (_, _) => UpdateActiveItem(navigationManager);

        UpdateActiveItem(navigationManager);
    }
}