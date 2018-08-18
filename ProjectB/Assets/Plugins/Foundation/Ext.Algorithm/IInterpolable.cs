using System;
using System.Collections.Generic;
namespace Ext.Algorithm
{
    public interface IInterpolable
    {
        float Interpolate(float t);
        bool IsClamped01 { get; }
    }


    public class Linear : IInterpolable
    {
        public float Interpolate(float t) { return t; }

        public bool IsClamped01 { get { return false; } }

        private Linear() { }

        public static readonly Linear Instance = new Linear();

        public override string ToString()
        {
            return "Linear";
        }
    }


    public class Acceleration : IInterpolable
    {
        public float Interpolate(float t) { return (float)Math.Pow(t, 0 < this.factor ? this.factor : 1f); }

        public bool IsClamped01 { get { return false; } }


        public Acceleration(float factor = 1f) { this.factor = factor; }

        float factor;
        public float Factor
        {
            set { this.factor = value; }
            get { return this.factor; }
        }

        public override string ToString()
        {
            return string.Format("Acceleration:{{Factor:{0}}}", this.factor);
        }
    }


    public class Bezier3 : IInterpolable
    {
        public float Interpolate(float t)
        {
            // ---------------------------------------------------------------------------
            // NOTE: Bezier3 공식 정리
            // ---------------------------------------------------------------------------
            // B3(P0, P1, P2, t) = (1 - t) * ((1 - t) * P0 + t * P1) + t * ((1 - t) * P1 + t * P2)
            //                   = (1 - t)^2 * P0 + 2 * (1 - t) * t * P1 + t^2 * P2
            //
            // 만약 P0, P1, P2 값이 이렇게 고정된다 할 경우:
            // P0 = 0
            // P1 = input
            // P2 = 1
            //
            // 이렇다면,
            // 0 을 곱하는 구간은 결과가 0 이므로 통채로 제거.
            // 1 을 곱하는 구간은 결과가 변화 없음이므로 * 1 구간만 제거.
            //
            //                   #################################################
            //                   #### 0 을 곱하는 앞 구간 제거 : (1 - t)^2 * 0 ####
            //                   #################################################
            //                   = (1 - t)^2 * 0 + 2 * (1 - t) * t * P1 + t^2 * 1  
            //                   = 2 * (1 - t) * t * P1 + t^2 * 1
            //                   #################################################
            //                   #### 1 을 곱하는 뒷 구간 제거 : * 1 ##############
            //                   #################################################
            //                   = 2 * (1 - t) * t * P1 + t^2 * 1
            //                   = 2 * (1 - t) * t * p1 + t^2
            //                   #################################################
            // 정리된 공식:
            // B3(0, P1, 1, t) = 2 * (1 - t) * t * p1 + t^2
            //
            // 이에 대한 증명:
            // 
            // P0 = 0
            // P1 = 0.3
            // P2 = 1
            // t = 0.5
            //
            // 이렇게 값이 넘어온다 가정하고,
            // TEST1.
            // = (1 - t) * ((1 - t) * P0 + t * P1) + t * ((1 - t) * P1 + t * P2)
            // = (1 - 0.5) * ((1 - 0.5) * 0 + 0.5 * 0.3) + 0.5 * ((1 - 0.5) * 0.3 + 0.5 * 1)
            // = 0.4
            //
            // TEST2.
            // = (1 - t)^2 * P0 + 2 * (1 - t) * t * P1 + t^2 * P2
            // = (1 - 0.5)^2 * 0 + 2 * (1 - 0.5) * 0.5 * 0.3 + 0.5^2 * 1
            // = 0.4
            //
            // TEST3.
            // = 2 * (1 - t) * t * P1 + t^2
            // = 2 * (1 - 0.5) * 0.5 * 0.3 + 0.5^2
            // = 0.4
            //
            // 
            // ※ ※ ※ ※ ※ ※ ※ ※ ※ ※ ※ ※ ※ ※ ※ ※ ※ ※ 
            // ※ Bezier3(0, P1, 1, t) 의 정규화된 공식 ※ 
            // ※ ※ ※ ※ ※ ※ ※ ※ ※ ※ ※ ※ ※ ※ ※ ※ ※ ※ 
            //
            // B3(0, P1, 1, t)  =  2 * (1 - t) * t * P1 + t^2
            //
            // ※ ※ ※ ※ ※ ※ ※ ※ ※ ※ ※ ※ ※ ※ ※ ※ ※ ※ 

            return 2 * (1 - t) * t * this.p1 + t * t;
        }
        
        public bool IsClamped01 { get { return false; } }


