MvvmBlazor
================
[![Build Status](https://img.shields.io/endpoint.svg?url=https%3A%2F%2Factions-badge.atrox.dev%2Fchris579%2FMvvmBlazor%2Fbadge&style=flat-square)](https://github.com/chris579/MvvmBlazor/actions)
[![NuGet](https://img.shields.io/nuget/v/MvvmBlazor.svg?style=flat-square)](https://www.nuget.org/packages/MvvmBlazor)

BlazorMVVM is a small framework for building Blazor and BlazorServerside apps. With it's simple to use MVVM pattern you can boost up your development speed while minimizing the hazzle to just make it work.

## Get started
MvvmBlazor is available on [NuGet](https://www.nuget.org/packages/MvvmBlazor). You will need .NET Core 3.0 or later to use this library.

## Usage
### Startup
The library needs to be initialized in order to use it. This is done in your `Startup` class.

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvvm();
    }
}
```

### Components
Components need to inherit the base class `MvvmBlazor.Components.MvvmComponentBase` if you want to inject your view model manually. You can set a binding on any view model you like to.

If you want full support use `MvvmBlazor.Components.MvvmComponentBase<T>` and specify your view model type as a generic argument.
Your view model will get auto injected into the component and set as a binding context.

#### BindingSource
The binding source is the default source object a binding will be made to. It is set automatically when using the base class `MvvmBlazor.Components.MvvmComponentBase<T>` where `T` is your view model type. You can access it via the `BindingContext` property in your component.

#### Bindings
Bindings are achieved via the `Bind` method in the component. If the value of the bound property has changed the component will be told to rerender automatically. In this example we assume that the class `ClockViewModel` has a property called `DateTime`.
```csharp
@inherits MvvmComponentBase<ClockViewModel>

Current time: @Bind(x => x.DateTime)
```

Bindings can also be done by specifying the binding source explicitly:
```csharp
@inherits MvvmComponentBase
@inject ClockViewModel ClockViewModel

Current time: @Bind(ClockViewModel, x => x.DateTime)
```

Bindings also handle background updating automatically. No need to invoke the main thread.

### EventHandlers
Event handles work just the way they work in blazor. When you use the non generic base class you can bind any injected object on them.

```csharp
@inherits MvvmComponentBase
@inject CounterViewModel CounterViewModel

<button @onclick="@CounterViewModel.IncrementCount">Click me</button>
```

When using the generic base class you can directly bind them to your binding context.
```csharp
@inherits MvvmComponentBase<CounterViewModel>

<button @onclick="@BindingContext.IncrementCount">Click me</button>
```

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
View models have access to the same lifecycle methods as a component when they are set as a binding context. They are documented [here](https://docs.microsoft.com/en-us/aspnet/core/blazor/components?view=aspnetcore-3.0#lifecycle-methods).

#### Dispose
View models are implementing the [`IDisposable` pattern of Blazor](https://docs.microsoft.com/en-us/aspnet/core/blazor/components?view=aspnetcore-3.0#component-disposal-with-idisposable). Inside ViewModels, Disposing is achieved by overriding the `Dispose(bool disposing)` method.

## Examples
Examples for Blazor and Serverside Blazor can be found [here](https://github.com/chris579/MvvmBlazor/tree/master/samples).
