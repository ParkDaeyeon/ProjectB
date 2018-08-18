using UnityEngine;
using UnityEngine.UI;

namespace Ext.Unity3D.UI
{
    public class ScrollStalker : ManagedUIComponent
    {
        [SerializeField]
        ScrollRect scrollRect;
        public ScrollRect ScrollRect
        {
            set { this.scrollRect = value; }
            get { return this.scrollRect; }
        }
        public bool Scrollable
        {
            get
            {
                if (!this.scrollRect)
                    return false;
                
                var viewport = this.scrollRect.viewport;
                if (!viewport)
                    return false;

                var content = this.scrollRect.content;
                if (!content)
                    return false;

                //Debug.Log("name:" + this.name + ", viewport:" + viewport.rect + ", content:" + content.rect);
                return this.horizontal ? viewport.GetWidth() < content.GetWidth() : viewport.GetHeight() < content.GetHeight();
            }
        }
        public float NormalizedPosition
        {
            set
            {
                if (!this.scrollRect)
                    return;

                if (this.horizontal)
                    this.scrollRect.horizontalNormalizedPosition = value;
                else
                    this.scrollRect.verticalNormalizedPosition = value;
            }
            get
            {
                if (!this.scrollRect)
                    return 0;
                
                return this.horizontal ? this.scrollRect.horizontalNormalizedPosition : this.scrollRect.verticalNormalizedPosition;
            }
        }


        [SerializeField]
        bool horizontal;
        public bool Horizontal
        {
            set { this.horizontal = value; }
            get { return this.horizontal; }
        }
        
        [SerializeField]
        bool autoHide;
        public bool AutoHide
        {
            set { this.autoHide = value; }
            get { return this.autoHide; }
        }

        [SerializeField]
        RectTransform bar;
        public RectTransform Bar
        {
            set { this.bar = value; }
            get { return this.bar; }
        }
        
        [SerializeField]
        RectTransform thumb;
        public RectTransform Thumb
        {
            set { this.thumb = value; }
            get { return this.thumb; }
        }

        void OnEnable()
        {
            this.Refresh(false);
        }

        int lastScrollable = -1;
        float lastPosition = float.MinValue;

        public void Refresh(bool update = true)
        {
            this.lastScrollable = -1;
            this.lastPosition = float.MinValue;
            if (update)
                this.LateUpdate();
        }
        
        void LateUpdate()
        {
            if (!this.scrollRect || !this.thumb)
                return;

            if (this.autoHide)
            {
                var scrollable = this.Scrollable;
                var scrollablei = (scrollable ? 1 : 0);
                if (this.lastScrollable != scrollablei)
                {
                    this.lastScrollable = scrollablei;
                    if (this.bar)
                        this.bar.gameObject.SetActive(scrollable);
                    this.thumb.gameObject.SetActive(scrollable);
                }
            }

            var normalizePosition = this.NormalizedPosition;
            if (this.lastPosition != normalizePosition)
                this.thumb.anchoredPosition = Vector2.Lerp(this.from, this.to, this.lastPosition = normalizePosition);
        }

        [SerializeField]
        Vector2 from;
        public Vector2 From
        {
            set { this.from = value; }
            get { return this.from; }
        }

        [SerializeField]
        Vector2 to;
        public Vector2 To
        {
            set { this.to = value; }
            get { return this.to; }
        }


#if UNITY_EDITOR
        protected override void OnEditorTestingLooped()
        {
            base.OnEditorTestingLooped();
            this.Refresh();
        }
#endif// UNITY_EDITOR
    }
}
