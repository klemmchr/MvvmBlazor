using System;
using System.Collections.Specialized;
using System.ComponentModel;
using Moq;
using MvvmBlazor.Internal.WeakEventListener;
using Shouldly;
using Xunit;

namespace MvvmBlazor.Tests.Internal.WeakEventListener
{
    public class WeakEventManagerTests
    {
        [Fact]
        public void WeakEventManager_AddWeakEventListener_Collection_FiresEvent()
        {
            var collectionAddObject = new object();
            var source = new Mock<INotifyCollectionChanged>();

            var invocations = 0;

            var wem = new WeakEventManager();
            wem.AddWeakEventListener(source.Object, (s, a) =>
            {
                s.ShouldBe(source.Object);
                a.Action.ShouldBe(NotifyCollectionChangedAction.Add);
                a.NewItems.Count.ShouldBe(1);
                a.NewItems[0].ShouldBe(collectionAddObject);

                invocations++;
            });

            source.Raise(x => x.CollectionChanged += null,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, collectionAddObject));

            invocations.ShouldBe(1);
        }

        [Fact]
        public void WeakEventManager_AddWeakEventListener_Custom_FiresEvent()
        {
            var source = new Mock<INotifyPropertyChanged>();

            var invocations = 0;

            var wem = new WeakEventManager();
            wem.AddWeakEventListener<INotifyPropertyChanged, PropertyChangedEventArgs>(source.Object,
                nameof(INotifyPropertyChanged.PropertyChanged), (s, a) =>
                {
                    s.ShouldBe(source.Object);

                    invocations++;
                });

            source.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs("Foo"));

            invocations.ShouldBe(1);
        }

        [Fact]
        public void WeakEventManager_AddWeakEventListener_FiresEventAfterGC()
        {
            var source = new Mock<INotifyPropertyChanged>();

            var invocations = 0;

            var wem = new WeakEventManager();
            wem.AddWeakEventListener<INotifyPropertyChanged, PropertyChangedEventArgs>(source.Object,
                nameof(INotifyPropertyChanged.PropertyChanged), (s, a) =>
                {
                    s.ShouldBe(source.Object);

                    invocations++;
                });

            GC.Collect();

            source.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs("Foo"));

            invocations.ShouldBe(1);
        }

        [Fact]
        public void WeakEventManager_AddWeakEventListener_Property_FiresEvent()
        {
            const string propertyName = "TestProperty";

            var source = new Mock<INotifyPropertyChanged>();

            var invocations = 0;

            var wem = new WeakEventManager();
            wem.AddWeakEventListener(source.Object, (s, a) =>
            {
                s.ShouldBe(source.Object);
                a.PropertyName.ShouldBe(propertyName);

                invocations++;
            });

            source.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs(propertyName));

            invocations.ShouldBe(1);
        }

        [Fact]
        public void WeakEventManager_ClearWeakEventListeners_EventNoLongerFires()
        {
            var source = new Mock<INotifyPropertyChanged>();

            var invocations = 0;

            var wem = new WeakEventManager();
            wem.AddWeakEventListener<INotifyPropertyChanged, PropertyChangedEventArgs>(source.Object,
                nameof(INotifyPropertyChanged.PropertyChanged), (s, a) =>
                {
                    s.ShouldBe(source.Object);

                    invocations++;
                });

            wem.ClearWeakEventListeners();

            source.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs("Foo"));

            invocations.ShouldBe(0);
        }

        [Fact]
        public void WeakEventManager_RemoveWeakEventListener_EventNoLongerFires()
        {
            var source = new Mock<INotifyPropertyChanged>();

            var invocations = 0;

            var wem = new WeakEventManager();
            wem.AddWeakEventListener<INotifyPropertyChanged, PropertyChangedEventArgs>(source.Object,
                nameof(INotifyPropertyChanged.PropertyChanged), (s, a) =>
                {
                    s.ShouldBe(source.Object);

                    invocations++;
                });

            wem.RemoveWeakEventListener(source.Object);

            source.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs("Foo"));

            invocations.ShouldBe(0);
        }
    }
}