using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
namespace Ext.Collection.AntiGC
{
    /// <summary>
    /// GC 를 막기 위해 설계한 class 타입용 리스트형 pool
    /// </summary>
    /// <typeparam name="T">리스트형 pool 에 적용할 타입</typeparam>
    public class CachedPool<T> : ISerializable, IDeserializationCallback
    {
        protected LinkedList<T> Cache;
        protected LinkedList<T> Pool;

        // NOTE: 
        public LinkedList<T> AccessCache
        {
            get
            {
                return this.Cache;
            }
        }
        public LinkedList<T> AccessPool
        {
            get
            {
                return this.Pool;
            }
        }

        protected int _Capacity = 0;
        protected int _FreeCount = 0;
        protected int _Count = 0;

        public int Capacity             { get { return this._Capacity; } }
        public int FreeCount            { get { return this._FreeCount; } }
        public int Count                { get { return this._Count; } }

        public delegate T TAllocator(CachedPool<T> pool);
        public TAllocator Allocator     { set; get; }
        
        public const int DEFAULT_CAPACITY = 16;

        public CachedPool()
        {
            this.Cache = new LinkedList<T>();
            this.Pool = new LinkedList<T>();
        }

        public CachedPool(TAllocator allocator)
        {
            this.Cache = new LinkedList<T>();
            this.Pool = new LinkedList<T>();

            this.Allocator = allocator;
        }

        public CachedPool(int capacity, TAllocator allocator)
        {
            this.Cache = new LinkedList<T>();
            this.Pool = new LinkedList<T>();

            this.Allocator = allocator;

            this.Reserve(capacity);
        }

        public LinkedListNode<T> First  { get { return this.Pool.First; } }
        public LinkedListNode<T> Last   { get { return this.Pool.Last; } }

        public LinkedListNode<T> AllocAfter(LinkedListNode<T> node)
        {
            LinkedListNode<T> newNode = this.PopCache();
            this.Pool.AddAfter(node, newNode);
            ++this._Count;
            return newNode;
        }

        public LinkedListNode<T> AllocBefore(LinkedListNode<T> node)
        {
            LinkedListNode<T> newNode = this.PopCache();
            this.Pool.AddBefore(node, newNode);
            ++this._Count;
            return newNode;
        }

        public LinkedListNode<T> AllocFirst()
        {
            LinkedListNode<T> newNode = this.PopCache();
            this.Pool.AddFirst(newNode);
            ++this._Count;
            return newNode;
        }

        public LinkedListNode<T> AllocLast()
        {
            LinkedListNode<T> newNode = this.PopCache();
            this.Pool.AddLast(newNode);
            ++this._Count;
            return newNode;
        }

        public void Reserve(int reserveCount)
        {
            #region DEBUG
            //if (0 > reserveCount)
            //    throw new System.InvalidOperationException("ERROR: CachedPool.Reserve() failed. reserveCount is = " + reserveCount);

            //if (null == this.Allocator)
            //    throw new System.InvalidOperationException("ERROR: CachedPool.Reserve() failed. the allocator is null");
            #endregion DEBUG

            int count = reserveCount - this._Capacity;
            for (int n = 0; n < count; ++n)
                this.Cache.AddLast(new LinkedListNode<T>(this.Allocator(this)));

            this._Capacity = reserveCount;
            this._FreeCount = this.Cache.Count;
        }

        protected LinkedListNode<T> PopCache()
        {
            LinkedListNode<T> node = this.Cache.Last;
            if (null == node)
            {
                int capacity = this._Capacity * 2;
                this.Reserve(CachedPool<T>.DEFAULT_CAPACITY > capacity ? CachedPool<T>.DEFAULT_CAPACITY : capacity);
                node = this.Cache.Last;
                //if (null == node)
                //    throw new System.Exception("ERROR: CachedPool.PopCache() failed. internal error");
            }

            this.Cache.Remove(node);
            --this._FreeCount;

            return node;
        }

        public LinkedListNode<T> Find(T value)
        {
            return this.Pool.Find(value);
        }
        public LinkedListNode<T> Find(Predicate<T> match)
        {
            if (0 >= this._Count)
                return null;

            LinkedListNode<T> node = this.Pool.First;
            while (null != node)
            {
                if (match(node.Value))
                    return node;

                node = node.Next;
            }
            return null;
        }

        public LinkedListNode<T> FindLast(T value)
        {
            return this.Pool.FindLast(value);
        }
        public LinkedListNode<T> FindLast(Predicate<T> match)
        {
            if (0 >= this._Count)
                return null;

            LinkedListNode<T> node = this.Pool.Last;
            while (null != node)
            {
                if (match(node.Value))
                    return node;

                node = node.Previous;
            }
            return null;
        }

        public void Remove(LinkedListNode<T> node)
        {
            this.Pool.Remove(node);
            this.Cache.AddLast(node);
            --this._Count;
            ++this._FreeCount;
        }

        public bool Remove(T value)
        {
            LinkedListNode<T> node = this.Pool.Find(value);
            if (null == node)
                return false;

            this.Remove(node);
            return true;
        }

        public void RemoveFirst()
        {
            LinkedListNode<T> node = this.Pool.First;
            this.Remove(node);
        }

        public void RemoveLast()
        {
            LinkedListNode<T> node = this.Pool.Last;
            this.Remove(node);
        }

        public void Clear()
        {
            LinkedListNode<T> node = this.Pool.First;
            LinkedListNode<T> nodeTemp;
            while (null != node)
            {
                nodeTemp = node;
                node = node.Next;

                this.Pool.Remove(nodeTemp);
                this.Cache.AddLast(nodeTemp);
            }

            this._Count = 0;
            this._FreeCount = this._Capacity;
        }

        /// <summary>
        /// 데이터를 모두 날림으로써 reference count 등과 같은 민감한 문제를 해결한다.
        /// 그 대신, GC 가 돌아 성능 하락을 초래하므로 특정한 상황이 아니면 사용하지 않는다.
        /// </summary>
        public void Purge()
        {
            this.Pool.Clear();
            this.Cache.Clear();

            this._Count = 0;
            this._FreeCount = 0;
            this._Capacity = 0;
        }


#region C# COLLECTION BASIC INTERFACE
        public bool Contains(T value)
        {
            return null != this.Find(value);
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            this.Pool.GetObjectData(info, context);
        }

        public virtual void OnDeserialization(object sender)
        {
            this.Pool.OnDeserialization(sender);
        }
#endregion C# COLLECTION BASIC INTERFACE
    }
}