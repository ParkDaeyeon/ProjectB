using UnityEngine;
using System;
namespace Ext.Unity3D
{
    [Serializable]
    public class SpriteCache
    {
        [SerializeField]
        Sprite sprite;
        public Sprite Sprite
        {
            get { return this.sprite; }
        }
        public static implicit operator Sprite(SpriteCache cache)
        {
            return null != cache ? cache.sprite : null;
        }

        public Texture2D Texture
        {
            get { return this.sprite ? this.sprite.texture : null; }
        }

        [SerializeField]
        Rect uvRect;
        public Rect UVRect
        {
            get { return this.uvRect; }
        }

        [SerializeField]
        Vector2 sizeOffset;
        public Vector2 SizeOffset
        {
            get { return this.sizeOffset; }
        }
        public Vector2 Size
        {
            get
            {
                if (this.sprite)
                    return this.sprite.rect.size - this.sizeOffset;

                return Vector2.zero;
            }
        }


        public SpriteCache(Sprite sprite)
        {
            this.Assign(sprite);
        }
        public void Assign(Sprite sprite)
        {
            this.sprite = sprite;
            this.uvRect = sprite.GetUVRect();
            this.sizeOffset = Vector2.zero;
        }


        public SpriteCache(Sprite sprite, Vector2 sizeOffset)
        {
            this.Assign(sprite, sizeOffset);
        }
        public void Assign(Sprite sprite, Vector2 sizeOffset)
        {
            this.sprite = sprite;
            this.uvRect = sprite.GetUVRect();
            this.sizeOffset = sizeOffset;
        }


        public SpriteCache(Sprite sprite, Rect pixelOffset)
        {
            this.Assign(sprite, pixelOffset);
        }
        public void Assign(Sprite sprite, Rect pixelOffset)
        {
            this.sprite = sprite;
            this.uvRect = sprite.GetUVRect(pixelOffset);
            this.sizeOffset = Vector2.zero;
        }

        
        public SpriteCache(Sprite sprite, Rect pixelOffset, Vector2 sizeOffset)
        {
            this.Assign(sprite, pixelOffset, sizeOffset);
        }
        public void Assign(Sprite sprite, Rect pixelOffset, Vector2 sizeOffset)
        {
            this.sprite = sprite;
            this.uvRect = sprite.GetUVRect(pixelOffset);
            this.sizeOffset = sizeOffset;
        }

        public static implicit operator bool(SpriteCache cache)
        {
            return null != cache;
        }

        public static bool IsValid(SpriteCache cache)
        {
            return null != cache && cache.sprite;
        }

        public override string ToString()
        {
            return string.Format("{{\"sprite\": {0}, \"texture\": {1}, \"uv\": [{2}, {3}, {4}, {5}], \"offset\": [{6}, {7}] }}",
                                (null == this.sprite ? "null" : string.Format("\"{0}\"", this.sprite.name)),
                                (null == this.Texture ? "null" : string.Format("\"{0}\"", this.Texture.name)),
                                this.uvRect.xMin, this.uvRect.yMin, this.uvRect.xMax, this.uvRect.yMax,
                                this.sizeOffset.x, this.sizeOffset.y);
        }
    }
}