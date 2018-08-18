using System;
using System.Collections.Generic;
using System.Reflection;

namespace Program.Core
{
    public interface ISingleton
    {
        void OpenSingleton();
        void CloseSingleton();
    }
    public abstract class InternalSingleton<T> where T : class, ISingleton, new()
    {
        protected InternalSingleton() {}

        public static bool IsOpened { get { return null != InternalSingleton<T>.instance; } }

        static int generation = 0;
        public static int Generation { get { return InternalSingleton<T>.generation; } }

        public static void Open()
        {
#if LOG_DEBUG
#if UNITY_EDITOR
            UnityEngine.Debug.Log(string.Format("SINGLETON:OPEN:{0}", typeof(T)));
#endif// UNITY_EDITOR
#endif// LOG_DEBUG
            InternalSingleton<T>.Close();
            var instance = new T();
            InternalSingleton<T>.instance = instance;
            InternalSingleton<T>.instance.OpenSingleton();
        }

        public static void Close()
        {
            if (null != InternalSingleton<T>.instance)
            {
#if LOG_DEBUG
#if UNITY_EDITOR
                UnityEngine.Debug.Log(string.Format("SINGLETON:CLOSE:{0}", typeof(T)));
#endif// UNITY_EDITOR
#endif// LOG_DEBUG
                InternalSingleton<T>.instance.CloseSingleton();
                InternalSingleton<T>.instance = null;

                ++InternalSingleton<T>.generation;
            }
        }

        static T instance;
        protected static T Singleton
        {
            get
            {
                var inst = InternalSingleton<T>.instance;
                if (null == inst)
                    throw new Exception(string.Format("SINGLETON:NOT_OPENED:{0}", typeof(T)));

                return inst;
            }
        }
    }


    public abstract class InternalSingletonMT<T> : InternalSingleton<T> where T : class, ISingleton, new()
    {
        protected InternalSingletonMT() { }

        static object sync = new object();
        protected static new T Singleton
        {
            get
            {
                lock (InternalSingletonMT<T>.sync)
                {
                    return InternalSingleton<T>.Singleton;
                }
            }
        }
    }
}
