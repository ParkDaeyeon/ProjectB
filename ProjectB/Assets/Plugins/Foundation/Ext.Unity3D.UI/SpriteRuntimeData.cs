using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
namespace Ext.Unity3D.UI
{
    public class SpriteRuntimeData
    {
        protected SpriteRuntimeData(Sprite sprite)
        {
            this.Update(sprite);
        }

        public void Update(Sprite sprite)
        {
            if (sprite)
            {
                this.triangles = sprite.triangles;
                this.uv = sprite.uv;
                this.vertices = sprite.vertices;
            }
            else
            {
                this.triangles = SpriteRuntimeData.EMPTY_TRIANGLES;
                this.uv = SpriteRuntimeData.EMPTY_UVS;
                this.vertices = SpriteRuntimeData.EMPTY_VERTICES;
            }
        }

        public static readonly ushort[] EMPTY_TRIANGLES = new ushort[0];
        public static readonly Vector2[] EMPTY_UVS = new Vector2[0];
        public static readonly Vector2[] EMPTY_VERTICES = new Vector2[0];


        int refCount = 1;
        public int RefCount
        {
            get { return this.refCount; }
        }

#if UNITY_EDITOR
        public bool IsValidReference
        {
            get { return null != this.triangles; }
        }
#endif// UNITY_EDITOR

        ushort[] triangles;
        public ushort[] Triangles
        {
            get { return this.triangles; }
        }

        Vector2[] uv;
        public Vector2[] UVs
        {
            get { return this.uv; }
        }

        Vector2[] vertices;
        public Vector2[] Vertices
        {
            get { return this.vertices; }
        }
        

        public static class Shared
        {
            public static SpriteRuntimeData Regist(Sprite sprite)
            {
                if (!sprite)
                    return null;

                SpriteRuntimeData data;
                if (Shared.map.TryGetValue(sprite, out data))
                    ++data.refCount;
                else
                    Shared.map.Add(sprite, data = new SpriteRuntimeData(sprite));

                return data;
            }

            public static bool Unregist(Sprite sprite)
            {
                if (!sprite)
                    return true;

                SpriteRuntimeData data;
                if (Shared.map.TryGetValue(sprite, out data))
                {
                    --data.refCount;
                    if (0 < data.refCount)
                        return false;

                    Shared.map.Remove(sprite);
                }

                return true;
            }

            public static void Clear()
            {
                Shared.map.Clear();
            }

            public static void UpdateAll()
            {
                var enumerator = Shared.map.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var pair = enumerator.Current;
                    var sprite = pair.Key;
                    var data = pair.Value;
                    data.Update(sprite);
                }
            }

            static Dictionary<Sprite, SpriteRuntimeData> map = new Dictionary<Sprite, SpriteRuntimeData>();
        }
    }
}
