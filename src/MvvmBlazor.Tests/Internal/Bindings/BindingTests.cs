using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using Moq;
using MvvmBlazor.Internal.Bindings;
using MvvmBlazor.Internal.WeakEventListener;
using Shouldly;
using Xunit;

namespace MvvmBlazor.Tests.Internal.Bindings
{
    public class BindingTests
    {
        [Fact]
        public void AddsCollectionEventListener_WhenInitializing()
        {
            const string propertyName = "propertyName";

            var source = new Mock<INotifyPropertyChanged>();
            var collection = new Mock<INotifyCollectionChanged>();
            var wem = new Mock<IWeakEventManager>();
            var propertyInfo = new Mock<PropertyInfo>();
            propertyInfo.Setup(x => x.Name).Returns(propertyName);
            propertyInfo.Setup(x => x.ReflectedType).Returns(typeof(INotifyCollectionChanged));
            propertyInfo.SetupSequence(x => x.GetValue(It.IsAny<INotifyPropertyChanged>(), null))
                .Returns(collection.Object);

            var binding = new Binding(source.Object, propertyInfo.Object, wem.Object);
            binding.Initialize();

            wem.Verify(x =>
                x.AddWeakEventListener(
                    It.IsAny<INotifyCollectionChanged>(),
                    It.IsAny<Action<INotifyCollectionChanged, NotifyCollectionChangedEventArgs>>()));
        }

        [Fact]
        public void AddsCollectionEventListener_WhenPropertyChangesToNotNull()
        {
            const string propertyName = "propertyName";

            var source = new Mock<INotifyPropertyChanged>();
            var collection = new Mock<INotifyCollectionChanged>();
            var wem = new Mock<IWeakEventManager>();
            var propertyInfo = new Mock<PropertyInfo>();
            propertyInfo.Setup(x => x.Name).Returns(propertyName);
            propertyInfo.Setup(x => x.ReflectedType).Returns(typeof(INotifyCollectionChanged));
            wem.Setup(x =>
                    x.AddWeakEventListener(
                        It.IsAny<INotifyPropertyChanged>(),
                        It.IsAny<Action<INotifyPropertyChanged, PropertyChangedEventArgs>>()))
                .Callback<INotifyPropertyChanged, Action<INotifyPropertyChanged, PropertyChangedEventArgs>>(
                    (sender, action) =>
                        sender.PropertyChanged += (s, a) => action((INotifyPropertyChanged) s, a));
            propertyInfo.Setup(x => x.GetValue(It.IsAny<INotifyPropertyChanged>(), null))
                .Returns(collection.Object);

            var binding = new Binding(source.Object, propertyInfo.Object, wem.Object);
            binding.Initialize();

            source.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs(propertyName));
            wem.Verify(x => x.AddWeakEventListener(It.IsAny<INotifyCollectionChanged>(),
                It.IsAny<Action<INotifyCollectionChanged, NotifyCollectionChangedEventArgs>>()));
        }

        [Fact]
        public void Dispose_RemovesCollectionListener()
        {
            const string propertyName = "propertyName";

            var source = new Mock<INotifyPropertyChanged>();
            var collection = new Mock<INotifyCollectionChanged>();
            var wem = new Mock<IWeakEventManager>();
            var propertyInfo = new Mock<PropertyInfo>();
            propertyInfo.Setup(x => x.Name).Returns(propertyName);
            propertyInfo.Setup(x => x.ReflectedType).Returns(typeof(INotifyCollectionChanged));
            wem.Setup(x =>
                    x.AddWeakEventListener(
                        It.IsAny<INotifyPropertyChanged>(),
                        It.IsAny<Action<INotifyPropertyChanged, PropertyChangedEventArgs>>()))
                .Callback<INotifyPropertyChanged, Action<INotifyPropertyChanged, PropertyChangedEventArgs>>(
                    (sender, action) =>
                        sender.PropertyChanged += (s, a) => action((INotifyPropertyChanged) s, a));
            propertyInfo.SetupSequence(x => x.GetValue(It.IsAny<INotifyPropertyChanged>(), null))
                .Returns(collection.Object)
                .Returns((object) null);

            var binding = new Binding(source.Object, propertyInfo.Object, wem.Object);
            binding.Initialize();
            binding.Dispose();

            wem.Verify(x => x.RemoveWeakEventListener(collection.Object));
        }

        [Fact]
        public void Dispose_RemovesEventListener()
        {
            const string propertyName = "propertyName";

            var source = new Mock<INotifyPropertyChanged>();
            var collection = new Mock<INotifyCollectionChanged>();
            var wem = new Mock<IWeakEventManager>();
            var propertyInfo = new Mock<PropertyInfo>();
            propertyInfo.Setup(x => x.Name).Returns(propertyName);
            propertyInfo.Setup(x => x.ReflectedType).Returns(typeof(INotifyCollectionChanged));
            wem.Setup(x =>
                    x.AddWeakEventListener(
                        It.IsAny<INotifyPropertyChanged>(),
                        It.IsAny<Action<INotifyPropertyChanged, PropertyChangedEventArgs>>()))
                .Callback<INotifyPropertyChanged, Action<INotifyPropertyChanged, PropertyChangedEventArgs>>(
                    (sender, action) =>
                        sender.PropertyChanged += (s, a) => action((INotifyPropertyChanged) s, a));
            propertyInfo.Setup(x => x.GetValue(It.IsAny<INotifyPropertyChanged>(), null))
                .Returns(collection.Object);

            var binding = new Binding(source.Object, propertyInfo.Object, wem.Object);
            binding.Initialize();
            binding.Dispose();

            wem.Verify(x => x.RemoveWeakEventListener(source.Object));
        }

