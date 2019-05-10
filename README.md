MvvmBlazor
================
[![Build Status](https://dev.azure.com/chris579/MvvmBlazor/_apis/build/status/MvvmBlazor-CI?branchName=master)](https://dev.azure.com/chris579/MvvmBlazor/_build/latest?definitionId=3&branchName=master)
[![NuGet](https://img.shields.io/nuget/v/MvvmBlazor.svg?style=flat)](https://www.nuget.org/packages/MvvmBlazor)

A lightweight Blazor MVVM Library. The main goal is to achieve a MVVM Pattern in Blazor and Serverside Blazor. Plus, it aims to mitigate the need to manually force the component to rerender if binding values have changed changed without user interaction.

## Get started
MvvmBlazor will be available soon on NuGet.

## Usage
### Components
Components need to inherit the base class `MvvmBlazor.Components.ComponentBaseMvvm`.  
Layout components need to inherit the base class `MvvmBlazor.Components.LayoutComponentBaseMvvm`.

#### BindingSource
The binding source is the default source object a binding will be made to. It needs to be set by overriding the `SetBindingContext` method:
```csharp
protected override ViewModelBase SetBindingContext()
{
    return FetchDataViewModel;
}
```
The BindingSource will be disposed automatically when the component is disposed.

#### Bindings
Bindings are achieved via the `Bind` method in the component. If the value of the bound property has changed the component will be told to rerender automatically.
```csharp
@if (Bind(() => FetchDataViewModel.Forecasts) == null)
{
    <p>
        <em>Loading...</em>
    </p>
}

@foreach (var forecast in Bind(() => FetchDataViewModel.Forecasts))
{
    <tr>
        <td>@forecast.Date.ToShortDateString()</td>
        <td>@forecast.TemperatureC</td>
        <td>@forecast.TemperatureF</td>
        <td>@forecast.Summary</td>
    </tr>
}
```
Bindings can also be done by specifying the binding source explicitly:
```csharp
@if (Bind(FetchDataViewModel, () => FetchDataViewModel.Forecasts) == null)
{
    <p>
        <em>Loading...</em>
    </p>
}

@foreach (var forecast in Bind(FetchDataViewModel, () => FetchDataViewModel.Forecasts))
{
    <tr>
        <td>@forecast.Date.ToShortDateString()</td>
        <td>@forecast.TemperatureC</td>
        <td>@forecast.TemperatureF</td>
        <td>@forecast.Summary</td>
    </tr>
}
```
Bound view models are also disposed when the component is disposed.

They are not suited for real two-way bindings (e.g. for inputs). Two-way bindings can be done in the usual way by binding to the property directly.

### ViewModel
View models need to inherit the base class `MvvmBlazor.ViewModel.ViewModelBase`.

#### Property implementation
Bindable properties need to raise the `PropertyChanged` event on the ViewModel.

The `Set`-Method is performing an equality check and is raising this event if needed.
An example implementation could look like this:
```csharp
private int _currentCount;
public int CurrentCount
{
    get => _currentCount;
    set => Set(ref _currentCount, value);
}
```

#### Lifecycle methods
View models have access to the same lifecycle methods as a component. They are documented [here](https://docs.microsoft.com/en-us/aspnet/core/blazor/components?view=aspnetcore-3.0#lifecycle-methods).

#### Dispose
View models are implementing the [`IDisposable` pattern of Blazor](https://docs.microsoft.com/en-us/aspnet/core/blazor/components?view=aspnetcore-3.0#component-disposal-with-idisposable). Inside ViewModels, Disposing is achieved by overriding the `Cleanup` method. It will be called when the component is disposed.

## Examples
Examples for Blazor and Serverside Blazor can be found [here](https://github.com/chris579/MvvmBlazor/tree/master/samples).