        public Bezier3(float p1)
        {
            this.p1 = p1;
        }

        float p1;
        public float P1
        {
            set { this.p1 = value; }
            get { return this.p1; }
        }

        public override string ToString()
        {
            return string.Format("Bezier3:{{P1:{0}}}", this.p1);
        }
    }


    public class Bezier4 : IInterpolable
    {
        public float Interpolate(float t)
        {
            //float iT = 1.0f - t;
            //float b0 = iT * iT * iT;
            //float b1 = 3 * t * iT * iT;
            //float b2 = 3 * t * t * iT;
            //float b3 = t * t * t;

            //return 0 * b0 + this.p1 * b1 + this.p2 * b2 + 1 * b3;

            float iT = 1.0f - t;
            //float b0 = iT * iT * iT;
            float b1 = 3 * t * iT * iT;
            float b2 = 3 * t * t * iT;
            float b3 = t * t * t;

            //return 0 * b0 + this.p1 * b1 + this.p2 * b2 + 1 * b3;
            return this.p1 * b1 + this.p2 * b2 + b3;
        }
        
        public bool IsClamped01 { get { return false; } }

        
        public Bezier4(float p1, float p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }

        float p1;
        public float P1
        {
            set { this.p1 = value; }
            get { return this.p1; }
        }

        float p2;
        public float P2
        {
            set { this.p2 = value; }
            get { return this.p2; }
        }
        public override string ToString()
        {
            return string.Format("Bezier4:{{P1:{0}, P2:{1}}}", this.p1, this.p2);
        }
    }


    public class Spline : IInterpolable, IClosable
    {
        /// <summary>
        /// Gets interpolated value for specified argument.
        /// </summary>
        /// <param name="t">Argument value for interpolation. Must be within 
        /// the interval bounded by lowest ang highest <see cref="this.keys"/> values.</param>
        public float Interpolate(float t)
        {
            if (!this.IsOpened)
                return t;

            if (0 == t)
                return this.points[0].value;
            if (1 == t)
                return this.points[this.points.Length - 1].value;

            int gap = 0;
            var previous = float.MinValue;

            // At the end of this iteration, "gap" will contain the index of the interval
            // between two known values, which contains the unknown z, and "previous" will
            // contain the biggest z value among the known samples, left of the unknown z
            for (int n = 0; n < this.points.Length; ++n)
            {
                if (this.points[n].key < t && this.points[n].key > previous)
                {
                    previous = this.points[n].key;
                    gap = n + 1;
                }
            }

            var x1 = t - previous;
            var x2 = this.h[gap] - x1;

            return ((-this.a[gap - 1] / 6 * (x2 + this.h[gap]) * x1 + this.points[gap - 1].value) * x2 +
                    (-this.a[gap] / 6 * (x1 + this.h[gap]) * x2 + this.points[gap].value) * x1) / this.h[gap];
        }
        
        public bool IsClamped01 { get { return false; } }

        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="nodes">Collection of known points for further interpolation.
        /// Should contain at least two items.</param>
        public Spline(IList<Point> points)
        {
            this.Update(points);
        }

        public struct Point
        {
            public float key;
            public float value;
        }
        Point[] points;
        public Point[] Points { get { return this.points; } }
        public int Count { get { return null != this.points ? this.points.Length : 0; } }
        float[] h;
        float[] a;


        public void Update(IList<Point> points)
        {
            if (null == points)
            {
                this.Close();
                return;
            }

            var count = points.Count;
            if (count < 2)
            {
                this.Close();
                return;
            }

            var first = points[0];
            var last = points[points.Count - 1];
            points[0] = new Point { key = 0, value = first.value, };
            points[points.Count - 1] = new Point { key = 1, value = last.value, };

            this.points = new Point[count];
            for (int n = 0; n < count; ++n)
                this.points[n] = points[n];
            this.a = new float[count];
            this.h = new float[count];

            for (int n = 1; n < count; ++n)
            {
                this.h[n] = this.points[n].key - this.points[n - 1].key;
            }

            if (count > 2)
            {
                // TODO: buffer
                var sub = new float[count - 1];
                var diag = new float[count - 1];
                var sup = new float[count - 1];

                for (int n = 1; n <= count - 2; ++n)
                {
                    diag[n] = (this.h[n] + this.h[n + 1]) / 3;
                    sup[n] = this.h[n + 1] / 6;
                    sub[n] = this.h[n] / 6;
                    this.a[n] = (this.points[n + 1].value - this.points[n].value) / this.h[n + 1] - (this.points[n].value - this.points[n - 1].value) / this.h[n];
                }

                Spline.SolveTridiag(sub, diag, sup, ref this.a, count - 2);
            }
        }

