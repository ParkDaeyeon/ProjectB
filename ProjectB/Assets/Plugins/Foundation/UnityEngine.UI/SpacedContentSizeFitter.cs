using Ext.Unity3D;
using Ext.Unity3D.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
/*
started Unity 5.4.2p4

Since the RectSize of UI Text with using components of LetterSpacing and ContentSizeFitter,
you couldn't get size exactly without Left-Alignment of UI Text.
Because of spaced text, by the LetterSpacing can't resizing Rect.
*/
namespace UnityEngine.UI
{
    public class SpacedContentSizeFitter : ContentSizeFitter
    {
        [SerializeField]
        Text text;
        [SerializeField]
        LetterSpacing letterSpacing;
        protected override void Awake()
        {
            base.Awake();

            this.text = this.GetComponent<Text>();
            this.letterSpacing = this.GetComponent<LetterSpacing>();
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            this.SetLayoutHorizontal();
            this.SetLayoutVertical();
            this.SetDirty();
        }
#endif// UNITY_EDITOR

        float Spacing { get { return this.letterSpacing ? this.letterSpacing.spacing : 0f; } }

        void ResizeRect(bool width = false, bool height = false)
        {
            if (!width && !height)
                return;

            var text = this.text;
            if (!text)
                return;

            var delta = this.Spacing != 0f && text.text.Length != 0 ? (this.Spacing * (float)text.fontSize / 100f) * (text.text.Length - 1) : 0;
            var rectTF = text.rectTransform;
            var sizeDelta = rectTF.sizeDelta;
            sizeDelta.x = width ? text.preferredWidth + delta : sizeDelta.x;
            sizeDelta.y = height ? text.preferredHeight : sizeDelta.y;
            rectTF.sizeDelta = sizeDelta;
        }

        public override void SetLayoutHorizontal()
        {
            switch (this.horizontalFit)
            {
            case FitMode.PreferredSize:
                this.ResizeRect(true, false);
                break;
            default:
                base.SetLayoutHorizontal();
                break;
            }
        }

        public override void SetLayoutVertical()
        {
            switch (this.verticalFit)
            {
            case FitMode.PreferredSize:
                this.ResizeRect(false, true);
                break;
            default:
                base.SetLayoutVertical();
                break;
            }
        }
    }
}
