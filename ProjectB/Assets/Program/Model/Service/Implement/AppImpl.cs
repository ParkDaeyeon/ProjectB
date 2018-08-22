using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Ext;
using Ext.Unity3D;
using Ext.Unity3D.UI;
using Ext.String;
using Ext.Event;
using Ext.IO;
using Ext.Async;
using Ext.Algorithm;

using ReadWriteCsv;
using Newtonsoft.Json;

using Program.Core;
using Program.View;
using Program.View.Pop;
using Program.View.Common;
using Program.Model.Domain.Asset;
using Program.Model.Domain.Asset.Base;
using Program.Model.Domain.Code;
using Program.Presents.Helper;

namespace Program.Model.Service.Implement
{
    public sealed class AppImpl : InternalSingleton<AppImpl._Singleton>
    {
        AppImpl() { }

        public static bool UpdateBgm
        {
            set { AppImpl.Singleton.UpdateBgm = value; }
            get { return AppImpl.Singleton.UpdateBgm; }
        }
        
        public static void Update()
        {
            AppImpl.Singleton.Update();
        }

        public static void Suspend()
        {
            AppImpl.Singleton.Suspend();
        }

        public static void Resume()
        {
            AppImpl.Singleton.Resume();
        }

        public static void Reset()
        {
            Fade.Out(Color.black, 0);
        }

        public static void SetEventSystem(EventSystem eventSystem, StandaloneInputModule inputModule)
        {
            AppImpl.Singleton.SetEventSystem(eventSystem, inputModule);
        }

        public static bool EnableInput
        {
            set { AppImpl.Singleton.EnableInput = value; }
            get { return AppImpl.Singleton.EnableInput; }
        }

        static AudioSource[] audioSources;
        public static AudioSource[] AudioSources
        {
            set { AppImpl.audioSources = value; }
            get { return AppImpl.audioSources; }
        }

        public class _Singleton : ISingleton
        {
            public void OpenSingleton()
            {
                Responsive.ViewportAreaSize = new Vector2(1920, 1080);
                Present.OnCheckBack = () =>
                {
                    if (Wait.IsActive || LoadingIndicator.IsActive)
                        return Present.BackResult.Blocked;

                    if (Popup.IsShownPop)
                    {
                        var popup = Popup.Focused;
                        if (popup)
                        {
                            popup.OnBack();
                            return Present.BackResult.Blocked;
                        }
                    }

                    return Present.BackResult.Ok;
                };

                var gen = AppImpl.Generation;
                ViewAsync.ResetSignal(new CancellableSignal(() => gen != App.Generation));
            }

            public void CloseSingleton()
            {
                ViewAsync.ResetSignal(null);
                ViewString<long>.SetProvider(null);

                BgmManager.Stop(true);
                BgmManager.Reset();
                BgmPlayer.Close();
            }


            bool updateBgm = true;
            internal bool UpdateBgm
            {
                set { this.updateBgm = value; }
                get { return this.updateBgm; }
            }


            internal void Update()
            {
                if (this.updateBgm)
                {
                    BgmPlayer.Update();
                    BgmManager.Update();
                }
            }
            
            internal void Suspend()
            {
                BgmPlayer.PauseAll();

                BaseView.AppSuspend();
            }

            internal void Resume()
            {
                BgmPlayer.ResumeAll();

                BaseView.AppResume();
            }
            
            EventSystem eventSystem;
            StandaloneInputModule inputModule;
            internal void SetEventSystem(EventSystem eventSystem, StandaloneInputModule inputModule)
            {
                this.eventSystem = eventSystem;
                this.inputModule = inputModule;
            }

            internal bool EnableInput
            {
                set
                {
                    if (this.eventSystem)
                        this.eventSystem.enabled = value;
                    if (this.inputModule)
                        this.inputModule.enabled = value;
                }
                get
                {
                    if (this.eventSystem)
                        return this.eventSystem.enabled;

                    return false;
                }
            }
        }
    }
}
