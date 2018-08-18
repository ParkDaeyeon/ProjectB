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
    [CustomEditor(typeof(OptimizedRectImage), true), CanEditMultipleObjects, InitializeOnLoad]
    public class OptimizedRectImageEditor : OptimizedImageEditor
    {
        SerializedObject sobj;
        SerializedProperty uvOffset;
        SerializedProperty uvScale;
        SerializedProperty uvPivot;

        protected override void OnEnable()
        {
            base.OnEnable();

            this.sobj = new SerializedObject(this.targets);
            this.uvOffset = this.sobj.FindProperty("uvOffset");
            this.uvScale = this.sobj.FindProperty("uvScale");
            this.uvPivot = this.sobj.FindProperty("uvPivot");
        }

        public override void OnInspectorOptimizedImage()
        {
            base.OnInspectorOptimizedImage();

            var targets = this.sobj.targetObjects;

            this.sobj.Update();

            EditorGUI.showMixedValue = this.uvOffset.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            var uvOffset = EditorGUILayout.Vector2Field("UV Offset", this.uvOffset.vector2Value);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                this.uvOffset.vector2Value = uvOffset;
                foreach (var target in this.targets)
                {
                    var image = target as OptimizedRectImage;
                    image.UvOffset = uvOffset;
                }
                this.sobj.ApplyModifiedProperties();
                this.sobj.Update();
            }

            EditorGUI.showMixedValue = this.uvScale.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            var uvScale = EditorGUILayout.Vector2Field("UV Scale", this.uvScale.vector2Value);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                this.uvScale.vector2Value = uvScale;
                foreach (var target in this.targets)
                {
                    var image = target as OptimizedRectImage;
                    image.UvScale = uvScale;
                }
                this.sobj.ApplyModifiedProperties();
                this.sobj.Update();
            }

            EditorGUI.showMixedValue = this.uvPivot.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            var uvPivot = EditorGUILayout.Vector2Field("UV Pivot", this.uvPivot.vector2Value);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                this.uvPivot.vector2Value = uvPivot;
                foreach (var target in this.targets)
                {
                    var image = target as OptimizedRectImage;
                    image.UvPivot = uvPivot;
                }
                this.sobj.ApplyModifiedProperties();
                this.sobj.Update();
            }

            if (GUILayout.Button("Update Uv Rect"))
            {
                foreach (var target in this.targets)
                {
                    var image = target as OptimizedRectImage;
                    image.UpdateUvRect();
                }
                this.sobj.ApplyModifiedProperties();
                this.sobj.Update();
            }
        }
    }
}
