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
    public class ForemostGroundSystemCanvas : ForceAwakeView
    {
        static ForemostGroundSystemCanvas instance;
        public static ForemostGroundSystemCanvas Instance
        {
            get { return ForemostGroundSystemCanvas.instance; }
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

            ForemostGroundSystemCanvas.instance = this;
        }

        protected override void OnDestroy()
        {
            if (this == ForemostGroundSystemCanvas.instance)
                ForemostGroundSystemCanvas.instance = null;

            base.OnDestroy();
        }
    }
}
