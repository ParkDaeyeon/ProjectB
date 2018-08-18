using UnityEngine;
using System;
using System.Collections;
using System.Reflection;

namespace Ext.Unity3D
{
    /// <summary>
    /// Static utility class to work around lack of support for Keyframe.tangentMode
    /// This utility class mimics the functionality that happens behind the scenes in UnityEditor when you manipulate an AnimationCurve. All of this information
    /// was discovered via .net reflection, and thus relies on reflection to work
    /// --testure 09/05/2012
    /// </summary>
    public static class AnimationCurveUtility
    {
        public enum TangentMode
        {
            Editable,
            Smooth,
            Linear,
            Stepped
        }

        public enum TangentDirection
        {
            Left,
            Right
        }

        public static void SetLinear(ref AnimationCurve curve, TangentMode mode = TangentMode.Linear)
        {
            Type t = typeof(UnityEngine.Keyframe);
            FieldInfo field = t.GetField("m_TangentMode", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            for (int i = 0; i < curve.length; i++)
            {
                object boxed = curve.keys[i]; // getting around the fact that Keyframe is a struct by pre-boxing
                field.SetValue(boxed, GetNewTangentKeyMode((int)field.GetValue(boxed), TangentDirection.Left, mode));
                field.SetValue(boxed, GetNewTangentKeyMode((int)field.GetValue(boxed), TangentDirection.Right, mode));
                curve.MoveKey(i, (Keyframe)boxed);
                curve.SmoothTangents(i, 0f);
            }
        }

        public static int GetNewTangentKeyMode(int currentTangentMode, TangentDirection leftRight, TangentMode mode)
        {
            int output = currentTangentMode;

            if (leftRight == TangentDirection.Left)
            {
                output &= -7;
                output |= ((int)mode) << 1;
            }
            else
            {
                output &= -25;
                output |= ((int)mode) << 3;
            }
            return output;
        }
    }
}
