using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Ext;
using Ext.Unity3D;
using Ext.Unity3D.UI;
using Ext.Event;
using Ext.Async;

using Program.View.Common;

namespace Program.View
{
    public class LoadingIndicator : BaseView
    {
        public const int SCREEN_LOCK = 7775;
        
        [SerializeField]
        UICache introOutro;
        [SerializeField]
        Animation objectAnim;
        
        //public void LockInputGuides(bool @lock)
        //{
        //    InputGuideView.SetLock(@lock, 0x00001000);
        //}

        bool activate;
        bool inOutAction;
        float waitForDelay = 0;
        int generation;
        public static bool IsActive
        {
            get
            {
                var inst = LoadingIndicator.instance;
                if (!inst)
                    return false;

                return inst.activate;
            }
        }

        static void SetActiveWithDelay(bool value, float delay)
        {
            var inst = LoadingIndicator.instance;
            if (!inst)
                return;

            if (value == inst.activate)
                return;

            if (value && 0 < delay)
            {
                if (0 == inst.waitForDelay)
                {
                    inst.waitForDelay = delay;
                    var gen = inst.generation;
                    var signal = new CancellableSignal(() => !inst || gen != inst.generation || inst.IsDone);
                    CoroutineTaskManager.AddTask(inst.OnActiveWithDelay(signal,
                                                                        delay,
                                                                        Time.realtimeSinceStartup));
                }
                else
                {
                    inst.waitForDelay = Mathf.Min(inst.waitForDelay, delay);
                }
            }
            else
                inst.OnActive(value);
        }

        IEnumerator OnActiveWithDelay(CancellableSignal signal, float delay, float prev)
        {
            do
            {
                yield return null;
                if (CancellableSignal.IsCancelled(signal))
                {
                    this.waitForDelay = 0;
                    //Debug.Log("CANCEL");
                    yield break;
                }

                var now = Time.realtimeSinceStartup;
                var delta = now - prev;

                //var old = this.waitForDelay;
                this.waitForDelay = Mathf.Max(0, this.waitForDelay - delta);
                //Debug.Log("DELAY:" + this.waitForDelay + ", OLD:" + old + ", DT:" + delta + ", PREV:" + prev + ", NOW:" + now);
                prev = now;
            }
            while (0 < this.waitForDelay);
            
            this.OnActive(true);
        }

        void OnActive(bool value)
        {
            //Debug.Log("ONACTIVE:" + value + ", OLD:" + this.activate + ", GEN:" + this.generation);
            this.activate = value;
            this.waitForDelay = 0;
            ++this.generation;
            BroadcastTunnel<int, bool>.Notify(LoadingIndicator.SCREEN_LOCK, value);
            //this.LockInputGuides(value);

            if (this.introOutro && this.introOutro.Animation)
            {
                float startTime = 0;
                if (this.inOutAction && this.introOutro.Animation.isPlaying)
                {
                    startTime = 1 - this.introOutro.CurrentAnimState.normalizedTime;

                    if (value)
                    {
                        if (null != this.outroDone)
                            this.outroDone();
                    }
                    else
                    {
                        if (null != this.introDone)
                            this.introDone();
                    }
                }

                this.introOutro.SetCurrentAnimPair(value ? 0 : 1);
                this.introOutro.Animation.ForcePlay(true);
                if (0 < startTime)
                {
                    this.introOutro.CurrentAnimState.normalizedTime = startTime;
                    this.introOutro.Animation.Sample();
                }
                this.inOutAction = true;
                if (value)
                    this.gameObject.SetActive(true);
            }
            else
            {
                this.inOutAction = false;
                this.gameObject.SetActive(value);
            }

            if (value)
                this.objectAnim.ForcePlay(true);
        }
        

        static LoadingIndicator instance;

        protected override void Awake()
        {
            base.Awake();

            LoadingIndicator.instance = this;

            if (this.introOutro && this.introOutro.Animation)
                UICache.OptimizeAnimPairOnetime(this.introOutro);
        }

        protected override void OnDestroy()
        {
            if (this == LoadingIndicator.instance)
                LoadingIndicator.instance = null;

            base.OnDestroy();
        }



        public delegate bool IsDoneCallback();
        LinkedList<IsDoneCallback> list = new LinkedList<IsDoneCallback>();

        Action introDone;
        public static Action IntroDone
        {
            set
            {
                var inst = LoadingIndicator.instance;
                if (!inst)
                    return;

                inst.introDone = value;
            }
            get
            {
                var inst = LoadingIndicator.instance;
                if (!inst)
                    return null;

                return inst.introDone;
            }
        }

        Action outroDone;
        public static Action OutroDone
        {
            set
            {
                var inst = LoadingIndicator.instance;
                if (!inst)
                    return;

                inst.outroDone = value;
            }
            get
            {
                var inst = LoadingIndicator.instance;
                if (!inst)
                    return null;

                return inst.outroDone;
            }
        }

        public static void On(IsDoneCallback callback, float delay = 0)
        {
            if (null == callback)
                throw new Exception("LOADING_INDICATOR:ON:EXCEPT:BAD_CALLBACK");

            var inst = LoadingIndicator.instance;
            if (!inst)
                return;

            inst.list.AddLast(callback);

            LoadingIndicator.SetActiveWithDelay(true, delay);
        }

        public static void Close(bool immediate = false)
        {
            var inst = LoadingIndicator.instance;
            if (!inst)
                return;

            inst.list.Clear();

            if (immediate)
                LoadingIndicator.SetActiveWithDelay(false, 0);
        }
        
        bool IsDone
        {
            get
            {
                var node = this.list.First;
                var nodeTemp = node;
                while (null != node)
                {
                    var isDoneChecker = node.Value;
                    nodeTemp = node;
                    node = node.Next;

                    if (isDoneChecker())
                        this.list.Remove(nodeTemp);
                }
                
                return 0 == this.list.Count;
            }
        }
        
        void Update()
        {
            if (!this.activate)
                return;
            
            if (this.IsDone)
                LoadingIndicator.SetActiveWithDelay(false, 0);
        }

        void LateUpdate()
        {
            if (this.activate)
            {
                if (this.inOutAction)
                {
                    if (this.introOutro.Animation.isPlaying)
                        return;

                    this.inOutAction = false;
                    if (null != this.introDone)
                        this.introDone();
                }
            }
            else if (this.inOutAction)
            {
                if (!this.introOutro.Animation.isPlaying)
                {
                    this.inOutAction = false;
                    if (null != this.outroDone)
                        this.outroDone();

                    this.gameObject.SetActive(false);
                }
            }
        }
#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            if (!this.introOutro)
                this.introOutro = new UICache(this.gameObject);

            if (!this.objectAnim)
                this.objectAnim = this.FindComponent<Animation>("Area/Content");
        }
#endif // UNITY_EDITOR
    }
}