using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MvvmBlazor.Internal.WeakEventListener;
using Shouldly;
using Xunit;

namespace MvvmBlazor.Tests.Internal.WeakEventListener
{
    public class WeakEventManagerTests
    {
        private TestPublisher _publisher;
        private TestSubscriber _subscriber;

        private void Setup()
        {
            // we need to instantiate these in a separate method because there are some debug features that can keep objects
            // alive for the entire scope of the method they are created in even when they go out of scope and GC.Collect
            // is called explicitly
            _publisher = new TestPublisher();
            _subscriber = new TestSubscriber(_publisher);
        }

        [Fact]
        public void WeakEventManager_AddWeakEventListener_FiresEvent_Test()
        {
            var publisher = new TestPublisher();
            var subscriber = new TestSubscriber(publisher);

            publisher.Fire();

            subscriber.Invocations.ShouldBe(1);
        }

        [Fact]
        public void WeakEventManager_AddWeakEventListener_Typed_FiresEvent_Test()
        {
            var publisher = new TestPublisher();
            var subscriber = new TestSubscriber();
            subscriber.StartTyped(publisher);

            publisher.Fire();

            subscriber.Invocations.ShouldBe(1);
        }

        [Fact]
        public void WeakEventManager_AddWeakEventListener_Custom_FiresEvent_Test()
        {
            var publisher = new TestPublisher();
            var subscriber = new TestSubscriber();
            subscriber.StartCustom(publisher);

            publisher.FireProperty();

            subscriber.Invocations.ShouldBe(1);
        }

        [Fact]
        public void WeakEventManager_AddWeakEventListener_TypedCustom_FiresEvent_Test()
        {
            var publisher = new TestPublisher();
            var subscriber = new TestSubscriber();
            subscriber.StartTypedCustom(publisher);

            publisher.FireProperty();

            subscriber.Invocations.ShouldBe(1);
        }

        [Fact]
        public void WeakEventManager_AddWeakEventListener_Property_FiresEvent_Test()
        {
            var publisher = new TestPublisher();
            var subscriber = new TestSubscriber();
            subscriber.StartProperty(publisher);

            publisher.FireProperty();

            subscriber.Invocations.ShouldBe(1);
        }

        [Fact]
        public void WeakEventManager_AddWeakEventListener_FiresEventAfterGC_Test()
        {
            Setup();

            GC.Collect();

            _publisher.Fire();

            _subscriber.Invocations.ShouldBe(1);
        }

        [Fact]
        public void WeakEventManager_RemoveWeakEventListener_EventNoLongerFires_Test()
        {
            var publisher = new TestPublisher();
            var subscriber = new TestSubscriber(publisher);

            subscriber.Stop();
            publisher.Fire();

            subscriber.Invocations.ShouldBe(0);
        }

        [Fact]
        public void WeakEventManager_ClearWeakEventListeners_EventNoLongerFires_Test()
        {
            var publisher = new TestPublisher();
            var subscriber = new TestSubscriber(publisher);

            subscriber.Clear();
            publisher.Fire();

            subscriber.Invocations.ShouldBe(0);
        }

        [Fact]
        public void WeakEventManager_RemoveWeakEventListener_CustomDelegateEventNoLongerFires_Test()
        {
            var publisher = new TestPublisher();
            var subscriber = new TestSubscriber(publisher);
            subscriber.StartProperty(publisher);

            subscriber.Stop();
            publisher.FireProperty();

            subscriber.Invocations.ShouldBe(0);
        }

        [Fact]
        public void WeakEventManager_ClearWeakEventListeners_ClearsAllListeners_Test()
        {
            var publisher1 = new TestPublisher();
            var publisher2 = new TestPublisher();
            var subscriber = new TestSubscriber(publisher1);
            subscriber.Start(publisher2);

            subscriber.Clear();
            publisher1.Fire();
            publisher2.Fire();

            subscriber.Invocations.ShouldBe(0);
        }

