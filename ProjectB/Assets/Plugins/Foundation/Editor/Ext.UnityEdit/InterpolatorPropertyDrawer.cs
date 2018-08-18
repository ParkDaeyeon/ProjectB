using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Ext.Algorithm;
using Ext.Unity3D;
using Ext.Unity3D.UI;

namespace Ext.UnityEdit
{
    [CustomPropertyDrawer(typeof(Interpolator))]
    public class InterpolatorPropertyDrawer : PropertyDrawer
    {
        const float heightField = 20;
        const float heightCurve = 100;

        protected class Maker
        {
            SerializedProperty baseProperty;
            public SerializedProperty BaseProperty { get { return this.baseProperty; } }


            SerializedProperty algorithmProperty;
            public SerializedProperty AlgorithmProperty { get { return this.algorithmProperty; } }
            public Interpolator.AlgorithmType Algorithm
            {
                set { this.algorithmProperty.enumValueIndex = (int)value; }
                get { return (Interpolator.AlgorithmType)this.algorithmProperty.enumValueIndex; }
            }

            SerializedProperty floatValuesProperty;
            public SerializedProperty FloatValuesProperty { get { return this.floatValuesProperty; } }
            public float[] FloatValues
            {
                set
                {
                    var count = this.floatValuesProperty.arraySize = null != value ? value.Length : 0;
                    for (int n = 0; n < count; ++n)
                        this.floatValuesProperty.GetArrayElementAtIndex(n).floatValue = value[n];
                }
                get
                {
                    var value = new float[this.floatValuesProperty.arraySize];
                    for (int n = 0; n < value.Length; ++n)
                        value[n] = this.floatValuesProperty.GetArrayElementAtIndex(n).floatValue;

                    return value;
                }
            }

            SerializedProperty intValueProperty;
            public SerializedProperty IntValueProperty { get { return this.intValueProperty; } }
            public int IntValue
            {
                set { this.intValueProperty.intValue = value; }
                get { return this.intValueProperty.intValue; }
            }


            SerializedProperty editorAlgorithmProperty;
            public SerializedProperty EditorAlgorithmProperty { get { return this.editorAlgorithmProperty; } }
            public Interpolator.AlgorithmType EditorAlgorithm
            {
                set
                {
                    this.editorAlgorithmProperty.enumValueIndex = (int)value;
                    this.Apply();
                }
                get { return (Interpolator.AlgorithmType)this.editorAlgorithmProperty.enumValueIndex; }
            }

            public float[] MakeFloatValues()
            {
                switch (this.EditorAlgorithm)
                {
                case Interpolator.AlgorithmType.Acceleration:
                    return new float[] { this.EditorVarFactor, };

                case Interpolator.AlgorithmType.Bezier3:
                    return new float[] { this.EditorVarP1, };

                case Interpolator.AlgorithmType.Bezier4:
                    return new float[] { this.EditorVarP1, this.EditorVarP2 };

                case Interpolator.AlgorithmType.Spline:
                    {
                        var points = this.EditorVarPoints;
                        var count = null != points ? points.Length : 0;
                        var value = new float[count * 2];
                        for (int n = 0; n < count; ++n)
                        {
                            var point = points[n];
                            value[(n * 2)] = point.key;
                            value[(n * 2) + 1] = point.value;
                        }
                        return value;
                    }
                }

                return null;
            }

            public int MakeIntValue()
            {
                switch (this.EditorAlgorithm)
                {
                case Interpolator.AlgorithmType.Ease:
                    return (int)this.EditorVarEquation;
                }

                return 0;
            }

            SerializedProperty editorVarFactorProperty;
            public SerializedProperty EditorVarFactorProperty { get { return this.editorVarFactorProperty; } }
            public float EditorVarFactor
            {
                set
                {
                    this.editorVarFactorProperty.floatValue = value;
                    this.Apply();
                }
                get { return this.editorVarFactorProperty.floatValue; }
            }

