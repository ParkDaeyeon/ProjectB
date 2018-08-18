//using UnityEngine;
//using UnityEngine.UI;
//using System;
//using System.Collections.Generic;

//namespace Ext.Unity3D.UI
//{
//    public class UIAnimSequencer : MonoBehaviour
//    {
//        [Serializable]
//        public class Element
//        {
//            public Element(float playTime,
//                           UIAnim anim)
//            {
//                this.playTime = playTime;
//                this.anim = anim;
//            }

//            [SerializeField]
//            float playTime;
//            public float PlayTime
//            {
//                set { this.playTime = value; }
//                get { return this.playTime; }
//            }

//            public float LastTime
//            {
//                get { return this.playTime + (this.anim ? this.anim.TotalDuration : 0); }
//            }

//            [SerializeField]
//            UIAnim anim;
//            public UIAnim Anim
//            {
//                set { this.anim = value; }
//                get { return this.anim; }
//            }
//        }

//        List<Element> list = new List<Element>();
//        public List<Element> List { get { return this.list; } }


//        [SerializeField]
//        float totalDuration;
//        public float TotalDuration
//        {
//            get { return this.totalDuration; }
//        }
//        void UpdateTotalDuration()
//        {
//            var max = 0f;
//            for (int n = 0, cnt = this.list.Count; n < cnt; ++n)
//                max = Mathf.Max(max, this.list[n].LastTime);

//            this.totalDuration = max;
//        }

//    }
//}
