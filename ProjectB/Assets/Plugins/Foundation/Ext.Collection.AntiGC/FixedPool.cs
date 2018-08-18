using System;
using System.Collections;
using System.Collections.Generic;
namespace Ext.Collection.AntiGC
{
    /// <summary>
    /// GC 를 막기 위해 설계한 class 용 정적 컨테이너.
    /// 
    /// OOP 의 '다형성' 은 메모리 관리 문제로 다음과 같이 제한적인 기능만 제공한다.
    /// -> abstract class 나 interface 를 T 로 설정할 수 있다.
    /// -> 컨테이너 사용전에 실제 할당할 객체의 allocator 를 연동한다.
    /// -> 할당한 객체를 delete 하지 않고 cache 로 재사용하기에 여러 타입 지원 불가, allocator 의 할당 타입은 반드시 1가지로 fix 해야 한다.
    /// </summary>
    /// <typeparam name="T">배열에 적용할 class 타입</typeparam>
    public class FixedPool<T> where T : class//, new()
    {
        protected T[] _Cache;
        protected T[] _Pool;

        protected int _Capacity;
        protected int _AllocCount;
        protected int _FreeCount;
        protected int _Count;

        public int Capacity { get { return this._Capacity; } }
        public int AllocCount { get { return this._AllocCount; } }
        public int FreeCount { get { return this._FreeCount; } }
        public int Count { get { return this._Count; } }




        public delegate T TAllocator(FixedPool<T> pool);
        protected TAllocator _Allocator;
        public TAllocator Allocator { set { this._Allocator = value; } get { return this._Allocator; } }

        protected FixedPool() { }

        public FixedPool(int capacity)
        {
            if (0 >= capacity)
                throw new System.InvalidOperationException(string.Format("ERROR: new FixedPool create failed. invalid param. capacity = {0}, reserveCount = 0", capacity));

            this._Capacity = capacity;
            this._Cache = new T[capacity];
            this._Pool = new T[capacity];
        }
        public FixedPool(int capacity, int reserveCount, TAllocator allocator)
        {
            if (0 >= capacity || 0 > reserveCount)
                throw new System.InvalidOperationException(string.Format("ERROR: new FixedPool create failed. invalid param. capacity = {0}, reserveCount = {1}", capacity, reserveCount));

            if (capacity < reserveCount)
                throw new System.InvalidOperationException(string.Format("ERROR: new FixedPool create failed. capacity < reserveCount. capacity = {0}, reserveCount = {1}", capacity, reserveCount));

            this._Capacity = capacity;
            this._Cache = new T[capacity];
            this._Pool = new T[capacity];
            this._Allocator = allocator;

            this.ReserveAdd(reserveCount);
        }

        public T this[int idx]
        {
            // NOTE: FixedPool 은 Alloc(cache to pool) <---> Free(pool to cache) 로 동작하기에 외부 레퍼런스를 대입하는 기능은 지원하지 않는다.

            //set
            //{
            //    if (0 > idx || this._Count <= idx)
            //        throw new System.ArgumentOutOfRangeException(string.Format("ERROR: FixedPool[idx] failed. idx = {0}, count = {1}", idx, this._Count));

            //    this._Pool[idx] = value;
            //}
            get
            {
                #region DEBUG
                //if (0 > idx || this._Count <= idx)
                //    throw new System.ArgumentOutOfRangeException(string.Format("ERROR: FixedPool[idx] failed. idx = {0}, count = {1}", idx, this._Count));
                #endregion DEBUG

                return this._Pool[idx];
            }
        }


        public T Alloc()
        {
            if (this._Count >= this._Capacity)
                throw new System.OverflowException(string.Format("ERROR: FixedPool.Add() failed. capacity = {0}, count = {1}", this._Capacity, this._Count));

            if (0 == this._FreeCount)
                this.ReserveAdd(1);

            return this._Pool[this._Count++] = this._Cache[--this._FreeCount];
        }

        public T AllocAt(int idx)
        {
            if (0 > idx || this._Count < idx)
                throw new System.ArgumentOutOfRangeException(string.Format("ERROR: FixedPool.AddAt() failed. idx = {0}, count = {1}", idx, this._Count));

            if (this._Count >= this._Capacity)
                throw new System.OverflowException(string.Format("ERROR: FixedPool.AddAt() failed. capacity = {0}, count = {1}", this._Capacity, this._Count));

            if (0 == this._FreeCount)
                this.ReserveAdd(1);

            T item = this._Cache[--this._FreeCount];

            for (int n = this._Count - 1; n != (idx - 1); --n)
                this._Pool[n + 1] = this._Pool[n];

            this._Pool[idx] = item;
            ++this._Count;

            return item;
        }

