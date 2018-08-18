using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using Ext.Unity3D;

namespace Ext.UnityEdit
{
    [CustomPropertyDrawer(typeof(SerializedDictionary<,>))]
    public class SerializedDictionaryDrawer : PropertyDrawer
    {
        bool foldout = true;
        List<float> heights = new List<float>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            label.text = label.text + " Size";
            
            var contentPosition = EditorGUI.PrefixLabel(position, label);
            float w = contentPosition.width;
            contentPosition.width *= 0.75f;
            contentPosition.height = 18;

            var propBuiltKeys = property.FindPropertyRelative("builtKeys");
            var propBuiltValues = property.FindPropertyRelative("builtValues");
            
            propBuiltKeys.arraySize = 
            propBuiltValues.arraySize = Mathf.Max(0, EditorGUI.IntField(contentPosition, propBuiltKeys.arraySize));

            var foldPos = contentPosition;
            foldPos.x += w * 0.8f;
            foldPos.width = w * 0.2f;
            this.foldout = EditorGUI.Foldout(foldPos, this.foldout, "info", true);
            if (this.foldout)
            {
                ++EditorGUI.indentLevel;
                w = position.width;

                var indexRect = contentPosition;
                var keysRect = contentPosition;
                var valuesRect = contentPosition;

                indexRect.y = keysRect.y = valuesRect.y = contentPosition.y + 18;
                indexRect.x = 0;
                indexRect.width = w * 0.1f;
                keysRect.x = w * 0.15f;
                keysRect.width = w * 0.4f;
                valuesRect.x = w * 0.55f;
                valuesRect.width = w * 0.4f;

                EditorGUI.LabelField(keysRect, "Key");
                EditorGUI.LabelField(valuesRect, "Value");

                this.heights.Clear();
                this.heights.Add(20);

                var deletedIndex = -1;
                for (int n = 0, cnt = propBuiltKeys.arraySize; n < cnt; ++n)
                {
                    keysRect.y += this.heights[n];
                    valuesRect.y += this.heights[n];
                    indexRect.y += this.heights[n];
                    EditorGUI.LabelField(indexRect, n.ToString());
                    Rect buttonRect = new Rect(valuesRect.xMax + 10, valuesRect.y, 18, valuesRect.height);

                    var propKey = propBuiltKeys.GetArrayElementAtIndex(n);
                    var propValue = propBuiltValues.GetArrayElementAtIndex(n);
                    EditorGUI.PropertyField(keysRect, propKey, new GUIContent(), true);
                    EditorGUI.PropertyField(valuesRect, propValue, new GUIContent(), true);
                    if (GUI.Button(buttonRect, "X"))
                    {
                        deletedIndex = n;
                        break;
                    }
                    this.heights.Add(Mathf.Max(EditorGUI.GetPropertyHeight(propKey, new GUIContent(), true),
                                               EditorGUI.GetPropertyHeight(propValue, new GUIContent(), true)));
                }

                --EditorGUI.indentLevel;

                EditorGUI.EndProperty();

                if (-1 != deletedIndex)
                {
                    // NOTE: crashing on Unity5
                    //propBuiltKeys.DeleteArrayElementAtIndex(deletedIndex);
                    //propBuiltValues.DeleteArrayElementAtIndex(deletedIndex);
                    this.RemoveAt(propBuiltKeys, propBuiltValues, deletedIndex);
                }
            }
        }

        void RemoveAt(SerializedProperty propBuiltKeys, SerializedProperty propBuiltValues, int index)
        {
            for (int n = index + 1, cnt = propBuiltKeys.arraySize; n < cnt; ++n)
            {
                propBuiltKeys.MoveArrayElement(n, n - 1);
                propBuiltValues.MoveArrayElement(n, n - 1);
            }

            --propBuiltKeys.arraySize;
            --propBuiltValues.arraySize;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float extraHeight = 0;

            foreach (float f in this.heights)
                extraHeight += f;

            return base.GetPropertyHeight(property, label) + 4 + (foldout ? extraHeight : 0);
        }
    }
}
