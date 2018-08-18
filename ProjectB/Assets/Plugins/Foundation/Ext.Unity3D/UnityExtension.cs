using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security;
using System.Runtime.InteropServices;
#if UNITY_EDITOR
using System.Reflection;
#endif// UNITY_EDITOR
using Ext.String;

namespace Ext.Unity3D
{
    public static class UnityExtension
    {
        public static bool CheckValidAssetName(this string name,
                                               bool folder = false)
        {
            // ------------------------------------------
            // NOTE:
            // ------------------------------------------
            // Hidden Assets
            // During the import process, Unity completely ignores the following files and folders in the Assets folder(or a sub - folder within it):
            // 1. Hidden folders.
            // 2. Files and folders which start with ‘.’.
            // 3. Files and folders which end with ‘~’.
            // 4. Files and folders named cvs.
            // 5. Files with the extension .tmp.
            // ------------------------------------------
            // 1. Hidden folders.
            // pass

            if (0 == name.Length)
                return false;

            // 2. Files and folders which start with ‘.’.
            if ('.' == name[0])
                return false;

            // 3. Files and folders which end with ‘~’.
            if ('~' == name[name.Length - 1])
                return false;

            // 4. Files and folders named cvs.
            if ("cvs" == name)
                return false;

            // 5. Files with the extension .tmp.
            if (!folder)
            {
                var tmpIdx = name.LastIndexOf(".tmp");
                if (-1 != tmpIdx && (name.Length - ".tmp".Length) == tmpIdx)
                    return false;
            }

            return true;
        }

        public static bool CheckValidAssetPath(this string path,
                                               bool folder = false,
                                               bool recursive = false)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            if (-1 != path.IndexOf('\\'))
                path = path.Replace('\\', '/');

            var tailSlash = '/' == path[path.Length - 1];

            var nameLast = path.Length - 1;
            var prevSlashIndex = tailSlash ? path.LastIndexOf('/', nameLast - 1) :
                                             path.LastIndexOf('/');
            var nameStart = prevSlashIndex + 1;
            var name = tailSlash ? path.Substring(nameStart, nameLast - nameStart) :
                                   path.Substring(nameStart);

            if ("Assets" != name)
                return true;

            if (!UnityExtension.CheckValidAssetName(name, folder))
                return false;

            if (recursive)
            {
                for (; 0 < prevSlashIndex; --prevSlashIndex)
                {
                    nameLast = prevSlashIndex - 1;
                    prevSlashIndex = path.LastIndexOf('/', nameLast);
                    nameStart = prevSlashIndex + 1;
                    name = path.Substring(nameStart, (nameLast + 1) - nameStart);
                    if ("Assets" == name)
                        break;

                    if (!UnityExtension.CheckValidAssetName(name, true))
                        return false;
                }
            }

            return true;
        }

        public static string ToAssetPath(this string path, bool folder = false)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            if (-1 != path.IndexOf('\\'))
                path = path.Replace('\\', '/');

