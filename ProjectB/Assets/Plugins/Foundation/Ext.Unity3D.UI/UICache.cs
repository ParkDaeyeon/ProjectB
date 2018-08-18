using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Ext.Unity3D.UI
{
    [Serializable]
    public class UICache
    {
        [SerializeField]
        GameObject gameObject;
        public GameObject GameObject
        {
            get { return this.gameObject; }
        }

        [SerializeField]
        RectTransform cachedRectTransform;
        public RectTransform CachedRectTransform
        {
            get { return this.cachedRectTransform; }
        }
        public Transform CachedTransform
        {
            get { return this.cachedRectTransform; }
        }

        [SerializeField]
        Animation animation;
        public Animation Animation
        {
            get { return this.animation; }
        }
        
        public struct AnimPair
        {
            public AnimationState state;
            public AnimationClip clip;
        }
        List<AnimPair> animPairs = new List<AnimPair>(4); 
        public List<AnimPair> AnimPairs
        {
            get { return this.animPairs; }
        }

        bool isOptimizedAnimPair = false;
        public bool IsOptimizedAnimPair
        {
            get { return this.isOptimizedAnimPair; }
        }
        
        public static void OptimizeAnimPairOnetime(UICache cache)
        {
            if (!cache)
                return;

            if (cache.isOptimizedAnimPair)
                return;

            cache.OptimizeAnimPair();
        }

        public void OptimizeAnimPair()
        {
            this.isOptimizedAnimPair = true;
            
            if (null == this.animPairs)
                this.animPairs = new List<AnimPair>(4);

            this.animPairs.Clear();
            if (this.animation)
            {
                foreach (AnimationState state in this.animation)
                {
                    if (null == state)
                    {
#if UNITY_EDITOR
                        if (!UnityEditor.EditorApplication.isPlaying)
                            Debug.LogWarning(string.Format("UICACHE_BAD_ANIM_STATE:{0}", null != this.GameObject ? this.GameObject.name : "Null"));
#endif// UNITY_EDITOR
                        continue;
                    }
                    this.animPairs.Add(new AnimPair { state = state, clip = state.clip, });
                }

                this.currentAnimPair = this.GetAnimPair(this.FindAnimPair(this.animation.clip));
            }
            else
                this.currentAnimPair = default(AnimPair);
        }

        public AnimPair GetAnimPair(int index)
        {
            if (-1 < index && index < this.animPairs.Count)
                return animPairs[index];

            return default(AnimPair);
        }

        public int FindAnimPair(AnimationClip clip)
        {
            if (!this.isOptimizedAnimPair)
                this.OptimizeAnimPair();

            for (int n = 0, cnt = this.animPairs.Count; n < cnt; ++n)
            {
                if (this.animPairs[n].clip == clip)
                    return n;
            }

            return -1;
        }
        public int FindAnimPair(AnimationState state)
        {
            if (!this.isOptimizedAnimPair)
                this.OptimizeAnimPair();

            for (int n = 0, cnt = this.animPairs.Count; n < cnt; ++n)
            {
                if (this.animPairs[n].state == state)
                    return n;
            }

            return -1;
        }

        public bool SetCurrentAnimPair(int index)
        {
            if (!this.isOptimizedAnimPair)
                this.OptimizeAnimPair();

            if (this.animation)
            {
                if (-1 < index && index < this.animPairs.Count)
                {
                    var animPair = this.currentAnimPair = this.animPairs[index];
                    this.animation.clip = animPair.clip;
                    return true;
                }
            }

            return false;
        }


        AnimPair currentAnimPair;
        public AnimPair CurrentAnimPair
        {
            get
            {
                if (!this.isOptimizedAnimPair)
                    this.OptimizeAnimPair();

                return this.currentAnimPair;
            }
        }
        public AnimationClip CurrentAnimClip { get { return this.CurrentAnimPair.clip; } }
        public AnimationState CurrentAnimState { get { return this.CurrentAnimPair.state; } }

        [SerializeField]
        Graphic graphic;
        public Graphic Graphic { get { return this.graphic; } }
#if UNITY_EDITOR
        [SerializeField]
        [HideInInspector]
        bool editorFoldout = false;
        public bool EditorFoldout { set { this.editorFoldout = value; } get { return this.editorFoldout; } }
#endif// UNITY_EDITOR

        public UICache(GameObject go)
        {
            this.Assign(go);
        }

        public virtual void Assign(GameObject go)
        {
            if (go)
            {
                this.gameObject = go;

                this.cachedRectTransform = go.GetComponent<RectTransform>();

                this.animation = go.GetComponent<Animation>();
                this.OptimizeAnimPair();

                this.graphic = go.GetComponent<Graphic>();
            }
            else
            {
                this.gameObject = null;

                this.cachedRectTransform = null;

                this.animation = null;
                this.currentAnimPair = default(AnimPair);
                this.graphic = null;
            }
        }
        
        public void Rebuild()
        {
            this.Assign(this.gameObject);
        }

        public void Rebuild(bool active)
        {
            this.Assign(this.gameObject);

            this.SetActive(active);
        }
        
        public static void Destroy(UICache target)
        {
            if (target)
            {
                try { GameObject.Destroy(target.GameObject); }
                catch (System.Exception e) { Debug.LogWarning(e); }
            }
        }

        public static void Destroy(UICache target, float t)
        {
            if (target)
            {
                try { GameObject.Destroy(target.GameObject, t); }
                catch (System.Exception e) { Debug.LogWarning(e); }
            }
        }
        
        public void SetActive(bool value)
        {
            if (this.gameObject)
                this.gameObject.SetActive(value);
        }
        public bool IsActivated
        {
            get { return this.gameObject ? this.gameObject.activeSelf : false; }
        }

        public void SetShow(bool value)
        {
            var trans = this.CachedRectTransform;
            if (!trans)
                return;

            if (value)
                this.CachedRectTransform.ShowTransform();
            else
                this.CachedRectTransform.HideTransform();
        }
        public bool IsShown
        {
            get
            {
                var trans = this.CachedRectTransform;
                return trans ? !trans.IsHideTransform() : false;
            }
        }
        
        public void AnimationPlayAndShow(bool play, bool isRewind = true)
        {
            if (play)
            {
                if (!this.IsActivated)
                    this.SetActive(true);

                if (isRewind || !this.animation.isPlaying)
                    this.animation.ForcePlay(true);
            }
            else
            {
                if (this.IsActivated)
                    this.SetActive(false);
            }
        }

        public void AnimationPlayAndShow(bool play, float time)
        {
            if (play)
            {
                if (!this.IsActivated)
                    this.SetActive(true);

                this.animation.ForcePlay(time);
            }
            else
            {
                if (this.IsActivated)
                    this.SetActive(false);
            }
        }

        public bool AnimationAutoHide()
        {
            if (this.IsActivated)
            {
                var animState = this.CurrentAnimState;

                if ((null != this.animation && !this.animation.isPlaying) || (null != animState && 1 <= animState.normalizedTime))
                {
                    this.SetActive(false);
                    return true;
                }
            }

            return false;
        }

        public float AnimationSampleInit()
        {
            var animState = this.CurrentAnimState;
            if (null == animState)
                return 0;

            var oldSpeed = animState.speed;
            animState.speed = 0;
            this.animation.Play();
            return oldSpeed;
        }
        public float AnimationSampleInit(int index)
        {
            if (!this.SetCurrentAnimPair(index))
                return 0;
            
            var animState = this.currentAnimPair.state;
            if (null == animState)
                return 0;

            var oldSpeed = animState.speed;
            animState.speed = 0;
            this.animation.Play();
            return oldSpeed;
        }

        public void AnimationSampleClear(float oldSpeed = 1f)
        {
            var animState = this.CurrentAnimState;
            if (null == animState)
                return;

            this.animation.Stop();
            animState.speed = oldSpeed;
        }

        public bool AnimationSample(float progress)
        {
            var animState = this.CurrentAnimState;
            if (null == animState)
                return false;

            if (!this.animation.isPlaying)
                return false;

            animState.normalizedTime = progress;
            this.animation.Sample();
            return true;
        }

        public float GetAnimationSample()
        {
            var animState = this.CurrentAnimState;
            if (null == animState)
                return 0;

            return animState.normalizedTime;
        }
        
        public static implicit operator bool(UICache cache)
        {
            return null != cache && cache.gameObject;
        }

        public override string ToString()
        {
            return string.Format("{{NAME:{0}, TRANS:{1}, GH:{2}, ANIM:{3}, ANIM_S:{4}}}",
                                (null == this.GameObject ? "null" : this.GameObject.name),
                                (null == this.CachedRectTransform ? "null" : this.CachedRectTransform.ToString()),
                                (null == this.graphic ? "null" : this.graphic.ToString()),
                                (null == this.animation ? "null" : null == this.animation.clip ? "None" : this.animation.clip.name),
                                (null == this.CurrentAnimState ? "null" : this.CurrentAnimState.ToString()));
        }
    }
}