        [Fact]
        public void WeakEventManager_AddWeakEventListener_Publisher_GC_Test()
        {
            Setup();
            var reference = new WeakReference(_subscriber);

            _subscriber = null;

            GC.Collect();

            reference.IsAlive.ShouldBeFalse();
            _publisher.Fire();
        }

        [Fact]
        public void WeakEventManager_AddWeakEventListener_Subscriber_GC_Test()
        {
            Setup();
            var reference = new WeakReference(_publisher);

            _publisher = null;
            _subscriber.Publisher = null;

            GC.Collect();

            reference.IsAlive.ShouldBeFalse();
        }

        [Fact]
        public void WeakEventManager_AddWeakEventListener_Both_GC_Test()
        {
            Setup();
            var reference1 = new WeakReference(_publisher);
            var reference2 = new WeakReference(_subscriber);

            _publisher = null;
            _subscriber = null;

            GC.Collect();

            reference1.IsAlive.ShouldBeFalse();
            reference2.IsAlive.ShouldBeFalse();
        }
    }

    public class TestEventArgs : EventArgs
    {
        public string Value { get; }

        public TestEventArgs(string value)
        {
            Value = value;
        }
    }

    public interface ITestEventSource : INotifyPropertyChanged
    {
        event EventHandler<TestEventArgs> TheEvent;
    }

    public class TestPublisherBase : ITestEventSource
    {
        public event EventHandler<TestEventArgs> TheEvent;
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnTheEvent([CallerMemberName] string name = "")
        {
            TheEvent?.Invoke(this, new TestEventArgs(name));
        }

        protected void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class TestPublisher : TestPublisherBase
    {
        public void Fire() => OnTheEvent();
        public void FireProperty() => OnPropertyChanged();
    }

    public class TestSubscriber
    {
        public int Invocations { get; private set; }

        public TestPublisher Publisher { get; set; }

        private readonly WeakEventManager _manager = new WeakEventManager();

        public TestSubscriber() { }

        public TestSubscriber(TestPublisher publisher)
        {
            Publisher = publisher;
            _manager.AddWeakEventListener<TestPublisher, TestEventArgs>(publisher, nameof(publisher.TheEvent), OnTheEvent);
        }

        public void StartTyped(TestPublisher publisher)
        {
            _manager.AddWeakEventListener<TestPublisher, TestEventArgs>(publisher, (t, e) => t.TheEvent += e, (t, e) => t.TheEvent -= e, OnTheEvent);
        }

        public void Start(TestPublisher publisher)
        {
            _manager.AddWeakEventListener<TestPublisher, TestEventArgs>(publisher, nameof(publisher.TheEvent), OnTheEvent);
        }

        public void StartProperty(TestPublisher publisher)
        {
            _manager.AddWeakEventListener<TestPublisher>(publisher, OnPropertyChanged);
        }

        public void StartCustom(TestPublisher publisher)
        {
            _manager.AddWeakEventListener<TestPublisher, PropertyChangedEventArgs>(publisher, nameof(publisher.PropertyChanged), OnPropertyChanged);
        }

        public void StartTypedCustom(TestPublisher publisher)
        {
            _manager.AddWeakEventListener<TestPublisher, PropertyChangedEventArgs, PropertyChangedEventHandler>(publisher, (t, e) => t.PropertyChanged += e, (t, e) => t.PropertyChanged -= e, OnPropertyChanged);
        }

        public void Stop()
        {
            _manager.RemoveWeakEventListener(Publisher);
        }

        public void Clear()
        {
            _manager.ClearWeakEventListeners();
        }

        private void OnTheEvent(TestPublisher sender, TestEventArgs e)
        {
            Invocations++;
        }

        private void OnPropertyChanged(TestPublisher sender, PropertyChangedEventArgs e)
        {
            Invocations++;
        }
    }
}