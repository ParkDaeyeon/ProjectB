using System;
using System.Collections;
using System.Collections.Generic;
using Ext;
using Ext.Collection.AntiGC;
#if LOG_DEBUG
using Ext.Debugging;
#endif// LOG_DEBUG
namespace MVC
{
    public static class SequentialModel<DataType>
    {
        public static bool IsOpened { private set; get; }


        static string CLASSNAME_ = null;
        public static string CLASSNAME
        {
            get
            {
                if (string.IsNullOrEmpty(SequentialModel<DataType>.CLASSNAME_))
                    SequentialModel<DataType>.CLASSNAME_ = string.Format("SequentialModel<{0}>", typeof(DataType).Name);

                return SequentialModel<DataType>.CLASSNAME_;
            }
        }

        public static void Open(int collectionCount = 1)
        {
            SequentialModel<DataType>.Close();
            SequentialModel<DataType>.IsOpened = true;
#if LOG_DEBUG
            ClosableDebugger.Open(CLASSNAME);
#endif// LOG_DEBUG
            ModelDisposer.Regist(typeof(SequentialModel<DataType>), SequentialModel<DataType>.Close);

            var colls = SequentialModel<DataType>.collections = new Collection[collectionCount];
            for (int n = 0, cnt = colls.Length; n < cnt; ++n)
                colls[n] = new Collection(n);
        }

        public static void Close()
        {
            if (SequentialModel<DataType>.IsOpened)
            {
                SequentialModel<DataType>.IsOpened = false;
#if LOG_DEBUG
                ClosableDebugger.Close(CLASSNAME);
#endif// LOG_DEBUG
                if (!ModelDisposer.IsWork)
                    ModelDisposer.Unregist(typeof(SequentialModel<DataType>));

                var colls = SequentialModel<DataType>.collections;
                SequentialModel<DataType>.collections = null;

                for (int n = 0, cnt = colls.Length; n < cnt; ++n)
                    colls[n].__InternalAccessList().Clear();
            }
        }

        

        public delegate bool Condition(DataType data);
        public static void Setup(IEnumerable<DataType> datas,
                                 int collectionIndex = 0,
                                 Condition cond = null,
                                 Comparison<DataType> sortComparer = null)
        {
            try
            {
                if (!SequentialModel<DataType>.IsOpened)
                    throw new Exception("not opened");

                if (null == datas)
                    throw new NullReferenceException("datas is null");

                var coll = SequentialModel<DataType>.GetCollection(collectionIndex);
                if (null == coll)
                    throw new ArgumentOutOfRangeException("invalid collectionIndex");

                var list = coll.__InternalAccessList();

                var enumerator = datas.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var data = enumerator.Current;

                    if (null == data)
                        throw new NullReferenceException("data(element) is null");

                    if (null == cond || cond(data))
                        list.Add(data);
                }

                if (null != sortComparer)
                    list.Sort(sortComparer);
            }
            catch (Exception e)
            {
                string args = string.Format("{0}, {1}, {2}, {3}",
                                            ModelException.ToStringOrNull(datas),
                                            collectionIndex,
                                            ModelException.ToStringOrNull(cond),
                                            ModelException.ToStringOrNull(sortComparer));
                ModelException me = new ModelException(CLASSNAME, "Setup", args, e);

                if (null != ModelException.ExceptHandler)
                    ModelException.ExceptHandler(me);
                else
                    throw me;
            }
        }




        public class Collection : IEnumerable<DataType>
        {
            List<DataType> list = new List<DataType>();


            int collectionIndex = -1;
            public int CollectionIndex { get { return this.collectionIndex; } }
            public Collection(int collectionIndex) { this.collectionIndex = collectionIndex; }


            public int Count { get { return this.list.Count; } }

            public IEnumerable<DataType> Values { get { return this.list; } }
            public IEnumerator<DataType> GetEnumerator() { return this.list.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() { return this.list.GetEnumerator(); }

            public DataType this[int index]
            {
                get
                {
                    if (-1 < index && index < this.Count)
                        return this.list[index];

                    return default(DataType);
                }
            }

            public List<DataType> __InternalAccessList() { return this.list; }
        }


        static Collection[] collections = null;
        public static int CollectionCount { get { return SequentialModel<DataType>.IsOpened ? SequentialModel<DataType>.collections.Length : 0; } }

        public static Collection GetCollection(int collectionIndex = 0)
        {
            if (-1 < collectionIndex && collectionIndex < SequentialModel<DataType>.CollectionCount)
                return SequentialModel<DataType>.collections[collectionIndex];

            return null;
        }

        public static IEnumerable<Collection> Collections
        {
            get
            {
                if (!SequentialModel<DataType>.IsOpened)
                    yield break;

                var cs = SequentialModel<DataType>.collections;
                for (int n = 0, cnt = cs.Length; n < cnt; ++n)
                    yield return cs[n];
            }
        }
    }
}
