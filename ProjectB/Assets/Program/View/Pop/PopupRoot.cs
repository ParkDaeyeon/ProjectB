using UnityEngine;
using UnityEngine.UI;
using Ext;
using Ext.Unity3D;
using Ext.Unity3D.UI;
using System;

namespace Program.View.Pop
{
    public class PopupRoot : BaseView
    {
        public static PopupRoot Instance;

        [SerializeField]
        Canvas canvas;
        public Canvas Canvas { get { return this.canvas; } }

        [SerializeField]
        Canvas foremostCanvas;
        public Canvas ForemostCanvas { get { return this.foremostCanvas; } }

        [SerializeField]
        RectTransform foremostCanvasTrans;
        public RectTransform ForemostCanvasTrans
        {
            get
            {
                if (!this.foremostCanvasTrans)
                {
                    if (this.foremostCanvas)
                        this.foremostCanvasTrans = this.foremostCanvas.GetComponent<RectTransform>();
                }

                return this.foremostCanvasTrans;
            }
        }


        protected override void Awake()
        {
            base.Awake();

            PopupRoot.Instance = this;
        }

        protected override void OnDestroy()
        {
            if (PopupRoot.Instance == this)
                PopupRoot.Instance = null;

            base.OnDestroy();
        }


#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();

            this.canvas = this.FindComponent<Canvas>();
            this.foremostCanvas = this.FindComponent<Canvas>("Foremost");
            if (!this.foremostCanvas)
                this.foremostCanvas = this.FindComponent<Canvas>("../../ForemostGroundSystemCanvas/FrontPopupRoot");
        }
#endif // UNITY_EDITOR
    }
}