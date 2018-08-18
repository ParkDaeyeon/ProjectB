using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Ext.Unity3D.UI;
namespace Ext.UnityEdit.UI
{
    [CustomEditor(typeof(OptimizedFilledImage), true), CanEditMultipleObjects, InitializeOnLoad]
    public class OptimizedFilledImageEditor : OptimizedImageEditor
    {
        SerializedObject sobj;
        SerializedProperty fillAmountOptimized;
        SerializedProperty borderMin;
        SerializedProperty borderMax;
        SerializedProperty pixelPerfect;

        protected override void OnEnable()
        {
            base.OnEnable();

            this.sobj = new SerializedObject(this.targets);
            this.fillAmountOptimized = this.sobj.FindProperty("fillAmountOptimized");
            this.borderMin = this.sobj.FindProperty("borderMin");
            this.borderMax = this.sobj.FindProperty("borderMax");
            this.pixelPerfect = this.sobj.FindProperty("pixelPerfect");
        }

        public override void OnInspectorOptimizedImage()
        {
            base.OnInspectorOptimizedImage();

            var targets = this.sobj.targetObjects;

            this.sobj.Update();

            EditorGUI.showMixedValue = this.fillAmountOptimized.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            var fillAmountOptimized = EditorGUILayout.Slider("Fill Amount (Optimized)", this.fillAmountOptimized.floatValue, 0, 1);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                this.fillAmountOptimized.floatValue = fillAmountOptimized;
                foreach (var target in this.targets)
                {
                    var image = target as OptimizedFilledImage;
                    image.fillAmount = fillAmountOptimized;
                }
                this.sobj.ApplyModifiedProperties();
                this.sobj.Update();
            }


            EditorGUI.showMixedValue = this.borderMin.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            var borderMin = EditorGUILayout.Vector2Field("Border Min", this.borderMin.vector2Value);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                this.borderMin.vector2Value = borderMin;
                this.sobj.ApplyModifiedProperties();
                this.sobj.Update();
            }


            EditorGUI.showMixedValue = this.borderMax.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            var borderMax = EditorGUILayout.Vector2Field("Border Max", this.borderMax.vector2Value);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                this.borderMax.vector2Value = borderMax;
                this.sobj.ApplyModifiedProperties();
                this.sobj.Update();
            }



            EditorGUI.showMixedValue = this.pixelPerfect.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            var pixelPerfect = EditorGUILayout.Toggle("Pixel Perfect", this.pixelPerfect.boolValue);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                this.pixelPerfect.boolValue = pixelPerfect;
                this.sobj.ApplyModifiedProperties();
                this.sobj.Update();
            }
        }
    }
}
