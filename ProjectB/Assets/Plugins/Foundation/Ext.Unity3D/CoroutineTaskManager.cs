using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Ext.Unity3D
{
    public sealed class CoroutineTaskManager : MonoBehaviour
    {
        public static Coroutine AddTask(IEnumerator routine)
        {
            try
            {
                if (CoroutineTaskManager.singleton && CoroutineTaskManager.singleton.gameObject.activeSelf)
                    return CoroutineTaskManager.singleton.StartCoroutine(routine);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e.StackTrace);
                Debug.LogError(e);
            }
            return null;
        }

        public static void ClearTasks()
        {
            if (CoroutineTaskManager.singleton)
                CoroutineTaskManager.singleton.StopAllCoroutines();
        }

        public static void RemoveTask(Coroutine co)
        {
            try
            {
                if (CoroutineTaskManager.singleton)
                    CoroutineTaskManager.singleton.StopCoroutine(co);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e.StackTrace);
                Debug.LogError(e);
            }
        }

        void Awake()
        {
            UnityEngine.Object.DontDestroyOnLoad(this);

            //if (null == CoroutineTaskManager.Singleton || this != CoroutineTaskManager.Singleton)
            //        if (null == CoroutineTaskManager.Singleton)
            CoroutineTaskManager.singleton = this;
        }

        void OnDestroy()
        {
            if (this == CoroutineTaskManager.singleton)
                CoroutineTaskManager.singleton = null;
        }

        static CoroutineTaskManager singleton = null;


        public enum STATE
        {
            Sleep = 0,
            Wait,
            Run,
        }
    }
}