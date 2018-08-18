using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Ext.Unity3D.UI;
using Ext.Unity3D;

namespace Ext.UnityEdit.UI
{
    [CustomPropertyDrawer(typeof(UICache))]
    public class UICachePropertyDrawer : PropertyDrawer
    {
        List<SerializedProperty> properties = new List<SerializedProperty>(32);
        
        protected virtual void ReadProperties(SerializedProperty source, List<SerializedProperty> properties)
        {
            properties.Add(source.FindPropertyRelative("cachedRectTransform"));
            properties.Add(source.FindPropertyRelative("animation"));
            properties.Add(source.FindPropertyRelative("graphic"));
        }

        protected virtual void ResetProperties(UICache source, List<SerializedProperty> properties, ref int index)
        {
            properties[index++].objectReferenceValue = source.CachedRectTransform;
            properties[index++].objectReferenceValue = source.Animation;
            properties[index++].objectReferenceValue = source.Graphic;
        }

        protected virtual UICache CreateCache(GameObject go)
        {
            return new UICache(go);
        }

        protected virtual void OnDrawProperties(List<SerializedProperty> properties, Rect contentPosition, float defaultHeight)
        {
            var defaultColor = GUI.color;

            foreach (var property in properties)
            {
                contentPosition = this.OnDrawProperty(property, contentPosition, defaultHeight, defaultColor);
            }

            GUI.color = defaultColor;
        }

        protected virtual Rect OnDrawProperty(SerializedProperty property, Rect contentPosition, float defaultHeight, Color defaultColor)
        {
            contentPosition.y += defaultHeight + 2;
            var available = property.objectReferenceValue;
            GUI.color = available ? Color.green : defaultColor;
            EditorGUI.LabelField(contentPosition, string.Format("{0}: {1}", property.name, available ? "ON" : "OFF"));
            return contentPosition;
        }

        public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(pos, label, property);
            var contentPosition = EditorGUI.PrefixLabel(pos, label);
            var w = contentPosition.width;
            contentPosition.width *= 0.75f;

            var defaultHeight = base.GetPropertyHeight(property, label);
            contentPosition.height = defaultHeight;
            EditorGUI.indentLevel = 0;

            var propGo = property.FindPropertyRelative("gameObject");
            this.properties.Clear();
            this.ReadProperties(property, this.properties);

            var obj = propGo.objectReferenceValue;
            var go = obj != null ? (GameObject)obj : null;
            var goAfter = (GameObject)EditorGUI.ObjectField(contentPosition, go, typeof(GameObject), true);
            if (go != goAfter)
            {
                Debug.Log("UICACHE_RESET:" + property.displayName);
                propGo.objectReferenceValue = goAfter;
                int index = 0;
                this.ResetProperties(this.CreateCache(goAfter), this.properties, ref index);
            }

            Rect foldPos = contentPosition;
            foldPos.x += w * 0.8f;
            foldPos.width = w * 0.2f;
            SerializedProperty propFold = property.FindPropertyRelative("editorFoldout");
            propFold.boolValue = EditorGUI.Foldout(foldPos, propFold.boolValue, "info", true);
            if (propFold.boolValue)
            {
                this.OnDrawProperties(this.properties, contentPosition, defaultHeight);
            }

            EditorGUI.EndProperty();
        }

        protected virtual float GetTotalHeight(SerializedProperty property, float defaultHeight)
        {
            return (defaultHeight + 2) * 3;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var propFold = property.FindPropertyRelative("editorFoldout");
            float defaultHeight = base.GetPropertyHeight(property, label);
            return defaultHeight + (propFold.boolValue ? this.GetTotalHeight(property, defaultHeight) : 0);
        }
    }
}