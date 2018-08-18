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
    [CustomEditor(typeof(OptimizedQuadPolyImage), true), CanEditMultipleObjects, InitializeOnLoad]
    public class OptimizedQuadPolyImageEditor : OptimizedImageEditor
    {
        SerializedObject sobj;
        SerializedProperty fixToAffineUV;

        public struct VertexProperty
        {
            public SerializedProperty position;
            public SerializedProperty color;

            public VertexProperty(SerializedProperty vertex)
            {
                this.position = vertex.FindPropertyRelative("position");
                this.color = vertex.FindPropertyRelative("color");
            }
        }
        VertexProperty LT;
        VertexProperty RT;
        VertexProperty RB;
        VertexProperty LB;

        SerializedProperty uvOffset;
        SerializedProperty uvScale;
        SerializedProperty editorDrawGizmo;

        protected override void OnEnable()
        {
            base.OnEnable();

            this.sobj = new SerializedObject(this.targets);
            this.fixToAffineUV = this.sobj.FindProperty("fixToAffineUV");
            var quad = this.sobj.FindProperty("quad");
            this.LT = new VertexProperty(quad.FindPropertyRelative("LT"));
            this.RT = new VertexProperty(quad.FindPropertyRelative("RT"));
            this.RB = new VertexProperty(quad.FindPropertyRelative("RB"));
            this.LB = new VertexProperty(quad.FindPropertyRelative("LB"));
            this.uvOffset = this.sobj.FindProperty("uvOffset");
            this.uvScale = this.sobj.FindProperty("uvScale");
            this.editorDrawGizmo = this.sobj.FindProperty("editorDrawGizmo");
        }

        public override void OnInspectorOptimizedImage()
        {
            base.OnInspectorOptimizedImage();

            var targets = this.sobj.targetObjects;

            this.sobj.Update();

            EditorGUI.showMixedValue = this.fixToAffineUV.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            var fixToAffineUV = EditorGUILayout.Toggle("Fix To Affine UV", this.fixToAffineUV.boolValue);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                this.fixToAffineUV.boolValue = fixToAffineUV;
                this.sobj.ApplyModifiedProperties();
                this.sobj.Update();
            }

            this.OnInspecterVertexGUI(this.LT);
            this.OnInspecterVertexGUI(this.RT);
            this.OnInspecterVertexGUI(this.RB);
            this.OnInspecterVertexGUI(this.LB);

            EditorGUI.showMixedValue = this.uvOffset.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            var uvOffset = EditorGUILayout.Vector2Field("UV Offset", this.uvOffset.vector2Value);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                this.uvOffset.vector2Value = uvOffset;
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
                this.sobj.ApplyModifiedProperties();
                this.sobj.Update();
            }


            EditorGUI.showMixedValue = this.editorDrawGizmo.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            var editorDrawGizmo = EditorGUILayout.Toggle("Draw Gizmos", this.editorDrawGizmo.boolValue);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                this.editorDrawGizmo.boolValue = editorDrawGizmo;
                this.sobj.ApplyModifiedProperties();
                this.sobj.Update();
            }
        }

        protected virtual void OnInspecterVertexGUI(VertexProperty vertex)
        {
            EditorGUI.showMixedValue = vertex.position.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            var position = EditorGUILayout.Vector2Field("Position", vertex.position.vector2Value);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                vertex.position.vector2Value = position;
                this.sobj.ApplyModifiedProperties();
                this.sobj.Update();
            }


            EditorGUI.showMixedValue = vertex.color.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            var color = EditorGUILayout.ColorField("Color", vertex.color.colorValue);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                vertex.color.colorValue = color;
                this.sobj.ApplyModifiedProperties();
                this.sobj.Update();
            }
        }
    }
}
