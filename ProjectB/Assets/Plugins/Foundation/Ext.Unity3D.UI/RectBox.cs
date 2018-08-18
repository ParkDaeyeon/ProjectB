using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
namespace Ext.Unity3D.UI
{
    public class RectBox : AlignedBox
    {
        [SerializeField]//Vector2로 바꿔야함
        float minUnitWidth = 0;
        public float MinUnitWidth
        {
            set { this.minUnitWidth = value; }
            get { return this.minUnitWidth; }
        }
        [SerializeField]
        float minUnitHeight = 0;
        public float MinUnitHeight
        {
            set { this.minUnitHeight = value; }
            get { return this.minUnitHeight; }
        }

        public override float GetLengthAt(int index)
        {
            var element = this[index];

            float lengthA = this.IsHorizontal ? this.minUnitWidth : this.minUnitHeight;
            float lengthB = this.IsHorizontal ? 
                            element.CachedRectTransform.rect.width * element.CachedRectTransform.localScale.x :
                            element.CachedRectTransform.rect.height * element.CachedRectTransform.localScale.y;
            return Mathf.Max(lengthA, lengthB);
        }

#if UNITY_EDITOR
        [SerializeField]
#endif// UNITY_EDITOR
        float actualLength;
        public override float ActualLength
        {
            get { return this.actualLength; }
        }

        public override void UpdateAlignment()
        {
            this.actualLength = 0f;
            var count = this.Count;

            if (this.IsForward)
            {
                for (int n = count - 1; n >= 0; --n)
                {
                    this.UpdateAlignmentAt(n);
                }
            }
            else
            {
                for (int n = 0; n < count; ++n)
                {
                    this.UpdateAlignmentAt(n);
                }
            }
            this.actualLength -= this.Spacing;

            base.UpdateAlignment();
        }

        protected override void UpdateAlignmentAt(int index)
        {
            base.UpdateAlignmentAt(index);

            var item = this[index];

            if (!this.GetElemState(index))
            {
                item.SetActive(false);
                return;
            }

            item.SetActive(true);

            if(null == item.CachedRectTransform)
            {
#if LOG_DEBUG
                Debug.LogWarningFormat("[RectBox.UpdateAlignmentAt] Transform is Null ! / Index {0} / OwnerName {1}", index, this.name);
#endif // LOG_DEBUG
                return;
            }

            var pos = item.CachedRectTransform.anchoredPosition;

            if (this.IsHorizontal)
                pos.x = -this.actualLength;
            else
                pos.y = -this.actualLength;

            item.CachedRectTransform.anchoredPosition = pos;

            this.actualLength += this.GetLengthAt(index) + this.Spacing;
        }
        



        [SerializeField]
        List<bool> elemStates = new List<bool>();

        void AutoResizeElemStateBuffer()
        {
            var elemCount = this.ElementsCount;
            while (elemCount > this.elemStates.Count)
                this.elemStates.Add(true);
        }

        public void SetElemState(int elemIndex, bool state)
        {
            if (!this.IsAvailableElemIndex(elemIndex))
                return;

            this.AutoResizeElemStateBuffer();
            this.elemStates[elemIndex] = state;
        }

        public void SetElemStateRange(int elemIndex, int count, bool state)
        {
            if (!this.IsAvailableElemIndex(elemIndex))
                return;

            this.AutoResizeElemStateBuffer();

            for (int n = elemIndex, cnt = elemIndex + count; n < cnt; ++n)
            {
                if (!this.IsAvailableElemIndex(n))
                    break;

                this.elemStates[n] = state;
            }
        }

        public void SetElemStateAll(bool state)
        {
            this.SetElemStateRange(0, this.ElementsCount, state);
        }

        public bool GetElemState(int elemIndex)
        {
            if (!this.IsAvailableElemIndex(elemIndex))
                return false;

            this.AutoResizeElemStateBuffer();
            return this.elemStates[elemIndex];
        }

        public void ClearElementStates()
        {
            this.elemStates.Clear();
        }


        [SerializeField]
        bool autoAlignment;
        public bool AutoAlignment
        {
            set { this.autoAlignment = value; }
            get { return this.autoAlignment; }
        }

        void LateUpdate()
        {
#if !TEST_AUTO_LAYOUT
            if (!this.autoAlignment)
                return;
#endif// !TEST_AUTO_LAYOUT

            this.UpdateAlignment();
        }


#if UNITY_EDITOR
        [SerializeField]
        bool editorAutoCollect = false;

        [SerializeField]
        bool editorUpdateLayout = false;

        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();

            if (this.editorAutoCollect)
                this.EditorAutoCollect();

            this.EditorCheckForMissing();

            this.SetupPositions();

            this.AutoResizeElemStateBuffer();

            if (this.editorUpdateLayout)
                this.UpdateAlignment();
        }


        public void EditorAutoCollect()
        {
            this.ClearElements();
            this.ClearElementStates();
            var alignBase = this.AlignBase;
            if (!alignBase)
                return;

            var rectTrans = this.CachedRectTransform;
            for (int n = 0, cnt = alignBase.childCount; n < cnt; ++n)
            {
                var rect = alignBase.GetChild(n) as RectTransform;
                if (!rect)
                    continue;

                if (rectTrans == rect)
                    continue;

                this.AddElement(new UICache(rect.gameObject));
            }
        }



        /// <summary>
        /// For SharedStatus
        /// </summary>
        void EditorCheckForMissing()
        {
            var alignBase = this.AlignBase;
            if (alignBase && 0 < this.ElementsCount)
            {
                var cache = this.Elements[0];
                if (!cache)
                {
                    AlignedBox.EditorResetByAlignBase(this, alignBase);
                }
            }
        }
#endif// UNITY_EDITOR
    }
}
