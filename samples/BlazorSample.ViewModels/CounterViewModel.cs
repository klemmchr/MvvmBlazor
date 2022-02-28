namespace BlazorSample.ViewModels;

public partial class CounterViewModel : ViewModelBase
{
    [Notify] private int _currentCount;

    public void IncrementCount()
    {
        CurrentCount++;
    }
}