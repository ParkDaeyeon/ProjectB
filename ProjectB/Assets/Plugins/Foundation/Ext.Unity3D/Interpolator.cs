using UnityEngine;

using System;
using System.Collections.Generic;

using Ext.Algorithm;

namespace Ext.Unity3D
{
	[Serializable]
    public class Interpolator : IInterpolable, ISerializationCallbackReceiver
    {
        public enum AlgorithmType
        {
            Linear = 0,
            Acceleration,
            Bezier3,
            Bezier4,
            Spline,
            Ease,
            Constant,
            Custom,
        }

        [SerializeField]
        AlgorithmType algorithm;
        public AlgorithmType Algorithm { get { return this.algorithm; } }


        [SerializeField]
        float[] floatValues;
        public float[] FloatValues
        {
            get { return this.floatValues; }
        }
        public int FloatCount
        {
            get { return null != this.floatValues ? this.floatValues.Length : 0; }
        }
        public float GetFloatValue(int index)
        {
            if (-1 < index && index < this.FloatCount)
                return this.floatValues[index];

            return 0;
        }


        [SerializeField]
        int intValue;
        public int IntValue
        {
            get { return this.intValue; }
        }


        bool isDirty = false;
        public bool IsDirty
        {
            set { this.isDirty = value;} 
            get { return this.isDirty; }
        }



        public class Exception : System.Exception
        {
            public enum Reason
            {
                FloatValuesNotEmpty,
                FloatValuesNull,
                FloatValuesWrongSize,
                IntValueOutOfRange,
            }
            Reason reason;
            public Reason GetReason() { return this.reason; }


            AlgorithmType algorithm;
            public AlgorithmType Algorithm { get { return this.algorithm; } }

            float[] floatValues;
            public float[] FloatValues { get { return this.floatValues; } }

            int intValue;
            public int IntValue { get { return this.intValue; } }

            internal Exception(Reason reason, AlgorithmType algorithm, float[] floatValues, int intValue)
                : base(string.Format("Interpolator Change Exception: Reason:{0}, Algorithm:{1}, FloatValues:{2}, IntValue:{3}",
                                     reason,
                                     algorithm,
                                     (null != floatValues ? string.Format("[{0}]", floatValues.Length.ToString()) : "null"),
                                     intValue))
            {
                this.reason = reason;
                this.algorithm = algorithm;
                this.floatValues = floatValues;
                this.intValue = intValue;
            }
        }
        public static void Verify(AlgorithmType algorithm, float[] floatValues, int intValue)
        {
            switch (algorithm)
            {
            case AlgorithmType.Linear:
            case AlgorithmType.Constant:
                {
                    if (null != floatValues && 0 < floatValues.Length)
                        throw new Exception(Exception.Reason.FloatValuesNotEmpty, algorithm, floatValues, intValue);

                    break;
                }

            case AlgorithmType.Acceleration:
                {
                    if (null == floatValues)
                        throw new Exception(Exception.Reason.FloatValuesNull, algorithm, floatValues, intValue);

                    if (1 != floatValues.Length)
                        throw new Exception(Exception.Reason.FloatValuesWrongSize, algorithm, floatValues, intValue);

                    break;
                }

            case AlgorithmType.Bezier3:
                {
                    if (null == floatValues)
                        throw new Exception(Exception.Reason.FloatValuesNull, algorithm, floatValues, intValue);

                    if (1 != floatValues.Length)
                        throw new Exception(Exception.Reason.FloatValuesWrongSize, algorithm, floatValues, intValue);

                    break;
                }

            case AlgorithmType.Bezier4:
                {
                    if (null == floatValues)
                        throw new Exception(Exception.Reason.FloatValuesNull, algorithm, floatValues, intValue);

                    if (2 != floatValues.Length)
                        throw new Exception(Exception.Reason.FloatValuesWrongSize, algorithm, floatValues, intValue);

                    break;
                }

            case AlgorithmType.Spline:
                {
                    if (null == floatValues)
                        throw new Exception(Exception.Reason.FloatValuesNull, algorithm, floatValues, intValue);

                    if (0 == floatValues.Length || 1 == (floatValues.Length % 2))
                        throw new Exception(Exception.Reason.FloatValuesWrongSize, algorithm, floatValues, intValue);

                    break;
                }

            case AlgorithmType.Ease:
                {
                    if (null != floatValues && 0 < floatValues.Length)
                        throw new Exception(Exception.Reason.FloatValuesNotEmpty, algorithm, floatValues, intValue);

                    if (0 > intValue || intValue >= Ease.EquationCount)
                        throw new Exception(Exception.Reason.IntValueOutOfRange, algorithm, floatValues, intValue);

                    break;
                }
            }
        }

