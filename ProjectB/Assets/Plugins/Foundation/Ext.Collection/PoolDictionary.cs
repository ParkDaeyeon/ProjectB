using System;
using System.Collections;
using System.Collections.Generic;
namespace Ext.Collection
{
    // TODO: background(ram cache) 가 linked-list 이므로, 오브젝트가 1000 개를 넘기면 자료구조를 다시 생각 할 필요가 있다.
    public class PoolDictionary<TKey, TValue> : IEnumerable
    {
        public class RefData
        {
            int refCount;
            public int RefCount
            {
                get { return this.refCount; }
            }

            TKey key;
            public TKey Key
            {
                get { return this.key; }
            }

            TValue value;
            public TValue Value
            {
                get { return this.value; }
            }

            int weight;
            public int Weight
            {
                get { return this.weight; }
            }
            internal void SetWeight(int value) { this.weight = value; }

            int gen;
            public int Generation
            {
                get { return this.gen; }
            }

            LinkedListNode<RefData> bgNode;
            public LinkedListNode<RefData> BackgroundNode
            {
                get { return this.bgNode; }
            }
            internal void SetBackgroundNode(LinkedListNode<RefData> value) { this.bgNode = value; }

            public RefData(TKey key, TValue value, int weight)
            {
                this.refCount = 0;
                this.key = key;
                this.value = value;
                this.weight = weight;

                this.bgNode = null;
            }

            public void AddRef()
            {
                ++this.refCount;
            }

            public bool ReleaseRef()
            {
                if (0 < this.refCount)
                    --this.refCount;

                return 0 == this.refCount;
            }

            public void ResetGeneration()
            {
                if (this.gen < int.MaxValue)
                    ++this.gen;
                else
                    this.gen = 0;
            }

            public void Discard()
            {
                this.refCount = 0;
            }
        }

        Dictionary<TKey, RefData> foreground;
        LinkedList<RefData> background;


        int capacity;
        public int Capacity
        {
            get { return this.capacity; }
        }

        public int Count { get { return this.foreground.Count + this.background.Count; } }
        public int CommitCount { get { return this.foreground.Count; } }
        public int UncommitCount { get { return this.background.Count; } }

        int weightCapacity;
        public int WeightCapacity
        {
            get { return this.weightCapacity; }
        }

        int weight;
        public int Weight
        {
            get { return this.weight; }
        }

        public enum Flag
        {
            Null,
            Committed,
            Uncommitted,
        }

        public bool Contains(TKey key)
        {
            RefData refData;
            return Flag.Null != this.TryGetValue(key, out refData);
        }

        public bool IsCommitted(TKey key)
        {
            RefData refData;
            return Flag.Committed == this.TryGetValue(key, out refData);
        }

        public bool IsUncommitted(TKey key)
        {
            RefData refData;
            return Flag.Uncommitted == this.TryGetValue(key, out refData);
        }

        public Flag QueryState(TKey key)
        {
            RefData refData;
            return this.TryGetValue(key, out refData);
        }


        protected Flag TryGetValue(TKey key, out RefData refData)
        {
            if (this.foreground.TryGetValue(key, out refData))
                return Flag.Committed;

            LinkedListNode<RefData> node = this.background.First;
            while (null != node)
            {
                refData = node.Value;
                if (refData.Key.Equals(key))
                {
                    refData.SetBackgroundNode(node);
                    return Flag.Uncommitted;
                }
                node = node.Next;
            }

            refData = null;
            return Flag.Null;
        }


        public RefData Add(TKey key, TValue value, int weight, bool commit = true)
        {
            this.CheckWeight(weight, null);

            if (0 > weight)
                weight = 0;

            var refData = new RefData(key, value, weight);

            if (commit)
            {
                refData.AddRef();
                refData.ResetGeneration();
                this.foreground.Add(key, refData);
            }
            else
                refData.SetBackgroundNode(this.background.AddFirst(refData));

            this.weight += weight;
            return refData;
        }

