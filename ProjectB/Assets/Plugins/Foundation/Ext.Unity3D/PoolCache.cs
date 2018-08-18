using UnityEngine;
using UnityEngine.UI;
namespace Ext.Unity3D
{
    public class PoolCache : ManagedComponent
    {
        [SerializeField]
        int index = -1;
        public int Index { get { return this.index; } }


        static long uniqueKeyCounter = 0;
        long uniqueKey;
        public long UniqueKey { get { return this.uniqueKey; } }


        [SerializeField]
        Pool pool;
        public Pool Pool { get { return this.pool; } }
        internal void SetPool(Pool pool) { this.pool = pool; }

        bool use = false;
        public bool Use { get { return this.use; } }
        internal void InternalSetUse(bool value)
        {
            if (value == this.use)
                return;

            this.use = value;
            if (value)
                this.uniqueKey = ++PoolCache.uniqueKeyCounter;
        }
        


#if UNITY_EDITOR
        public void EditorSetIndex(int value)
        {
            this.index = value;
        }

        public void EditorSetPool(Pool pool)
        {
            this.pool = pool;
        }
#endif// UNITY_EDITOR

        public override string ToString()
        {
            return string.Format("{{Type:{0}, Index:{1}, UniqueKey:{2}, Used:{3}}}", this.GetType().Name, this.Index, this.UniqueKey, this.Use);
        }
    }
}