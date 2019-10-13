using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace cs_timed_silver
{
    /// <summary>
    /// From here: https://github.com/jonesdwg/ObservableSync (currently not used, it is here just for future inspiration)
    /// Read this many many times and change your implementation of ObservableCollection synchronizaton method to be with loosely coupled objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableCollectionSynchronizer<T>
    {
        private readonly ObservableCollection<T> _destination;
        private readonly List<ISync> _syncs = new List<ISync>();

        public ObservableCollectionSynchronizer(ObservableCollection<T> destination)
        {
            _destination = destination ?? throw new ArgumentNullException(nameof(destination));
        }

        public IDisposable Synchronize(ObservableCollection<T> source)
        {
            return Synchronize(source, i => i);
        }

        public IDisposable Synchronize<TSource>(ObservableCollection<TSource> source, Func<TSource, T> selector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var item = new Sync<TSource>(this, source, selector);
            _syncs.Add(item);
            item.Initialize();
            return item;
        }

        private void Insert(ISync sync, int index, T destination)
        {
            var offset = GetOffsetOrThrow(sync, index);
            _destination.Insert(offset, destination);
            sync.Count++;
        }

        private void RemoveAt(ISync sync, int index)
        {
            var offset = GetOffsetOrThrow(sync, index);
            _destination.RemoveAt(offset);
            sync.Count--;
        }

        private void Move<TSource>(Sync<TSource> sync, int oldIndex, int newIndex)
        {
            var offset = GetOffsetOrThrow(sync);
            _destination.Move(offset + oldIndex, offset + newIndex);
        }

        private void Replace<TSource>(Sync<TSource> sync, int index, T item)
        {
            var offset = GetOffsetOrThrow(sync, index);
            _destination[offset] = item;
        }

        private int GetOffsetOrThrow(ISync sync, int index = 0)
        {
            var offset = index;
            for (var i = 0; i < _syncs.Count; i++)
            {
                if (_syncs[i] != sync)
                {
                    offset += _syncs[i].Count;
                }
                else
                {
                    return offset;
                }
            }
            throw new ArgumentException("Unrecognized Source collection", nameof(sync));
        }

        private interface ISync : IDisposable
        {
            int Count { get; set; }
        }

        private class Sync<TSource> : ISync
        {
            private bool _isDisposed;
            private readonly ObservableCollectionSynchronizer<T> _parent;
            private readonly ObservableCollection<TSource> _source;
            Func<TSource, T> _selector;

            public Sync(ObservableCollectionSynchronizer<T> parent, ObservableCollection<TSource> source, Func<TSource, T> selector)
            {
                _parent = parent;
                _source = source;
                _selector = selector ?? throw new ArgumentNullException(nameof(selector));
            }

            public int Count { get; set; }

            internal void Initialize()
            {
                AddAll();
                _source.CollectionChanged += OnCollectionChangedHandler;
            }

            private void AddAll()
            {
                for (var i = 0; i < _source.Count; i++)
                {
                    _parent.Insert(this, i, _selector(_source[i]));
                }
            }

            private void OnCollectionChangedHandler(object sender, NotifyCollectionChangedEventArgs e)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        {
                            for (var i = 0; i < e.NewItems.Count; i++)
                            {
                                _parent.Insert(this, i + e.NewStartingIndex, _selector((TSource)e.NewItems[i]));
                            }
                            break;
                        }
                    case NotifyCollectionChangedAction.Remove:
                        {
                            for (var i = e.OldItems.Count - 1; i >= 0; i--)
                            {
                                _parent.RemoveAt(this, i + e.OldStartingIndex);
                            }
                            break;
                        }
                    case NotifyCollectionChangedAction.Move:
                        {
                            if (e.OldItems.Count != 1 || e.NewItems.Count != 1)
                                throw new CollectionSynchronizationException("Can only support moving one item at a time");

                            _parent.Move(this, e.OldStartingIndex, e.NewStartingIndex);

                            break;
                        }
                    case NotifyCollectionChangedAction.Replace:
                        {
                            if (e.OldItems.Count != 1 || e.NewItems.Count != 1)
                                throw new CollectionSynchronizationException("Can only support moving one item at a time");

                            _parent.Replace(this, e.NewStartingIndex, _selector((TSource)e.NewItems[0]));
                            break;
                        }
                    case NotifyCollectionChangedAction.Reset:
                        {
                            Clear();
                            AddAll();
                            break;
                        }
                    default: throw new CollectionSynchronizationException($"Unrecognized change action {e.Action}");
                }
            }

            public void Dispose()
            {
                if (_isDisposed)
                    return;

                _isDisposed = true;

                _source.CollectionChanged -= OnCollectionChangedHandler;

                Clear();

                _parent._syncs.Remove(this);
            }

            private void Clear()
            {
                for (var i = Count - 1; i >= 0; i--)
                {
                    _parent.RemoveAt(this, i);
                }
            }
        }
    }

    [Serializable]
    public class CollectionSynchronizationException : Exception
    {
        public CollectionSynchronizationException() { }
        public CollectionSynchronizationException(string message) : base(message) { }
        public CollectionSynchronizationException(string message, Exception inner) : base(message, inner) { }
        protected CollectionSynchronizationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
