using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using Ext;
using Ext.Unity3D;
using Ext.Unity3D.UI;
using Ext.Event;

namespace Program.View
{
    public class Fade : BaseView
    {
        public const int SCREEN_LOCK = 7772;
        
        public static bool IsActive
        {
            private set
            {
                var inst = Fade.instance;
                if (!inst)
                    return;

                if (value == Fade.IsActive)
                    return;

                inst.gameObject.SetActive(value);

                BroadcastTunnel<int, bool>.Notify(Fade.SCREEN_LOCK, value);
            }
            get
            {
                var inst = Fade.instance;
                if (!inst)
                    return false;

                return inst.gameObject.activeSelf;
            }
        }


        public static bool IsRunning
        {
            get
            {
                var inst = Fade.instance;
                if (!inst)
                    return false;

                return State.None != inst.state;
            }
        }

        public static Color Color
        {
            get
            {
                var inst = Fade.instance;
                if (!inst)
                    return Color.black;

                return inst.fill.color;
            }
        }

        public enum State
        {
            None,
            FadeIn,
            FadeOut,
        }

        State state;
        public static State GetState()
        {
            var inst = Fade.instance;
            if (!inst)
                return State.None;

            return inst.state;
        }

        float timeStart;
        public static float TimeStart
        {
            get
            {
                var inst = Fade.instance;
                if (!inst)
                    return 0;

                return inst.timeStart;
            }
        }

        float timeLength;
        public static float TimeLength
        {
            get
            {
                var inst = Fade.instance;
                if (!inst)
                    return 0;

                return inst.timeLength;
            }
        }


        Action callback;
        public static Action Callback
        {
            get
            {
                var inst = Fade.instance;
                if (!inst)
                    return null;

                return inst.callback;
            }
        }


        static Fade instance;

        protected override void Awake()
        {
            base.Awake();

            Fade.instance = this;
        }

        protected override void OnDestroy()
        {
            if (this == Fade.instance)
                Fade.instance = null;

            base.OnDestroy();
        }


        [SerializeField]
        Graphic fill;

        public static void Out(Color color, float timeLength, Action callback = null)
        {
            var inst = Fade.instance;
            if (!inst)
                return;

            inst.fill.color = color;
            inst.timeStart = Time.realtimeSinceStartup;
            inst.timeLength = timeLength;
            inst.callback = callback;

            inst.state = State.FadeOut;
            Fade.IsActive = true;

            Fade.instance.Update();
        }

        public static void In(float timeLength, Action callback = null)
        {
            if (!Fade.IsActive)
            {
                if (null != callback)
                    callback();

                return;
            }
            
            var inst = Fade.instance;
            if (!inst)
                return;
            
            inst.timeStart = Time.realtimeSinceStartup;
            inst.timeLength = timeLength;
            inst.callback = callback;

            inst.state = State.FadeIn;
            inst.Update();
        }


        public static void Close()
        {
            if (!Fade.IsActive)
                return;
            
            var inst = Fade.instance;
            if (!inst)
                return;
            
            inst.state = State.None;
            inst.callback = null;
            Fade.IsActive = false;
        }

        public static void Complete()
        {
            if (!Fade.IsActive)
                return;
            
            var inst = Fade.instance;
            if (!inst)
                return;

            var state = inst.state;
            inst.state = State.None;

            var callback = inst.callback;
            inst.callback = null;
            
            if (State.FadeIn == state)
                Fade.IsActive = false;

            if (null != callback)
                callback();
        }


        void Update()
        {
            if (State.None == this.state)
                return;

            var deltaTime = Time.realtimeSinceStartup - this.timeStart;
            var progress = 0f;
            if (0 < this.timeLength)
                progress = Mathf.Clamp01(deltaTime / this.timeLength);
            else
                progress = 1f;

            float alpha = State.FadeIn == this.state ? 1f - progress : progress;

            var color = this.fill.color;
            color.a = alpha;
            this.fill.color = color;

            if (1f == progress)
                Fade.Complete();
        }


#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();
            this.fill = this.CachedTransform.GetComponent<Graphic>();
        }
#endif // UNITY_EDITOR
    }
}