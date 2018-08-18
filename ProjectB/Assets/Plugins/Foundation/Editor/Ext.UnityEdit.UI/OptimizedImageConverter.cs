using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Ext.Unity3D.UI;
using Ext.Unity3D;

namespace Ext.UnityEdit.UI
{
    public class OptimizedImageConverter : EditorWindow
    {
        enum ConvertTo
        {
            OptimizedImage,
            OptimizedFilledImage,
            OptimizedQuadPolyImage,
            OptimizedRectImage,
            Gauge,
            OriginalImage,
        }

        class Element
        {
            internal Image image;
            internal ConvertTo convertTo;
        }


        [MenuItem("Window/Optimized Image Converter")]
        static void ShowWindow()
        {
            EditorWindow.GetWindow<OptimizedImageConverter>("Converter");
        }

        List<Element> elements = new List<Element>();

        CachedBoolean hierarchy = new CachedBoolean("OptimizedImageConverter.hierarchy", true);
        CachedBoolean includeInactive = new CachedBoolean("OptimizedImageConverter.includeInactive", true);
        CachedBoolean imageOnly = new CachedBoolean("OptimizedImageConverter.imageOnly", true);

        public static string GetGameObjectPath(Transform trans)
        {
            if (!trans)
                return "";

            var list = new LinkedList<Transform>();
            list.AddLast(trans);
            while (trans.parent)
                list.AddFirst(trans = trans.parent);

            var sb = new StringBuilder();
            var node = list.First;
            while (null != node)
            {
                sb.Append('/').Append(node.Value.name);
                node = node.Next;
            }

            return sb.ToString();
        }

        Vector2 scroll;
        void OnGUI()
        {
            if (GUILayout.Button("Clear"))
                this.Clear();

            if (GUILayout.Button("Find All"))
                this.FindAll();

            EditorGUILayout.BeginHorizontal();
            this.hierarchy.Value = EditorGUILayout.Toggle("Hierarchy", this.hierarchy.Value);
            this.includeInactive.Value = EditorGUILayout.Toggle("Include Inactive", this.includeInactive.Value);
            this.imageOnly.Value = EditorGUILayout.Toggle("Image Only", this.imageOnly.Value);
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Append Selected"))
                this.AppendSelected(this.hierarchy.Value, this.includeInactive.Value);

            this.scroll = EditorGUILayout.BeginScrollView(this.scroll);

            foreach (var elem in this.elements)
            {
                EditorGUILayout.BeginHorizontal();

                var image = elem.image;
                if (image)
                {
                    var path = OptimizedImageConverter.GetGameObjectPath(image.transform);
                    EditorGUILayout.LabelField(path);
                    EditorGUILayout.PrefixLabel(image.GetType().Name);
                    elem.convertTo = (ConvertTo)EditorGUILayout.EnumPopup(elem.convertTo);
                }
                else
                    EditorGUILayout.HelpBox("Missing", MessageType.Warning);

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
            
            if (GUILayout.Button("Convert"))
                this.Convert();
        }

        void Clear()
        {
            this.elements.Clear();
        }

        void FindAll()
        {
            this.Clear();

            var roots = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var root in roots)
                this.AppendInHierarchy(root, true);
        }

        void AppendSelected(bool hierarchy,
                            bool includeInactive)
        {
            if (hierarchy)
            {
                var selections = Selection.GetFiltered(typeof(GameObject), SelectionMode.TopLevel);
                foreach (var selection in selections)
                    this.AppendInHierarchy(selection as GameObject, includeInactive);
            }
            else
            {
                var selections = Selection.GetFiltered(typeof(Image), SelectionMode.TopLevel);
                foreach (var selection in selections)
                    this.Append(selection as Image);
            }
        }

        void AppendInHierarchy(GameObject target, bool includeInactive)
        {
            if (!target)
                return;

            var images = target.GetComponentsInChildren<Image>(includeInactive);
            foreach (var image in images)
                this.Append(image);
        }

