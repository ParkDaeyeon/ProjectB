using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Ext.Collection;

namespace Ext.Unity3D
{
    [RequireComponent(typeof(Animation))]
    public class IndexedAnimation : IndexableComponent
    {
        [SerializeField]
        Animation anim;
        public Animation Anim { get { return this.anim; } }
        public override int Count { get { return this.clips.GetLengthSafe(); } }


        [SerializeField]
        AnimationClip[] clips;
        public AnimationClip[] Clips { get { return this.clips; } }
        public AnimationClip SelectedClip { get { return this.clips.GetAtSafe(this.index); } }


        AnimationState[] states;
        public AnimationState[] States
        {
            get
            {
                if (null == this.states)
                    this.Setup();
                return this.states;
            }
        }
        public AnimationState SelectedState { get { return this.states.GetAtSafe(this.index); } }

        protected override void OnApply()
        {
            base.OnApply();

            var prevClip = this.anim.clip;
            var nextClip = this.SelectedClip;
            if (prevClip == nextClip)
                return;

            if (prevClip)
                this.anim.Stop();

            this.anim.clip = nextClip;

            if (nextClip)
                this.anim.Rewind();
        }

        void Awake()
        {
            this.Setup();
        }

        bool setup = false;
        public void Setup()
        {
            if (this.setup)
                return;

            this.setup = true;

            if (!this.anim)
                return;
            
            if (null == this.clips)
                return;

            this.states = new AnimationState[this.clips.Length];

            for (int n = 0, cnt = this.clips.Length; n < cnt; ++n)
            {
                var clip = this.clips[n];
                if (clip)
                    this.states[n] = this.anim[clip.name];
                else
                    this.states[n] = null;
            }
        }

        public override string ToString()
        {
            return string.Format("{{{0}, anim:{1}, index:{2}, count:{3}}}", base.ToString(), this.anim, this.index, this.Count);
        }


#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();

            this.EditorSetIndexableComponent();
        }

        protected override void OnEditorRebuild()
        {
            base.OnEditorRebuild();

            this.anim = this.GetComponent<Animation>();

            var count = this.anim.GetClipCount();
            int index = 0;

            this.clips = new AnimationClip[count];
            foreach (var state in this.anim)
                this.clips[index++] = ((AnimationState)state).clip;

            UnityExtension.SetDirtyAll(this.transform);
        }

        protected override void OnEditorAddCurrent()
        {
            base.OnEditorAddCurrent();


        }
#endif// UNITY_EDITOR
    }
}