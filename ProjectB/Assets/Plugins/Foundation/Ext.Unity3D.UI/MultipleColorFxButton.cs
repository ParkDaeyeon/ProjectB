using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Ext.Unity3D.UI
{
    public class MultipleColorFxButton : MultipleColorButton
    {
        [SerializeField]
        bool disableWithFX = true;

        [SerializeField]
        List<Graphic> fxFollowerReference;
        public List<Graphic> FxFollowersRef
        {
            set { this.fxFollowerReference = value; }
            get { return this.fxFollowerReference; }
        }

        protected override void ColorTween(SelectionState state, Color targetColor, float duration)
        {
            base.ColorTween(state, targetColor, duration);

            this.SetVisble(state, targetColor, duration);
        }

        void SetVisble(SelectionState state, Color targetColor, float duration)
        {
            if (!this.targetGraphic)
                return;

            var reference = this.fxFollowerReference;
            if (null == reference) return;

            for (int n = 0, cnt = reference.Count; n < cnt; ++n)
            {
                var graphic = reference[n];
                if (!graphic)
                    continue;

                if (this.disableWithFX)
                {
                    graphic.gameObject.SetActive(state != SelectionState.Disabled);
                }
            }
        }
    }
}