        public bool IsOpened { get { return null != this.points; } }
        public void Open() { }
        public void Close()
        {
            this.points = null;
            this.h = null;
            this.a = null;
        }

        /// <summary>
        /// Solve linear system with tridiagonal n*n matrix "a"
        /// using Gaussian elimination without pivoting.
        /// </summary>
        static void SolveTridiag(float[] sub, float[] diag, float[] sup, ref float[] b, int end)
        {
            int n;

            for (n = 2; n <= end; ++n)
            {
                sub[n] = sub[n] / diag[n - 1];
                diag[n] = diag[n] - sub[n] * sup[n - 1];
                b[n] = b[n] - sub[n] * b[n - 1];
            }

            b[end] = b[end] / diag[end];

            for (n = end - 1; n >= 1; --n)
            {
                b[n] = (b[n] - sup[n] * b[n + 1]) / diag[n];
            }
        }

        public override string ToString()
        {
            return string.Format("Spline:{{Count:{0}}}", this.Count);
        }
    }


    public class Ease : IInterpolable
    {
        public float Interpolate(float t)
        {
            t = Ease.Clamp01(t);
            return null != this.easeFunc ? this.easeFunc(t) : t;
        }

        public bool IsClamped01 { get { return true; } }



        public Ease(Equations eq)
        {
            this.Equation = eq;
        }


        public enum Equations
        {
            Linear = 0,
            QuadEaseOut,
            QuadEaseIn,
            QuadEaseInOut,
            QuadEaseOutIn,
            ExpoEaseOut,
            ExpoEaseIn,
            ExpoEaseInOut,
            ExpoEaseOutIn,
            CubicEaseOut,
            CubicEaseIn,
            CubicEaseInOut,
            CubicEaseOutIn,
            QuartEaseOut,
            QuartEaseIn,
            QuartEaseInOut,
            QuartEaseOutIn,
            QuintEaseOut,
            QuintEaseIn,
            QuintEaseInOut,
            QuintEaseOutIn,
            CircEaseOut,
            CircEaseIn,
            CircEaseInOut,
            CircEaseOutIn,
            SineEaseOut,
            SineEaseIn,
            SineEaseInOut,
            SineEaseOutIn,
            ElasticEaseOut,
            ElasticEaseIn,
            ElasticEaseInOut,
            ElasticEaseOutIn,
            BounceEaseOut,
            BounceEaseIn,
            BounceEaseInOut,
            BounceEaseOutIn,
            BackEaseOut,
            BackEaseIn,
            BackEaseInOut,
            BackEaseOutIn,

            Last = BackEaseOutIn,
        }
        Equations equation;
        public Equations Equation
        {
            set
            {
                this.equation = value;
                this.easeFunc = Ease.Functions[(int)this.equation];
            }
            get { return this.equation; }
        }


        // NOTE: GC Spiked
        //public static readonly int EquationCount = Enum.GetValues(typeof(Equations)).Length;
        public static readonly int EquationCount = (int)Equations.Last + 1;

        EaseFunc easeFunc;

        

        public static readonly float PI = (float)Math.PI;
        public static readonly float PI_HALF = (float)(Math.PI * 0.5);


        public delegate float EaseFunc(float t);
        public static readonly EaseFunc[] Functions = new EaseFunc[]
        {
            Ease.Linear,
            Ease.QuadEaseOut,
            Ease.QuadEaseIn,
            Ease.QuadEaseInOut,
            Ease.QuadEaseOutIn,
            Ease.ExpoEaseOut,
            Ease.ExpoEaseIn,
            Ease.ExpoEaseInOut,
            Ease.ExpoEaseOutIn,
            Ease.CubicEaseOut,
            Ease.CubicEaseIn,
            Ease.CubicEaseInOut,
            Ease.CubicEaseOutIn,
            Ease.QuartEaseOut,
            Ease.QuartEaseIn,
            Ease.QuartEaseInOut,
            Ease.QuartEaseOutIn,
            Ease.QuintEaseOut,
            Ease.QuintEaseIn,
            Ease.QuintEaseInOut,
            Ease.QuintEaseOutIn,
            Ease.CircEaseOut,
            Ease.CircEaseIn,
            Ease.CircEaseInOut,
            Ease.CircEaseOutIn,
            Ease.SineEaseOut,
            Ease.SineEaseIn,
            Ease.SineEaseInOut,
            Ease.SineEaseOutIn,
            Ease.ElasticEaseOut,
            Ease.ElasticEaseIn,
            Ease.ElasticEaseInOut,
            Ease.ElasticEaseOutIn,
            Ease.BounceEaseOut,
            Ease.BounceEaseIn,
            Ease.BounceEaseInOut,
            Ease.BounceEaseOutIn,
            Ease.BackEaseOut,
            Ease.BackEaseIn,
            Ease.BackEaseInOut,
            Ease.BackEaseOutIn
        };

