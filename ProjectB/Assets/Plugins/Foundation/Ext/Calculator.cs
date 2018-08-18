using System;

namespace Ext
{
    public static class Calculator
    {
        public enum Operator
        {
            GreatorOrEqual = 0,// NOTE: cond <= value
            Equal,// NOTE: cond == value
            LessOrEqual,// NOTE: cond >= value
        }

        public static bool Evaluation<A, B>(Operator oper, A target, B current)
            where B : IComparable<A>
        {
            var delta = current.CompareTo(target);

            switch (oper)
            {
            case Calculator.Operator.GreatorOrEqual:
                return 0 <= delta;

            case Calculator.Operator.Equal:
                return 0 == delta;

            case Calculator.Operator.LessOrEqual:
                return 0 >= delta;
            }

            return false;
        }

        public static double Progress(Operator oper, double target, double current)
        {
            if (0 >= target)
                return 0;

            switch (oper)
            {
            case Calculator.Operator.GreatorOrEqual:
                var delta = current / target;
                return 0 > delta ? 0 : 1 < delta ? 1 : delta;

            default:
                return Calculator.Evaluation(oper, target, current) ? 1 : 0;
            }
        }

        public static bool IsProgressable(Operator oper)
        {
            switch (oper)
            {
            case Calculator.Operator.GreatorOrEqual:
                return true;

            default:
                return false;
            }
        }
    }
}
