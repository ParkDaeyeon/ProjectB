//using UnityEngine;
//using UnityEngine.UI;
//using UnityEditor;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Reflection;
//using System.Text;
//using Ext.Unity3D.UI;
//namespace Ext.UnityEdit.UI
//{
//    [CustomPropertyDrawer(typeof(RectCullObject))]
//    public class RectCullingObjectPropertyDrawer : PropertyDrawer
//    {
//        public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
//        {
//            label = EditorGUI.BeginProperty(pos, label, property);
//            Rect contentPosition = EditorGUI.PrefixLabel(pos, label);
//            float w = contentPosition.width;
//            contentPosition.width *= 0.75f;

//            float defaultHeight = base.GetPropertyHeight(property, label);
//            contentPosition.height = defaultHeight;
//            EditorGUI.indentLevel = 0;

//            this.OnDrawProperty_Sprite(contentPosition, property);

//            EditorGUI.EndProperty();
//        }


//        protected void OnDrawProperty_Sprite(Rect contentPosition, SerializedProperty property)
//        {
//            SerializedProperty propGraphic = property.FindPropertyRelative("graphic");
//            UnityEngine.Object originalObject = propGraphic.objectReferenceValue;
//            Graphic originalGraphic = originalObject != null ? (Graphic)originalObject : null;
//            Graphic newGraphic = (Graphic)EditorGUI.ObjectField(contentPosition, originalGraphic, typeof(Graphic));

//            if (originalGraphic != newGraphic)
//            {
//                Debug.Log("RECTCULLING_RESET_GRAPHIC:" + property.displayName);
//                propGraphic.objectReferenceValue = newGraphic;
//            }
//        }
//    }
//}