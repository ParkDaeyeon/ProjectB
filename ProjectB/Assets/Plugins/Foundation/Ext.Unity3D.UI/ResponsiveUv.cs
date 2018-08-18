using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
namespace Ext.Unity3D.UI
{
    public abstract class ResponsiveUv : Responsive
    {
        public override int Order
        {
            get { return 1; }
        }

        protected Vector2 GetCropedUvScale(Vector2 viewSize, Vector2 spriteSize)
        {
            var cropedViewSize = viewSize * this.GetScaleFactor(viewSize, spriteSize);

            var uvX = cropedViewSize.x / spriteSize.x;
            var uvY = cropedViewSize.y / spriteSize.y;

            var cropedUvScale = this.ToRatio(uvX, uvY);
            return cropedUvScale;
        }

        protected float GetScaleFactor(Vector2 sourceSize, Vector2 targetRectSize)
        {
            var sourceRatio = this.ToRatio(sourceSize);
            var targetRectRatio = this.ToRatio(targetRectSize);

            if (sourceRatio.x != targetRectRatio.x)
                return sourceSize.y / targetRectSize.y;

            else if (sourceRatio.y != targetRectRatio.y)
                return sourceSize.x / targetRectSize.x;

            else
                return sourceSize.x / targetRectSize.x;
        }

        protected Vector2 ToRatio(Vector2 size)
        {
            return this.ToRatio(size.x, size.y);
        }

        protected Vector2 ToRatio(float width, float height)
        {
            if (width == height)
                return Vector2.one;

            else if (width > height)
                return this.ToRatioWhenWidthIsOne(width, height);

            else
                return this.ToRatioWhenHeightIsOne(width, height);
        }

        protected Vector2 ToRatioWhenWidthIsOne(float width, float height)
        {
            return new Vector2(1f, height / width);
        }

        protected Vector2 ToRatioWhenHeightIsOne(float width, float height)
        {
            return new Vector2(width / height, 1f);
        }
    }
}
