using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
namespace Ext.Unity3D.UI
{
    [ExecuteInEditMode]
    public class SpritesComponent : SequentialUIComponent
    {
        [SerializeField]
        protected SpriteCache[] sprites;
        public IEnumerable<SpriteCache> Elements
        {
            get { return this.sprites; }
        }

        public SpriteCache this[int index]
        {
            get
            {
                if (null != this.sprites && 0 <= index && index < this.sprites.Length)
                    return this.sprites[index];

                return null;
            }
        }

        public bool IsValid { get { return null != this.sprites ? 0 < this.sprites.Length : false; } }


        public int Count
        {
            get
            {
                return null != this.sprites ? this.sprites.Length : 0;
            }
        }

        public int Last
        {
            get
            {
                return null != this.sprites && 0 < this.sprites.Length ? this.sprites.Length - 1 : 0;
            }
        }



        public void Rebuild(SpriteCache[] newArray)
        {
            if (newArray == this.sprites)
                return;

            this.cachedRuntimeDatas.Clear();

            var oldArray = this.sprites;
            this.sprites = newArray;
            this.RebuildCaches();

            if (null != oldArray)
            {
                for (int n = 0, cnt = oldArray.Length; n < cnt; ++n)
                {
                    var sc = oldArray[n];
                    if (sc)
                        SpriteRuntimeData.Shared.Unregist(sc);
                }
            }
        }


        event Action<bool> onRebuildCaches;
        public event Action<bool> RebuildCacheListener
        {
            add { this.onRebuildCaches += value; }
            remove { this.onRebuildCaches -= value; }
        }

        void RebuildCaches()
        {
            this.cachedRuntimeDatas.Clear();

            if (null != this.sprites)
            {
                for (int n = 0, cnt = this.sprites.Length; n < cnt; ++n)
                {
                    var sc = this.sprites[n];
                    this.cachedRuntimeDatas.Add(sc ? SpriteRuntimeData.Shared.Regist(sc) : null);
                }
            }

            if (null != this.onRebuildCaches)
                this.onRebuildCaches(true);
        }
        

        List<SpriteRuntimeData> cachedRuntimeDatas = new List<SpriteRuntimeData>();
        public SpriteRuntimeData GetCachedRuntimeData(int index)
        {
            if (-1 < index && index < this.Count)
            {
                var runtime = index < this.cachedRuntimeDatas.Count ? this.cachedRuntimeDatas[index] : null;
#if UNITY_EDITOR
                if (null == runtime)
                {
                    var sc = this.sprites[index];
                    if (sc)
                    {
                        this.editorDirtyRuntimeCaches = true;
#if LOG_DEBUG
                        Debug.Log(string.Format("SPRITES:MODIFIED_SPRITE_INDEX:{0}, SPRITE:{1}", index, sc));
#endif// LOG_DEBUG
                    }
                }
#endif// UNITY_EDITOR

                        return runtime;
            }

            return null;
        }


        protected virtual void Awake()
        {
            this.RebuildCaches();
        }

        protected virtual void OnDestroy()
        {
            this.cachedRuntimeDatas.Clear();

            if (null != this.sprites)
            {
                for (int n = 0, cnt = this.sprites.Length; n < cnt; ++n)
                {
                    var sc = this.sprites[n];
                    if (sc)
                        SpriteRuntimeData.Shared.Unregist(sc);
                }
            }

            if (null != this.onRebuildCaches)
                this.onRebuildCaches(false);
        }



#if UNITY_EDITOR
        void EditorResetRuntimeCaches()
        {
            this.cachedRuntimeDatas.Clear();

            if (null != this.sprites)
            {
                for (int n = 0, cnt = this.sprites.Length; n < cnt; ++n)
                {
                    var sc = this.sprites[n];
                    if (sc)
                    {
                        SpriteRuntimeData.Shared.Unregist(sc);
                        this.cachedRuntimeDatas.Add(SpriteRuntimeData.Shared.Regist(sc));
                    }
                    else
                        this.cachedRuntimeDatas.Add(null);
                }
            }

            if (null != this.onRebuildCaches)
                this.onRebuildCaches(true);
        }

