using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
namespace Ext.Unity3D.UI
{
    public class ManagedCanvasManager : ManagedUIComponent
    {
        [SerializeField]
        Camera mainCamera;
        public Camera MainCamera { set { this.mainCamera = value; } get { return this.mainCamera; } }
        public bool EnableMainCamera
        {
            set
            {
                if (this.mainCamera)
                    this.mainCamera.gameObject.SetActive(value);
            }
            get
            {
                return this.mainCamera.gameObject.activeSelf;
            }
        }

        [SerializeField]
        Camera foremostCamera;
        public Camera ForemostCamera { set { this.foremostCamera = value; } get { return this.foremostCamera; } }
        public bool EnableForemostCamera
        {
            set
            {
                if (this.foremostCamera)
                    this.foremostCamera.gameObject.SetActive(value);
            }
            get
            {
                return this.foremostCamera.gameObject.activeSelf;
            }
        }


        static ManagedCanvasManager instance;
        public static ManagedCanvasManager Instance { get { return ManagedCanvasManager.instance; } }
        void Start()
        {
            ManagedCanvasManager.instance = this;
        }
        void OnDestroy()
        {
            if (this == ManagedCanvasManager.instance)
                ManagedCanvasManager.instance = null;
        }

#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();

            if (!this.mainCamera)
			{
                this.mainCamera = this.FindComponent<Camera>();
	            if (!this.mainCamera)
    	            this.mainCamera = this.FindComponent<Camera>("MainCamera");
			}
        }
#endif// UNITY_EDITOR
    }
}
