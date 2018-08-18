using System;
using System.Collections;
using System.Collections.Generic;
namespace MVC
{
    public static class ModelDisposer
    {
        static Dictionary<Type, Action> disposeMap = new Dictionary<Type, Action>();
        public static Dictionary<Type, Action> DisposeMap { get { return ModelDisposer.disposeMap; } }

        public static bool IsWork { private set; get; }

        public static void CloseAll()
        {
            ModelDisposer.IsWork = true;

            foreach (Action callback in ModelDisposer.disposeMap.Values)
                callback();

            ModelDisposer.disposeMap.Clear();
            ModelDisposer.IsWork = false;
        }

        public static void Regist(Type type, Action callback)
        {
            ModelDisposer.disposeMap.Add(type, callback);
        }

        public static void Unregist(Type type)
        {
            ModelDisposer.disposeMap.Remove(type);
        }
    }
}