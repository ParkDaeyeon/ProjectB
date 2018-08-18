using UnityEngine;
using System;
using System.Collections.Generic;

namespace Ext.Unity3D
{
    public class AnimationController : ManagedComponent
    {
        [SerializeField]
        Animation anim;
        public Animation Animation
        {
            set { this.anim = value; }
            get { return this.anim; }
        }
        
        public void ResetPlay()
        {
            if (!this.anim)
                return;

            this.SetActive(true);
            this.anim.Rewind();
            this.anim.Sample();
			this.anim.Play();
        }

        [SerializeField]
        bool autoHide;
        public bool AutoHide
        {
            set { this.autoHide = value; }
            get { return this.autoHide; }
        }

        [SerializeField]
        bool autoInactive;
        public bool AutoInactive
        {
            set { this.autoInactive = value; }
            get { return this.autoInactive; }
        }
        
        void Update()
        {
            if (!this.anim)
                return;

            var playing = this.anim.isPlaying;

            if (this.autoHide)
                this.SetShow(playing);

            if (this.autoInactive)
                this.SetActive(playing);
            
            if (!playing)
            {
                if (null != this.onCompleteListener)
                    this.onCompleteListener();
            }
        }

        void OnEnable()
        {
            if (null != this.onEnabledListener)
                this.onEnabledListener();
        }
        
        event Action onEnabledListener;
        public event Action EnabledListener
        {
            add { this.onEnabledListener += value; }
            remove { this.onEnabledListener -= value; }
        }
        



        event Action onCompleteListener;
        public event Action CompleteListener
        {
            add { this.onCompleteListener += value; }
            remove { this.onCompleteListener -= value; }
        }



        public void OnEvent(int value)
        {
            if (null != this.onEventListener)
                this.onEventListener(value);
        }

        event Action<int> onEventListener;
        public event Action<int> OnEventListener
        {
            add { this.onEventListener += value; }
            remove { this.onEventListener -= value; }
        }



        public void OnEventAtIndex(int index)
        {
            var evt = this.GetEvent(index);
            if (null != evt)
                evt();
        }

        List<Action> eventList = new List<Action>();
        public void AddEvent(Action evt)
        {
            this.eventList.Add(evt);
        }
        public void RemoveAllEvent()
        {
            this.eventList.Clear();
        }
        public Action GetEvent(int index)
        {
            if (index < 0 || this.eventList.Count <= index)
                return null;

            return this.eventList[index];
        }


#if UNITY_EDITOR
        [SerializeField]
        bool editorForceRebuild;
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();

            if (this.editorForceRebuild || !this.anim)
                this.anim = this.FindComponent<Animation>();
        }

        protected override void OnEditorTestingLooped()
        {
            base.OnEditorTestingLooped();

            this.Update();
        }
#endif// UNITY_EDITOR
    }
}
