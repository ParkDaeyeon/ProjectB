using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
namespace Ext.Unity3D.UI
{
    public class MultipleColorButton : Button
    {
        List<Graphic> followersReference;
        public List<Graphic> FollowersRef
        {
            set { this.followersReference = value; }
            get { return this.followersReference; }
        }

        void Collect()
        {
            this.targetGraphic = this.image;
        }

        protected override void Awake()
        {
            base.Awake();

            this.Collect();
        }
        
        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            switch (this.transition)
            {
            case Transition.ColorTint:
                var colors = this.colors;
                Color color;
                switch (state)
                {
                case SelectionState.Normal:
                    color = colors.normalColor;
                    break;
                case SelectionState.Highlighted:
                    color = colors.highlightedColor;
                    break;
                case SelectionState.Pressed:
                    color = colors.pressedColor;
                    break;
                case SelectionState.Disabled:
                    color = colors.disabledColor;
                    break;
                default:
                    color = Color.black;
                    break;
                }
                var duration = instant ? 0f : this.colors.fadeDuration;

                this.ColorTween(state, color * colors.colorMultiplier, duration);
                break;
            }
        }

        protected virtual void ColorTween(SelectionState state, Color targetColor, float duration)
        {
            if (!this.targetGraphic)
            {
                this.Collect();
                if (!this.targetGraphic)
                    return;
            }

            base.targetGraphic.CrossFadeColor(targetColor, duration, true, true);

            var reference = this.followersReference;
            if (null == reference)
                return;

            for (int n = 0, cnt = reference.Count; n < cnt; ++n)
            {
                var graphic = reference[n];
                if (!graphic)
                    continue;

                graphic.CrossFadeColor(targetColor, duration, true, true);
            }
        }
    }
}
