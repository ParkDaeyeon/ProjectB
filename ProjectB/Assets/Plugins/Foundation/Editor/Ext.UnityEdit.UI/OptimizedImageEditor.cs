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
    [CustomEditor(typeof(OptimizedImage), true), CanEditMultipleObjects, InitializeOnLoad]
    public class OptimizedImageEditor : ImageEditor
    {
        SerializedObject sobj;
        SerializedProperty resizeAutomatically;
        SerializedProperty overrideRendering;
        SerializedProperty caches;

        protected override void OnEnable()
        {
            base.OnEnable();

            this.sobj = new SerializedObject(this.targets);
            this.resizeAutomatically = this.sobj.FindProperty("resizeAutomatically");
            this.overrideRendering = this.sobj.FindProperty("overrideRendering");
            this.caches = this.sobj.FindProperty("caches");
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Revert To UnityEngine.UI.Image"))
            {
                foreach (var target in this.targets)
                    OptimizedImageConverter.Convert<Image>(target as OptimizedImage);
                return;
            }

            base.OnInspectorGUI();

            EditorGUI.showMixedValue = this.resizeAutomatically.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            var resizeAutomatically = EditorGUILayout.Toggle("Resize Automatically", this.resizeAutomatically.boolValue);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                this.resizeAutomatically.boolValue = resizeAutomatically;
                this.sobj.ApplyModifiedProperties();
                this.sobj.Update();
            }

            EditorGUI.showMixedValue = this.overrideRendering.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            var overrideRendering = EditorGUILayout.Toggle("Override Rendering", this.overrideRendering.boolValue);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                this.overrideRendering.boolValue = overrideRendering;
                this.sobj.ApplyModifiedProperties();
                this.sobj.Update();
            }

            if (!this.overrideRendering.boolValue)
                return;

            this.OnInspectorOptimizedImage();
        }

        enum CachableState
        {
            None,
            All,
            Mixed,
        }

        public virtual void OnInspectorOptimizedImage()
        {
            var targets = this.sobj.targetObjects;
            var cashableCount = 0;
            var allCount = targets.Length;

            foreach (var target in targets)
            {
                var image = target as OptimizedImage;
                if (!image)
                    continue;

                image.EditorCheckCachable();
                if (image.Cacheable)
                    ++cashableCount;

                var status = string.Format("{0} : {1}",
                                           image.name,
                                           image.Cacheable ? (image.EditorCheckCached() ? "CACHED" : "NON-CACHED") : "CUSTOM");
                EditorGUILayout.HelpBox(status, MessageType.Info);
            }
            this.sobj.Update();

            var cachableState = 0 == cashableCount ? CachableState.None :
                                allCount == cashableCount ? CachableState.All :
                                CachableState.Mixed;

            switch (cachableState)
            {
            case CachableState.All:
                EditorGUI.showMixedValue = this.caches.hasMultipleDifferentValues;
                EditorGUI.BeginChangeCheck();
                var sprites = (SpritesComponent)EditorGUILayout.ObjectField("Caches", this.caches.objectReferenceValue, typeof(SpritesComponent), true);
                EditorGUI.showMixedValue = false;
                if (EditorGUI.EndChangeCheck())
                {
                    this.caches.objectReferenceValue = sprites;
                    this.sobj.ApplyModifiedProperties();
                    foreach (var target in targets)
                    {
                        var image = target as OptimizedImage;
                        if (!image)
                            continue;

                        image.ResetSprite();
                    }
                    this.sobj.Update();
                }
                break;
            }
        }
    }
}
