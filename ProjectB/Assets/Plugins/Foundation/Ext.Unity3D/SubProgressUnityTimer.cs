using UnityEngine;
using System;

namespace Ext.Unity3D
{
    public class SubProgressUnityTimer : SubProgress.ILimitTimer
    {
        public float CurrentTime
        {
            get { return Time.realtimeSinceStartup; }
        }

        float manualTime = 1f;
        public float ManualTime
        {
            set { this.manualTime = value; }
            get { return this.manualTime; }
        }
    }
}
