using UnityEngine;

using System;
using System.Text;

using Ext.Event;

using System.Collections.Generic;

namespace Program.Core
{
    public class Observer : InternalSingleton<Observer._Singleton>
    {
        public static string Group
        {
            set { Observer.Singleton.Group = value; }
            get { return Observer.Singleton.Group; }
        }

        public static void Add(string key, Action receiver)
        {
            Observer.Singleton.Add(key, receiver);
        }
        public static void Add<V>(string key, Action<V> receiver)
        {
            Observer.Singleton.Add(key, receiver);
        }

        public static void Remove(string key, Action receiver)
        {
            if (!Observer.IsOpened)
                return;
            Observer.Singleton.Remove(key, receiver);
        }
        public static void Remove<V>(string key, Action<V> receiver)
        {
            if (!Observer.IsOpened)
                return;
            Observer.Singleton.Remove(key, receiver);
        }

        public static void Clear()
        {
            if (!Observer.IsOpened)
                return;
            Observer.Singleton.Clear();
        }

        public static void Notify(string key)
        {
            Observer.Singleton.Notify(key);
        }
        public static void Notify<V>(string key, V value, Action<V> setFunc = null)
        {
            Observer.Singleton.Notify(key, value, setFunc);
        }
        public static bool TryNotify<V>(string key, V value, V oldValue, Action<V> setFunc = null)
        {
            return Observer.Singleton.TryNotify(key, value, oldValue, setFunc);
        }

        public class _Singleton : ISingleton
        {
            public void OpenSingleton()
            {
            }

            public void CloseSingleton()
            {
                BroadcastTunnel<string, object>.RemoveAllByGroup(this.group);
            }

            string group = "superb.project";
            internal string Group
            {
                set
                {
                    BroadcastTunnel<string, object>.ChangeGroup(this.group, value);
                    this.group = value;
                }
                get { return this.group; }
            }


            Dictionary<int, Action<object>> wrappedReceiverMap = new Dictionary<int, Action<object>>();
            internal void Add(string key, Action receiver)
            {
#if LOG_DEBUG
                Debug.Log(string.Format("OBSERVER:{0}, ADD:{1}", key, receiver));
#endif// LOG_DEBUG
                this.DoAdd(key, receiver.GetHashCode(), (arg) => receiver());
            }

            internal void Add<V>(string key, Action<V> receiver)
            {
#if LOG_DEBUG
                Debug.Log(string.Format("OBSERVER:{0}, ADD:{1}", key, receiver));
#endif// LOG_DEBUG
                this.DoAdd(key, receiver.GetHashCode(), (arg) => receiver((V)arg));
            }

            void DoAdd(string key, int hash, Action<object> wrappedReceiver)
            {
                this.wrappedReceiverMap[hash] = wrappedReceiver;
                BroadcastTunnel<string, object>.Add(key, wrappedReceiver, this.group);
            }
            

            internal bool Remove(string key, Action receiver)
            {
#if LOG_DEBUG
                Debug.Log(string.Format("OBSERVER:{0}, REMOVE:{1}", key, receiver));
#endif// LOG_DEBUG

                return this.DoRemove(key, receiver.GetHashCode());
            }

            internal bool Remove<V>(string key, Action<V> receiver)
            {
#if LOG_DEBUG
                Debug.Log(string.Format("OBSERVER:{0}, REMOVE:{1}", key, receiver));
#endif// LOG_DEBUG

                return this.DoRemove(key, receiver.GetHashCode());
            }

            bool DoRemove(string key, int hash)
            {
                Action<object> wrappedReceiver;
                if (this.wrappedReceiverMap.TryGetValue(hash, out wrappedReceiver))
                {
                    this.wrappedReceiverMap.Remove(hash);
                    return BroadcastTunnel<string, object>.Remove(key, wrappedReceiver, this.group);
                }

                return false;
            }


            internal void Clear()
            {
                this.wrappedReceiverMap.Clear();
                BroadcastTunnel<string, object>.RemoveAllByGroup(this.group);
            }

            internal void Notify(string key)
            {
#if LOG_DEBUG
                Debug.Log(string.Format("OBSERVER:{0}", key));
#endif// LOG_DEBUG
                BroadcastTunnel<string, object>.Notify(key, null, this.group);
            }

            internal void Notify<V>(string key, V value, Action<V> setFunc = null)
            {
                if (null != setFunc)
                    setFunc(value);

#if LOG_DEBUG
                if ("ticketcharger.updatedms" != key)// TODO: Ignore List
                    Debug.Log(string.Format("OBSERVER:{0}, VALUE:{1}, SET?:{2}", key, value, (null != setFunc)));
#endif// LOG_DEBUG
                BroadcastTunnel<string, object>.Notify(key, value, this.group);
            }
            
            internal bool TryNotify<V>(string key, V value, V oldValue, Action<V> setFunc = null)
            {
                if (value.Equals(oldValue))
                    return false;

                if (null != setFunc)
                    setFunc(value);

#if LOG_DEBUG
                Debug.Log(string.Format("OBSERVER_TRY_SUCCEED:{0}, VALUE:{1}, SET?:{2}", key, value, (null != setFunc)));
#endif// LOG_DEBUG
                BroadcastTunnel<string, object>.Notify(key, value, this.group);
                return true;
            }
        }
    }
}
