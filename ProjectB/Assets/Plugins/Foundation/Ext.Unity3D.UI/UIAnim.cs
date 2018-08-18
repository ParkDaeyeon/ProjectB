using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace Ext.Unity3D.UI
{
    public class UIAnim : MonoBehaviour
    {
        public static float GetSysTime()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying)
                return Time.time;

            return Time.realtimeSinceStartup;
#else// UNITY_EDITOR
            return Time.time;
#endif// UNITY_EDITOR
        }

        public enum Formula
        {
            AnchoredXY,
            AnchoredX,
            AnchoredY,
            ScaleXY,
            ScaleX,
            ScaleY,
            RotateXYZ,
            RotateXY,
            RotateXZ,
            RotateYZ,
            RotateX,
            RotateY,
            RotateZ,
            ColorRGBA,
            ColorRGB,
            ColorRGA,
            ColorRBA,
            ColorRG,
            ColorRB,
            ColorRA,
            ColorGB,
            ColorGA,
            ColorBA,
            ColorR,
            ColorG,
            ColorB,
            ColorA,
            Index,
            Sprite,
            Extra,
            Anim,
            FillAmount,
            CanvasA,
        }

        [Serializable]
        public class Curve
        {
            [SerializeField]
            Formula formula;
            public Formula Formula
            {
                set { this.formula = value; }
                get { return this.formula; }
            }
            
            [SerializeField]
            RectTransform transform;
            public RectTransform Transform
            {
                set { this.transform = value; }
                get { return this.transform; }
            }

            [SerializeField]
            Graphic graphic;
            public Graphic Graphic
            {
                set { this.graphic = value; }
                get { return this.graphic; }
            }

            [SerializeField]
            Image image;
            public Image Image
            {
                set { this.Image = value; }
                get { return this.Image; }
            }

            [SerializeField]
            CanvasGroup canvasGroup;
            public CanvasGroup CanvasGroup
            {
                set { this.canvasGroup = value; }
                get { return this.canvasGroup; }
            }

            [SerializeField]
            IndexableComponent indexable;
            public IndexableComponent Indexable
            {
                set { this.indexable = value; }
                get { return this.indexable; }
            }

            [SerializeField]
            SpriteAnimation spa;
            public SpriteAnimation Spa
            {
                set { this.spa = value; }
                get { return this.spa; }
            }

            [SerializeField]
            int customId;
            public int CustomId
            {
                set { this.customId = value; }
                get { return this.customId; }
            }

            [SerializeField]
            UIAnim subAnim;
            public UIAnim SubAnim
            {
                set { this.subAnim = value; }
                get { return this.subAnim; }
            }

            [Serializable]
            public class Point
            {
                [SerializeField]
                Interpolator interpolator;
                public Interpolator Interpolator
                {
                    set { this.interpolator = value; }
                    get { return this.interpolator; }
                }
                
                [SerializeField]
                float value;
                public float Value
                {
                    set { this.value = value; }
                    get { return this.value; }
                }

                [SerializeField]
                float time;
                public float Time
                {
                    set { this.time = value; }
                    get { return this.time; }
                }
            }

            [SerializeField]
            List<Point> points;
            public List<Point> Points
            {
                get { return this.points; }
            }
            
            public int Count { get { return null != this.points ? this.points.Count : 0; } }
            public int Last { get { return Mathf.Max(0, this.Count - 1); } }
            public bool Available { get { return 0 < this.Count; } }
            public Point this[int index] { get { return -1 < index && index < this.Count ? this.points[index] : null; } }
            public Point FirstPoint { get { return this[0]; } }
            public Point LastPoint { get { return this[this.Count - 1]; } }
            public float StartTime
            {
                get
                {
                    var point = this.FirstPoint;
                    return null != point ? point.Time : 0;
                }
            }
            public float LastTime
            {
                get
                {
                    var point = this.LastPoint;
                    return null != point ? point.Time : 0;
                }
            }

            public bool Contains(float time)
            {
                return this.Available && this.StartTime <= time && time <= this.LastTime;
            }


            int position;
            bool done;
            public bool Done
            {
                get { return this.done; }
            }

            void ClearStatus()
            {
                this.done = false;
                this.isCaptured = false;
            }
            public void Rewind()
            {
                this.position = 0;
                this.ClearStatus();

                if (this.subAnim)
                    this.subAnim.Rewind();
            }
            public void FastFoward()
            {
                this.position = this.Last;
                this.ClearStatus();

                if (this.subAnim)
                    this.subAnim.FastFoward();
            }
            public void Seek(float time)
            {
                if (time < this.StartTime)
                {
                    this.position = 0;
                }
                else if (time > this.LastTime)
                {
                    this.position = this.Last;
                }
                else
                {
                    var current = this[this.position];
                    if (null != current)
                    {
                        if (current.Time < time)
                            this.UpdateFoward(time);
                        else if (current.Time > time)
                            this.UpdateCurrent(time);
                    }
                    else
                    {
                        this.position = this.BinarySearch(time);
                    }
                }

                this.ClearStatus();
                if (this.subAnim)
                {
                    for (int n = 0, cnt = this.subAnim.Count; n < cnt; ++n)
                        this.subAnim[n].ClearStatus();
                }
            }

            public int BinarySearch(float time)
            {
                int low = 0;
                int high = this.points.Count - 1;
                int middle = (low + high + 1) / 2;
                int location = -1;

                do
                {
                    var point = this.points[middle];
                    if (time < point.Time)
                        high = middle - 1;
                    else if (time > point.Time)
                        low = middle + 1;
                    else
                    {
                        location = middle;
                        break;
                    }

                    middle = (low + high + 1) / 2;

                } while ((low <= high) && (location == -1));

                return location;
            }


            public bool JumpTo(float amount)
            {
                var current = this[this.position];
                if (null != current)
                {
                    if (0 < amount)
                        this.UpdateFoward(current.Time + amount);
                    else if (0 > amount)
                        this.UpdateCurrent(current.Time + amount);

                    this.ClearStatus();
                    if (this.subAnim)
                    {
                        for (int n = 0, cnt = this.subAnim.Count; n < cnt; ++n)
                            this.subAnim[n].ClearStatus();
                    }
                    return true;
                }

                return false;
            }
            void UpdateFoward(float time)
            {
                do
                {
                    var next = this[this.position + 1];
                    if (null == next || next.Time > time)
                        return;

                    ++this.position;
                }
                while (this.position < this.Last);
            }
            void UpdateCurrent(float time)
            {
                do
                {
                    var current = this[this.position];
                    if (null == current || current.Time <= time)
                        return;

                    --this.position;
                }
                while (this.position > -1);
            }
            void UpdateBackward(float time)
            {
                do
                {
                    var prev = this[this.position - 1];
                    if (null == prev || prev.Time < time)
                        return;

                    --this.position;
                }
                while (this.position > -1);
            }

            public bool Sample(UIAnim anim, float time, bool foward)
            {
                if (foward)
                    this.UpdateFoward(time);
                else
                    this.UpdateBackward(time);

                return this.Capture(anim, time, foward);
            }

            bool isCaptured = false;
            float prevValue;
            public bool Capture(UIAnim anim, float time, bool foward)
            {
                var current = this[this.position];
                if (null == current)
                {
                    this.done = true;
                    return false;
                }

                var next = foward ? this[this.position + 1] : this[this.position - 1];
                if (null == next)
                {
                    if (time >= current.Time)
                    {
                        this.DoUpdate(anim, current.Value);
                        this.done = true;
                    }
                    return false;
                }

                if (!foward)
                {
                    var temp = current;
                    current = next;
                    next = temp;
                }

                var start = current.Time;
                var end = next.Time;
                var duration = end - start;

                var delta = 0 >= duration ? 1 :
                            foward ? Mathf.Clamp01((time - start) / duration) :
                                     1 - Mathf.Clamp01((time - start) / duration);

                var interpolate = current.Interpolator;
                var t = null != interpolate ? interpolate.Interpolate(delta) : delta;

                var from = current.Value;
                var to = null != next ? next.Value : from;

                var value = Mathf.Lerp(from, to, t);
                this.DoUpdate(anim, value);
                return true;
            }

            void DoUpdate(UIAnim anim, float value)
            {
                if (!this.isCaptured || this.prevValue != value)
                    this.OnUpdate(anim, value);

                this.isCaptured = true;
                this.prevValue = value;
            }

            void OnUpdate(UIAnim anim, float value)
            {
                switch (this.formula)
                {
                case Formula.AnchoredXY:    this.OnUpdateAnchoredXY (value); break;
                case Formula.AnchoredX:     this.OnUpdateAnchoredX  (value); break;
                case Formula.AnchoredY:     this.OnUpdateAnchoredY  (value); break;
                case Formula.ScaleXY:       this.OnUpdateScaleXY    (value); break;
                case Formula.ScaleX:        this.OnUpdateScaleX     (value); break;
                case Formula.ScaleY:        this.OnUpdateScaleY     (value); break;
                case Formula.RotateXYZ:     this.OnUpdateRotateXYZ  (value); break;
                case Formula.RotateXY:      this.OnUpdateRotateXY   (value); break;
                case Formula.RotateXZ:      this.OnUpdateRotateXZ   (value); break;
                case Formula.RotateYZ:      this.OnUpdateRotateYZ   (value); break;
                case Formula.RotateX:       this.OnUpdateRotateY    (value); break;
                case Formula.RotateY:       this.OnUpdateRotateY    (value); break;
                case Formula.RotateZ:       this.OnUpdateRotateZ    (value); break;
                case Formula.ColorRGBA:     this.OnUpdateColorRGBA  (value); break;
                case Formula.ColorRGB:      this.OnUpdateColorRGB   (value); break;
                case Formula.ColorRGA:      this.OnUpdateColorRGA   (value); break;
                case Formula.ColorRBA:      this.OnUpdateColorRBA   (value); break;
                case Formula.ColorRG:       this.OnUpdateColorRG    (value); break;
                case Formula.ColorRB:       this.OnUpdateColorRB    (value); break;
                case Formula.ColorRA:       this.OnUpdateColorRA    (value); break;
                case Formula.ColorGB:       this.OnUpdateColorGB    (value); break;
                case Formula.ColorGA:       this.OnUpdateColorGA    (value); break;
                case Formula.ColorBA:       this.OnUpdateColorBA    (value); break;
                case Formula.ColorR:        this.OnUpdateColorR     (value); break;
                case Formula.ColorG:        this.OnUpdateColorG     (value); break;
                case Formula.ColorB:        this.OnUpdateColorB     (value); break;
                case Formula.ColorA:        this.OnUpdateColorA     (value); break;
                case Formula.Index:         this.OnUpdateIndex      (value); break;
                case Formula.Sprite:        this.OnUpdateSprite     (value); break;
                case Formula.Extra:         this.OnUpdateExtra      (value, anim); break;
                case Formula.Anim:          this.OnUpdateAnim       (value); break;
                case Formula.FillAmount:    this.OnUpdateFillAmount (value); break;
                case Formula.CanvasA:       this.OnUpdateCanvasA    (value); break;
                }

#if UNITY_EDITOR
                if (!UnityEditor.EditorApplication.isPlaying)
                {
                    if (this.transform)
                        UnityEditor.EditorUtility.SetDirty(this.transform);
                    if (this.graphic)
                        UnityEditor.EditorUtility.SetDirty(this.graphic);
                    if (this.indexable)
                        UnityEditor.EditorUtility.SetDirty(this.indexable);
                    if (this.spa)
                        UnityEditor.EditorUtility.SetDirty(this.spa);
                }
#endif// UNITY_EDITOR
            }

            void OnUpdateAnchoredXY(float value)
            {
                if (!this.transform)
                    return;

                var v = this.transform.anchoredPosition;
                v.x = v.y = value;
                this.transform.anchoredPosition = v;
            }

            void OnUpdateAnchoredX(float value)
            {
                if (!this.transform)
                    return;

                var v = this.transform.anchoredPosition;
                v.x = value;
                this.transform.anchoredPosition = v;
            }

            void OnUpdateAnchoredY(float value)
            {
                if (!this.transform)
                    return;

                var v = this.transform.anchoredPosition;
                v.y = value;
                this.transform.anchoredPosition = v;
            }

            void OnUpdateScaleXY(float value)
            {
                if (!this.transform)
                    return;

                var v = this.transform.localScale;
                v.x = v.y = value;
                this.transform.localScale = v;
            }

            void OnUpdateScaleX(float value)
            {
                if (!this.transform)
                    return;

                var v = this.transform.localScale;
                v.x = value;
                this.transform.localScale = v;
            }

            void OnUpdateScaleY(float value)
            {
                if (!this.transform)
                    return;

                var v = this.transform.localScale;
                v.y = value;
                this.transform.localScale = v;
            }

            void OnUpdateRotateXYZ(float value)
            {
                if (!this.transform)
                    return;

                var v = this.transform.localRotation;
                v.x = v.y = v.z = value;
                this.transform.localRotation = v;
            }

            void OnUpdateRotateXY(float value)
            {
                if (!this.transform)
                    return;

                var v = this.transform.localRotation;
                v.x = v.y = value;
                this.transform.localRotation = v;
            }

            void OnUpdateRotateXZ(float value)
            {
                if (!this.transform)
                    return;

                var v = this.transform.localRotation;
                v.x = v.z = value;
                this.transform.localRotation = v;
            }

            void OnUpdateRotateYZ(float value)
            {
                if (!this.transform)
                    return;

                var v = this.transform.localRotation;
                v.y = v.z = value;
                this.transform.localRotation = v;
            }

            void OnUpdateRotateX(float value)
            {
                if (!this.transform)
                    return;

                var v = this.transform.localRotation;
                v.x = value;
                this.transform.localRotation = v;
            }

            void OnUpdateRotateY(float value)
            {
                if (!this.transform)
                    return;

                var v = this.transform.localRotation;
                v.y = value;
                this.transform.localRotation = v;
            }

            void OnUpdateRotateZ(float value)
            {
                if (!this.transform)
                    return;

                var v = this.transform.localRotation;
                v.z = value;
                this.transform.localRotation = v;
            }

            void OnUpdateColorRGBA(float value)
            {
                if (!this.graphic)
                    return;

                var v = this.graphic.color;
                v.r = v.g = v.b = v.a = value;
                this.graphic.color = v;
                this.graphic.SetVerticesDirty();
            }

            void OnUpdateColorRGB(float value)
            {
                if (!this.graphic)
                    return;

                var v = this.graphic.color;
                v.r = v.g = v.b = value;
                this.graphic.color = v;
                this.graphic.SetVerticesDirty();
            }
            void OnUpdateColorRGA(float value)
            {
                if (!this.graphic)
                    return;

                var v = this.graphic.color;
                v.r = v.g = v.a = value;
                this.graphic.color = v;
                this.graphic.SetVerticesDirty();
            }

            void OnUpdateColorRBA(float value)
            {
                if (!this.graphic)
                    return;

                var v = this.graphic.color;
                v.r = v.b = v.a = value;
                this.graphic.color = v;
                this.graphic.SetVerticesDirty();
            }


            void OnUpdateColorRG(float value)
            {
                if (!this.graphic)
                    return;

                var v = this.graphic.color;
                v.r = v.g = value;
                this.graphic.color = v;
                this.graphic.SetVerticesDirty();
            }

            void OnUpdateColorRB(float value)
            {
                if (!this.graphic)
                    return;

                var v = this.graphic.color;
                v.r = v.b = value;
                this.graphic.color = v;
                this.graphic.SetVerticesDirty();
            }

            void OnUpdateColorRA(float value)
            {
                if (!this.graphic)
                    return;

                var v = this.graphic.color;
                v.r = v.a = value;
                this.graphic.color = v;
                this.graphic.SetVerticesDirty();
            }

            void OnUpdateColorGB(float value)
            {
                if (!this.graphic)
                    return;

                var v = this.graphic.color;
                v.g = v.b = value;
                this.graphic.color = v;
                this.graphic.SetVerticesDirty();
            }

            void OnUpdateColorGA(float value)
            {
                if (!this.graphic)
                    return;

                var v = this.graphic.color;
                v.g = v.a = value;
                this.graphic.color = v;
                this.graphic.SetVerticesDirty();
            }

            void OnUpdateColorBA(float value)
            {
                if (!this.graphic)
                    return;

                var v = this.graphic.color;
                    v.b = v.a = value;
                this.graphic.color = v;
                this.graphic.SetVerticesDirty();
            }
            
            void OnUpdateColorR(float value)
            {
                if (!this.graphic)
                    return;

                var v = this.graphic.color;
                v.r = value;
                this.graphic.color = v;
                this.graphic.SetVerticesDirty();
            }

            void OnUpdateColorG(float value)
            {
                if (!this.graphic)
                    return;

                var v = this.graphic.color;
                v.g = value;
                this.graphic.color = v;
                this.graphic.SetVerticesDirty();
            }

            void OnUpdateColorB(float value)
            {
                if (!this.graphic)
                    return;

                var v = this.graphic.color;
                v.b = value;
                this.graphic.color = v;
                this.graphic.SetVerticesDirty();
            }

            void OnUpdateColorA(float value)
            {
                if (!this.graphic)
                    return;

                var v = this.graphic.color;
                v.a = value;
                this.graphic.color = v;
                this.graphic.SetVerticesDirty();
            }

            void OnUpdateIndex(float value)
            {
                if (!this.indexable)
                    return;

                this.indexable.Index = (int)value;
            }

            void OnUpdateSprite(float value)
            {
                if (!this.spa)
                    return;

                this.spa.CurrentIndex = (int)value;
            }

            void OnUpdateExtra(float value, UIAnim anim)
            {
                var listener = anim.OnExtraListener;
//#if LOG_DEBUG
//                Debug.Log(string.Format("UIANIM:ON_UPDATE_EXTRA:{0}, CID:{1}, VAL:{2}, POS:{3}, PREV:{4}, IS_CAP:{5}",
//                                         anim,
//                                         customId,
//                                         value,
//                                         this.position,
//                                         this.prevValue,
//                                         this.isCaptured));
//#endif// LOG_DEBUG
                if (null != listener)
                    listener(anim, this.customId, value);
            }

            void OnUpdateAnim(float value)
            {
                if (!this.subAnim)
                    return;

                this.subAnim.NormalizedTime = value;
                this.subAnim.Sample();
            }

            void OnUpdateFillAmount(float value)
            {
                if (!this.image)
                    return;

                this.image.fillAmount = value;
            }

            void OnUpdateCanvasA(float value)
            {
                if (!this.canvasGroup)
                    return;

                this.canvasGroup.alpha = value;
            }
        }

        [SerializeField]
        List<Curve> curves;
        public List<Curve> Curves
        {
            get { return this.curves; }
        }
        public int Count { get { return null != this.curves ? this.curves.Count : 0; } }
        public int Last { get { return Mathf.Max(0, this.Count - 1); } }
        public bool Available { get { return 0 < this.Count; } }
        public Curve this[int index] { get { return -1 < index && index < this.Count ? this.curves[index] : null; } }

        public void OptimizeInterpolator()
        {
            for (int n = 0, cnt = this.curves.Count; n < cnt; ++n)
            {
                var curve = this.curves[n];
                for (int n2 = 0, cnt2 = curve.Count; n2 < cnt2; ++n2)
                {
                    var p = curve[n2];
                    p.Interpolator.Apply();
                }
            }
        }


        [SerializeField]
        float totalDuration;
        public float TotalDuration
        {
            get { return this.totalDuration; }
        }
        void UpdateTotalDuration()
        {
            var max = 0f;
            for (int n = 0, cnt = this.curves.Count; n < cnt; ++n)
                max = Mathf.Max(max, this.curves[n].LastTime);

            this.totalDuration = max;
        }


        [SerializeField]
        float speed = 1;
        public float Speed
        {
            set
            {
                //if (value == this.speed)
                //    return;

                //this.Update();
                this.speed = value;
                this.anchorTime = UIAnim.GetSysTime() - this.currentTime;
            }
            get { return this.speed; }
        }

        bool playing;
        public bool IsPlaying { get { return this.playing; } }

        bool paused;
        public bool IsPaused { get { return this.IsPlaying && this.paused; } }

        float anchorTime;
        float currentTime;
        public float CurrentTime
        {
            set
            {
                if (value == this.currentTime)
                    return;

                this.currentTime = value;
                this.anchorTime = UIAnim.GetSysTime() - value;

                for (int n = 0, cnt = this.curves.Count; n < cnt; ++n)
                    this.curves[n].Seek(value);
            }
            get { return this.currentTime; }
        }


        public float NormalizedTime
        {
            set { this.CurrentTime = Mathf.Clamp01(value) * this.totalDuration; }
            get { return Mathf.Clamp01(this.currentTime / this.totalDuration); }
        }

        [SerializeField]
        bool loop = false;
        public bool Loop
        {
            set { this.loop = value; }
            get { return this.loop; }
        }

        [SerializeField]
        bool pingpong = false;
        public bool PingPong
        {
            set { this.pingpong = value; }
            get { return this.pingpong; }
        }

        public void Rewind()
        {
            this.currentTime = 0;
            for (int n = 0, cnt = this.curves.Count; n < cnt; ++n)
                this.curves[n].Rewind();

            this.foward = true;
            this.paused = false;
        }

        public void FastFoward()
        {
            this.currentTime = this.totalDuration;
            for (int n = 0, cnt = this.curves.Count; n < cnt; ++n)
                this.curves[n].FastFoward();

            this.paused = false;
        }

        public void RewindPlay()
        {
            this.Stop();
            this.Rewind();
            this.Sample();
            this.Play();
        }

        public void Play()
        {
            if (!this.playing)
            {
                this.Rewind();
                this.anchorTime = UIAnim.GetSysTime();
            }

            this.playing = true;

            if (this.paused)
            {
                this.paused = false;
                this.anchorTime = UIAnim.GetSysTime() - this.currentTime;
            }
        }

        public void Pause()
        {
            if (!this.playing || this.paused)
                return;

            this.paused = true;
        }

        public void RewindStop()
        {
            this.Stop();
            this.Rewind();
            this.Sample();
        }

        public void Stop()
        {
            this.playing = false;
            this.paused = false;

            if (this.animStopCallback != null)
                this.animStopCallback();
        }

        Action animStopCallback;
        public void SetStopedListener(Action callback)
        {
            this.animStopCallback = callback;
        }

        bool ChangePingpong(float sysTime)
        {
            var count = this.currentTime / this.totalDuration;
            var counti = (int)count;
            var mod = counti % 2;
            
            if (1 == mod)
                this.foward = !this.foward;

            if (!this.loop)
            {
                if (this.foward || 1 < counti)
                {
                    this.Rewind();
                    this.currentTime = 0;
                    this.Sample();
                    this.Stop();
                    return false;
                }
            }
            
            if (this.foward)
                this.Rewind();
            else
                this.FastFoward();

            var delta = count - counti;
            this.currentTime = delta * this.totalDuration;
            this.anchorTime = sysTime - this.currentTime;
            this.Sample();

            return true;
        }

        public void Update()
        {
            if (!this.playing)
                return;

            if (this.paused)
                return;

            if (this.totalDuration == 0)
            {
                this.Stop();
                return;
            }
            var sysTime = UIAnim.GetSysTime();
            
            this.currentTime = (sysTime - this.anchorTime) * this.speed;

            if (this.totalDuration <= this.currentTime)
            {
                if (this.pingpong)
                {
                    this.ChangePingpong(sysTime);
                }
                else if (this.loop)
                {
                    var currentTime = this.currentTime % this.totalDuration;
                    this.Rewind();
                    this.currentTime = currentTime;
                    this.Sample();
                }
                else
                {
                    this.currentTime = this.totalDuration;
                    this.Sample();
                    this.Stop();
                }
            }
            else
                this.Sample();
        }

        bool foward = true;
        public void Sample()
        {
            for (int n = 0, cnt = this.curves.Count; n < cnt; ++n)
            {
                var curve = this.curves[n];
                if (curve.Done)
                    continue;
                
                curve.Sample(this, this.currentTime, this.foward);
            }
        }

        public void Capture()
        {
            for (int n = 0, cnt = this.curves.Count; n < cnt; ++n)
            {
                var curve = this.curves[n];
                if (curve.Done)
                    continue;

                curve.Capture(this, this.currentTime, this.foward);
            }
        }



        Action<UIAnim, int, float> onExtra;
        public Action<UIAnim, int, float> OnExtraListener
        {
            set { this.onExtra = value; }
            get { return this.onExtra; }
        }



        [SerializeField]
        bool playAutomatically;
        public bool PlayAutomatically { set { this.playAutomatically = value; } get { return this.playAutomatically; } }

        void OnEnable()
        {
            if (this.playAutomatically || this.IsPaused)
                this.Play();
        }

        void OnDisable()
        {
            if (this.IsPlaying)
                this.Pause();
        }