        void Append(Image image, ConvertTo convertTo = ConvertTo.OptimizedImage)
        {
            if (!image)
                return;

            if (null != this.elements.Find((elem) => elem.image == image))
                return;

            if (this.imageOnly.Value)
            {
                if (image.GetType() != typeof(Image))
                    return;
            }

            this.elements.Add(new Element { image = image, convertTo = convertTo, });
        }
        void Convert()
        {
            foreach (var elem in this.elements)
            {
                switch (elem.convertTo)
                {
                case ConvertTo.OptimizedImage:
                    elem.image = OptimizedImageConverter.Convert<OptimizedImage>(elem.image);
                    break;

                case ConvertTo.OptimizedFilledImage:
                    elem.image = OptimizedImageConverter.Convert<OptimizedFilledImage>(elem.image);
                    break;

                case ConvertTo.OptimizedQuadPolyImage:
                    elem.image = OptimizedImageConverter.Convert<OptimizedQuadPolyImage>(elem.image);
                    break;


                case ConvertTo.OptimizedRectImage:
                    elem.image = OptimizedImageConverter.Convert<OptimizedRectImage>(elem.image);
                    break;

                case ConvertTo.Gauge:
                    elem.image = OptimizedImageConverter.Convert<Gauge>(elem.image);
                    break;

                case ConvertTo.OriginalImage:
                    elem.image = OptimizedImageConverter.Convert<Image>(elem.image);
                    break;
                }
            }
        }
        
        public static To Convert<To>(Image from)
            where To : Image
        {
            if (!from)
                return null;

            if (from.GetType() == typeof(To))
                return from as To;

            // -------------------------------------------------
            // NOTE: From Graphic
            // -------------------------------------------------
#if UNITY_5_6_OR_NEWER
            var color = from.color;
            var raycastTarget = from.raycastTarget;
#endif// UNITY_5_6_OR_NEWER
            // -------------------------------------------------


            // -------------------------------------------------
            // NOTE: From MaskableGrapic
            // -------------------------------------------------
#if UNITY_5_6_OR_NEWER
            var maskable = from.maskable;
            var onCullStateChanged = from.onCullStateChanged;
#endif// UNITY_5_6_OR_NEWER
            // -------------------------------------------------


            // -------------------------------------------------
            // NOTE: From Image
            // -------------------------------------------------
#if UNITY_5_6_OR_NEWER
            var alphaHitTestMinimumThreshold = from.alphaHitTestMinimumThreshold;
            var fillAmount = from.fillAmount;
            var fillCenter = from.fillCenter;
            var fillClockwise = from.fillClockwise;
            var fillMethod = from.fillMethod;
            var fillOrigin = from.fillOrigin;
            var material = from.material;
            var overrideSprite = from.overrideSprite;
            var preserveAspect = from.preserveAspect;
            var sprite = from.sprite;
            var type = from.type;
#endif// UNITY_5_6_OR_NEWER
            // -------------------------------------------------

            var go = from.gameObject;

            var components = go.GetComponents<Component>();
            int moveUp = components.Length;
            foreach (var component in components)
            {
                if (component == from)
                    break;

                --moveUp;
            }

            OptimizedImage.DestroyImmediate(from);
            moveUp -= 1;

            var to = go.AddComponent<To>();
                
            // -------------------------------------------------
            // NOTE: To Graphic
            // -------------------------------------------------
#if UNITY_5_6_OR_NEWER
            to.color = color;
            to.raycastTarget = raycastTarget;
#endif// UNITY_5_6_OR_NEWER
            // -------------------------------------------------


            // -------------------------------------------------
            // NOTE: To MaskableGrapic
            // -------------------------------------------------
#if UNITY_5_6_OR_NEWER
            to.maskable = maskable;
            to.onCullStateChanged = onCullStateChanged;
#endif// UNITY_5_6_OR_NEWER
            // -------------------------------------------------


            // -------------------------------------------------
            // NOTE: To Image
            // -------------------------------------------------
#if UNITY_5_6_OR_NEWER
            to.alphaHitTestMinimumThreshold = alphaHitTestMinimumThreshold;
            to.fillAmount = fillAmount;
            to.fillCenter = fillCenter;
            to.fillClockwise = fillClockwise;
            to.fillMethod = fillMethod;
            to.fillOrigin = fillOrigin;
            to.material = material;
            to.overrideSprite = overrideSprite;
            to.preserveAspect = preserveAspect;
            to.sprite = sprite;
            to.type = type;
#endif// UNITY_5_6_OR_NEWER
            // -------------------------------------------------

            for (int n = 0; n < moveUp; ++n)
                UnityEditorInternal.ComponentUtility.MoveComponentUp(to);

            return to;
        }
    }
}
