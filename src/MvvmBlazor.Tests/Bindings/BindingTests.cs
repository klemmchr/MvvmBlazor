using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using Moq;
using MvvmBlazor.Bindings;
using MvvmBlazor.Internal.WeakEventListener;
using Shouldly;
using Xunit;

namespace MvvmBlazor.Tests.Bindings
{
    public class BindingTests
    {
        [Fact]
        public void ShouldEqualSameSourceAndProperty()
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
        public void ShouldNotEqualDifferentSourceAndProperty()
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
            binding.BindingValueChanged += (sender, args) => { hasChanged = true; };

            collection.Raise(x => x.CollectionChanged += null,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
                    new List<object> {new object()}));

            hasChanged.ShouldBeFalse();
        }

        [Fact]
        public void ShouldNotRaiseBindingValueChangedWhenUninitialized()
        {
            const string propertyName = "propertyName";

            var source = new Mock<INotifyPropertyChanged>();
            var wem = new Mock<IWeakEventManager>();
            var propertyInfo = new Mock<PropertyInfo>();
            propertyInfo.Setup(x => x.Name).Returns(propertyName);

            var hasChanged = false;
            var binding = new Binding(source.Object, propertyInfo.Object, wem.Object);
            binding.BindingValueChanged += (sender, args) =>
            {
                sender.ShouldBe(binding);
                args.ShouldBe(EventArgs.Empty);

                hasChanged = true;
            };

            source.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs(propertyName));
            hasChanged.ShouldBeFalse();
        }

        [Fact]
        public void ShouldRaiseBindingValueChangedOnCollectionWhenChanged()
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
                        sender.CollectionChanged += (s, a) => action((INotifyCollectionChanged)s, a));

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
        public void ShouldRaiseBindingValueChangedWhenPropertyChanged()
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
                        sender.PropertyChanged += (s, a) => action((INotifyPropertyChanged)s, a));

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
        public void ShouldValidateParameters()
        {
            Should.Throw<ArgumentNullException>(() => new Binding(null, new Mock<PropertyInfo>().Object, new Mock<IWeakEventManager>().Object));
            Should.Throw<ArgumentNullException>(() => new Binding(new Mock<INotifyPropertyChanged>().Object, null, new Mock<IWeakEventManager>().Object));
            Should.Throw<ArgumentNullException>(() => new Binding(new Mock<INotifyPropertyChanged>().Object, new Mock<PropertyInfo>().Object, null));
        }
    }
}