using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
namespace Ext.Unity3D.UI
{
    public static class UIExtension
    {
        public static T CreateUIObject<T>(string name, Transform parent) where T : Component
        {
            return UIExtension.CreateUIObject<T>(name, parent, Vector2.zero, Vector2.zero, Vector3.one, Quaternion.identity);
        }
        public static T CreateUIObject<T>(string name, Transform parent, Vector2 anchoredPos) where T : Component
        {
            return UIExtension.CreateUIObject<T>(name, parent, anchoredPos, Vector2.zero, Vector3.one, Quaternion.identity);
        }
        public static T CreateUIObject<T>(string name, Transform parent, Vector2 anchoredPos, Vector2 size) where T : Component
        {
            return UIExtension.CreateUIObject<T>(name, parent, anchoredPos, size, Vector3.one, Quaternion.identity);
        }
        public static T CreateUIObject<T>(string name, Transform parent, Vector2 anchoredPos, Vector2 size, Vector3 scale) where T : Component
        {
            return UIExtension.CreateUIObject<T>(name, parent, anchoredPos, size, scale, Quaternion.identity);
        }
        public static T CreateUIObject<T>(string name, Transform parent, Vector2 anchoredPos, Vector2 size, Vector3 scale, Quaternion localRot) where T : Component
        {
            GameObject go = new GameObject(name);
            RectTransform rt = go.AddComponent<RectTransform>();
            rt.SetParent(parent);
            rt.localPosition = Vector3.zero;
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = size;
            rt.localScale = scale;
            rt.localRotation = localRot;
            T component = go.AddComponent<T>();
            return component;
        }


        public static void SetRed(this Graphic widget, float r)
        {
            Color color = widget.color;
            color.r = r;
            widget.color = color;
            widget.SetVerticesDirty();
        }
        public static void SetGreen(this Graphic widget, float g)
        {
            Color color = widget.color;
            color.g = g;
            widget.color = color;
            widget.SetVerticesDirty();
        }
        public static void SetBlue(this Graphic widget, float b)
        {
            Color color = widget.color;
            color.b = b;
            widget.color = color;
            widget.SetVerticesDirty();
        }
        public static void SetAlpha(this Graphic widget, float a)
        {
            Color color = widget.color;
            color.a = a;
            widget.color = color;
            widget.SetVerticesDirty();
        }

        public static void SetRedGreen(this Graphic widget, float r, float g)
        {
            Color color = widget.color;
            color.r = r;
            color.g = g;
            widget.color = color;
            widget.SetVerticesDirty();
        }
        public static void SetRedBlue(this Graphic widget, float r, float b)
        {
            Color color = widget.color;
            color.r = r;
            color.b = b;
            widget.color = color;
            widget.SetVerticesDirty();
        }
        public static void SetRedAlpha(this Graphic widget, float r, float a)
        {
            Color color = widget.color;
            color.r = r;
            color.a = a;
            widget.color = color;
            widget.SetVerticesDirty();
        }

        public static void SetRGB(this Graphic widget, float r, float g, float b)
        {
            widget.color = new Color(r, g, b);
            widget.SetVerticesDirty();
        }
        public static void SetRGA(this Graphic widget, float r, float g, float a)
        {
            Color color = widget.color;
            color.r = r;
            color.g = g;
            color.a = a;
            widget.color = color;
            widget.SetVerticesDirty();
        }

        public static void SetRGBA(this Graphic widget, float r, float g, float b, float a)
        {
            widget.color = new Color(r, g, b, a);
            widget.SetVerticesDirty();
        }

        public static string ToRichTextHexColorCode(this Color color)
        {
            return "#" + ColorUtility.ToHtmlStringRGB(color);
        }

        public static void SetSprite(this RawImage imgRaw, SpriteCache sc)
        {
            if (!imgRaw)
                return;

            if (!sc)
                return;

            UIExtension.OnSetSprite(imgRaw, sc);
        }

        static void OnSetSprite(RawImage imgRaw, SpriteCache sc)
        {
            var texture = sc.Texture;
            if (imgRaw.texture != texture)
                imgRaw.texture = texture;

            imgRaw.uvRect = sc.UVRect;
        }

        public static bool TrySetSprite(this Graphic graphic, SpriteCache sc)
        {
            if (!sc)
                return false;

            if (graphic is Image)
            {
                var img = (Image)graphic;
                if (img.sprite != sc)
                    img.sprite = sc;
                return true;
            }
            else if (graphic is RawImage)
            {
                var imgRaw = (RawImage)graphic;
                UIExtension.OnSetSprite(imgRaw, sc);
                return true;
            }

            return false;
        }

        public static void SetSpriteWithOffset(this RawImage imgRaw, SpriteCache sc, Rect offset)
        {
            if (!imgRaw)
                return;

            if (!sc)
                return;

            UIExtension.OnSetSpriteWithOffset(imgRaw, sc, offset);
        }

        static void OnSetSpriteWithOffset(RawImage imgRaw, SpriteCache sc, Rect offset)
        {
            var texture = sc.Texture;
            if (imgRaw.texture != texture)
                imgRaw.texture = texture;

            imgRaw.uvRect = sc.UVRect;

            if (texture)
            {
                var w = texture.width;
                var h = texture.height;
                offset.xMin /= w;
                offset.yMin /= h;
                offset.xMax /= w;
                offset.yMax /= h;
            }

            var uvRect = sc.UVRect;
            uvRect.min += offset.min;
            uvRect.size -= offset.max;
            imgRaw.uvRect = uvRect;
        }

        public static bool TrySetUV(this Graphic graphic, Rect uv)
        {
            if (graphic is Image)
            {
                Debug.LogWarning(string.Format("The sprite shaders don't support it.\ngraphic:{0}", graphic));
            }
            else if (graphic is RawImage)
            {
                RawImage imgRaw = (RawImage)graphic;
                imgRaw.uvRect = uv;
                return true;
            }

            return false;
        }

        public static object GetData(this Graphic thiz)
        {
            if (thiz)
            {
                if (thiz is Image)
                {
                    return ((Image)thiz).sprite;
                }
                else if (thiz is RawImage)
                {
                    return ((RawImage)thiz).texture;
                }
                else if (thiz is Text)
                {
                    return ((Text)thiz).text;
                }
            }

            return null;
        }

        public static bool SetData(this Graphic thiz, object data)
        {
            if (thiz)
            {
                if (thiz is Image)
                {
                    var target = ((Image)thiz);
                    target.sprite = data is Sprite ? (Sprite)data : null;
                    return true;
                }
                else if (thiz is RawImage)
                {
                    var target = ((RawImage)thiz);
                    target.texture = data is Texture ? (Texture)data : null;
                    return true;
                }
                else if (thiz is Text)
                {
                    var target = ((Text)thiz);
                    target.text = data is string ? (string)data : "";
                    return true;
                }
            }

            return false;
        }

        public static bool IsValid(this Graphic thiz)
        {
            if (thiz)
            {
                if (thiz is Image)
                {
                    return ((Image)thiz).sprite;
                }
                else if (thiz is RawImage)
                {
                    return ((RawImage)thiz).texture;
                }
                else if (thiz is Text)
                {
                    return !string.IsNullOrEmpty(((Text)thiz).text);
                }
            }

            return false;
        }
    }
}