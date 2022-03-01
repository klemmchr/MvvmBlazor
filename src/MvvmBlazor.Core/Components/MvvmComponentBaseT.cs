namespace MvvmBlazor.Components;

[MvvmComponent]
// ReSharper disable once PartialTypeWithSinglePart
public abstract partial class MvvmComponentBase<T> : MvvmComponentBase where T : ViewModelBase { }