            return path.Remove(0, Application.dataPath.Length - "Assets".Length);
        }

        public static string ToIoPath(this string assetPath, bool folder = false)
        {
            if (string.IsNullOrEmpty(assetPath))
                return null;
            
            var path = Application.dataPath + assetPath.Remove(0, "Assets".Length);
            if (-1 != path.IndexOf('\\'))
                path = path.Replace('\\', '/');

            return path;
        }

        public static T CreateObject<T>(string name) where T : Component
        {
            return UnityExtension.CreateObject<T>(name, null, Vector3.zero, Vector3.one, Quaternion.identity);
        }
        public static T CreateObject<T>(string name, Transform parent) where T : Component
        {
            return UnityExtension.CreateObject<T>(name, parent, Vector3.zero, Vector3.one, Quaternion.identity);
        }
        public static T CreateObject<T>(string name, Vector3 localPos) where T : Component
        {
            return UnityExtension.CreateObject<T>(name, null, localPos, Vector3.one, Quaternion.identity);
        }
        public static T CreateObject<T>(string name, Vector3 localPos, Vector3 localScale) where T : Component
        {
            return UnityExtension.CreateObject<T>(name, null, localPos, localScale, Quaternion.identity);
        }
        public static T CreateObject<T>(string name, Quaternion localRot) where T : Component
        {
            return UnityExtension.CreateObject<T>(name, null, Vector3.zero, Vector3.one, localRot);
        }
        public static T CreateObject<T>(string name, Transform parent, Vector3 localPos) where T : Component
        {
            return UnityExtension.CreateObject<T>(name, parent, localPos, Vector3.one, Quaternion.identity);
        }
        public static T CreateObject<T>(string name, Transform parent, Vector3 localPos, Vector3 localScale) where T : Component
        {
            return UnityExtension.CreateObject<T>(name, parent, localPos, localScale, Quaternion.identity);
        }
        public static T CreateObject<T>(string name, Transform parent, Quaternion localRot) where T : Component
        {
            return UnityExtension.CreateObject<T>(name, parent, Vector3.zero, Vector3.one, localRot);
        }
        public static T CreateObject<T>(string name, Transform parent, Vector3 localPos, Vector3 localScale, Quaternion localRot) where T : Component
        {
            GameObject go = new GameObject(name);
            Transform t = go.transform;
            t.SetParent(parent);
            t.localPosition = localPos;
            t.localScale = localScale;
            t.localRotation = localRot;
            T component = go.AddComponent<T>();
            return component;
        }

        public static string GetHirarchyPath(this GameObject thiz, int skipParent = 0)
        {
            return thiz.transform.GetHirarchyPath();
        }
        public static string GetHirarchyPath(this Transform thiz, int skipParent = 0)
        {
            LinkedList<Transform> list = null;
            Transform parent = thiz.parent;
            while (parent)
            {
                if (null == list)
                    list = new LinkedList<Transform>();

                list.AddFirst(parent);
                parent = parent.parent;
            }

            StringBuilder sb = new StringBuilder(256);
            if (null != list)
            {
                int count = 0;
                LinkedListNode<Transform> node = list.First;
                while (null != node)
                {
                    if (count >= skipParent)
                        sb.Append(node.Value.name).Append('/');

                    ++count;
                    node = node.Next;
                }
            }
            return sb.Append(thiz.name).ToString();
        }

        public static void Quit()
        {
            Debug.Log("UNITY_EXTENSION_QUIT");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else// UNITY_EDITOR
            Application.Quit();
#endif// UNITY_EDITOR
        }



        public static void HideTransform(this Transform trans)
        {
            if (!trans)
                return;

            Vector3 pos = trans.localPosition;
            if (pos.y < 1000000)
            {
                pos.y += 1100000;
                trans.localPosition = pos;
            }
        }
        public static void ShowTransform(this Transform trans)
        {
            if (!trans)
                return;

            Vector3 pos = trans.localPosition;
            if (pos.y >= 1000000)
            {
                pos.y -= 1100000;
                trans.localPosition = pos;
            }
        }
        public static bool IsHideTransform(this Transform trans)
        {
            if (!trans)
                return true;

            Vector3 pos = trans.localPosition;
            return pos.y >= 1000000;
        }



        public static void HideTransformVer2(this Transform trans)
        {
            if (!trans)
                return;

            Vector3 pos = trans.localPosition;
            if (pos.x < 1000000)
            {
                pos.x += 1100000;
                trans.localPosition = pos;
            }
        }
        public static void ShowTransformVer2(this Transform trans)
        {
            if (!trans)
                return;

            Vector3 pos = trans.localPosition;
            if (pos.x >= 1000000)
            {
                pos.x -= 1100000;
                trans.localPosition = pos;
            }
        }
        public static bool IsHideTransformVer2(this Transform trans)
        {
            if (!trans)
                return true;

            Vector3 pos = trans.localPosition;
            return pos.x >= 1000000;
        }




        /// <summary>
        /// === Works only for textures previously loaded from Resources (see Resources.Load()) ===
        /// Finds all scene's Materials, looking for textures with names from 'texturesNamesToUnloading_', removes founded textures from materials
        /// and try to unload them. After this it finds all scene's Texture2D objects and also try to unload them
        /// </summary>
        public static void TexturesForcedUnloading(List<string> texturesNamesToUnloading_)
        {
            Material[] allResMats = Resources.FindObjectsOfTypeAll(typeof(Material)) as Material[];
            if (allResMats != null)
            {
                List<Material> matsToTextureDelete = new List<Material>();

                foreach (Material m in allResMats)
                    if (m != null && m.mainTexture != null && texturesNamesToUnloading_.Contains(m.mainTexture.name))
                        matsToTextureDelete.Add(m);

                if (matsToTextureDelete.Count > 0)
                {
                    foreach (Material m in matsToTextureDelete)
                    {
                        if (m.mainTexture == null)
                            continue;

#if LOG_DEBUG
                        Debug.Log(string.Format("Remove texture '{0}' from material '{1}'", m.mainTexture.name, m.name));
#endif// LOG_DEBUG

                        Texture textureToUnload = m.mainTexture;
                        m.mainTexture = null;

#if LOG_DEBUG
                        Debug.Log(string.Format("Try to unload <Texture> '{0}' (after remove from material)", textureToUnload.name));
#endif// LOG_DEBUG

                        textureToUnload.Unload();
                        textureToUnload = null;
                    }
                }
            }

            Texture2D[] allResTextures = Resources.FindObjectsOfTypeAll(typeof(Texture2D)) as Texture2D[];
            if (allResTextures != null)
            {
                List<Texture2D> texturesToUnload = new List<Texture2D>();
                foreach (Texture2D t in allResTextures)
                    if (t != null && texturesNamesToUnloading_.Contains(t.name))
                        texturesToUnload.Add(t);

                if (texturesToUnload.Count > 0)
                {
                    foreach (Texture2D t in texturesToUnload)
                    {
                        if (t == null)
                            continue;

#if LOG_DEBUG
                        Debug.Log(string.Format("Try to unload <Texture2D> '{0}' (as is)", t.name));
#endif// LOG_DEBUG

                        t.Unload();
                    }
                }
            }

            Resources.UnloadUnusedAssets();
        }

        public static void Unload<T>(this T asset) where T : UnityEngine.Object
        {
            if (asset)
                Resources.UnloadAsset(asset);
        }

        public static void Destroy(this UnityEngine.Object obj)
        {
            if (obj)
                UnityEngine.Object.Destroy(obj);
        }

        public static void Destroy(this GameObject go)
        {
            if (go)
                GameObject.Destroy(go);
        }




        public static void ForceAwake(this GameObject thiz)
        {
            if (!thiz.activeSelf)
            {
                thiz.SetActive(true);
                thiz.SetActive(false);
            }
        }




        public static Transform Search(this Transform target, string name)
        {
            return UnityExtension.Search(target, name, false);
        }

        public static Transform SearchPartial(this Transform target, string name)
        {
            return UnityExtension.Search(target, name, true);
        }

        private static Transform Search(Transform target, string name, bool isPartial)
        {
            Transform c = null;

            if (isPartial)
            {
                for (int n = 0, nMax = target.childCount; n < nMax; ++n)
                {
                    c = target.GetChild(n);
                    if (c.name.Contains(name))
                        return c;
                }
            }
            else
            {
                for (int n = 0, nMax = target.childCount; n < nMax; ++n)
                {
                    c = target.GetChild(n);
                    if (c.name.Equals(name))
                        return c;
                }
            }

            Transform ret = null;
            for (int n = 0, nMax = target.childCount; n < nMax; ++n)
                if (null != (ret = UnityExtension.Search(target.GetChild(n), name, isPartial)))
                    return ret;

            return null;
        }




        public static Transform[] FindAll(this Transform target, string name)
        {
            List<Transform> buffer = new List<Transform>(32);

            UnityExtension.Collect(target, name, buffer, false);

            return buffer.ToArray();
        }

        public static Transform[] FindPartialAll(this Transform target, string name)
        {
            List<Transform> buffer = new List<Transform>(32);

            UnityExtension.Collect(target, name, buffer, true);

            return buffer.ToArray();
        }

        public static Transform[] SearchAll(this Transform target, string name)
        {
            List<Transform> buffer = new List<Transform>(32);

            UnityExtension.CollectAll(target, name, buffer, false);

            return buffer.ToArray();
        }

        public static Transform[] SearchPartialAll(this Transform target, string name)
        {
            List<Transform> buffer = new List<Transform>(32);

            UnityExtension.CollectAll(target, name, buffer, true);

            return buffer.ToArray();
        }


        private static void Collect(Transform target, string name, List<Transform> buffer, bool isPatial)
        {
            Transform c = null;

            if (isPatial)
            {
                for (int n = 0, nMax = target.childCount; n < nMax; ++n)
                {
                    c = target.GetChild(n);
                    if (c.name.Contains(name))
                        buffer.Add(c);
                }
            }
            else
            {
                for (int n = 0, nMax = target.childCount; n < nMax; ++n)
                {
                    c = target.GetChild(n);
                    if (c.name.Equals(name))
                        buffer.Add(c);
                }
            }
        }

        private static void CollectAll(Transform target, string name, List<Transform> buffer, bool isPatial)
        {
            UnityExtension.Collect(target, name, buffer, isPatial);

            for (int n = 0, nMax = target.childCount; n < nMax; ++n)
                UnityExtension.CollectAll(target.GetChild(n), name, buffer, isPatial);
        }





        public static T GetComponentInParentRoot<T>(this Component target) where T : Component
        {
            if (!target)
                return null;

            return UnityExtension.GetComponentInParentRoot<T>(target.transform);
        }
        public static T GetComponentInParentRoot<T>(this GameObject target) where T : Component
        {
            if (!target)
                return null;

            return UnityExtension.GetComponentInParentRoot<T>(target.transform);
        }
        public static T GetComponentInParentRoot<T>(this Transform target) where T : Component
        {
            if (!target)
                return null;

            T compo = null, finded = null;
            do
            {
                if ((compo = target.GetComponent<T>()))
                    finded = compo;

                target = target.parent;
            }
            while (null != target);

            return finded;
        }

        //public static T GetComponentInParentNear<T>(this Transform target) where T : Component
        //{
        //    if (!target)
        //        return null;

        //    T compo = null;
        //    do
        //    {
        //        if ((compo = target.GetComponent<T>()))
        //            return compo;

        //        target = target.parent;
        //    }
        //    while (null != target);

        //    return null;
        //}



        public static T[] GetComponentsInFirstChildren<T>(this Component target, bool childrenOnly, bool includeInactive = false) where T : Component
        {
            if (!target)
                return new T[0];

            return UnityExtension.GetComponentsInFirstChildren<T>(target.transform, childrenOnly, includeInactive);
        }
        public static T[] GetComponentsInFirstChildren<T>(this GameObject target, bool childrenOnly, bool includeInactive = false) where T : Component
        {
            if (!target)
                return new T[0];

            return UnityExtension.GetComponentsInFirstChildren<T>(target.transform, childrenOnly, includeInactive);
        }
        public static T[] GetComponentsInFirstChildren<T>(this Transform target, bool childrenOnly, bool includeInactive = false) where T : Component
        {
            if (!target)
                return new T[0];

            List<T> list = new List<T>();

            if (childrenOnly)
            {
                foreach (Transform t in target)
                    UnityExtension.GetComponentsInFirstChildrenInternal<T>(t, includeInactive, list);
            }
            else
                UnityExtension.GetComponentsInFirstChildrenInternal<T>(target, includeInactive, list);

            return list.ToArray();
        }
        static void GetComponentsInFirstChildrenInternal<T>(Transform target, bool includeInactive, List<T> list) where T : Component
        {
            if (!target)
                return;

            if (!includeInactive && !target.gameObject.activeSelf)
                return;

            T com = target.GetComponent<T>();
            if (com)
            {
                list.Add(com);
                return;
            }

            foreach (Transform t in target)
                UnityExtension.GetComponentsInFirstChildrenInternal<T>(t, includeInactive, list);
        }







        public static void MultiplyLocalScale(this Transform t, float scalar)
        {
            Vector3 v = t.localScale;
            v *= scalar;
            t.localScale = v;
        }
        public static void MultiplyLocalScale(this Transform t, Vector3 v3d)
        {
            Vector3 v = t.localScale;
            v.Scale(v3d);
            t.localScale = v;
        }
        public static void MultiplyLocalScale(this Transform t, float x, float y, float z)
        {
            Vector3 v = t.localScale;
            v.x *= x;
            v.y *= y;
            v.z *= z;
            t.localScale = v;
        }


        public static void MultiplyLocalScaleX(this Transform t, float x)
        {
            Vector3 v = t.localScale;
            v.x *= x;
            t.localScale = v;
        }
        public static void MultiplyLocalScaleY(this Transform t, float y)
        {
            Vector3 v = t.localScale;
            v.y *= y;
            t.localScale = v;
        }
        public static void MultiplyLocalScaleZ(this Transform t, float z)
        {
            Vector3 v = t.localScale;
            v.z *= z;
            t.localScale = v;
        }
        public static void MultiplyLocalScaleXY(this Transform t, float x, float y)
        {
            Vector3 v = t.localScale;
            v.x *= x;
            v.y *= y;
            t.localScale = v;
        }
        public static void MultiplyLocalScaleYZ(this Transform t, float y, float z)
        {
            Vector3 v = t.localScale;
            v.y *= y;
            v.z *= z;
            t.localScale = v;
        }
        public static void MultiplyLocalScaleXZ(this Transform t, float x, float z)
        {
            Vector3 v = t.localScale;
            v.x *= x;
            v.z *= z;
            t.localScale = v;
        }
















        public static void MultiplyLocalPosition(this Transform t, float scalar)
        {
            Vector3 v = t.localPosition;
            v *= scalar;
            t.localPosition = v;
        }
        public static void MultiplyLocalPosition(this Transform t, Vector3 v3d)
        {
            Vector3 v = t.localPosition;
            v.Scale(v3d);
            t.localPosition = v;
        }
        public static void MultiplyLocalPosition(this Transform t, float x, float y, float z)
        {
            Vector3 v = t.localPosition;
            v.x *= x;
            v.y *= y;
            v.z *= z;
            t.localPosition = v;
        }


        public static void MultiplyLocalPositionX(this Transform t, float x)
        {
            Vector3 v = t.localPosition;
            v.x *= x;
            t.localPosition = v;
        }
        public static void MultiplyLocalPositionY(this Transform t, float y)
        {
            Vector3 v = t.localPosition;
            v.y *= y;
            t.localPosition = v;
        }
        public static void MultiplyLocalPositionZ(this Transform t, float z)
        {
            Vector3 v = t.localPosition;
            v.z *= z;
            t.localPosition = v;
        }
        public static void MultiplyLocalPositionXY(this Transform t, float x, float y)
        {
            Vector3 v = t.localPosition;
            v.x *= x;
            v.y *= y;
            t.localPosition = v;
        }
        public static void MultiplyLocalPositionYZ(this Transform t, float y, float z)
        {
            Vector3 v = t.localPosition;
            v.y *= y;
            v.z *= z;
            t.localPosition = v;
        }
        public static void MultiplyLocalPositionXZ(this Transform t, float x, float z)
        {
            Vector3 v = t.localPosition;
            v.x *= x;
            v.z *= z;
            t.localPosition = v;
        }







        public static void MultiplyWorldPosition(this Transform t, float scalar)
        {
            Vector3 v = t.position;
            v *= scalar;
            t.position = v;
        }
        public static void MultiplyWorldPosition(this Transform t, Vector3 v3d)
        {
            Vector3 v = t.position;
            v.Scale(v3d);
            t.position = v;
        }
        public static void MultiplyWorldPosition(this Transform t, float x, float y, float z)
        {
            Vector3 v = t.position;
            v.x *= x;
            v.y *= y;
            v.z *= z;
            t.position = v;
        }


        public static void MultiplyWorldPositionX(this Transform t, float x)
        {
            Vector3 v = t.position;
            v.x *= x;
            t.position = v;
        }
        public static void MultiplyWorldPositionY(this Transform t, float y)
        {
            Vector3 v = t.position;
            v.y *= y;
            t.position = v;
        }
        public static void MultiplyWorldPositionZ(this Transform t, float z)
        {
            Vector3 v = t.position;
            v.z *= z;
            t.position = v;
        }
        public static void MultiplyWorldPositionXY(this Transform t, float x, float y)
        {
            Vector3 v = t.position;
            v.x *= x;
            v.y *= y;
            t.position = v;
        }
        public static void MultiplyWorldPositionYZ(this Transform t, float y, float z)
        {
            Vector3 v = t.position;
            v.y *= y;
            v.z *= z;
            t.position = v;
        }
        public static void MultiplyWorldPositionXZ(this Transform t, float x, float z)
        {
            Vector3 v = t.position;
            v.x *= x;
            v.z *= z;
            t.position = v;
        }


        public static void SetWorldPositionX(this Transform t, float x)
        {
            Vector3 v = t.position;
            v.x = x;
            t.position = v;
        }
        public static void SetWorldPositionY(this Transform t, float y)
        {
            Vector3 v = t.position;
            v.y = y;
            t.position = v;
        }
        public static void SetWorldPositionZ(this Transform t, float z)
        {
            Vector3 v = t.position;
            v.z = z;
            t.position = v;
        }
        public static void SetWorldPositionXY(this Transform t, float x, float y)
        {
            Vector3 v = t.position;
            v.x = x;
            v.y = y;
            t.position = v;
        }
        public static void SetWorldPositionYZ(this Transform t, float y, float z)
        {
            Vector3 v = t.position;
            v.y = y;
            v.z = z;
            t.position = v;
        }
        public static void SetWorldPositionXZ(this Transform t, float x, float z)
        {
            Vector3 v = t.position;
            v.x = x;
            v.z = z;
            t.position = v;
        }


        public static void SetLocalPositionX(this Transform t, float x)
        {
            Vector3 v = t.localPosition;
            v.x = x;
            t.localPosition = v;
        }
        public static void SetLocalPositionY(this Transform t, float y)
        {
            Vector3 v = t.localPosition;
            v.y = y;
            t.localPosition = v;
        }
        public static void SetLocalPositionZ(this Transform t, float z)
        {
            Vector3 v = t.localPosition;
            v.z = z;
            t.localPosition = v;
        }
        public static void SetLocalPositionXY(this Transform t, float x, float y)
        {
            Vector3 v = t.localPosition;
            v.x = x;
            v.y = y;
            t.localPosition = v;
        }
        public static void SetLocalPositionYZ(this Transform t, float y, float z)
        {
            Vector3 v = t.localPosition;
            v.y = y;
            v.z = z;
            t.localPosition = v;
        }
        public static void SetLocalPositionXZ(this Transform t, float x, float z)
        {
            Vector3 v = t.localPosition;
            v.x = x;
            v.z = z;
            t.localPosition = v;
        }


        public static void SetAnchoredPositionX(this RectTransform t, float x)
        {
            Vector2 v = t.anchoredPosition;
            v.x = x;
            t.anchoredPosition = v;
        }

        public static void SetAnchoredPositionY(this RectTransform t, float y)
        {
            Vector2 v = t.anchoredPosition;
            v.y = y;
            t.anchoredPosition = v;
        }

        public static void SetAnchoredPositionXY(this RectTransform t, float x, float y)
        {
            Vector2 v = t.anchoredPosition;
            v.x = x;
            v.y = y;
            t.anchoredPosition = v;
        }

        public static void CopyRectTransform(this RectTransform t, RectTransform sample)
        {
            t.pivot = sample.pivot;
            t.anchorMin = sample.anchorMin;
            t.anchorMax = sample.anchorMax;
            t.anchoredPosition = sample.anchoredPosition;
            t.sizeDelta = sample.sizeDelta;
            t.localRotation = sample.localRotation;
            t.localScale = sample.localScale;
        }





        public static Vector2 ScaleX(this Vector2 v2d, float x)
        {
            v2d.x *= x;
            return v2d;
        }
        public static Vector2 ScaleY(this Vector2 v2d, float y)
        {
            v2d.y *= y;
            return v2d;
        }
        public static Vector2 ScaleXY(this Vector2 v2d, float x, float y)
        {
            v2d.x *= x;
            v2d.y *= y;
            return v2d;
        }


        public static Vector3 ScaleX(this Vector3 v3d, float x)
        {
            v3d.x *= x;
            return v3d;
        }
        public static Vector3 ScaleY(this Vector3 v3d, float y)
        {
            v3d.y *= y;
            return v3d;
        }
        public static Vector3 ScaleZ(this Vector3 v3d, float z)
        {
            v3d.z *= z;
            return v3d;
        }
        public static Vector3 ScaleXY(this Vector3 v3d, float x, float y)
        {
            v3d.x *= x;
            v3d.y *= y;
            return v3d;
        }
        public static Vector3 ScaleYZ(this Vector3 v3d, float y, float z)
        {
            v3d.y *= y;
            v3d.z *= z;
            return v3d;
        }
        public static Vector3 ScaleXZ(this Vector3 v3d, float x, float z)
        {
            v3d.x *= x;
            v3d.z *= z;
            return v3d;
        }
        public static Vector3 ScaleXYZ(this Vector3 v3d, float x, float y, float z)
        {
            v3d.x *= x;
            v3d.y *= y;
            v3d.z *= z;
            return v3d;
        }






        public static void SetSize(this Transform trans, Vector2 newSize)
        {
            if (!trans)
                return;

            Vector2 scale = trans.localScale;
            scale.x = newSize.x;
            scale.y = newSize.y;
            trans.localScale = scale;
        }
        public static void SetWidth(this Transform trans, float newSize)
        {
            if (!trans)
                return;

            UnityExtension.SetSize(trans, new Vector2(newSize, trans.localScale.y));
        }
        public static void SetHeight(this Transform trans, float newSize)
        {
            if (!trans)
                return;

            UnityExtension.SetSize(trans, new Vector2(trans.localScale.y, newSize));
        }



        public static void RewindAndSample(this Animation anim)
        {
            var playAutomaticallyOld = anim.playAutomatically;
            anim.playAutomatically = false;

            anim.Rewind();
            anim.Play();
            anim.Sample();
            anim.Stop();

            if (playAutomaticallyOld)
                anim.playAutomatically = true;
        }

        public static void ForceStopWithFF(this Animation anim, AnimationState state = null)
        {
            anim.playAutomatically = false;

            anim.Play();

            if (null == state)
                state = anim[anim.clip.name];
            if (null != state)
                state.normalizedTime = 1;

            anim.Sample();
            anim.Stop();

            anim.enabled = false;
        }

        public static void ForceStop(this Animation anim, bool rewind)
        {
            if (!anim)
                return;

            anim.playAutomatically = false;

            if (rewind)
            {
                anim.Rewind();
                anim.Play();
                anim.Sample();
                anim.Stop();
            }
            else
            {
                anim.Stop();
            }

            anim.enabled = false;
        }

        public static void ForcePlay(this Animation anim, bool rewind)
        {
            if (null == anim)
                return;

            anim.enabled = true;

            if (rewind)
            {
                anim.Rewind();
            }

            anim.Sample();
            anim.Play();
        }

        public static void ForcePlay(this Animation anim, float time)
        {
            if (null == anim)
                return;

            anim.enabled = true;

            anim[anim.clip.name].time = time;
            anim.Sample();
            anim.Play();
        }



        public static string GetHttpStatus(this WWW www)
        {
            string httpStatus;
            if (!www.responseHeaders.TryGetValue("Status", out httpStatus) &&
                !www.responseHeaders.TryGetValue("STATUS", out httpStatus) &&
                !www.responseHeaders.TryGetValue("status", out httpStatus))
            {
                httpStatus = "NOTFOUND";
#if LOG_NET
                Debug.LogWarning(string.Format("UNITY_EXT:WWW_HTTP_ERROR:STATUS_NOT_FOUND:{0}, TEXT:{1}", www.error, www.text));
#endif// LOG_NET
            }

            return httpStatus;
        }

        public static void DestroyChild(Transform parent, string name)
        {
            Transform child = parent.Find(name);

            if (child != null)
            {
                // NOTE: 해당 오브젝트를 바로 삭제
                GameObject.DestroyImmediate(child.gameObject);
            }
        }


        public static Color32 ToColor32Safe(this string hexColorCode)
        {
            if ('#' == hexColorCode[0])
                hexColorCode = hexColorCode.Remove(0, 1);

            return hexColorCode.ToColor32();
        }

        public static Color32 ToColor32(this string hexString)
        {
            uint colorCode = System.Convert.ToUInt32(hexString, 16);
            return colorCode.ToColor32();
        }

        public static Color32 ToColor32(this uint colorCode)
        {
            return new Color32((byte)(colorCode >> 24), (byte)(colorCode >> 16), (byte)(colorCode >> 8), (byte)colorCode);
        }


        public static Color32 ToColor24Safe(this string hexColorCode)
        {
            if ('#' == hexColorCode[0])
                hexColorCode = hexColorCode.Remove(0, 1);

            return hexColorCode.ToColor24();
        }

        public static Color32 ToColor24(this string hexString)
        {
            uint colorCode = System.Convert.ToUInt32(hexString, 16);
            return colorCode.ToColor24();
        }

        public static Color32 ToColor24(this uint colorCode)
        {
            return new Color32((byte)(colorCode >> 16), (byte)(colorCode >> 8), (byte)colorCode, 255);
        }

        public static string ToHexString(this Color color)
        {
            var factor = 255;
            return string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", (int)color.r * factor,
                                                             (int)color.g * factor,
                                                             (int)color.b * factor,
                                                             (int)color.a * factor);
        }


        public static string ToBuiltInPath(this string filename)
        {
            if (null == filename)
                return null;

            if (filename.Contains(Application.streamingAssetsPath))
                return filename;

            string extractedSubPath = null;
            int idx = 0;
            if (-1 != (idx = filename.IndexOf(Application.persistentDataPath)))
            {
                extractedSubPath = filename.Substring(idx + Application.persistentDataPath.Length);
            }
            else if (-1 != (idx = filename.IndexOf(Application.temporaryCachePath)))
            {
                extractedSubPath = filename.Substring(idx + Application.temporaryCachePath.Length);
            }

            if (null != extractedSubPath)
            {
                return Application.streamingAssetsPath + extractedSubPath;
            }

            return null;
        }


        public static string ToBuiltInUri(this string filename)
        {
            return MakeFileUri.Rebuild(filename.ToBuiltInPath());
        }

        public static string ToPlainString(this SecureString secureString)
        {
            if (null == secureString)
                throw new ArgumentNullException("SECURESTRING_TO_STRING");

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToBSTR(secureString);
                return Marshal.PtrToStringAuto(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeBSTR(unmanagedString);
            }
        }

        public static SecureString ToSecureString(this string str)
        {
            if (null == str)
                throw new ArgumentNullException("STRING_TO_SECURESTRING");

            unsafe
            {
                fixed (char* charPtr = str)
                {
                    var result = new SecureString(charPtr, str.Length);
                    result.MakeReadOnly();
                    return result;
                }
            }
        }

