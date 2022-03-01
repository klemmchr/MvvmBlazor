namespace MvvmBlazor.Tests.Internal.WeakEventListener;

public class WeakEventManagerTests
{
    [Fact]
    public void AddWeakEventListener_collection_fires_event()
    {
        var collectionAddObject = new object();
        var source = new Mock<INotifyCollectionChanged>();

        var invocations = 0;

        var wem = new WeakEventManager();
        wem.AddWeakEventListener(
            source.Object,
            (s, a) =>
            {
                s.ShouldBe(source.Object);
                a.Action.ShouldBe(NotifyCollectionChangedAction.Add);
                a.NewItems.ShouldNotBeNull();
                a.NewItems.Count.ShouldBe(1);
                a.NewItems[0].ShouldBe(collectionAddObject);

                invocations++;
            }
        );

        source.Raise(
            x => x.CollectionChanged += null,
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, collectionAddObject)
        );

        invocations.ShouldBe(1);
    }

    [Fact]
    public void AddWeakEventListener_custom_fires_event()
    {
        var source = new Mock<INotifyPropertyChanged>();

        var invocations = 0;

        var wem = new WeakEventManager();
        wem.AddWeakEventListener<INotifyPropertyChanged, PropertyChangedEventArgs>(
            source.Object,
            nameof(INotifyPropertyChanged.PropertyChanged),
            (s, a) =>
            {
                s.ShouldBe(source.Object);

                invocations++;
            }
        );

        source.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs("Foo"));

        invocations.ShouldBe(1);
    }

    [Fact]
    public void AddWeakEventListener_fires_event_after_GC()
    {
        var source = new Mock<INotifyPropertyChanged>();

        var invocations = 0;

        var wem = new WeakEventManager();
        wem.AddWeakEventListener<INotifyPropertyChanged, PropertyChangedEventArgs>(
            source.Object,
            nameof(INotifyPropertyChanged.PropertyChanged),
            (s, a) =>
            {
                s.ShouldBe(source.Object);

                invocations++;
            }
        );

        GC.Collect();

        source.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs("Foo"));

        invocations.ShouldBe(1);
    }

    [Fact]
    public void AddWeakEventListener_property_fires_event()
    {
        const string propertyName = "TestProperty";

        var source = new Mock<INotifyPropertyChanged>();

        var invocations = 0;

        var wem = new WeakEventManager();
        wem.AddWeakEventListener(
            source.Object,
            (s, a) =>
            {
                s.ShouldBe(source.Object);
                a.PropertyName.ShouldBe(propertyName);

                invocations++;
            }
        );

        source.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs(propertyName));

        invocations.ShouldBe(1);
    }

    [Fact]
    public void ClearWeakEventListeners_event_no_longer_fires()
    {
        var source = new Mock<INotifyPropertyChanged>();

        var invocations = 0;

        var wem = new WeakEventManager();
        wem.AddWeakEventListener<INotifyPropertyChanged, PropertyChangedEventArgs>(
            source.Object,
            nameof(INotifyPropertyChanged.PropertyChanged),
            (s, a) =>
            {
                s.ShouldBe(source.Object);

                invocations++;
            }
        );

        wem.ClearWeakEventListeners();

        source.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs("Foo"));

        invocations.ShouldBe(0);
    }

    [Fact]
    public void RemoveWeakEventListener_event_no_longer_fires()
    {
        var source = new Mock<INotifyPropertyChanged>();

        var invocations = 0;

        var wem = new WeakEventManager();
        wem.AddWeakEventListener<INotifyPropertyChanged, PropertyChangedEventArgs>(
            source.Object,
            nameof(INotifyPropertyChanged.PropertyChanged),
            (s, a) =>
            {
                s.ShouldBe(source.Object);

                invocations++;
            }
        );

        wem.RemoveWeakEventListener(source.Object);

        source.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs("Foo"));

        invocations.ShouldBe(0);
    }
}