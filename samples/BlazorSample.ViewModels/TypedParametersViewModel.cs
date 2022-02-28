using BlazorSample.Domain.Converters;

namespace BlazorSample.ViewModels;

public class TypedParametersViewModel : ViewModelBase
{
    private NavigationManager _navigationManager = null!;

    [Parameter] public IdType? Id { get; set; }

    public void NavigateToRandomId()
    {
        _navigationManager.NavigateTo($"/typed-parameters/{Guid.NewGuid()}");
    }

    public override void OnInitialized()
    {
        _navigationManager = RootServiceProvider.GetRequiredService<NavigationManager>();
    }

    public override void OnParametersSet()
    {
        if (Id is null)
        {
            NavigateToRandomId();
        }
    }
}