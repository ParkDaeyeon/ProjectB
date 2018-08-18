using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using UnityEngine;
using UnityEngine.EventSystems;

using Ext.Unity3D;

using ReadWriteCsv;
using Newtonsoft.Json;

using Program.Core;
using Program.Model.Service;
using Program.Model.Service.Implement;
using Program.View;
using Program.View.Common;

namespace Program
{
    public class Startup : ManagedComponent
    {
        static Startup instance;
        public static Startup Instance
        {
            get { return Startup.instance; }
        }

        [SerializeField]
        AudioSource[] audioSources;
        public AudioSource[] AudioSources
        {
            get { return this.audioSources; }
        }

        [SerializeField]
        Camera mainCamera;
        public Camera MainCamera
        {
            get { return this.mainCamera; }
        }

        [SerializeField]
        EventSystem mainEventSystem;
        public EventSystem MainEventSystem
        {
            get { return this.mainEventSystem; }
        }

        [SerializeField]
        StandaloneInputModule mainInputModule;
        public StandaloneInputModule MainInputModule
        {
            get { return this.mainInputModule; }
        }

        [SerializeField]
        GameObject appObject;
        public GameObject AppObject
        {
            get { return this.appObject; }
        }

        void Awake()
        {
            GameObject.DontDestroyOnLoad(this.gameObject);
            GameObject.DontDestroyOnLoad(this.appObject);

            if (!this.enabled)
                return;

            Startup.instance = this;
        }
        void Start()
        {
            this.StartCoroutine(this.OnLoading());
        }

        [SerializeField]
        string presentName = "Entry";
        Type presentType;


        static Type forceStartPresent = null;
        public static Type ForceStartPresent
        {
            set { Startup.forceStartPresent = value; }
            get { return Startup.forceStartPresent; }
        }

        bool loaded = false;
        IEnumerator OnLoading()
        {
            var done = false;
            LoadingIndicator.On(() => done);

            App.Open();
            yield return null;

            AppImpl.AudioSources = this.audioSources;
            yield return null;

            AppImpl.Open();
            yield return null;

            AppImpl.SetEventSystem(this.mainEventSystem, this.mainInputModule);
            yield return null;

            this.presentType = Startup.ForceStartPresent;
            if (null == this.presentType)
            {
                var presentFullname = string.Format("Program.Presents.{0}", this.presentName);
                this.presentType = Type.GetType(presentFullname);
            }
            yield return null;
            this.loaded = true;

            if (null != this.presentType)
                Present.OpenNextPresent(this.presentType, Present.LoadRuleData.Direct, null, () => done = true);
        }




        void OnDestroy()
        {
            if (this != Startup.instance)
                return;

            Startup.instance = null;

            GameObject.Destroy(this.appObject);
            this.appObject = null;
        }


        void Update()
        {
            if (this != Startup.instance)
                return;

            if (!this.loaded)
                return;

            try
            {
                App.Update();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e);
            }
        }
        
        void StartSuspend()
        {
            if (!Present.IsSuspended)
            {
                App.Suspend();
            }
        }

        void StartResume()
        {
            if (Present.IsSuspended)
            {
                App.Resume();
            }
        }



#if UNITY_EDITOR// || UNITY_ANDROID
        void OnApplicationFocus(bool focus)
        {
#if LOG_SUSPEND
            Debug.Log("OnApplicationFocus:" + focus);
#endif// LOG_SUSPEND

            if (!App.IsOpened)
                return;

            if (focus)
                this.StartResume();
            else
                this.StartSuspend();
        }
#endif// UNITY_EDITOR || UNITY_ANDROID

        void OnApplicationPause(bool pause)
        {
            if (!App.IsOpened)
                return;

            if (pause)
                this.StartSuspend();
            else
                this.StartResume();
        }

#if UNITY_EDITOR
        void OnEditorPlayModeChanged()
        {
            this.OnApplicationPause(UnityEditor.EditorApplication.isPaused);
        }
        
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();
            
            // TODO: mainCamera
            this.audioSources = this.mainCamera.FindComponentsInChildren<AudioSource>();
        }
#endif// UNITY_EDITOR
    }
}
