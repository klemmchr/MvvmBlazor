using Microsoft.AspNetCore.Components;
using MvvmBlazor;

namespace BlazorSample.Components.Shared;

[MvvmComponent]
public abstract partial class CustomBaseComponent<T> : CustomBaseComponent
{

}

[MvvmComponent]
public abstract partial class CustomBaseComponent : ComponentBase
{

}