            SerializedProperty editorVarP1Property;
            public SerializedProperty EditorVarP1Property { get { return this.editorVarP1Property; } }
            public float EditorVarP1
            {
                set
                {
                    this.editorVarP1Property.floatValue = value;
                    this.Apply();
                }
                get { return this.editorVarP1Property.floatValue; }
            }

            SerializedProperty editorVarP2Property;
            public SerializedProperty EditorVarP2Property { get { return this.editorVarP2Property; } }
            public float EditorVarP2
            {
                set
                {
                    this.editorVarP2Property.floatValue = value;
                    this.Apply();
                }
                get { return this.editorVarP2Property.floatValue; }
            }

            SerializedProperty editorVarPointsProperty;
            public SerializedProperty EditorVarPointsProperty { get { return this.editorVarPointsProperty; } }
            public Interpolator.SplinePoint[] EditorVarPoints
            {
                set
                {
                    var count = this.editorVarPointsProperty.arraySize = null != value ? value.Length : 0;
                    for (int n = 0; n < count; ++n)
                    {
                        var pointProperty = this.editorVarPointsProperty.GetArrayElementAtIndex(n);
                        var point = value[n];
                        pointProperty.FindPropertyRelative("key").floatValue = point.key;
                        pointProperty.FindPropertyRelative("value").floatValue = point.value;
                    }
                    this.Apply();
                }
                get
                {
                    var count = this.editorVarPointsProperty.arraySize;
                    var value = new Interpolator.SplinePoint[count];
                    for (int n = 0; n < count; ++n)
                    {
                        var pointProperty = this.editorVarPointsProperty.GetArrayElementAtIndex(n);
                        value[n] = new Interpolator.SplinePoint
                        {
                            key = pointProperty.FindPropertyRelative("key").floatValue,
                            value = pointProperty.FindPropertyRelative("value").floatValue,
                        };
                    }

                    return value;
                }
            }

            SerializedProperty editorVarEquationProperty;
            public SerializedProperty EditorVarEquationProperty { get { return this.editorVarEquationProperty; } }
            public Ease.Equations EditorVarEquation
            {
                set
                {
                    this.editorVarEquationProperty.enumValueIndex = (int)value;
                    this.Apply();
                }
                get { return (Ease.Equations)this.editorVarEquationProperty.enumValueIndex; }
            }

            public Maker(SerializedProperty property)
            {
                this.baseProperty = property;

                this.algorithmProperty = property.FindPropertyRelative("algorithm");
                this.floatValuesProperty = property.FindPropertyRelative("floatValues");
                this.intValueProperty = property.FindPropertyRelative("intValue");

                this.editorAlgorithmProperty = property.FindPropertyRelative("editorAlgorithm");
                this.editorVarFactorProperty = property.FindPropertyRelative("editorVarFactor");
                this.editorVarP1Property = property.FindPropertyRelative("editorVarP1");
                this.editorVarP2Property = property.FindPropertyRelative("editorVarP2");
                this.editorVarPointsProperty = property.FindPropertyRelative("editorVarPoints");
                this.editorVarEquationProperty = property.FindPropertyRelative("editorVarEquation");
            }

            public void Apply()
            {
                var algorithm = this.EditorAlgorithm;
                var floatValues = this.MakeFloatValues();
                var intValue = this.MakeIntValue();
                try
                {
                    Interpolator.Verify(algorithm, floatValues, intValue);
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                    return;
                }

                this.Algorithm = algorithm;
                this.FloatValues = floatValues;
                this.IntValue = intValue;
                EditorUtility.SetDirty(this.baseProperty.serializedObject.targetObject);
            }

            public Interpolator DummyInstance
            {
                get
                {
                    try
                    {
                        return Interpolator.Create(this.Algorithm, this.FloatValues, this.IntValue);
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning(e);
                        return null;
                    }
                }
            }
        }



        public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(pos, label, property);
            var contentRect = EditorGUI.PrefixLabel(pos, label);

            var maker = new Maker(property);
            
            var algorithm = this.OnGUIEnum(ref contentRect, maker.EditorAlgorithm, (newValue) =>
            {
                maker.EditorAlgorithm = newValue;
            });