#if UNITY_EDITOR
        public static void SetDirtyAll(Transform t)
        {
            if (!t)
            {
                Debug.LogWarning(string.Format("SET_DIRTY_ALL_BAD_TRANSFORM"));
                return;
            }

            Component[] childrens = t.GetComponentsInChildren<Component>(true);
            for (int n = 0; n < childrens.Length; ++n)
            {
                Component c = childrens[n];
                if (c)
                    UnityEditor.EditorUtility.SetDirty(childrens[n]);
                else
                    Debug.LogWarning("MISSING COMPONENT AT:" + n);
            }

            UnityExtension.SetDirtyTransform(t);
        }

        static void SetDirtyTransform(Transform t)
        {
            if (t)
            {
                UnityEditor.EditorUtility.SetDirty(t);

                foreach (Transform tc in t)
                {
                    UnityExtension.SetDirtyTransform(tc);
                }
            }
            else
            {
                Debug.LogWarning("NULL TRANSFORM!!");
            }
        }




        public static T SaveAsset<T>(UnityEngine.Object directoryObj, string name, byte[] bytes, bool force) where T : UnityEngine.Object
        {
            if (null == directoryObj)
            {
                Debug.LogWarning("UNITY_EXTENSION_EDITWARN(SaveAsset):DIR_OBJ_IS_NULL");
                return null;
            }

            string assetFoldername = null;
            try
            {
                assetFoldername = UnityEditor.AssetDatabase.GetAssetPath(directoryObj);
            }
            catch (Exception e)
            {
                Debug.LogWarning("UNITY_EXTENSION_EDITWARN(SaveAsset):GET_ASSET_PATH_EXCEPT, EXCEPT:\n" + e);
                return null;
            }

            if (!string.IsNullOrEmpty(assetFoldername))
            {
                assetFoldername = assetFoldername.Replace('\\', '/');
                if ('/' != assetFoldername[0])
                    assetFoldername = '/' + assetFoldername;
                if ('/' != assetFoldername[assetFoldername.Length - 1])
                    assetFoldername += '/';
                if (0 == assetFoldername.IndexOf("/Assets/"))
                    assetFoldername = assetFoldername.Substring("/Assets/".Length - 1);

                string foldername = Application.dataPath.Replace('\\', '/') + assetFoldername;
                if (!Directory.Exists(foldername))
                {
                    Debug.LogWarning("UNITY_EXTENSION_EDITWARN(SaveAsset):DIR_NOT_FOUND:" + foldername);
                    return null;
                }


                string filename = string.Format("{0}{1}", foldername, name);
                try
                {
                    var fullLastDir = Path.GetDirectoryName(filename);
                    if (Directory.Exists(fullLastDir))
                        Directory.CreateDirectory(fullLastDir);

                    if (File.Exists(filename))
                    {
                        if (!force)
                            throw new Exception("DUPLICATED");

                        File.Delete(filename);
                    }

                    File.WriteAllBytes(filename, bytes);
                }
                catch (Exception e)
                {
                    Debug.LogWarning("UNITY_EXTENSION_EDITWARN(SaveAsset):WRITE_FAILED:" + filename + ", EXCEPT:\n" + e.ToString());
                }


                string assetname = string.Format("Assets{0}{1}", assetFoldername, name);
                UnityEditor.AssetDatabase.Refresh();
                UnityEditor.AssetDatabase.ImportAsset(assetname, UnityEditor.ImportAssetOptions.ForceUpdate);
                UnityEditor.AssetDatabase.Refresh();

                try
                {
                    var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetname);
                    if (asset)
                        return asset;
                    else
                        throw new FileNotFoundException(assetname);
                }
                catch (Exception e)
                {
                    Debug.LogWarning("UNITY_EXTENSION_EDITWARN(SaveAsset):ASSET_NOT_FOUND:" + assetname + ", EXCEPT:\n" + e.ToString());
                }
            }
            else
            {
                Debug.LogWarning("UNITY_EXTENSION_EDITWARN(SaveAsset):GET_ASSET_PATH_FAILED, OBJ:" + directoryObj);
            }

            return null;
        }


        static bool TranslateDict<K, FROM, TO>(SortedDictionary<K, FROM> from,
                                               SortedDictionary<K, TO> to)
            where FROM : class
            where TO : class
        {
            var succeed = false;
            foreach (var pair in from)
            {
                var key = pair.Key;
                if (to.ContainsKey(key))
                    continue;

                var value = pair.Value as TO;
                if (null == value)
                    continue;

                to.Add(key, value);
                succeed = true;
            }
            return succeed;
        }

        public struct LoadAssetFilter
        {
            public struct Iterator
            {
                public string prefix;
                public string ext;
                public int start;
                public int zeroFilled;
                public bool hex;
                public override string ToString()
                {
                    return JsonUtility.ToJson(this, false);
                }
            }
            public Iterator[] iterators;
            public string[] names;
            public bool bruteForce;
            public StringCond[] conds;
            public bool recursive;
            public string[] labels;
            public override string ToString()
            {
                return JsonUtility.ToJson(this, false);
            }
        }
        public static T[] LoadAssetAtPathsByFilter<T>(UnityEngine.Object dirObj,
                                                      LoadAssetFilter filter)
            where T : UnityEngine.Object
        {
            var dict = new SortedDictionary<string, T>(new StringComparer());
            if (!UnityExtension.LoadAssetAtPathsByFilter<T>(dirObj, filter, dict))
                return new T[0];

            return new List<T>(dict.Values).ToArray();
        }
        public static bool LoadAssetAtPathsByFilter<T>(UnityEngine.Object dirObj,
                                                       LoadAssetFilter filter,
                                                       SortedDictionary<string, T> dict)
            where T : UnityEngine.Object
        {
            var dictRaw = new SortedDictionary<string, UnityEngine.Object>(new StringComparer());
            if (!UnityExtension.LoadAssetAtPathsByFilter(typeof(T),
                                                         dirObj,
                                                         filter,
                                                         dictRaw))
                return false;

            if (!UnityExtension.TranslateDict(dictRaw, dict))
            {
                Debug.LogWarning(string.Format("LOAD_ASSET_BY_FILTER<{0}>:FILE_NOT_FOUND:DIROBJ:{1}, FILTER:{2}",
                                                typeof(T).Name,
                                                null != dirObj ? dirObj.ToString() : "null",
                                                filter));
                return false;
            }

            return true;
        }

        public static T[] LoadAssetAtPathsByFilter<T>(string dirname,
                                                      LoadAssetFilter filter)
            where T : UnityEngine.Object
        {
            var dict = new SortedDictionary<string, T>(new StringComparer());
            if (!UnityExtension.LoadAssetAtPathsByFilter<T>(dirname, filter, dict))
                return new T[0];

            return new List<T>(dict.Values).ToArray();
        }
        public static bool LoadAssetAtPathsByFilter<T>(string dirname, 
                                                       LoadAssetFilter filter,
                                                       SortedDictionary<string, T> dict)
            where T : UnityEngine.Object
        {
            var dictRaw = new SortedDictionary<string, UnityEngine.Object>(new StringComparer());
            if (!UnityExtension.OnLoadAssetAtPathsByFilter(typeof(T),
                                                           dirname,
                                                           filter,
                                                           dictRaw))
                return false;

            if (!UnityExtension.TranslateDict(dictRaw, dict))
            {
                Debug.LogWarning(string.Format("LOAD_ASSET_BY_FILTER<{0}>:FILE_NOT_FOUND:DIRNAME:{1}, FILTER:{2}",
                                                typeof(T).Name,
                                                null != dirname ? dirname : "null",
                                                filter));
                return false;
            }

            return true;
        }

        public static UnityEngine.Object[] LoadAssetAtPathsByFilter(Type type,
                                                                    UnityEngine.Object dirObj,
                                                                    LoadAssetFilter filter)
        {
            var dict = new SortedDictionary<string, UnityEngine.Object>(new StringComparer());
            if (!UnityExtension.LoadAssetAtPathsByFilter(type, dirObj, filter, dict))
                return new UnityEngine.Object[0];

            return new List<UnityEngine.Object>(dict.Values).ToArray();
        }
        public static bool LoadAssetAtPathsByFilter(Type type,
                                                    UnityEngine.Object dirObj,
                                                    LoadAssetFilter filter,
                                                    SortedDictionary<string, UnityEngine.Object> dict)
        {
            if (null == dirObj)
            {
                Debug.LogWarning(string.Format("LOAD_ASSET_BY_FILTER<{0}>:DIROBJ_IS_NULL:FILTER:{1}",
                                                null != type ? type.Name : "null",
                                                filter));
                return false;
            }

            var dirnameSub = default(string);
            try
            {
                dirnameSub = UnityEditor.AssetDatabase.GetAssetPath(dirObj);
            }
            catch (Exception e)
            {
                Debug.LogWarning(string.Format("LOAD_ASSET_BY_FILTER<{0}>:GET_ASSET_PATH_EXCEPT:OBJ:{1}, FILTER:{2}, EXCEPT:{3}",
                                                null != type ? type.Name : "null",
                                                null != dirObj ? dirObj.ToString() : "null",
                                                filter,
                                                e));
                return false;
            }

            if (!string.IsNullOrEmpty(dirnameSub))
            {
                var dirnameAssets = Application.dataPath.Replace('\\', '/');
                var dirnameProject = dirnameAssets.Remove(dirnameAssets.LastIndexOf("Assets"));
                var dirname = dirnameProject + dirnameSub;

                return UnityExtension.LoadAssetAtPathsByFilter(type,
                                                               dirname,
                                                               filter,
                                                               dict);
            }
            else
            {
                Debug.LogWarning(string.Format("LOAD_ASSET_BY_FILTER<{0}>:GET_ASSET_PATH_FAILED:OBJ:{1}, FILTER:{2}",
                                                null != type ? type.Name : "null",
                                                null != dirObj ? dirObj.ToString() : "null",
                                                filter));
                return false;
            }
        }

        public static UnityEngine.Object[] LoadAssetAtPathsByFilter(Type type,
                                                                    string dirname,
                                                                    LoadAssetFilter filter)
        {
            var dict = new SortedDictionary<string, UnityEngine.Object>(new StringComparer());
            if (!UnityExtension.LoadAssetAtPathsByFilter(type, dirname, filter, dict))
                return new UnityEngine.Object[0];

            return new List<UnityEngine.Object>(dict.Values).ToArray();
        }
        public static bool LoadAssetAtPathsByFilter(Type type,
                                                    string dirname,
                                                    LoadAssetFilter filter,
                                                    SortedDictionary<string, UnityEngine.Object> dict)
        {
            if (null == type || !typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                Debug.LogWarning(string.Format("LOAD_ASSET_BY_FILTER<{0}>:INVALID_TYPE:DIRNAME:{1}, FILTER:{2}",
                                                null != type ? type.Name : "null",
                                                null != dirname ? dirname : "null",
                                                filter));
                return false;
            }

            if (string.IsNullOrEmpty(dirname))
            {
                Debug.LogWarning(string.Format("LOAD_ASSET_BY_FILTER<{0}>:EMPTY_DIRNAME:FILTER:{1}",
                                                null != type ? type.Name : "null",
                                                filter));
                return false;
            }

            if (null == dict)
            {
                Debug.LogWarning(string.Format("LOAD_ASSET_BY_FILTER<{0}>:DICT_IS_NULL:DIRNAME:{1}, FILTER:{2}",
                                                null != type ? type.Name : "null",
                                                null != dirname ? dirname : "null",
                                                filter));
                return false;
            }

            if (!UnityExtension.OnLoadAssetAtPathsByFilter(type,
                                                           dirname,
                                                           filter,
                                                           dict))
            {
                Debug.LogWarning(string.Format("LOAD_ASSET_BY_FILTER<{0}>:FILE_NOT_FOUND:DIRNAME:{1}, FILTER:{2}",
                                                null != type ? type.Name : "null",
                                                null != dirname ? dirname : "null",
                                                filter));
                return false;
            }

            return true;
        }

        public class StringComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return x.CompareTo(y);
            }
        }
        enum AppendAssetResult
        {
            Ok,
            Duplicate,
            NotFound,
            InvalidLabel,
            Except,
        }
        static bool CheckAssetLabel(UnityEngine.Object asset, string[] filterLabels)
        {
            if (null == filterLabels)
                return true;

            var founded = false;
            var labels = UnityEditor.AssetDatabase.GetLabels(asset);
            if (null != labels)
            {
                foreach (var label in labels)
                {
                    foreach (var filterLabel in filterLabels)
                    {
                        if (label == filterLabel)
                        {
                            founded = true;
                            break;
                        }
                    }
                }
            }
            return founded;
        }
        static AppendAssetResult AppendAsset(Type type,
                                             string assetname,
                                             SortedDictionary<string, UnityEngine.Object> dict,
                                             string[] filterLabels = null)
        {
            if (dict.ContainsKey(assetname))
                return AppendAssetResult.Duplicate;

            try
            {
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath(assetname, type);
                if (asset)
                {
                    if (!UnityExtension.CheckAssetLabel(asset, filterLabels))
                        return AppendAssetResult.InvalidLabel;

                    dict.Add(assetname, asset);
                    return AppendAssetResult.Ok;
                }
                else
                    return AppendAssetResult.NotFound;
            }
            catch (Exception e)
            {
                Debug.LogWarning(string.Format("APPEND_ASSET<{0}>:ERROR:ASSET:{0}, EXCEPT:{1}",
                                                null != type ? type.Name : "null",
                                                assetname,
                                                e));
                return AppendAssetResult.Except;
            }
        }
        static bool OnLoadAssetAtPathsByFilter(Type type,
                                               string dirname,
                                               LoadAssetFilter filter,
                                               SortedDictionary<string, UnityEngine.Object> dict,
                                               bool start = true)
        {
            dirname = dirname.Replace('\\', '/');
            if ('/' != dirname[dirname.Length - 1])
                dirname += '/';

            if (!dirname.CheckValidAssetPath(true, start))
                return false;

            var assetFolder = dirname.ToAssetPath();
            var assetFolderLen = assetFolder.Length;
            var sb = new StringBuilder(assetFolder, 1024);
            var appended = false;
            if (null != filter.iterators)
            {
                foreach (var iterator in filter.iterators)
                {
                    sb.Length = assetFolderLen;
                    if (!string.IsNullOrEmpty(iterator.prefix))
                        sb.Append(iterator.prefix);
                    var prefixLen = sb.Length;

                    var format = default(string);
                    if (0 < iterator.zeroFilled)
                    {
                        format = iterator.hex ? string.Format("X{0}", iterator.zeroFilled) :
                                                string.Format("D{0}", iterator.zeroFilled);
                    }
                    else if (iterator.hex)
                        format = "X";

                    var index = iterator.start;
                    do
                    {
                        if (!string.IsNullOrEmpty(format))
                            sb.Append(index.ToString(format));
                        else
                            sb.Append(index);

                        if (!string.IsNullOrEmpty(iterator.ext))
                            sb.Append(iterator.ext);

                        var assetname = sb.ToString();
                        sb.Length = prefixLen;

                        var ret = UnityExtension.AppendAsset(type,
                                                             assetname,
                                                             dict,
                                                             filter.labels);
                        if (AppendAssetResult.Ok == ret)
                            appended = true;
                        else if (AppendAssetResult.NotFound == ret)
                            break;

                        ++index;
                    }
                    while (true);
                }
            }
            if (null != filter.names)
            {
                foreach (string name in filter.names)
                {
                    if (string.IsNullOrEmpty(name))
                        continue;

                    sb.Length = assetFolderLen;
                    sb.Append(name);
                    var assetname = sb.ToString();
                    var ret = UnityExtension.AppendAsset(type,
                                                         assetname,
                                                         dict,
                                                         filter.labels);
                    if (AppendAssetResult.Ok == ret)
                        appended = true;
                }
            }
            if (filter.bruteForce)
            {
                var filenames = Directory.GetFiles(dirname);
                foreach (string filename in filenames)
                {
                    if (string.IsNullOrEmpty(filename))
                        continue;

                    var name = filename.Remove(0, dirname.Length);
                    if (null != filter.conds)
                    {
                        var founded = false;
                        foreach (var cond in filter.conds)
                        {
                            if (cond.Evaluate(name))
                            {
                                founded = true;
                                break;
                            }
                        }
                        if (!founded)
                            continue;
                    }

                    sb.Length = assetFolderLen;
                    sb.Append(name);
                    var assetname = sb.ToString();
                    var ret = UnityExtension.AppendAsset(type,
                                                         assetname,
                                                         dict,
                                                         filter.labels);
                    if (AppendAssetResult.Ok == ret)
                        appended = true;
                }
            }
            if (filter.recursive)
            {
                var subnames = Directory.GetDirectories(dirname);
                foreach (var subname in subnames)
                {
                    appended |= UnityExtension.OnLoadAssetAtPathsByFilter(type,
                                                                          subname,
                                                                          filter,
                                                                          dict,
                                                                          false);
                }
            }

            return appended;
        }

        //public static Transform SelectedParent
        //{
        //    get
        //    {
        //        Transform[] ts = UnityEditor.Selection.transforms;
        //        if (null != ts)
        //        {
        //            foreach (Transform t in ts)
        //                if (t)
        //                    return t;
        //        }

        //        return null;
        //    }
        //}

        public static UnityEditor.EditorWindow GameView
        {
            get
            {
                System.Reflection.Assembly assembly = typeof(UnityEditor.EditorWindow).Assembly;
                Type type = assembly.GetType("UnityEditor.GameView");
                return UnityEditor.EditorWindow.GetWindow(type);
            }
        }

        public static Vector2 GameViewSize
        {
            get
            {
                System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
                System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                System.Object Res = GetSizeOfMainGameView.Invoke(null, null);
                return (Vector2)Res;
            }
        }
        public static Point2 GameViewSizeI
        {
            get
            {
                var gameViewSize = UnityExtension.GameViewSize;
                return new Point2((int)gameViewSize.x, (int)gameViewSize.y);
            }
        }

        public static void RepaintAllViews()
        {
            UnityEditor.SceneView.RepaintAll();
            UnityExtension.GameView.Repaint();
        }




        public static bool HasPersistantListener(UnityEngine.Events.UnityEventBase eventBase,
                                                 UnityEngine.Object target,
                                                 string methodName)
        {
            for (int n = 0, cnt = eventBase.GetPersistentEventCount(); n < cnt; ++n)
            {
                var name = eventBase.GetPersistentMethodName(n);
                var obj = eventBase.GetPersistentTarget(n);
                if (name == methodName && obj == target)
                    return true;
            }

            return false;
        }

        public static void SetOnClickPersistantListener(UnityEngine.UI.Button button,
                                                        UnityEngine.Object target,
                                                        string methodName,
                                                        UnityEngine.Events.UnityAction method)
        {
            if (!button)
                return;

            var eventBase = button.onClick;

            if (!UnityExtension.HasPersistantListener(eventBase, target, methodName))
                UnityEditor.Events.UnityEventTools.AddPersistentListener(eventBase, method);
        }

        public static void SetOnClickGenericPersistantListener<T>(UnityEngine.UI.Button button,
                                                                  UnityEngine.Object target,
                                                                  string methodName,
                                                                  UnityEngine.Events.UnityAction<T> method,
                                                                  T argument,
                                                                  Action<UnityEngine.Events.UnityEventBase, UnityEngine.Events.UnityAction<T>, T> register)
        {
            if (!button)
                return;

            var eventBase = button.onClick;

            if (!UnityExtension.HasPersistantListener(eventBase, target, methodName))
                register(eventBase, method, argument);
        }

        public static void SetOnClickBoolPersistantListener(UnityEngine.UI.Button button,
                                                            UnityEngine.Object target,
                                                            string methodName,
                                                            UnityEngine.Events.UnityAction<bool> method,
                                                            bool argument)
        {
            UnityExtension.SetOnClickGenericPersistantListener(button, target, methodName, method, argument,
                                                               UnityEditor.Events.UnityEventTools.AddBoolPersistentListener);
        }

        public static void SetOnClickFloatPersistantListener(UnityEngine.UI.Button button,
                                                             UnityEngine.Object target,
                                                             string methodName,
                                                             UnityEngine.Events.UnityAction<float> method,
                                                             float argument)
        {
            UnityExtension.SetOnClickGenericPersistantListener(button, target, methodName, method, argument,
                                                               UnityEditor.Events.UnityEventTools.AddFloatPersistentListener);
        }

        public static void SetOnClickIntPersistantListener(UnityEngine.UI.Button button,
                                                           UnityEngine.Object target,
                                                           string methodName,
                                                           UnityEngine.Events.UnityAction<int> method,
                                                           int argument)
        {
            UnityExtension.SetOnClickGenericPersistantListener(button, target, methodName, method, argument,
                                                               UnityEditor.Events.UnityEventTools.AddIntPersistentListener);
        }

        public static void SetOnClickObjectPersistantListener<T>(UnityEngine.UI.Button button,
                                                                 UnityEngine.Object target,
                                                                 string methodName,
                                                                 UnityEngine.Events.UnityAction<T> method,
                                                                 T argument) where T : UnityEngine.Object
        {
            UnityExtension.SetOnClickGenericPersistantListener(button, target, methodName, method, argument,
                                                               UnityEditor.Events.UnityEventTools.AddObjectPersistentListener);
        }

        public static void SetOnClickStringPersistantListener(UnityEngine.UI.Button button,
                                                              UnityEngine.Object target,
                                                              string methodName,
                                                              UnityEngine.Events.UnityAction<string> method,
                                                              string argument)
        {
            UnityExtension.SetOnClickGenericPersistantListener(button, target, methodName, method, argument,
                                                               UnityEditor.Events.UnityEventTools.AddStringPersistentListener);
        }
        
        public static void ClearOnClickPersistantListener(UnityEngine.UI.Button button)
        {
            if (!button)
                return;

            var eventBase = button.onClick;
            for (int n = eventBase.GetPersistentEventCount() - 1; n >= 0; --n)
                UnityEditor.Events.UnityEventTools.RemovePersistentListener(eventBase, n);
        }
