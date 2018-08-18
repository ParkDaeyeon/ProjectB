using UnityEngine;
using System;
using System.Collections.Generic;

namespace Ext.Unity3D.UI
{
	public class NormalizedAction : ManagedComponent
	{
        [Serializable]
        public class State
        {
            [SerializeField]
            float totalTime;
            public float TotalTime { set { this.totalTime = value; } get { return this.totalTime; } }

            [SerializeField]
            Interpolator interpolator;
            public Interpolator Interpolator { set { this.interpolator = value; } get { return this.interpolator; } }

            public float GetProgress(float startTime, float currentTime)
            {
                var delta = currentTime - startTime;
                var t01 = Mathf.Clamp01(delta / this.totalTime);
                if (null != this.interpolator)
                    return this.interpolator.Interpolate(t01);
                else
                    return t01;
            }
        }

        [SerializeField]
        List<State> status = new List<State>();
        public List<State> Statuses { get { return this.status; } }

        public State this[int index]
        {
            get
            {
                return -1 < index && index < this.Count ? this.status[index] : null;
            }
        }

        public int Count { get { return this.status.Count; } }


        int currentIndex = -1;
        public int CurrentIndex
        {
            set { this.currentIndex = value; }
            get { return this.currentIndex; }
        }
        
        public State CurrentState { get { return this[this.currentIndex]; } }


        [SerializeField]
        float value = 1;
        public float Value
        {
            set
            {
                this.value = value;
                this.Stop();
            }
            get { return this.value; }
        }

        float startValue;
        public float CurrentValue { get { return Mathf.Lerp(this.startValue, this.value, this.Progress); } }


        Func<float> customTimer;
        public Func<float> CustomTimer { set { this.customTimer = value; } get { return this.customTimer; } }

        public void Play(int stateIndex, float value)
        {
            this.startValue = this.CurrentValue;
            this.value = value;
            if (this.value == this.startValue)
            {
                this.Stop();
                return;
            }

            this.startTime = this.CurrentTime;
            this.currentIndex = stateIndex;
        }

        float startTime;
        public float StartTime { get { return this.startTime; } }
        public float CurrentTime { get { return null != this.customTimer ? this.customTimer() : Time.time; } }


        public void Stop()
        {
            this.currentIndex = -1;
        }

        public float Progress
        {
            get
            {
                var state = this.CurrentState;
                return null != state ? state.GetProgress(this.startTime, this.CurrentTime) : 1;
            }
        }

        public bool IsPlaying
        {
            get
            {
                var state = this.CurrentState;
                return null != state ? 1 > state.GetProgress(this.startTime, this.CurrentTime) : false;
            }
        }

        public float TimeLength
        {
            get
            {
                var state = this.CurrentState;
                return null != state ? state.TotalTime : 0;
            }
        }
	}
}