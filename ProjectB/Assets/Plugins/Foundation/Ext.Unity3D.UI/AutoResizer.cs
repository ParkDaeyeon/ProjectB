using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
namespace Ext.Unity3D.UI
{
    public class AutoResizer : ManagedUIComponent
    {
        public enum TYPE
        {
            Expand,
        }
        [SerializeField]
        protected TYPE type;
        public TYPE Type
        {
            set { this.type = value; }
            get { return this.type; }
        }

        [SerializeField]
        protected RectTransform target;
        public RectTransform Target
        {
            set { this.target = value; }
            get { return this.target; }
        }

        [SerializeField]
        protected new BoxCollider2D collider;
        public BoxCollider2D Collider
        {
            set { this.collider = value; }
            get { return this.collider; }
        }

        void OnEnable()
        {
            this.UpdateScreenSize();
        }

        public void UpdateScreenSize()
        {
            if (this.target)
            {
                Vector2 size = this.target.sizeDelta;
                Vector3 scale = this.target.localScale;
                if (TYPE.Expand == this.type)
                {
                    scale = size.CalcExpandAspectScale();
                    this.target.localScale = scale;
                }

                //if (this.collider)
                //{
                //    Vector2 scaledSize = size;
                //    scaledSize.x *= scale.x;
                //    scaledSize.y *= scale.y;
                //    this.collider.size = scaledSize;
                //}
            }
        }


#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();

            this.target = this.CachedTransform as RectTransform;
            this.collider = this.CachedTransform.GetComponent<BoxCollider2D>();
        }
        
        protected override void OnEditorTestingLooped()
        {
            base.OnEditorTestingLooped();
            this.UpdateScreenSize();
        }
#endif// UNITY_EDITOR
    }
}