        /// <summary>
        /// Alloc 성능을 최대한 끌어올리기 위해 추가적인 MemAlloc 을 미리 수행한다.
        /// Elem 개수 예측이 가능한 대부분의 경우 Reserve 사용을 권장함.
        /// </summary>
        /// <param name="capacity">추가 Alloc 할 개체 수</param>
        public void ReserveAdd(int reserveCount)
        {
            if (this._AllocCount + reserveCount > this._Capacity)
                throw new System.OverflowException(string.Format("ERROR: FixedPool.ReserveAdd() failed. capacity = {0}, alloc count = {1}, add count = {2}, alloc count + add count = {3}", this._Capacity, this._AllocCount, reserveCount, this._AllocCount + reserveCount));

            if (null == this._Allocator)
                throw new System.InvalidOperationException("ERROR: FixedPool.ReserveAdd() failed. the allocator is null");

            // regist cache
            for (int n = 0; n < reserveCount; ++n)
                this._Cache[this._FreeCount + n] = this._Allocator(this);

            this._FreeCount += reserveCount;
            this._AllocCount += reserveCount;
        }
        /// <summary>
        /// Add 성능을 최대한 끌어올리기 위해 미리 실 데이터 Alloc 을 수행한다.
        /// Elem 개수 예측이 가능한 대부분의 경우 Reserve 사용을 권장함.
        /// </summary>
        /// <param name="capacity">추가 Alloc 할 개체 수. capacity 를 초과하면 남은 capacity 만큼만</param>
        public void ReserveAddSafe(int reserveCount)
        {
            if (this._AllocCount + reserveCount > this._Capacity)
                reserveCount = this._Capacity - this._AllocCount;

            this.ReserveAdd(reserveCount);
        }

        // NOTE: 쓸대없는 new T[] 를 막기 위해 AllocRange 와 같은 함수는 지원하지 않는다.
        //       (GC 가 좋아하는 heap 사용을 최대한 줄이고 stack 을 쓰기 위함)

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
            if (0 > idx || this._Count <= idx)
                throw new System.ArgumentOutOfRangeException(string.Format("ERROR: FixedPool.RemoveAt() failed. idx = {0}, count = {1}", idx, this._Count));

            this._Cache[this._FreeCount] = this._Pool[idx];
            ++this._FreeCount;

            for (int n = idx + 1; n < this._Count; ++n)
                this._Pool[n - 1] = this._Pool[n];

            --this._Count;
        }

        public void RemoveRange(int startIdx, int count)
        {
            if (0 == count)
                return;

            int endIdx = startIdx + count;

            if (this._Count <= startIdx || this._Count < endIdx)
                throw new System.ArgumentOutOfRangeException(string.Format("ERROR: FixedPool.RemoveRange() failed. start idx = {0}, end idx = {1}, count = {2}", startIdx, endIdx, this._Count));

            for (int n = startIdx; n < count; ++n)
                this._Cache[this._FreeCount + n] = this._Pool[n];
            
            if (this._Count > endIdx)
            {
                for (int n = endIdx; n < this._Count; ++n)
                    this._Pool[n - count] = this._Pool[n];
            }

            this._FreeCount += count;
            this._Count -= count;
        }

        public void Clear()
        {
            for (int n = 0; n < this._Count; ++n)
                this._Cache[this._FreeCount + n] = this._Pool[n];

            this._FreeCount += this._Count;
            this._Count = 0;
        }

        ///// <summary>
        ///// Remove 또는 RemoveAt 이 빈번하게 일어난다면, Reset 을 통해 단체로 갱신 시킬 수 있다.
        ///// 다만 잘못 사용했을때의 책임은 프로그래머 몫이다.
        ///// </summary>
        //public void Reset(T[] used, int startIdx, int count)
        //{
        //    this.Clear();

        //    int endIdx = startIdx + count;
        //    for (int n = startIdx; n < endIdx; ++n)
        //    {
        //        T item = used[n];

        //        for (int n2 = 0; n2 < this._FreeCount; ++n2)
        //        {
        //            if (item == this._Cache[n])
        //            {
        //                this._Pool[this._Count++] = item;

