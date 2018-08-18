using System;
using System.Collections;
using System.Collections.Generic;
namespace Ext.Collection.AntiGC
{
    /// <summary>
    /// GC 를 막기 위해 설계한 모든 타입용 배열 컨테이너.
    /// 
    /// </summary>
    /// <typeparam name="T">배열에 적용할 타입</typeparam>
    public class FastArray<T> : ICloneable, IList<T> where T : class
    {
        T[] pool;
        public T[] Pool { get { return this.pool; } }
        
        int capacity;
        public int Capacity { get { return this.capacity; } }

        int count;
        public int Count { get { return this.count; } }

        protected bool fix = false;
        public bool Fix { get { return this.fix; } }

        protected FastArray() { }

        public FastArray(int capacity)
        {
            if (0 >= capacity)
                capacity = 1;

            this.pool = new T[capacity];
            this.capacity = capacity;
        }

        public FastArray(int capacity, int count)
        {
            if (0 >= capacity)
                capacity = 1;

            this.pool = new T[capacity];
            this.capacity = capacity;

            this.Resize(count);
        }

        public FastArray(int capacity, T[] items)
        {
            if (0 >= capacity)
                capacity = 1;

            this.pool = new T[capacity];
            this.capacity = capacity;

            this.AddRange(items, 0, items.Length);
        }

        // NOTE: unsafe
        public static explicit operator FastArray<T>(T[] items)
        {
            return FastArray<T>.Cast(items);
        }
        
        public static FastArray<T> Cast(T[] items)
        {
            FastArray<T> thiz = new FastArray<T>();
            thiz.pool = items;
            thiz.capacity = thiz.count = items.Length;
            return thiz;
        }


        public T this[int idx]
        {
            set
            {
                this.pool[idx] = value;
            }
            get
            {
                return this.pool[idx];
            }
        }

        public void Resize(int count)
        {
            if (count > this.capacity)
                this.AllocateMore();

            this.count = count;
        }

        public void Reserve(int reserveCount)
        {
            if (this.capacity >= reserveCount)
                return;

            T[] newPool = new T[reserveCount];
            if (0 < this.count)
                Array.Copy(this.pool, newPool, this.count);

            this.pool = newPool;
            this.capacity = reserveCount;
        }

        protected void AllocateMore()
        {
            if (this.fix)
                throw new OverflowException(string.Format("ERROR: FastValueArray.AllocateMore() failed. capacity:{0}, count:{1}", this.capacity, this.count));

            this.Reserve((this.count << 1) > 32 ? (this.count << 1) : 32);
        }

        public void Add(T item)
        {
            if (this.count >= this.capacity)
                this.AllocateMore();

            this.pool[this.count++] = item;
        }

        public void AddAt(T item, int idx)
        {
            if (this.count >= this.capacity)
                this.AllocateMore();

            for (int n = this.count - 1, nLast = (idx - 1); n != nLast; --n)
                this.pool[n + 1] = this.pool[n];

            this.pool[idx] = item;
            ++this.count;
        }
        public void Insert(int index, T item)
        {
            this.AddAt(item, index);
        }

        public void AddRange(T[] items, int start, int count)
        {
            for (int n = start, nMax = start + count; n < nMax; ++n)
                this.Add(items[n]);
        }

        public void AddAtRange(T[] items, int idx, int start, int count)
        {
            for (int n = start, nMax = start + count; n < nMax; ++n, ++idx)
                this.AddAt(items[n], idx);
        }

        public delegate T Creator(int idx);
        public void Fill(int idx, int count, Creator ctor)
        {
            if (this.count < (idx + count))
                this.count = (idx + count);

            for (int n = 0; n < count; ++n, ++idx)
                this.pool[idx] = ctor(idx);
        }

        public bool Remove(T item)
        {
            int idx = this.IndexOf(item);
            if (-1 == idx)
                return false;

            this.RemoveAt(idx);
            return true;
        }

        public void RemoveAt(int idx)
        {
            this.pool[idx] = default(T);

            for (int n = idx + 1, nMax = this.count; n < nMax ; ++n)
                this.pool[n - 1] = this.pool[n];

            --this.count;
        }

        public void RemoveRange(int startIdx, int count)
        {
            if (0 > startIdx || this.count < startIdx)
                throw new ArgumentOutOfRangeException(string.Format("ERROR: FastValueArray.RemoveRange() failed. start idx:{0}, count:{1}", startIdx, this.count));

            if (0 == count || this.count == startIdx)
                return;

            if (0 > count || this.count < startIdx + count)
                count = this.count - startIdx;

            int endIdx = startIdx + count;

            if (this.count > endIdx)
            {
                for (int n = startIdx; n < endIdx; ++n)
                    this.pool[n] = default(T);

                for (int n = endIdx, nMax = this.count; n < nMax; ++n)
                    this.pool[n - count] = this.pool[n];
            }
            else
            {
                for (int n = startIdx, nMax = this.count; n < nMax; ++n)
                    this.pool[n] = default(T);
            }

            this.count -= count;
        }

        public void Clear()
        {
            for (int n = 0, nMax = this.count; n < nMax; ++n)
                this.pool[n] = default(T);

            this.Resize(0);
        }

        public void Purge(int capacity)
        {
            if (0 >= capacity)
                throw new InvalidOperationException(string.Format("ERROR: FastArray.Purge() failed. capacity:{0}", capacity));

            this.pool = new T[capacity];
            this.capacity = capacity;

            this.count = 0;
        }

