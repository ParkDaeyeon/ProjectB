using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Ext.Unity3D.UI
{
    public class EventSensitiveScrollRect : MonoBehaviour
    {
        private static float SCROLL_MARGIN = 0.3f; // how much to "overshoot" when scrolling, relative to the selected item's height

        private ScrollRect sr;


        public void Awake()
        {
            this.sr = this.gameObject.GetComponent<ScrollRect>();
        }

        bool oneFrame = false;
        void Update()
        {
            if (this.oneFrame)
                return;

            this.oneFrame = true;

            // helper vars
            float contentHeight = this.sr.content.rect.height;
            float viewportHeight = this.sr.viewport.rect.height;

            var dd = this.transform.parent.GetComponent<Dropdown>();
            if (!dd)
                return;
            
            var selectedIndex = dd.value;
            if (0 > selectedIndex || selectedIndex > dd.options.Count)
                return;

            var content = this.sr.content;
            var c = content.GetChild(selectedIndex + 1);
            if (!c)
                return;

            var selected = c.transform as RectTransform;

            // what bounds must be visible?
            float centerLine = selected.localPosition.y; // selected item's center
            float upperBound = centerLine + (selected.rect.height / 2f); // selected item's upper bound
            float lowerBound = centerLine - (selected.rect.height / 2f); // selected item's lower bound

            // what are the bounds of the currently visible area?
            float lowerVisible = (contentHeight - viewportHeight) * this.sr.normalizedPosition.y - contentHeight;
            float upperVisible = lowerVisible + viewportHeight;

            // is our item visible right now?
            float desiredLowerBound;
            if (upperBound > upperVisible)
            {
                // need to scroll up to upperBound
                desiredLowerBound = upperBound - viewportHeight + selected.rect.height * SCROLL_MARGIN + viewportHeight / 2;
            }
            else if (lowerBound < lowerVisible)
            {
                // need to scroll down to lowerBound
                desiredLowerBound = lowerBound - selected.rect.height * SCROLL_MARGIN - viewportHeight / 2;
            }
            else if (-(viewportHeight / 2) > centerLine)
            {
                // to center
                desiredLowerBound = lowerBound - selected.rect.height * SCROLL_MARGIN - viewportHeight / 2;
            }
            else
            {
                // item already visible - all good
                return;
            }

            // normalize and set the desired viewport
            float normalizedDesired = (desiredLowerBound + contentHeight) / (contentHeight - viewportHeight);
            this.sr.normalizedPosition = new Vector2(0f, Mathf.Clamp01(normalizedDesired));
        }
    }
}
