using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using MvvmBlazor.Collections.ObjectModel;

namespace MvvmBlazor.Collections
{
    public class ObservableCollection<T> : System.Collections.ObjectModel.ObservableCollection<T>
    {
        protected void InsertItems(int index, IEnumerable<T> items)
        {
            CheckReentrancy();

            var list = items.ToList();
            for (var i = 0; i < list.Count; i++)
            {
                var item = list[i];
                base.InsertItem(index + i, item);
            }

            OnCountPropertyChanged();
            OnIndexerPropertyChanged();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list, index));
        }

        public void AddRange(IEnumerable<T> items)
        {
            InsertItems(Items.Count, items);
        }

        protected void OnCountPropertyChanged()
        {
            OnPropertyChanged(EventArgsCache.CountPropertyChanged);
        }

        protected void OnIndexerPropertyChanged()
        {
            OnPropertyChanged(EventArgsCache.IndexerPropertyChanged);
        }
    }
}