using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
namespace Ext.Unity3D.UI
{
    [AddComponentMenu("UI/Ext/Sprite Animated Gauge")]
    public class SpriteAnimatedGauge : SpritesComponent
    {
        public override bool UsePixelOffset { get { return this.enablePixelOffset; } }

        public override Rect GetPixelOffset(int index) { return this.pixelOffset; }


        [SerializeField]
        Gauge gauge;
        public Gauge Gauge { get { return this.gauge; } }
        public float FillAmount
        {
            set { this.gauge.FillAmount = value; }
            get { return this.gauge.FillAmount; }
        }


        [SerializeField]
        bool enablePixelOffset = false;
        public bool EnablePixelOffset
        {
            set { this.enablePixelOffset = value; }
            get { return this.enablePixelOffset; }
        }
        
        [SerializeField]
        Rect pixelOffset = new Rect(0, 0, 0, 0);
        public Rect PixelOffset
        {
            set
            {
                if (this.pixelOffset != value)
                {
                    this.pixelOffset = value;
                    this.ResetPixelOffset();
                }
            }
            get { return this.pixelOffset; }
        }
        

        [SerializeField]
        float oneFrameSeconds = 1 / 60f;
        public float OneFrameSeconds { set { this.oneFrameSeconds = value; } get { return this.oneFrameSeconds; } }

        [SerializeField]
        int animationStart = 0;
        public int AnimationStart { set { this.animationStart = value; } get { return this.animationStart; } }

        [SerializeField]
        int animationCount = 2;
        public int AnimationCount { set { this.animationCount = value; } get { return this.animationCount; } }
        public int AnimationLast { get { return Mathf.Min(this.Last, 0 < this.animationCount ? (this.animationStart + this.animationCount) - 1 : this.animationStart); } }

        [SerializeField]
        bool loop = true;
        public bool Loop { set { this.loop = value; } get { return this.loop; } }

        [SerializeField]
        int currentIndex = 0;
        public int CurrentIndex
        {
            set
            {
                this.deltaSum = 0;
                this.SetFrame(value, true);
            }
            get { return this.currentIndex; }
        }


        public float NormalizedTime
        {
            set
            {
                if (1 > this.animationCount)
                    return;

                value = Mathf.Clamp01(value);
                var indexf = this.animationStart + this.animationCount * value;
                this.currentIndex = (int)indexf;

                var remain = indexf - this.currentIndex;
                this.deltaSum = this.oneFrameSeconds * remain;

                this.SetFrame(this.currentIndex, true);

                if (1 == value && !this.loop)
                    this.MarkAsEnd();
            }
            get
            {
                if (1 > this.animationCount)
                    return 1;

                var normal = (this.currentIndex - this.animationStart) / this.animationCount;
                var normalOneFrame = 1 / this.animationCount;
                var remain = normalOneFrame * Mathf.Clamp01(this.deltaSum / this.oneFrameSeconds);
                
                var normalIndex = Mathf.Clamp01(normal + remain);
                if (this.loop && 1 == normalIndex)
                    return 0.99999999f;

                return normalIndex;
            }
        }
        



        bool isPlaying = false;
        public bool IsPlaying { get { return this.isPlaying; } }

        float deltaSum = 0;
        public void Play(int startIdx = -1)
        {
            this.Stop();
            this.isPlaying = true;

            if (-1 == startIdx)
                startIdx = this.animationStart;

            this.SetFrame(startIdx, true);
        }

        public void Stop()
        {
            this.isPlaying = false;
            this.deltaSum = 0;
        }

        void MarkAsEnd()
        {
            this.isPlaying = false;
            this.deltaSum = this.oneFrameSeconds + 0.1f;
        }
        


        [SerializeField]
        int pausedIndex = -1;
        public int PausedIndex { set { this.pausedIndex = value; } get { return this.pausedIndex; } }


        protected override void Update()
        {
            base.Update();

            if (!this.isPlaying)
                return;

            if (0 == this.Count || 1 >= this.animationCount)
                return;

            if (-1 < this.pausedIndex && this.pausedIndex < this.animationCount)
            {
                this.SetFrame(this.pausedIndex);
                return;
            }

            this.UpdateFrame(Time.deltaTime);
        }

        protected virtual void UpdateFrame(float deltaTime)
        {
            this.deltaSum += deltaTime;
            if (this.deltaSum < this.oneFrameSeconds)
                return;

            this.deltaSum = 0;

            int idx = this.currentIndex + 1;
            this.SetFrame(idx);
        }

        public void SetFrame(int idx, bool force = false)
        {
            var last = this.AnimationLast;
            if (idx > last)
            {
                if (this.loop)
                {
                    idx = Mathf.Max(0, this.animationStart);
                }
                else if (this.currentIndex != last)
                {
                    idx = last;
                }
                else
                {
                    this.MarkAsEnd();
                    return;
                }
            }

            if (null == this.Elements || 0 > idx || idx > this.Last)
                return;

            if (!force && idx == this.currentIndex)
                return;
            
            SpriteCache sc = this.sprites[this.currentIndex = idx];
            if (!sc)
                return;
            
            if (!this.gauge)
                return;

            this.gauge.sprite = sc;
        }


#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();

            if (!this.gauge)
                this.gauge = this.FindComponent<Gauge>();
            
            if (this.gauge && !this.gauge.Caches)
                this.gauge.Caches = this;

            this.SetFrame(-1 != this.pausedIndex ? this.pausedIndex : this.animationStart, true);
            this.Stop();
        }
        
        [SerializeField]
        float editorTestRealtime;
        protected override void OnEditorTesting()
        {
            base.OnEditorTesting();
            this.editorTestRealtime = Time.realtimeSinceStartup;
            this.Play();
        }

        protected override void OnEditorTestingLooped()
        {
            base.OnEditorTestingLooped();

            var now = Time.realtimeSinceStartup;
            var deltaTime = now - this.editorTestRealtime;
            this.editorTestRealtime = now;
            this.UpdateFrame(deltaTime);
        }
#endif// UNITY_EDITOR
    }
}