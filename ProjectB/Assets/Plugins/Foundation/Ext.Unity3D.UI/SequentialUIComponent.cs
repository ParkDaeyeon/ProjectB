using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

namespace Ext.Unity3D.UI
{
    public abstract class SequentialUIComponent : SequentialComponent
    {
        [SerializeField, HideInInspector]
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