        public static float Clamp01(float t)
        {
            return 0 > t ? 0 : 1 < t ? 1 : t;
        }


        // NOTE: Linear
        public static float Linear(float t)
        {
            return t;
        }


        // NOTE: Expo
        public static float ExpoEaseOut(float t)
        {
            return (1 == t) ? 1 : -(float)Math.Pow(2, -10 * t) + 1;
        }

        public static float ExpoEaseIn(float t)
        {
            return (0 == t) ? 0 : (float)Math.Pow(2, 10 * (t - 1));
        }

        public static float ExpoEaseInOut(float t)
        {
            return (t < 0.5f) ? Ease.ExpoEaseIn(t * 2) * .5f : Ease.ExpoEaseOut((t * 2) - 1) * .5f + .5f;
        }

        public static float ExpoEaseOutIn(float t)
        {
            return (t < 0.5f) ? Ease.ExpoEaseOut(t * 2) * .5f : Ease.ExpoEaseIn((t * 2) - 1) * .5f + .5f;
        }


        // NOTE: Circ
        public static float CircEaseOut(float t)
        {
            return (float)Math.Sqrt(1 - (t = t - 1) * t);
        }

        public static float CircEaseIn(float t)
        {
            return -((float)Math.Sqrt(1 - t * t) - 1);
        }

        public static float CircEaseInOut(float t)
        {
            return (t < 0.5f) ? Ease.CircEaseIn(t * 2) * .5f : Ease.CircEaseOut((t * 2) - 1) * .5f + .5f;
        }

        public static float CircEaseOutIn(float t)
        {
            return (t < 0.5f) ? Ease.CircEaseOut(t * 2) * .5f : Ease.CircEaseIn((t * 2) - 1) * .5f + .5f;
        }


        // NOTE: Quad
        public static float QuadEaseOut(float t)
        {
            return -(t * (t - 2));
        }

        public static float QuadEaseIn(float t)
        {
            return t * t;
        }

        public static float QuadEaseInOut(float t)
        {
            return (t < 0.5f) ? Ease.QuadEaseIn(t * 2) * .5f : Ease.QuadEaseOut((t * 2) - 1) * .5f + .5f;
        }

        public static float QuadEaseOutIn(float t)
        {
            return (t < 0.5f) ? Ease.QuadEaseOut(t * 2) * .5f : Ease.QuadEaseIn((t * 2) - 1) * .5f + .5f;
        }


        // NOTE: Sine
        public static float SineEaseOut(float t)
        {
            return (float)Math.Sin(t * Ease.PI_HALF);
        }

        public static float SineEaseIn(float t)
        {
            return (float)(1 - Math.Cos(t * Ease.PI_HALF));
        }

        public static float SineEaseInOut(float t)
        {
            return (t < 0.5f) ? Ease.SineEaseIn(t * 2) * .5f : Ease.SineEaseOut((t * 2) - 1) * .5f + .5f;
        }

        public static float SineEaseOutIn(float t)
        {
            return (t < 0.5f) ? Ease.SineEaseOut(t * 2) * .5f : Ease.SineEaseIn((t * 2) - 1) * .5f + .5f;
        }

        

        // NOTE: Cubic
        public static float CubicEaseOut(float t)
        {
            return ((t = t - 1) * t * t + 1);
        }

        public static float CubicEaseIn(float t)
        {
            return t * t * t;
        }

        public static float CubicEaseInOut(float t)
        {
            return (t < 0.5f) ? Ease.CubicEaseIn(t * 2) * .5f : Ease.CubicEaseOut((t * 2) - 1) * .5f + .5f;
        }

        public static float CubicEaseOutIn(float t)
        {
            return (t < 0.5f) ? Ease.CubicEaseOut(t * 2) * .5f : Ease.CubicEaseIn((t * 2) - 1) * .5f + .5f;
        }


        // NOTE: Quartic
        public static float QuartEaseOut(float t)
        {
            return -((t = t - 1) * t * t * t - 1);
        }

        public static float QuartEaseIn(float t)
        {
            return t * t * t * t;
        }

