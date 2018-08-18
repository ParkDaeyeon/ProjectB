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
//    // NOTE: 기간제 에셋이나 채널 전용 에셋 등, 데이터를 한번 걸러야 할 때 사용한다.
//    public static class ConditionedModel<DataType>
//    {
//        public static bool IsOpened { private set; get; }


//        static string CLASSNAME_ = null;
//        public static string CLASSNAME
//        {
//            get
//            {
//                if (string.IsNullOrEmpty(ConditionedModel<DataType>.CLASSNAME_))
//                    ConditionedModel<DataType>.CLASSNAME_ = string.Format("ConditionedModel<{0}>", typeof(DataType).Name);

//                return ConditionedModel<DataType>.CLASSNAME_;
//            }
//        }

//        public delegate bool Condition(DataType data);
//        public static void Open(IEnumerable<DataType> datas, Condition cond)
//        {
//            ConditionedModel<DataType>.Close();
//            ConditionedModel<DataType>.IsOpened = true;
//#if LOG_DEBUG
//            ClosableDebugger.Open(CLASSNAME);
//#endif// LOG_DEBUG
//            ModelDisposer.Regist(typeof(ConditionedModel<DataType>), ConditionedModel<DataType>.Close);

//            var list = ConditionedModel<DataType>.list = new List<DataType>();
//            try
//            {
//                if (null == datas)
//                    throw new NullReferenceException("datas is null");

//                if (null == cond)
//                    throw new NullReferenceException("cond is null");

//                var enumerator = datas.GetEnumerator();
//                while (enumerator.MoveNext())
//                {
//                    var data = enumerator.Current;

//                    if (null == data)
//                        throw new NullReferenceException("data(element) is null");

//                    if (cond(data))
//                        list.Add(data);
//                }
//            }
//            catch (Exception e)
//            {
//                string args = string.Format("{0}, {1}", ModelException.ToStringOrNull(datas), ModelException.ToStringOrNull(cond));
//                ModelException me = new ModelException(CLASSNAME, "Open", args, e);

//                if (null != ModelException.ExceptHandler)
//                    ModelException.ExceptHandler(me);
//                else
//                    throw me;
//            }
//        }

//        public static void Close()
//        {
//            if (ConditionedModel<DataType>.IsOpened)
//            {
//                ConditionedModel<DataType>.IsOpened = false;
//#if LOG_DEBUG
//                ClosableDebugger.Close(CLASSNAME);
//#endif// LOG_DEBUG
//                if (!ModelDisposer.IsWork)
//                    ModelDisposer.Unregist(typeof(ConditionedModel<DataType>));

//                ConditionedModel<DataType>.list = null;
//            }
//        }


//        static List<DataType> list = null;
//        public static int Count { get { return ConditionedModel<DataType>.IsOpened ? ConditionedModel<DataType>.list.Count : 0; } }


//        public static IEnumerable<DataType> Values
//        {
//            get
//            {
//                if (ConditionedModel<DataType>.IsOpened)
//                {
//                    var list = ConditionedModel<DataType>.list;
//                    for (int n = 0, cnt = list.Count; n < cnt; ++n)
//                        yield return list[n];
//                }
//            }
//        }


//        public static DataType At(int index)
//        {
//            if (-1 < index && index < ConditionedModel<DataType>.Count)
//                return ConditionedModel<DataType>.list[index];

//            return default(DataType);
//        }
//    }
//}
