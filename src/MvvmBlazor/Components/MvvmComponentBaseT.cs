using MvvmBlazor.ViewModel;

namespace MvvmBlazor.Components
{
    [MvvmComponent]
    public abstract partial class MvvmComponentBase<T> : MvvmComponentBase where T : ViewModelBase
    {
    }
}