        public static float QuartEaseInOut(float t)
        {
            return (t < 0.5f) ? Ease.QuartEaseIn(t * 2) * .5f : Ease.QuartEaseOut((t * 2) - 1) * .5f + .5f;
        }

        public static float QuartEaseOutIn(float t)
        {
            return (t < 0.5f) ? Ease.QuartEaseOut(t * 2) * .5f : Ease.QuartEaseIn((t * 2) - 1) * .5f + .5f;
        }

        

        // NOTE: Quintic
        public static float QuintEaseOut(float t)
        {
            return ((t = t - 1) * t * t * t * t + 1);
        }

        public static float QuintEaseIn(float t)
        {
            return t * t * t * t * t;
        }

        public static float QuintEaseInOut(float t)
        {
            return (t < 0.5f) ? Ease.QuintEaseIn(t * 2) * .5f : Ease.QuintEaseOut((t * 2) - 1) * .5f + .5f;
        }

        public static float QuintEaseOutIn(float t)
        {
            return (t < 0.5f) ? Ease.QuintEaseOut(t * 2) * .5f : Ease.QuintEaseIn((t * 2) - 1) * .5f + .5f;
        }

        

        // NOTE: Elastic
        public static float ElasticEaseOut(float t)
        {
            if (t == 1)
                return 1;

            float p = .3f;
            float s = p / 4;

            return ((float)Math.Pow(2, -10 * t) * (float)Math.Sin((t - s) * (2 * Ease.PI) / p) + 1 + 0);
        }

        public static float ElasticEaseIn(float t)
        {
            if (t == 1)
                return 1;

            float p = .3f;
            float s = p / 4;

            return -((float)Math.Pow(2, 10 * (t -= 1)) * (float)Math.Sin((t - s) * (2 * Ease.PI) / p));
        }

        public static float ElasticEaseInOut(float t)
        {
            return (t < 0.5f) ? Ease.ElasticEaseIn(t * 2) * .5f : Ease.ElasticEaseOut((t * 2) - 1) * .5f + .5f;
        }

        public static float ElasticEaseOutIn(float t)
        {
            return (t < 0.5f) ? Ease.ElasticEaseOut(t * 2) * .5f : Ease.ElasticEaseIn((t * 2) - 1) * .5f + .5f;
        }

        
        // NOTE: Bounce
        public static float BounceEaseOut(float t)
        {
            if (t < (0.363636f))
                return (7.5625f * t * t);
            else if (t < 0.727273f)
                return (7.5625f * (t -= 0.545455f) * t + .75f);
            else if (t < 0.909091f)
                return (7.5625f * (t -= 0.818182f) * t + .9375f);
            else
                return (7.5625f * (t -= 0.954545f) * t + .984375f);
        }

        public static float BounceEaseIn(float t)
        {
            return 1 - Ease.BounceEaseOut(1 - t);
        }

        public static float BounceEaseInOut(float t)
        {
            return (t < 0.5f) ? Ease.BounceEaseIn(t * 2) * .5f : Ease.BounceEaseOut(t * 2 - 1) * .5f + .5f;
        }

        public static float BounceEaseOutIn(float t)
        {
            return (t < 0.5f) ? Ease.BounceEaseOut(t * 2) * .5f : Ease.BounceEaseIn((t * 2) - 1) * .5f + .5f;
        }

        

        // NOTE: Back
        public static float BackEaseOut(float t)
        {
            return ((t = t - 1) * t * ((1.70158f + 1) * t + 1.70158f) + 1);
        }

        public static float BackEaseIn(float t)
        {
            return t * t * ((1.70158f + 1) * t - 1.70158f);
        }

        public static float BackEaseInOut(float t)
        {
            float s = 1.70158f;
            if ((t *= 2) < 1)
                return 0.5f * (t * t * (((s *= (1.525f)) + 1) * t - s));
            return 0.5f * ((t -= 2) * t * (((s *= (1.525f)) + 1) * t + s) + 2);
        }

        public static float BackEaseOutIn(float t)
        {
            return (t < 0.5f) ? Ease.BackEaseOut(t * 2) * .5f : Ease.BackEaseIn((t * 2) - 1) * .5f + .5f;
        }

        public override string ToString()
        {
            return string.Format("Ease:{{Equation:{0}}}", this.equation);
        }
    }


    public class Constant : IInterpolable
    {
        public float Interpolate(float t) { return t < 1 ? 0 : 1; }

        public bool IsClamped01 { get { return false; } }

        private Constant() { }

        public static readonly Constant Instance = new Constant();

        public override string ToString()
        {
            return "Constant";
        }
    }
}
