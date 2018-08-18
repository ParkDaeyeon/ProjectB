using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Ext.Unity3D.UI
{
    public class ResponsiveScale : Responsive
    {
        public override int Order
        {
            get { return 1; }
        }

        protected override void OnResize()
        {
            var safeAreaSize = Responsive.GetAreaSizeByMode(AreaMode.Safe);
            var viewportSize = Responsive.GetAreaSizeByMode(AreaMode.Viewport);
            var scale = UnityExtension.GetScaleOfAreaSize(safeAreaSize, viewportSize);

            var transform = this.CachedRectTransform;
            
            var newScaleDelta = new Vector3(this.originScale.x, this.originScale.y, 1);

            if (this.horizontal)
                newScaleDelta.x /= scale;
            if (this.vertical)
                newScaleDelta.y /= scale;

            newScaleDelta.x = Mathf.Clamp(newScaleDelta.x, this.minScale.x, this.maxScale.x);
            newScaleDelta.y = Mathf.Clamp(newScaleDelta.y, this.minScale.y, this.maxScale.y);

            transform.localScale = newScaleDelta;
        }

        [SerializeField]
        bool horizontal;
        [SerializeField]
        bool vertical;

        [SerializeField]
        Vector2 minScale = new Vector2(-1f, -1f);
        [SerializeField]
        Vector2 maxScale = Vector2.one;
        [SerializeField]
        Vector2 originScale = Vector2.one;
    }
}
