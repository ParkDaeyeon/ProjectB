using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Ext;
using Ext.Unity3D;
using Ext.Unity3D.UI;

namespace Program.View
{
    public static class ViewTimer
    {
        public static long GetTimeMsLossy()
        {
            return (long)(Time.time * 1000);
        }


        static Func<long> onGetTimeMsStrict;
        public static Func<long> OnGetTimeMsStrict
        {
            set { ViewTimer.onGetTimeMsStrict = value; }
            get { return ViewTimer.onGetTimeMsStrict; }
        }
        public static long GetTimeMsStrict()
        {
            return null != ViewTimer.onGetTimeMsStrict ? ViewTimer.onGetTimeMsStrict() : 0;
        }


        static Func<long> onGetTimeMsNetwork;
        public static Func<long> OnGetTimeMsNetwork
        {
            set { ViewTimer.onGetTimeMsNetwork = value; }
            get { return ViewTimer.onGetTimeMsNetwork; }
        }
        public static long GetTimeMsNetwork()
        {
            return null != ViewTimer.onGetTimeMsNetwork ? ViewTimer.onGetTimeMsNetwork() : 0;
        }



        public enum Function
        {
            Lossy,
            Strict,
            Network,
        }
        static Func<long> GetTimeMsFunction(Function func)
        {
            switch (func)
            {
            case Function.Strict: return ViewTimer.onGetTimeMsStrict;
            case Function.Network: return ViewTimer.onGetTimeMsNetwork;
            default: return ViewTimer.GetTimeMsLossy;
            }
        }

        public static Timer CreateTimer(Function func = Function.Lossy,
                                        long offsetMs = 0,
                                        long durationMs = 0)
        {
            return new Timer(ViewTimer.GetTimeMsFunction(func), offsetMs, durationMs);
        }

        public static DynamicTimer CreateDynamicTimer(Function func = Function.Lossy,
                                                      long offsetMs = 0,
                                                      long durationMs = 0)
        {
            return new DynamicTimer(ViewTimer.GetTimeMsFunction(func), offsetMs, durationMs);
        }
    }
}
