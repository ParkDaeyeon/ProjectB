using Ext.Unity3D.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Ext.Unity3D.UI
{
    public class ResponsiveUvImage : ResponsiveUv
    {
        protected override void OnResize()
        {
            var viewSize = this.GetAreaSizeByCurrent();
            this.UpdateLayoutByCropedSprite(viewSize);
        }

        [SerializeField]
        OptimizedRectImage targetRectImage;
        void UpdateLayoutByCropedSprite(Vector2 viewSize)
        {
            var image = this.targetRectImage;

            var sprite = image.sprite;

            if (null == sprite)
                return;

            var spriteSize = sprite.rect.size;

            var uvOffset = image.UvOffset;
            var uvScale = this.GetCropedUvScale(viewSize, spriteSize);
            image.SetUv(uvOffset, uvScale);
        }

#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();
            
            this.targetRectImage = this.FindComponent<OptimizedRectImage>();
        }
#endif// UNITY_EDITOR
    }
}
