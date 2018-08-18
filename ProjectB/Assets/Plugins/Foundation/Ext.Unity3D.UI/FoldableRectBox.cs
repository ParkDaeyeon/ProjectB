using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
namespace Ext.Unity3D.UI
{
    public class FoldableRectBox : RectBox
    {
        [SerializeField]
        bool useFold;
        bool IsFoldable
        {
            get
            {
                if(useFold)
                    return this.Count < this.dataCount;
                else
                    return false;
            }
        }

        [SerializeField]
        int count;
        public override int Count
        {
            get { return this.count; }
        }
        [SerializeField]
        int dataCount;
        public int DataCount
        {
            set { this.dataCount = value; }
            get { return this.dataCount; }
        }

        public int FoldingCount
        {
            get
            {
                if (this.IsFoldable)
                    return Mathf.Abs(this.Count - (this.dataCount + 1));
                else
                    return 0;
            }
        }

        [SerializeField]
        ManagedUIComponent foldBase;
        [SerializeField]
        Text foldCountText;
        [SerializeField]
        string format = "{0}";
        public string Format
        {
            set { this.format = value; }
            get { return this.format; }
        }

        public override void UpdateAlignment()
        {
            base.UpdateAlignment();

            this.SetElemStateAll(false);
            if (this.count > this.dataCount)
                this.count = this.dataCount;

            if (this.count > this.ElementsCount)
                this.count = this.ElementsCount;

            if (this.count < 1) return;

            for (int n = 0, cnt = this.count - 1; n < cnt; ++n)
            {
                this.SetElemState(n, true);
            }

            int lastIndex = this.Count - 1;
            if (IsFoldable)
            {
                this.foldBase.SetActive(true);

                var foldPos = this.foldBase.CachedTransform.localPosition;
                var pos = this.IsHorizontal ?
                            new Vector2(this.ActualLength, foldPos.y) :
                            new Vector2(foldPos.x, this.ActualLength);

                pos.y = this.IsForward ?
                        pos.y :
                        pos.y * -1;

                this.foldBase.CachedTransform.localPosition = pos;

                this.foldCountText.text = string.Format(this.format, this.FoldingCount);
            }
            else
            {
                if (this.foldBase == null)
                    return;

                this.SetElemState(lastIndex, true);

                this.foldBase.SetActive(false);
            }
        }

#if UNITY_EDITOR

        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();

            this.count = this.ElementsCount;

            this.foldBase = this.FindComponent<ManagedUIComponent>("FoldBase");
            this.foldCountText = this.FindComponent<Text>("FoldBase/Text");
        }
#endif// UNITY_EDITOR
    }
}
