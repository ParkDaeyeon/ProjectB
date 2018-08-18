using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Ext.Collection.AntiGC;
namespace Ext.Unity3D
{
    public class Pool : ManagedComponent, IEnumerable<PoolCache>
    {
        [SerializeField]
        Transform cacheBase;
        public Transform CacheBase { get { return this.cacheBase; } }


        [SerializeField]
        PoolCache[] caches;
        public int Count { get { return null != this.caches ? this.caches.Length : 0; } }
        public PoolCache this[int index]
        {
            get
            {
                if (index < 0 || this.Count <= index)
                    return null;

                return this.caches[index];
            }
        }
        public IEnumerator<PoolCache> GetEnumerator()
        {
            for (int n = 0, cnt = this.Count; n < cnt; ++n)
                yield return this.caches[n];
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }


        int nextIndexCounter = 0;
        public int NextIndex
        {
            get
            {
                for (int n = 0, cnt = this.Count; n < cnt; ++n)
                {
                    if (this.nextIndexCounter >= cnt)
                        this.nextIndexCounter = 0;

                    var c = this.caches[this.nextIndexCounter];
                    if (!c.Use)
                        return this.nextIndexCounter;

                    ++this.nextIndexCounter;
                }

                //Debug.LogWarning("GRAHPIC_POOL_WARN:TOO_MANY_USING_CACHE:" + this.GetType().Name + ", POOL_COUNT:" + this.Count);
                return -1;
            }
        }
        public int NextIndexForce
        {
            get
            {
                ++this.nextIndexCounter;
                if (this.nextIndexCounter >= this.Count)
                    this.nextIndexCounter = 0;

                return this.nextIndexCounter;
            }
        }

        public int RemainCount { get { return this.Count - this.VisibledCount; } }


        CachedList<PoolCache> visibledList;
		public LinkedListNode<PoolCache> VisibledFirstNode { get { return null != this.visibledList ? this.visibledList.First : null; } }
		public int VisibledCount { get { return null != this.visibledList ? this.visibledList.Count : 0; } }





        void Awake()
		{
            this.Setup();
        }
        
        public void Setup()
        {
            if (null != this.visibledList)
                return;
            
            this.visibledList = new CachedList<PoolCache>(this.Count);
            for (int n = 0, cnt = this.Count; n < cnt; ++n)
                this.HideCache(n);

            this.OnSetup();
        }

        protected virtual void OnSetup() { }


        void OnDestroy()
        {
            this.caches = null;
            this.visibledList = null;
        }



        public PoolCache ShowCache(bool force = false)
        {
            var idx = force ? this.NextIndexForce : this.NextIndex;
            if (-1 == idx)
            {
#if UNITY_EDITOR || LOG_DEBUG || TEST || CHEAT
                Debug.LogWarning("POOL:OVER:" + this.GetType().Name);
#endif// UNITY_EDITOR || LOG_DEBUG || TEST || CHEAT
                return null;
            }
            return this.ShowCache(idx);
        }

        public PoolCache ShowCache(int index)
        {
            var c = this[index];
            if (null == c)
                return null;

            c.SetActive(true);

            if (!c.Use)
            {
                c.InternalSetUse(true);
                if (null != this.visibledList)
                    this.visibledList.AddLast(c);
            }
            return c;
        }

        public bool HideCache(int index)
        {
            var c = this[index];
            if (null == c)
                return false;

            c.SetActive(false);

            if (!c.Use)
                return false;

            c.InternalSetUse(false);
            if (null != this.visibledList)
                this.visibledList.Remove(c);
            return true;
        }

        public void HideAll()
        {
            for (int n = 0, cnt = this.Count; n < cnt; ++n)
            {
                var c = this.caches[n];
                if (c.Use)
                    this.HideCache(c.Index);
            }

            this.nextIndexCounter = 0;
        }


#if UNITY_EDITOR
        [SerializeField]
        int editorCacheCount;
        [SerializeField]
        PoolCache editorCachePrefab;
        [SerializeField]
        int editorZeroFillCount = 3;
        [SerializeField]
        bool editorRecreate = true;

        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();

            if (this.editorRecreate && this.editorCachePrefab)
            {
                if (this.cacheBase)
                {
                    this.cacheBase.SetParent(null);
                    GameObject.DestroyImmediate(this.cacheBase.gameObject);
                }

                this.cacheBase = this.OnEditorCreateCachebase();

                var format = 0 < this.editorZeroFillCount ? string.Format("{{0:D{0}}}", this.editorZeroFillCount) : "{{0}}";

                this.caches = new PoolCache[Mathf.Max(this.editorCacheCount, 0)];

                var tOrigin = this.editorCachePrefab.CachedTransform;
                for (int n = 0, cnt = this.caches.Length; n < cnt; ++n)
                {
                    var go = GameObject.Instantiate(this.editorCachePrefab.gameObject);
                    var t = go.transform;
                    t.SetParent(this.cacheBase);
                    t.localPosition = Vector3.zero;
                    t.localScale = tOrigin.localScale;
                    t.localRotation = tOrigin.localRotation;
                }

                int idx = 0;
                foreach (Transform t in this.cacheBase)
                {
                    var c = t.GetComponent<PoolCache>();
                    c.name = string.Format(format, idx);
                    c.EditorSetIndex(idx);
                    c.EditorSetPool(this);
                    this.caches[idx++] = c;

                    this.OnEditorBuildCache(c, c.CachedTransform);
                    c.SetPool(this);
                    c.EditorSetting();
                    c.SetActive(false);
                }
            }
        }

        protected virtual Transform OnEditorCreateCachebase()
        {
            Transform t = new GameObject("CacheBase").transform;
            t.SetParent(this.CachedTransform);
            t.localPosition = Vector3.zero;
            t.localScale = Vector3.one;
            t.localRotation = Quaternion.identity;
            return t;
        }

        protected virtual void OnEditorBuildCache(PoolCache c, Transform ct) { }
#endif// UNITY_EDITOR

        public override string ToString()
        {
            return string.Format("{{Type:{0}, Name:{1}, Count:{2}, Remain:{3}}}", this.GetType().Name, this.name, this.Count, this.RemainCount);
        }
    }
}
