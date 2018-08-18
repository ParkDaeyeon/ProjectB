using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Ext.Unity3D.UI.Pool
{
    public class QuadPolyPool : GraphicPool, IEnumerable<QuadPolyCache>
    {
        public new QuadPolyCache this[int index] { get { return base[index] as QuadPolyCache; } }

        public new IEnumerator<QuadPolyCache> GetEnumerator()
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
