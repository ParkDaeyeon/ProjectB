using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Ext.Unity3D;
using Ext.Unity3D.UI;
namespace Ext.UnityEdit.UI
{
    [CustomPropertyDrawer(typeof(SpriteCache))]
    public class SpriteCachePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(pos, label, property);
            Rect contentPosition = EditorGUI.PrefixLabel(pos, label);
            float w = contentPosition.width;
            contentPosition.width *= 0.75f;

            float defaultHeight = base.GetPropertyHeight(property, label);
            contentPosition.height = defaultHeight;
            EditorGUI.indentLevel = 0;

            SerializedProperty propSprite = property.FindPropertyRelative("sprite");
            SerializedProperty propTexture = property.FindPropertyRelative("texture");
            SerializedProperty propUvRect = property.FindPropertyRelative("uvRect");

            UnityEngine.Object originalObject = propSprite.objectReferenceValue;
            Sprite originalSprite = originalObject != null ? (Sprite)originalObject : null;
            Sprite newSprite = (Sprite)EditorGUI.ObjectField(contentPosition, originalSprite, typeof(Sprite), false);

            if (originalSprite != newSprite)
            {
                Debug.Log("SPRITECACHE_RESET:" + property.displayName);
                if (null != propSprite)
                    propSprite.objectReferenceValue = newSprite;
                if (null != propTexture)
                    propTexture.objectReferenceValue = newSprite ? newSprite.texture : null;
                if (null != propUvRect)
                    propUvRect.rectValue = newSprite.GetUVRect();
            }

            EditorGUI.EndProperty();
        }
    }
}