        //                for (int n3 = n2 + 1; n3 < this._FreeCount; ++n3)
        //                    this._Cache[n3 - 1] = this._Cache[n3];

        //                --this._FreeCount;
        //                break;
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// 데이터를 모두 날림으로써 reference count 등과 같은 민감한 문제를 해결한다.
        /// 그 대신, GC 가 돌아 성능 하락을 초래하므로 특정한 상황이 아니면 사용하지 않는다.
        /// </summary>
        public void Purge()
        {
            this.Clear();
            for (int n = 0; n < this._FreeCount; ++n)
                this._Cache[n] = null;

            this._FreeCount = 0;
            this._AllocCount = 0;
        }


#region C# COLLECTION BASIC INTERFACE
        public delegate int SearchComparison<T2>(T x, T2 y);
        public int BinarySearch<T2>(T2 item, SearchComparison<T2> comparison)
        {
            return this.BinarySearch<T2>(0, this._Count, item, comparison);
        }
        public int BinarySearch<T2>(int startIdx, int count, T2 item, SearchComparison<T2> comparison)
        {
            if (0 == count)
                return -1;

            int endIdx = startIdx + count;
            int key = startIdx + (int)(count * 0.5);

            int pos = comparison(this._Pool[key], item);

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

        public bool Contains(T item)
        {
            return -1 != this.IndexOf(item);
        }

        public void ForEach(Action<T> action)
        {
            for (int n = 0; n < this._Count; ++n)
                action(this._Pool[n]);
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < this._Count; ++i)
            {
                yield return this._Pool[i];
            }
        }

        public int IndexOf(T item)
        {
            return this.IndexOf(item, 0, this._Count);
        }
        public int IndexOf(T item, int startIdx)
        {
            if (startIdx > this._Count)
                throw new System.IndexOutOfRangeException(string.Format("ERROR: FixedPool.IndexOf(startIdx) failed. start idx = {0}, count = {1}", startIdx, this._Count));

            return this.IndexOf(item, startIdx, this._Count - startIdx);
        }
        public int IndexOf(T item, int startIdx, int count)
        {
            for (int n = startIdx; n < count; ++n)
                if (Object.ReferenceEquals(this._Pool[n], item))
                    return n;

            return -1;
        }

        public int LastIndexOf(T item)
        {
            return this.LastIndexOf(item, 0, this._Count);
        }
        public int LastIndexOf(T item, int startIdx)
        {
            if (startIdx > this._Count)
                throw new System.IndexOutOfRangeException(string.Format("ERROR: FixedPool.LastIndexOf(startIdx) failed. start idx = {0}, count = {1}", startIdx, this._Count));

            return this.LastIndexOf(item, startIdx, this._Count - startIdx);
        }
        public int LastIndexOf(T item, int startIdx, int count)
        {
            for (int n = (startIdx + count) - 1; n != (startIdx - 1); --n)
                if (Object.ReferenceEquals(this._Pool[n], item))
                    return n;

            return -1;
        }

        public void Reverse()
        {
            this.Reverse(0, this._Count);
        }
        public void Reverse(int startIdx, int count)
        {
            int lastIdx = (startIdx + count) - 1;
            int centerIdx = startIdx + (count / 2);
            T temp;
            for (int idx = startIdx, n = 0; idx < centerIdx; ++idx, ++n)
            {
                temp = this._Pool[idx];
                this._Pool[idx] = this._Pool[lastIdx - n];
                this._Pool[lastIdx - n] = temp;
            }
        }

        public void Sort(Comparison<T> comparison)
        {
            this.Sort(0, this._Count, comparison);
        }
        public void Sort(int startIdx, int count, Comparison<T> comparison)
        {
            PoolComparer comparer;
            comparer.Comp = comparison;
            Array.Sort(this._Pool, startIdx, count, comparer);
        }

        protected struct PoolComparer : IComparer<T>
        {
            public Comparison<T> Comp;
            public int Compare(T x, T y)
            {
                return this.Comp(x, y);
            }
        }

        public T[] ToArray()
        {
            return this._Pool;
        }
#endregion C# COLLECTION BASIC INTERFACE

        public T[] Trim()
        {
            int max = this._Count;

            T[] arr = new T[max];
            for (int n = 0; n < max; ++n)
                arr[n] = this._Pool[n];

            return arr;
        }
    }
}