        public void Purge(int capacity, int count)
        {
            if (0 >= capacity)
                throw new InvalidOperationException(string.Format("ERROR: FastArray.Purge() failed. capacity:{0}", capacity));

            this.pool = new T[capacity];
            this.capacity = capacity;

            this.Resize(count);
        }


#region C# COLLECTION BASIC INTERFACE
        public delegate int SearchComparison<T2>(T x, T2 y);
        public int BinarySearch<T2>(T2 item, SearchComparison<T2> comparison)
        {
            return this.BinarySearch<T2>(0, this.count, item, comparison);
        }
        public int BinarySearch<T2>(int startIdx, int count, T2 item, SearchComparison<T2> comparison)
        {
            if (0 == count)
                return -1;

            int endIdx = startIdx + count;
            int key = startIdx + (int)(count * 0.5);

            int pos = comparison(this.pool[key], item);

            if (0 < pos) // 1
            {
                return this.BinarySearch((key + 1), endIdx - (key + 1), item, comparison);
            }
            else if (0 > pos) // -1
            {
                return this.BinarySearch(startIdx, key - startIdx, item, comparison);
            }
            else // get !
            {
                return key;
            }
        }

        public object Clone()
        {
            return FastArray<T>.Copy(this);
        }

        public bool Contains(T item)
        {
            return -1 != this.IndexOf(item);
        }

        public static FastArray<T> Copy(FastArray<T> src)
        {
            FastArray<T> newArr = new FastArray<T>(src.capacity);
            for (int n = 0, nMax = src.count; n < nMax; ++n)
                newArr.Add(src[n]);

            return newArr;
        }

        public void CopyTo(int sourceIndex, T[] destination, int destinationIndex, int count)
        {
            #region DEBUG
            if (destination.Length < destinationIndex + count)
                throw new ArgumentOutOfRangeException(string.Format("ERROR: FastArray.CopyTo() failed. out of range:dest.Length:{0}, end:{1})", destination.Length, destinationIndex + count));
            #endregion DEBUG

            for (int n = 0; n < count; ++n)
                destination[destinationIndex + n] = this.pool[sourceIndex + n];
        }


        public int IndexOf(T item)
        {
            return this.IndexOf(item, 0, this.count);
        }
        public int IndexOf(T item, int startIdx)
        {
            return this.IndexOf(item, startIdx, this.count - startIdx);
        }
        public int IndexOf(T item, int startIdx, int count)
        {
            int endIdx = startIdx + count;
            for (int n = startIdx; n < endIdx; ++n)
            {
                if (item.Equals(this.pool[n]))
                    return n;
            }

            return -1;
        }

        public int LastIndexOf(T item)
        {
            return this.LastIndexOf(item, 0, this.count);
        }
        public int LastIndexOf(T item, int startIdx)
        {
            return this.LastIndexOf(item, startIdx, this.count - startIdx);
        }
        public int LastIndexOf(T item, int startIdx, int count)
        {
            for (int n = (startIdx + count) - 1, nFirstIdx = (startIdx - 1); n != nFirstIdx; --n)
            {
                if (item.Equals(this.pool[n]))
                    return n;
            }

            return -1;
        }

        public void Reverse()
        {
            this.Reverse(0, this.count);
        }
        public void Reverse(int startIdx, int count)
        {
            int lastIdx = (startIdx + count) - 1;
            int centerIdx = startIdx + (count / 2);
            T temp;
            for (int idx = startIdx, n = 0; idx < centerIdx; ++idx, ++n)
            {
                temp = this.pool[idx];
                this.pool[idx] = this.pool[lastIdx - n];
                this.pool[lastIdx - n] = temp;
            }
        }

        public void Sort(Comparison<T> comparison)
        {
            this.Sort(0, this.count, comparison);
        }
        protected PoolComparer comparer = new PoolComparer();
        public void Sort(int startIdx, int count, Comparison<T> comparison)
        {
            comparer.Comp = comparison;
            Array.Sort(this.pool, startIdx, count, comparer);
        }

        protected class PoolComparer : IComparer<T>
        {
            public Comparison<T> Comp;
            public int Compare(T x, T y)
            {
                return this.Comp(x, y);
            }
        }

        /// <summary>
        /// unsafe
        /// </summary>
        /// <returns>unsafe array reference</returns>
        public T[] ToArray()
        {
            return this.pool;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int n = 0, cnt = this.count; n < cnt; ++n)
                yield return this.pool[n];
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return this.GetEnumerator();
        }

        public bool IsReadOnly { get { return false; } }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (null == array)
                throw new ArgumentNullException("ERROR: FastArray.CopyTo() failed. array is null");

            if (0 > arrayIndex)
                throw new ArgumentOutOfRangeException(string.Format("ERROR: FastArray.CopyTo() failed. arrayIndex is negative:{0}", arrayIndex));

            if (array.Length <= arrayIndex)
                throw new ArgumentException(string.Format("ERROR: FastArray.CopyTo() failed. invalid arrayIndex:{0}, array.Length:{1}", arrayIndex, array.Length));

            int remainArraySize = array.Length - arrayIndex;
            if (this.count > remainArraySize)
                throw new ArgumentException(string.Format("ERROR: FastArray.CopyTo() failed. remain array size:{0}, list.Count:{1}, arrayIndex:{2}, array.Length:{3}", remainArraySize, this.count, arrayIndex, array.Length));

            for (int n = 0; n < remainArraySize; ++n)
                array[arrayIndex + n] = this.pool[n];
        }
#endregion C# COLLECTION BASIC INTERFACE

        public T[] Trim(int start, int count)
        {
            int max = start + count;

            if (this.count < max)
                throw new InvalidOperationException(string.Format("ERROR: FastArray.Trim() failed. list.count:{0}, start:{1}, count:{2}, max:{3}", this.count, start, count, max));

            T[] arr = new T[count];
            for (int n = start; n < max; ++n)
                arr[n] = this.pool[n];

            return arr;
        }
    }
}