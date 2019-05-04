using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using Moq;
using MvvmBlazor.Bindings;
using MvvmBlazor.Collections;
using NUnit.Framework;
using Shouldly;

namespace MvvmBlazor.Tests.Bindings
{
    public class BindingTests
    {
        [Test]
        public void ShouldValidateParameters()
        {
            Should.Throw<ArgumentNullException>(() => new Binding(null, new Mock<PropertyInfo>().Object));
            Should.Throw<ArgumentNullException>(() => new Binding(new Mock<INotifyPropertyChanged>().Object, null));
        }

        [Test]
        public void ShouldNotRaiseBindingValueChangedWhenUninitialized()
        {
            const string propertyName = "propertyName";

            var source = new Mock<INotifyPropertyChanged>();

            var propertyInfo = new Mock<PropertyInfo>();
            propertyInfo.Setup(x => x.Name).Returns(propertyName);

            var binding = new Binding(source.Object, propertyInfo.Object);
            binding.BindingValueChanged += (sender, args) =>
            {
                sender.ShouldBe(binding);
                args.ShouldBe(EventArgs.Empty);
                Assert.Fail();
            };

            source.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs(propertyName));

            Assert.Pass();
        }

        [Test]
        public void ShouldRaiseBindingValueChangedWhenPropertyChanged()
        {
            const string propertyName = "propertyName";

            var source = new Mock<INotifyPropertyChanged>();

            var propertyInfo = new Mock<PropertyInfo>();
            propertyInfo.Setup(x => x.Name).Returns(propertyName);

            var binding = new Binding(source.Object, propertyInfo.Object);
            binding.Initialize();
            binding.BindingValueChanged += (sender, args) =>
            {
                sender.ShouldBe(binding);
                args.ShouldBe(EventArgs.Empty);
                Assert.Pass();
            };

            source.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs(propertyName));

            Assert.Fail();
        }

        [Test]
        public void ShouldNotRaiseBindingValueChangedOnCollectionWhenUninitialized()
        {
            const string propertyName = "propertyName";

            var source = new Mock<INotifyPropertyChanged>();
            var collection = new Mock<ObservableCollection<object>>();

            var propertyInfo = new Mock<PropertyInfo>();
            propertyInfo.Setup(x => x.Name).Returns(propertyName);
            propertyInfo.Setup(x => x.ReflectedType).Returns(typeof(INotifyCollectionChanged));
            propertyInfo.Setup(x => x.GetValue(It.IsAny<object>(), It.IsAny<object[]>())).Returns(collection.Object);

            var binding = new Binding(source.Object, propertyInfo.Object);
            binding.BindingValueChanged += (sender, args) =>
            {
                Assert.Fail();
            };

            collection.Raise(x => x.CollectionChanged += null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<object>() { new object() }));

            Assert.Pass();
        }

        [Test]
        public void ShouldRaiseBindingValueChangedOnCollectionWhenChanged()
        {
            const string propertyName = "propertyName";

            var source = new Mock<INotifyPropertyChanged>();
            var collection = new Mock<ObservableCollection<object>>();

            var propertyInfo = new Mock<PropertyInfo>();
            propertyInfo.Setup(x => x.Name).Returns(propertyName);
            propertyInfo.Setup(x => x.ReflectedType).Returns(typeof(INotifyCollectionChanged));
            propertyInfo.Setup(x => x.GetValue(It.IsAny<object>(), It.IsAny<object[]>())).Returns(collection.Object);

            var binding = new Binding(source.Object, propertyInfo.Object);
            binding.Initialize();
            binding.BindingValueChanged += (sender, args) =>
            {
                sender.ShouldBe(binding);
                args.ShouldBe(EventArgs.Empty);
                Assert.Pass();
            };

            collection.Raise(x => x.CollectionChanged += null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<object>() { new object() }));

            Assert.Fail();
        }

        [Test]
        public void ShouldEqualSameSourceAndProperty()
        {
            const string propertyName = "propertyName";

            var source = new Mock<INotifyPropertyChanged>();

            var propertyInfo = new Mock<PropertyInfo>();
            propertyInfo.Setup(x => x.Name).Returns(propertyName);

            var binding1 = new Binding(source.Object, propertyInfo.Object);
            var binding2 = new Binding(source.Object, propertyInfo.Object);

            binding1.GetHashCode().ShouldBe(binding2.GetHashCode());
            binding1.Equals(binding2).ShouldBeTrue();
        }

        [Test]
        public void ShouldNotEqualDifferentSourceAndProperty()
        {
            const string propertyName = "propertyName";

            var source1 = new Mock<INotifyPropertyChanged>();
            var source2 = new Mock<INotifyPropertyChanged>();

            var propertyInfo1 = new Mock<PropertyInfo>();
            propertyInfo1.Setup(x => x.Name).Returns(propertyName);

            var propertyInfo2 = new Mock<PropertyInfo>();
            propertyInfo2.Setup(x => x.Name).Returns(propertyName + "Foo");

            var binding1 = new Binding(source1.Object, propertyInfo1.Object);
            var binding2 = new Binding(source2.Object, propertyInfo2.Object);

            binding1.GetHashCode().ShouldNotBe(binding2.GetHashCode());
            binding1.Equals(binding2).ShouldBeFalse();
        }
    }
}
