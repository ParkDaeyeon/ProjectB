using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Ext.Unity3D;

namespace Ext.Unity3D.UI
{
    [ExecuteInEditMode]
    public class OptimizedImage : UnityEngine.UI.Image
    {
        [SerializeField]
        bool resizeAutomatically = false;
        public bool ResizeAutomatically
        {
            set { this.resizeAutomatically = value; }
            get { return this.resizeAutomatically; }
        }

        [SerializeField]
        bool overrideRendering = true;
        public bool OverrideRendering
        {
            set
            {
                if (value == this.overrideRendering)
                    return;

                this.overrideRendering = value;
                this.SetVerticesDirty();
            }
            get { return this.overrideRendering; }
        }

        public void ResetSprite()
        {
            this.SetSprite(this.overrideSprite);
        }

        void SetSprite(Sprite value)
        {
#if UNITY_EDITOR
            this.editorSpriteChanged = false;
#endif// UNITY_EDITOR

            var prev = this.selectedSprite;
            this.selectedSprite = value;

            if (this.Cacheable)
                this.UpdateCache(prev);

            if (this.resizeAutomatically)
            {
                if (value && value != prev)
                {
                    var rectTransform = this.rectTransform;
                    if (rectTransform)
                    {
                        var pixelsPerUnit = this.pixelsPerUnit;
                        float w = value.rect.width / pixelsPerUnit;
                        float h = value.rect.height / pixelsPerUnit;
                        rectTransform.anchorMax = rectTransform.anchorMin;
                        rectTransform.sizeDelta = new Vector2(w, h);
                    }
                }
            }

            if (this.overrideRendering)
                this.OnSpriteChanged(prev);
        }
        protected virtual void OnSpriteChanged(Sprite prev) { }


        Sprite selectedSprite;
        protected Sprite SelectedSprite
        {
            get { return this.selectedSprite; }
        }

        public virtual bool Cacheable
        {
            get { return this.overrideRendering; }
        }

        [SerializeField]
        SpritesComponent caches;
        public SpritesComponent Caches
        {
            set
            {
                if (value == this.caches)
                    return;

                if (this.caches)
                    this.caches.RebuildCacheListener -= this.OnRebuildCache;

                this.caches = value;

                if (this.caches)
                    this.caches.RebuildCacheListener += this.OnRebuildCache;

                this.ResetSprite();
            }
            get { return this.caches; }
        }

        void OnRebuildCache(bool available)
        {
            this.ResetSprite();
        }

        int cacheIndex = -1;
        SpriteRuntimeData runtimeData;
        protected virtual int UpdateCache(Sprite prev)
        {
            var prevCached = this.Cached;

            var ret = 0;
            if (this.selectedSprite)
            {
                if (-1 != (this.cacheIndex = this.FindCacheIndex(this.selectedSprite)))
                {
                    this.runtimeData = this.caches.GetCachedRuntimeData(this.cacheIndex);
                    ret = 1;
                }
                else
                {
                    this.runtimeData = SpriteRuntimeData.Shared.Regist(this.selectedSprite);
                    ret = 2;
                }
            }
            else
            {
                this.cacheIndex = -1;
                this.runtimeData = null;
            }

            if (!prevCached)
                SpriteRuntimeData.Shared.Unregist(prev);

            return ret;
        }
        
        public virtual bool Cached
        {
            get { return -1 != this.cacheIndex; }
        }

        public virtual int CacheCount
        {
            get { return this.caches ? this.caches.Count : 0; }
        }

        public virtual SpriteCache GetCache(int index)
        {
            return this.caches ? this.caches[index] : null;
        }

        public virtual int FindCacheIndex(Sprite sprite)
        {
            return this.caches ? this.caches.FindIndex(sprite) : -1;
        }


        bool awaked = false;
        protected override void Awake()
        {
            base.Awake();

            if (this.caches)
                this.caches.RebuildCacheListener += this.OnRebuildCache;
            this.awaked = true;
        }
        protected override void OnDestroy()
        {
            if (this.caches)
                this.caches.RebuildCacheListener -= this.OnRebuildCache;

            this.SetSprite(null);
            base.OnDestroy();
        }

#if UNITY_EDITOR
        bool editorValidating;
        bool editorSpriteChanged;
        protected override void OnValidate()
        {
            this.editorValidating = true;
            base.OnValidate();
            this.editorValidating = false;
        }
#endif// UNITY_EDITOR
        