        public static Interpolator Create(AlgorithmType algorithm, float[] floatValues, int intValue)
        {
            Interpolator.Verify(algorithm, floatValues, intValue);

            var interpolator = new Interpolator();
            interpolator.algorithm = algorithm;
            interpolator.floatValues = floatValues;
            interpolator.intValue = intValue;
            interpolator.Apply();
            return interpolator;
        }

        public void ChangeLinear()
        {
            this.Change(AlgorithmType.Linear, null, 0);
        }

        public void ChangeAcceleration(float factor)
        {
            this.Change(AlgorithmType.Acceleration, new float[] { factor, }, 0);
        }

        public void ChangeBezier3(float p0)
        {
            this.Change(AlgorithmType.Bezier3, new float[] { p0, }, 0);
        }

        public void ChangeBezier4(float p0, float p1)
        {
            this.Change(AlgorithmType.Bezier4, new float[] { p0, p1, }, 0);
        }

        public void ChangeSpline(Spline.Point[] points)
        {
            if (null == points)
                throw new Exception(Exception.Reason.FloatValuesNull, algorithm, null, intValue);

            if (2 > points.Length)
                throw new Exception(Exception.Reason.FloatValuesWrongSize, algorithm, new float[0], intValue);

            var first = points[0];
            var last = points[points.Length - 1];
            points[0] = new Spline.Point { key = 0, value = first.value, };
            points[points.Length - 1] = new Spline.Point { key = 1, value = last.value, };

            var floatValues = new float[points.Length * 2];
            for (int n = 0, cnt = points.Length; n < cnt; ++n)
            {
                floatValues[(n * 2)] = points[n].key;
                floatValues[(n * 2) + 1] = points[n].value;
            }

            this.Change(AlgorithmType.Spline, floatValues, 0);
        }

        public void ChangeEase(Ease.Equations eq)
        {
            this.Change(AlgorithmType.Ease, null, (int)eq);
        }

        public void ChangeConstant()
        {
            this.Change(AlgorithmType.Constant, null, 0);
        }

        public void ChangeCustom(IInterpolable custom)
        {
            this.algorithm = AlgorithmType.Custom;
            this.isDirty = false;
            this.floatValues = null;
            this.internalInterpolator__ = custom;
        }
        
        public void Change(AlgorithmType algorithm, float[] floatValues, int intValue)
        {
            Interpolator.Verify(algorithm, floatValues, intValue);

            if (this.algorithm != algorithm)
            {
                this.Invalidate();
                this.algorithm = algorithm;
            }
            else
            {
                this.isDirty = true;
            }

            this.floatValues = floatValues;
            this.intValue = intValue;
            this.Apply();
        }

        IInterpolable internalInterpolator__;
        public IInterpolable InternalInterpolator
        {
            get
            {
                if (!this.Apply())
                    return null;

                return this.internalInterpolator__;
            }
        }

        public bool Apply()
        {
            if (null == this.internalInterpolator__)
                this.internalInterpolator__ = this.CreateInterpolator();

            if (null == this.internalInterpolator__)
                return false;

            if (this.isDirty)
                this.UpdateInterpolator(this.internalInterpolator__);

            return true;
        }

        public void Invalidate()
        {
            this.internalInterpolator__ = null;
        }



