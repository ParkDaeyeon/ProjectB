using UnityEngine;

namespace Ext.Unity3D
{
    public static class SpriteExtension
    {
        public static Rect GetUVRect(this Sprite sprite, bool fullRect = true)
        {
            if (!sprite)
                return default(Rect);

            var uvRect = sprite.textureRect;
            if (fullRect)
            {
                var offset = sprite.textureRectOffset;
                uvRect.xMin -= offset.x;
                uvRect.yMin -= offset.y;

                var rect = sprite.rect;
                uvRect.width = rect.width;
                uvRect.height = rect.height;
            }

            var texture = sprite.texture;
            if (texture)
            {
                float w = texture.width;
                float h = texture.height;
                uvRect.xMin /= w;
                uvRect.xMax /= w;
                uvRect.yMin /= h;
                uvRect.yMax /= h;
            }

            return uvRect;
        }
        
        public static Rect GetUVRect(this Sprite sprite, Rect pixelOffset, bool fullRect = true)
        {
            if (!sprite)
                return default(Rect);
            
            var uvRect = sprite.textureRect;
            if (fullRect)
            {
                var offset = sprite.textureRectOffset;
                uvRect.xMin -= offset.x;
                uvRect.yMin -= offset.y;

                var rect = sprite.rect;
                uvRect.width = rect.width;
                uvRect.height = rect.height;
            }
            uvRect.xMin += pixelOffset.xMin;
            uvRect.yMin += pixelOffset.yMin;
            uvRect.xMax -= pixelOffset.width;
            uvRect.yMax -= pixelOffset.height;

            var texture = sprite.texture;
            if (texture)
            {
                float w = texture.width;
                float h = texture.height;
                uvRect.xMin /= w;
                uvRect.xMax /= w;
                uvRect.yMin /= h;
                uvRect.yMax /= h;
            }

            return uvRect;
        }
    }
}
