MvvmBlazor
================
[![Build Status](https://img.shields.io/endpoint.svg?url=https%3A%2F%2Factions-badge.atrox.dev%2Fchris579%2FMvvmBlazor%2Fbadge&style=flat-square)](https://github.com/chris579/MvvmBlazor/actions)
[![NuGet](https://img.shields.io/nuget/v/MvvmBlazor.svg?style=flat-square)](https://www.nuget.org/packages/MvvmBlazor)

BlazorMVVM is a small framework for building Blazor and BlazorServerside apps. With its simple to use MVVM pattern you
can boost up your development speed while minimizing the hazzle to just make it work.

## Get started

MvvmBlazor is available on [NuGet](https://www.nuget.org/packages/MvvmBlazor). You will need **.NET 5** to use this
library. The library needs to be added to the DI container in order to use it. This is done in your `Startup` class.

```csharp
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
argument. Your view model will get auto injected into the component and set as a binding context.

#### BindingSource

The binding source is the default source object a binding will be made to. It is set automatically when using the base
class `MvvmBlazor.Components.MvvmComponentBase<T>` where `T` is your view model type. You can access it via
the `BindingContext` property in your component.

#### Bindings

Bindings are achieved via the `Bind` method in the component. If the value of the bound property has changed the
component will be told to rerender automatically. In this example we assume that the class `ClockViewModel` has a
property called `DateTime`.

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

#### Collection Bindings

If you want to have a collection that automatically notifies the component when it has changed you should use one that
implements `INotifyCollectionChanged`, e.g. `ObservableCollection<T>`.

In List scenarios you often chain view models to achieve bindings for every list element on it's corresponding view
model. Given this view models

```csharp
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

```csharp
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

The `Set`-Method is performing an equality check and is raising this event if needed. An example implementation could
look like this:

```csharp
private int _currentCount;
public int CurrentCount
{
    get => _currentCount;
    set => Set(ref _currentCount, value);
}
```

#### Lifecycle methods

View models have access to the same lifecycle methods as a component when they are set as a binding context. They are
documented [here](https://docs.microsoft.com/en-us/aspnet/core/blazor/components?view=aspnetcore-3.0#lifecycle-methods).

#### Parameters

Parameters are automatically populated to the view model. Declare a parameter in your component

```csharp
@code {
    [Parameter]
    public string Name { get; set; }
}
```

and declare the same parameter in your view model

```csharp
class ViewModel: ViewModelBase
{
    [Parameter]
    public string Name { get; set; }
}
```

The parameter will be passed before `OnInitialized` and `OnInitializedAsync` are invoked. Therefore, the parameters are
available from any lifecycle method. You can't access them in your constructor and you're not supposed to do that
either.

#### Dispose

Since ViewModels are being injected through depencency injection the
DI [takes care](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-5.0#disposal-of-services)
of disposing ViewModels.

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

This sample tries to incorporate a ports and adapters architectural approach and shows that this library works the same
for the client and the server. It also displays the advantages of the MVVM pattern which allows you to share the logic
for your frontend even when the business logic differs.

As an example the weather forecast view model just references the interface of the service that is responsible to gather
data. For Blazor serverside, it directly populates it, for Blazor serverside it gathers them from an api. The advantages
of such patterns will especially get interesting when Blazor Native and Blazor Embedded will be available.

