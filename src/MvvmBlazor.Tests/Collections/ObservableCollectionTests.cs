using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using MvvmBlazor.Collections;
using NUnit.Framework;
using Shouldly;

namespace MvvmBlazor.Tests.Collections
{
    public class ObservableCollectionTests
    {
        [Test]
        public void AddRange_ShouldRaiseEvent()
        {
            var itemsToAdd = new List<object>()
            {
                new object(),
                new object(),
            };

            var collection = new ObservableCollection<object>();
            collection.CollectionChanged += (sender, args) =>
            {
                sender.ShouldBe(collection);
                args.ShouldNotBeNull();
                args.Action.ShouldBe(NotifyCollectionChangedAction.Add);

                for (var i = 0; i < args.NewItems.Count; i++)
                {
                    args.NewItems[i].ShouldBe(itemsToAdd[i]);
                }

                Assert.Pass();
            };
            collection.AddRange(itemsToAdd);

            Assert.Fail();
        }
    }
}