#if UNITY_EDITOR
        [SerializeField]
        bool editorAutoSetup = false;
        [SerializeField]
        bool editorAutoTargetSelf = false;
        [SerializeField]
        bool editorTestPlay = false;
        [SerializeField]
        bool editorTestPause = false;
        [SerializeField]
        bool editorTestStop = false;
        [SerializeField]
        float editorTestTime = 0;
        [SerializeField]
        bool editorTestSampleByTime = false;
        [SerializeField]
        float editorTestNormalizedTime = 0;
        [SerializeField]
        bool editorTestSampleByNormalizedTime = false;

        private void OnDrawGizmos()
        {
            if (this.editorAutoSetup)
            {
                this.editorAutoSetup = false;
                this.EditorSetting();
            }

            if (this.editorTestPlay)
            {
                this.editorTestPlay = false;
                this.Play();
                UnityEditor.EditorApplication.update -= this.DoPreview;
                UnityEditor.EditorApplication.update += this.DoPreview;
            }
            if (this.editorTestPause)
            {
                this.editorTestPause = false;
                this.Pause();
                UnityEditor.EditorApplication.update -= this.DoPreview;
            }
            if (this.editorTestStop)
            {
                this.editorTestStop = false;
                this.Stop();
                this.Rewind();
                this.Sample();
                this.editorTestTime = 0;
                this.editorTestNormalizedTime = 0;
                UnityEditor.EditorApplication.update -= this.DoPreview;
            }
            if (this.editorTestSampleByTime)
            {
                this.editorTestSampleByTime = false;
                this.CurrentTime = this.editorTestTime;
                this.Sample();
            }
            if (this.editorTestSampleByNormalizedTime)
            {
                this.editorTestSampleByNormalizedTime = false;
                this.NormalizedTime = this.editorTestNormalizedTime;
                this.Sample();
            }
        }

        void DoPreview()
        {
            this.Update();

            this.editorTestTime = this.CurrentTime;
            this.editorTestNormalizedTime = this.NormalizedTime;

            if (!this.IsPlaying)
                UnityEditor.EditorApplication.update -= this.DoPreview;
        }


        public void EditorSetting()
        {
            if (!this)
                return;
            
            this.UpdateTotalDuration();
            if (this.editorAutoTargetSelf)
            {
                for (int n = 0, cnt = this.curves.Count; n < cnt; ++n)
                {
                    var curve = this.curves[n];
                    switch (curve.Formula)
                    {
                    case Formula.AnchoredXY:
                    case Formula.AnchoredX:
                    case Formula.AnchoredY:
                    case Formula.ScaleXY:
                    case Formula.ScaleX:
                    case Formula.ScaleY:
                    case Formula.RotateXYZ:
                    case Formula.RotateXY:
                    case Formula.RotateXZ:
                    case Formula.RotateYZ:
                    case Formula.RotateX:
                    case Formula.RotateY:
                    case Formula.RotateZ:
                        if (!curve.Transform)
                            curve.Transform = this.GetComponent<RectTransform>();
                        break;

                    case Formula.ColorRGBA:
                    case Formula.ColorRGB:
                    case Formula.ColorRGA:
                    case Formula.ColorRBA:
                    case Formula.ColorRG:
                    case Formula.ColorRB:
                    case Formula.ColorRA:
                    case Formula.ColorGB:
                    case Formula.ColorGA:
                    case Formula.ColorBA:
                    case Formula.ColorR:
                    case Formula.ColorG:
                    case Formula.ColorB:
                    case Formula.ColorA:
                        if (!curve.Graphic)
                            curve.Graphic = this.GetComponent<Graphic>();
                        break;

                    case Formula.Index:
                        if (!curve.Indexable)
                            curve.Indexable = this.GetComponent<IndexableComponent>();
                        break;

                    case Formula.Sprite:
                        if (!curve.Spa)
                            curve.Spa = this.GetComponent<SpriteAnimation>();
                        break;

                    case Formula.Extra:
                        break;

                    case Formula.Anim:
                        if (!curve.SubAnim)
                            curve.SubAnim = this.GetComponent<UIAnim>();
                        break;

                    case Formula.FillAmount:
                        if (!curve.Image)
                            curve.Image = this.GetComponent<Image>();
                        break;

                    case Formula.CanvasA:
                        if (!curve.CanvasGroup)
                            curve.CanvasGroup = this.GetComponent<CanvasGroup>();
                        break;
                    }
                }
            }
            
            if (this.transform)
            {
                UnityExtension.SetDirtyAll(this.transform);
            }
            else
            {
                Debug.LogWarning(string.Format("UIANIMATOR_BAD_TRANSFORM:{0}", this.name));
            }
        }
#endif// UNITY_EDITOR
    }
}
