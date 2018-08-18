using UnityEngine;
using System;
using System.Collections.Generic;
namespace Ext.Unity3D
{
    public static class ScreenReducer
    {
        public class ResolutionInfo
        {
            int w;
            public int Width { get { return this.w; } }

            int h;
            public int Height { get { return this.h; } }

            double a;
            public double AspectRatio { get { return this.a; } }

            int magnitude;
            public int Magnitude { get { return this.magnitude; } }

            public ResolutionInfo(int w, int h)
            {
                this.w = w;
                this.h = h;
                this.a = w / (double)h;
                this.magnitude = w * h;
            }
        }

        static List<ResolutionInfo> resolutions = new List<ResolutionInfo>(8);
        public static List<ResolutionInfo> Resolutions { get { return ScreenReducer.resolutions; } }

        public static int SystemWidth
        {
            set { ScreenReducer.systemWidth = value; }
            get { return ScreenReducer.systemWidth; }
        }

        public static int SystemHeight
        {
            set { ScreenReducer.systemHeight = value; }
            get { return ScreenReducer.systemHeight; }
        }


        static int systemWidth = Screen.width;
        static int systemHeight = Screen.height;
        static bool isInited = false;
        public static bool IsInited { get { return ScreenReducer.isInited; } }
        public static void InitOnetime()
        {
            if (ScreenReducer.isInited)
                return;

            ScreenReducer.isInited = true;
            ScreenReducer.systemWidth = Screen.width;
            ScreenReducer.systemHeight = Screen.height;
        }


        public static void Restore()
        {
            int sw = ScreenReducer.systemWidth;
            int sh = ScreenReducer.systemHeight;
#if LOG_DEBUG
            Debug.Log("SCREEN_REDUCER:RESTORE_RESOLUTION:SW:" + sw + ", SH:" + sh + ", W:" + Screen.width + ", H:" + Screen.height);
#endif// LOG_DEBUG
            if (sw != Screen.width || sh != Screen.height)
                Screen.SetResolution(sw, sh, Screen.fullScreen);
        }

        public static void Apply(int idx)
        {
            //      ---- 1920 x 1080 기준 [16:9] ----

            //      [16:10]
            //      ((16/9)/(16/9))=1
            //      ((16/10)/(16/9))=0.9
            //      (((1-((16/10)/(16/9)))/2)+((16/10)/(16/9)))=0.95

            //      W=1920*(((1-((16/10)/(16/9)))/2)+((16/10)/(16/9)))=1824
            //      W=1824

            //      H=1824*(10/16)=1140
            //      H=1140

            //      verify:
            //      1824*1140=2079360
            //      1920*1080=2073600
            //      2079360/2073600=1.0027778

            //      result:
            //      0.2% 오차


            //      [3:2]
            //      ((16/9)/(16/9))=1
            //      ((3/2)/(16/9))=0.84375
            //      (((1-((3/2)/(16/9)))/2)+((3/2)/(16/9)))=0.921875

            //      W=1920*(((1-((3/2)/(16/9)))/2)+((3/2)/(16/9)))=1770
            //      W=1770

            //      H=1770*(2/3)=1180
            //      H=1180

            //      verify:
            //      1770*1180=2088600
            //      1920*1080=2073600
            //      2088600/2073600=1.0072338

            //      result:
            //      0.7% 오차


            //      [4:3]
            //      ((16/9)/(16/9))=1
            //      ((4/3)/(16/9))=0.75
            //      (((1-((4/3)/(16/9)))/2)+((4/3)/(16/9)))=0.875

            //      W=1920*(((1-((4/3)/(16/9)))/2)+((4/3)/(16/9)))=1680
            //      W=1680

            //      H=1680*(3/4)=1260
            //      H=1260

            //      verify:
            //      1680*1260=2116800
            //      1920*1080=2073600
            //      2116800/2073600=1.020833

            //      result:
            //      2% 오차 -_-



            if (null == ScreenReducer.resolutions)
            {
                Debug.LogWarning("SCREEN_REDUCER:RESET_RESOLUTION:INTERNAL_ERROR");
                return;
            }

            if (0 > idx || idx >= ScreenReducer.resolutions.Count)
            {
                Debug.LogWarning("SCREEN_REDUCER:RESET_RESOLUTION:BAD_RESOLUTION_IDX:" + idx);
                return;
            }
            int sw = ScreenReducer.systemWidth;
            int sh = ScreenReducer.systemHeight;
            double sa = sw / (double)sh;
            int smagnitude = sw * sh;

#if LOG_DEBUG
            Debug.Log("SCREEN_REDUCER:RESET_RESOLUTION:SW:" + sw + ", SH:" + sh + ", SA:" + sa + ", SMAG:" + smagnitude);
#endif// LOG_DEBUG

            var target = ScreenReducer.resolutions[idx];
#if LOG_DEBUG
            Debug.Log("SCREEN_REDUCER:RESET_RESOLUTION:TW:" + target.Width + ", TH:" + target.Height + ", TA:" + target.AspectRatio + ", TMAG:" + target.Magnitude);
            Debug.Log("SCREEN_REDUCER:RESET_RESOLUTION:DMAG > TMAG:" + (smagnitude > target.Magnitude));
#endif// LOG_DEBUG

            if (smagnitude > target.Magnitude)
            {
                double reductionAspectRatio = sa / target.AspectRatio;
                double reductionScale = ((1 - reductionAspectRatio) / 2) + reductionAspectRatio;
                int w = (int)(target.Width * reductionScale);

                double daInv = sh / (double)sw;
                int h = (int)(w * daInv);

                if (w != Screen.width || h != Screen.height)
                {
                    Screen.SetResolution(w, h, Screen.fullScreen);
#if LOG_DEBUG
                    Debug.Log("SCREEN_REDUCER:RESET_RESOLUTION:CHANGE_SCREEN:REDUCTION_ASPECTRATIO:" + reductionAspectRatio + ", REDUCTION_SCALE:" + reductionScale + ",\nW:" + w + ", H:" + h);
#endif// LOG_DEBUG
                }
            }
        }

        public static void LogCurrentScreen()
        {
            Debug.Log("SCREEN_REDUCER:LOG_CURRENT_SCREEN\nScreen.width:" + Screen.width + ", Screen.height:" + Screen.height + ", Screen.fullScreen:" + Screen.fullScreen);
        }
    }
}