            switch (algorithm)
            {
            case Interpolator.AlgorithmType.Linear:
                this.OnGUILinear(ref contentRect, maker);
                break;

            case Interpolator.AlgorithmType.Acceleration:
                this.OnGUIAcceleration(ref contentRect, maker);
                break;

            case Interpolator.AlgorithmType.Bezier3:
                this.OnGUIBezier3(ref contentRect, maker);
                break;

            case Interpolator.AlgorithmType.Bezier4:
                this.OnGUIBezier4(ref contentRect, maker);
                break;

            case Interpolator.AlgorithmType.Spline:
                this.OnGUISpline(ref contentRect, maker);
                break;

            case Interpolator.AlgorithmType.Ease:
                this.OnGUIEase(ref contentRect, maker);
                break;

            case Interpolator.AlgorithmType.Constant:
                this.OnGUIConstant(ref contentRect, maker);
                break;

            case Interpolator.AlgorithmType.Custom:
                this.OnGUICustom(ref contentRect, maker);
                break;
            }

            EditorGUI.EndProperty();
        }
        

        protected virtual void OnGUICurve(ref Rect contentRect, Interpolator dummyInstance, AnimationCurveUtility.TangentMode mode = AnimationCurveUtility.TangentMode.Smooth)
        {
            if (null == dummyInstance)
                return;

            var count = 100;
            var keyFrames = new Keyframe[count];
            for (int n = 0; n < count; ++n)
            {
                var t = n / (float)(count - 1);
                var k = new Keyframe(t, dummyInstance.Interpolate(t));
                keyFrames[n] = k;
            }

            var curve = new AnimationCurve(keyFrames);
            AnimationCurveUtility.SetLinear(ref curve, mode);

            contentRect.height = InterpolatorPropertyDrawer.heightCurve;
            EditorGUI.CurveField(contentRect, curve);
            contentRect.y += InterpolatorPropertyDrawer.heightCurve;
        }

        protected float OnGUIFloat(ref Rect contentRect, float oldValue, string fieldName, Action<float> changeCallback)
        {
            contentRect.height = InterpolatorPropertyDrawer.heightField;
            var newValue = EditorGUI.FloatField(contentRect, fieldName, oldValue);
            contentRect.y += InterpolatorPropertyDrawer.heightField;

            if (oldValue != newValue)
            {
                if (null != changeCallback)
                    changeCallback(newValue);
            }

            return newValue;
        }

        protected int OnGUIInt(ref Rect contentRect, int oldValue, string fieldName, Action<int> changeCallback)
        {
            contentRect.height = InterpolatorPropertyDrawer.heightField;
            var newValue = EditorGUI.IntField(contentRect, fieldName, oldValue);
            contentRect.y += InterpolatorPropertyDrawer.heightField;

            if (oldValue != newValue)
            {
                if (null != changeCallback)
                    changeCallback(newValue);
            }

            return newValue;
        }

        protected T OnGUIEnum<T>(ref Rect contentRect, T oldValue, Action<T> changeCallback)
        {
            contentRect.height = InterpolatorPropertyDrawer.heightField;
            var newValue = (T)(object)EditorGUI.EnumPopup(contentRect, (Enum)(object)oldValue);
            contentRect.y += InterpolatorPropertyDrawer.heightField;
            
            if (oldValue.GetHashCode() != newValue.GetHashCode())
            {
                if (null != changeCallback)
                    changeCallback(newValue);
            }

            return newValue;
        }