        public override void SetAllDirty()
        {
#if UNITY_EDITOR
            if (this.editorValidating)
            {
                var sprite = this.overrideSprite;
                if (this.selectedSprite != sprite)
                    this.editorSpriteChanged = true;
                base.SetAllDirty();
                return;
            }
#endif// UNITY_EDITOR
            if (this.awaked)
            {
                var sprite = this.overrideSprite;
                if (this.selectedSprite != sprite)
                    this.SetSprite(sprite);
            }

            base.SetAllDirty();
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            if (!this.overrideRendering)
            {
                base.OnPopulateMesh(toFill);
                return;
            }
            
            var rectTrans = this.rectTransform;
            this.translator.Update(rectTrans, sprite);

            this.OnPopulateMeshMainProcess(this.translator, rectTrans, sprite, toFill);
        }

        protected virtual void OnPopulateMeshMainProcess(Translator translator,
                                                         RectTransform rectTrans,
                                                         Sprite sprite,
                                                         VertexHelper toFill)
        {
            if (!sprite || Type.Simple != this.type)
            {
                base.OnPopulateMesh(toFill);
                return;
            }

            var baseColor = this.color;
            toFill.Clear();

            if (null != this.runtimeData)
            {
                var vertices = this.runtimeData.Vertices;
                var uv = this.runtimeData.UVs;
                for (int n = 0, cnt = vertices.Length; n < cnt; ++n)
                    toFill.AddVert(translator.Translate(vertices[n]), baseColor, uv[n]);

                var tris = this.runtimeData.Triangles;
                var ti = 0;
                var triLen = tris.Length;
                for (int n = 0; n < triLen; n += 3)
                    toFill.AddTriangle((int)tris[ti++],
                                       ti < triLen ? (int)tris[ti++] : -1,
                                       ti < triLen ? (int)tris[ti++] : -1);
            }
        }
        


        public class Translator
        {
            public void Update(RectTransform rectTrans,
                               Sprite sprite)
            {
                var size = rectTrans.rect.size;
                var sizeHalf = size * 0.5f;

                var pivot = rectTrans.pivot;
                var sprCenter = sprite ? sprite.bounds.center * sprite.pixelsPerUnit : Vector3.zero;

                var center = new Vector2(-(pivot.x - 0.5f) * (size.x + sprCenter.x),
                                         -(pivot.y - 0.5f) * (size.y + sprCenter.y));

                this.min.x = -sizeHalf.x + center.x;
                this.min.y = -sizeHalf.y + center.y;

                this.max.x = +sizeHalf.x + center.x;
                this.max.y = +sizeHalf.y + center.y;

                if (sprite)
                {
                    var sprSize = sprite.bounds.size;
                    this.scale = new Vector2(1 / sprSize.x, 1 / sprSize.y);
                }
                else
                    this.scale = Vector2.one;
            }

            public Vector2 min;
            public Vector2 max;
            public Vector2 scale;
            
            public Vector2 Translate(Vector2 position)
            {
                return new Vector2(Mathf.Lerp(this.min.x, this.max.x, position.x * this.scale.x + 0.5f),
                                   Mathf.Lerp(this.min.y, this.max.y, position.y * this.scale.y + 0.5f));
            }
        }
        Translator translator = new Translator();
        public Translator GetTranslator()
        {
            return this.translator;
        }

        protected virtual void Update()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
                this.ResetSprite();
            else if (this.editorSpriteChanged)
                this.ResetSprite();
#endif// UNITY_EDITOR
        }


#if UNITY_EDITOR
        protected override void OnEnable()
        {
            base.OnEnable();

            if (!UnityEditor.EditorApplication.isPlaying)
            {
                if (this.caches)
                    this.caches.RebuildCacheListener += this.OnRebuildCache;

                this.ResetSprite();
            }
        }
        protected override void OnDisable()
        {
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                if (this.caches)
                    this.caches.RebuildCacheListener -= this.OnRebuildCache;
            }

            base.OnDisable();
        }

        public void EditorCheckCachable()
        {
            if (!this.Cacheable)
                this.caches = null;
        }

        public bool EditorCheckCached()
        {
            if (!this.gameObject.activeInHierarchy)
            {
                var sprite = this.overrideSprite;
                if (this.selectedSprite != sprite)
                    this.SetSprite(sprite);
            }

            return this.Cached;
        }
#endif// UNITY_EDITOR
    }
}
