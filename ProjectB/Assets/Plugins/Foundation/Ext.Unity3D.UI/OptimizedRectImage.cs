using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif// UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Ext.Unity3D.UI
{
    [ExecuteInEditMode]
    public class OptimizedRectImage : OptimizedImage
    {
        [SerializeField]
        Vector2 uvOffset = Vector2.zero;
        public Vector2 UvOffset
        {
            set
            {
                if (this.uvOffset == value)
                    return;

                this.uvOffset = value;
                this.UpdateUvRect();
            }
            get { return this.uvOffset; }
        }

        [SerializeField]
        Vector2 uvScale = Vector2.one;
        public Vector2 UvScale
        {
            set
            {
                if (this.uvScale == value)
                    return;

                this.uvScale = value;
                this.UpdateUvRect();
            }
            get { return this.uvScale; }
        }

        [SerializeField]
        Vector2 uvPivot = new Vector2(0.5f, 0.5f);
        public Vector2 UvPivot
        {
            set
            {
                if (this.uvPivot == value)
                    return;

                this.uvPivot = value;
                this.UpdateUvRect();
            }
            get { return this.uvScale; }
        }

        public void SetUv(Vector2 uvOffset, Vector2 uvScale)
        {
            this.uvOffset = uvOffset;
            this.uvScale = uvScale;
            this.UpdateUvRect();
        }

        Vector2 offsetMin;
        Vector2 offsetMax;

        Rect spriteUvRect;
        public Rect SpriteUvRect
        {
            get { return this.spriteUvRect; }
        }

        Rect currentUvRect;
        public Rect CurrentUvRect
        {
            get { return this.currentUvRect; }
        }

        public void UpdateUvRect()
        {
            this.OnUpdateUvRect(this.overrideSprite, true);
        }

        protected void OnUpdateUvRect(Sprite sprite, bool rebuild)
        {
            if (sprite)
            {
                var spriteRect = sprite.rect;
                var textureRect = sprite.textureRect;
                this.offsetMin = sprite.textureRectOffset;
                this.offsetMax = spriteRect.size - textureRect.size - offsetMin;

                this.spriteUvRect = sprite.GetUVRect(false);
            }
            else
            {
                this.offsetMin =
                this.offsetMax = Vector2.zero;

                this.spriteUvRect = default(Rect);
            }

            var spriteUvSize = this.spriteUvRect.size;

            var offset = this.uvOffset;
            var scale = this.uvScale;

            var uvPos = this.spriteUvRect.position;
            uvPos.x += spriteUvSize.x * offset.x;
            uvPos.y += spriteUvSize.y * offset.y;

            var uvSize = spriteUvSize;
            uvSize.x *= scale.x;
            uvSize.y *= scale.y;

            var uvSizeAdded = uvSize - spriteUvSize;
            uvPos.x += Mathf.Lerp(0, -uvSizeAdded.x, this.uvPivot.x);
            uvPos.y += Mathf.Lerp(0, -uvSizeAdded.y, this.uvPivot.y);

            var currentUvRect = new Rect(uvPos, uvSize);
            if (this.currentUvRect != currentUvRect)
            {
                this.currentUvRect = currentUvRect;

                if (rebuild)
                    this.SetVerticesDirty();
            }
        }

        protected override void OnPopulateMeshMainProcess(Translator translator,
                                                          RectTransform rectTrans,
                                                          Sprite sprite,
                                                          VertexHelper toFill)
        {
            if (!sprite || Type.Simple != this.type ||
                (sprite.packed && SpritePackingMode.Rectangle != sprite.packingMode))
            {
                base.OnPopulateMeshMainProcess(translator,
                                               rectTrans,
                                               sprite,
                                               toFill);
                return;
            }

            toFill.Clear();

            var baseColor = this.color;
            var size = rectTrans.GetSize();
            var uv = this.currentUvRect;

            var min = -size;
            var max = size;
            var transMin = translator.Translate(min);
            var transMax = translator.Translate(max);
            var rect = Rect.MinMaxRect(transMin.x, transMin.y,
                                       transMax.x, transMax.y);

            rect.min += this.offsetMin;
            rect.max -= this.offsetMax;

            toFill.AddVert(new Vector2(rect.xMin, rect.yMax), baseColor, new Vector2(uv.xMin, uv.yMax));
            toFill.AddVert(rect.max, baseColor, uv.max);
            toFill.AddVert(rect.min, baseColor, uv.min);
            toFill.AddVert(new Vector2(rect.xMax, rect.yMin), baseColor, new Vector2(uv.xMax, uv.yMin));

            toFill.AddTriangle(0, 1, 2);
            toFill.AddTriangle(2, 1, 3);
        }



        protected override void OnSpriteChanged(Sprite prev)
        {
            var sprite = this.SelectedSprite;
            this.OnUpdateUvRect(sprite, false);
        }

        public override bool Cacheable
        {
            get { return false; }
        }
    }
}
