using System;
using System.Collections.Generic;

namespace Program.Core
{
    public static class ManagedSingletonDisposer
    {
        static Dictionary<Type, Action> autoDisposer = new Dictionary<Type, Action>();
        public static void Add<T>() where T : class, ISingleton, new()
        {
            if (ManagedSingletonDisposer.autoDisposer.ContainsKey(typeof(T)))
                return;

            ManagedSingletonDisposer.autoDisposer.Add(typeof(T), ManagedSingleton<T>.Close);
        }
        public static void Remove<T>() where T : class, ISingleton, new()
        {
            ManagedSingletonDisposer.autoDisposer.Remove(typeof(T));
        }

        public static void Clear()
        {
            ManagedSingletonDisposer.autoDisposer.Clear();
        }

        public static void Run()
        {
            var newMap = new Dictionary<Type, Action>(ManagedSingletonDisposer.autoDisposer);
            ManagedSingletonDisposer.Clear();

            foreach (var pair in newMap)
            {
                var close = pair.Value;
                if (null != close)
                    close();
            }
        }
    }


    public abstract class ManagedSingleton<T> : InternalSingleton<T>
        where T : class, ISingleton, new()
    {
        public new static void Open()
        {
            InternalSingleton<T>.Open();
            ManagedSingletonDisposer.Add<T>();
        }
        public new static void Close()
        {
            InternalSingleton<T>.Close();
            ManagedSingletonDisposer.Remove<T>();
        }
    }


    public abstract class ManagedSingletonMT<T> : InternalSingletonMT<T>
        where T : class, ISingleton, new()
    {
        public new static void Open()
        {
            InternalSingletonMT<T>.Open();
            ManagedSingletonDisposer.Add<T>();
        }
        public new static void Close()
        {
            InternalSingletonMT<T>.Close();
            ManagedSingletonDisposer.Remove<T>();
        }
    }
}