#endif // UNITY_EDITOR

        public static Vector2 ScreenSize
        {
            get
            {
#if UNITY_EDITOR
                return UnityExtension.GameViewSize;
#else// UNITY_EDITOR
                return new Vector2(Screen.width, Screen.height);
#endif// UNITY_EDITOR
            }
        }
        public static Point2 ScreenSizeI
        {
            get
            {
#if UNITY_EDITOR
                return UnityExtension.GameViewSizeI;
#else// UNITY_EDITOR
                return new Point2(Screen.width, Screen.height);
#endif// UNITY_EDITOR
            }
        }

        public static Vector2 ActualCalculatedAreaSize(Vector2 areaSize, Vector2 baseSize)
        {
            var ratio = UnityExtension.GetScaleOfAreaSize(areaSize, baseSize);
            return areaSize / ratio;
        }

        public static Vector2 GetScaledScreenAreaSize(Vector2 baseSize)
        {
            var screenSize = UnityExtension.ScreenSize;
            return UnityExtension.ActualCalculatedAreaSize(screenSize, baseSize);
        }



#if UNITY_EDITOR
        static bool editorTestOnDummyArea = false;
        public static bool EditorTestOnDummyArea
        {
            set { UnityExtension.editorTestOnDummyArea = value; }
            get { return UnityExtension.editorTestOnDummyArea; }
        }
        static Vector2 editorTestDummyAreaPivot;
        public static Vector2 EditorTestDummyAreaPivot
        {
            set { UnityExtension.editorTestDummyAreaPivot = value; }
            get { return UnityExtension.editorTestDummyAreaPivot; }
        }
        static Vector2 editorTestDummyAreaRate = new Vector2(0.5f, 0.5f);
        public static Vector2 EditorTestDummyAreaRate
        {
            set { UnityExtension.editorTestDummyAreaRate = value; }
            get { return UnityExtension.editorTestDummyAreaRate; }
        }
        public static Rect EditorTestCalcDummyArea(float x01, float y01, float w01, float h01)
        {
            var screen = UnityExtension.ScreenSize;
            var area = new Vector2(screen.x * Mathf.Clamp01(w01), screen.y * Mathf.Clamp01(h01));
            var space = screen - area;
            return new Rect(space.x * Mathf.Clamp01(x01), space.y * Mathf.Clamp01(y01), area.x, area.y);
        }
