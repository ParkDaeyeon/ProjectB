using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
namespace Ext.Unity3D.UI
{
    public class ResponsiveSize : Responsive
    {
        public override int Order
        {
            get { return 1; }
        }

        protected override void OnResize()
        {
            var viewSize = this.GetAreaSizeByCurrent();
            var viewRatio = viewSize.x / viewSize.y;
            var normalizedAspectRatio = this.ToNormalizedAspectRatio(viewRatio);
            var interpolatedAspectRatio = this.ToInterpolatedAspectRatio(normalizedAspectRatio);

            this.UpdateLayoutBySize(viewSize, interpolatedAspectRatio);
        }

        void UpdateLayoutBySize(Vector2 viewSize, Vector2 interpolatedViewRatio)
        {
            var size = this.ToSize(interpolatedViewRatio);

            var trans = this.CachedRectTransform;
            var newSizeDelta = trans.sizeDelta;
            if (this.horizontal)
                newSizeDelta.x = size.x;
            if (this.vertical)
                newSizeDelta.y = size.y;

            trans.sizeDelta = newSizeDelta;
        }
        
        [SerializeField]
        Interpolator horzInterpolator;
        [SerializeField]
        Interpolator vertInterpolator;

        [SerializeField]
        Vector2 minAspectRatio;
        [SerializeField]
        Vector2 maxAspectRatio;

        Vector2 ToNormalizedAspectRatio(float aspectRatio)
        {
            var rangeA = this.maxAspectRatio - this.minAspectRatio;
            return new Vector2
            (
                Mathf.Clamp01(0 != rangeA.x ? (aspectRatio - this.minAspectRatio.x) / rangeA.x : 0),
                Mathf.Clamp01(0 != rangeA.y ? (aspectRatio - this.minAspectRatio.y) / rangeA.y : 0)
            );
        }

        Vector2 ToInterpolatedAspectRatio(Vector2 normalizedAspectRatio)
        {
            if (null != this.horzInterpolator)
                normalizedAspectRatio.x = this.horzInterpolator.Interpolate(normalizedAspectRatio.x);
            if (null != this.vertInterpolator)
                normalizedAspectRatio.y = this.vertInterpolator.Interpolate(normalizedAspectRatio.y);

            return normalizedAspectRatio;
        }
        
        [SerializeField]
        bool horizontal = true;
        [SerializeField]
        bool vertical = true;

        [SerializeField]
        Vector2 minSize;
        [SerializeField]
        Vector2 maxSize;

        public Vector2 ToSize(Vector2 aspectRatio)
        {
            return new Vector2
            (
                Mathf.Lerp(this.minSize.x, this.maxSize.x, aspectRatio.x),
                Mathf.Lerp(this.minSize.y, this.maxSize.y, aspectRatio.y)
            );
        }
    }
}
