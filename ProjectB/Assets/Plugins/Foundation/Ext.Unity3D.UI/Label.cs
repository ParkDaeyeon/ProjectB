using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Collections;

namespace Ext.Unity3D.UI
{
    [Serializable]
    public class Label
    {
        [SerializeField]
        Graphic[] elems;
        [SerializeField]
        bool autoVisibleWithValid = true;

        public Label(Graphic[] elems = null, bool autoValidVisible = true)
        {
            this.Reuild(elems);
            this.autoVisibleWithValid = autoValidVisible;
        }
        public void Reuild(Graphic[] elems)
        {
            this.elems = elems;
        }

        internal Graphic[] __access()
        {
            return this.elems;
        }

        internal bool __autoVisibleWithValid()
        {
            return this.autoVisibleWithValid;
        }

        internal void __set_autoVisibleWithValid(bool value)
        {
            this.autoVisibleWithValid = value;
        }
        
        public static implicit operator bool(Label labels)
        {
            return null != labels && null != labels.elems && 0 < labels.elems.Length;
        }

        public override string ToString()
        {
            return string.Format("{{Count:{0}, Auto:{1}}}",
                                 null != this.elems ? this.elems.Length : 0,
                                 this.autoVisibleWithValid);
        }
    }

    public static class LabelExtension
    {
        public static IEnumerator<Graphic> GetElements(this Label thiz)
        {
            if (!thiz)
                yield break;

            var elems = thiz.__access();
            for (int n = 0, cnt = elems.Length; n < cnt; ++n)
                yield return elems[n];
        }

        public static int GetCount(this Label thiz)
        {
            if (!thiz)
                return 0;
            
            return thiz.__access().Length;
        }

        public static Graphic Get(this Label thiz, int index)
        {
            if (!thiz)
                return null;

            var elems = thiz.__access();
            return -1 < index && index < elems.Length ? elems[index] : null;
        }

        public static object GetData(this Label thiz, int index)
        {
            return thiz.Get(index).GetData();
        }

        public static bool SetData(this Label thiz, int index, object data)
        {
            return thiz.Get(index).SetData(data);
        }

        public static object[] GetDatas(this Label thiz)
        {
            if (!thiz)
                return new object[0];

            var elems = thiz.__access();
            var count = elems.Length;
            var datas = new object[count];
            for (int n = 0; n < count; ++n)
                datas[n] = elems[n].GetData();

            return datas;
        }

        public static void SetDatas(this Label thiz, object[] datas)
        {
            if (!thiz)
                return;

            var elems = thiz.__access();
            var count = elems.Length;
            var autoVisible = thiz.__autoVisibleWithValid();
            for (int n = 0; n < count; ++n)
            {
                var label = elems[n];
                var data = null != datas && n < datas.Length ? datas[n] : null;
                label.SetData(data);
                if (autoVisible && label)
                    label.gameObject.SetActive(label.IsValid());
            }
        }

        public static bool IsValid(this Label thiz, int index)
        {
            return thiz.Get(index).IsValid();
        }

        public static bool IsValidAny(this Label thiz)
        {
            if (thiz)
            {
                var elems = thiz.__access();
                var count = elems.Length;
                for (int n = 0; n < count; ++n)
                {
                    if (elems[n].IsValid())
                        return true;
                }
            }
            return false;
        }

        public static void SetVisible(this Label thiz, bool value)
        {
            if (thiz)
            {
                var elems = thiz.__access();
                var count = elems.Length;
                for (int n = 0; n < count; ++n)
                {
                    var label = elems[n];
                    if (label)
                        label.gameObject.SetActive(value);
                }
            }
        }
        
        public static bool IsVisibledAny(this Label thiz)
        {
            if (thiz)
            {
                var elems = thiz.__access();
                var count = elems.Length;
                for (int n = 0; n < count; ++n)
                {
                    var label = elems[n];
                    if (label && label.gameObject.activeSelf)
                        return true;
                }
            }

            return false;
        }

        public static void SetVisibleWithValid(this Label thiz)
        {
            if (thiz)
            {
                var elems = thiz.__access();
                var count = elems.Length;
                for (int n = 0; n < count; ++n)
                {
                    var label = elems[n];
                    if (label)
                        label.gameObject.SetActive(label.IsValid());
                }
            }
        }

        public static bool IsAutoVisibleWithValid(this Label thiz)
        {
            return null != thiz ? thiz.__autoVisibleWithValid() : false;
        }

        public static void SetAutoVisibleWithValid(this Label thiz, bool value)
        {
            if (null != thiz)
                thiz.__set_autoVisibleWithValid(value);
        }
    }
}
