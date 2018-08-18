namespace Ext.Collection.AntiGC
{
    /// <summary>
    /// GC 를 막기 위해 설계한 struct or value 타입용 배열(정적) 컨테이너.
    /// 
    /// </summary>
    /// <typeparam name="T">배열에 적용할 타입</typeparam>
    public class FixedValueArray<T> : FastValueArray<T> where T : struct
    {
        public FixedValueArray(int capacity)
            : base(capacity)
        {
            this.fix = true;
        }

        public FixedValueArray(int capacity, int count)
            : base(capacity, count)
        {
            this.fix = true;
        }
    }
}