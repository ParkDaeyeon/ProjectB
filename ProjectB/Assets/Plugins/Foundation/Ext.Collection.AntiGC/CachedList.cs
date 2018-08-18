using System;
using System.Collections.Generic;
namespace Ext.Collection.AntiGC
{
    /// <summary>
    /// GC 를 막기 위해 설계한 모든 타입용 연결리스트
    /// </summary>
    /// <typeparam name="T">연결리스트에 적용할 타입</typeparam>
    public class CachedList<T> : CachedPool<T>, ICloneable
    {
        // CLEAR: GC-Lambda(=>) // save reference
        public CachedList()
            : base((p) => { return default(T); })
        {
        }

        public CachedList(int capacity)
            : base(capacity, (p) => { return default(T); })
        {
        }

        public LinkedListNode<T> AddAfter(LinkedListNode<T> node, T value)
        {
            LinkedListNode<T> newNode = this.AllocAfter(node);
            newNode.Value = value;
            return newNode;
        }

        public LinkedListNode<T> AddBefore(LinkedListNode<T> node, T value)
        {
            LinkedListNode<T> newNode = this.AllocBefore(node);
            newNode.Value = value;
            return newNode;
        }

        public LinkedListNode<T> AddFirst(T value)
        {
            LinkedListNode<T> newNode = this.AllocFirst();
            newNode.Value = value;
            return newNode;
        }

        public LinkedListNode<T> AddLast(T value)
        {
            LinkedListNode<T> newNode = this.AllocLast();
            newNode.Value = value;
            return newNode;
        }


#region C# COLLECTION BASIC INTERFACE
        public object Clone()
        {
            return CachedList<T>.Copy(this);
        }

        public static CachedList<T> Copy(CachedList<T> src)
        {
            CachedList<T> newList = new CachedList<T>(src.Capacity);
            LinkedListNode<T> srcNode = src.First;
            while (null != srcNode)
            {
                newList.AddLast(srcNode.Value);
                srcNode = srcNode.Next;
            }

            return newList;
        }

        public void CopyTo(T[] array, int index)
        {
            int count = this.Count;
            if (array.Length < count)
                throw new System.ArgumentOutOfRangeException(string.Format("ERROR: CachedPool.CopyTo() failed. out of range(dst len = {0}, end idx = {1})", array.Length, index + count));

            LinkedListNode<T> node = this.First;
            for (int n = 0; n < count; ++n)
            {
                array[index + n] = node.Value;
                node = node.Next;
            }
        }
#endregion C# COLLECTION BASIC INTERFACE


        public void Poll(CachedList<T> buffer)
        {
            if (null == buffer)
                return;

            var nodeBuff = buffer.First;
            while (null != nodeBuff)
            {
                this.AddLast(nodeBuff.Value);
                nodeBuff = nodeBuff.Next;
            }
            buffer.Clear();
        }
    }
}