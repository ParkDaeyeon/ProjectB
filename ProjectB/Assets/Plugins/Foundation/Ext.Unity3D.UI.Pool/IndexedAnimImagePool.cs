using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Ext.Unity3D.UI.Pool
{
    public class IndexedAnimImagePool : ImagePool, IEnumerable<IndexedAnimImageCache>
    {
        public new IndexedAnimImageCache this[int index] { get { return base[index] as IndexedAnimImageCache; } }

        public new IEnumerator<IndexedAnimImageCache> GetEnumerator()
        {
            for (int n = 0, cnt = this.Count; n < cnt; ++n)
                yield return this[n];
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }



#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();
        }

        protected override void OnEditorBuildCache(PoolCache c_, Transform ct)
        {
            base.OnEditorBuildCache(c_, ct);
        }
#endif// UNITY_EDITOR
    }
}