        protected void CheckWeight(int weight, RefData modify)
        {
            if (this.Count >= this.capacity || (0 < this.weightCapacity && 0 < weight && this.weight + weight >= this.weightCapacity))
            {
                if (0 == this.background.Count)
                {
                    if (null == modify && 0 != this.foreground.Count)
                    {
                        throw new Exception("PoolDictionary Error: overflow");
                    }
                    else
                    {
                        return;
                    }
                }

                LinkedListNode<RefData> node = this.background.Last;
                LinkedListNode<RefData> temp;
                while (this.Count >= this.capacity || (0 < this.weightCapacity && 0 < weight && this.weight + weight >= this.weightCapacity))
                {
                    if (null == node)
                        return;

                    if (null != modify && modify.BackgroundNode == node)
                    {
                        node = node.Previous;
                        continue;
                    }

                    this.weight -= node.Value.Weight;
                    if (0 > this.weight) // NOTE: 계산 미스인데 걍 넘어가!!
                        this.weight = 0;

                    temp = node;
                    node = node.Previous;

                    this.background.Remove(temp);
                    temp.Value.SetBackgroundNode(null);
                }
            }
        }

        public bool ModifyWeight(TKey key, int weight)
        {
            if (0 > weight)
                weight = 0;

            RefData refData;
            Flag flag = this.TryGetValue(key, out refData);
            if (Flag.Null == flag)
                return false;

            this.weight -= refData.Weight;
            if (0 > this.weight) // NOTE: 계산 미스인데 걍 넘어가!!
                this.weight = 0;

            if (0 < weight)
                this.CheckWeight(weight, refData);

            refData.SetWeight(weight);
            this.weight += weight;
            return true;
        }

        public RefData Commit(TKey key)
        {
            RefData refData;
            Flag flag = this.TryGetValue(key, out refData);
            if (Flag.Null == flag)
                return null;

            refData.AddRef();
            if (Flag.Uncommitted == flag)
            {
                refData.ResetGeneration();
                this.background.Remove(refData.BackgroundNode);
                this.foreground.Add(refData.Key, refData);
                refData.SetBackgroundNode(null);
            }
            return refData;
        }

        public bool Decommit(TKey key, bool keep, bool force, int gen = -1)
        {
            RefData refData;
            Flag flag = this.TryGetValue(key, out refData);
            if (Flag.Committed != flag)
                return false;

            if (-1 < gen)
            {
                if (refData.Generation != gen)
                    return false;
            }

            if (!force && !refData.ReleaseRef())
                return false;

            refData.Discard();
            if (keep)
                refData.SetBackgroundNode(this.background.AddFirst(refData));
            else
                this.DecreaseWeight(refData);

            this.foreground.Remove(key);
            return true;
        }
        
        public void DecommitForceAll(bool keep)
        {
            foreach (RefData refData in this.foreground.Values)
            {
                refData.Discard();
                if (keep)
                    refData.SetBackgroundNode(this.background.AddFirst(refData));
                else
                    this.DecreaseWeight(refData);
            }

            this.foreground.Clear();
        }


        public bool Remove(TKey key)
        {
            RefData refData = null;
            switch (this.TryGetValue(key, out refData))
            {
            default:
            case Flag.Null:
                return false;

            case Flag.Committed:
                refData.Discard();
                this.foreground.Remove(key);
                break;

            case Flag.Uncommitted:
                this.background.Remove(refData.BackgroundNode);
                refData.SetBackgroundNode(null);
                break;
            }
            this.DecreaseWeight(refData);
            return true;
        }

        void DecreaseWeight(RefData refData)
        {
            this.weight -= refData.Weight;
            if (0 > this.weight) // NOTE: 계산 미스인데 걍 넘어가!!
                this.weight = 0;
        }

        public void RemoveUnusedAll()
        {
            LinkedListNode<RefData> node = this.background.First;
            while (null != node)
            {
                node.Value.SetBackgroundNode(null);

                this.weight -= node.Value.Weight;
                if (0 > this.weight) // NOTE: 계산 미스인데 걍 넘어가!!
                    this.weight = 0;

                node = node.Next;
            }

            this.background.Clear();
        }

        public void Purge()
        {
            if (null != this.foreground)
            {
                if (0 < this.foreground.Count)
                {
                    foreach (RefData refData in this.foreground.Values)
                        refData.Discard();
                }

                this.foreground.Clear();
            }

            if (null != this.background)
            {
                LinkedListNode<RefData> node = this.background.First;
                while (null != node)
                {
                    node.Value.SetBackgroundNode(null);
                    node = node.Next;
                }

                this.background.Clear();
            }

            this.weight = 0;
        }

