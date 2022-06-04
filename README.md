MvvmBlazor
================
[![Build Status](https://img.shields.io/endpoint.svg?url=https%3A%2F%2Factions-badge.atrox.dev%2Fchris579%2FMvvmBlazor%2Fbadge&style=flat-square)](https://github.com/chris579/MvvmBlazor/actions) [![NuGet](https://img.shields.io/nuget/v/MvvmBlazor.svg?style=flat-square)](https://www.nuget.org/packages/MvvmBlazor) [![NuGet](https://img.shields.io/nuget/dt/MvvmBlazor)](https://www.nuget.org/packages/MvvmBlazor) 


MvvmBlazor is a small framework for building Blazor WebAssembly and Blazor Server apps. With its easy-to-use MVVM pattern you
can increase your development speed while minimising the effort required to make it work.

## Getting started

MvvmBlazor is available on [NuGet](https://www.nuget.org/packages/MvvmBlazor). You will need **.NET 6** to use this
library.

The library needs to be added to the DI container in order to use it. This is done in your `Startup` class.

```c#
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvvm();
    }
}
```

## Usage

### Components

Components need to inherit the base class `MvvmBlazor.Components.MvvmComponentBase` if you want to inject your view
model manually. You can set a binding on any view model you like to.

If you want full support use `MvvmBlazor.Components.MvvmComponentBase<T>` and specify your view model type as a generic
argument.
Your view model will get auto injected into the component and set as a binding context.

#### BindingSource

The binding source is the default source object a binding will be made to. It is set automatically when using the base
class `MvvmBlazor.Components.MvvmComponentBase<T>` where `T` is your view model type. You can access it via
the `BindingContext` property in your component.

#### Bindings

Bindings are achieved via the `Bind` method in the component. If the value of the bound property has changed the
component will be told to rerender automatically. In this example we assume that the class `ClockViewModel` has a
property called `DateTime`.

```c#
@inherits MvvmComponentBase<ClockViewModel>

Current time: @Bind(x => x.DateTime)
```

Bindings can also be done by specifying the binding source explicitly:

```c#
@inherits MvvmComponentBase
@inject ClockViewModel ClockViewModel

Current time: @Bind(ClockViewModel, x => x.DateTime)
```

Bindings also handle background updating automatically. No need to invoke the main thread.

#### Collection Bindings

If you want to have a collection that automatically notifies the component when it has changed you should use one that
implements `INotifyCollectionChanged`, e.g. `ObservableCollection<T>`.

In List scenarios you often chain view models to achieve bindings for every list element on it's corresponding view
model. Given this view models

```c#
class MainViewModel
{
    public ObservableCollection<SubViewModel> Items { get; }
}

class SubViewModel
{
    private string _name;
    public string Name
    {
        get => _name;
        set => Set(ref, _name, value);
    }
}
```

you can use bindings on your sub view models like this

```c#
@foreach (var item in Bind(x => x.Items))
{
    <label>@Bind(item, x => x.Name)</label>
}
```

This way the name of every list item is bound to it's corresponding entry in the view. If you change the name on any
list item, it will be changed in the view too.

### EventHandlers

Event handles work just the way they work in blazor. When you use the non generic base class you can bind any injected
object on them.

```c#
@inherits MvvmComponentBase
@inject CounterViewModel CounterViewModel

<button @onclick="@CounterViewModel.IncrementCount">Click me</button>
```

When using the generic base class you can directly bind them to your binding context.

```c#
@inherits MvvmComponentBase<CounterViewModel>

<button @onclick="@BindingContext.IncrementCount">Click me</button>
```

### ViewModel

View models need to inherit the base class `MvvmBlazor.ViewModel.ViewModelBase`.

If you register a view model as scoped it will be tied to the lifetime of the component and disposed when the component is disposed.
This allows you to inject scoped services that should be short lived (e.g. a `DbContext`) without the need for using a factory.

Note: Some services need to be resolved from the root service provider (e.g. `NavigationManager`). To do this you can access the root service provider via the `RootServiceProvider` property.

#### Property implementation

Bindable properties need to raise the `PropertyChanged` event on the ViewModel.

The `Set`-Method is performing an equality check and is raising this event if needed. An example implementation could
look like this:

```c#
private int _currentCount;
public int CurrentCount
{
    get => _currentCount;
    set => Set(ref _currentCount, value);
}
```

As an alternative you can leverage the power of source generators to do the tedious work for you.
Just declare a private field inside of a view model and decorate if with the `Notify` attribute. The matching property will be auto generated for you.
```c#
[Notify]
private int _currentCount;
```
Note: Some third party IDEs may not recognize source generators at the current point. They could report that the property does not exist while the project builds fine.

#### Lifecycle methods

View models have access to the same lifecycle methods as a component when they are set as a binding context. They are
documented [here](https://docs.microsoft.com/en-us/aspnet/core/blazor/components?view=aspnetcore-3.0#lifecycle-methods).

#### Parameters

Parameters are automatically populated to the view model. Declare a parameter in your component

```c#
@code {
    [Parameter]
    public string Name { get; set; }
}
```

and declare the same parameter in your view model

```c#
class ViewModel: ViewModelBase
{
    [Parameter]
    public string Name { get; set; }
}
```

Cascading parameters are supported as well.
The parameter will be passed when parameters are set on the component. More information regarding the lifecycle can be
found in
the [Microsoft Documentation](https://docs.microsoft.com/en-us/aspnet/core/blazor/components/lifecycle?view=aspnetcore-6.0#lifecycle-events)
.

##### Type conversion

When binding parameters in a component Blazor restricts you to a finite list of primitive types you can use. In some
scenarios you might want to bind to a strong type and perform automatic conversions to it. A typical use case for this
would be a strongly typed identifier that you use in your application.

View model parameters support type conversion
using [`TypeConverter`](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.typeconverter?view=net-6.0).
Your component parameters still needs to be a supported primitive type, however your view model parameter can be
strongly typed and will get auto converted. An example for this can be found in the TypedParameter sample.

#### Dispose

Since ViewModels are being injected through dependency injection in the scope of the component the
DI [takes care](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-5.0#disposal-of-services)
of disposing ViewModels.

## Advanced scenarios
Some libraries may force you to use a different base class than `MvvmComponentBase`.
To solve this issue you can create a custom mvvm component using source generators.

Create a partial class and decorate it with the `MvvmComponent` attribute.

```c#
[MvvmComponent]
public abstract partial class CustomComponentBase : UiLibraryComponentBase
{

}
```

As a reference point see [`MvvmComponentBase`](https://github.com/klemmchr/MvvmBlazor/blob/master/src/MvvmBlazor.Core/Components/MvvmComponentBase.cs) and [`MvvmComponentBase<T>`](https://github.com/klemmchr/MvvmBlazor/blob/master/src/MvvmBlazor.Core/Components/MvvmComponentBaseT.cs).
Both are generated by a source generator as well.

## Examples

Examples for Blazor and Serverside Blazor can be
found [here](https://github.com/chris579/MvvmBlazor/tree/master/samples).

You will find several projects in there

- *BlazorServersideSample*
  A server for the blazor serverside sample
- *BlazorClientsideSample.Server*
  The server for the blazor clientside sample
- *BlazorClientsideSample.Client*
  The client for the blazor clientside sample

These projects act as wrapper projects for the main functionality that is shared among these examples.

- *BlazorSample.Components*
  The components and pages for the samples
- *BlazorSample.ViewModels*
  The view models for the pages
- *BlazorSample.Domain*
  Domain logic, stuff shared between components and view models

