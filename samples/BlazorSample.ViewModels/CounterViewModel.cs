using MvvmBlazor.ViewModel;

namespace BlazorSample.ViewModels;

public class CounterViewModel : ViewModelBase
{
    private int _currentCount;

    public int CurrentCount
    {
        get => _currentCount;
        set => Set(ref _currentCount, value, nameof(CurrentCount));
    }

    public void IncrementCount()
    {
        CurrentCount++;
    }
}