        protected virtual void OnEnable()
        {
            if (!UnityEditor.EditorApplication.isPlaying)
                this.EditorResetRuntimeCaches();
        }

        bool editorDirtyRuntimeCaches = false;
#endif// UNITY_EDITOR
        protected virtual void Update()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                if (this.editorDirtyRuntimeCaches)
                {
                    this.editorDirtyRuntimeCaches = false;
                    this.EditorResetRuntimeCaches();
                }
            }
#endif// UNITY_EDITOR
        }


        public virtual bool UsePixelOffset { get { return false; } }

        public virtual Rect GetPixelOffset(int index) { return new Rect(0, 0, 0, 0); }


        public void ResetPixelOffset()
        {
            if (null == this.sprites)
                return;

            for (int n = 0; n < this.sprites.Length; ++n)
            {
                SpriteCache sc = this.sprites[n];
                if (sc && sc)
                {
                    if (this.UsePixelOffset)
                        sc.Assign(sc, this.GetPixelOffset(n));
                    else
                        sc.Assign(sc);
                }
            }
        }

        public int FindIndex(Sprite sprite)
        {
            if (sprite)
            {
                for (int n = 0, cnt = this.Count; n < cnt; ++n)
                {
                    var sc = this.sprites[n];
                    if (!sc)
                        continue;

                    if (sc == sprite)
                        return n;
                }
            }

            return -1;
        }

        public int FindIndex(SpriteCache spriteCache)
        {
            if (spriteCache)
            {
                for (int n = 0, cnt = this.Count; n < cnt; ++n)
                {
                    var sc = this.sprites[n];
                    if (!sc)
                        continue;

                    if (sc == spriteCache)
                        return n;
                }
            }

            return -1;
        }



#if UNITY_EDITOR
        [Serializable]
        protected class AutoSpriteSettingInfo : SequentialSettingBasicInfo
        {
            public override string GetTag() { return ""; }
            public override Type GetAssetType() { return typeof(Sprite); }
            public AutoSpriteSettingInfo()
            {
                this.IsUse = true;
                this.NameExt = ".png";
            }
        }
        protected override SequentialSettingInfo[] EditorGetSequentialSettings()
        {
            return new SequentialSettingBasicInfo[] { this.editorAutoSpriteSettings };
        }

        [SerializeField]
        protected AutoSpriteSettingInfo editorAutoSpriteSettings;

        [SerializeField]
        bool editorAddMode = false;

        [SerializeField]
        bool editorResetPixelOffset = false;

        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();

            if (this.editorResetPixelOffset)
                this.ResetPixelOffset();
        }

        protected override void OnEditorSetting_Sequential(string tag, UnityEngine.Object[] objects)
        {
            if (this.editorAddMode && null != this.sprites)
            {
                var old = this.sprites;
                var index = 0;
                this.sprites = new SpriteCache[old.Length + objects.Length];
                for (int n = 0; n < this.sprites.Length; ++n)
                {
                    if (n < old.Length)
                        this.sprites[n] = old[n];
                    else
                    {
                        if (this.UsePixelOffset)
                            this.sprites[n] = new SpriteCache((Sprite)objects[index], this.GetPixelOffset(index));
                        else
                            this.sprites[n] = new SpriteCache((Sprite)objects[index]);

                        ++index;
                    }
                }
            }
            else
            {
                this.sprites = new SpriteCache[objects.Length];
                for (int n = 0; n < this.sprites.Length; ++n)
                {
                    if (this.UsePixelOffset)
                        this.sprites[n] = new SpriteCache((Sprite)objects[n], this.GetPixelOffset(n));
                    else
                        this.sprites[n] = new SpriteCache((Sprite)objects[n]);
                }
            }
        }

        public void EditorSetSprites(SpriteCache[] scs)
        {
            this.sprites = scs;
        }

        protected override void OnEditorPostSetting()
        {
            base.OnEditorPostSetting();

            this.EditorResetRuntimeCaches();
        }
#endif// UNITY_EDITOR
    }
}