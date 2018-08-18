using UnityEditor;
using UnityEditor.UI;
using Ext.Unity3D.UI;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Ext.UnityEdit.UI
{
    [CustomEditor(typeof(MultipleColorFxButton), true), CanEditMultipleObjects, InitializeOnLoad]
    public class MultipleColorFxButtonEditor : ButtonEditor
    {
        SerializedObject sobj;
        SerializedProperty disableWithFX;

        protected override void OnEnable()
        {
            base.OnEnable();

            this.sobj = new SerializedObject(this.targets);
            this.disableWithFX = this.sobj.FindProperty("disableWithFX");
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.showMixedValue = this.disableWithFX.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            var disableWithFX = EditorGUILayout.Toggle("Disable With FX", this.disableWithFX.boolValue);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                this.disableWithFX.boolValue = disableWithFX;
                this.sobj.ApplyModifiedProperties();
                this.sobj.Update();
            }

            base.OnInspectorGUI();
        }
    }
}
    