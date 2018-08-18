using Ext.Unity3D.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Ext.Unity3D.UI
{
    public class ResponsiveUvRawImage : ResponsiveUv
    {
        [SerializeField]
        RawImage targetRawImage;

        protected override void OnResize()
        {
            var viewSize = this.GetAreaSizeByCurrent();
            this.UpdateLayoutByCropedTexture(viewSize);
        }

        void UpdateLayoutByCropedTexture(Vector2 viewSize)
        {
            var image = this.targetRawImage;

            var texture = image.texture;

            var uvOffset = this.originUvPosition;
            var uvScale = this.originUvSize;

            var widthPixelCount  = texture.width * uvScale.x;
            var heightPixelCount = texture.height * uvScale.y;

            var spriteSize = new Vector2(widthPixelCount, heightPixelCount);

            var cropedUvScale = this.GetCropedUvScale(viewSize, spriteSize);
            cropedUvScale.x *= uvScale.x;
            cropedUvScale.y *= uvScale.y;

            uvOffset.x = (uvScale.x - cropedUvScale.x) * 0.5f;
            uvOffset.y = (uvScale.y - cropedUvScale.y) * 0.5f;

            image.uvRect = new Rect(uvOffset, cropedUvScale);
        }


        [SerializeField]
        Vector2 originUvSize     = Vector2.zero;
        [SerializeField]
        Vector2 originUvPosition = Vector2.zero;

#if UNITY_EDITOR
        [SerializeField]
        bool editorRebuild = false;

        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();

            if (this.editorRebuild)
            {
                this.editorRebuild = false;

                var rawImage = this.targetRawImage;
                var uvRect   = rawImage.uvRect;

                var position = uvRect.position;
                var size     = uvRect.size;

                this.originUvPosition = position;
                this.originUvSize     = size;
            }

            this.targetRawImage = this.FindComponent<RawImage>();
        }
#endif// UNITY_EDITOR
    }
}
