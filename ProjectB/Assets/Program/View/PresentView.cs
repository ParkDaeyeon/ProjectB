using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Ext.Unity3D;
using Ext.Unity3D.UI;
using Program.View.Common;

namespace Program.View
{
    public abstract class PresentView : BaseView
    {
        public enum BaseEvent
        {
            Hide = 10000,
            Show = 10001,
        }

        [SerializeField]
        Canvas baseCanvas;
        public Canvas BaseCanvas
        {
            get { return this.baseCanvas; }
        }


        [SerializeField]
        Canvas[] canvases;
        public Canvas[] Canvases
        {
            get { return this.canvases; }
        }

        public void ShowCanvases()
        {
            if (null != this.canvases)
            {
                for (int n = 0, cnt = this.canvases.Length; n < cnt; ++n)
                {
                    var canvas = this.canvases[n];
                    if (!canvas)
                        continue;

                    canvas.enabled = true;
                }
            }
        }

        public void HideCanvases()
        {
            if (null != this.canvases)
            {
                for (int n = 0, cnt = this.canvases.Length; n < cnt; ++n)
                {
                    var canvas = this.canvases[n];
                    if (!canvas)
                        continue;

                    canvas.enabled = false;
                }
            }
        }

        bool isDiscarded = false;
        public bool IsDiscarded
        {
            get { return this.IsDiscarded; }
        }
        public void Discard()
        {
            this.isDiscarded = true;
            this.group.alpha = 0;
            this.HideCanvases();
        }

        [SerializeField]
        CanvasGroup group;
        public CanvasGroup Group
        {
            get { return this.group; }
        }
        public float GroupAlpha
        {
            set
            {
                if (this.isDiscarded)
                    return;

                this.group.alpha = value;
            }
            get { return this.group.alpha; }
        }

        bool isStarted = false;
        public bool IsStarted
        {
            get { return this.isStarted; }
        }

        void Start()
        {
            this.isStarted = true;
            this.OnStart();
        }
        protected virtual void OnStart()
        {
        }

#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();

            this.baseCanvas = this.FindComponent<Canvas>("Canvas");
            this.group = this.FindComponent<CanvasGroup>();
            this.canvases = this.FindComponentsInChildren<Canvas>();

            var canvasScaler = this.baseCanvas.FindComponent<CanvasScaler>();
            if (canvasScaler)
            {
                var scaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                var matchMode = CanvasScaler.ScreenMatchMode.Expand;
                var resolution = new Vector2(1280, 720);
                var pixelPerUnit = 100;

                if (scaleMode != canvasScaler.uiScaleMode ||
                    matchMode != canvasScaler.screenMatchMode ||
                    resolution != canvasScaler.referenceResolution ||
                    pixelPerUnit != canvasScaler.referencePixelsPerUnit)
                {
                    UnityEditor.Undo.RecordObject(canvasScaler, "Canvas Scaler");

                    canvasScaler.uiScaleMode = scaleMode;
                    canvasScaler.screenMatchMode = matchMode;
                    canvasScaler.referenceResolution = resolution;
                    canvasScaler.referencePixelsPerUnit = pixelPerUnit;
                }
            }
        }
#endif// UNITY_EDITOR
    }
}