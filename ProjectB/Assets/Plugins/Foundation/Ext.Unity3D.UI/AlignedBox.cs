using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
namespace Ext.Unity3D.UI
{
    public abstract class AlignedBox : ManagedUIComponent
    {
        protected enum Layout
        {
            Horizontal,
            Vertical
        }
        [SerializeField]
        protected Layout layout;
        public bool IsHorizontal
        {
            get
            {
                return this.layout == Layout.Horizontal;
            }
        }
        protected enum Direction
        {
            Forward,
            Backward,
        }
        [SerializeField]
        protected Direction direction;
        public bool IsForward
        {
            get
            {
                return this.direction == Direction.Forward;
            }
        }

        //CachedRectTransform
        public Vector2 Size
        {
            get
            {
                return this.CachedRectTransform ? this.CachedRectTransform.rect.size : Vector2.zero;
            }
        }

        //레이아웃에 따라 다른 값이 적용된다.
        float Length
        {
            set
            {
                if (this.IsHorizontal)
                    this.CachedRectTransform.SetWidth(value);
                else
                    this.CachedRectTransform.SetHeight(value);
            }
            get
            {
                return this.IsHorizontal ?
                        this.CachedRectTransform.GetWidth() :
                        this.CachedRectTransform.GetHeight();
            }
        }

        public virtual float ActualLength
        {
            get
            {
                var count = this.Count;
                if (0 >= count)
                    return 0;

                var last = count - 1;

                float length = 0;
                if (this.IsForward)
                {
                    for (int n = 0; n < count; ++n)
                    {
                        length += this.ActualLengthOfElemnt(n);
                    }
                }
                else
                {
                    for (int n = count - 1; n >= 0; --n)
                    {
                        length += this.ActualLengthOfElemnt(n);
                    }
                }
                length -= this.ActualLengthOfElemnt(last);

                return length;
            }
        }
        protected virtual float ActualLengthOfElemnt(int index)
        {
            return this.GetLengthAt(index) + this.GetSpacingAt(index); ;
        }


        //AlignBase

        public enum ALIGN
        {
            LeftOrBottom,
            Center,
            RightOrTop,
            Fixed,
        }
        [SerializeField]
        protected ALIGN align = ALIGN.LeftOrBottom;
        public ALIGN Align
        {
            set { this.align = value; }
            get { return this.align; }
        }
        [SerializeField]
        RectTransform alignBase;
        public RectTransform AlignBase
        {
            get { return this.alignBase; }
        }
        [SerializeField]
        protected Vector2 alignOffset;// Anchored Position
        public Vector2 AlignOffset
        {
            set { this.alignOffset = value; }
            get { return this.alignOffset; }
        }

        [SerializeField]
        float spacing = 0;
        public float Spacing { get { return this.spacing; } }
        public virtual float GetSpacingAt(int index)
        {
            return this.spacing;
        }




        public static void UpdateAlignmentAndLayout(AlignedBox box, IEnumerable<ILayoutController> layouts)
        {
            if (null != layouts)
            {
                var enumerator = layouts.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var layout = enumerator.Current;
                    if (null == layout)
                        continue;

                    layout.SetLayoutHorizontal();
                    layout.SetLayoutVertical();
                }
            }

            if (box)
                box.UpdateAlignment();
        }
        public virtual void UpdateAlignment()
        {
            var length = this.ActualLength;

            if (ALIGN.Fixed == this.align)
            {
                this.UpdateBestFit();
                return;
            }

            if (this.autoFit)
                this.Length = length;

            var scaleScalar = this.UpdateBestFit();

            var emptySpace = this.Length - (length * scaleScalar);

            var pos = this.alignBase.anchoredPosition;

            float alignPos = ALIGN.LeftOrBottom == this.align ? -emptySpace :
                             ALIGN.Center == this.align ? -emptySpace * 0.5f :
                             0;

            pos =   this.IsHorizontal ?
                    new Vector2(alignPos, 0) : //pos.x 변경
                    new Vector2(0, alignPos);  //pos.y 변경

            this.alignBase.anchoredPosition = pos + this.alignOffset;
        }
        protected virtual void UpdateAlignmentAt(int index) { }

