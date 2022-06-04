namespace BlazorSample.ViewModels;

public class CascadingViewModel : ViewModelBase
{
    [CascadingParameter]
    public string? Name { get; set; }
}