using System;
using System.Collections.Generic;
namespace Ext.Event
{
    public static class EventTunnel<K, V>
    {
        static Dictionary<K, Action<V>> map;
        
        public static void Send(K key, V param)
        {
            if (null != EventTunnel<K, V>.map)
            {
                Action<V> receiver;
                if (EventTunnel<K, V>.map.TryGetValue(key, out receiver))
                {
                    receiver(param);
                }
            }
        }
        
        public static void Regist(K key, Action<V> receiver)
        {
            if (null == EventTunnel<K, V>.map)
                EventTunnel<K, V>.map = new Dictionary<K, Action<V>>();

            EventTunnel<K, V>.map.Add(key, receiver);
        }

        public static bool Unregist(K key)
        {
            if (null == EventTunnel<K, V>.map)
                return false;

            bool ret = EventTunnel<K, V>.map.Remove(key);
            if (0 == EventTunnel<K, V>.map.Count)
                EventTunnel<K, V>.map = null;

            return ret;
        }

        public static void UnregistAll(K key, Action<object>[] values)
        {
            if (null == EventTunnel<K, V>.map)
                return;

            EventTunnel<K, V>.map.Clear();
            EventTunnel<K, V>.map = null;
        }
    }
}