        public int GetReferenceCount(TKey key)
        {
            RefData refData;
            Flag flag = this.TryGetValue(key, out refData);
            if (Flag.Committed == flag || Flag.Uncommitted == flag)
            {
                return refData.RefCount;
            }

            return -1;
        }



        public PoolDictionary(int capacity, int weightCapacity)
        {
            this.Initialize(capacity, weightCapacity);
        }
        public void Initialize(int capacity, int weightCapacity)
        {
            this.Purge();

            this.foreground = new Dictionary<TKey, RefData>(capacity);
            this.background = new LinkedList<RefData>();
            this.capacity = capacity;
            this.weightCapacity = weightCapacity;
            this.weight = 0;
        }

        // TODO: dynamic change weight capacity

        public IEnumerator GetEnumerator()
        {
            foreach (RefData refData in this.foreground.Values)
                yield return refData;

            LinkedListNode<RefData> node = this.background.First;
            while (null != node)
            {
                yield return node.Value;
                node = node.Next;
            }
        }

        struct GarbageData
        {
            public TKey Key;
            public int Generation;

            public GarbageData(TKey key, int gen)
            {
                this.Key = key;
                this.Generation = gen;
            }
        }
        object garbageSync = new object();
        LinkedList<GarbageData> garbageList = new LinkedList<GarbageData>();

        internal void AddGarbage(TKey key, int foregroundGeneration)
        {
            lock (this.garbageSync)
                this.garbageList.AddLast(new GarbageData(key, foregroundGeneration));
        }

        public void ClearGarbages()
        {
            if (0 == this.garbageList.Count)
                return;

            lock (this.garbageSync)
            {
                var node = this.garbageList.First;
                while (null != node)
                {
                    GarbageData gd = node.Value;
                    this.Decommit(gd.Key, true, false, gd.Generation);
                    node = node.Next;
                }

                this.garbageList.Clear();
            }
        }
    }








    
    public class PoolDictionaryAutoObj<TKey, TValue> : IDisposable
    {
        PoolDictionary<TKey, TValue> dict;
        public PoolDictionary<TKey, TValue> Dictionary
        {
            get { return this.dict; }
        }
        
        PoolDictionary<TKey, TValue>.RefData refData;
        public virtual bool Valid { get { return !this.disposed && 0 < this.refData.RefCount; } }
        public int RefCount { get { return this.refData.RefCount; } }
        public TKey Key { get { return this.refData.Key; } }
        public TValue Value { get { return this.refData.Value; } }
        public int Generation { get { return this.refData.Generation; } }
        public virtual bool CanRecycle { get { return true; } }

        public PoolDictionaryAutoObj(TKey key,
                                     TValue value,
                                     int weight,
                                     PoolDictionary<TKey, TValue> dict = null)
        {
            if (null != dict)
            {
                this.refData = dict.Add(key, value, weight);
                this.dict = dict;
            }
            else
            {
                this.refData = new PoolDictionary<TKey, TValue>.RefData(key, value, weight);
                this.refData.AddRef();
            }
        }

        public PoolDictionaryAutoObj(PoolDictionary<TKey, TValue>.RefData refData, PoolDictionary<TKey, TValue> dict)
        {
            this.refData = refData;
            this.dict = dict;
        }

        ~PoolDictionaryAutoObj()
        {
            if (this.disposed)
                return;

            if (null != this.dict)
                this.dict.AddGarbage(this.Key, this.Generation);
        }

        bool disposed;
        public void Dispose()
        {
            if (this.disposed)
                return;

            this.disposed = true;

            if (null != this.refData)
            {
                if (null != this.dict)
                {
                    var recycle = this.CanRecycle;
                    if (recycle)
                        this.dict.Decommit(this.Key, true, false);
                    else
                        this.dict.Remove(this.Key);
                }
                else
                    this.refData.Discard(); // NOTE: open with null dict
            }
            this.dict = null;
        }

        public virtual bool ModifyWeight(int weight)
        {
            if (null != this.dict)
                return this.dict.ModifyWeight(this.Key, weight);

            return false;
        }
    }
}