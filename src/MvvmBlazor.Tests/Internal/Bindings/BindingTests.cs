using System.Collections.ObjectModel;

namespace MvvmBlazor.Tests.Internal.Bindings;

public class BindingTests
{
    [Fact]
    public void Adds_collection_event_listener_when_initializing()
    {
        const string propertyName = "propertyName";

        var source = new Mock<INotifyPropertyChanged>();
        var collection = new Mock<INotifyCollectionChanged>();
        var wem = new Mock<IWeakEventManager>();
        var propertyInfo = new Mock<PropertyInfo>();
        propertyInfo.Setup(x => x.Name).Returns(propertyName);
        propertyInfo.Setup(x => x.PropertyType).Returns(typeof(INotifyCollectionChanged));
        propertyInfo.SetupSequence(x => x.GetValue(It.IsAny<INotifyPropertyChanged>(), null))
            .Returns(collection.Object);

        using var binding = new Binding(source.Object, propertyInfo.Object, wem.Object);
        binding.Initialize();

        wem.Verify(
            x => x.AddWeakEventListener(
                It.IsAny<INotifyCollectionChanged>(),
                It.IsAny<Action<INotifyCollectionChanged, NotifyCollectionChangedEventArgs>>()
            )
        );
    }

    [Fact]
    public void Adds_collection_event_listener_when_property_changes_to_not_null()
    {
        const string propertyName = "propertyName";

        var source = new Mock<INotifyPropertyChanged>();
        var collection = new Mock<INotifyCollectionChanged>();
        var wem = new Mock<IWeakEventManager>();
        var propertyInfo = new Mock<PropertyInfo>();
        propertyInfo.Setup(x => x.Name).Returns(propertyName);
        propertyInfo.Setup(x => x.PropertyType).Returns(typeof(INotifyCollectionChanged));
        wem.Setup(
                x => x.AddWeakEventListener(
                    It.IsAny<INotifyPropertyChanged>(),
                    It.IsAny<Action<INotifyPropertyChanged, PropertyChangedEventArgs>>()
                )
            )
            .Callback<INotifyPropertyChanged, Action<INotifyPropertyChanged, PropertyChangedEventArgs>>(
                (sender, action) => sender.PropertyChanged += (s, a) => action((INotifyPropertyChanged)s!, a)
            );
        propertyInfo.Setup(x => x.GetValue(It.IsAny<INotifyPropertyChanged>(), null)).Returns(collection.Object);

        using var binding = new Binding(source.Object, propertyInfo.Object, wem.Object);
        binding.Initialize();

        source.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs(propertyName));
        wem.Verify(
            x => x.AddWeakEventListener(
                It.IsAny<INotifyCollectionChanged>(),
                It.IsAny<Action<INotifyCollectionChanged, NotifyCollectionChangedEventArgs>>()
            )
        );
    }

    [Fact]
    public void Dispose_removes_collection_listener()
    {
        const string propertyName = "propertyName";

        var source = new Mock<INotifyPropertyChanged>();
        var collection = new Mock<INotifyCollectionChanged>();
        var wem = new Mock<IWeakEventManager>();
        var propertyInfo = new Mock<PropertyInfo>();
        propertyInfo.Setup(x => x.Name).Returns(propertyName);
        propertyInfo.Setup(x => x.PropertyType).Returns(typeof(INotifyCollectionChanged));
        wem.Setup(
                x => x.AddWeakEventListener(
                    It.IsAny<INotifyPropertyChanged>(),
                    It.IsAny<Action<INotifyPropertyChanged, PropertyChangedEventArgs>>()
                )
            )
            .Callback<INotifyPropertyChanged, Action<INotifyPropertyChanged, PropertyChangedEventArgs>>(
                (sender, action) => sender.PropertyChanged += (s, a) => action((INotifyPropertyChanged)s!, a)
            );
        propertyInfo.SetupSequence(x => x.GetValue(It.IsAny<INotifyPropertyChanged>(), null))
            .Returns(collection.Object)
            .Returns((object?)null);

        using (var binding = new Binding(source.Object, propertyInfo.Object, wem.Object))
        {
            binding.Initialize();
        }

        wem.Verify(x => x.RemoveWeakEventListener(collection.Object));
    }

    [Fact]
    public void Dispose_removes_event_listener()
    {
        const string propertyName = "propertyName";

        var source = new Mock<INotifyPropertyChanged>();
        var collection = new Mock<INotifyCollectionChanged>();
        var wem = new Mock<IWeakEventManager>();
        var propertyInfo = new Mock<PropertyInfo>();
        propertyInfo.Setup(x => x.Name).Returns(propertyName);
        propertyInfo.Setup(x => x.PropertyType).Returns(typeof(INotifyCollectionChanged));
        wem.Setup(
                x => x.AddWeakEventListener(
                    It.IsAny<INotifyPropertyChanged>(),
                    It.IsAny<Action<INotifyPropertyChanged, PropertyChangedEventArgs>>()
                )
            )
            .Callback<INotifyPropertyChanged, Action<INotifyPropertyChanged, PropertyChangedEventArgs>>(
                (sender, action) => sender.PropertyChanged += (s, a) => action((INotifyPropertyChanged)s!, a)
            );
        propertyInfo.Setup(x => x.GetValue(It.IsAny<INotifyPropertyChanged>(), null)).Returns(collection.Object);

        using (var binding = new Binding(source.Object, propertyInfo.Object, wem.Object))
        {
            binding.Initialize();
        }

        wem.Verify(x => x.RemoveWeakEventListener(source.Object));
    }

    [Fact]
    public void Equals_not_different_source_and_property()
    {
        const string propertyName = "propertyName";

        var source1 = new Mock<INotifyPropertyChanged>();
        var source2 = new Mock<INotifyPropertyChanged>();

        var propertyInfo1 = new Mock<PropertyInfo>();
        propertyInfo1.Setup(x => x.Name).Returns(propertyName);
        var wem = new Mock<IWeakEventManager>();
        var propertyInfo2 = new Mock<PropertyInfo>();
        propertyInfo2.Setup(x => x.Name).Returns(propertyName + "Foo");

        using var binding1 = new Binding(source1.Object, propertyInfo1.Object, wem.Object);
        using var binding2 = new Binding(source2.Object, propertyInfo2.Object, wem.Object);

        binding1.GetHashCode().ShouldNotBe(binding2.GetHashCode());
        binding1.Equals(binding2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_same_source_and_property()
    {
        const string propertyName = "propertyName";

        var source = new Mock<INotifyPropertyChanged>();
        var wem = new Mock<IWeakEventManager>();

        var propertyInfo = new Mock<PropertyInfo>();
        propertyInfo.Setup(x => x.Name).Returns(propertyName);

        using var binding1 = new Binding(source.Object, propertyInfo.Object, wem.Object);
        using var binding2 = new Binding(source.Object, propertyInfo.Object, wem.Object);

        binding1.GetHashCode().ShouldBe(binding2.GetHashCode());
        binding1.Equals(binding2).ShouldBeTrue();
    }

    [Fact]
    public void Ignores_PropertyChangedEvent_when_property_name_does_not_match()
    {
        const string propertyName = "propertyName";

        var source = new Mock<INotifyPropertyChanged>();
        var collection = new Mock<INotifyCollectionChanged>();
        var wem = new Mock<IWeakEventManager>();
        var propertyInfo = new Mock<PropertyInfo>();
        propertyInfo.Setup(x => x.Name).Returns(propertyName);
        propertyInfo.Setup(x => x.PropertyType).Returns(typeof(INotifyCollectionChanged));
        wem.Setup(
                x => x.AddWeakEventListener(
                    It.IsAny<INotifyPropertyChanged>(),
                    It.IsAny<Action<INotifyPropertyChanged, PropertyChangedEventArgs>>()
                )
            )
            .Callback<INotifyPropertyChanged, Action<INotifyPropertyChanged, PropertyChangedEventArgs>>(
                (sender, action) => sender.PropertyChanged += (s, a) => action((INotifyPropertyChanged)s!, a)
            );
        propertyInfo.Setup(x => x.GetValue(It.IsAny<INotifyPropertyChanged>(), null)).Returns(collection.Object);

        var bindingValueChangedRaised = false;
        using var binding = new Binding(source.Object, propertyInfo.Object, wem.Object);
        binding.Initialize();
        binding.BindingValueChanged += (s, e) => bindingValueChangedRaised = true;

        source.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs("foo"));

        bindingValueChangedRaised.ShouldBeFalse();
    }

    [Fact]
    public void Does_not_raise_BindingValueChanged_when_uninitialized()
    {
        const string propertyName = "propertyName";

        var source = new Mock<INotifyPropertyChanged>();
        var wem = new Mock<IWeakEventManager>();
        var propertyInfo = new Mock<PropertyInfo>();
        propertyInfo.Setup(x => x.Name).Returns(propertyName);

        var hasChanged = false;
        using var binding = new Binding(source.Object, propertyInfo.Object, wem.Object);
        binding.BindingValueChanged += (sender, args) => { hasChanged = true; };

        source.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs(propertyName));
        hasChanged.ShouldBeFalse();
    }

    [Fact]
    public void Raises_BindingValueChanged_when_collection_changed()
    {
        const string propertyName = "propertyName";

        var source = new Mock<INotifyPropertyChanged>();
        var collection = new Mock<ObservableCollection<object>>();
        var wem = new Mock<IWeakEventManager>();

        var propertyInfo = new Mock<PropertyInfo>();
        propertyInfo.Setup(x => x.Name).Returns(propertyName);
        propertyInfo.Setup(x => x.PropertyType).Returns(typeof(INotifyCollectionChanged));
        propertyInfo.Setup(x => x.GetValue(It.IsAny<object>(), It.IsAny<object[]>())).Returns(collection.Object);
        wem.Setup(
                x => x.AddWeakEventListener(
                    It.IsAny<INotifyCollectionChanged>(),
                    It.IsAny<Action<INotifyCollectionChanged, NotifyCollectionChangedEventArgs>>()
                )
            )
            .Callback<INotifyCollectionChanged, Action<INotifyCollectionChanged, NotifyCollectionChangedEventArgs>>(
                (sender, action) => sender.CollectionChanged += (s, a) => action((INotifyCollectionChanged)s!, a)
            );

        var hasChanged = false;
        using var binding = new Binding(source.Object, propertyInfo.Object, wem.Object);
        binding.Initialize();
        binding.BindingValueChanged += (sender, args) =>
        {
            sender.ShouldBe(binding);
            args.ShouldBe(EventArgs.Empty);
            hasChanged = true;
        };

        collection.Raise(
            x => x.CollectionChanged += null,
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<object> { new() })
        );

        hasChanged.ShouldBeTrue();
    }

    [Fact]
    public void Raises_BindingValueChanged_when_property_changed()
    {
        const string propertyName = "propertyName";

        var source = new Mock<INotifyPropertyChanged>();
        var wem = new Mock<IWeakEventManager>();
        var propertyInfo = new Mock<PropertyInfo>();
        propertyInfo.Setup(x => x.Name).Returns(propertyName);
        wem.Setup(
                x => x.AddWeakEventListener(
                    It.IsAny<INotifyPropertyChanged>(),
                    It.IsAny<Action<INotifyPropertyChanged, PropertyChangedEventArgs>>()
                )
            )
            .Callback<INotifyPropertyChanged, Action<INotifyPropertyChanged, PropertyChangedEventArgs>>(
                (sender, action) => sender.PropertyChanged += (s, a) => action((INotifyPropertyChanged)s!, a)
            );

        var hasChanged = false;
        using var binding = new Binding(source.Object, propertyInfo.Object, wem.Object);
        binding.Initialize();
        binding.BindingValueChanged += (sender, args) =>
        {
            sender.ShouldBe(binding);
            args.ShouldBe(EventArgs.Empty);
            hasChanged = true;
        };

        source.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs(propertyName));

        hasChanged.ShouldBeTrue();
    }

    [Fact]
    public void Removes_collection_event_listener_when_property_changes_to_null()
    {
        const string propertyName = "propertyName";

        var source = new Mock<INotifyPropertyChanged>();
        var collection = new Mock<INotifyCollectionChanged>();
        var wem = new Mock<IWeakEventManager>();
        var propertyInfo = new Mock<PropertyInfo>();
        propertyInfo.Setup(x => x.Name).Returns(propertyName);
        propertyInfo.Setup(x => x.PropertyType).Returns(typeof(INotifyCollectionChanged));
        wem.Setup(
                x => x.AddWeakEventListener(
                    It.IsAny<INotifyPropertyChanged>(),
                    It.IsAny<Action<INotifyPropertyChanged, PropertyChangedEventArgs>>()
                )
            )
            .Callback<INotifyPropertyChanged, Action<INotifyPropertyChanged, PropertyChangedEventArgs>>(
                (sender, action) => sender.PropertyChanged += (s, a) => action((INotifyPropertyChanged)s!, a)
            );
        propertyInfo.SetupSequence(x => x.GetValue(It.IsAny<INotifyPropertyChanged>(), null))
            .Returns(collection.Object)
            .Returns((object?)null);

        using var binding = new Binding(source.Object, propertyInfo.Object, wem.Object);
        binding.Initialize();

        source.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs(propertyName));
        wem.Verify(x => x.RemoveWeakEventListener(It.IsAny<INotifyCollectionChanged>()));
    }

    [Fact]
    public void Should_not_raise_BindingValueChanged_on_collection_when_uninitialized()
    {
        const string propertyName = "propertyName";

        var source = new Mock<INotifyPropertyChanged>();
        var collection = new Mock<ObservableCollection<object>>();
        var wem = new Mock<IWeakEventManager>();
        var propertyInfo = new Mock<PropertyInfo>();
        propertyInfo.Setup(x => x.Name).Returns(propertyName);
        propertyInfo.Setup(x => x.PropertyType).Returns(typeof(INotifyCollectionChanged));
        propertyInfo.Setup(x => x.GetValue(It.IsAny<object>(), It.IsAny<object[]>())).Returns(collection.Object);

        var hasChanged = false;
        using var binding = new Binding(source.Object, propertyInfo.Object, wem.Object);
        binding.BindingValueChanged += (_, __) => hasChanged = true;

        collection.Raise(
            x => x.CollectionChanged += null,
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<object> { new() })
        );

        hasChanged.ShouldBeFalse();
    }
}