        [Fact]
        public void Equals_NotDifferentSourceAndProperty()
        {
            const string propertyName = "propertyName";

            var source1 = new Mock<INotifyPropertyChanged>();
            var source2 = new Mock<INotifyPropertyChanged>();

            var propertyInfo1 = new Mock<PropertyInfo>();
            propertyInfo1.Setup(x => x.Name).Returns(propertyName);
            var wem = new Mock<IWeakEventManager>();
            var propertyInfo2 = new Mock<PropertyInfo>();
            propertyInfo2.Setup(x => x.Name).Returns(propertyName + "Foo");

            var binding1 = new Binding(source1.Object, propertyInfo1.Object, wem.Object);
            var binding2 = new Binding(source2.Object, propertyInfo2.Object, wem.Object);

            binding1.GetHashCode().ShouldNotBe(binding2.GetHashCode());
            binding1.Equals(binding2).ShouldBeFalse();
        }

        [Fact]
        public void Equals_SameSourceAndProperty()
        {
            const string propertyName = "propertyName";

            var source = new Mock<INotifyPropertyChanged>();
            var wem = new Mock<IWeakEventManager>();

            var propertyInfo = new Mock<PropertyInfo>();
            propertyInfo.Setup(x => x.Name).Returns(propertyName);

            var binding1 = new Binding(source.Object, propertyInfo.Object, wem.Object);
            var binding2 = new Binding(source.Object, propertyInfo.Object, wem.Object);

            binding1.GetHashCode().ShouldBe(binding2.GetHashCode());
            binding1.Equals(binding2).ShouldBeTrue();
        }

        [Fact]
        public void IgnoresPropertyChangedEvent_WhenPropertyNameDoesNotMatch()
        {
            const string propertyName = "propertyName";

            var source = new Mock<INotifyPropertyChanged>();
            var collection = new Mock<INotifyCollectionChanged>();
            var wem = new Mock<IWeakEventManager>();
            var propertyInfo = new Mock<PropertyInfo>();
            propertyInfo.Setup(x => x.Name).Returns(propertyName);
            propertyInfo.Setup(x => x.ReflectedType).Returns(typeof(INotifyCollectionChanged));
            wem.Setup(x =>
                    x.AddWeakEventListener(
                        It.IsAny<INotifyPropertyChanged>(),
                        It.IsAny<Action<INotifyPropertyChanged, PropertyChangedEventArgs>>()))
                .Callback<INotifyPropertyChanged, Action<INotifyPropertyChanged, PropertyChangedEventArgs>>(
                    (sender, action) =>
                        sender.PropertyChanged += (s, a) => action((INotifyPropertyChanged) s, a));
            propertyInfo.Setup(x => x.GetValue(It.IsAny<INotifyPropertyChanged>(), null))
                .Returns(collection.Object);

            var bindingValueChangedRaised = false;
            var binding = new Binding(source.Object, propertyInfo.Object, wem.Object);
            binding.Initialize();
            binding.BindingValueChanged += (s, e) => bindingValueChangedRaised = true;

            source.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs("foo"));

