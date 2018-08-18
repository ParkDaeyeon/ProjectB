namespace Ext
{
    public struct Tuple<T1, T2>
    {
        public T1 _1;
        public T2 _2;
        public Tuple(T1 _1, T2 _2)
        {
            this._1 = _1;
            this._2 = _2;
        }
    }

    public struct Tuple<T1, T2, T3>
    {
        public T1 _1;
        public T2 _2;
        public T3 _3;
        public Tuple(T1 _1, T2 _2, T3 _3)
        {
            this._1 = _1;
            this._2 = _2;
            this._3 = _3;
        }
    }

    public struct Tuple<T1, T2, T3, T4>
    {
        public T1 _1;
        public T2 _2;
        public T3 _3;
        public T4 _4;
        public Tuple(T1 _1, T2 _2, T3 _3, T4 _4)
        {
            this._1 = _1;
            this._2 = _2;
            this._3 = _3;
            this._4 = _4;
        }
    }
}