        //Elements

        [SerializeField]
        List<UICache> elems;
        public List<UICache> Elements
        {
            get { return this.elems; }
        }
        public int ElementsCount
        {
            get { return this.elems.Count; }
        }
        public virtual int Count
        {
            get { return this.ElementsCount; }
        }
        public virtual int AvailableElementCount
        {
            get { return this.Count; }
        }

        #region Element Collection Methods / AlignedBox 동작과 크게 관련이 없는 함수들
        UICache GetElement(int index)
        {
            if (!this.IsAvailableElemIndex(index))
                return null;

            return this.elems[index];
        }

        public bool IsAvailableElemIndex(int index)
        {
            return -1 < index && index < this.ElementsCount;
        }

        public UICache this[int index]
        {
            get { return this.GetElement(index); }
        }

        public int FindElement(UICache elem)
        {
            for (int n = 0, cnt = this.ElementsCount; n < cnt; ++n)
            {
                if (elem == this.elems[n])
                    return n;
            }

            return -1;
        }
        public int FindElement(GameObject go)
        {
            for (int n = 0, cnt = this.ElementsCount; n < cnt; ++n)
            {
                if (go == this.elems[n].GameObject)
                    return n;
            }

            return -1;
        }
        public int FindElement(Transform t)
        {
            for (int n = 0, cnt = this.ElementsCount; n < cnt; ++n)
            {
                if (t == this.elems[n].CachedTransform)
                    return n;
            }

            return -1;
        }

        public bool ContainsElement(UICache elem)
        {
            return -1 != this.FindElement(elem);
        }
        public bool ContainsElement(GameObject go)
        {
            return -1 != this.FindElement(go);
        }
        public bool ContainsElement(Transform t)
        {
            return -1 != this.FindElement(t);
        }

        public bool AddElement(UICache elem)
        {
            if (this.ContainsElement(elem))
                return false;

            this.elems.Add(elem);
            return true;
        }

        public bool InsertElement(int index, UICache elem)
        {
            if (this.ContainsElement(elem))
                return false;

            if (0 > index)
                index = 0;
            if (index >= this.ElementsCount)
                this.elems.Add(elem);
            else
                this.elems.Insert(index, elem);

            return true;
        }

        public bool RemoveElement(UICache elem)
        {
            return this.RemoveElementAt(this.FindElement(elem));
        }
        public bool RemoveElementAt(int index)
        {
            if (0 > index || index >= this.ElementsCount)
                return false;

            this.elems.RemoveAt(index);
            return true;
        }

        public void ClearElements()
        {
            this.elems.Clear();
        }

        #endregion

        public virtual float GetLengthAt(int index)
        {
            if (-1 < index && index < this.elems.Count)
            {
                var element = this.elems[index].CachedRectTransform;
                var length = this.IsHorizontal ? element.rect.size.x : element.rect.size.y;
                return length;
            }
            return 0;

        }


        protected virtual void SetupPositions()
        {
            if (ALIGN.Fixed == this.align)
                return;

            var alignBase = this.AlignBase;
            if (alignBase)
            {
                this.SetRightTopAnchor(alignBase);   //right top
                alignBase.anchoredPosition = Vector2.zero;
                alignBase.sizeDelta = Vector2.zero;
            }

            bool isHorizontal = this.IsHorizontal;
            if (this.IsForward)
            {
                for (int n = 0, cnt = this.ElementsCount; n < cnt; ++n)
                {
                    var element = this.elems[n];

                    this.SetRightTopAnchor(element.CachedRectTransform);//right top

                    var anchorPos = element.CachedRectTransform.anchoredPosition;

                    if (isHorizontal)
                        anchorPos.x = 0;
                    else
                        anchorPos.y = 0;

                    element.CachedRectTransform.anchoredPosition = anchorPos;
                }
            }
            else
            {
                for (int n = this.ElementsCount - 1, cnt = 0; n >= cnt; --n)
                {
                    var element = this.elems[n];

                    this.SetRightTopAnchor(element.CachedRectTransform);//right top

                    var anchorPos = element.CachedRectTransform.anchoredPosition;

                    if (isHorizontal)
                        anchorPos.x = 0;
                    else
                        anchorPos.y = 0;

                    element.CachedRectTransform.anchoredPosition = anchorPos;
                }
            }
        }

