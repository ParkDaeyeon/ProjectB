using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Ext.Unity3D;
using Ext.Unity3D.UI;

namespace Ext.UnityEdit.UI
{
    [CustomEditor(typeof(Gauge), true), CanEditMultipleObjects, InitializeOnLoad]
    public class GaugeEditor : OptimizedFilledImageEditor
    {
        SerializedObject sobj;
        SerializedProperty tail;
        SerializedProperty tailOffsetMin;
        SerializedProperty tailOffsetMax;
        SerializedProperty alwaysShowTail;
        SerializedProperty alwaysShowTailEnd;
        SerializedProperty tailForFillAmount;

        protected override void OnEnable()
        {
            base.OnEnable();

            this.sobj = new SerializedObject(this.targets);
            this.tail = this.sobj.FindProperty("tail");
            this.tailOffsetMin = this.sobj.FindProperty("tailOffsetMin");
            this.tailOffsetMax = this.sobj.FindProperty("tailOffsetMax");
            this.alwaysShowTail = this.sobj.FindProperty("alwaysShowTail");
            this.alwaysShowTailEnd = this.sobj.FindProperty("alwaysShowTailEnd");
            this.tailForFillAmount = this.sobj.FindProperty("tailForFillAmount");
        }

        public override void OnInspectorOptimizedImage()
        {
            base.OnInspectorOptimizedImage();

            var targets = this.sobj.targetObjects;

            this.sobj.Update();

            EditorGUI.showMixedValue = this.tail.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            var tail = (RectTransform)EditorGUILayout.ObjectField("Tail", this.tail.objectReferenceValue, typeof(RectTransform), true);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                this.tail.objectReferenceValue = tail;
                this.sobj.ApplyModifiedProperties();
                this.sobj.Update();
            }

            EditorGUI.showMixedValue = this.tailOffsetMin.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            var tailOffsetMin = EditorGUILayout.FloatField("Tail Offset Min", this.tailOffsetMin.floatValue);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                this.tailOffsetMin.floatValue = tailOffsetMin;
                this.sobj.ApplyModifiedProperties();
                this.sobj.Update();
            }

            EditorGUI.showMixedValue = this.tailOffsetMax.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            var tailOffsetMax = EditorGUILayout.FloatField("Tail Offset Max", this.tailOffsetMax.floatValue);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                this.tailOffsetMax.floatValue = tailOffsetMax;
                this.sobj.ApplyModifiedProperties();
                this.sobj.Update();
            }

            EditorGUI.showMixedValue = this.alwaysShowTail.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            var alwaysShowTail = EditorGUILayout.Toggle("Always Show Tail", this.alwaysShowTail.boolValue);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                this.alwaysShowTail.boolValue = alwaysShowTail;
                this.sobj.ApplyModifiedProperties();
                this.sobj.Update();
            }

            EditorGUI.showMixedValue = this.alwaysShowTailEnd.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            var alwaysShowTailEnd = EditorGUILayout.Toggle("Always Show Tail End", this.alwaysShowTailEnd.boolValue);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                this.alwaysShowTailEnd.boolValue = alwaysShowTailEnd;
                this.sobj.ApplyModifiedProperties();
                this.sobj.Update();
            }

            EditorGUI.showMixedValue = this.tailForFillAmount.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            var tailForFillAmount = EditorGUILayout.Toggle("Tail 4 Fill Amount", this.tailForFillAmount.boolValue);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                this.tailForFillAmount.boolValue = tailForFillAmount;
                this.sobj.ApplyModifiedProperties();
                this.sobj.Update();
            }
        }
    }
}
