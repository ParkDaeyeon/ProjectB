using UnityEngine;
using System;
using System.Collections.Generic;

namespace Ext
{
    public class RefCounted<T> where T : class
    {
        public RefCounted(T target, Action<T> destroy)
        {
            this.target = target;
            this.destroy = destroy;
        }

        T target;
        public T Target { get { return this.target; } }
        public static explicit operator T(RefCounted<T> thiz)
        {
            return thiz.target;
        }
        public static implicit operator bool(RefCounted<T> thiz)
        {
            return null != thiz.target;
        }


        Action<T> destroy;

        int refCount;
        public int ReferenceCount { get { return this.refCount; } }
        
        public void AddRef()
        {
            ++this.refCount;
        }

        public void ReleaseRef()
        {
            --this.refCount;
            if (0 >= this.refCount)
            {
                this.refCount = 0;
                if (null != this.target && null != this.destroy)
                    this.destroy(this.target);
                this.target = null;
                this.destroy = null;
            }
        }
    }
}
