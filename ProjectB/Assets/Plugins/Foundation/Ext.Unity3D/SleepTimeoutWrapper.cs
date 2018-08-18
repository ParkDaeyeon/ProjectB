using UnityEngine;
namespace Ext.Unity3D
{
    public static class SleepTimeoutWrapper
    {
        static bool activate = false;
        public static bool Active
        {
            set { SleepTimeoutWrapper.activate = value; }
            get { return SleepTimeoutWrapper.activate; }
        }

        static float delay = 0;
        public static float Delay
        {
            set { SleepTimeoutWrapper.delay = value; }
            get { return SleepTimeoutWrapper.delay; }
        }

        public static void UpdateSystemSetting()
        {
            if (SleepTimeoutWrapper.activate)
            {
                float delta = Time.realtimeSinceStartup - SleepTimeoutWrapper.delay;
                if (15f < delta)
                {
                    SleepTimeoutWrapper.activate = false;
                    Screen.sleepTimeout = (int)SleepTimeout.SystemSetting;
                }
            }
        }

        public static void SetSystemSetting()
        {
            if (SleepTimeout.NeverSleep == Screen.sleepTimeout && !SleepTimeoutWrapper.activate)
            {
                SleepTimeoutWrapper.activate = true;
                SleepTimeoutWrapper.delay = Time.realtimeSinceStartup;
            }
        }

        public static void SetNeverSleep()
        {
            Screen.sleepTimeout = (int)SleepTimeout.NeverSleep;
            SleepTimeoutWrapper.activate = false;
        }
    }
}