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
    [AddComponentMenu("UI/Ext/SimpleGauge")]
    public sealed class SimpleGauge : ManagedUIComponent
    {
        [SerializeField]
        OptimizedFilledImage image;
        public OptimizedFilledImage OptImage { get { return this.image; } }

        [SerializeField]
        float paddingStart = 0f;
        public float PaddingStart { get { return this.paddingStart; } }

        [SerializeField]
        float paddingLast = 0f;
        public float PaddingLast { get { return this.paddingLast; } }

        public float Range { get { return 1 - (this.paddingStart + this.paddingLast); } }


        void OnEnable()
        {
            this.inited = true;
            this.UpdateFillAmount();
            this.UpdateTail();
        }

        bool inited = true;
        float value;
        public float Value
        {
            set
            {
                var rate = Mathf.Clamp01(value);
                if (this.value != rate || this.inited)
                {
                    this.inited = false;
                    this.SetValue(rate);
                }
            }
            get
            {
                return this.value;
            }
        }

        public void SetValue(float value)
        {
            this.value = Mathf.Clamp01(value);
            this.UpdateFillAmount();
        }

        public void UpdateFillAmount()
        {
            var fillAmount = 0f;
            if (0 == this.value)
                fillAmount = 0;
            else if (1 == this.value)
                fillAmount = 1;
            else
            {
                var range = this.Range;
                fillAmount = this.paddingStart + range * this.value;
            }

            this.image.FillAmount = fillAmount;

            this.UpdateTail();
        }

        public float FillAmount
        {
            get { return this.image.FillAmount; }
        }

        public Image.FillMethod GetFillMethod()
        {
            return this.image.fillMethod;
        }

        public Image.OriginVertical GetOriginVertical()
        {
            return (Image.OriginVertical)this.image.fillOrigin;
        }

        public Image.OriginHorizontal GetOriginHorizontal()
        {
            return (Image.OriginHorizontal)this.image.fillOrigin;
        }


        public Vector2 Size
        {
            get { return this.CachedRectTransform.GetSize(); }
        }


        [SerializeField]
        RectTransform tail;
        public RectTransform Tail { get { return this.tail; } }

        [SerializeField]
        bool alwaysShowTail;
        public bool AlwaysShowTail
        {
            set
            {
                if (this.alwaysShowTail != value)
                {
                    this.alwaysShowTail = value;
                    this.UpdateTail();
                }
            }
            get { return this.alwaysShowTail; }
        }

        void UpdateTail()
        {
            if (!this.tail)
                return;

            if (this.alwaysShowTail || (0 < this.Value && this.Value < 1))
            {
                var size = this.Size;
                var fillAmount = this.FillAmount;
                
                var tailPos = this.tail.anchoredPosition;

                // TODO: Scale
                switch (this.GetFillMethod())
                {
                case Image.FillMethod.Horizontal:
                    switch (this.GetOriginHorizontal())
                    {
                    case Image.OriginHorizontal.Left:
                        tailPos = new Vector2(size.x * fillAmount, tailPos.y);
                        break;

                    case Image.OriginHorizontal.Right:
                        tailPos = new Vector2(size.x * (1 - fillAmount), tailPos.y);
                        break;
                    }
                    break;
                case Image.FillMethod.Vertical:
                    switch (this.GetOriginVertical())
                    {
                    case Image.OriginVertical.Bottom:
                        tailPos = new Vector2(tailPos.x, size.y * fillAmount);
                        break;

                    case Image.OriginVertical.Top:
                        tailPos = new Vector2(tailPos.x, size.y * (1 - fillAmount));
                        break;
                    }
                    break;
                }

                this.tail.anchoredPosition = tailPos;

                var tailGo = this.tail.gameObject;
                if (!tailGo.activeSelf)
                    tailGo.SetActive(true);
            }
            else
            {
                var tailGo = this.tail.gameObject;
                if (tailGo.activeSelf)
                    tailGo.SetActive(false);
            }
        }

#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();
            
            this.image = this.GetComponent<OptimizedFilledImage>();
        }

        [SerializeField]
        float editorTestValue;
        protected override void OnEditorTesting()
        {
            base.OnEditorTesting();
            this.SetValue(this.editorTestValue);
        }

        protected override void OnEditorTestingLooped()
        {
            base.OnEditorTestingLooped();
            this.Value = this.editorTestValue;
        }
#endif// UNITY_EDITOR
    }
}
