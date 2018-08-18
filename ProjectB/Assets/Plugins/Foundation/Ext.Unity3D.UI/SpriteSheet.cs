using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
namespace Ext.Unity3D.UI
{
    public class SpriteSheet : ScriptableObject
    {
        [SerializeField]
        protected List<Sprite> sprites = new List<Sprite>();
        public List<Sprite> Sprites { get { return this.sprites; } }
        
        protected Dictionary<string, Sprite> spriteMap = new Dictionary<string, Sprite>();
        public Dictionary<string, Sprite> SpriteMap { get { return this.spriteMap; } }

        //bool initialized = false;
        //void OnEnable()
        //{
        //    if (!this.initialized)
        //    {
        //        this.initialized = true;
        //        this.Update();
        //    }
        //}

        public bool ForceUpdateAll()
        {
            bool isRemoved = this.RemoveMissingSprites();
            isRemoved |= this.ResetMap(true);
            return isRemoved;
        }


        public List<int> GetMissingSpriteIndexes()
        {
            List<int> indexes = null;
            for (int n = 0, count = this.sprites.Count; n < count; ++n)
            {
                if (!this.sprites[n])
                {
                    if (null == indexes)
                        indexes = new List<int>();

                    indexes.Add(n);
                }
            }

            return indexes;
        }


        public List<Sprite> IndexesToSprites(List<int> indexes)
        {
            List<Sprite> sprites = new List<Sprite>();

            if (null != indexes)
            {
                for (int n = 0, count = indexes.Count; n < count; ++n)
                    sprites.Add(this.sprites[indexes[n]]);
            }

            return sprites;
        }


        public bool RemoveMissingSprites()
        {
            List<int> indexes = this.GetMissingSpriteIndexes();
            if (null != indexes)
            {
                for (int n = indexes.Count - 1; n >= 0; --n)
                    this.sprites.RemoveAt(indexes[n]);

                return true;
            }

            return false;
        }


        public bool ResetMap(bool removeExceptSprites)
        {
            List<Sprite> exceptSprites = null;
            this.spriteMap.Clear();
            foreach (Sprite s in this.sprites)
            {
                if (!s)
                    continue;

                try
                {
                    this.spriteMap.Add(s.name, s);
                }
                catch (ArgumentException e)
                {
                    Debug.LogWarning(string.Format("SPRITE_SHEET_UPDATE_FAILED:DUPLICATED_KEY:{0}, EXCEPT:\n{1}", s.name, e));

                    if (removeExceptSprites)
                    {
                        if (null == exceptSprites)
                            exceptSprites = new List<Sprite>();

                        exceptSprites.Add(s);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning(string.Format("SPRITE_SHEET_UPDATE_FAILED:KEY:{0}, EXCEPT:\n{1}", s.name, e));

                    if (removeExceptSprites)
                    {
                        if (null == exceptSprites)
                            exceptSprites = new List<Sprite>();

                        exceptSprites.Add(s);
                    }
                }
            }

            if (null != exceptSprites)
            {
                for (int n = 0, count = exceptSprites.Count; n < count; ++n)
                    this.sprites.Remove(exceptSprites[n]);

                return true;
            }

            return false;
        }
    }
}