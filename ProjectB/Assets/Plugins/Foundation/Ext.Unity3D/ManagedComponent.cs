using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Ext.Unity3D
{
    public abstract class ManagedComponent : SetupableComponent
    {
        [SerializeField, HideInInspector]
        Transform cachedTransform;
        public Transform CachedTransform
        {
            get
            {
                if (!this)
                    return null;
                if (!this.cachedTransform)
                    this.cachedTransform = this.transform;
                return this.cachedTransform;
            }
        }

        public void SetActive(bool value)
        {
            this.gameObject.SetActive(value);
        }
        public bool IsActivated
        {
            get { return this.gameObject.activeSelf; }
        }
        
        public void SetShow(bool value)
        {
            if (value)
                this.CachedTransform.ShowTransform();
            else
                this.CachedTransform.HideTransform();
        }
        public bool IsShown
        {
            get { return !this.CachedTransform.IsHideTransform(); }
        }

#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();
            this.cachedTransform = this.transform;
        }
#endif// UNITY_EDITOR
    }
}
