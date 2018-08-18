using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

namespace Ext.Unity3D.UI
{
    public class ScrollRectExt : ScrollRect 
    {
        [SerializeField]
        public bool useMinClamped;
        [SerializeField]
        Vector2 minClampedPosition;

        [SerializeField]
        bool useMaxClamped;
        [SerializeField]
        Vector2 maxClampedPosition;

        bool isEndDrag = false;
        public Action OnBeginDragHandler;
        public Action<float> OnEndVelocityHandler;

        protected override void SetContentAnchoredPosition(Vector2 position)
        {
            if (this.isEndDrag)
            {
                float v = 0.0f;
                float n = 0.0f;
                if (this.horizontal)
                {
                    v = this.velocity.x;
                    n = this.normalizedPosition.x;
                }
                if (this.vertical)
                {
                    v = this.velocity.y;
                    n = this.normalizedPosition.y;
                }
                v = Mathf.Round(v);
                if (v == 0.0f || v == 1.0f || v == -1.0f)
                {
                    
                    this.isEndDrag = false;
                    if (this.OnEndVelocityHandler != null)
                        this.OnEndVelocityHandler(n);
                }
            }

            if (this.useMaxClamped)
            {
                position.x = this.maxClampedPosition.x < position.x ? this.maxClampedPosition.x : position.x;
                position.y = this.maxClampedPosition.y < position.y ? this.maxClampedPosition.y : position.y;
            }

            if (this.useMinClamped)
            {
                position.x = this.minClampedPosition.x < position.x ? position.x : this.minClampedPosition.x;
                position.y = this.minClampedPosition.y < position.y ? position.y : this.minClampedPosition.y;
            }
            base.SetContentAnchoredPosition(position);
        }

        public override void OnBeginDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            if (this.OnBeginDragHandler != null)
                this.OnBeginDragHandler();
        }

        public override void OnEndDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            this.isEndDrag = true;
        }

        public override void OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnDrag(eventData);
        }
    }
}