        protected void SetLeftBottomAnchor(RectTransform t)
        {
            this.SetAnchor(t, 0);
        }
        protected void SetRightTopAnchor(RectTransform t)
        {
            this.SetAnchor(t, 1);
        }
        protected void SetAnchor(RectTransform t, int value)
        {
            var pivot = t.pivot;
            var anchorMin = t.anchorMin;
            var anchorMax = t.anchorMax;

            if (this.IsHorizontal)
            {
                pivot.x = value;
                anchorMin.x = value;
                anchorMax.x = value;
            }
            else
            {
                pivot.y = value;
                anchorMin.y = value;
                anchorMax.y = value;
            }

            t.pivot = pivot;
            t.anchorMin = anchorMin;
            t.anchorMax = anchorMax;
        }

        [SerializeField]
        bool autoFit = false;
        public bool AutoFit
        {
            set { this.autoFit = value; }
            get { return this.autoFit; }
        }

        [SerializeField]
        bool bestFit;
        public bool BestFit
        {
            set { this.bestFit = value; }
            get { return this.bestFit; }
        }
        float UpdateBestFit()
        {
            if (this.bestFit)
            {
                var length = this.Length;
                var actualLength = this.ActualLength;
                if (0 < length && length < actualLength)
                {
                    var scale = Vector3.one;
                    var scaleScalar = length / actualLength;
                    scale.x =
                    scale.y = scaleScalar;
                    this.alignBase.localScale = scale;

                    return scaleScalar;
                }
            }

            this.alignBase.localScale = Vector3.one;
            return 1;
        }

        public void ShowElems(int count, bool value)
        {
            if (this.IsForward)
            {
                for (int n = 0; n < count; ++n)
                {
                    this.ShowElement(n, value);
                }
            }
            else
            {
                for (int n = count - 1; n >= 0; --n)
                {
                    this.ShowElement(n, value);
                }
            }
        }
        public void ShowElement(int index, bool value)
        {
            var element = this.elems[index];
            if (element)
            {
                if (value)
                    element.CachedTransform.ShowTransformVer2();
                else
                    element.CachedTransform.HideTransformVer2();
            }
        }

        [SerializeField]
        bool autoVisible;
        public bool AutoVisible
        {
            set { this.autoVisible = value; }
            get { return this.autoVisible; }
        }
        void OnEnable()
        {
            if (this.autoVisible)
                this.ShowElems(this.ElementsCount, true);
            this.Rebuild();
        }
        void OnDisable()
        {
            if (this.autoVisible)
                this.ShowElems(this.ElementsCount, false);
        }

        public void Rebuild()
        {
            if (!this.enabled)
                return;

            this.OnRebuild();
        }

        protected virtual void OnRebuild() { }
#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();
        }

        protected void EditorResetElements(List<UICache> elems)
        {
            this.elems = elems;
        }

        protected void EditorResetAlignBase(RectTransform rectTrans)
        {
            this.alignBase = rectTrans;
        }


        public static void EditorReset(AlignedBox box, IEnumerable<Transform> elems)
        {
            if (!box)
                return;

            box.OnEditorReset(elems);
        }

        protected virtual void OnEditorReset(IEnumerable<Transform> elems)
        {
            this.ClearElements();
            foreach (Transform child in elems)
                this.AddElement(new UICache(this.gameObject));
        }



        public static void EditorResetByAlignBase(AlignedBox box, Transform alignBase)
        {
            if (!box)
                return;

            box.OnEditorResetByAlignBase(alignBase);
        }

        protected virtual void OnEditorResetByAlignBase(Transform alignedBase)
        {
            List<Transform> childrens = new List<Transform>();
            foreach (Transform child in alignedBase)
                childrens.Add(child);

            this.OnEditorReset(childrens);
        }
#endif// UNITY_EDITOR
    }
}