using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Ext.Collection.AntiGC;
using Ext.Algorithm;

namespace Ext.Unity3D.UI
{
    public class MoveAnimator : ManagedUIComponent
    {
        [SerializeField]
        float duration;

        [SerializeField]
        float delay;

        [SerializeField]
        RectTransform obj;

        [SerializeField]
        RectTransform source;
        public RectTransform Source{ get { return this.source; } set { this.source = value; } }

        [SerializeField]
        RectTransform destination;
        public RectTransform Destination{ get { return this.destination; } set { this.destination = value; } }

        [SerializeField]
        Interpolator interpolator;
        public Interpolator Interpolator { get { return this.interpolator; } set { this.interpolator = value; } }


        public float TotalTime { get { return this.duration + this.delay; } }
        public Action PlayEndHandler;

        [SerializeField]
        bool isPlaying = true;
        public bool IsPlaying 
        { 
            set 
            { 
                this.isPlaying = value; 

                if (this.isPlaying)
                {
                    if (this.obj == null || this.source == null || this.destination == null)
                        this.isPlaying = false;
                }
            } 
            get 
            { 
                return this.isPlaying; 
            } 
        }


        void Update()
        {
            if (!this.isPlaying)
                return;

            this.UpdatePositions();
        }

        float currentTime = 0.0f;
        Vector2 position;
        public void UpdatePositions()
        {
            if (this.isPlaying == false)
                return;
            
            this.currentTime += Time.smoothDeltaTime;

            if (this.currentTime > this.delay)
            {
                float t = (this.currentTime - this.delay) / this.duration; 

                t = this.interpolator.Interpolate(t);

                this.position.x = this.Linear(t, this.source.anchoredPosition.x, this.destination.anchoredPosition.x);
                this.position.y = this.Linear(t, this.source.anchoredPosition.y, this.destination.anchoredPosition.y);

                this.obj.anchoredPosition = position;

                if (t >= 1.0f)
                {
                    if (this.PlayEndHandler != null)
                    {
                        this.PlayEndHandler();
                        this.PlayEndHandler = null;
                    }
                    
                    this.IsPlaying = false;
                }
                
            }
        }

        public void ResetPositions()
        {
            this.currentTime = 0.0f;
            this.obj.anchoredPosition = this.source.anchoredPosition;
        }
        

        float Linear(float t, float p1, float p2)
        {
            return t * (p2 - p1) + p1;
        }

#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();

            this.obj = this.GetComponent<RectTransform>();
        }
#endif// UNITY_EDITOR
    }
}