        protected virtual void OnGUILinear(ref Rect contentRect, Maker maker)
        {
            this.OnGUICurve(ref contentRect, maker.DummyInstance);
        }
        protected virtual void OnGUIAcceleration(ref Rect contentRect, Maker maker)
        {
            this.OnGUIFloat(ref contentRect, maker.EditorVarFactor, "Factor", (newValue) =>
            {
                maker.EditorVarFactor = newValue;
            });
            this.OnGUICurve(ref contentRect, maker.DummyInstance);
        }
        protected virtual void OnGUIBezier3(ref Rect contentRect, Maker maker)
        {
            this.OnGUIFloat(ref contentRect, maker.EditorVarP1, "P1", (newValue) =>
            {
                maker.EditorVarP1 = newValue;
            });
            this.OnGUICurve(ref contentRect, maker.DummyInstance);
        }
        protected virtual void OnGUIBezier4(ref Rect contentRect, Maker maker)
        {
            this.OnGUIFloat(ref contentRect, maker.EditorVarP1, "P1", (newValue) =>
            {
                maker.EditorVarP1 = newValue;
            });
            this.OnGUIFloat(ref contentRect, maker.EditorVarP2, "P2", (newValue) =>
            {
                maker.EditorVarP2 = newValue;
            });
            this.OnGUICurve(ref contentRect, maker.DummyInstance);
        }
        protected virtual void OnGUISpline(ref Rect contentRect, Maker maker)
        {
            var editorVarPointsProperty = maker.EditorVarPointsProperty;

            contentRect.height = InterpolatorPropertyDrawer.heightField;
            var oldSize = editorVarPointsProperty.arraySize;
            var newSize = Mathf.Max(EditorGUI.IntField(contentRect, "Count", oldSize), 3);
            contentRect.y += InterpolatorPropertyDrawer.heightField;
            

            if (oldSize != newSize)
            {
                editorVarPointsProperty.arraySize = newSize;
                maker.Apply();
            }

            var x = contentRect.xMin;
            var w = contentRect.width;
            var wh = w * 0.5f;
            contentRect.width = wh;
            for (int n = 0; n < newSize; ++n)
            {
                var elemProperty = editorVarPointsProperty.GetArrayElementAtIndex(n);
                var keyProperty = elemProperty.FindPropertyRelative("key");
                var valueProperty = elemProperty.FindPropertyRelative("value");
                var y = contentRect.yMin;
                this.OnGUIFloat(ref contentRect, keyProperty.floatValue, string.Format("[{0}] Key", n), (newValue) =>
                {
                    keyProperty.floatValue = newValue;
                    maker.Apply();
                });
                contentRect.xMin += wh;
                contentRect.yMin = y;
                this.OnGUIFloat(ref contentRect, elemProperty.FindPropertyRelative("value").floatValue, "Value", (newValue) =>
                {
                    valueProperty.floatValue = newValue;
                    maker.Apply();
                });
                contentRect.xMin = x;
            }
            contentRect.width = w;

            this.OnGUICurve(ref contentRect, maker.DummyInstance);
        }
        protected virtual void OnGUIEase(ref Rect contentRect, Maker maker)
        {
            this.OnGUIEnum(ref contentRect, maker.EditorVarEquation, (newValue) =>
            {
                maker.EditorVarEquation = newValue;
            });
            this.OnGUICurve(ref contentRect, maker.DummyInstance);
        }
        protected virtual void OnGUIConstant(ref Rect contentRect, Maker maker)
        {
            this.OnGUICurve(ref contentRect, maker.DummyInstance, AnimationCurveUtility.TangentMode.Stepped);
        }
        protected virtual void OnGUICustom(ref Rect contentRect, Maker maker)
        {
            this.OnGUICurve(ref contentRect, maker.DummyInstance);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = base.GetPropertyHeight(property, label);

            var maker = new Maker(property);
            switch (maker.EditorAlgorithm)
            {
            case Interpolator.AlgorithmType.Acceleration:
            case Interpolator.AlgorithmType.Bezier3:
            case Interpolator.AlgorithmType.Ease:
                height += InterpolatorPropertyDrawer.heightField;
                break;

            case Interpolator.AlgorithmType.Bezier4:
                height += InterpolatorPropertyDrawer.heightField * 2;
                break;

            case Interpolator.AlgorithmType.Spline:
                height += InterpolatorPropertyDrawer.heightField * (maker.EditorVarPointsProperty.arraySize + 1);
                break;
            }

            height += InterpolatorPropertyDrawer.heightCurve;
            height += 5;// NOTE: padding
            return height;
        }
    }
}