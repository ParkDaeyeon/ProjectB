using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Ext;
using Ext.Unity3D;
using Ext.Unity3D.UI;

namespace Program.View
{
    public class ForceAwakeView : BaseView
    {
        protected override void Awake()
        {
            base.Awake();

            Transform t = this.transform;
            var views = t.GetComponentsInChildren<BaseView>(true);
            for (int n = 0, count = views.Length; n < count; ++n)
                views[n].gameObject.ForceAwake();

            var compos = t.GetComponentsInChildren<ForceAwakeComponent>(true);
            for (int n = 0, count = compos.Length; n < count; ++n)
                compos[n].gameObject.ForceAwake();
        }
    }
}