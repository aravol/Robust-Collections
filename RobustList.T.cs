using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Robust
{
    /// <summary>
    /// Represents a list which will survive multithreading and nested mutation via immutable collections
    /// </summary>
    /// <typeparam name="T">The type of tiems contained in this List</typeparam>
    public class RobustList<T> : IList<T>, IList, IReadOnlyList<T>
    {
        private ImmutableList<T> _underlyingList = ImmutableList<T>.Empty;
        private object _lockKey = new object();

        public int IndexOf(T item)
        {
            return _underlyingList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            lock(_lockKey)
                _underlyingList = _underlyingList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            lock (_underlyingList)
                _underlyingList = _underlyingList.RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                return _underlyingList[index];
            }
            set
            {
                lock(_lockKey)
                {
                    _underlyingList = _underlyingList
                        .Select((o, i) => i == index ? o : value)
                        .ToImmutableList();
                }
            }
        }

        public void Add(T item)
        {
            lock (_underlyingList)
                _underlyingList = _underlyingList.Add(item);
        }

        public void Clear()
        {
            lock (_lockKey) _underlyingList = _underlyingList.Clear();
        }

        public bool Contains(T item)
        {
            return _underlyingList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _underlyingList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _underlyingList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            lock (_lockKey)
                if (_underlyingList.Contains(item))
                {
                    _underlyingList = _underlyingList.Remove(item); return true;
                }
                else return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _underlyingList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_underlyingList).GetEnumerator();
        }

        public int Add(object value)
        {
            lock (_lockKey)
            {
                _underlyingList = _underlyingList.Add((T)value);
                return _underlyingList.Count;
            }
        }

        public bool Contains(object value)
        {
            if (value is T)
            {
                return _underlyingList.Contains((T)value);
            }
            else return false;
        }

        public int IndexOf(object value)
        {
            if (value is T)
                return _underlyingList.IndexOf((T)value);
            else return -1;
        }

        public void Insert(int index, object value)
        {
            lock (_lockKey)
                _underlyingList = _underlyingList.Insert(index, (T)value);
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public void Remove(object value)
        {
            lock (_lockKey)
                _underlyingList = _underlyingList.Remove((T)value);
        }

        object IList.this[int index]
        {
            get { return ((IList)_underlyingList)[index]; }
            set
            {
                lock (_lockKey)
                {
                    _underlyingList = _underlyingList
                        .Select((o, i) => i == index ? o : (T)value)
                        .ToImmutableList();
                }
            }
        }

        public void CopyTo(Array array, int index)
        {
            ((IList)_underlyingList).CopyTo(array, index);
        }

        public bool IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }
    }
}
