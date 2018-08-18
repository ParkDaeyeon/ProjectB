using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Ext;
using Ext.Unity3D;
using Ext.Unity3D.UI;
using Ext.Async;

namespace Program.View
{
    public static class ViewAsync
    {
        static CancellableSignal signal;

        public static void ResetSignal(CancellableSignal value)
        {
            if (null != ViewAsync.signal)
                CancellableSignal.Cancel(ViewAsync.signal);

            ViewAsync.signal = value;
        }

        public static CancellableSignal GetSignal()
        {
            return ViewAsync.signal;
        }
    }
}
