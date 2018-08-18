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
    [CustomPropertyDrawer(typeof(ButtonCache))]
    public class ButtonCacheInspactor : UICachePropertyDrawer
    {
        protected override void ReadProperties(SerializedProperty source, List<SerializedProperty> properties)
        {
            base.ReadProperties(source, properties);
            properties.Add(source.FindPropertyRelative("button"));
            properties.Add(source.FindPropertyRelative("label"));
        }

        protected override void ResetProperties(UICache source_, List<SerializedProperty> properties, ref int index)
        {
            base.ResetProperties(source_, properties, ref index);

            var source = (ButtonCache)source_;
            properties[index++].objectReferenceValue = source.Button;
            var propLabel = properties[index++];
            var propLabelElems = propLabel.FindPropertyRelative("elems");
            var countLabelElems = propLabelElems.arraySize = source.Label.GetCount();
            for (int n = 0; n < countLabelElems; ++n)
            {
                var propLabelElem = propLabel.GetArrayElementAtIndex(n);
                propLabelElem.objectReferenceValue = source.Label.Get(n);
            }
            var propLabelAutoVisible = propLabel.FindPropertyRelative("autoVisibleWithValid");
            propLabelAutoVisible.boolValue = source.Label.IsAutoVisibleWithValid();
        }

        protected override UICache CreateCache(GameObject go)
        {
            return new ButtonCache(go);
        }

        protected override float GetTotalHeight(SerializedProperty property, float defaultHeight)
        {
            var propLabels = null != property ? property.FindPropertyRelative("label") : null;
            var propLabelElems = null != propLabels ? propLabels.FindPropertyRelative("elems") : null;
            var countLabelElems = null != propLabelElems ? propLabelElems.arraySize : 0;
            var unitHeight = defaultHeight + 2;
            return base.GetTotalHeight(property, defaultHeight) + (unitHeight * 3) + unitHeight * (countLabelElems + 1);
        }

        protected override Rect OnDrawProperty(SerializedProperty property, Rect contentPosition, float defaultHeight, Color defaultColor)
        {
            if ("label" == property.name)
            {
                contentPosition.y += defaultHeight + 2;
                var propLabelElems = property.FindPropertyRelative("elems");
                var countLabelElems = propLabelElems.arraySize;
                var available = 0 < countLabelElems;
                GUI.color = available ? Color.green : defaultColor;
                EditorGUI.LabelField(contentPosition, string.Format("{0}: {1}", property.name, countLabelElems));

                for (int n = 0; n < countLabelElems; ++n)
                {
                    contentPosition.y += defaultHeight + 2;
                    var countLabelElem = propLabelElems.GetArrayElementAtIndex(n);
                    var objLabelElem = countLabelElem.objectReferenceValue;
                    GUI.color = objLabelElem ? Color.cyan : defaultColor;
                    EditorGUI.LabelField(contentPosition, string.Format(" - [{0}]: {1}", n, objLabelElem ? objLabelElem.name : ""));
                }

                contentPosition.y += defaultHeight + 2;
                var propLabelAutoVisible = property.FindPropertyRelative("autoVisibleWithValid");
                propLabelAutoVisible.boolValue = EditorGUI.Toggle(contentPosition, string.Format(" - autoVisibleWithValid:"), propLabelAutoVisible.boolValue);

                return contentPosition;
            }
            else
                return base.OnDrawProperty(property, contentPosition, defaultHeight, defaultColor);
        }
    }
}