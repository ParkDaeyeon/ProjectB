using UnityEngine;
using System;
using System.Collections;
namespace Ext.Unity3D
{
    public class SpriteResizer : MonoBehaviour
    {
        [SerializeField]
        protected Transform cachedTransform;
        [SerializeField]
        protected RectTransform rectTransform;
        [SerializeField]
        protected SpriteRenderer spriteRenderer;
        // DEPRECATED: GCSpike
        [SerializeField]
        protected BoxCollider2D optionalCollider;
        [SerializeField]
        protected float scalePerUnit = 100;

        public enum TYPE
        {
            Fixed,
            Expand,
        }
        [SerializeField]
        protected TYPE type;
        public TYPE Type
        {
            set { this.type = value; }
            get { return this.type; }
        }

        void OnEnable()
        {
            this.UpdateResize();
            this.UpdateCollider();
        }

        public void UpdateResize()
        {
            if (!this.rectTransform || !this.spriteRenderer)
                return;

            Sprite sprite = this.spriteRenderer.sprite;
            if (!sprite)
                return;

            Vector2 spriteSize = sprite.rect.size;
            Vector2 targetSize = this.rectTransform.rect.size;
            Vector3 scale = this.cachedTransform.localScale;
            scale.x = (targetSize.x / spriteSize.x) * this.scalePerUnit;
            scale.y = (targetSize.y / spriteSize.y) * this.scalePerUnit;
            if (TYPE.Expand == this.type)
            {
                Vector2 calcScale = targetSize.CalcExpandAspectScale();
                scale.x *= calcScale.x;
                scale.y *= calcScale.y;
            }
            this.cachedTransform.localScale = scale;
        }


        public void UpdateCollider()
        {
            if (!this.optionalCollider)
                return;

            Sprite sprite = this.spriteRenderer.sprite;
            if (!sprite)
                return;

            Vector2 spriteSize = sprite.rect.size;
            Vector2 colliderScale;
            colliderScale.x = spriteSize.x / sprite.pixelsPerUnit;
            colliderScale.y = spriteSize.y / sprite.pixelsPerUnit;

            this.optionalCollider.size = colliderScale;
        }


#if UNITY_EDITOR
        [SerializeField]
        protected bool editorSettings;
        [SerializeField]
        protected bool editorUpdate;
        void OnDrawGizmos()
        {
            if (this.editorSettings)
            {
                this.editorSettings = false;

                this.cachedTransform = this.transform;
                this.rectTransform = this.GetComponent<RectTransform>();
                this.spriteRenderer = this.GetComponent<SpriteRenderer>();
                this.optionalCollider = this.GetComponent<BoxCollider2D>();
            }

            if (this.editorUpdate)
            {
                this.editorUpdate = false;

                this.UpdateResize();
                UnityExtension.RepaintAllViews();
            }
        }
#endif// UNITY_EDITOR
    }
}