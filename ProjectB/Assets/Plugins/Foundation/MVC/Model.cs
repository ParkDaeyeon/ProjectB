using System;
using System.Collections;
using System.Collections.Generic;
#if LOG_DEBUG
using Ext.Debugging;
#endif// LOG_DEBUG
namespace MVC
{
    public static class Model<KeyType, DataType>
    {
        public static bool IsOpened { private set; get; }

        public delegate KeyType GetKeyCallback(DataType data);
        public static GetKeyCallback GetKey { private set; get; }

        static string CLASSNAME_ = null;
        public static string CLASSNAME
        {
            get
            {
                if (string.IsNullOrEmpty(Model<KeyType, DataType>.CLASSNAME_))
                    Model<KeyType, DataType>.CLASSNAME_ = string.Format("Model<{0}, {1}>", typeof(KeyType).Name, typeof(DataType).Name);

                return Model<KeyType, DataType>.CLASSNAME_;
            }
        }

        public static void Open(GetKeyCallback getKey, IEnumerable<DataType> datas = null)
        {
            Model<KeyType, DataType>.Close();
            Model<KeyType, DataType>.IsOpened = true;
#if LOG_DEBUG
            ClosableDebugger.Open(CLASSNAME);
#endif// LOG_DEBUG
            ModelDisposer.Regist(typeof(Model<KeyType, DataType>), Model<KeyType, DataType>.Close);

            Model<KeyType, DataType>.GetKey = getKey;

            if (null != datas)
                Model<KeyType, DataType>.AddRange(datas);
        }

        public static void Close()
        {
            if (Model<KeyType, DataType>.IsOpened)
            {
                Model<KeyType, DataType>.IsOpened = false;
#if LOG_DEBUG
                ClosableDebugger.Close(CLASSNAME);
#endif// LOG_DEBUG
                if (!ModelDisposer.IsWork)
                    ModelDisposer.Unregist(typeof(Model<KeyType, DataType>));

                Model<KeyType, DataType>.Clear();
                Model<KeyType, DataType>.UnregistUpdateCallbackAll();

                Model<KeyType, DataType>.GetKey = null;
                Model<KeyType, DataType>.map.Clear();
            }
        }


        static Dictionary<KeyType, DataType> map = new Dictionary<KeyType, DataType>();
        public static Dictionary<KeyType, DataType> Map { get { return Model<KeyType, DataType>.map; } }
        public static int Count { get { return Model<KeyType, DataType>.IsOpened ? Model<KeyType, DataType>.map.Count : 0; } }


        public static void Add(DataType data, bool force = false)
        {
            if (null == data)
                return;

            try
            {
                var key = Model<KeyType, DataType>.GetKey(data);
                if (force)
                    Model<KeyType, DataType>.map[key] = data;
                else
                    Model<KeyType, DataType>.map.Add(key, data);
            }
            catch (Exception e)
            {
                string args = string.Format("{0}", ModelException.ToStringOrNull(data));
                ModelException me = new ModelException(CLASSNAME, "Add", args, e);

                if (null != ModelException.ExceptHandler)
                    ModelException.ExceptHandler(me);
                else
                    throw me;
            }
        }

        public static void AddRange(IEnumerable<DataType> datas)
        {
            if (null == datas)
                return;

            var enumerator = datas.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var data = enumerator.Current;
                Model<KeyType, DataType>.Add(data);
            }
        }

        public static DataType Get(KeyType key)
        {
            if (Model<KeyType, DataType>.IsOpened)
            {
                DataType data;
                if (Model<KeyType, DataType>.map.TryGetValue(key, out data))
                    return data;
            }

            return default(DataType);
        }

        public static void Set(KeyType key, DataType value)
        {
            if (Model<KeyType, DataType>.IsOpened)
                Model<KeyType, DataType>.map[key] = value;
        }


        public static IEnumerable<KeyValuePair<KeyType, DataType>> KeyValues
        {
            get
            {
                if (Model<KeyType, DataType>.IsOpened)
                {
                    var enumerator = Model<KeyType, DataType>.map.GetEnumerator();
                    while (enumerator.MoveNext())
                        yield return enumerator.Current;
                }
            }
        }
        public static IEnumerable<KeyType> Keys
        {
            get
            {
                if (Model<KeyType, DataType>.IsOpened)
                {
                    var enumerator = Model<KeyType, DataType>.map.GetEnumerator();
                    while (enumerator.MoveNext())
                        yield return enumerator.Current.Key;
                }
            }
        }
        public static IEnumerable<DataType> Values
        {
            get
            {
                if (Model<KeyType, DataType>.IsOpened)
                {
                    var enumerator = Model<KeyType, DataType>.map.GetEnumerator();
                    while (enumerator.MoveNext())
                        yield return enumerator.Current.Value;
                }
            }
        }

