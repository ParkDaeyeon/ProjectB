using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using Ext;
using Ext.Unity3D;
using Ext.String;
using Ext.Event;
using Ext.Async;
using Ext.Unity3D.UI;
using Ext.IO;

using Newtonsoft.Json;

using Program.Core;
using Program.View;
using Program.Model.Service.Implement;

namespace Program.Model.Service
{
    public sealed class App : InternalSingleton<App._Singleton>
    {
        App() { }

        public static void Update()
        {
            App.Singleton.Update();
        }

        public static void LateUpdate()
        {
            App.Singleton.LateUpdate();
        }


        public static void RegistSuspendListener(Action<bool> listener)
        {
            App.Singleton.RegistSuspendListener(listener);
        }
        public static bool UnregistSuspendListener(Action<bool> listener)
        {
            return App.Singleton.UnregistSuspendListener(listener);
        }
        public static void ClearSuspendListeners()
        {
            App.Singleton.ClearSuspendListeners();
        }

        public static bool Suspended
        {
            get { return App.Singleton.Suspended; }
        }
        
        public static void Suspend()
        {
            App.Singleton.Suspend();
        }

        public static void Resume()
        {
            App.Singleton.Resume();
        }

        public static void Reset()
        {
            if (App.IsOpened)
                App.Singleton.MarkAsResetting();

            if (AppImpl.IsOpened)
                AppImpl.Reset();

            Restart.Open();
        }

        public static void Quit()
        {
            try
            {
                UnityExtension.Quit();
            }
            catch (Exception e)
            {
                Debug.LogWarning(string.Format("APP:QUIT_EXCEPTION:{0}", e));
            }
        }

        public static void AddExtraCloseEvent(string key, Action call)
        {
            App.Singleton.AddExtraCloseEvent(key, call);
        }
        public static Action GetExtraCloseEvent(string key)
        {
            return App.Singleton.GetExtraCloseEvent(key);
        }
        public static bool RemoveExtraCloseEvent(string key)
        {
            return App.Singleton.RemoveExtraCloseEvent(key);
        }


        public class _Singleton : ISingleton
        {
            void OnStorageLog(string content)
            {
                Debug.Log(content);
            }

            public void OpenSingleton()
            {
                Storage.Logger += this.OnStorageLog;
                Storage.Open();
                Preference.Open();

                Observer.Open();

                Asset.Open();
                Present.Open();

#if LOG_MEMORY
                this.StartLogMemoryInfo();
#endif// LOG_MEMORY
            }

            public void CloseSingleton()
            {
#if LOG_MEMORY
                this.StopLogMemoryInfo();
#endif// LOG_MEMORY

                var newDict = new Dictionary<string, Action>(this.extraCloseEvents);
                this.extraCloseEvents.Clear();
                foreach (var call in newDict.Values)
                {
                    if (null != call)
                        call();
                }

                Present.Close();
                Asset.Close();
                
                Observer.Close();

                Storage.Close();
                Storage.Logger -= this.OnStorageLog;
                Preference.Close();

                Resources.UnloadUnusedAssets();
            }


            bool isResetting = false;
            internal void MarkAsResetting()
            {
                this.isResetting = true;
            }
            internal bool IsResetting
            {
                get { return this.isResetting; }
            }


            Dictionary<string, Action> extraCloseEvents = new Dictionary<string, Action>();
            internal void AddExtraCloseEvent(string key, Action call)
            {
                this.extraCloseEvents[key] = call;
            }
            internal Action GetExtraCloseEvent(string key)
            {
                Action call;
                if (this.extraCloseEvents.TryGetValue(key, out call))
                    return call;

                return null;
            }
            internal bool RemoveExtraCloseEvent(string key)
            {
                return this.extraCloseEvents.Remove(key);
            }


            internal void Update()
            {
                if (this.isResetting)
                    return;

                if (AppImpl.IsOpened)
                    AppImpl.Update();

                Present.UpdatePresent();
            }

            internal void LateUpdate()
            {
                if (this.isResetting)
                    return;

                BaseView.UpdateViews();
                Responsive.UpdateViews();
            }


            List<Action<bool>> suspendListeners = new List<Action<bool>>();
            internal void RegistSuspendListener(Action<bool> listener)
            {
                this.suspendListeners.Add(listener);
            }
            internal bool UnregistSuspendListener(Action<bool> listener)
            {
                return this.suspendListeners.Remove(listener);
            }
            internal void ClearSuspendListeners()
            {
                this.suspendListeners.Clear();
            }

            bool suspended = false;
            internal bool Suspended
            {
                get { return this.suspended; }
            }

            internal void Suspend()
            {
                if (this.isResetting)
                    return;

                this.suspended = true;

                if (AppImpl.IsOpened)
                    AppImpl.Suspend();

                for (int n = 0, cnt = this.suspendListeners.Count; n < cnt; ++n)
                {
                    var listener = this.suspendListeners[n];
                    if (null != listener)
                        listener(true);
                }

                Present.Suspend();
            }

            internal void Resume()
            {
                if (this.isResetting)
                    return;

                this.suspended = false;

                if (AppImpl.IsOpened)
                    AppImpl.Resume();

                for (int n = 0, cnt = this.suspendListeners.Count; n < cnt; ++n)
                {
                    var listener = this.suspendListeners[n];
                    if (null != listener)
                        listener(false);
                }

                Present.Resume();
            }
        }
    }
}
