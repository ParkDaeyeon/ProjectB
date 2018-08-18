using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
namespace Ext.Unity3D
{
    using THIS = ForceOrientation;
    public static class ForceOrientation
    {
        struct CompareData
        {
            public bool allow;
            public ScreenOrientation orientation;

            public CompareData(bool allow, ScreenOrientation orientation)
            {
                this.allow = allow;
                this.orientation = orientation;
            }
        }
        static CompareData[] compareDatas = new CompareData[4];

        public static void Update()
        {
            var allowPortU = Screen.autorotateToPortrait;
            var allowPortD = Screen.autorotateToPortraitUpsideDown;
            var allowLandL = Screen.autorotateToLandscapeLeft;
            var allowLandR = Screen.autorotateToLandscapeRight;

            var indexer = 0;
            var orientationCurrent = Screen.orientation;
            switch (orientationCurrent)
            {
            default:
            case ScreenOrientation.AutoRotation:
                return;

            case ScreenOrientation.Portrait:
                THIS.compareDatas[indexer++] = new CompareData(allowPortU, ScreenOrientation.Portrait);
                THIS.compareDatas[indexer++] = new CompareData(allowPortD, ScreenOrientation.PortraitUpsideDown);
                THIS.compareDatas[indexer++] = new CompareData(allowLandL, ScreenOrientation.LandscapeLeft);
                THIS.compareDatas[indexer++] = new CompareData(allowLandR, ScreenOrientation.LandscapeRight);
                break;

            case ScreenOrientation.PortraitUpsideDown:
                THIS.compareDatas[indexer++] = new CompareData(allowPortD, ScreenOrientation.PortraitUpsideDown);
                THIS.compareDatas[indexer++] = new CompareData(allowPortU, ScreenOrientation.Portrait);
                THIS.compareDatas[indexer++] = new CompareData(allowLandL, ScreenOrientation.LandscapeLeft);
                THIS.compareDatas[indexer++] = new CompareData(allowLandR, ScreenOrientation.LandscapeRight);
                break;

            case ScreenOrientation.LandscapeLeft:
                THIS.compareDatas[indexer++] = new CompareData(allowLandL, ScreenOrientation.LandscapeLeft);
                THIS.compareDatas[indexer++] = new CompareData(allowLandR, ScreenOrientation.LandscapeRight);
                THIS.compareDatas[indexer++] = new CompareData(allowPortU, ScreenOrientation.Portrait);
                THIS.compareDatas[indexer++] = new CompareData(allowPortD, ScreenOrientation.PortraitUpsideDown);
                break;

            case ScreenOrientation.LandscapeRight:
                THIS.compareDatas[indexer++] = new CompareData(allowLandR, ScreenOrientation.LandscapeRight);
                THIS.compareDatas[indexer++] = new CompareData(allowLandL, ScreenOrientation.LandscapeLeft);
                THIS.compareDatas[indexer++] = new CompareData(allowPortU, ScreenOrientation.Portrait);
                THIS.compareDatas[indexer++] = new CompareData(allowPortD, ScreenOrientation.PortraitUpsideDown);
                break;
            }

            for (int n = 0, cnt = THIS.compareDatas.Length; n < cnt; ++n)
            {
                var compareData = THIS.compareDatas[n];
                if (compareData.allow)
                {
                    if (compareData.orientation != orientationCurrent)
                        Screen.orientation = compareData.orientation;

                    break;
                }
            }
        }
    }
}
