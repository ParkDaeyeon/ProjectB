using UnityEngine;
using System;


namespace Ext.Unity3D
{
    [Serializable]
    public class LinearStep
    {
        public LinearStep() { }
        public LinearStep(float start, float end, int minStep, int maxStep)
        {
            this.start = start;
            this.end = end;
            this.minStep = minStep;
            this.maxStep = maxStep;
            //this.numberOfSteps = numberOfSteps;
        }

        [SerializeField]
        float start = 0;
        public float Start
        {
            set { this.start = value; }
            get { return this.start; }
        }

        [SerializeField]
        float end = 1;
        public float End
        {
            set { this.end = value; }
            get { return this.end; }
        }

        //[SerializeField]
        //uint numberOfSteps = 0;
        [SerializeField]
        int minStep = 0;
        public int MinStep { set { this.minStep = value; } }
        [SerializeField]
        int maxStep = 0;
        public int MaxStep { set { this.maxStep = value; } }
        public uint NumberOfSteps
        {
            get
            {
                var numOfSteps = this.maxStep + (this.minStep < 0 ? -this.minStep : this.minStep);
                return (uint)Mathf.Max(0, numOfSteps);
            }
        }
        
        public float Range
        {
            get { return this.end - this.start; }
        }

        public float Unit
        {
            get
            {
                if (0 == this.NumberOfSteps)
                    return 0;

                var range = this.Range;
                if (0 == range)
                    return 0;

                return range / (float)this.NumberOfSteps;
            }
        }



        public float NormalToValue(float normalizedValue)
        {
            return Mathf.Lerp(this.start, this.end, normalizedValue);
        }
        public int NormalToOptionValue(float normalizedValue)
        {
            return (int)Mathf.Lerp(this.minStep, this.maxStep, normalizedValue);
        }
        public uint NormalToStep(float normalizedValue)
        {
            return (uint)Mathf.Max(0, Mathf.RoundToInt(normalizedValue * this.NumberOfSteps));
        }

        public float ToSteppedNormal(float normalizedValue)
        {
            normalizedValue = Mathf.Clamp01(normalizedValue);

            if (0 < this.NumberOfSteps)
            {
                var steppedValue = Mathf.RoundToInt(this.NumberOfSteps * normalizedValue);
                normalizedValue = Mathf.Clamp01(steppedValue / (float)this.NumberOfSteps);
            }

            return normalizedValue;
        }

        public float ToSteppedNormalWithValue(float value)
        {
            var range = this.Range;
            if (0 == range)
                return 0;

            return this.ToSteppedNormal((value - this.start) / range);
        }

        public float ToSteppedNormalWithStep(uint step)
        {
            if (0 == this.NumberOfSteps)
                return 0;

            return this.ToSteppedNormal(step / (float)this.NumberOfSteps);
        }
        
        public uint ToClampedStep(uint step)
        {
            return step < this.NumberOfSteps ? step : this.NumberOfSteps;
        }

        public float OptionValueToNormal(int value)
        {
            return (this.maxStep + value) / (float)this.NumberOfSteps;
        }

        public override string ToString()
        {
            return string.Format("{{Start:{0}, End:{1}, NumberOfSteps:{2}, Range:{3}, Unit:{4}}}",
                                  this.Start, this.End, this.NumberOfSteps, this.Range, this.Unit);
        }
    }
}
