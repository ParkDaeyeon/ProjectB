using System;
using UnityEngine;
using UnityEngine.UI;
namespace Ext.Unity3D.UI.Pool
{
    public class _2DCache : PoolCache
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

#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();
            this.cachedRectTransform = this.GetComponent<RectTransform>();
        }
#endif// UNITY_EDITOR
    }
}
