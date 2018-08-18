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
    public class OptimizedFilledImage : OptimizedImage
    {
        // NOTE: Image.fillAmount = GC spiked (VERY SUCKS UNITY3D-UI ENGINEER)
        [SerializeField]
        float fillAmountOptimized;
        public float FillAmount
        {
            set
            {
                if (this.fillAmountOptimized == value)
                    return;

                this.fillAmountOptimized = value;
                this.SetVerticesDirty();
            }
            get { return this.fillAmountOptimized; }
        }


        [SerializeField]
        Vector2 borderMin;
        public Vector2 BorderMin
        {
            set
            {
                if (this.borderMin == value)
                    return;
                
                this.borderMin = value;
                this.SetVerticesDirty();
            }
            get { return this.borderMin; }
        }

        [SerializeField]
        Vector2 borderMax;
        public Vector2 BorderMax
        {
            set
            {
                if (this.borderMax == value)
                    return;

                this.borderMax = value;
                this.SetVerticesDirty();
            }
            get { return this.borderMax; }
        }

        [SerializeField]
        bool pixelPerfect;
        public bool PixelPerfect
        {
            set
            {
                if (this.pixelPerfect == value)
                    return;

                this.pixelPerfect = value;
                this.SetVerticesDirty();
            }
            get { return this.pixelPerfect; }
        }


        public class FillData
        {
            float start;
            public float Start
            {
                get { return this.start; }
            }

            float range;
            public float Range
            {
                get { return this.range; }
            }

            float ratio;
            public float Ratio
            {
                get { return this.ratio; }
            }

            public void Update(OptimizedFilledImage image)
            {
                switch (image.fillMethod)
                {
                case FillMethod.Horizontal:
                case FillMethod.Vertical:
                    break;

                default:
                    // NOT SUPPORT
                    return;
                }

                var size = image.rectTransform.sizeDelta;

                var fillBorderMin = 0f;
                var fillBorderMax = 0f;

                var offsetMin = image.offsetMin;
                var offsetMax = image.offsetMax;

                switch (image.fillMethod)
                {
                case FillMethod.Horizontal:
                    this.start = offsetMin.x;
                    this.range = size.x - (this.start + offsetMax.x);
                    fillBorderMin = image.borderMin.x / size.x;
                    fillBorderMax = image.borderMax.x / size.x;
                    break;

                case FillMethod.Vertical:
                    this.start = offsetMin.y;
                    this.range = size.y - (this.start + offsetMax.y);
                    fillBorderMin = image.borderMin.y / size.y;
                    fillBorderMax = image.borderMax.y / size.y;
                    break;
                }

                var fillAmount = image.FillAmount;
                this.ratio = Mathf.Lerp(fillBorderMin, 1 - fillBorderMax, fillAmount);

                if (image.pixelPerfect)
                {
                    var current = this.start + this.range * this.ratio;
                    this.ratio = 0 < this.range ? (Mathf.Floor(current) - this.start) / this.range : 0;
                }
            }
        }
        FillData fill = new FillData();
        public FillData Fill
        {
            get { return this.fill; }
        }

        protected override void Awake()
        {
            base.Awake();

            this.fillAmountOptimized = base.fillAmount;
        }
        
        protected override void OnPopulateMeshMainProcess(Translator translator,
                                                          RectTransform rectTrans,
                                                          Sprite sprite,
                                                          VertexHelper toFill)
        {
            switch (this.fillMethod)
            {
            case FillMethod.Horizontal:
            case FillMethod.Vertical:
                break;

            default:
                // NOT SUPPORT
                base.OnPopulateMeshMainProcess(translator,
                                               rectTrans,
                                               sprite,
                                               toFill);
                return;
            }

            toFill.Clear();

            this.fill.Update(this);

            var fillAmount = this.FillAmount;
            if (0 < fillAmount)
            {
                var baseColor = this.color;
                var size = rectTrans.sizeDelta;
                var uv = this.uvRect;

                var min = -size;
                var max = size;
                var transMin = translator.Translate(min);
                var transMax = translator.Translate(max);
                var rect = Rect.MinMaxRect(transMin.x, transMin.y,
                                           transMax.x, transMax.y);
                
                rect.min += this.offsetMin;
                rect.max -= this.offsetMax;
                
                if (1 > fillAmount)
                {
                    var ratio = this.fill.Ratio;

                    switch (this.fillMethod)
                    {
                    case FillMethod.Horizontal:
                        switch (this.fillOrigin)
                        {
                        case 0:
                            rect.xMax = Mathf.Lerp(rect.xMin, rect.xMax, ratio);
                            uv.xMax = Mathf.Lerp(uv.xMin, uv.xMax, ratio);
                            break;

                        case 1:
                            rect.xMin = Mathf.Lerp(rect.xMax, rect.xMin, ratio);
                            uv.xMin = Mathf.Lerp(uv.xMax, uv.xMin, ratio);
                            break;
                        }
                        break;

                    case FillMethod.Vertical:
                        switch (this.fillOrigin)
                        {
                        case 0:
                            rect.yMax = Mathf.Lerp(rect.yMin, rect.yMax, ratio);
                            uv.yMax = Mathf.Lerp(uv.yMin, uv.yMax, ratio);
                            break;

                        case 1:
                            rect.yMin = Mathf.Lerp(rect.yMax, rect.yMin, ratio);
                            uv.yMin = Mathf.Lerp(uv.yMax, uv.yMin, ratio);
                            break;
                        }
                        break;
                    }
                }

                toFill.AddVert(new Vector2(rect.xMin, rect.yMax), baseColor, new Vector2(uv.xMin, uv.yMax));
                toFill.AddVert(rect.max, baseColor, uv.max);
                toFill.AddVert(rect.min, baseColor, uv.min);
                toFill.AddVert(new Vector2(rect.xMax, rect.yMin), baseColor, new Vector2(uv.xMax, uv.yMin));

                toFill.AddTriangle(0, 1, 2);
                toFill.AddTriangle(2, 1, 3);
            }
        }


        Vector2 offsetMin;
        Vector2 offsetMax;
        
        Rect uvRect;
        protected override void OnSpriteChanged(Sprite prev)
        {
            var sprite = this.SelectedSprite;
            if (sprite)
            {
                var spriteRect = sprite.rect;
                var textureRect = sprite.textureRect;
                this.offsetMin = sprite.textureRectOffset;
                this.offsetMax = spriteRect.size - textureRect.size - offsetMin;
                
                this.uvRect = sprite.GetUVRect(false);
            }
            else
            {
                this.offsetMin = 
                this.offsetMax = Vector2.zero;
                
                this.uvRect = default(Rect);
            }
        }

        public override bool Cacheable
        {
            get { return false; }
        }
    }
}
