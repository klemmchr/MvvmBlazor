using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MvvmBlazor.ViewModel;

namespace BlazorSample.ViewModels;

public class ParametersViewModel : ViewModelBase
{
    private NavigationManager _navigationManager = null!;

    [Parameter] public string? Name { get; set; }

    public string? NewName { get; set; }

    public void NavigateToNewName()
    {
        if (string.IsNullOrEmpty(NewName))
        {
            return;
        }

        _navigationManager.NavigateTo($"/parameters/{NewName}");
    }

    public override void OnInitialized()
    {
        _navigationManager = RootServiceProvider.GetRequiredService<NavigationManager>();
    }
}