using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using BluePrintAssembler.Annotations;

namespace BluePrintAssembler.Utils
{
    public class NotifyObservableCollection<T> : ObservableCollection<T> where T : INotifyPropertyChanged
    {
        public NotifyObservableCollection()
        {
        }

        public NotifyObservableCollection([NotNull] List<T> list) : base(list)
        {
            foreach (var t in list)
                t.PropertyChanged += Handle;
        }

        public NotifyObservableCollection([NotNull] IEnumerable<T> collection) : base(collection)
        {
            foreach (var t in this)
                t.PropertyChanged += Handle;
        }

        private void Handle(object sender, PropertyChangedEventArgs args)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, null));
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (object t in e.NewItems)
                {
                    ((T) t).PropertyChanged += Handle;
                }
            }

            if (e.OldItems != null)
            {
                foreach (object t in e.OldItems)
                {
                    ((T) t).PropertyChanged -= Handle;
                }
            }

            base.OnCollectionChanged(e);
        }
    }
}