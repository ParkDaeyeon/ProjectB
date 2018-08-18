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
    [AddComponentMenu("UI/Ext/Gauge")]
    [ExecuteInEditMode]
    public sealed class Gauge : OptimizedFilledImage
    {
        [SerializeField]
        RectTransform tail;
        public RectTransform Tail
        {
            set { this.tail = value; }
            get { return this.tail; }
        }


        [SerializeField]
        float tailOffsetMin;
        public float TailOffsetMin
        {
            set
            {
                if (this.tailOffsetMin == value)
                    return;

                this.tailOffsetMin = value;
                this.UpdateTail();
            }
            get { return this.tailOffsetMin; }
        }

        [SerializeField]
        float tailOffsetMax;
        public float TailOffsetMax
        {
            set
            {
                if (this.tailOffsetMax == value)
                    return;

                this.tailOffsetMax = value;
                this.UpdateTail();
            }
            get { return this.tailOffsetMax; }
        }

        public float TailRange
        {
            get { return 1 - (this.tailOffsetMin + this.tailOffsetMax); }
        }


        [SerializeField]
        bool alwaysShowTail;
        public bool AlwaysShowTail
        {
            set
            {
                if (value == this.alwaysShowTail)
                    return;

                this.alwaysShowTail = value;
                this.UpdateTail();
            }
            get { return this.alwaysShowTail; }
        }


        [SerializeField]
        bool alwaysShowTailEnd;
        public bool AlwaysShowTailEnd
        {
            set
            {
                if (value == this.alwaysShowTailEnd)
                    return;

                this.alwaysShowTailEnd = value;
                this.UpdateTail();
            }
            get { return this.alwaysShowTailEnd; }
        }


        [SerializeField]
        bool tailForFillAmount;
        public bool TailForFillAmount
        {
            set
            {
                if (value == this.tailForFillAmount)
                    return;

                this.tailForFillAmount = value;
                this.UpdateTail();
            }
            get { return this.tailForFillAmount; }
        }



        protected override void Update()
        {
            base.Update();
            
            this.UpdateTail();
        }


        void UpdateTail()
        {
            if (!this.tail)
                return;

            var fillAmount = this.FillAmount;
            if (this.alwaysShowTail ||
                (0 < fillAmount && (this.alwaysShowTailEnd || fillAmount < 1)))
            {
                var rectTrans = this.rectTransform;
                
                var tailPos = this.tail.anchoredPosition;
                var fill = this.Fill;
                var range = fill.Range;
                var ratio = this.tailForFillAmount ? this.FillAmount : fill.Ratio;
                var pos = ratio * range;

                var min = this.tailOffsetMin;
                if (min > pos)
                    pos = min;

                var max = range - this.tailOffsetMax;
                if (max < pos)
                    pos = max;

                switch (this.fillMethod)
                {
                case FillMethod.Horizontal:
                    tailPos = new Vector2(pos, tailPos.y);
                    break;

                case FillMethod.Vertical:
                    tailPos = new Vector2(tailPos.x, pos);
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
    }
}
