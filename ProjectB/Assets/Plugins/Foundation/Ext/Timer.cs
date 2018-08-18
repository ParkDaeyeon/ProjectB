using Ext.Algorithm;
using System;
using System.Globalization;
using System.Text;

namespace Ext
{
    public class Timer
    {
        static Func<long> timeMsFuncDefault;
        public static Func<long> TimeMsFunctionDefault
        {
            set { Timer.timeMsFuncDefault = value; }
            get { return Timer.timeMsFuncDefault; }
        }

        public Timer(Func<long> timeMsFunc = null,
                     long offsetMs = 0,
                     long durationMs = 0)
        {
            this.timeMsFunc = timeMsFunc;
            this.offsetMs = offsetMs;
            this.durationMs = durationMs;
        }
        
        Func<long> timeMsFunc;
        public void SetTimeMsFunction(Func<long> value)
        {
            this.timeMsFunc = value;
            this.Update();
        }
        public Func<long> GetTimeMsFunction()
        {
            return this.timeMsFunc;
        }

        public long GetTimeMs()
        {
            if (null != this.timeMsFunc)
                return this.timeMsFunc();

            if (null != Timer.timeMsFuncDefault)
                return Timer.timeMsFuncDefault();

            return 0;
        }
        
        long offsetMs;
        public void SetOffsetMs(long value)
        {
            this.offsetMs = value;
            this.Update();
        }
        public long GetOffsetMs()
        {
            return this.offsetMs;
        }
        
        long durationMs;
        public void SetDurationMs(long value)
        {
            this.durationMs = 0 < value ? value : 0;
            this.Update();
        }
        public long GetDurationMs()
        {
            return this.durationMs;
        }
        
        long deltaMs;
        public long GetDeltaMs()
        {
            return this.deltaMs;
        }
        long clampedDeltaMs;
        public long GetClampedDeltaMs()
        {
            return this.clampedDeltaMs;
        }
        public void Update()
        {
            this.deltaMs =
            this.clampedDeltaMs = this.OnCalculateDeltaMs();

            if (0 < this.durationMs)
            {
                if (0 > this.clampedDeltaMs)
                    this.clampedDeltaMs = 0;
                else if (this.durationMs < this.clampedDeltaMs)
                    this.clampedDeltaMs = this.durationMs;
            }
        }
        protected virtual long OnCalculateDeltaMs()
        {
            return this.GetTimeMs() - this.offsetMs;
        }
        
        public double GetProgress()
        {
            if (0 >= this.durationMs)
                return 1;

            var current = Timer.ToSeconds(this.clampedDeltaMs);
            var length = Timer.ToSeconds(this.durationMs);
            return current / length;
        }

        public long GetRemainMs()
        {
            if (0 >= this.durationMs)
                return 0;


            return this.durationMs - this.clampedDeltaMs;
        }
        
        public override string ToString()
        {
            return new StringBuilder().Append('{')
                .Append("\'offsetMs\': ").Append(this.offsetMs).Append(", ")
                .Append("\'durationMs\': ").Append(this.durationMs).Append(", ")
                .Append("\'timeMs\': ").Append(this.deltaMs).Append(", ")
                .Append("\'progress\': ").Append(this.GetProgress()).Append(", ")
                .Append("\'remainMs\': ").Append(this.GetRemainMs()).Append(", ")
                .Append("\'timeMsFunc\': ").Append(null != this.timeMsFunc).Append('}').ToString();
        }
        
        public static double ToSeconds(long ms)
        {
            return ms * 0.001;
        }

        public static long ToMilliseconds(double seconds)
        {
            return (long)(seconds * 1000);
        }
        
        public static double ToClamp01(double value)
        {
            return 0 > value ? 0 : value > 1 ? 1 : value;
        }
    }


    public class DynamicTimer : Timer
    {
        public DynamicTimer(Func<long> timeMsFunc = null,
                            long offsetMs = 0,
                            long durationMs = 0)
            : base(timeMsFunc, offsetMs, durationMs)
        {}

        long fromMs;
        protected override long OnCalculateDeltaMs()
        {
            if (!this.play)
                return 0;

            return this.GetTimeMs() - this.fromMs;
        }
        
        bool play = false;
        public bool Playing
        {
            get { return this.play; }
        }
        
        public void Play()
        {
            if (this.play)
                return;

            this.play = true;

            var ms = this.GetTimeMs();
            this.fromMs = ms - this.GetOffsetMs();

            if (this.Paused)
                this.pausedMs = ms;

            this.Update();
        }

        public void Stop()
        {
            this.play = false;
            this.Update();
        }

        ulong pauseStatus = 0x0000000000000000;
        long pausedMs;
        public void SetPause(bool value, int stateIndex = 0)
        {
            if (stateIndex < 0 || 63 < stateIndex)
            {
#if LOG_DEBUG
#if UNITY_5_6_OR_NEWER
                UnityEngine.Debug.LogWarning("DYNAMIC_TIMER:INVALID_STATEINDEX:" + stateIndex);
#endif// UNITY_5_6_OR_NEWER
#endif// LOG_DEBUG
                return;
            }

            bool prev = this.Paused;
            this.pauseStatus = BitFlag.SetField(this.pauseStatus, stateIndex, value);
            bool next = this.Paused;

            if (prev != next)
            {
                var ms = this.GetTimeMs();
                if (next)
                {
                    this.pausedMs = ms;
                }
                else
                {
                    this.fromMs += ms - this.pausedMs;
                    this.pausedMs = 0;
                }
            }
        }
        public bool Paused
        {
            get { return 0 != this.pauseStatus; }
        }
        
        public void SetProgress(double value)
        {
            if (0 == this.GetDurationMs())
                return;

            var prev = this.GetProgress();
            var next = Timer.ToClamp01(value);
            this.fromMs -= (long)((next - prev) * this.GetDurationMs());
            this.Update();
        }
        
        public void SetRemainMs(long value)
        {
            if (0 == this.GetDurationMs())
                return;

            this.fromMs += value - this.GetRemainMs();
            this.Update();
        }
        
        public void SetDeltaTime(long value)
        {
            this.fromMs = this.GetTimeMs() - value;
            this.Update();
        }
        
        public override string ToString()
        {
            return new StringBuilder().Append('{')
                .Append("\'base\':").Append(base.ToString()).Append(", ")
                .Append("\'fromMs\': ").Append(this.fromMs).Append(", ")
                .Append("\'play\': ").Append(this.play).Append(", ")
                .Append("\'pauseStatus\': ").Append(this.pauseStatus).Append(", ")
                .Append("\'pausedMs\': ").Append(this.pausedMs).Append('}').ToString();
        }
    }
}