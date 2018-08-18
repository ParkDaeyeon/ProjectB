using System;
using System.Collections.Generic;
namespace Ext.Event
{
    public static class ListenerUtility
    {
        public static void Regist<Func>(LinkedList<Func> listeners, Func listener)
        {
            listeners.AddLast(listener);
        }
        public static bool Unregist<Func>(LinkedList<Func> listeners, Func listener)
        {
            var node = listeners.Last;
            while (null != node)
            {
                var value = node.Value;
                node = node.Previous;

                if (object.ReferenceEquals(value, listener))
                {
                    listeners.Remove(node);
                    return true;
                }
            }
            return false;
        }
        public static void Clear<Func>(LinkedList<Func> listeners)
        {
            listeners.Clear();
        }

        public static void Calls(LinkedList<Action> listeners)
        {
            var node = listeners.First;
            while (null != node)
            {
                var value = node.Value;
                node = node.Next;

                if (null != value)
                    value();
            }
        }

        public static void CallsWithArg1<T1>(LinkedList<Action<T1>> listeners, T1 arg1)
        {
            var node = listeners.First;
            while (null != node)
            {
                var value = node.Value;
                node = node.Next;

                if (null != value)
                    value(arg1);
            }
        }

        public static void CallsWithArg2<T1, T2>(LinkedList<Action<T1, T2>> listeners, T1 arg1, T2 arg2)
        {
            var node = listeners.First;
            while (null != node)
            {
                var value = node.Value;
                node = node.Next;

                if (null != value)
                    value(arg1, arg2);
            }
        }

        public static void CallsWithArg3<T1, T2, T3>(LinkedList<Action<T1, T2, T3>> listeners, T1 arg1, T2 arg2, T3 arg3)
        {
            var node = listeners.First;
            while (null != node)
            {
                var value = node.Value;
                node = node.Next;

                if (null != value)
                    value(arg1, arg2, arg3);
            }
        }
    }
}