        public virtual IInterpolable CreateInterpolator()
        {
            this.isDirty = false;

            switch (this.algorithm)
            {
            case AlgorithmType.Linear:
                return Linear.Instance;

            case AlgorithmType.Acceleration:
                return new Acceleration(this.floatValues[0]);

            case AlgorithmType.Bezier3:
                return new Bezier3(this.floatValues[0]);

            case AlgorithmType.Bezier4:
                return new Bezier4(this.floatValues[0], this.floatValues[1]);

            case AlgorithmType.Spline:
                return new Spline(Interpolator.ToSplintPoint(this.floatValues));

            case AlgorithmType.Ease:
                return new Ease((Ease.Equations)this.intValue);

            case AlgorithmType.Constant:
                return Constant.Instance;
            }

            return null;
        }

        public virtual bool UpdateInterpolator(IInterpolable internalInterpolator)
        {
            this.isDirty = false;

            switch (this.algorithm)
            {
            case AlgorithmType.Linear:
            case AlgorithmType.Constant:
                return true;

            case AlgorithmType.Acceleration:
                {
                    var inst = (Acceleration)internalInterpolator;
                    inst.Factor = this.floatValues[0];
                    return true;
                }

            case AlgorithmType.Bezier3:
                {
                    var inst = (Bezier3)internalInterpolator;
                    inst.P1 = this.floatValues[0];
                    return true;
                }

            case AlgorithmType.Bezier4:
                {
                    var inst = (Bezier4)internalInterpolator;
                    inst.P1 = this.floatValues[0];
                    inst.P2 = this.floatValues[1];
                    return true;
                }

            case AlgorithmType.Spline:
                {
                    var inst = (Spline)internalInterpolator;
                    inst.Update(Interpolator.ToSplintPoint(this.floatValues));
                    return true;
                }

            case AlgorithmType.Ease:
                {
                    var inst = (Ease)internalInterpolator;
                    inst.Equation = (Ease.Equations)this.IntValue;
                    return true;
                }
            }

            return false;
        }

        public static List<Spline.Point> ToSplintPoint(float[] floatValues)
        {
            var list = new List<Spline.Point>();
            for (int n = 0, cnt = floatValues.Length; n < cnt; n += 2)
            {
                list.Add(new Spline.Point
                {
                    key = 0 == n ? 0
                                 : (cnt - 1) == n ? 1
                                                  : floatValues[n],
                    value = floatValues[n + 1],
                });
            }

            return list;
        }

        
        public bool IsClamped01
        {
            get
            {
                var internalInterpolator = this.InternalInterpolator;
                return null != internalInterpolator ? internalInterpolator.IsClamped01 : true;
            }
        }

        public float Interpolate(float t)
        {
            var internalInterpolator = this.InternalInterpolator;
            return null != internalInterpolator ? internalInterpolator.Interpolate(t) : 0;
        }

        public override string ToString()
        {
            var internalInterpolator = this.InternalInterpolator;
            return null != internalInterpolator ? internalInterpolator.ToString() : "Unknown";
        }

        public void OnBeforeSerialize()
        {
            this.Invalidate();
        }

        public void OnAfterDeserialize()
        {
            this.Apply();
        }

#if UNITY_EDITOR
//#pragma warning disable 0426
#pragma warning disable 0414
        [SerializeField]
        AlgorithmType editorAlgorithm;

        [SerializeField]
        float editorVarFactor = 2;

        [SerializeField]
        float editorVarP1 = 0.5f;

        [SerializeField]
        float editorVarP2 = 0.75f;

        [Serializable]
        public struct SplinePoint
        {
            public float key;
            public float value;

            public static implicit operator Spline.Point(SplinePoint point)
            {
                return new Spline.Point
                {
                    key = point.key,
                    value = point.value,
                };
            }
        }
        [SerializeField]
        SplinePoint[] editorVarPoints = new SplinePoint[]
        {
            new SplinePoint { key = 0f, value = 0f, },
            new SplinePoint { key = 0.5f, value = 0.25f, },
            new SplinePoint { key = 1f, value = 1f, },
        };

        [SerializeField]
        Ease.Equations editorVarEquation;
//#pragma warning restore 0426
#pragma warning restore 0414
#endif// UNITY_EDITOR
    }
}
