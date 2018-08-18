using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Ext.Collection.AntiGC;
using System;

namespace Ext.Unity3D.UI.Pool
{
    public class _2DPool : Ext.Unity3D.Pool, IEnumerable<_2DCache>
    {
        [SerializeField]
        RectTransform cachedRectTransform;
        public RectTransform CachedRectTransform
        {
            get
            {
                if (!this.cachedRectTransform)
                    this.cachedRectTransform = this.GetComponent<RectTransform>();
                return this.cachedRectTransform;
            }
        }

        public new _2DCache this[int index] { get { return base[index] as _2DCache; } }

        public new IEnumerator<_2DCache> GetEnumerator()
        {
            for (int n = 0, cnt = this.Count; n < cnt; ++n)
                yield return this[n];
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public _2DCache ShowCache(int index, Vector2 pos)
        {
            var c = base.ShowCache(index) as _2DCache;
            if (c)
                c.CachedRectTransform.anchoredPosition = pos;

            return c;
        }
        
#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();
            this.cachedRectTransform = this.GetComponent<RectTransform>();
        }

        protected override Transform OnEditorCreateCachebase()
        {
            var t = base.OnEditorCreateCachebase().gameObject.AddComponent<RectTransform>();
            t.anchorMin = Vector2.zero;
            t.anchorMax = Vector2.one;
            t.pivot = new Vector2(0.5f, 0.5f);
            t.anchoredPosition = Vector2.zero;
            t.sizeDelta = Vector2.zero;
            return t;
        }
#endif// UNITY_EDITOR
    }
}
