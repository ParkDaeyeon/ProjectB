//using System;
//using System.Collections;
//using System.Collections.Generic;
//using Ext;
//using Ext.Collection.AntiGC;
//#if LOG_DEBUG
//using Ext.Debugging;
//#endif// LOG_DEBUG
//namespace MVC
//{
//    // NOTE: 에셋을 순차적으로 정렬해서 보여줘야 할 때 사용한다.
//    // SortedModel 은 IndexedModel 과 기능이 겹치지만, SortedModel 의 경우는 ConditionedModel 과 조합으로 써도 된다.
//    // (index 를 Sort 값으로만 참고하고 그 후엔 index 를 아예 안 쓰기 때문이다. 대신 한번 담을 때 Sort(Quick sort) 의 손해가 있다)
//    public static class SortedModel<DataType>
//    {
//        public static bool IsOpened { private set; get; }


//        static string CLASSNAME_ = null;
//        public static string CLASSNAME
//        {
//            get
//            {
//                if (string.IsNullOrEmpty(SortedModel<DataType>.CLASSNAME_))
//                    SortedModel<DataType>.CLASSNAME_ = string.Format("SortedModel<{0}>", typeof(DataType).Name);

//                return SortedModel<DataType>.CLASSNAME_;
//            }
//        }

//        public static void Open(IEnumerable<DataType> datas, Comparison<DataType> comp)
//        {
//            SortedModel<DataType>.Close();
//            SortedModel<DataType>.IsOpened = true;
//#if LOG_DEBUG
//            ClosableDebugger.Open(CLASSNAME);
//#endif// LOG_DEBUG
//            ModelDisposer.Regist(typeof(SortedModel<DataType>), SortedModel<DataType>.Close);

//            var list = SortedModel<DataType>.list = new List<DataType>();
//            try
//            {
//                if (null == datas)
//                    throw new NullReferenceException("datas is null");

//                if (null == comp)
//                    throw new NullReferenceException("comp is null");

//                var enumerator = datas.GetEnumerator();
//                while (enumerator.MoveNext())
//                {
//                    var data = enumerator.Current;

//                    if (null == data)
//                        throw new NullReferenceException("data(element) is null");

//                    list.Add(data);
//                }

//                list.Sort(comp);
//            }
//            catch (Exception e)
//            {
//                string args = string.Format("{0}, {1}", ModelException.ToStringOrNull(datas), ModelException.ToStringOrNull(comp));
//                ModelException me = new ModelException(CLASSNAME, "Open", args, e);

//                if (null != ModelException.ExceptHandler)
//                    ModelException.ExceptHandler(me);
//                else
//                    throw me;
//            }
//        }


//        public static void Close()
//        {
//            if (SortedModel<DataType>.IsOpened)
//            {
//                SortedModel<DataType>.IsOpened = false;
//#if LOG_DEBUG
//                ClosableDebugger.Close(CLASSNAME);
//#endif// LOG_DEBUG
//                if (!ModelDisposer.IsWork)
//                    ModelDisposer.Unregist(typeof(SortedModel<DataType>));

//                SortedModel<DataType>.list = null;
//            }
//        }


//        static List<DataType> list = null;
//        public static int Count { get { return SortedModel<DataType>.IsOpened ? SortedModel<DataType>.list.Count : 0; } }

//        public static IEnumerable<DataType> Values
//        {
//            get
//            {
//                if (SortedModel<DataType>.IsOpened)
//                {
//                    var list = SortedModel<DataType>.list;
//                    for (int n = 0, cnt = list.Count; n < cnt; ++n)
//                        yield return list[n];
//                }
//            }
//        }


//        public static DataType At(int index)
//        {
//            if (-1 < index && index < SortedModel<DataType>.Count)
//                return SortedModel<DataType>.list[index];

//            return default(DataType);
//        }
//    }
//}
