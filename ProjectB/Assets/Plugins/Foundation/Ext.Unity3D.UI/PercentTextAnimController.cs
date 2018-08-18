using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Ext.String;
using System.Globalization;

namespace Ext.Unity3D.UI
{
    public class PercentTextAnimController : NumberTextAnimController
    {
        [SerializeField]
        int decimalCount = -1;
        public int DecimalCount
        {
            set
            {
                if (value == this.decimalCount)
                    return;

                var delta = value - this.decimalCount;

                this.decimalCount = value;

                if (0 < delta)
                    this.Value = this.Next * (uint)Mathf.Pow(10, delta);
                else
                    this.Value = this.Next / (uint)Mathf.Pow(10, -delta);

                this.UpdateValue();
            }
            get { return this.decimalCount; }
        }


        [SerializeField]
        string percentFormat = "{0}%";
        public string PercentFormat
        {
            set { this.percentFormat = value; }
            get { return this.percentFormat; }
        }


        Func<PercentTextAnimController, double, string> onOverrideOutput;
        public Func<PercentTextAnimController, double, string> OnOverrideOutput
        {
            set { this.onOverrideOutput = value; }
            get { return this.onOverrideOutput; }
        }

        public override string ToStringValue(uint value)
        {
            double realValue = this.ToPercent(value);

            if (null != this.onOverrideOutput)
            {
                var output = this.onOverrideOutput(this, realValue);
                if (!string.IsNullOrEmpty(output))
                    return output;
            }

            return string.Format(this.percentFormat, StringExtension.ToSeparatedString(realValue, ',', this.UseComma, this.decimalCount, true));
        }


        public double Percent
        {
            set { this.Value = this.ToUnsignedInteger(value); }
            get { return this.ToPercent(this.Value); }
        }

        public double NextPercent
        {
            set { this.Next = this.ToUnsignedInteger(value); }
            get { return this.ToPercent(this.Next); }
        }

        public double ToPercent(uint value)
        {
            return 0 < this.decimalCount ? (double)value / Mathf.Pow(10, this.decimalCount) : value;
        }
        public uint ToUnsignedInteger(double value)
        {
            return 0 < this.decimalCount ? (uint)(value * Mathf.Pow(10, this.decimalCount)) : (uint)value;
        }


#if UNITY_EDITOR
        [SerializeField]
        double editorTestNextPercentValue;
        protected override void OnEditorTesting()
        {
            base.OnEditorTesting();
            this.NextPercent = this.editorTestNextPercentValue;
        }
#endif// UNITY_EDITOR
    }
}
