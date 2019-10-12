using Microsoft.AspNetCore.Components;
using MvvmBlazor.ViewModel;

namespace BlazorSample.Client.ViewModel
{
    public class ParametersViewModel: ViewModelBase
    {
        private readonly NavigationManager _navigationManager;

        [Parameter]
        public string Name { get; set; }

        public string NewName { get; set; }

        public ParametersViewModel(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

        public void NavigateToNewName()
        {
            if (string.IsNullOrEmpty(NewName))
                return;

            _navigationManager.NavigateTo($"/parameters/{NewName}");
        }
    }
}