        public static bool Remove(KeyType key)
        {
            if (Model<KeyType, DataType>.IsOpened)
            {
                try
                {
                    return Model<KeyType, DataType>.map.Remove(key);
                }
                catch (Exception e)
                {
                    string args = string.Format("{0}", ModelException.ToStringOrNull(key));
                    ModelException me = new ModelException(CLASSNAME, "Remove", args, e);
                    if (null != ModelException.ExceptHandler)
                        ModelException.ExceptHandler(me);
                    else
                        throw me;
                }
            }

            return false;
        }

        public static void Clear()
        {
            if (Model<KeyType, DataType>.IsOpened)
                Model<KeyType, DataType>.map.Clear();
        }





        // NOTE: Observer patterns.

        static Dictionary<int, List<Action<int, DataType>>> updateCallbacks = new Dictionary<int, List<Action<int, DataType>>>();
        public static Dictionary<int, List<Action<int, DataType>>> UpdateCallbacks { get { return Model<KeyType, DataType>.updateCallbacks; } }
        public static void Update(int id, DataType data)
        {
            List<Action<int, DataType>> callbacks;
            if (Model<KeyType, DataType>.updateCallbacks.TryGetValue(id, out callbacks))
            {
                try
                {
                    foreach (Action<int, DataType> callback in callbacks)
                    {
                        if (null != callback)
                            callback(id, data);
                    }
                }
                catch (Exception e)
                {
                    string args = string.Format("{0}, {1}", ModelException.ToStringOrNull(id), ModelException.ToStringOrNull(data));
                    ModelException me = new ModelException(CLASSNAME, "Update", args, e);
                    if (null != ModelException.ExceptHandler) ModelException.ExceptHandler(me);
                    else throw me;
                }
            }
        }

        public static void RegistUpdateCallback(int id, Action<int, DataType> callback)
        {
            if (null == callback)
                return;

            try
            {
                List<Action<int, DataType>> callbacks;
                if (!Model<KeyType, DataType>.updateCallbacks.TryGetValue(id, out callbacks))
                {
                    callbacks = new List<Action<int, DataType>>();
                    Model<KeyType, DataType>.updateCallbacks.Add(id, callbacks);
                }

                callbacks.Add(callback);
            }
            catch (Exception e)
            {
                string args = string.Format("{0}, {1}", ModelException.ToStringOrNull(id), ModelException.ToStringOrNull(callback));
                ModelException me = new ModelException(CLASSNAME, "RegistUpdateCallback", args, e);
                if (null != ModelException.ExceptHandler) ModelException.ExceptHandler(me);
                else throw me;
            }
        }

        public static bool UnregistUpdateCallback(int id, Action<int, DataType> callback)
        {
            if (null == callback)
                return false;

            List<Action<int, DataType>> callbacks;
            if (Model<KeyType, DataType>.updateCallbacks.TryGetValue(id, out callbacks))
            {
                try
                {
                    for (int n = 0, count = callbacks.Count; n < count; ++n)
                    {
                        if (callback == callbacks[n])
                        {
                            callbacks.RemoveAt(n);
                            if (0 == callbacks.Count)
                                Model<KeyType, DataType>.updateCallbacks.Remove(id);

                            return true;
                        }
                    }
                }
                catch (Exception e)
                {
                    string args = string.Format("{0}, {1}", ModelException.ToStringOrNull(id), ModelException.ToStringOrNull(callback));
                    ModelException me = new ModelException(CLASSNAME, "UnregistUpdateCallback", args, e);
                    if (null != ModelException.ExceptHandler) ModelException.ExceptHandler(me);
                    else throw me;
                }
            }

            return false;
        }

        public static void UnregistUpdateCallbackAll()
        {
            Model<KeyType, DataType>.updateCallbacks.Clear();
        }

        public static bool UnregistUpdateCallbackAllById(int id)
        {
            return Model<KeyType, DataType>.updateCallbacks.Remove(id);
        }
    }
}
