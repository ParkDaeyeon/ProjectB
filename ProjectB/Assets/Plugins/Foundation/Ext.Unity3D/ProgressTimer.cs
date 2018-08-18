using System;
using UnityEngine;
namespace Ext.Unity3D
{
    public class ProgressTimer : MonoBehaviour
    {
        [SerializeField]
        float duration;
        public float Duration { get { return this.duration; } }
        public void SetDuration(float value) { this.duration = value; }


        [SerializeField]
        bool durationZeroIsComplete = false;
        public bool DurationZeroIsComplete
        {
            set { this.durationZeroIsComplete = value; }
            get { return this.durationZeroIsComplete; }
        }


        [SerializeField]
        float speed = 1;
        public float Speed { get { return this.speed; } }
        public void SetSpeed(float value) { this.speed = value; }

        public void Play()
        {
            this.accumulateTime = 0;

            if (null != this.onPlay)
                this.onPlay();
        }

        bool pause;
        public void SetPause(bool pause)
        {
            this.pause = pause;
        }
        public bool IsPaused
        {
            get { return this.pause; }
        }

        float accumulateTime;
        public float AccumulateTime
        {
            get
            {
                if (this.ifInactiveThenAlwaysZero)
                {
                    if (!this.isActiveAndEnabled)
                        return 0;
                }

                return this.accumulateTime;
            }
        }

        public float Progress
        {
            get
            {
                if (this.ifInactiveThenAlwaysZero)
                {
                    if (!this.isActiveAndEnabled)
                        return 0;
                }
                
                if (0 == this.duration)
                    return this.durationZeroIsComplete ? 1 : 0;

                if (this.loop)
                    return (this.accumulateTime / this.duration) % 1f;
                else
                    return Mathf.Clamp01(this.accumulateTime / this.duration);
            }
        }

        void Update()
        {
            if (this.pause)
                return;

            this.accumulateTime += Time.deltaTime * this.speed;
        }


        [SerializeField]
        bool playAutomatically;
        public bool PlayAutomatically
        {
            set { this.playAutomatically = value; }
            get { return this.playAutomatically; }
        }

        [SerializeField]
        bool loop;
        public bool Loop
        {
            set { this.loop = value; }
            get { return this.loop; }
        }


        [SerializeField]
        bool ifInactiveThenAlwaysZero;
        public bool IfInactiveThenAlwaysZero
        {
            set { this.ifInactiveThenAlwaysZero = value; }
            get { return this.ifInactiveThenAlwaysZero; }
        }


        Action onPlay;
        public Action OnPlay
        {
            set { this.onPlay = value; }
            get { return this.onPlay; }
        }

        void OnEnable()
        {
            if (this.playAutomatically)
                this.Play();
        }
    }
}