            bindingValueChangedRaised.ShouldBeFalse();
        }

        [Fact]
        public void NotRaises_BindingValueChanged_WhenUninitialized()
        {
            const string propertyName = "propertyName";

            var source = new Mock<INotifyPropertyChanged>();
            var wem = new Mock<IWeakEventManager>();
            var propertyInfo = new Mock<PropertyInfo>();
            propertyInfo.Setup(x => x.Name).Returns(propertyName);

            var hasChanged = false;
            var binding = new Binding(source.Object, propertyInfo.Object, wem.Object);
            binding.BindingValueChanged += (sender, args) => { hasChanged = true; };

            source.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs(propertyName));
            hasChanged.ShouldBeFalse();
        }

        [Fact]
        public void Raises_BindingValueChanged_WhenCollectionChanged()
        {
            const string propertyName = "propertyName";

            var source = new Mock<INotifyPropertyChanged>();
            var collection = new Mock<ObservableCollection<object>>();
            var wem = new Mock<IWeakEventManager>();

            var propertyInfo = new Mock<PropertyInfo>();
            propertyInfo.Setup(x => x.Name).Returns(propertyName);
            propertyInfo.Setup(x => x.ReflectedType).Returns(typeof(INotifyCollectionChanged));
            propertyInfo.Setup(x => x.GetValue(It.IsAny<object>(), It.IsAny<object[]>())).Returns(collection.Object);
            wem.Setup(x =>
                    x.AddWeakEventListener(
                        It.IsAny<INotifyCollectionChanged>(),
                        It.IsAny<Action<INotifyCollectionChanged, NotifyCollectionChangedEventArgs>>()))
                .Callback<INotifyCollectionChanged, Action<INotifyCollectionChanged, NotifyCollectionChangedEventArgs>>(
                    (sender, action) =>
                        sender.CollectionChanged += (s, a) => action((INotifyCollectionChanged) s, a));

            var hasChanged = false;
            var binding = new Binding(source.Object, propertyInfo.Object, wem.Object);
            binding.Initialize();
            binding.BindingValueChanged += (sender, args) =>
            {
                sender.ShouldBe(binding);
                args.ShouldBe(EventArgs.Empty);
                hasChanged = true;
            };

            collection.Raise(x => x.CollectionChanged += null,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
                    new List<object> {new object()}));

            hasChanged.ShouldBeTrue();
        }

        [Fact]
        public void Raises_BindingValueChanged_WhenPropertyChanged()
        {
            const string propertyName = "propertyName";

            var source = new Mock<INotifyPropertyChanged>();
            var wem = new Mock<IWeakEventManager>();
            var propertyInfo = new Mock<PropertyInfo>();
            propertyInfo.Setup(x => x.Name).Returns(propertyName);
            wem.Setup(x =>
                    x.AddWeakEventListener(
                        It.IsAny<INotifyPropertyChanged>(),
                        It.IsAny<Action<INotifyPropertyChanged, PropertyChangedEventArgs>>()))
                .Callback<INotifyPropertyChanged, Action<INotifyPropertyChanged, PropertyChangedEventArgs>>(
                    (sender, action) =>
                        sender.PropertyChanged += (s, a) => action((INotifyPropertyChanged) s, a));

            var hasChanged = false;
            var binding = new Binding(source.Object, propertyInfo.Object, wem.Object);
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
        public void RemovesCollectionEventListener_WhenPropertyChangesToNull()
        {
            const string propertyName = "propertyName";

            var source = new Mock<INotifyPropertyChanged>();
            var collection = new Mock<INotifyCollectionChanged>();
            var wem = new Mock<IWeakEventManager>();
            var propertyInfo = new Mock<PropertyInfo>();
            propertyInfo.Setup(x => x.Name).Returns(propertyName);
            propertyInfo.Setup(x => x.ReflectedType).Returns(typeof(INotifyCollectionChanged));
            wem.Setup(x =>
                    x.AddWeakEventListener(
                        It.IsAny<INotifyPropertyChanged>(),
                        It.IsAny<Action<INotifyPropertyChanged, PropertyChangedEventArgs>>()))
                .Callback<INotifyPropertyChanged, Action<INotifyPropertyChanged, PropertyChangedEventArgs>>(
                    (sender, action) =>
                        sender.PropertyChanged += (s, a) => action((INotifyPropertyChanged) s, a));
            propertyInfo.SetupSequence(x => x.GetValue(It.IsAny<INotifyPropertyChanged>(), null))
                .Returns(collection.Object)
                .Returns((object) null);

            var binding = new Binding(source.Object, propertyInfo.Object, wem.Object);
            binding.Initialize();

            source.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs(propertyName));
            wem.Verify(x => x.RemoveWeakEventListener(It.IsAny<INotifyCollectionChanged>()));
        }

        [Fact]
        public void ShouldNotRaiseBindingValueChangedOnCollectionWhenUninitialized()
        {
            const string propertyName = "propertyName";

            var source = new Mock<INotifyPropertyChanged>();
            var collection = new Mock<ObservableCollection<object>>();
            var wem = new Mock<IWeakEventManager>();
            var propertyInfo = new Mock<PropertyInfo>();
            propertyInfo.Setup(x => x.Name).Returns(propertyName);
            propertyInfo.Setup(x => x.ReflectedType).Returns(typeof(INotifyCollectionChanged));
            propertyInfo.Setup(x => x.GetValue(It.IsAny<object>(), It.IsAny<object[]>())).Returns(collection.Object);

            var hasChanged = false;
            var binding = new Binding(source.Object, propertyInfo.Object, wem.Object);
            binding.BindingValueChanged += (_, __) => hasChanged = true;

            collection.Raise(x => x.CollectionChanged += null,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
                    new List<object> {new object()}));

            hasChanged.ShouldBeFalse();
        }

        [Fact]
        public void ShouldValidateParameters()
        {
            Should.Throw<ArgumentNullException>(() =>
                new Binding(null, new Mock<PropertyInfo>().Object, new Mock<IWeakEventManager>().Object));
            Should.Throw<ArgumentNullException>(() =>
                new Binding(new Mock<INotifyPropertyChanged>().Object, null, new Mock<IWeakEventManager>().Object));
            Should.Throw<ArgumentNullException>(() =>
                new Binding(new Mock<INotifyPropertyChanged>().Object, new Mock<PropertyInfo>().Object, null));
        }
    }
}