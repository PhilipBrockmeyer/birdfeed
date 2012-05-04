using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace BirdFeed
{
    public class CircularQueue<T> : IEnumerable<T>
    {
        public Int32 Capacity { get; private set; }
        private Int32 _currentIndex;
        private Int32 _insertionIndex;
        private readonly CircularQueueEnumerator<T> _enumerator;
        private readonly List<ItemWrapper<T>> _items;
        public CircularQueue(Int32 capacity)
        {

            if (capacity < 1)
                throw new ArgumentException("The capacity must be 1 or greater.", "capacity");
            this._items = new List<ItemWrapper<T>>(capacity);
            this._enumerator = new CircularQueueEnumerator<T>(this);
            this._currentIndex = 0;
            this._insertionIndex = 0;
            this.Capacity = capacity;
        }

        public IEnumerator<T> GetEnumerator()
        {

            return this._enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {

            return this._enumerator;
        }

        public void Add(T item)
        {

            if (this._items.Count == 0)
            {

                this._items.Add(new ItemWrapper<T>(item));
            }

            else
            {

                this._insertionIndex++;
                this._items.Insert(this._insertionIndex, new ItemWrapper<T>(item));
            }

        }

        private void PruneItems()
        {

            if (this._items.Count <= this.Capacity)
                return;
            while (this._items.Count > this.Capacity)
            {

                var oldestItem = this._items.OrderBy(x => x.DateTime).First();
                this._items.Remove(oldestItem);
            }

        }

        private bool MoveNext()
        {

            if (this._items.Count == 0)
                return false;
            var currentItem = this._items[_currentIndex];
            this.PruneItems();
            var currentIndexAfterPrune = this._items.IndexOf(currentItem);
            this._currentIndex = (currentIndexAfterPrune + 1) % Math.Min(this.Capacity, this._items.Count);
            this._insertionIndex = this._currentIndex;
            return true;
        }



        private void Reset()
        {

            this._currentIndex = 0;
        }

        private T GetCurrent()
        {

            if (this._items.Count == 0)
                return default(T);
            return this._items[this._currentIndex].Item;
        }

        private class ItemWrapper<T>
        {

            public DateTime DateTime { get; private set; }
            public T Item { get; private set; }
            public ItemWrapper(T item)
            {

                this.Item = item;
                this.DateTime = DateTime.Now;
            }
        }

        private class CircularQueueEnumerator<T> : IEnumerator<T>
        {
            private readonly CircularQueue<T> _parent;
            public CircularQueueEnumerator(CircularQueue<T> parent)
            {

                this._parent = parent;
            }

            public T Current
            {

                get { return this._parent.GetCurrent(); }
            }

            public void Dispose() { }
            object IEnumerator.Current
            {

                get { return this._parent.GetCurrent(); }
            }

            public bool MoveNext()
            {

                return this._parent.MoveNext();
            }

            public void Reset()
            {

                this._parent.Reset();
            }
        }
    }
}

