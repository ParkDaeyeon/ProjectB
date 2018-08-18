using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
namespace Ext.Unity3D.UI
{
    public class ManagedCanvas : ManagedUIComponent
    {
        [SerializeField]
        Canvas canvas;
        public Canvas Canvas
        {
            set
            {
                this.canvas = value;
                this.Setup();
            }
            get { return this.canvas; }
        }


        void Start()
        {
            this.Setup();
        }

        
        public enum Mode
        {
            Default,
            Foremost,
        }
        [SerializeField]
        Mode mode;
        public Mode GetMode()
        {
            return this.mode;
        }

        public void Setup()
        {
            if (!this.canvas)
                return;

            var mgr = ManagedCanvasManager.Instance;
            if (!mgr)
                return;

            if (RenderMode.ScreenSpaceCamera != this.canvas.renderMode)
                this.canvas.renderMode = RenderMode.ScreenSpaceCamera;

            var oldCamera = this.canvas.worldCamera;
            var newCamera = oldCamera;
            switch (this.mode)
            {
            case Mode.Default:
                newCamera = mgr.MainCamera;
                break;

            case Mode.Foremost:
                newCamera = mgr.ForemostCamera;
                break;
            }
            if (oldCamera != newCamera)
                this.canvas.worldCamera = newCamera;
        }


#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();

            if (!this.canvas)
                this.canvas = this.FindComponent<Canvas>();
        }
#endif// UNITY_EDITOR
    }
}