#endif/// UNITY_EDITOR

        static Rect originSafeArea = new Rect(0, 0, 0, 0);
        static void SetOriginSafeArea(Rect value, bool force = false)
        {
            var originSize = UnityExtension.originSafeArea.size;
            if (originSize.magnitude <= 0 ||
                originSize == UnityExtension.ScreenSize ||
                force)
            {
                UnityExtension.originSafeArea = value;
            }
        }
        static Rect GetOriginSafeArea()
        {
            return UnityExtension.originSafeArea;
        }

        public static Rect ScreenSafeAreaRect
        {
            get
            {
                var result = new Rect(0, 0, 0, 0);
#if UNITY_EDITOR
                if (UnityExtension.editorTestOnDummyArea)
                {
                    result = UnityExtension.EditorTestCalcDummyArea(UnityExtension.editorTestDummyAreaPivot.x,
                                                                    UnityExtension.editorTestDummyAreaPivot.y,
                                                                    UnityExtension.editorTestDummyAreaRate.x,
                                                                    UnityExtension.editorTestDummyAreaRate.y);
                    UnityExtension.SetOriginSafeArea(result, true);
                    return UnityExtension.GetOriginSafeArea();
                }
#endif/// UNITY_EDITOR
#if UNITY_IOS && !UNITY_EDITOR
                result = IOSExtension.UI.IOSSafeArea.GetSafeArea();
                UnityExtension.SetOriginSafeArea(result);
                return UnityExtension.GetOriginSafeArea();
#else// UNITY_IOS && !UNITY_EDITOR
                result = new Rect(Vector2.zero, UnityExtension.ScreenSize);
                UnityExtension.SetOriginSafeArea(result);
                return UnityExtension.GetOriginSafeArea();
#endif// UNITY_IOS && !UNITY_EDITOR
            }
        }

        public static float GetScaleOfAreaSize(Vector2 areaSize, Vector2 baseSize)
        {
            var widthRatio = areaSize.x / baseSize.x;
            var heightRatio = areaSize.y / baseSize.y;
            var minRatio = Mathf.Min(widthRatio, heightRatio);
            var maxRatio = Mathf.Max(widthRatio, heightRatio);
            var scale = Mathf.Min(minRatio, maxRatio);

            return scale;
        }

        public static Rect GetCalculatedSafeAreaRect(Rect originSafeAreaRect, Vector2 baseSize)
        {
            var screenSize = UnityExtension.ScreenSize;
            var screenAspectRatio = screenSize.x / screenSize.y;
            
            var scale = 1f;
            var baseWidth = baseSize.x;
            var baseHeight = baseSize.y;
            var baseRatio = baseWidth / baseHeight;

            var xValue = baseWidth;
            var yValue = baseHeight;
            if (baseRatio < screenAspectRatio)
            {
                var areaWidth =
                xValue = Mathf.Round(baseHeight * screenAspectRatio);

                scale = areaWidth / screenSize.x;
            }
            else
            {
                var areaHeight =
                yValue = Mathf.Round(baseWidth / screenAspectRatio);

                scale = areaHeight / screenSize.y;
            }
            
            var w = Mathf.FloorToInt(originSafeAreaRect.width * scale);
            var h = Mathf.FloorToInt(originSafeAreaRect.height * scale);
            var x = Mathf.FloorToInt(((originSafeAreaRect.x * scale * 2) - (xValue - w)) * 0.5f);
            var y = Mathf.FloorToInt(((originSafeAreaRect.y * scale * 2) - (yValue - h)) * 0.5f);

            return new Rect(x, y, w, h);
        }
        public static Rect GetCalculatedSafeAreaRect(Vector2 baseSize)
        {
            return UnityExtension.GetCalculatedSafeAreaRect(UnityExtension.ScreenSafeAreaRect, baseSize);
        }


        public static Vector2 CalcExpandAspectScale(this Vector2 size)
        {
            Vector2 screenSize = UnityExtension.ScreenSize;

            float aspectRatio = size.x / size.y;
            float aspectRatioScreen = screenSize.x / screenSize.y;

            Vector2 scale = Vector2.one;
            if (aspectRatio < aspectRatioScreen)
                scale.x = aspectRatioScreen / aspectRatio;
            else
                scale.y = aspectRatio / aspectRatioScreen;

            return scale;
        }

        public static Vector2 CalcExpandAspectSize(this Vector2 size)
        {
            Vector2 scale = UnityExtension.CalcExpandAspectScale(size);
            size.x *= scale.x;
            size.y *= scale.y;
            return size;
        }








        public static GameObject FindObject(this GameObject go,
                                            string path)
        {
            var trans = UnityExtension.FindTransform(go, path);
            return trans ? trans.gameObject : null;
        }

        public static Transform FindTransform(this GameObject go,
                                              string path)
        {
            return go ? go.transform.Find(path) : null;
        }

        public static Canvas GetParentCanvas(this Component c)
        {
            return c ? c.GetComponentInParent<Canvas>() : null;
        }

        public static GameObject FindObject(this Transform trans,
                                            string path)
        {
            trans = UnityExtension.FindTransform(trans, path);
            return trans ? trans.gameObject : null;
        }

        public static Transform FindTransform(this Transform trans,
                                              string path)
        {
            return trans ? trans.Find(path) : null;
        }




        public static GameObject FindObject(this Component c,
                                            string path)
        {
            var trans = UnityExtension.FindTransform(c, path);
            return trans ? trans.gameObject : null;
        }

        public static Transform FindTransform(this Component c,
                                              string path)
        {
            return c ? c.transform.Find(path) : null;
        }




        public static GameObject FindObject(this ManagedComponent mc,
                                            string path)
        {
            var trans = UnityExtension.FindTransform(mc, path);
            return trans ? trans.gameObject : null;
        }

        public static Transform FindTransform(this ManagedComponent mc,
                                              string path)
        {
            return mc ? mc.CachedTransform.Find(path) : null;
        }







        public static T GetOrAddComponent<T>(this GameObject go,
                                             Action<T> createCallback = null)
            where T : Component
        {
            if (go)
            {
                var component = go.GetComponent<T>();
                if (!component)
                {
                    component = go.AddComponent<T>();
                    if (null != createCallback)
                        createCallback(component);
                }
            }

            return null;
        }

        public static T GetOrAddComponent<T>(this Component c,
                                             Action<T> createCallback = null)
            where T : Component
        {
            return c ? UnityExtension.GetOrAddComponent(c.gameObject, createCallback) : null;
        }



        public static T FindOrAddComponent<T>(this GameObject go,
                                              string path = null,
                                              Action<T> createCallback = null)
            where T : Component
        {
            if (go)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    var trans = UnityExtension.FindTransform(go, path);
                    if (trans)
                        return trans.GetOrAddComponent<T>();
                }
                else
                    return go.GetOrAddComponent<T>();
            }

            return null;
        }

        public static T FindOrAddComponent<T>(this Component c,
                                              string path = null,
                                              Action<T> createCallback = null)
            where T : Component
        {
            return c ? UnityExtension.FindOrAddComponent(c.gameObject, path, createCallback) : null;
        }
        



        public static T FindComponent<T>(this GameObject go,
                                         string path = null)
            where T : Component
        {
            if (go)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    var trans = UnityExtension.FindTransform(go, path);
                    if (trans)
                        return trans.GetComponent<T>();
                }
                else
                    return go.GetComponent<T>();
            }

            return null;
        }

        public static T FindComponent<T>(this Component c,
                                         string path = null)
            where T : Component
        {
            return c ? UnityExtension.FindComponent<T>(c.gameObject, path) : null;
        }
        



        public static T[] FindComponents<T>(this GameObject go,
                                            string path = null)
            where T : Component
        {
            if (go)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    var trans = UnityExtension.FindTransform(go, path);
                    if (trans)
                        return trans.GetComponents<T>();
                }
                else
                    return go.GetComponents<T>();
            }

            return new T[0];
        }

        public static T[] FindComponents<T>(this Component c,
                                            string path = null)
            where T : Component
        {
            return c ? UnityExtension.FindComponents<T>(c.gameObject, path) : new T[0];
        }



        public static T[] FindComponentsInChildren<T>(this GameObject go,
                                                      string path = null)
            where T : Component
        {
            if (go)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    var trans = UnityExtension.FindTransform(go, path);
                    if (trans)
                        return trans.GetComponentsInChildren<T>(true);
                }
                else
                    return go.GetComponentsInChildren<T>(true);
            }

            return new T[0];
        }

        public static T[] FindComponentsInChildren<T>(this Component c,
                                                      string path = null)
            where T : Component
        {
            return c ? UnityExtension.FindComponentsInChildren<T>(c.gameObject, path) : new T[0];
        }
        
        public static Transform[] FindTransformsInChildren(this GameObject go,
                                                           string path = null)
        {
            return UnityExtension.FindComponentsInChildren<Transform>(go, path);
        }

        public static Transform[] FindTransformsInChildren(this Component c,
                                                           string path = null)
        {
            return UnityExtension.FindComponentsInChildren<Transform>(c, path);
        }

        public static GameObject[] FindObjectsInChildren(this GameObject go,
                                                         string path = null)
        {
            if (go)
            {
                var ts = UnityExtension.FindTransformsInChildren(go, path);
                var gos = new GameObject[ts.Length];
                for (int n = 0, cnt = ts.Length; n < cnt; ++n)
                {
                    if (ts[n])
                        gos[n] = ts[n].gameObject;
                }

                return gos;
            }

            return new GameObject[0];
        }
        public static GameObject[] FindObjectsInChildren(this Component c,
                                                         string path = null)
        {
            return c ? UnityExtension.FindObjectsInChildren(c.gameObject, path) : new GameObject[0];
        }



        static T[] ToNameFilter<T>(T[] array,
                                   string keyword,
                                   SystemExtension.Filter filter = SystemExtension.Filter.Contains)
            where T : Component
        {
            var list = new List<T>(array.Length);
            for (int n = 0, cnt = array.Length; n < cnt; ++n)
            {
                var obj = array[n];
                if (!obj || !SystemExtension.ToFilter(obj.name, keyword, filter))
                    continue;

                list.Add(obj);
            }
            return list.ToArray();
        }

        static GameObject[] ToNameFilter(GameObject[] array,
                                         string keyword,
                                         SystemExtension.Filter filter = SystemExtension.Filter.Contains)
        {
            var list = new List<GameObject>(array.Length);
            for (int n = 0, cnt = array.Length; n < cnt; ++n)
            {
                var obj = array[n];
                if (!obj || !SystemExtension.ToFilter(obj.name, keyword, filter))
                    continue;

                list.Add(obj);
            }
            return list.ToArray();
        }


        public static T[] FindComponentsInChildrenWithName<T>(this GameObject go,
                                                              string keyword,
                                                              SystemExtension.Filter filter = SystemExtension.Filter.Contains,
                                                              string path = null)
            where T : Component
        {
            return UnityExtension.ToNameFilter(UnityExtension.FindComponentsInChildren<T>(go, path),
                                               keyword,
                                               filter);
        }

        public static T[] FindComponentsInChildrenWithName<T>(this Component c,
                                                              string keyword,
                                                              SystemExtension.Filter filter = SystemExtension.Filter.Contains,
                                                              string path = null)
            where T : Component
        {
            return UnityExtension.ToNameFilter(UnityExtension.FindComponentsInChildren<T>(c, path),
                                               keyword,
                                               filter);
        }

        public static GameObject[] FindObjectsInChildrenWithName(this GameObject go,
                                                                 string keyword,
                                                                 SystemExtension.Filter filter = SystemExtension.Filter.Contains,
                                                                 string path = null)
        {
            return UnityExtension.ToNameFilter(UnityExtension.FindObjectsInChildren(go, path),
                                               keyword,
                                               filter);
        }

        public static GameObject[] FindObjectsInChildrenWithName(this Component c,
                                                                 string keyword,
                                                                 SystemExtension.Filter filter = SystemExtension.Filter.Contains,
                                                                 string path = null)
        {
            return UnityExtension.ToNameFilter(UnityExtension.FindObjectsInChildren(c, path),
                                               keyword,
                                               filter);
        }



        static T[] ToTagFilter<T>(T[] array,
                                  string keyword,
                                  SystemExtension.Filter filter = SystemExtension.Filter.Contains)
            where T : Component
        {
            var list = new List<T>(array.Length);
            for (int n = 0, cnt = array.Length; n < cnt; ++n)
            {
                var obj = array[n];
                if (!obj || !SystemExtension.ToFilter(obj.tag, keyword, filter))
                    continue;

                list.Add(obj);
            }
            return list.ToArray();
        }

        static GameObject[] ToTagFilter(GameObject[] array,
                                        string keyword,
                                        SystemExtension.Filter filter = SystemExtension.Filter.Contains)
        {
            var list = new List<GameObject>(array.Length);
            for (int n = 0, cnt = array.Length; n < cnt; ++n)
            {
                var obj = array[n];
                if (!obj || !SystemExtension.ToFilter(obj.tag, keyword, filter))
                continue;

                list.Add(obj);
            }
            return list.ToArray();
        }
        
        public static T[] FindComponentsInChildrenWithTag<T>(this GameObject go,
                                                             string keyword,
                                                             SystemExtension.Filter filter = SystemExtension.Filter.Contains,
                                                             string path = null)
            where T : Component
        {
            return UnityExtension.ToTagFilter(UnityExtension.FindComponentsInChildren<T>(go, path),
                                              keyword,
                                              filter);
        }

        public static T[] FindComponentsInChildrenWithTag<T>(this Component c,
                                                             string keyword,
                                                             SystemExtension.Filter filter = SystemExtension.Filter.Contains,
                                                             string path = null)
            where T : Component
        {
            return UnityExtension.ToTagFilter(UnityExtension.FindComponentsInChildren<T>(c, path),
                                              keyword,
                                              filter);
        }

        public static GameObject[] FindObjectsInChildrenWithTag(this GameObject go,
                                                                string keyword,
                                                                SystemExtension.Filter filter = SystemExtension.Filter.Contains,
                                                                string path = null)
        {
            return UnityExtension.ToTagFilter(UnityExtension.FindObjectsInChildren(go, path),
                                              keyword,
                                              filter);
        }

        public static GameObject[] FindObjectsInChildrenWithTag(this Component c,
                                                                string keyword,
                                                                SystemExtension.Filter filter = SystemExtension.Filter.Contains,
                                                                string path = null)
        {
            return UnityExtension.ToTagFilter(UnityExtension.FindObjectsInChildren(c, path),
                                              keyword,
                                              filter);
        }


        
        static T[] ToOptionalFilter<T>(T[] array,
                                       string keyword,
                                       SystemExtension.Filter filter = SystemExtension.Filter.Contains)
            where T : Component
        {
            var list = new List<T>(array.Length);
            for (int n = 0, cnt = array.Length; n < cnt; ++n)
            {
                var component = array[n];
                if (!component)
                    continue;

                bool successful = false;
                var opts = component.GetComponents<OptionalComponent>();
                for (int n2 = 0, cnt2 = opts.Length; n2 < cnt2; ++n2)
                {
                    var opt = opts[n2];
                    if (!opt || !SystemExtension.ToFilter(opt.Options, keyword, filter))
                        continue;

                    var specific = opt.SpecificComponents;
                    if (null != specific && 0 < specific.Count)
                    {
                        for (int n3 = 0, cnt3 = specific.Count; n3 < cnt3; ++n3)
                        {
                            var specificComponent = specific[n3];
                            if (component == specificComponent)
                            {
                                successful = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        successful = true;
                        break;
                    }
                }

                if (successful)
                    list.Add(component);
            }
            return list.ToArray();
        }

        static GameObject[] ToOptionalFilter(GameObject[] array,
                                             string keyword,
                                             SystemExtension.Filter filter = SystemExtension.Filter.Contains)
        {
            var list = new List<GameObject>(array.Length);
            for (int n = 0, cnt = array.Length; n < cnt; ++n)
            {
                var obj = array[n];
                if (!obj)
                    continue;

                bool successful = false;
                var opts = obj.GetComponents<OptionalComponent>();
                for (int n2 = 0, cnt2 = opts.Length; n2 < cnt2; ++n2)
                {
                    var opt = opts[n2];
                    if (!opt || !SystemExtension.ToFilter(opt.Options, keyword, filter))
                        continue;

                    successful = true;
                    break;
                }

                if (successful)
                    list.Add(obj);
            }
            return list.ToArray();
        }

        public static T[] FindComponentsInChildrenWithOptional<T>(this GameObject go,
                                                                  string keyword,
                                                                  SystemExtension.Filter filter = SystemExtension.Filter.Contains,
                                                                  string path = null)
            where T : Component
        {
            return UnityExtension.ToOptionalFilter(UnityExtension.FindComponentsInChildren<T>(go, path),
                                                   keyword,
                                                   filter);
        }

        public static T[] FindComponentsInChildrenWithOptional<T>(this Component c,
                                                                  string keyword,
                                                                  SystemExtension.Filter filter = SystemExtension.Filter.Contains,
                                                                  string path = null)
            where T : Component
        {
            return UnityExtension.ToOptionalFilter(UnityExtension.FindComponentsInChildren<T>(c, path),
                                                   keyword,
                                                   filter);
        }

        public static GameObject[] FindObjectsInChildrenWithOptional(this GameObject go,
                                                                     string keyword,
                                                                     SystemExtension.Filter filter = SystemExtension.Filter.Contains,
                                                                     string path = null)
        {
            return UnityExtension.ToOptionalFilter(UnityExtension.FindObjectsInChildren(go, path),
                                                   keyword,
                                                   filter);
        }

        public static GameObject[] FindObjectsInChildrenWithOptional(this Component c,
                                                                     string keyword,
                                                                     SystemExtension.Filter filter = SystemExtension.Filter.Contains,
                                                                     string path = null)
        {
            return UnityExtension.ToOptionalFilter(UnityExtension.FindObjectsInChildren(c, path),
                                                   keyword,
                                                   filter);
        }




        public static T FindTraversal<T>(Transform trans,
                                         Func<Transform, T> command,
                                         bool left)
            where T : Component
        {
            var component = command(trans);
            if (component)
                return component;

            if (left)
            {
                for (int n = 0, count = trans.childCount; n < count; ++n)
                {
                    var child = trans.GetChild(n);
                    component = UnityExtension.FindTraversal<T>(child, command, left);
                    if (component)
                        return component;
                }
            }
            else
            {
                for (int n = trans.childCount - 1; n >= 0; --n)
                {
                    var child = trans.GetChild(n);
                    component = UnityExtension.FindTraversal<T>(child, command, left);
                    if (component)
                        return component;
                }
            }

            return null;
        }

        static T FindTraversalCommand_GetComponent<T>(Transform trans)
            where T : Component
        {
            return trans.GetComponent<T>();
        }
        public static T FindComponentInChildren<T>(this GameObject go,
                                                   string path = null,
                                                   bool left = true)
            where T : Component
        {
            if (go)
            {
                Func<Transform, T> command = UnityExtension.FindTraversalCommand_GetComponent<T>;

                if (!string.IsNullOrEmpty(path))
                {
                    var trans = UnityExtension.FindTransform(go, path);
                    if (trans)
                        return UnityExtension.FindTraversal<T>(trans, command, left);
                }
                else
                    return UnityExtension.FindTraversal<T>(go.transform, command, left);
            }

            return null;
        }
        public static T FindComponentInChildren<T>(this Component c,
                                                   string path = null,
                                                   bool left = true)
            where T : Component
        {
            return c ? UnityExtension.FindComponentInChildren<T>(c.gameObject, path, left) : null;
        }




        public static T FindComponentInChildrenWithName<T>(this GameObject go,
                                                           string keyword,
                                                           SystemExtension.Filter filter = SystemExtension.Filter.Contains,
                                                           string path = null,
                                                           bool left = true)
            where T : Component
        {
            if (go)
            {
                Func<Transform, T> command = (trans) =>
                {
                    if (SystemExtension.ToFilter(trans.name, keyword, filter))
                        return trans.GetComponent<T>();

                    return null;
                };

                if (!string.IsNullOrEmpty(path))
                {
                    var trans = UnityExtension.FindTransform(go, path);
                    if (trans)
                        return UnityExtension.FindTraversal<T>(trans, command, left);
                }
                else
                    return UnityExtension.FindTraversal<T>(go.transform, command, left);
            }

            return null;
        }
        public static T FindComponentInChildrenWithName<T>(this Component c,
                                                           string keyword,
                                                           SystemExtension.Filter filter = SystemExtension.Filter.Contains,
                                                           string path = null,
                                                           bool left = true)
            where T : Component
        {
            return c ? UnityExtension.FindComponentInChildrenWithName<T>(c.gameObject,
                                                                         keyword,
                                                                         filter,
                                                                         path,
                                                                         left) : null;
        }

        public static GameObject FindObjectInChildrenWithName(this GameObject go,
                                                              string keyword,
                                                              SystemExtension.Filter filter = SystemExtension.Filter.Contains,
                                                              string path = null,
                                                              bool left = true)
        {
            var trans = go ? UnityExtension.FindComponentInChildrenWithName<Transform>(go,
                                                                                       keyword,
                                                                                       filter,
                                                                                       path,
                                                                                       left) : null;
            return trans ? trans.gameObject : null;
        }

        public static GameObject FindObjectInChildrenWithName(this Component c,
                                                              string keyword,
                                                              SystemExtension.Filter filter = SystemExtension.Filter.Contains,
                                                              string path = null,
                                                              bool left = true)
        {
            var trans = c ? UnityExtension.FindComponentInChildrenWithName<Transform>(c.gameObject,
                                                                                      keyword,
                                                                                      filter,
                                                                                      path,
                                                                                      left) : null;
            return trans ? trans.gameObject : null;
        }




        public static T FindComponentInChildrenWithTag<T>(this GameObject go,
                                                          string keyword,
                                                          SystemExtension.Filter filter = SystemExtension.Filter.Contains,
                                                          string path = null,
                                                          bool left = true)
            where T : Component
        {
            if (go)
            {
                Func<Transform, T> command = (trans) =>
                {
                    if (SystemExtension.ToFilter(trans.tag, keyword, filter))
                        return trans.GetComponent<T>();

                    return null;
                };

                if (!string.IsNullOrEmpty(path))
                {
                    var trans = UnityExtension.FindTransform(go, path);
                    if (trans)
                        return UnityExtension.FindTraversal<T>(trans, command, left);
                }
                else
                    return UnityExtension.FindTraversal<T>(go.transform, command, left);
            }

            return null;
        }

        public static T FindComponentInChildrenWithTag<T>(this Component c,
                                                          string keyword,
                                                          SystemExtension.Filter filter = SystemExtension.Filter.Contains,
                                                          string path = null,
                                                          bool left = true)
            where T : Component
        {
            return c ? UnityExtension.FindComponentInChildrenWithTag<T>(c.gameObject,
                                                                        keyword,
                                                                        filter,
                                                                        path,
                                                                        left) : null;
        }

        public static GameObject FindObjectInChildrenWithTag(this GameObject go,
                                                             string keyword,
                                                             SystemExtension.Filter filter = SystemExtension.Filter.Contains,
                                                             string path = null,
                                                             bool left = true)
        {
            var trans = go ? UnityExtension.FindComponentInChildrenWithTag<Transform>(go,
                                                                                      keyword,
                                                                                      filter,
                                                                                      path,
                                                                                      left) : null;
            return trans ? trans.gameObject : null;
        }

        public static GameObject FindObjectInChildrenWithTag(this Component c,
                                                             string keyword,
                                                             SystemExtension.Filter filter = SystemExtension.Filter.Contains,
                                                             string path = null,
                                                             bool left = true)
        {
            var trans = c ? UnityExtension.FindComponentInChildrenWithTag<Transform>(c.gameObject,
                                                                                     keyword,
                                                                                     filter,
                                                                                     path,
                                                                                     left) : null;
            return trans ? trans.gameObject : null;
        }




        public static T FindComponentInChildrenWithOptional<T>(this GameObject go,
                                                               string keyword,
                                                               SystemExtension.Filter filter = SystemExtension.Filter.Contains,
                                                               string path = null,
                                                               bool left = true)
            where T : Component
        {
            if (go)
            {
                Func<Transform, T> command = (trans) =>
                {
                    var opts = trans.GetComponents<OptionalComponent>();
                    for (int n = 0, cnt = opts.Length; n < cnt; ++n)
                    {
                        var opt = opts[n];
                        if (!opt || !SystemExtension.ToFilter(opt.Options, keyword, filter))
                            continue;
                        
                        return trans.GetComponent<T>();
                    }
                    
                    return null;
                };

                if (!string.IsNullOrEmpty(path))
                {
                    var trans = UnityExtension.FindTransform(go, path);
                    if (trans)
                        return UnityExtension.FindTraversal<T>(trans, command, left);
                }
                else
                    return UnityExtension.FindTraversal<T>(go.transform, command, left);
            }

            return null;
        }

        public static T FindComponentInChildrenWithOptional<T>(this Component c,
                                                               string keyword,
                                                               SystemExtension.Filter filter = SystemExtension.Filter.Contains,
                                                               string path = null,
                                                               bool left = true)
            where T : Component
        {
            return c ? UnityExtension.FindComponentInChildrenWithOptional<T>(c.gameObject,
                                                                             keyword,
                                                                             filter,
                                                                             path,
                                                                             left) : null;
        }

        public static GameObject FindObjectInChildrenWithOptional(this GameObject go,
                                                                  string keyword,
                                                                  SystemExtension.Filter filter = SystemExtension.Filter.Contains,
                                                                  string path = null,
                                                                  bool left = true)
        {
            var trans = go ? UnityExtension.FindComponentInChildrenWithOptional<Transform>(go,
                                                                                           keyword,
                                                                                           filter,
                                                                                           path,
                                                                                           left) : null;
            return trans ? trans.gameObject : null;
        }

        public static GameObject FindObjectInChildrenWithOptional(this Component c,
                                                                  string keyword,
                                                                  SystemExtension.Filter filter = SystemExtension.Filter.Contains,
                                                                  string path = null,
                                                                  bool left = true)
        {
            var trans = c ? UnityExtension.FindComponentInChildrenWithOptional<Transform>(c.gameObject,
                                                                                          keyword,
                                                                                          filter,
                                                                                          path,
                                                                                          left) : null;
            return trans ? trans.gameObject : null;
        }




        public static Texture2D ToTexture2D(this Sprite sprite, Rect addPixels = default(Rect))
        {
            var rect = sprite.textureRect;
            rect.xMin += addPixels.xMin;
            rect.xMax += addPixels.xMax;

            if (rect.width != sprite.texture.width)
            {
                rect.yMin += addPixels.yMin;
                rect.yMax += addPixels.yMax;

                var w = (int)rect.width;
                var h = (int)rect.height;
                //Debug.Log(sprite.name + ", W:" + w + ", H:" + h + ", R:" + rect + ", MIN:" + rect.min + ", MAX:" + rect.max);
                try
                {
                    var texture = new Texture2D(w, h);
                    var colors = sprite.texture.GetPixels((int)rect.x, (int)rect.y, w, h);
                    texture.SetPixels(colors);
                    texture.Apply();
                    return texture;
                }
                catch (Exception e)
                {
                    throw new Exception(sprite.name + ", W:" + w + ", H:" + h + ", R:" + rect + ", MIN:" + rect.min + ", MAX:" + rect.max + "\nE:" + e.ToString());
                }
            }
            else
                return sprite.texture;
        }

        public static Color GetColorChannel(this TextureFormat format)
        {
            switch (format)
            {
//          case TextureFormat.PVRTC_2BPP_RGB:      return new Color(1, 1, 1, 0);   // PVRTC_2BPP_RGB       Use PVRTC_RGB2 instead (UnityUpgradable)
//          case TextureFormat.PVRTC_2BPP_RGBA:     return new Color(1, 1, 1, 1);   // PVRTC_2BPP_RGBA      Use PVRTC_RGBA2 instead (UnityUpgradable)
//          case TextureFormat.PVRTC_4BPP_RGB:      return new Color(1, 1, 1, 0);   // PVRTC_4BPP_RGB       Use PVRTC_RGB4 instead (UnityUpgradable)
//          case TextureFormat.PVRTC_4BPP_RGBA:     return new Color(1, 1, 1, 1);   // PVRTC_4BPP_RGBA      Use PVRTC_RGBA4 instead (UnityUpgradable)
            case TextureFormat.Alpha8:              return new Color(0, 0, 0, 1);   // Alpha8               Alpha-only texture format.
            case TextureFormat.ARGB4444:            return new Color(1, 1, 1, 1);   // ARGB4444             A 16 bits/pixel texture format.Texture stores color with an alpha channel.
            case TextureFormat.RGB24:               return new Color(1, 1, 1, 0);   // RGB24                Color texture format, 8-bits per channel.
            case TextureFormat.RGBA32:              return new Color(1, 1, 1, 1);   // RGBA32               Color with alpha texture format, 8-bits per channel.
            case TextureFormat.ARGB32:              return new Color(1, 1, 1, 1);   // ARGB32               Color with alpha texture format, 8-bits per channel.
            case TextureFormat.RGB565:              return new Color(1, 1, 1, 0);   // RGB565               A 16 bit color texture format.
            case TextureFormat.R16:                 return new Color(1, 0, 0, 0);   // R16                  A 16 bit color texture format that only has a red channel.
            case TextureFormat.DXT1:                return new Color(1, 1, 1, 0);   // DXT1                 Compressed color texture format.
            case TextureFormat.DXT5:                return new Color(1, 1, 1, 1);   // DXT5                 Compressed color with alpha channel texture format.
            case TextureFormat.RGBA4444:            return new Color(1, 1, 1, 1);   // RGBA4444             Color and alpha texture format, 4 bit per channel.
            case TextureFormat.BGRA32:              return new Color(1, 1, 1, 1);   // BGRA32               Color with alpha texture format, 8-bits per channel.
            case TextureFormat.RHalf:               return new Color(1, 0, 0, 0);   // RHalf                Scalar (R) texture format, 16 bit floating point.
            case TextureFormat.RGHalf:              return new Color(1, 1, 0, 0);   // RGHalf               Two color (RG) texture format, 16 bit floating point per channel.
            case TextureFormat.RGBAHalf:            return new Color(1, 1, 1, 1);   // RGBAHalf             RGB color and alpha texture format, 16 bit floating point per channel.
            case TextureFormat.RFloat:              return new Color(1, 0, 0, 0);   // RFloat               Scalar (R) texture format, 32 bit floating point.
            case TextureFormat.RGFloat:             return new Color(1, 1, 0, 0);   // RGFloat              Two color (RG) texture format, 32 bit floating point per channel.
            case TextureFormat.RGBAFloat:           return new Color(1, 1, 1, 1);   // RGBAFloat            RGB color and alpha texture format, 32-bit floats per channel.
            case TextureFormat.YUY2:                return new Color(0, 0, 0, 0);   // YUY2                 A format that uses the YUV color space and is often used for video encoding or playback.
            case TextureFormat.RGB9e5Float:         return new Color(1, 1, 1, 0);   // RGB9e5Float          RGB HDR format, with 9 bit mantissa per channel and a 5 bit shared exponent.
            case TextureFormat.BC4:                 return new Color(1, 0, 0, 0);   // BC4                  Compressed one channel (R) texture format.
            case TextureFormat.BC5:                 return new Color(1, 1, 0, 0);   // BC5                  Compressed two-channel (RG) texture format.
            case TextureFormat.BC6H:                return new Color(1, 1, 1, 1);   // BC6H                 HDR compressed color texture format.
            case TextureFormat.BC7:                 return new Color(1, 1, 1, 1);   // BC7                  High quality compressed color texture format.
            case TextureFormat.DXT1Crunched:        return new Color(1, 1, 1, 0);   // DXT1Crunched         Compressed color texture format with Crunch compression for smaller storage sizes.
            case TextureFormat.DXT5Crunched:        return new Color(1, 1, 1, 1);   // DXT5Crunched         Compressed color with alpha channel texture format with Crunch compression for smaller storage sizes.
            case TextureFormat.PVRTC_RGB2:          return new Color(1, 1, 1, 0);   // PVRTC_RGB2           PowerVR (iOS) 2 bits/pixel compressed color texture format.
            case TextureFormat.PVRTC_RGBA2:         return new Color(1, 1, 1, 1);   // PVRTC_RGBA2          PowerVR (iOS) 2 bits/pixel compressed with alpha channel texture format.
            case TextureFormat.PVRTC_RGB4:          return new Color(1, 1, 1, 0);   // PVRTC_RGB4           PowerVR (iOS) 4 bits/pixel compressed color texture format.
            case TextureFormat.PVRTC_RGBA4:         return new Color(1, 1, 1, 1);   // PVRTC_RGBA4          PowerVR (iOS) 4 bits/pixel compressed with alpha channel texture format.
            case TextureFormat.ETC_RGB4:            return new Color(1, 1, 1, 0);   // ETC_RGB4             ETC (GLES2.0) 4 bits/pixel compressed RGB texture format.
            //case TextureFormat.ATC_RGB4:            return new Color(1, 1, 1, 0);   // ATC_RGB4             ATC (ATITC) 4 bits/pixel compressed RGB texture format.
            //case TextureFormat.ATC_RGBA8:           return new Color(1, 1, 1, 1);   // ATC_RGBA8            ATC (ATITC) 8 bits/pixel compressed RGB texture format.
            case TextureFormat.EAC_R:               return new Color(1, 0, 0, 0);   // EAC_R                ETC2 / EAC (GL ES 3.0) 4 bits/pixel compressed unsigned single-channel texture format.
            case TextureFormat.EAC_R_SIGNED:        return new Color(1, 0, 0, 0);   // EAC_R_SIGNED         ETC2 / EAC (GL ES 3.0) 4 bits/pixel compressed signed single-channel texture format.
            case TextureFormat.EAC_RG:              return new Color(1, 1, 0, 0);   // EAC_RG               ETC2 / EAC (GL ES 3.0) 8 bits/pixel compressed unsigned dual-channel(RG) texture format.
            case TextureFormat.EAC_RG_SIGNED:       return new Color(1, 1, 0, 0);   // EAC_RG_SIGNED        ETC2 / EAC (GL ES 3.0) 8 bits/pixel compressed signed dual-channel(RG) texture format.
            case TextureFormat.ETC2_RGB:            return new Color(1, 1, 1, 0);   // ETC2_RGB             ETC2 (GL ES 3.0) 4 bits/pixel compressed RGB texture format.
            case TextureFormat.ETC2_RGBA1:          return new Color(1, 1, 1, 1);   // ETC2_RGBA1           ETC2 (GL ES 3.0) 4 bits/pixel RGB+1-bit alpha texture format.
            case TextureFormat.ETC2_RGBA8:          return new Color(1, 1, 1, 1);   // ETC2_RGBA8           ETC2 (GL ES 3.0) 8 bits/pixel compressed RGBA texture format.
            case TextureFormat.ASTC_RGB_4x4:        return new Color(1, 1, 1, 0);   // ASTC_RGB_4x4         ASTC (4x4 pixel block in 128 bits) compressed RGB texture format.
            case TextureFormat.ASTC_RGB_5x5:        return new Color(1, 1, 1, 0);   // ASTC_RGB_5x5         ASTC (5x5 pixel block in 128 bits) compressed RGB texture format.
            case TextureFormat.ASTC_RGB_6x6:        return new Color(1, 1, 1, 0);   // ASTC_RGB_6x6         ASTC (6x6 pixel block in 128 bits) compressed RGB texture format.
            case TextureFormat.ASTC_RGB_8x8:        return new Color(1, 1, 1, 0);   // ASTC_RGB_8x8         ASTC (8x8 pixel block in 128 bits) compressed RGB texture format.
            case TextureFormat.ASTC_RGB_10x10:      return new Color(1, 1, 1, 0);   // ASTC_RGB_10x10       ASTC (10x10 pixel block in 128 bits) compressed RGB texture format.
            case TextureFormat.ASTC_RGB_12x12:      return new Color(1, 1, 1, 0);   // ASTC_RGB_12x12       ASTC (12x12 pixel block in 128 bits) compressed RGB texture format.
            case TextureFormat.ASTC_RGBA_4x4:       return new Color(1, 1, 1, 1);   // ASTC_RGBA_4x4        ASTC (4x4 pixel block in 128 bits) compressed RGBA texture format.
            case TextureFormat.ASTC_RGBA_5x5:       return new Color(1, 1, 1, 1);   // ASTC_RGBA_5x5        ASTC (5x5 pixel block in 128 bits) compressed RGBA texture format.
            case TextureFormat.ASTC_RGBA_6x6:       return new Color(1, 1, 1, 1);   // ASTC_RGBA_6x6        ASTC (6x6 pixel block in 128 bits) compressed RGBA texture format.
            case TextureFormat.ASTC_RGBA_8x8:       return new Color(1, 1, 1, 1);   // ASTC_RGBA_8x8        ASTC (8x8 pixel block in 128 bits) compressed RGBA texture format.
            case TextureFormat.ASTC_RGBA_10x10:     return new Color(1, 1, 1, 1);   // ASTC_RGBA_10x10      ASTC (10x10 pixel block in 128 bits) compressed RGBA texture format.
            case TextureFormat.ASTC_RGBA_12x12:     return new Color(1, 1, 1, 1);   // ASTC_RGBA_12x12      ASTC (12x12 pixel block in 128 bits) compressed RGBA texture format.
            case TextureFormat.ETC_RGB4_3DS:        return new Color(1, 1, 1, 0);   // ETC_RGB4_3DS         ETC 4 bits/pixel compressed RGB texture format.
            case TextureFormat.ETC_RGBA8_3DS:       return new Color(1, 1, 1, 1);   // ETC_RGBA8_3DS        ETC 4 bits/pixel RGB + 4 bits/pixel Alpha compressed texture format.
            case TextureFormat.RG16:                return new Color(1, 1, 0, 0);   // RG16                 Two color (RG) texture format, 8-bits per channel.
            case TextureFormat.R8:                  return new Color(1, 0, 0, 0);   // R8                   Scalar (R) render texture format, 8 bit fixed point.
            case TextureFormat.ETC_RGB4Crunched:    return new Color(1, 1, 1, 0);   // ETC_RGB4Crunched     Compressed color texture format with Crunch compression for smaller storage sizes.
            case TextureFormat.ETC2_RGBA8Crunched:  return new Color(1, 1, 1, 1);   // ETC2_RGBA8Crunched   Compressed color with alpha channel texture format using Crunch compression for smaller storage sizes.
                default:                                return new Color(0, 0, 0, 0);   // UNDEFINED            N/A
            }
        }
        public static Color GetTransparentChannel(this TextureFormat format)
        {
            switch (format)
            {
//          case TextureFormat.PVRTC_2BPP_RGB:      return new Color(1, 1, 1, 0);   // PVRTC_2BPP_RGB       Use PVRTC_RGB2 instead (UnityUpgradable)
//          case TextureFormat.PVRTC_2BPP_RGBA:     return new Color(0, 0, 0, 1);   // PVRTC_2BPP_RGBA      Use PVRTC_RGBA2 instead (UnityUpgradable)
//          case TextureFormat.PVRTC_4BPP_RGB:      return new Color(1, 1, 1, 0);   // PVRTC_4BPP_RGB       Use PVRTC_RGB4 instead (UnityUpgradable)
//          case TextureFormat.PVRTC_4BPP_RGBA:     return new Color(0, 0, 0, 1);   // PVRTC_4BPP_RGBA      Use PVRTC_RGBA4 instead (UnityUpgradable)
            case TextureFormat.Alpha8:              return new Color(0, 0, 0, 1);   // Alpha8               Alpha-only texture format.
            case TextureFormat.ARGB4444:            return new Color(0, 0, 0, 1);   // ARGB4444             A 16 bits/pixel texture format.Texture stores color with an alpha channel.
            case TextureFormat.RGB24:               return new Color(1, 1, 1, 0);   // RGB24                Color texture format, 8-bits per channel.
            case TextureFormat.RGBA32:              return new Color(0, 0, 0, 1);   // RGBA32               Color with alpha texture format, 8-bits per channel.
            case TextureFormat.ARGB32:              return new Color(0, 0, 0, 1);   // ARGB32               Color with alpha texture format, 8-bits per channel.
            case TextureFormat.RGB565:              return new Color(1, 1, 1, 0);   // RGB565               A 16 bit color texture format.
            case TextureFormat.R16:                 return new Color(1, 0, 0, 0);   // R16                  A 16 bit color texture format that only has a red channel.
            case TextureFormat.DXT1:                return new Color(1, 1, 1, 0);   // DXT1                 Compressed color texture format.
            case TextureFormat.DXT5:                return new Color(0, 0, 0, 1);   // DXT5                 Compressed color with alpha channel texture format.
            case TextureFormat.RGBA4444:            return new Color(0, 0, 0, 1);   // RGBA4444             Color and alpha texture format, 4 bit per channel.
            case TextureFormat.BGRA32:              return new Color(0, 0, 0, 1);   // BGRA32               Color with alpha texture format, 8-bits per channel.
            case TextureFormat.RHalf:               return new Color(1, 0, 0, 0);   // RHalf                Scalar (R) texture format, 16 bit floating point.
            case TextureFormat.RGHalf:              return new Color(1, 1, 0, 0);   // RGHalf               Two color (RG) texture format, 16 bit floating point per channel.
            case TextureFormat.RGBAHalf:            return new Color(0, 0, 0, 1);   // RGBAHalf             RGB color and alpha texture format, 16 bit floating point per channel.
            case TextureFormat.RFloat:              return new Color(1, 0, 0, 0);   // RFloat               Scalar (R) texture format, 32 bit floating point.
            case TextureFormat.RGFloat:             return new Color(1, 1, 0, 0);   // RGFloat              Two color (RG) texture format, 32 bit floating point per channel.
            case TextureFormat.RGBAFloat:           return new Color(0, 0, 0, 1);   // RGBAFloat            RGB color and alpha texture format, 32-bit floats per channel.
            case TextureFormat.YUY2:                return new Color(0, 0, 0, 0);   // YUY2                 A format that uses the YUV color space and is often used for video encoding or playback.
            case TextureFormat.RGB9e5Float:         return new Color(1, 1, 1, 0);   // RGB9e5Float          RGB HDR format, with 9 bit mantissa per channel and a 5 bit shared exponent.
            case TextureFormat.BC4:                 return new Color(1, 0, 0, 0);   // BC4                  Compressed one channel (R) texture format.
            case TextureFormat.BC5:                 return new Color(1, 1, 0, 0);   // BC5                  Compressed two-channel (RG) texture format.
            case TextureFormat.BC6H:                return new Color(0, 0, 0, 1);   // BC6H                 HDR compressed color texture format.
            case TextureFormat.BC7:                 return new Color(0, 0, 0, 1);   // BC7                  High quality compressed color texture format.
            case TextureFormat.DXT1Crunched:        return new Color(1, 1, 1, 0);   // DXT1Crunched         Compressed color texture format with Crunch compression for smaller storage sizes.
            case TextureFormat.DXT5Crunched:        return new Color(0, 0, 0, 1);   // DXT5Crunched         Compressed color with alpha channel texture format with Crunch compression for smaller storage sizes.
            case TextureFormat.PVRTC_RGB2:          return new Color(1, 1, 1, 0);   // PVRTC_RGB2           PowerVR (iOS) 2 bits/pixel compressed color texture format.
            case TextureFormat.PVRTC_RGBA2:         return new Color(0, 0, 0, 1);   // PVRTC_RGBA2          PowerVR (iOS) 2 bits/pixel compressed with alpha channel texture format.
            case TextureFormat.PVRTC_RGB4:          return new Color(1, 1, 1, 0);   // PVRTC_RGB4           PowerVR (iOS) 4 bits/pixel compressed color texture format.
            case TextureFormat.PVRTC_RGBA4:         return new Color(0, 0, 0, 1);   // PVRTC_RGBA4          PowerVR (iOS) 4 bits/pixel compressed with alpha channel texture format.
            case TextureFormat.ETC_RGB4:            return new Color(1, 1, 1, 0);   // ETC_RGB4             ETC (GLES2.0) 4 bits/pixel compressed RGB texture format.
            //case TextureFormat.ATC_RGB4:            return new Color(1, 1, 1, 0);   // ATC_RGB4             ATC (ATITC) 4 bits/pixel compressed RGB texture format.
            //case TextureFormat.ATC_RGBA8:           return new Color(0, 0, 0, 1);   // ATC_RGBA8            ATC (ATITC) 8 bits/pixel compressed RGB texture format.
            case TextureFormat.EAC_R:               return new Color(1, 0, 0, 0);   // EAC_R                ETC2 / EAC (GL ES 3.0) 4 bits/pixel compressed unsigned single-channel texture format.
            case TextureFormat.EAC_R_SIGNED:        return new Color(1, 0, 0, 0);   // EAC_R_SIGNED         ETC2 / EAC (GL ES 3.0) 4 bits/pixel compressed signed single-channel texture format.
            case TextureFormat.EAC_RG:              return new Color(1, 1, 0, 0);   // EAC_RG               ETC2 / EAC (GL ES 3.0) 8 bits/pixel compressed unsigned dual-channel(RG) texture format.
            case TextureFormat.EAC_RG_SIGNED:       return new Color(1, 1, 0, 0);   // EAC_RG_SIGNED        ETC2 / EAC (GL ES 3.0) 8 bits/pixel compressed signed dual-channel(RG) texture format.
            case TextureFormat.ETC2_RGB:            return new Color(1, 1, 1, 0);   // ETC2_RGB             ETC2 (GL ES 3.0) 4 bits/pixel compressed RGB texture format.
            case TextureFormat.ETC2_RGBA1:          return new Color(0, 0, 0, 1);   // ETC2_RGBA1           ETC2 (GL ES 3.0) 4 bits/pixel RGB+1-bit alpha texture format.
            case TextureFormat.ETC2_RGBA8:          return new Color(0, 0, 0, 1);   // ETC2_RGBA8           ETC2 (GL ES 3.0) 8 bits/pixel compressed RGBA texture format.
            case TextureFormat.ASTC_RGB_4x4:        return new Color(1, 1, 1, 0);   // ASTC_RGB_4x4         ASTC (4x4 pixel block in 128 bits) compressed RGB texture format.
            case TextureFormat.ASTC_RGB_5x5:        return new Color(1, 1, 1, 0);   // ASTC_RGB_5x5         ASTC (5x5 pixel block in 128 bits) compressed RGB texture format.
            case TextureFormat.ASTC_RGB_6x6:        return new Color(1, 1, 1, 0);   // ASTC_RGB_6x6         ASTC (6x6 pixel block in 128 bits) compressed RGB texture format.
            case TextureFormat.ASTC_RGB_8x8:        return new Color(1, 1, 1, 0);   // ASTC_RGB_8x8         ASTC (8x8 pixel block in 128 bits) compressed RGB texture format.
            case TextureFormat.ASTC_RGB_10x10:      return new Color(1, 1, 1, 0);   // ASTC_RGB_10x10       ASTC (10x10 pixel block in 128 bits) compressed RGB texture format.
            case TextureFormat.ASTC_RGB_12x12:      return new Color(1, 1, 1, 0);   // ASTC_RGB_12x12       ASTC (12x12 pixel block in 128 bits) compressed RGB texture format.
            case TextureFormat.ASTC_RGBA_4x4:       return new Color(0, 0, 0, 1);   // ASTC_RGBA_4x4        ASTC (4x4 pixel block in 128 bits) compressed RGBA texture format.
            case TextureFormat.ASTC_RGBA_5x5:       return new Color(0, 0, 0, 1);   // ASTC_RGBA_5x5        ASTC (5x5 pixel block in 128 bits) compressed RGBA texture format.
            case TextureFormat.ASTC_RGBA_6x6:       return new Color(0, 0, 0, 1);   // ASTC_RGBA_6x6        ASTC (6x6 pixel block in 128 bits) compressed RGBA texture format.
            case TextureFormat.ASTC_RGBA_8x8:       return new Color(0, 0, 0, 1);   // ASTC_RGBA_8x8        ASTC (8x8 pixel block in 128 bits) compressed RGBA texture format.
            case TextureFormat.ASTC_RGBA_10x10:     return new Color(0, 0, 0, 1);   // ASTC_RGBA_10x10      ASTC (10x10 pixel block in 128 bits) compressed RGBA texture format.
            case TextureFormat.ASTC_RGBA_12x12:     return new Color(0, 0, 0, 1);   // ASTC_RGBA_12x12      ASTC (12x12 pixel block in 128 bits) compressed RGBA texture format.
            case TextureFormat.ETC_RGB4_3DS:        return new Color(1, 1, 1, 0);   // ETC_RGB4_3DS         ETC 4 bits/pixel compressed RGB texture format.
            case TextureFormat.ETC_RGBA8_3DS:       return new Color(0, 0, 0, 1);   // ETC_RGBA8_3DS        ETC 4 bits/pixel RGB + 4 bits/pixel Alpha compressed texture format.
            case TextureFormat.RG16:                return new Color(1, 1, 0, 0);   // RG16                 Two color (RG) texture format, 8-bits per channel.
            case TextureFormat.R8:                  return new Color(1, 0, 0, 0);   // R8                   Scalar (R) render texture format, 8 bit fixed point.
            case TextureFormat.ETC_RGB4Crunched:    return new Color(1, 1, 1, 0);   // ETC_RGB4Crunched     Compressed color texture format with Crunch compression for smaller storage sizes.
            case TextureFormat.ETC2_RGBA8Crunched:  return new Color(0, 0, 0, 1);   // ETC2_RGBA8Crunched   Compressed color with alpha channel texture format using Crunch compression for smaller storage sizes.
            default:                                return new Color(0, 0, 0, 0);   // UNDEFINED            N/A
            }
        }

        public static Color Convert(this Color color, TextureFormat format)
        {
            return color * UnityExtension.GetColorChannel(format);
        }

        public static bool IsTransparent(this Color color, TextureFormat format)
        {
            var transparent = color * UnityExtension.GetTransparentChannel(format);
            return 0 == transparent.r &&
                   0 == transparent.g &&
                   0 == transparent.b &&
                   0 == transparent.a;
        }

        public static bool IsTransparency(this Texture2D texture,
                                          TextureFormat format,
                                          int startX,
                                          int startY,
                                          int endX,
                                          int endY)
        {
            for (var y = startY; y <= endY; ++y)
                for (var x = startX; x <= endX; ++x)
                    if (!texture.GetPixel(x, y).IsTransparent(format))
                        return false;

            return true;
        }

#if UNITY_ANDROID
        public static AndroidJavaObject ArrayToJava<T>(this T[] values, string elemClassJava)
        {
            AndroidJavaClass arrayClass = new AndroidJavaClass("java.lang.reflect.Array");
            AndroidJavaObject arrayObject = arrayClass.CallStatic<AndroidJavaObject>("newInstance", new AndroidJavaClass(elemClassJava), values.Length);
            for (int n = 0; n < values.Length; ++n)
            {
                arrayClass.CallStatic("set", arrayObject, n, new AndroidJavaObject(elemClassJava, values[n]));
            }

            return arrayObject;
        }

        public static AndroidJavaObject StringArrayToJava(this string[] values)
        {
            return values.ArrayToJava("java.lang.String");
        }
#endif// UNITY_ANDROID

#if UNITY_EDITOR
        public static Dictionary<C, List<T>> FindAllSerializableInstancesInHierarchy<C, T>(GameObject value)
            where C : Component
            where T : class
        {
            var foundAll = new Dictionary<C, List<T>>();
            if (value)
            {
                var exploredObjects = new HashSet<object>();
                var components = value.GetComponentsInChildren<C>(true);
                foreach (var component in components)
                {
                    if (!component)
                        continue;

                    var found = new List<T>();
                    UnityExtension.FindAllSerializableInstances(component, exploredObjects, found);

                    if (0 < found.Count)
                    {
                        if (!foundAll.ContainsKey(component))
                        {
                            foundAll.Add(component, found);
                        }
                        else
                        {
                            var old = foundAll[component];
                            foreach (var newItem in found)
                                if (!old.Contains(newItem))
                                    old.Add(newItem);
                        }
                    }
                }
            }

            return foundAll;
        }

        public static List<T> FindAllSerializableInstances<T>(object value) where T : class
        {
            var found = new List<T>();
            var exploredObjects = new HashSet<object>();
            UnityExtension.FindAllSerializableInstances(value, exploredObjects, found);

            return found;
        }

        static void FindAllSerializableInstances<T>(object value, HashSet<object> exploredObjects, List<T> found) where T : class
        {
            if (null == value)
                return;

            if (exploredObjects.Contains(value))
                return;

            exploredObjects.Add(value);

            try
            {
                var enumerable = value as IEnumerable;
                if (enumerable != null)
                {
                    foreach (var item in enumerable)
                    {
                        if (null == item)
                            continue;

                        UnityExtension.FindAllSerializableInstances<T>(item, exploredObjects, found);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }

            T possibleMatch = value as T;

            if (possibleMatch != null)
            {
                found.Add(possibleMatch);
            }

            var type = value.GetType();
            while (null != type &&
                   (type.IsSubclassOf(typeof(MonoBehaviour)) || type.IsSerializable) &&
                   (null == type.Namespace || (0 != type.Namespace.IndexOf("System") && 0 != type.Namespace.IndexOf("Unity"))))

            {
                //var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty);

                //foreach (var property in properties)
                //{
                //    var indexer = property.GetIndexParameters();
                //    if (null == indexer || 0 == indexer.Length)
                //    {
                //        var propertyValue = property.GetValue(value, null);
                //        UnityExtension.FindAllInstances<T>(propertyValue, exploredObjects, found);
                //    }
                //}

                var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

                foreach (var field in fields)
                {
                    var fieldValue = field.GetValue(value);
                    if (null == fieldValue)
                        continue;

                    UnityExtension.FindAllSerializableInstances<T>(fieldValue, exploredObjects, found);
                }

                type = type.BaseType;
            }
        }
#endif// UNITY_EDITOR
    }
}