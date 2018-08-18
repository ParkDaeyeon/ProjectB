using System;
namespace Ext
{
    public class SubProgress
    {
        public SubProgress(SubProgress.ILimitTimer limit = null)
        {
            this.limit = limit;
            if (null != limit)
                this.limitStartTime = limit.CurrentTime;
        }
    
        float progress = 0;
        public float Progress
        {
            get
            {
                if (this.UseLimitTimer)
                {
                    var l = this.LimitProgress;
                    var p = this.progress;
                    if (l < p)
                        return l;
                }

                return this.progress;
            }
        }

        float progressBegin = 0;
        public float ProgressBegin { get { return this.progressBegin; } }

        float progressEnd = 0;
        public float ProgressEnd { get { return this.progressEnd; } }


        public SubProgress SetDestination(float end)
        {
            this.progressBegin = this.progress = this.progressEnd;
            this.progressEnd = end;

            return this;
        }
        public float SubDelta { get { return this.progressEnd - this.progressBegin; } }


        public SubProgress SetSubProgress(float t)
        {
            // NOTE: Clamp01
            var t01 = (0 > t) ? 0 :
                      (1 < t) ? 1 :
                      t;

            float value = this.progressBegin + this.SubDelta * t01;
            if (this.progress < value)
                this.progress = value;

            return this;
        }
        public float GetSubProgress()
        {
            var delta = this.SubDelta;
            if (0 == delta)
                return 0;

            var adjust = this.progress - this.progressBegin;
            if (0 > adjust)
                return 0;

            return adjust / delta;
        }
        public bool IsDoneSubProgress
        {
            get
            {
                return this.Progress == this.progressEnd;
            }
        }


        public static implicit operator float(SubProgress thiz)
        {
            return thiz.Progress;
        }

        public void Clear()
        {
            this.progress = this.progressBegin = this.progressEnd = 0;
        }


        public interface ILimitTimer
        {
            float CurrentTime { get; }
            float ManualTime { get; }
        }

        SubProgress.ILimitTimer limit = null;
        float limitStartTime = 0;
        public SubProgress.ILimitTimer GetLimitTimer() { return this.limit; }
        public bool UseLimitTimer { get { return null != this.limit; } }
        public float LimitProgress { get { return (this.limit.CurrentTime - this.limitStartTime) / this.limit.ManualTime; } }
        public float GetLimitSubProgress()
        {
            var delta = this.SubDelta;
            if (0 == delta)
                return 0;

            var adjust = this.LimitProgress - this.progressBegin;
            if (0 > adjust)
                return 0;

            return adjust / delta;
        }


        public override string ToString()
        {
            return string.Format("{{progress:{0}, progressBegin:{1}, progressEnd:{2}}}", this.progress, this.progressBegin, this.progressEnd);
        }
    }
}
