using System;
using System.Collections.Generic;
namespace Ext.Collection.AntiGC
{
    /// <summary>
    /// GC 를 막기 위해 설계한 class 타입용 배열(정적) 컨테이너.
    /// 
    /// </summary>
    /// <typeparam name="T">배열에 적용할 타입</typeparam>
    public class FixedArray<T> : FastArray<T> where T : class
    {
        public FixedArray(int capacity)
            : base(capacity)
        {
            this.fix = true;
        }

        public FixedArray(int capacity, int count)
            : base(capacity, count)
        {
            this.fix = true;
        }
    }
}