using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
namespace Ext.Unity3D.UI
{
    [AddComponentMenu("UI/Ext/Sprite Animation")]
    public class SpriteAnimation : ManagedComponent
    {
        [SerializeField]
        Graphic target;
        public Graphic Target { get { return this.target; } }

        [SerializeField]
        Graphic[] followers;
        public Graphic[] Followers { get { return this.followers; } }


        [SerializeField]
        SpritesComponent sprites;
        public SpritesComponent Sprites { get { return this.sprites; } }
        public void SetSprites(SpritesComponent sprites)
        {
            this.sprites = sprites;
            this.SetIndex(this.CurrentIndex);
        }
        protected SpriteCache prevSpriteCache = null;

        public bool IsValid { get { return null != this.target && null != this.sprites ? this.sprites.IsValid : false; } }


        [SerializeField]
        protected float currentIndex = 0;

        [SerializeField]
        protected float offsetIndex = 0;
        public float OffsetIndex
        {
            set
            {
                this.offsetIndex = value;
                this.LateUpdate();
            }
            get { return this.offsetIndex; }
        }


        [SerializeField]
        protected bool isCircular = false;


        [SerializeField]
        bool isAutoNativeSize = false;
        public bool IsAutoNativeSize { get { return this.isAutoNativeSize; } }


        [SerializeField]
        bool usePixelRange = false;
        public bool UsePixelRange { get { return this.usePixelRange; } }

        [SerializeField]
        Rect pixelRange;
        public Rect PixelRange
        {
            set
            {
                if (value == this.pixelRange)
                    return;

                this.pixelRange = value;
                this.DoUpdate(this.CurrentSprite);
            }
            get { return this.pixelRange; }
        }


        public int CurrentIndex
        {
            set
            {
                if (!this.sprites)
                {
                    this.currentIndex = value;
                    return;
                }

                int cur = this.CurrentIndex;
                int next = this.isCircular ? value : Mathf.Clamp(value, 0, this.sprites.Last);
                if (cur == next)
                    return;

                this.SetIndex(next);
            }
            get { return (int)this.currentIndex; }
        }

        public int ActualIndex
        {
            get { return (int)(this.currentIndex + this.offsetIndex); }
        }

        void SetIndex(int value)
        {
            this.currentIndex = value;
            this.LateUpdate();
        }

        public int CurrentSpriteIndex
        {
            get
            {
                if (!this.sprites)
                    return -1;

                if (this.isCircular)
                {
                    int count = this.SpriteCount;
                    int index = (this.ActualIndex) % count;
                    if (0 > index)
                        index = index + count;

                    return index;
                }
                else
                    return Mathf.Clamp(this.ActualIndex, 0, this.sprites.Last);
            }
        }

        public SpriteCache CurrentSprite
        {
            get
            {
                var index = this.CurrentSpriteIndex;
                return -1 < index && index < this.SpriteCount ? this.sprites[this.CurrentSpriteIndex] : null;
            }
        }

        public int SpriteCount { get { return this.sprites ? this.sprites.Count : 0; } }


        public float NormalizedIndex
        {
            set
            {
                var count = this.SpriteCount;
                if (0 >= count)
                    return;

                var last = count - 1;
                this.CurrentIndex = (int)(this.currentIndex = Mathf.Clamp01(value) * last);
            }
            get
            {
                var count = this.SpriteCount;
                if (0 >= count)
                    return 0;

                var index = this.currentIndex;
                if (this.isCircular)
                {
                    index %= count;
                    if (0 > index)
                        index = index + count;
                }

                var last = count - 1;
                return index / last;
            }
        }


        [SerializeField]
        bool autoUpdate = true;
        public bool AutoUpdate
        {
            set { this.autoUpdate = value; }
            get { return this.autoUpdate; }
        }

        [SerializeField]
        protected Animation dependAnimation;    // NOTE: Optional
        public Animation DependAnimation
        {
            set
            {
                this.dependAnimation = value;
            }
            get
            {
                return this.dependAnimation;
            }
        }

        public void LateUpdate()
        {
            if (!this.sprites)
                return;

#if !TEST_AUTO_LAYOUT
            if (!this.autoUpdate)
                return;
#endif// !TEST_AUTO_LAYOUT

            if (this.dependAnimation && !this.dependAnimation.isPlaying)
                return;

            var sc = this.CurrentSprite;
            if (this.prevSpriteCache != sc)
                this.DoUpdate(sc);
        }

        [SerializeField]
        bool autoVisible = false;
        void SetAutoVisible(Graphic g, bool valid)
        {
            if (g)
                g.gameObject.SetActive(valid);
        }

        public void DoUpdate(SpriteCache sc)
        {
            var valid = SpriteCache.IsValid(sc);
            if (valid)
            {
                this.SetSprite(this.target, sc);

                if (null != this.followers)
                {
                    for (int n = 0, cnt = this.followers.Length; n < cnt; ++n)
                        this.SetSprite(this.followers[n], sc);
                }
            }

            if (this.autoVisible)
            {
                this.SetAutoVisible(this.target, valid);

                if (null != this.followers)
                {
                    for (int n = 0, cnt = this.followers.Length; n < cnt; ++n)
                        this.SetAutoVisible(this.followers[n], valid);
                }
            }

            this.prevSpriteCache = sc;
        }

        Vector2 cachedVec2 = Vector2.zero;
        void SetSprite(Graphic g, SpriteCache sc)
        {
            if (!g)
                return;

            g.TrySetSprite(sc);

            if (this.isAutoNativeSize)
                g.SetNativeSize();

            if (this.usePixelRange)
            {
                var uv = sc.UVRect;
                var w = uv.width;
                var h = uv.height;
                uv.xMin += w * this.pixelRange.xMin;
                uv.xMax -= w * this.pixelRange.xMax;
                uv.yMin += h * this.pixelRange.yMin;
                uv.yMax -= h * this.pixelRange.yMax;

                g.TrySetUV((Rect)uv);

                var trans = g.rectTransform;
                var rect = trans.rect;
                var size = rect.size;
                w = size.x;
                h = size.y;
                var min = trans.offsetMin;
                var vec2 = this.cachedVec2;
                vec2.Set(w * this.pixelRange.xMin, h * this.pixelRange.yMin);
                trans.offsetMin += vec2;
                vec2.Set(w * this.pixelRange.xMin, h * this.pixelRange.yMin);
                trans.offsetMax -= vec2;
                //trans.offsetMin += new Vector2(w * this.pixelRange.xMin, h * this.pixelRange.yMin);
                //trans.offsetMax -= new Vector2(w * this.pixelRange.xMax, h * this.pixelRange.yMax);
            }
        }


#if UNITY_EDITOR
        public void EditorSetSprites(SpritesComponent sprs)
        {
            this.sprites = sprs;
        }

        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();

            if (!this.target)
                this.target = this.GetComponent<Graphic>();

            if (!this.sprites)
                this.sprites = this.GetComponent<SpritesComponent>();

            if (null == this.sprites || 0 == this.sprites.Count)
                this.currentIndex = 0;
        }

        protected override void OnEditorTestingLooped()
        {
            base.OnEditorTestingLooped();
            this.LateUpdate();
        }
#endif// UNITY_EDITOR
    }
}