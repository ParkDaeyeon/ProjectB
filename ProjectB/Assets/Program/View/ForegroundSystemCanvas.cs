using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Ext;
using Ext.Unity3D;
using Ext.Unity3D.UI;
using Program.View.Common;

namespace Program.View
{
    public class ForegroundSystemCanvas : ForceAwakeView
    {
        static ForegroundSystemCanvas instance;
        public static ForegroundSystemCanvas Instance
        {
            get { return ForegroundSystemCanvas.instance; }
        }

        [SerializeField]
        protected Canvas canvas;
        public Canvas Canvas
        {
            get { return this.canvas; }
        }

        protected override void Awake()
        {
            base.Awake();

            ForegroundSystemCanvas.instance = this;
        }

        protected override void OnDestroy()
        {
            if (this == ForegroundSystemCanvas.instance)
                ForegroundSystemCanvas.instance = null;

            base.OnDestroy();
        }
    }
}
