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
    // NOTE: index 가 명확한 에셋에 사용한다.
    // (주의: ConditionedModel 과 같이 쓰면 index 가 비는 일이 발생, 로직적으로 문제가 있다. 차라리 SortedModel 을 권장한다)
    public static class IndexedModel<DataType> where DataType : IReadonlyIndexable<int>
    {
        public static bool IsOpened { private set; get; }


        static string CLASSNAME_ = null;
        public static string CLASSNAME
        {
            get
            {
                if (string.IsNullOrEmpty(IndexedModel<DataType>.CLASSNAME_))
                    IndexedModel<DataType>.CLASSNAME_ = string.Format("IndexedModel<{0}>", typeof(DataType).Name);

                return IndexedModel<DataType>.CLASSNAME_;
            }
        }
        public static void Open(IEnumerable<DataType> datas, int count)
        {
            IndexedModel<DataType>.Close();
            IndexedModel<DataType>.IsOpened = true;
#if LOG_DEBUG
            ClosableDebugger.Open(CLASSNAME);
#endif// LOG_DEBUG
            ModelDisposer.Regist(typeof(IndexedModel<DataType>), IndexedModel<DataType>.Close);

            var array = IndexedModel<DataType>.array = new DataType[count];
            try
            {
                if (null == datas)
                    throw new NullReferenceException("datas is null");

                var enumerator = datas.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var data = enumerator.Current;

                    if (null == data)
                        throw new NullReferenceException("data(element) is null");

                    var index = data.Index;

                    if (index < 0 || array.Length <= index)
                        throw new ArgumentException(string.Format("invalid data(element).index:{0}, count:{1}, data(element):{2}", index, count, data));

                    if (null != array[index])
                        throw new ArgumentException(string.Format("duplicated data(element).index:{0}, count:{1}, data(element):{2}, prev_data(element):{3}", index, count, data, array[index]));

                    array[index] = data;
                }
            }
            catch (Exception e)
            {
                string args = string.Format("{0}, {1}", ModelException.ToStringOrNull(datas), count);
                ModelException me = new ModelException(CLASSNAME, "Open", args, e);

                if (null != ModelException.ExceptHandler)
                    ModelException.ExceptHandler(me);
                else
                    throw me;
            }
        }

        public static void Close()
        {
            if (IndexedModel<DataType>.IsOpened)
            {
                IndexedModel<DataType>.IsOpened = false;
#if LOG_DEBUG
                ClosableDebugger.Close(CLASSNAME);
#endif// LOG_DEBUG
                if (!ModelDisposer.IsWork)
                    ModelDisposer.Unregist(typeof(IndexedModel<DataType>));

                IndexedModel<DataType>.array = null;
            }
        }


        static DataType[] array = null;
        public static DataType[] Array { get { return IndexedModel<DataType>.array; } }

        public static int Count { get { return IndexedModel<DataType>.IsOpened ? IndexedModel<DataType>.array.Length : 0; } }


        public static IEnumerable<DataType> Values
        {
            get
            {
                if (IndexedModel<DataType>.IsOpened)
                {
                    var array = IndexedModel<DataType>.array;
                    for (int n = 0, cnt = array.Length; n < cnt; ++n)
                        yield return array[n];
                }
            }
        }


        public static DataType At(int index)
        {
            if (-1 < index && index < IndexedModel<DataType>.Count)
                return IndexedModel<DataType>.array[index];

            return default(DataType);
        }
    }
}
